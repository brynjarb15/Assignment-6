using System;
using System.Collections.Generic;
using PizzaApi.Models;
using PizzaApi.Models.ViewModels;

namespace PizzaApi.Services
{
	public interface IPizzaService
	{
		//IEnumerable<MenuItemDTO> GetMenu();
		List<MenuItemDTO> GetMenu();
		MenuItemDTO SingleMenuItem(int menuItemID);

		void DeleteMenuItem(int menuItemID);

		void AddOrder(OrderViewModel orderViewModel);
	} 

}
