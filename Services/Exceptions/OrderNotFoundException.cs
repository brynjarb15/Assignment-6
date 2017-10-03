using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PizzaApi.Services.Exceptions
{
	public class OrderNotFoundException : Exception
	{
		public OrderNotFoundException() : base("Order with the given ID could not be found."){ }
	}
}
