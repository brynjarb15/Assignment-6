using System;

namespace PizzaApi.Models
{
	public class OrderLiteDTO
	{
		public int ID { get; set; }
		public DateTime DateOfOrder { get; set; }
		public string CurstomerName { get; set; }
		public bool IsPickup { get; set; }
		public string Address { get; set; }
	}
}