namespace ProxyApi.Tests.Unit
{
	using System;
	using System.Net;
	using Microsoft.AspNetCore.Http;
	using Microsoft.AspNetCore.Mvc;
	using Microsoft.AspNetCore.Mvc.Abstractions;
	using Microsoft.AspNetCore.Mvc.Filters;
	using Microsoft.AspNetCore.Routing;
	using NUnit.Framework;

	[TestFixture]
	internal class RequireAuthenticationTokenAttributeTests
	{
		private RequireAuthenticationTokenAttribute sut;

		private AuthorizationFilterContext fakeContext;

		[SetUp]
		public void BeforeEachTest()
		{
			sut = new RequireAuthenticationTokenAttribute();

			fakeContext = new AuthorizationFilterContext(
				new ActionContext(new DefaultHttpContext(), new RouteData(), new ActionDescriptor()),
				Array.Empty<IFilterMetadata>());
		}

		[Test]
		public void OnAuthorization_WhenAuthenticationTokenProvided_SetsRequestUser()
		{
			const string expectedAuthenticationToken = "575bab67-4f81-417a-8bae-cd062a0f9407";

			fakeContext.HttpContext.Request.Headers.Add(HttpHeaderName.Authorization, expectedAuthenticationToken);

			sut.OnAuthorization(fakeContext);

			var actual = fakeContext.HttpContext.User;

			Assert.That(actual, Is.Not.Null, nameof(fakeContext.HttpContext.User));
			Assert.That(actual.Identity.Name, Is.EqualTo(expectedAuthenticationToken).IgnoreCase, nameof(actual.Identity.Name));
		}

		[Test]
		public void OnAuthorization_WhenAuthenticationTokenIsNotProvided_Returns401()
		{
			sut.OnAuthorization(fakeContext);

			var actual = fakeContext.Result as UnauthorizedObjectResult;

			Assert.That(actual, Is.Not.Null, nameof(fakeContext.Result));
			Assert.That(actual.Value, Does.Contain("provide an authentication token").IgnoreCase, nameof(actual.Value));
		}

		[TestCase("test")]
		[TestCase("{df4f3ec0-5481-4ff8-9943-c824e4292f32}")]
		public void OnAuthorization_WhenAuthenticationTokenIsInvalid_Returns400(string invalidAuthenticationToken)
		{
			fakeContext.HttpContext.Request.Headers.Add(HttpHeaderName.Authorization, invalidAuthenticationToken);

			sut.OnAuthorization(fakeContext);

			var actual = fakeContext.Result as ObjectResult;

			Assert.That(actual, Is.Not.Null, nameof(fakeContext.Result));
			Assert.That(actual.StatusCode, Is.EqualTo((int)HttpStatusCode.BadRequest), nameof(actual.StatusCode));
			Assert.That(actual.Value, Does.Contain("valid authentication token").IgnoreCase, nameof(actual.Value));
		}
	}
}