namespace ProxyApi
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Net;
	using System.Net.Http;
	using System.Text.Json;
	using System.Threading.Tasks;
	using Microsoft.Extensions.Options;
	using Models;

	public class LinnworksWebApi : IWebApi
	{
		private readonly HttpClient httpClient;

		public LinnworksWebApi(HttpClient httpClient, IOptionsSnapshot<ProxyOptions> optionsSnapshot)
		{
			this.httpClient = httpClient;

			this.httpClient.BaseAddress = optionsSnapshot.Value.LinnWorksWebApiBaseUrl;
		}

		public async Task<IEnumerable<Category>> GetCategoriesAsync(Guid authorizationToken)
		{
			var requestMessage = new HttpRequestMessage(HttpMethod.Post, "Inventory/GetCategories");

			requestMessage.Headers.Add(HttpHeaderName.Authorization, authorizationToken.ToString());

			var response = await httpClient.SendAsync(requestMessage);

			if (response.StatusCode == HttpStatusCode.OK)
			{
				var responseStream = await response.Content.ReadAsStreamAsync();

				var entities = await JsonSerializer.DeserializeAsync<IEnumerable<Entities.Category>>(responseStream);

				return entities.Select(e => new Category
				{
					Id = e.CategoryId,
					Name = e.CategoryName
				});
			}

			var content = await response.Content.ReadAsStringAsync();

			throw new WebApiResponseException(response.StatusCode, content);
		}

		public async Task<IEnumerable<Dictionary<string, string>>> ExecuteCustomSqlQueryAsync(Guid authorizationToken, string sqlQuery)
		{
			if (string.IsNullOrWhiteSpace(sqlQuery))
				throw new ArgumentException("Provide the SQL query to execute.", nameof(sqlQuery));

			var requestMessage = new HttpRequestMessage(HttpMethod.Post, "Dashboards/ExecuteCustomScriptQuery");

			requestMessage.Headers.Add(HttpHeaderName.Authorization, authorizationToken.ToString());

			requestMessage.Content = new FormUrlEncodedContent(new[]
			{
				new KeyValuePair<string, string>("script", sqlQuery.Trim())
			});

			var response = await httpClient.SendAsync(requestMessage);

			if (response.StatusCode == HttpStatusCode.OK)
			{
				var responseStream = await response.Content.ReadAsStreamAsync();

				var result = await JsonSerializer.DeserializeAsync<Entities.CustomSqlQueryResult>(responseStream);

				if (result.IsError)
					throw new WebApiResponseException(response.StatusCode, result.ErrorMessage);

				return result.Results;
			}

			var responseContent = await response.Content.ReadAsStringAsync();

			throw new WebApiResponseException(response.StatusCode, responseContent);
		}

		public async Task<IEnumerable<Category>> GetCategoriesWithProductsCountAsync(Guid authorizationToken)
		{
			const string productsCountSql =
				"SELECT c.CategoryId, Count(*) AS ProductsCount " +
				"FROM StockItem AS i INNER JOIN ProductCategories AS c ON c.CategoryId = i.CategoryId " +
				"GROUP BY c.CategoryId";

			var categoriesTask = GetCategoriesAsync(authorizationToken);
			var productsCountTask = ExecuteCustomSqlQueryAsync(authorizationToken, productsCountSql);

			// It's done this way only because the test exercise explicitly requires it.
			// It's actually possible to get all categories with the total amount of products in them in a single HTTP
			// request, by executing the following SQL query:
			// SELECT c.CategoryId, c.CategoryName, Count(*) AS ProductsCount
			// FROM StockItem AS i INNER JOIN ProductCategories AS c ON c.CategoryId = i.CategoryId
			// GROUP BY c.CategoryId, c.CategoryName
			await Task.WhenAll(categoriesTask, productsCountTask);

			var categoriesWithProductsCount = new List<Category>(categoriesTask.Result);

			foreach (var dictionary in productsCountTask.Result.Where(d =>
				d.ContainsKey(nameof(Entities.Category.CategoryId)) &&
				d.ContainsKey(nameof(Category.ProductsCount))))
			{
				var category = categoriesWithProductsCount.FirstOrDefault(c => string.Equals(
					c.Id.ToString(),
					dictionary[nameof(Entities.Category.CategoryId)],
					StringComparison.OrdinalIgnoreCase));

				if (category != null)
					category.ProductsCount = ulong.Parse(dictionary[nameof(Category.ProductsCount)]);
			}

			return categoriesWithProductsCount;
		}

		public async Task<Category> CreateNewCategoryAsync(Guid authorizationToken, string categoryName)
		{
			if (string.IsNullOrWhiteSpace(categoryName))
				throw new ArgumentException("Provide the new category name.", nameof(categoryName));

			var requestMessage = new HttpRequestMessage(HttpMethod.Post, "Inventory/CreateCategory");

			requestMessage.Headers.Add(HttpHeaderName.Authorization, authorizationToken.ToString());

			requestMessage.Content = new FormUrlEncodedContent(new[]
			{
				new KeyValuePair<string, string>("categoryName", categoryName.Trim())
			});

			var response = await httpClient.SendAsync(requestMessage);

			if (response.StatusCode == HttpStatusCode.OK)
			{
				var responseStream = await response.Content.ReadAsStreamAsync();

				var entity = await JsonSerializer.DeserializeAsync<Entities.Category>(responseStream);

				return new Category
				{
					Id = entity.CategoryId,
					Name = entity.CategoryName
				};
			}

			var responseContent = await response.Content.ReadAsStringAsync();

			throw new WebApiResponseException(response.StatusCode, responseContent);
		}
	}
}