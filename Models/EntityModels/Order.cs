using System;

namespace PizzaApi.Models.EntityModels
{
	public class Order
	{
		public int ID { get; set; }
		public DateTime? DateOfOrder { get; set; }
		public string CustomerName { get; set; }
		public bool isPickup { get; set; }
		public string Address { get; set; }
		public bool isCancelled { get; set; }
	}
}