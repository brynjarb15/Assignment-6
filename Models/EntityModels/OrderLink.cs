using System;

namespace PizzaApi.Models.EntityModels
{
	///<summary>
	///This is a entity which links the ordered menu items to the order
	///</summary>
	public class OrderLink
	{
		///<summary>
		///The Id of the link
		///</summary>
		public int ID { get; set; }
		///<summary>
		///The id of the order
		///</summary>
		public int OrderId { get; set; }
		///<summary>
		///The id of the menu item
		///</summary>
		public int MenuItemId { get; set; }
	}
}