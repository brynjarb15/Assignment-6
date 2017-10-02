using System;
using System.ComponentModel.DataAnnotations;

namespace PizzaApi.Models.ViewModels
{
	public class MenuItemViewModel
	{
		[Required]
		public string Name { get; set; }
		[Required]
		public int SpicyLevel { get; set; }
		[Required]
		public string Description { get; set; }
		[Required]
		public double Price { get; set; }
	}
}