namespace ProxyApi
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Net;
	using System.Threading.Tasks;
	using Models;

	public class FakeLinnworksWebApi : ILinnworksWebApi
	{
		private readonly Random rng = new Random();

		private readonly List<Category> categories = new List<Category>
		{
			new Category
			{
				Id = Guid.Empty,
				Name = "Default",
				ProductsCount = 5
			},
			new Category
			{
				Id = Guid.Parse("be65ee42-4ca1-48ba-97bc-e67e2a635601"),
				Name = "Guitars",
				ProductsCount = 23
			}
		};

		public Task<Category> CreateNewCategoryAsync(string categoryName)
		{
			if (categories.Exists(c => string.Equals(c.Name, categoryName, StringComparison.OrdinalIgnoreCase)))
			{
				throw new WebApiKnownException(
					HttpStatusCode.BadRequest,
					"A category with the same name already exists. Use another name.");
			}

			var newCategory = new Category
			{
				Id = Guid.NewGuid(),
				Name = categoryName,
				ProductsCount = (ulong)rng.Next(0, 25)
			};

			categories.Add(newCategory);

			return Task.FromResult(newCategory);
		}

		public Task DeleteCategoryAsync(Guid categoryId)
		{
			categories.RemoveAll(c => c.Id == categoryId);

			return Task.FromResult(0);
		}

		public Task<Dictionary<string, string>[]> ExecuteCustomSqlQueryAsync(string sqlQuery)
		{
			return Task.FromResult(Array.Empty<Dictionary<string, string>>());
		}

		public Task<Category[]> GetCategoriesAsync()
		{
			return GetCategoriesWithProductsCountAsync();
		}

		public Task<Category[]> GetCategoriesWithProductsCountAsync()
		{
			return Task.FromResult(categories.ToArray());
		}

		public Task UpdateCategoryAsync(Entities.Category updatedCategory)
		{
			var categoryToUpdate = categories.FirstOrDefault(c => c.Id == updatedCategory.CategoryId);

			if (categoryToUpdate is null)
			{
				throw new WebApiKnownException(
					HttpStatusCode.BadRequest,
					"A category with such ID doesn't exist.");
			}

			if (categories.Exists(c => string.Equals(c.Name, updatedCategory.CategoryName, StringComparison.OrdinalIgnoreCase)))
			{
				throw new WebApiKnownException(
					HttpStatusCode.BadRequest,
					"A category with the same name already exists. Use another name.");
			}

			categoryToUpdate.Name = updatedCategory.CategoryName;

			return Task.FromResult(0);
		}
	}
}