namespace ProxyApi
{
	using System;
	using System.Net.Http;
	using Microsoft.Extensions.Options;

	public class LinnworksWebApiFactory : ILinnworksWebApiFactory
	{
		private static readonly FakeLinnworksWebApi FakeLinnworksWebApi = new FakeLinnworksWebApi();

		private readonly IHttpClientFactory httpClientFactory;
		private readonly IOptionsSnapshot<ProxyOptions> optionsSnapshot;

		public LinnworksWebApiFactory(IHttpClientFactory httpClientFactory, IOptionsSnapshot<ProxyOptions> optionsSnapshot)
		{
			this.httpClientFactory = httpClientFactory;
			this.optionsSnapshot = optionsSnapshot;
		}

		public ILinnworksWebApi Create(string authenticationToken)
		{
			if (string.Equals(authenticationToken, Guid.Empty.ToString(), StringComparison.OrdinalIgnoreCase))
				return FakeLinnworksWebApi;

			return new LinnworksWebApi(authenticationToken, httpClientFactory, optionsSnapshot);
		}
	}
}