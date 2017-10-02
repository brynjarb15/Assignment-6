﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using PizzaApi.Models.ViewModels;
using PizzaApi.Services;
using PizzaApi.Services.Exceptions;

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


		[HttpDelete]
		[Route("menu/{menuItemID:int}")]
		public IActionResult DeleteMenuItem(int menuItemID) // TODO
		{
			_pizzaService.DeleteMenuItem(menuItemID);
			return Ok();
		}

		[HttpPost]
		[Route("orders")]
		public IActionResult AddOrder([FromBody] OrderViewModel orderViewModel) // TODO
		{
			if (orderViewModel == null)
			{
				return BadRequest();
			}
			if (!ModelState.IsValid)
			{
				return StatusCode(412);
			}
			try{
				_pizzaService.AddOrder(orderViewModel);
			}
			catch(ItemNotOnMenuException e)
			{
				return StatusCode(412, e.Message);
			}

			return Ok();
		}

/*
		[HttpPost]
		[Route("menu")]
		public IActionResult AddMenuItem([FromBody] MenuItemViewModel menuItem) // TODO
		{
			return Ok();
		}

		[HttpDelete]
		[Route("menu/{menuItemID:int}")]
		public IActionResult DeleteMenuItem() // TODO
		{
			return Ok();
		}
*/
		[HttpGet]
		[Route("orders")]
		public IActionResult GetOrders() // TODO
		{
			var orders = _pizzaService.GetOrders();
			return Ok(orders);
		}

		[HttpGet]
		[Route("orders/{orderID:int}")]
		public IActionResult GetOrderByID(int orderID) // TODO
		{
			try
			{
				var orderById = _pizzaService.GetOrderByID(orderID);
				return Ok(orderById);
			}
			catch(ItemNotFoundException e)
			{
				return NotFound(e.Message);
			}
		}
/*
		[HttpPost]
		[Route("orders")]
		public IActionResult AddOrder([FromBody] OrderViewModel orderViewModel) // TODO
		{
			return Ok();
		}

		[HttpDelete]
		[Route("orders/{orderID:int}")]
		public IActionResult DeleteOrder(int orderID) // TODO
		{
			return Ok();
		}
*/
	}
}
