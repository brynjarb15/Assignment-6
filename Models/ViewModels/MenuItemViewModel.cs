using System;
using System.ComponentModel.DataAnnotations;

namespace PizzaApi.Models.ViewModels
{
	///<summary>
	///A view model of the menu items, which is the data required from users.
	///</summary>
	public class MenuItemViewModel
	{
		///<summary>
		///The name of the menu item
		///</summary>
		[Required]
		public string Name { get; set; }
		///<summary>
		///The level of spicyness of the menu item
		///</summary>
		[Required]
		[Range(1, int.MaxValue, ErrorMessage = "SpiceLevel must be 1 or more")]
		public int SpicyLevel { get; set; }
		///<summary>
		///The description of the menu item
		///</summary>
		[Required]
		public string Description { get; set; }
		///<summary>
		///The price of the menu item
		///</summary>
		[Required]
		[Range(0.1, Double.PositiveInfinity, ErrorMessage = "Price must be more than 0.1")]
		public double Price { get; set; }
	}
}