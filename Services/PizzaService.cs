using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Caching.Memory;
using PizzaApi.Models;
using PizzaApi.Models.EntityModels;
using PizzaApi.Models.ViewModels;
using PizzaApi.Repositories;
using PizzaApi.Services.Exceptions;

namespace PizzaApi.Services
{

	/// <summary>
	/// The service layer of the API
	/// Here will the buisness logic be
	/// </summary>
	public class PizzaService : IPizzaService
	{
		private readonly IUnitOfWork _uow;
		private readonly IPizzaRepository<MenuItem> _menuItems;
		private readonly IPizzaRepository<Order> _orders;
		private readonly IPizzaRepository<OrderLink> _orderLinks;
		private IMemoryCache _cache;

		private readonly string CACHE_MENU_ITEMS = "MenuItem";
		public PizzaService(IUnitOfWork uow, IMemoryCache memoryCache)
		{
			_uow = uow;
			_menuItems = _uow.GetRepository<MenuItem>();
			_orders = _uow.GetRepository<Order>();
			_orderLinks = _uow.GetRepository<OrderLink>();
			_cache = memoryCache;
		}
		///<summary>
		///returns a list of all items in the menu
		///</summary>
		public IEnumerable<MenuItemDTO> GetMenu()
		{
			IEnumerable<MenuItemDTO> menuItem;
			// Checks if the cache for all menu items is empty
			if(!_cache.TryGetValue(CACHE_MENU_ITEMS, out menuItem))
			{
				// If empty, a new list is gotten
				menuItem = (from i in _menuItems.All()
							where !i.isDeleted
							select new MenuItemDTO
							{
								ID = i.ID,
								Name = i.Name,
								Price = i.Price
							}).ToList();
				// If the list is empty, throw exception
				if(menuItem == null)
				{
					throw new NoItemsInListException();
				}
				// A time is set for how long the list will exist in the cache
				var cacheEntryOptions = new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromHours(8));
				// The new list is cached in MenuITem with how long it will exist
				_cache.Set(CACHE_MENU_ITEMS, menuItem, cacheEntryOptions);
			}

			// Return the list with all items
			return menuItem;
		}

		/// <summary>
		/// Gets a single item from the menu and returns it
		/// </summary>
		/// <param name="menuItemID">Id of the item it returns</param>
		/// <returns>Single Menu Item</returns>
		public MenuItemDTO SingleMenuItem(int menuItemID)
		{
			// Checks if the list of menu items is in cache, if not, a new list is gotten and the cache is set with that list
			// The list is then stored in listOfAllItems
			var listOfAllItems = GetMenu();
			// The item with the given ID is found in listOfAllItems
			MenuItemDTO menuItem = (from i in listOfAllItems
									where i.ID == menuItemID
									select new MenuItemDTO
									{
										ID = i.ID,
										Name = i.Name,
										Price = i.Price
									}).SingleOrDefault();
			// If menuItem returns null, then the item with that ID does not exist and an exceptions is thrown
			if(menuItem == null)
			{
				throw new ItemNotOnMenuException();
			}
			// Returns the single item with the given ID
			return menuItem;
		}

		/// <summary>
		///Adds a single item to the Menu
		/// </summary>
		/// <param name="newItem"></param>
		/// <returns></returns>
		public MenuItemDTO AddItemToMenu(MenuItemViewModel newItem)
		{
			// A new menu item is created from information given by the view model
			var item = new MenuItem
			{
				Name = newItem.Name,
				SpicyLevel = newItem.SpicyLevel,
				Description = newItem.Description,
				Price = newItem.Price
			};
			// The new item is added to the _menuItems table in the database
			_menuItems.Add(item);
			// Changes are saved in the database
			_uow.Save();
			// The previously stored cache is removed
			_cache.Remove(CACHE_MENU_ITEMS);

			return new MenuItemDTO
			{
				ID = item.ID,
				Name = item.Name,
				Price = item.Price
			};
		}
		/// <summary>
		/// Deletes an item from the menu database and empties the cache
		/// </summary>
		/// <param name="menuItemID">ID of the item to delete</param>
		public void DeleteMenuItem(int menuItemID)
		{
			// Get the item from the db
			var item = (from i in _menuItems.All()
						where i.ID == menuItemID &&
							  !i.isDeleted
						select i).SingleOrDefault();
			// If it is not found thow exception
			if (item == null)
			{
				throw new ItemNotFoundException();
			}
			// Mark it as deleted and save
			item.isDeleted = true;
			_uow.Save();

			_cache.Remove(CACHE_MENU_ITEMS);
		}

