namespace ProxyApi.Controllers
{
	using System;
	using System.Collections.Generic;
	using System.Net;
	using System.Threading.Tasks;
	using Microsoft.AspNetCore.Mvc;
	using Models;

	[Route("[controller]")]
	[ApiController]
	public class CategoriesController : ControllerBase
	{
		private readonly IWebApi webApi;

		private static readonly string UnauthorizedErrorMessage =
			"To access the API, provide an authorization token " +
			$"(a GUID in the {Guid.Empty} format) " +
			$"through the {HttpHeaderName.Authorization} HTTP request header.";

		public CategoriesController(IWebApi webApi)
		{
			this.webApi = webApi;
		}

		[HttpGet]
		public async Task<ActionResult<IEnumerable<Category>>> GetAsync(
			[FromHeader(Name = HttpHeaderName.Authorization)] Guid? authorizationToken = null)
		{
			if (!authorizationToken.HasValue)
				return Unauthorized(UnauthorizedErrorMessage);

			try
			{
				return Ok(await webApi.GetCategoriesWithProductsCountAsync(authorizationToken.Value));
			}
			catch (WebApiResponseException ex)
			{
				return StatusCode((int)ex.StatusCode, ex.Message);
			}
		}

		[HttpPost]
		public async Task<ActionResult<Category>> PostAsync(
			[FromHeader(Name = HttpHeaderName.Authorization)] Guid? authorizationToken,
			[FromForm] NewCategory newCategory)
		{
			if (!authorizationToken.HasValue)
				return Unauthorized(UnauthorizedErrorMessage);

			if (!ModelState.IsValid)
				return BadRequest(ModelState);

			try
			{
				var category = await webApi.CreateNewCategoryAsync(authorizationToken.Value, newCategory.Name);

				// Since the Linnworks Web API doesn't have an endpoint to get a category by its ID,
				// and the create new category endpoint also doesn't return the Location HTTP response header,
				// the URL for this header cannot be created, so there is no need to include it at all.
				return StatusCode((int)HttpStatusCode.Created, category);
			}
			catch (WebApiResponseException ex)
			{
				return StatusCode((int)ex.StatusCode, ex.Message);
			}
		}
	}
}