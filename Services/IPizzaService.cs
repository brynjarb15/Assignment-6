using System;
using System.Collections.Generic;
using PizzaApi.Models;
using PizzaApi.Models.ViewModels;

namespace PizzaApi.Services
{
	public interface IPizzaService
	{
		//IEnumerable<MenuItemDTO> GetMenu();
		IEnumerable<MenuItemDTO> GetMenu();
		MenuItemDTO SingleMenuItem(int menuItemID);
		MenuItemDTO AddItemToMenu(MenuItemViewModel newItem);
		void DeleteMenuItem(int menuItemID);
        List<OrderDTO> GetOrders();
		OrderDTO GetOrderByID(int orderID);
		void AddOrder(OrderViewModel orderViewModel);
	} 


}
