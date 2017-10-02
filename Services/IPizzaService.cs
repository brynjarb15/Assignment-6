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
<<<<<<< HEAD
		MenuItemDTO AddItemToMenu(MenuItemViewModel newItem);
=======

		void DeleteMenuItem(int menuItemID);
>>>>>>> 890dce353897d94450be17936d7886998e2e79ee
	} 

}
