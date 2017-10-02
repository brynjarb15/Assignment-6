namespace PizzaApi.Models.EntityModels
{
	///<summary>
	///The entity model of the menu items
	///</summary>
	public class MenuItem
	{
		///<summary>
		///The id of the menu item
		///</summary>
		public int ID { get; set; }
		///<summary>
		///The name of the menu item
		///</summary>
		public string Name { get; set; }
		///<summary>
		///The level of spicyness of the menu item
		///</summary>
		public int SpicyLevel { get; set; }
		///<summary>
		///The description of the menu item
		///</summary>
		public string Description { get; set; }
		///<summary>
		///The price of the menu item
		///</summary>
		public double Price { get; set; }
		///<summary>
		///Has the menu item been deleted from database
		///</summary>
		public bool isDeleted { get; set; }
	}
}