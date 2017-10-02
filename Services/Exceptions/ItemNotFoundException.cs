using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Api.Services.Exceptions
{
	public class ItemNotFoundException : Exception
	{
	public ItemNotFoundException() : base("Item with the given ID could not be found."){ }
	}
}
