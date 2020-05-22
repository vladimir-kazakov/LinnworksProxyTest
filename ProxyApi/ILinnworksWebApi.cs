namespace ProxyApi
{
	using System;
	using System.Collections.Generic;
	using System.Threading.Tasks;
	using Models;

	public interface ILinnworksWebApi
	{
		Task<Category> CreateNewCategoryAsync(string categoryName);

		Task DeleteCategoryAsync(Guid categoryId);

		Task<Category[]> GetCategoriesAsync();
		Task<Category[]> GetCategoriesWithProductsCountAsync();

		Task<Dictionary<string, string>[]> ExecuteCustomSqlQueryAsync(string sqlQuery);
	}
}