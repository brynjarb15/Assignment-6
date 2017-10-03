using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PizzaApi.Services.Exceptions
{
	public class NoItemsInListException : Exception
	{
		public NoItemsInListException() : base("This list is empty"){ }
	}
}
