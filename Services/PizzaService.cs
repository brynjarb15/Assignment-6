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

		public List<MenuItemDTO> GetMenu()
		{
			List<MenuItemDTO> menuItem;
			if(!_cache.TryGetValue("MenuItem", out menuItem))
			{
				menuItem = (from i in _menuItems.All()
							where !i.isDeleted
							select new MenuItemDTO
							{
								ID = i.ID,
								Name = i.Name,
								Price = i.Price
							}).ToList();
				var cacheEntryOptions = new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromHours(8));

				_cache.Set("MenuItem", menuItem, cacheEntryOptions);
			}
			return menuItem;
		}
		public MenuItemDTO SingleMenuItem(int menuItemID)
		{
			var item = (from i in _menuItems.All()
								where i.ID == menuItemID &&
									  !i.isDeleted
								select new MenuItemDTO
								{
									ID = i.ID,
									Name = i.Name,
									Price = i.Price
								}).SingleOrDefault();
			if(item == null)
			{
				throw new ItemNotFoundException();
			}
			return item;
		}


		public MenuItemDTO AddItemToMenu(MenuItemViewModel newItem)
		{
			var item = new MenuItem
			{ 
				Name = newItem.Name, 
				SpicyLevel = newItem.SpicyLevel, 
				Description = newItem.Description,
				Price = newItem.Price			
			};
			
			_menuItems.Add(item);
			_uow.Save();
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

			// TODO: empty the cache for the menu
		}

		/// <summary>
		/// Used to add an order to the database
		/// </summary>
		/// <param name="orderViewModel">View Model with the order to add</param>
		public OrderDTO AddOrder(OrderViewModel orderViewModel)
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
			return GetOrderByID(orderID);
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
			var item = (from o in _orders.All()
								where o.ID == orderID
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
	}
}