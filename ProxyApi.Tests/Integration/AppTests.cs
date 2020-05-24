namespace ProxyApi.Tests.Integration
{
	using System;
	using System.Net;
	using System.Net.Http;
	using System.Threading.Tasks;
	using Microsoft.AspNetCore.Mvc.Testing;
	using Microsoft.Net.Http.Headers;
	using NUnit.Framework;

	[TestFixture]
	internal class AppTests
	{
		private HttpClient httpClient;

		[SetUp]
		public void BeforeEachTest()
		{
			var factory = new WebApplicationFactory<Startup>();

			httpClient = factory.CreateClient();
		}

		[Test]
		public async Task Cors_IsEnabled()
		{
			const string expectedOrigin = "*";
			const string expectedMethod = "POST";
			const string expectedHeader = "test";
			var expectedMaxAge = TimeSpan.FromDays(1).TotalSeconds.ToString();

			var request = new HttpRequestMessage(HttpMethod.Options, "categories");

			request.Headers.Add(HttpHeaderName.Authorization, Guid.Empty.ToString());
			request.Headers.Add(HeaderNames.Origin, "localhost");
			request.Headers.Add(HeaderNames.AccessControlRequestMethod, expectedMethod);
			request.Headers.Add(HeaderNames.AccessControlRequestHeaders, expectedHeader);

			var actual = await httpClient.SendAsync(request);

			Assert.That(actual.StatusCode, Is.EqualTo(HttpStatusCode.NoContent), nameof(actual.StatusCode));

			Assert.That(actual.Headers.TryGetValues(HeaderNames.AccessControlAllowOrigin, out var actualOrigin), Is.True, nameof(HeaderNames.AccessControlAllowOrigin));
			Assert.That(string.Join(string.Empty, actualOrigin), Is.EqualTo(expectedOrigin), nameof(HeaderNames.AccessControlAllowOrigin));

			Assert.That(actual.Headers.TryGetValues(HeaderNames.AccessControlAllowMethods, out var actualMethod), Is.True, nameof(HeaderNames.AccessControlAllowMethods));
			Assert.That(string.Join(string.Empty, actualMethod), Is.EqualTo(expectedMethod), nameof(HeaderNames.AccessControlAllowMethods));

			Assert.That(actual.Headers.TryGetValues(HeaderNames.AccessControlAllowHeaders, out var actualHeader), Is.True, nameof(HeaderNames.AccessControlAllowHeaders));
			Assert.That(string.Join(string.Empty, actualHeader), Is.EqualTo(expectedHeader), nameof(HeaderNames.AccessControlAllowHeaders));

			Assert.That(actual.Headers.TryGetValues(HeaderNames.AccessControlMaxAge, out var actualMaxAge), Is.True, nameof(HeaderNames.AccessControlMaxAge));
			Assert.That(string.Join(string.Empty, actualMaxAge), Is.EqualTo(expectedMaxAge), nameof(HeaderNames.AccessControlMaxAge));
		}
	}
}