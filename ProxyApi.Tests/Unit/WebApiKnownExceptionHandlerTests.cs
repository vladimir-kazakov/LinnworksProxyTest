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
	internal class WebApiKnownExceptionHandlerTests
	{
		private WebApiKnownExceptionHandler sut;

		private ExceptionContext actualContext;

		[SetUp]
		public void BeforeEachTest()
		{
			sut = new WebApiKnownExceptionHandler();

			actualContext = new ExceptionContext(
				new ActionContext(new DefaultHttpContext(), new RouteData(), new ActionDescriptor()),
				Array.Empty<IFilterMetadata>());
		}

		[Test]
		public void OnException_WhenExceptionIsKnown_HandlesIt()
		{
			var expectedException = new WebApiKnownException(HttpStatusCode.NotFound, "test");

			actualContext.Exception = expectedException;

			sut.OnException(actualContext);

			var actual = actualContext.Result as ObjectResult;

			Assert.That(actual, Is.Not.Null, nameof(actualContext.Result));
			Assert.That(actual.StatusCode, Is.EqualTo((int)expectedException.StatusCode), nameof(actual.StatusCode));
			Assert.That(actual.Value, Is.EqualTo(expectedException.Message), nameof(actual.Value));
		}

		[Test]
		public void OnException_WhenExceptionIsUnknown_DoesNotHandleIt()
		{
			actualContext.Exception = new Exception("test");

			sut.OnException(actualContext);

			Assert.That(actualContext.Result, Is.Null, nameof(actualContext.Result));
		}
	}
}