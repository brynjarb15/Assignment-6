using System;
using Microsoft.EntityFrameworkCore;

namespace PizzaApi.Repositories
{
	/// <summary>
	/// Interface for Unit Of Work pattern
	/// </summary>
	public interface IUnitOfWork : IDisposable
	{
		IPizzaRepository<TEntity> GetRepository<TEntity>() where TEntity : class;
		void Save();
	}
}
