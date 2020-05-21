namespace ProxyApi.Models
{
	using System;

	public class Category
	{
		public Guid Id { get; set; }
		public string Name { get; set; }
		public ulong ProductsCount { get; set; }
	}
}