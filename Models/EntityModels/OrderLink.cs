using System;

namespace PizzaApi.Models.EntityModels
{
	public class OrderLink
	{
		public int ID { get; set; }
		public int OrderId { get; set; }
		public string MenuItemId { get; set; }
	}
}