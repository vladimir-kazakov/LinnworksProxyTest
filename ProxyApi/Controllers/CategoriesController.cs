namespace ProxyApi.Controllers
{
	using System;
	using System.Net;
	using System.Threading.Tasks;
	using Microsoft.AspNetCore.Mvc;
	using Models;

	[ApiController, Route("[controller]")]
	[ServiceFilter(typeof(AuthenticationActionFilter))]
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
			var webApi = webApiFactory.Create(User.Identity.Name);

			return await webApi.GetCategoriesWithProductsCountAsync();
		}

		[HttpPost]
		public async Task<ActionResult<Category>> PostAsync([FromForm] NewCategory newCategory)
		{
			if (!ModelState.IsValid)
				return BadRequest(ModelState);

			var webApi = webApiFactory.Create(User.Identity.Name);

			// Since the Linnworks Web API doesn't have an endpoint to get a category by its ID,
			// and the create new category endpoint also doesn't return the Location HTTP response header,
			// the URL for this header cannot be created, so there is no need to include it at all.
			return StatusCode((int)HttpStatusCode.Created, await webApi.CreateNewCategoryAsync(newCategory.Name));
		}

		[HttpDelete("{id}")]
		public async Task<IActionResult> DeleteAsync(Guid id)
		{
			var webApi = webApiFactory.Create(User.Identity.Name);

			await webApi.DeleteCategoryAsync(id);

			return NoContent();
		}
	}
}