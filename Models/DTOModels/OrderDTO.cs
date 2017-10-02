using System;
using System.Collections.Generic;

namespace PizzaApi.Models
{
	public class OrderDTO
	{
		public int ID { get; set; }
		public DateTime? DateOfOrder { get; set; }
		public string CustomerName { get; set; }
		public bool IsPickup { get; set; }
		public string Address { get; set; }
		public List<MenuItemDTO> OrderedItems { get; set; }
	}
}