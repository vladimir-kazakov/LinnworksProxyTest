namespace ProxyApi.Tests.Unit
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Net;
	using System.Threading.Tasks;
	using Controllers;
	using Microsoft.AspNetCore.Mvc;
	using Models;
	using NSubstitute;
	using NSubstitute.ExceptionExtensions;
	using NUnit.Framework;

	[TestFixture]
	class CategoriesControllerTests
	{
		private CategoriesController sut;

		private IWebApi fakeWebApi;

		[SetUp]
		public void BeforeEachTest()
		{
			fakeWebApi = Substitute.For<IWebApi>();

			sut = new CategoriesController(fakeWebApi);
		}

		private static Category[] GetTestCategories()
		{
			return new[]
			{
				new Category
				{
					Id = Guid.Empty,
					Name = "Default",
					ProductsCount = 1
				},
				new Category
				{
					Id = Guid.Parse("1e060577-e3f3-4bb2-a1b1-415d43edeb9b"),
					Name = "Test",
					ProductsCount = 18
				}
			};
		}

		[Test]
		public async Task Get_Normally_ReturnsAllCategories()
		{
			var expectedCategories = GetTestCategories();

			fakeWebApi.GetCategoriesWithProductsCountAsync(Arg.Any<Guid>()).Returns(expectedCategories);

			var response = await sut.GetAsync(Guid.Empty);
			var actual = response.Result as OkObjectResult;

			Assert.That(actual, Is.Not.Null, nameof(response.Result));

			var actualCategories = actual.Value as IEnumerable<Category>;

			Assert.That(actualCategories, Is.Not.Null, nameof(actual.Value));
			Assert.That(actualCategories.Count(), Is.EqualTo(expectedCategories.Length), nameof(actual.Value));
		}

		[Test]
		public async Task Get_WithoutAuthorizationToken_Returns401()
		{
			var response = await sut.GetAsync();
			var actual = response.Result as UnauthorizedObjectResult;

			Assert.That(actual, Is.Not.Null, nameof(response.Result));
			Assert.That(actual.Value, Does.Contain("provide an authorization token").IgnoreCase, nameof(actual.Value));
		}

		[Test]
		public async Task Get_WhenKnownExceptionHappens_PassesItThrough()
		{
			var expectedException = new WebApiResponseException(HttpStatusCode.Unauthorized, "test");

			fakeWebApi.GetCategoriesWithProductsCountAsync(Arg.Any<Guid>()).Throws(expectedException);

			var response = await sut.GetAsync(Guid.Empty);
			var actual = response.Result as ObjectResult;

			Assert.That(actual, Is.Not.Null, nameof(response.Result));
			Assert.That(actual.StatusCode, Is.EqualTo((int)expectedException.StatusCode), nameof(actual.StatusCode));
			Assert.That(actual.Value, Is.EqualTo(expectedException.Message), nameof(actual.Value));
		}

		[Test]
		public void Get_WhenUnknownExceptionHappens_ThrowsIt()
		{
			var expectedException = new Exception("test");

			fakeWebApi.GetCategoriesWithProductsCountAsync(Arg.Any<Guid>()).Throws(expectedException);

			var actualException = Assert.ThrowsAsync(
				expectedException.GetType(), async () => await sut.GetAsync(Guid.Empty), "Type");

			Assert.That(actualException.Message, Is.EqualTo(expectedException.Message), nameof(actualException.Message));
		}

		[Test]
		public async Task Post_Normally_ReturnsCreatedCategory()
		{
			var expectedCategory = new Category
			{
				Id = Guid.Parse("4347ddb1-2fc6-4c41-976e-6f225b78ac0a"),
				Name = "Test"
			};

			fakeWebApi.CreateNewCategoryAsync(Arg.Any<Guid>(), expectedCategory.Name).Returns(expectedCategory);

			var response = await sut.PostAsync(Guid.Empty, new NewCategory { Name = expectedCategory.Name });
			var actual = response.Result as ObjectResult;

			Assert.That(actual, Is.Not.Null, nameof(response.Result));
			Assert.That(actual.StatusCode, Is.EqualTo((int)HttpStatusCode.Created), nameof(actual.StatusCode));
			Assert.That(actual.Value, Is.EqualTo(expectedCategory), nameof(actual.Value));
		}

		[Test]
		public async Task Post_WithoutAuthorizationToken_Returns401()
		{
			var response = await sut.PostAsync(null, new NewCategory());
			var actual = response.Result as UnauthorizedObjectResult;

			Assert.That(actual, Is.Not.Null, nameof(response.Result));
			Assert.That(actual.Value, Does.Contain("provide an authorization token").IgnoreCase, nameof(response.Value));
		}

		[Test]
		public async Task Post_WhenNewCategoryIsInvalid_Returns400()
		{
			const string invalidPropertyName = "Test";
			const string expectedErrorMessage = "Expected";

			sut.ModelState.AddModelError(invalidPropertyName, expectedErrorMessage);

			var response = await sut.PostAsync(Guid.Empty, new NewCategory());
			var actual = response.Result as BadRequestObjectResult;

			Assert.That(actual, Is.Not.Null, nameof(response.Result));

			var actualError = (SerializableError)actual.Value;

			Assert.That(actualError.Count, Is.EqualTo(sut.ModelState.ErrorCount), nameof(sut.ModelState.ErrorCount));
			Assert.That(actualError.TryGetValue(invalidPropertyName, out var actualErrorMessages), Is.True, "PropertyName");
			Assert.That(string.Join(string.Empty, (string[])actualErrorMessages), Is.EqualTo(expectedErrorMessage), "ErrorMessage");
		}

		[Test]
		public async Task Post_WhenKnownExceptionHappens_PassesItThrough()
		{
			var expectedException = new WebApiResponseException(HttpStatusCode.Unauthorized, "test");

			fakeWebApi.CreateNewCategoryAsync(Arg.Any<Guid>(), Arg.Any<string>()).Throws(expectedException);

			var response = await sut.PostAsync(Guid.Empty, new NewCategory());
			var actual = response.Result as ObjectResult;

			Assert.That(actual, Is.Not.Null, nameof(response.Result));
			Assert.That(actual.StatusCode, Is.EqualTo((int)expectedException.StatusCode), nameof(actual.StatusCode));
			Assert.That(actual.Value, Is.EqualTo(expectedException.Message), nameof(actual.Value));
		}

		[Test]
		public void Post_WhenUnknownExceptionHappens_ThrowsIt()
		{
			var expectedException = new Exception("test");

			fakeWebApi.CreateNewCategoryAsync(Arg.Any<Guid>(), Arg.Any<string>()).Throws(expectedException);

			var actualException = Assert.ThrowsAsync(
				expectedException.GetType(), async () => await sut.PostAsync(Guid.Empty, new NewCategory()), "Type");

			Assert.That(actualException.Message, Is.EqualTo(expectedException.Message), nameof(actualException.Message));
		}
	}
}