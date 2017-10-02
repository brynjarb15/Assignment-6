using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PizzaApi.Models.ViewModels
{
	///<summary>
	///A view model of the order.
	///</summary>
	public class OrderViewModel
	{
		///<summary>
		///The date of the order
		///</summary>
		[Required]
		public DateTime? DateOfOrder { get; set; }
		///<summary>
		///The name of the customer
		///</summary>
		[Required]
		public string CustomerName { get; set; }
		///<summary>
		///Has the order been picked up
		///</summary>
		[Required]
		public bool IsPickup { get; set; }
		///<summary>
		///The address of the customer
		///</summary>
		[Required]
		public string Address { get; set; }
		///<summary>
		///A list of all order ids
		///</summary>
		[Required]
		public List<int> OrderItemsIds { get; set; }
	}
}