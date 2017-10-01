using System;

namespace PizzaApi.Models.ViewModels
{
	public class MenuItemViewModel
	{
		public string Name { get; set; }
        
		public int SpicyLevel { get; set; }

		public string Description { get; set; }

        public double Price { get; set; }
	}
}