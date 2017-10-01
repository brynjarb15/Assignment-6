using System;
using System.Collections.Generic;
using PizzaApi.Models;

namespace PizzaApi.Services
{
	public interface IPizzaService
	{
		//IEnumerable<MenuItemDTO> GetMenu();
		List<MenuItemDTO> GetMenu();
		MenuItemDTO SingleMenuItem(int menuItemID);
	} 

}
