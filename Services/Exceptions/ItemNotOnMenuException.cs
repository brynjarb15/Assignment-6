using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PizzaApi.Services.Exceptions
{
	public class ItemNotOnMenuException : Exception
	{
	public ItemNotOnMenuException() : base("Item with given id was not on the menu"){ }
	}
}
