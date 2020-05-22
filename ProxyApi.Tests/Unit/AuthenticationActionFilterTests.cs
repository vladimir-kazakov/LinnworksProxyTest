namespace ProxyApi.Tests.Unit
{
	using System;
	using System.Collections.Generic;
	using System.Net;
	using System.Threading.Tasks;
	using Microsoft.AspNetCore.Http;
	using Microsoft.AspNetCore.Mvc;
	using Microsoft.AspNetCore.Mvc.Abstractions;
	using Microsoft.AspNetCore.Mvc.Filters;
	using Microsoft.AspNetCore.Routing;
	using NSubstitute;
	using NUnit.Framework;

	[TestFixture]
	internal class AuthenticationActionFilterTests
	{
		private AuthenticationActionFilter sut;

		private ActionExecutingContext fakeContext;
		private ActionExecutionDelegate fakeNext;

		[SetUp]
		public void BeforeEachTest()
		{
			sut = new AuthenticationActionFilter();

			fakeContext = new ActionExecutingContext(
				new ActionContext(new DefaultHttpContext(), new RouteData(), new ActionDescriptor()),
				Array.Empty<IFilterMetadata>(),
				new Dictionary<string, object>(),
				null);

			fakeNext = Substitute.For<ActionExecutionDelegate>();
		}

		[Test]
		public async Task OnActionExecutionAsync_WhenAuthenticationTokenProvided_ProceedsToNextActionFilter()
		{
			const string expectedAuthenticationToken = "575bab67-4f81-417a-8bae-cd062a0f9407";

			fakeContext.HttpContext.Request.Headers.Add(
				HttpHeaderName.Authorization, expectedAuthenticationToken);

			await sut.OnActionExecutionAsync(fakeContext, fakeNext);

			var actual = fakeContext.HttpContext.User;

			Assert.That(actual, Is.Not.Null, nameof(fakeContext.HttpContext.User));
			Assert.That(actual.Identity.Name, Is.EqualTo(expectedAuthenticationToken).IgnoreCase, nameof(actual.Identity.Name));

			await fakeNext.Received(1).Invoke();
		}

		[Test]
		public async Task OnActionExecutionAsync_WhenAuthenticationTokenIsNotProvided_Returns401()
		{
			await sut.OnActionExecutionAsync(fakeContext, fakeNext);

			var actual = fakeContext.Result as UnauthorizedObjectResult;

			Assert.That(actual, Is.Not.Null, nameof(fakeContext.Result));
			Assert.That(actual.Value, Does.Contain("provide an authentication token").IgnoreCase, nameof(actual.Value));

			await fakeNext.DidNotReceive().Invoke();
		}

		[Test]
		public async Task OnActionExecutionAsync_WhenAuthenticationTokenIsInvalid_Returns400()
		{
			fakeContext.HttpContext.Request.Headers.Add(HttpHeaderName.Authorization, "test");

			await sut.OnActionExecutionAsync(fakeContext, fakeNext);

			var actual = fakeContext.Result as ObjectResult;

			Assert.That(actual, Is.Not.Null, nameof(fakeContext.Result));
			Assert.That(actual.StatusCode, Is.EqualTo((int)HttpStatusCode.BadRequest), nameof(actual.StatusCode));
			Assert.That(actual.Value, Does.Contain("valid authentication token").IgnoreCase, nameof(actual.Value));

			await fakeNext.DidNotReceive().Invoke();
		}
	}
}