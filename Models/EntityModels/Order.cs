using System;

namespace PizzaApi.Models.EntityModels
{
	public class Order
	{
		public int ID { get; set; }
		public DateTime DateOfOder { get; set; }
		public string CustomerName { get; set; }
		public bool isPickup { get; set; }
		public string Address { get; set; }
		bool isCancelled { get; set; }
	}
}