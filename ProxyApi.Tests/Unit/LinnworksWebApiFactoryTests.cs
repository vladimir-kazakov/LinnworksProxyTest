namespace ProxyApi.Tests.Unit
{
	using System.Net.Http;
	using Microsoft.Extensions.Options;
	using NSubstitute;
	using NUnit.Framework;

	[TestFixture]
	internal class LinnworksWebApiFactoryTests
	{
		private LinnworksWebApiFactory sut;

		[SetUp]
		public void BeforeEachTest()
		{
			var fakeHttpClientFactory = Substitute.For<IHttpClientFactory>();
			var fakeOptionsSnapshot = Substitute.For<IOptionsSnapshot<ProxyOptions>>();

			fakeHttpClientFactory.CreateClient().Returns(new HttpClient());

			fakeOptionsSnapshot.Value.Returns(new ProxyOptions());

			sut = new LinnworksWebApiFactory(fakeHttpClientFactory, fakeOptionsSnapshot);
		}

		[Test]
		public void Create_UsingEmptyGuid_ReturnsFakeLinnworksWebApi()
		{
			var actual = sut.Create("00000000-0000-0000-0000-000000000000");

			Assert.That(actual, Is.TypeOf<FakeLinnworksWebApi>());
		}

		[Test]
		public void Create_UsingNonEmptyGuid_ReturnsLinnworksWebApi()
		{
			var actual = sut.Create("b1caa86d-cd7e-4af4-9591-2d923d31fefa");

			Assert.That(actual, Is.TypeOf<LinnworksWebApi>());
		}
	}
}