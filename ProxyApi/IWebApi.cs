namespace ProxyApi
{
	using System;
	using System.Collections.Generic;
	using System.Threading.Tasks;
	using Models;

	public interface IWebApi
	{
		Task<IEnumerable<Category>> GetCategoriesAsync(Guid authorizationToken);
		Task<IEnumerable<Dictionary<string, string>>> ExecuteCustomSqlQueryAsync(Guid authorizationToken, string sqlQuery);
		Task<IEnumerable<Category>> GetCategoriesWithProductsCountAsync(Guid authorizationToken);
	}
}