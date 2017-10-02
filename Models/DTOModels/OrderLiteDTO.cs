using System;

namespace PizzaApi.Models
{
	///<summary>
	///The orders exposed to the user. Used when requesting all orders.
	///</summary>
	public class OrderLiteDTO
	{
		///<summary>
		///The id of the order
		///</summary>
		public int ID { get; set; }
		///<summary>
		///The date that the order was made
		///</summary>
		public DateTime? DateOfOrder { get; set; }
		///<summary>
		///The name of the custormer
		///</summary>
		public string CustomerName { get; set; }
		///<summary>
		///If the order has been picked up
		///</summary>
		public bool IsPickup { get; set; }
		///<summary>
		///The address of the customer
		///</summary>
		public string Address { get; set; }
	}
}