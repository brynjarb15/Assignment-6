﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using PizzaApi.Models;
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
			try
			{
				var menu = _pizzaService.GetMenu();
				return Ok(menu);
			}
			catch(NoItemsInListException e)
			{
				return StatusCode(204, e.Message);
			}
		}

		[HttpGet]
		[Route("menu/{menuItemID:int}", Name = "SingleMenuItem")]
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


		[HttpPost]
		[Route("menu")]
		public IActionResult AddItemToMenu([FromBody] MenuItemViewModel newItem)
		{
			if(newItem == null) { return BadRequest(); }
			if(!ModelState.IsValid){ return StatusCode(412); }

			var res = _pizzaService.AddItemToMenu(newItem);
			return Ok(res);
			
		}

		/// <summary>
		/// Used to delete item from the menu returns http code 404 if the id is not found or
		/// http code 204(No Content) if it was deleted
		/// </summary>
		/// <param name="menuItemID">ID of the item to delete</param>
		/// <returns>Http code 404 or http code 204</returns>
		[HttpDelete]
		[Route("menu/{menuItemID:int}")]
		public IActionResult DeleteMenuItem(int menuItemID)
		{
			try
			{
				_pizzaService.DeleteMenuItem(menuItemID);
			}
			catch(ItemNotFoundException e)
			{
				return NotFound(e.Message);
			}
			
			return NoContent();
		}

		/// <summary>
		/// Adds an order to the database
		/// </summary>
		/// <param name="orderViewModel">View model of the oreder that should be added</param>
		/// <returns></returns>
		[HttpPost]
		[Route("orders")]
		public IActionResult AddOrder([FromBody] OrderViewModel orderViewModel) // TODO
		{
			var order = new OrderDTO();
			if (orderViewModel == null)
			{
				return BadRequest();
			}
			if (!ModelState.IsValid)
			{
				return StatusCode(412);
			}
			try{
				order = _pizzaService.AddOrder(orderViewModel);
			}
			catch(ItemNotOnMenuException e)
			{
				return StatusCode(412, e.Message);
			}

			return CreatedAtRoute("SingleMenuItem", new { menuItemID = order.ID }, order);
		}

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
		[HttpDelete]
		[Route("orders/{orderID:int}")]
		public IActionResult DeleteOrder(int orderID) // TODO
		{
			return Ok();
		}
*/
	}
}
