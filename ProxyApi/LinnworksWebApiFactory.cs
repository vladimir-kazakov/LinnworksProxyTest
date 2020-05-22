namespace ProxyApi
{
	using System.Net.Http;
	using Microsoft.Extensions.Options;

	public class LinnworksWebApiFactory : ILinnworksWebApiFactory
	{
		private readonly IHttpClientFactory httpClientFactory;
		private readonly IOptionsSnapshot<ProxyOptions> optionsSnapshot;

		public LinnworksWebApiFactory(IHttpClientFactory httpClientFactory, IOptionsSnapshot<ProxyOptions> optionsSnapshot)
		{
			this.httpClientFactory = httpClientFactory;
			this.optionsSnapshot = optionsSnapshot;
		}

		public ILinnworksWebApi Create(string authenticationToken)
		{
			return new LinnworksWebApi(authenticationToken, httpClientFactory, optionsSnapshot);
		}
	}
}