		/// <summary>
		/// Used to add an order to the database
		/// </summary>
		/// <param name="orderViewModel">View Model with the order to add</param>
		public OrderDTO AddOrder(OrderViewModel orderViewModel)
		{
			// Get the MenuItems out of the ViewModel
			var items = orderViewModel.OrderItemsIds;
			// Check if the items are on the menu
			for (int i = 0; i < items.Count(); i++)
			{
				// Get the item
				var item = (from mi in _menuItems.All()
							 where mi.ID == items[i] &&
								   !mi.isDeleted
							 select mi).SingleOrDefault();
				// If the item is null quit
				if (item == null)
				{
					throw new ItemNotOnMenuException();
				}
			}
			// At this point we know that all the items are on the menu so we add the order.
			var order = new Order
			{
				DateOfOrder = orderViewModel.DateOfOrder,
				CustomerName = orderViewModel.CustomerName,
				isPickup = orderViewModel.IsPickup,
				Address = orderViewModel.Address,
				isCancelled = false
			};
			_orders.Add(order);
			_uow.Save();
			var orderID = order.ID; // Gets the id of the new order

			// Make new OrderLink for each of the item in the order
			for (int i = 0; i < items.Count(); i++)
			{
				// Make new OrderLink
				var orderLink = new OrderLink
				{
					OrderId = orderID,
					MenuItemId = items[i]
				};
				// Add the link to the table
				_orderLinks.Add(orderLink);
			}
			// Save the database
			_uow.Save();
			return GetOrderByID(orderID);
		}

		/// <summary>
		/// TODO: Fylla inní þetta
		/// </summary>
		/// <returns></returns>
		public List<OrderLiteDTO> GetOrders()
		{
			List<OrderLiteDTO> orders;

			orders = (from i in _orders.All()
					where !i.isCancelled
					select new OrderLiteDTO
					{
						ID = i.ID,
						DateOfOrder = i.DateOfOrder,
						CustomerName = i.CustomerName,
						IsPickup = i.isPickup,
						Address = i.Address
					}).ToList();

			return orders;
		}

		/// <summary>
		/// TODO: fylla inní þetta
		/// </summary>
		/// <param name="orderID"></param>
		/// <returns></returns>
		public OrderDTO GetOrderByID(int orderID)
		{
			var item = (from o in _orders.All()
								where o.ID == orderID && !o.isCancelled
								select new OrderDTO
								{
									ID = o.ID,
									DateOfOrder = o.DateOfOrder,
									CustomerName = o.CustomerName,
									IsPickup = o.isPickup,
									Address = o.Address,
									OrderedItems = (from ol in _orderLinks.All()
													join i in _menuItems.All() on ol.MenuItemId equals i.ID
													where !i.isDeleted && ol.OrderId == o.ID
													select new MenuItemDTO
													{
														ID = i.ID,
														Name = i.Name,
														Price = i.Price
													}).ToList()
								}).SingleOrDefault();
			if(item == null)
			{
				throw new OrderNotFoundException();
			}
			return item;
		}

		/// <summary>
		/// TODO: fylla inní þetta
		/// </summary>
		/// <param name="orderID"></param>
		public void DeleteOrder(int orderID)
		{
			var item = (from i in _orders.All()
						where i.ID == orderID && !i.isCancelled
						select i).SingleOrDefault();

			if(item == null)
			{
				throw new OrderNotFoundException();
			}

			item.isCancelled = true;
			_uow.Save();
		}
	}
}