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
		}
	}
}