using System;
using System.Collections.Generic;

namespace PizzaApi.Models
{
	///<summary>
	///The order details exposed to the user.
	///</summary>
	public class OrderDTO
	{
		///<summary>
		///The id of the order
		///</summary>
		public int ID { get; set; }
		///<summary>
		///The date of when the order was made
		///</summary>
		public DateTime? DateOfOrder { get; set; }
		///<summary>
		///The name of the customer
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
		///<summary>
		///The list of ordered items
		///</summary>
		public List<MenuItemDTO> OrderedItems { get; set; }
	}
}