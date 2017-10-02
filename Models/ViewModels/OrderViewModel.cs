using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PizzaApi.Models.ViewModels
{
	public class OrderViewModel
	{
		public DateTime DateOfOrder { get; set; }
		public string CustomerName { get; set; }

		public bool IsPickup { get; set; }
		[Required]
		public string Address { get; set; }
		public List<int> OrderItemsIds { get; set; }
	}
}