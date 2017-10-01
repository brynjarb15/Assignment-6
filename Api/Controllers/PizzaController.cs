using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Api.Services.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using PizzaApi.Services;

namespace PizzaApi.Controllers
{
	[Route("api/")]
	public class PizzaController : Controller
	{
		private readonly IPizzaService _pizzaService;
		private IMemoryCache _cache;

		public PizzaController(IPizzaService pizzaService, IMemoryCache memoryCache)
		{
			_pizzaService = pizzaService;
			_cache = memoryCache;
		}

		// GET api/
		[HttpGet]
		[Route("menu")]
		public IActionResult GetMenu()
		{
			var menu = _pizzaService.GetMenu();

			return Ok(menu);
		}

		[HttpGet]
		[Route("menu/{menuItemID:int}")]
		public IActionResult SingleMenuItem(int menuItemID)
		{
			try
			{
				var singleItem = _pizzaService.SingleMenuItem(menuItemID);
				return Ok(singleItem);
			}
			catch(ItemNotFoundException e)
			{
				return NotFound(e.Message);
			}
			
		}

		// GET api/values/5
/*		[HttpGet("{id}")]
		public string Get(int id)
		{
			return "value";
		}

		// POST api/values
		[HttpPost]
		public void Post([FromBody]string value)
		{
		}

		// PUT api/values/5
		[HttpPut("{id}")]
		public void Put(int id, [FromBody]string value)
		{
		}

		// DELETE api/values/5
		[HttpDelete("{id}")]
		public void Delete(int id)
		{
		}**/
	}
}
