namespace PizzaApi.Models.EntityModels
{
	public class MenuItem
	{
		public int ID { get; set; }
		public string Name { get; set; }
		public int SpicyLevel { get; set; }
		public string Description { get; set; }
		public double Price { get; set; }
		bool isDeleted { get; set; }
	}
}