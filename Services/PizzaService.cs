using System.Collections.Generic;
using System.Linq;
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
		
		public PizzaService(IUnitOfWork uow)
		{
			_uow = uow;
			_menuItems = _uow.GetRepository<MenuItem>();
			_orders = _uow.GetRepository<Order>();
			_orderLinks = _uow.GetRepository<OrderLink>();
		}

		public List<MenuItemDTO> GetMenu()
		{
			var menu = (from c in _menuItems.All()
							select new MenuItemDTO
							{
								ID = c.ID,
								Name = c.Name,
								Price = c.Price
							}).ToList();
			return menu;
		}
	}
}