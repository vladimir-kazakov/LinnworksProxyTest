﻿namespace ProxyApi.Controllers
{
	using System;
	using System.Net;
	using System.Security.Claims;
	using System.Threading.Tasks;
	using Microsoft.AspNetCore.Mvc;
	using Models;

	/// <remarks>
	/// The Linnworks Web API doesn't follow the RESTful API design guidelines,
	/// but this web API (proxy) does follow them. That's why some actions
	/// require strange data transformations.
	/// </remarks>
	[ApiController, Route("[controller]")]
	[RequireAuthenticationToken]
	[ServiceFilter(typeof(WebApiKnownExceptionHandler))]
	public class CategoriesController : ControllerBase
	{
		private readonly ILinnworksWebApiFactory webApiFactory;

		public CategoriesController(ILinnworksWebApiFactory webApiFactory)
		{
			this.webApiFactory = webApiFactory;
		}

		[HttpGet]
		public async Task<Category[]> GetAsync()
		{
			var webApi = webApiFactory.Create(User.FindFirstValue(ProxyClaimTypes.AuthenticationToken));

			return await webApi.GetCategoriesWithProductsCountAsync();
		}

		[HttpPost]
		public async Task<ActionResult<Category>> PostAsync(NewCategory newCategory)
		{
			var webApi = webApiFactory.Create(User.FindFirstValue(ProxyClaimTypes.AuthenticationToken));

			// Since the Linnworks Web API doesn't have an endpoint to get a category by its ID,
			// and the create new category endpoint also doesn't return the Location HTTP response header,
			// the URL for this header cannot be created.
			return StatusCode((int)HttpStatusCode.Created, await webApi.CreateNewCategoryAsync(newCategory.Name));
		}

		[HttpPut("{id}")]
		public async Task<IActionResult> PutAsync(Guid id, UpdatedCategory updatedCategory)
		{
			var webApi = webApiFactory.Create(User.FindFirstValue(ProxyClaimTypes.AuthenticationToken));

			await webApi.UpdateCategoryAsync(new Entities.Category
			{
				CategoryId = id,
				CategoryName = updatedCategory.Name
			});

			return NoContent();
		}

		[HttpDelete("{id}")]
		public async Task<IActionResult> DeleteAsync(Guid id)
		{
			var webApi = webApiFactory.Create(User.FindFirstValue(ProxyClaimTypes.AuthenticationToken));

			await webApi.DeleteCategoryAsync(id);

			return NoContent();
		}
	}
}