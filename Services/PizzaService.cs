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
	public class PizzaService : IPizzaService
	{
		private readonly IUnitOfWork _uow;
		private readonly IPizzaRepository<MenuItem> _menuItems;
		private readonly IPizzaRepository<Order> _orders;
		private readonly IPizzaRepository<OrderLink> _orderLinks;
		private IMemoryCache _cache;		
		public PizzaService(IUnitOfWork uow, IMemoryCache memoryCache)
		{
			_uow = uow;
			_menuItems = _uow.GetRepository<MenuItem>();
			_orders = _uow.GetRepository<Order>();
			_orderLinks = _uow.GetRepository<OrderLink>();
			_cache = memoryCache;
		}
		///<summary>
		///returns a list of all items in _menuItems in the database
		///</summary>
		public IEnumerable<MenuItemDTO> GetMenu()
		{
			IEnumerable<MenuItemDTO> menuItem;
			//Checks if the cache for all menu items is empty
			if(!_cache.TryGetValue("MenuItem", out menuItem))
			{
				//if empty, a new list is gotten
				menuItem = (from i in _menuItems.All()
							where !i.isDeleted
							select new MenuItemDTO
							{
								ID = i.ID,
								Name = i.Name,
								Price = i.Price
							}).ToList();
				// if the list is empty, throw exception
				if(menuItem == null)
				{
					throw new NoItemsInListException();
				}
				//a time is set for how long the list will exist in the cache
				var cacheEntryOptions = new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromHours(8));
				//the new list is cached in MenuITem with how long it will exist
				_cache.Set("MenuItem", menuItem, cacheEntryOptions);
			}
			//return the list with all items
			return menuItem;
		}
		public MenuItemDTO SingleMenuItem(int menuItemID)
		{
			MenuItemDTO menuItem;
			//if(!_cache.TryGetValue("MenuItem", out menuItem))
			//{
				menuItem = (from i in _menuItems.All()
							where i.ID == menuItemID &&
									!i.isDeleted
							select new MenuItemDTO
							{
								ID = i.ID,
								Name = i.Name,
								Price = i.Price
							}).SingleOrDefault();
				//var cacheEntryOptions = new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromHours(8));
				//_cache.Set("MenuItem", menuItem, cacheEntryOptions);
			//}
			
			if(menuItem == null)
			{
				throw new ItemNotFoundException();
			}
			return menuItem;
		}


		public MenuItemDTO AddItemToMenu(MenuItemViewModel newItem)
		{
			// a new menu item is created from information given by the view model
			var item = new MenuItem
			{ 
				Name = newItem.Name, 
				SpicyLevel = newItem.SpicyLevel, 
				Description = newItem.Description,
				Price = newItem.Price			
			};
			// the new item is added to the _menuItems table in the database
			_menuItems.Add(item);
			//changes are saved in the database
			_uow.Save();
			//the previously stored cache is removed
			_cache.Remove("MenuItem");
			
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
			var item = (from i in _menuItems.All()
						where i.ID == menuItemID
						select i).SingleOrDefault();

			if (item == null)
			{
				throw new ItemNotFoundException();
			}

			item.isDeleted = true;
			_uow.Save();

			// TODO: empty the cache for the menu
		}

		/// <summary>
		/// Used to add an order to the database
		/// </summary>
		/// <param name="orderViewModel">View Model with the order to add</param>
		public void AddOrder(OrderViewModel orderViewModel)
		{
			//TODO: Maybe do more testing on the viewModel


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
			var orderID = order.ID; // This seams to work to get the ID of the order

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
		}


		public List<OrderDTO> GetOrders()
		{
			List<OrderDTO> orders;
			
			orders = (from i in _orders.All()
					select new OrderDTO
					{
						ID = i.ID,
						DateOfOrder = i.DateOfOrder,
						CustomerName = i.CustomerName,
						IsPickup = i.isPickup,
						Address = i.Address
						//Vantar OrderedItems
					}).ToList();

			return orders;
		}

		public OrderDTO GetOrderByID(int orderID)
		{
			var item = (from i in _orders.All()
								where i.ID == orderID
								select new OrderDTO
								{
									ID = i.ID,
									DateOfOrder = i.DateOfOrder,
									CustomerName = i.CustomerName,
									IsPickup = i.isPickup,
									Address = i.Address
									//Vatnar OrderedItems
								}).SingleOrDefault();
			if(item == null)
			{
				throw new ItemNotFoundException();
			}
			return item;
		}
	}
}