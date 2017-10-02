namespace PizzaApi.Models
{
	///<summary>
	///The details about the menu item exposed to the user
	///</summary>
	public class MenuItemDTO
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
		///The price of the menu item
		///</summary>
		public double Price { get; set; }
	}
}