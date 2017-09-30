using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace PizzaApi.Repositories
{

	/// <summary>
	/// Class that implements Generic Repository pattern
	/// </summary>
	/// <typeparam name="T">Model class</typeparam>
	public class PizzaRepository<T> : IPizzaRepository<T> where T : class
	{
		private readonly IDbContext _context;
		private readonly DbSet<T> _dbset;

		/// <summary>
		/// Constructor that sets the DataContext and gets the DbSet
		/// </summary>
		/// <param name="context"></param>
		public PizzaRepository(IDbContext context)
		{
			_context = context;
			_dbset = context.Set<T>();
		}

		public virtual void Add(T entity)
		{
			_dbset.Add(entity);
		}

		public virtual void Delete(T entity)
		{
			var entry = _context.Entry(entity);
			entry.State = EntityState.Deleted;
		}

		public virtual void Update(T entity)
		{
			var entry = _context.Entry(entity);
			_dbset.Attach(entity);
			entry.State = EntityState.Modified;
		}

		public virtual IQueryable<T> All()
		{
			IQueryable<T> query = _dbset;
			return query;
		}
	}

}
