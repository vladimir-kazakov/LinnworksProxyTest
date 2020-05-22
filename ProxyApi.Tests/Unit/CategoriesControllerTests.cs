namespace ProxyApi.Tests.Unit
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Net;
	using System.Threading.Tasks;
	using Controllers;
	using Microsoft.AspNetCore.Mvc;
	using NSubstitute;
	using NSubstitute.ExceptionExtensions;
	using NUnit.Framework;
	using ProxyApi.Models;

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
		public async Task GetAsync_Normally_ReturnsAllCategories()
		{
			var expectedCategories = GetTestCategories();

			fakeWebApi.GetCategoriesWithProductsCountAsync(Arg.Any<Guid>()).Returns(expectedCategories);

			var actual = await sut.GetAsync(Guid.Empty);
			var actualResult = actual.Result as OkObjectResult;

			Assert.That(actualResult, Is.Not.Null, nameof(actual.Result));

			var actualValue = actualResult.Value as IEnumerable<Category>;

			Assert.That(actualValue, Is.Not.Null, nameof(actualResult.Value));
			Assert.That(actualValue.ToList().Count, Is.EqualTo(expectedCategories.Length), nameof(actualResult.Value));
		}

		[Test]
		public async Task GetAsync_WithoutToken_Returns401()
		{
			var actual = await sut.GetAsync();
			var actualResult = actual.Result as UnauthorizedObjectResult;

			Assert.That(actualResult, Is.Not.Null, nameof(actual.Result));

			var actualValue = actualResult.Value as string;

			Assert.That(actualValue, Does.Contain("provide an authorization token").IgnoreCase, nameof(actualResult.Value));
		}

		[Test]
		public async Task GetAsync_WhenKnownExceptionHappens_PassesItThrough()
		{
			var knownException = new WebApiResponseException(HttpStatusCode.Unauthorized, "test");

			fakeWebApi.GetCategoriesWithProductsCountAsync(Arg.Any<Guid>()).Throws(knownException);

			var actual = await sut.GetAsync(Guid.Empty);
			var actualResult = actual.Result as ObjectResult;

			Assert.That(actualResult, Is.Not.Null, nameof(actual.Result));
			Assert.That(actualResult.StatusCode, Is.EqualTo((int)knownException.StatusCode), nameof(actualResult.StatusCode));
			Assert.That(actualResult.Value, Is.EqualTo(knownException.Message), nameof(actualResult.Value));
		}

		[Test]
		public void GetAsync_WhenUnknownExceptionHappens_ThrowsIt()
		{
			var unknownException = new Exception("test");

			fakeWebApi.GetCategoriesWithProductsCountAsync(Arg.Any<Guid>()).Throws(unknownException);

			var ex = Assert.ThrowsAsync(unknownException.GetType(), async () => await sut.GetAsync(Guid.Empty), "Type");

			Assert.That(ex.Message, Is.EqualTo(unknownException.Message), nameof(ex.Message));
		}

		[Test]
		public async Task PostAsync_Normally_ReturnsCreatedCategory()
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
		public async Task PostAsync_WithoutToken_Returns401()
		{
			var response = await sut.PostAsync(null, new NewCategory { Name = "Test" });
			var actual = response.Result as UnauthorizedObjectResult;

			Assert.That(actual, Is.Not.Null, nameof(response.Result));
			Assert.That(actual.Value, Does.Contain("provide an authorization token").IgnoreCase, nameof(response.Value));
		}

		[Test]
		public async Task PostAsync_WhenNewCategoryIsInvalid_Returns400()
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
		public async Task PostAsync_WhenKnownExceptionHappens_PassesItThrough()
		{
			var knownException = new WebApiResponseException(HttpStatusCode.Unauthorized, "test");

			fakeWebApi.CreateNewCategoryAsync(Arg.Any<Guid>(), Arg.Any<string>()).Throws(knownException);

			var response = await sut.PostAsync(Guid.Empty, new NewCategory());
			var actual = response.Result as ObjectResult;

			Assert.That(actual, Is.Not.Null, nameof(response.Result));
			Assert.That(actual.StatusCode, Is.EqualTo((int)knownException.StatusCode), nameof(actual.StatusCode));
			Assert.That(actual.Value, Is.EqualTo(knownException.Message), nameof(actual.Value));
		}

		[Test]
		public void PostAsync_WhenUnknownExceptionHappens_ThrowsIt()
		{
			var unknownException = new Exception("test");

			fakeWebApi.CreateNewCategoryAsync(Arg.Any<Guid>(), Arg.Any<string>()).Throws(unknownException);

			var ex = Assert.ThrowsAsync(unknownException.GetType(), async () => await sut.PostAsync(Guid.Empty, new NewCategory()), "Type");

			Assert.That(ex.Message, Is.EqualTo(unknownException.Message), nameof(ex.Message));
		}
	}
}