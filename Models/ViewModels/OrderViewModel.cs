using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PizzaApi.Models.ViewModels
{
	public class OrderViewModel
	{
		[Required]
		public DateTime? DateOfOrder { get; set; }
		[Required]
		public string CustomerName { get; set; }
		[Required]
		public bool IsPickup { get; set; }
		[Required]
		public string Address { get; set; }
		[Required]
		public List<int> OrderItemsIds { get; set; }
	}
}