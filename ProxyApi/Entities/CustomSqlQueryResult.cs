namespace ProxyApi.Entities
{
	using System.Collections.Generic;

	public class CustomSqlQueryResult
	{
		public bool IsError { get; set; }
		public string ErrorMessage { get; set; }
		public Dictionary<string, string>[] Results { get; set; }
	}
}