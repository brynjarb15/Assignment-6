using Microsoft.EntityFrameworkCore;
using PizzaApi.Models;
using PizzaApi.Models.EntityModels;
using PizzaApi.Repositories;

namespace PizzaApi.Repositories
{
	public class AppDataContext : DbContext, IDbContext
	{
		public AppDataContext(DbContextOptions<AppDataContext> options)
		: base(options)
		{ }
		public DbSet<MenuItem>              MenuItems              { get; set; }
		public DbSet<Order>                 Orders                 { get; set; }
		public DbSet<OrderLink>             OrderLinks             { get; set; }
	}
}