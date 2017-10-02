using System;

namespace PizzaApi.Models.EntityModels
{
	///<summary>
	///Entity model for the order.
	///</summary>
	public class Order
	{
		///<summary>
		///The id of the order
		///</summary>
		public int ID { get; set; }
		///<summary>
		///The date of the order
		///</summary>
		public DateTime? DateOfOrder { get; set; }
		///<summary>
		///The name of the customer
		///</summary>
		public string CustomerName { get; set; }
		///<summary>
		///If the order has been picked up
		///</summary>
		public bool isPickup { get; set; }
		///<summary>
		///The address of the customer
		///</summary>
		public string Address { get; set; }
		///<summary>
		///Has the order been cancelled
		///</summary>
		public bool isCancelled { get; set; }
	}
}