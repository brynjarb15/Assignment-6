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
			///<summary>
			///Checks if the cache for all menu items is empty
			///</summary>
			if(!_cache.TryGetValue("MenuItem", out menuItem))
			{
				///<summary>
				///if empty, a new list is gotten
				///</summary>
				menuItem = (from i in _menuItems.All()
							where !i.isDeleted
							select new MenuItemDTO
							{
								ID = i.ID,
								Name = i.Name,
								Price = i.Price
							}).ToList();
				///<summary>
				/// if the list is empty, throw exception
				///</summary>
				if(menuItem == null)
				{
					throw new NoItemsInListException();
				}
				///<summary>
				///a time is set for how long the list will exist in the cache
				///</summary>
				var cacheEntryOptions = new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromHours(8));
				///<summary>
				///the new list is cached in MenuITem with how long it will exist
				///</summary>
				_cache.Set("MenuItem", menuItem, cacheEntryOptions);
			}
			///<summary>
			///return the list with all items
			///</summary>
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
			///<summary>
			/// a new menu item is created from information given by the view model
			///</summary>
			var item = new MenuItem
			{ 
				Name = newItem.Name, 
				SpicyLevel = newItem.SpicyLevel, 
				Description = newItem.Description,
				Price = newItem.Price			
			};
			///<summary>
			/// the new item is added to the _menuItems table in the database
			///</summary>
			_menuItems.Add(item);
			///<summary>
			///changes are saved in the database
			///<summary>
			_uow.Save();
			///<summary>
			///the previously stored cache is removed
			///</summary>
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
			///<summary>
			///Get the item from the db
			///<summary>
			var item = (from i in _menuItems.All()
						where i.ID == menuItemID &&
							  !i.isDeleted
						select i).SingleOrDefault();
			///<summary>
			///If it is not found thow exception
			///</summary>
			if (item == null)
			{
				throw new ItemNotFoundException();
			}
			///<summary>
			/// Mark it as deleted and save
			///</summary>
			item.isDeleted = true;
			_uow.Save();

			// TODO: empty the cache for the menu
		}

		/// <summary>
		/// Used to add an order to the database
		/// </summary>
		/// <param name="orderViewModel">View Model with the order to add</param>
		public OrderDTO AddOrder(OrderViewModel orderViewModel)
		{
			//TODO: Maybe do more testing on the viewModel

			///<summary>
			///Get the MenuItems out of the ViewModel
			///</summary>
			var items = orderViewModel.OrderItemsIds;
			///<summary>
			/// Check if the items are on the menu
			///</summary>
			for (int i = 0; i < items.Count(); i++)
			{
				///<summary>
				///Get the item
				///</summary>
				var item = (from mi in _menuItems.All()
							 where mi.ID == items[i] &&
								   !mi.isDeleted
							 select mi).SingleOrDefault();
				///<summary>
				///If the item is null quit
				///</summary>
				if (item == null)
				{
					throw new ItemNotOnMenuException();
				}
			}
			///<summary>
			///At this point we know that all the items are on the menu so we add the order.
			///</summary>
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
			var orderID = order.ID; ///<summary>This seams to work to get the ID of the order</summary>

			///<summary>
			///Make new OrderLink for each of the item in the order
			///</summary>
			for (int i = 0; i < items.Count(); i++)
			{
				///<summary>
				///Make new OrderLink
				///</summary>
				var orderLink = new OrderLink
				{
					OrderId = orderID,
					MenuItemId = items[i]
				};
				///<summary>
				///Add the link to the table
				///</summary>
				_orderLinks.Add(orderLink);
			}
			///<summary>
			///Save the database
			///</summary>
			_uow.Save();
			return GetOrderByID(orderID);
		}


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
													where ol.OrderId == o.ID
													join i in _menuItems.All() on ol.MenuItemId equals i.ID
													where !i.isDeleted
													select new MenuItemDTO
													{
														ID = i.ID,
														Name = i.Name,
														Price = i.Price
													}).ToList()
								}).SingleOrDefault();
			if(item == null)
			{
				throw new ItemNotFoundException();
			}
			return item;
		}

		public void DeleteOrder(int orderID)
		{
			var item = (from i in _orders.All()
						where i.ID == orderID && !i.isCancelled
						select i).SingleOrDefault();

			if(item == null)
			{
				throw new ItemNotFoundException();
			}

			item.isCancelled = true;
			_uow.Save();
		}
	}
}