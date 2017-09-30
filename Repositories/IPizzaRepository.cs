using System.Linq;

namespace PizzaApi.Repositories
{
	public interface IPizzaRepository<T> where T : class
	{
		void Add(T entity);
		void Delete(T entity);
		void Update(T entity);
		IQueryable<T> All();
	}
}
