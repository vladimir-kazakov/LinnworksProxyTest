namespace ProxyApi.Controllers
{
	using System;
	using System.Collections.Generic;
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
	}
}