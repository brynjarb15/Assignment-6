using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PizzaApi.Services;

namespace PizzaApi.Controllers
{
	[Route("api/[controller]")]
	public class PizzaController : Controller
	{
		private readonly IPizzaService _pizzaService;

		public PizzaController(IPizzaService pizzaService)
		{
			_pizzaService = pizzaService;
		}

		// GET api/
		[HttpGet]
		public IActionResult GetMenu()
		{
			var menu = _pizzaService.GetMenu();

			return Ok(menu);
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
