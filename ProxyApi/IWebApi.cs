namespace ProxyApi
{
	using System.Collections.Generic;
	using System.Threading.Tasks;
	using Models;

	public interface IWebApi
	{
		Task<Category> CreateNewCategoryAsync(string categoryName);

		Task<Category[]> GetCategoriesAsync();
		Task<Category[]> GetCategoriesWithProductsCountAsync();

		Task<Dictionary<string, string>[]> ExecuteCustomSqlQueryAsync(string sqlQuery);
	}
}