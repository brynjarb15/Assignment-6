using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Caching.Memory;
using PizzaApi.Models;
using PizzaApi.Models.EntityModels;
using PizzaApi.Repositories;

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
				menuItem = (from c in _menuItems.All()
							select new MenuItemDTO
							{
								ID = c.ID,
								Name = c.Name,
								Price = c.Price
							}).ToList();
				var cacheEntryOptions = new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromHours(8));

				_cache.Set("MenuItem", menuItem, cacheEntryOptions);
			}
			return menuItem;
		}
	}
}