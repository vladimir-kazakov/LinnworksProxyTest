namespace ProxyApi.Tests.Unit
{
	using System;
	using System.Linq;
	using System.Net;
	using System.Security.Claims;
	using System.Threading.Tasks;
	using Castle.Core.Internal;
	using Controllers;
	using Microsoft.AspNetCore.Http;
	using Microsoft.AspNetCore.Mvc;
	using Models;
	using NSubstitute;
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

			var fakeWebApiFactory = Substitute.For<ILinnworksWebApiFactory>();

			fakeWebApiFactory.Create(Arg.Any<string>()).Returns(fakeWebApi);

			sut = new CategoriesController(fakeWebApiFactory)
			{
				ControllerContext = new ControllerContext
				{
					HttpContext = new DefaultHttpContext
					{
						User = new ClaimsPrincipal(new AuthenticationTokenIdentity("0d3ed01c-e2f5-471e-aa3d-40c40a520ed1"))
					}
				}
			};
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
		public void Controller_RequiresAuthentication()
		{
			var filters = sut.GetType().GetAttributes<ServiceFilterAttribute>();

			var actual = filters.Where(f => f.ServiceType == typeof(AuthenticationActionFilter));

			Assert.That(actual.Count(), Is.EqualTo(1));
		}

		[Test]
		public void Controller_HandlesWebApiKnownExceptions()
		{
			var filters = sut.GetType().GetAttributes<ServiceFilterAttribute>();

			var actual = filters.Where(f => f.ServiceType == typeof(WebApiKnownExceptionHandler));

			Assert.That(actual.Count(), Is.EqualTo(1));
		}

		[Test]
		public async Task Get_Normally_ReturnsAllCategories()
		{
			var expectedCategories = GetTestCategories();

			fakeWebApi.GetCategoriesWithProductsCountAsync().Returns(expectedCategories);

			var actual = await sut.GetAsync();

			Assert.That(actual.Length, Is.EqualTo(expectedCategories.Length), nameof(actual.Length));
		}

		[Test]
		public async Task Post_Normally_ReturnsCreatedCategory()
		{
			var expectedCategory = new Category
			{
				Id = Guid.Parse("4347ddb1-2fc6-4c41-976e-6f225b78ac0a"),
				Name = "Test"
			};

			fakeWebApi.CreateNewCategoryAsync(expectedCategory.Name).Returns(expectedCategory);

			var response = await sut.PostAsync(new NewCategory { Name = expectedCategory.Name });
			var actual = response.Result as ObjectResult;

			Assert.That(actual, Is.Not.Null, nameof(response.Result));
			Assert.That(actual.StatusCode, Is.EqualTo((int)HttpStatusCode.Created), nameof(actual.StatusCode));
			Assert.That(actual.Value, Is.EqualTo(expectedCategory), nameof(actual.Value));
		}

		[Test]
		public async Task Post_WhenNewCategoryIsInvalid_Returns400()
		{
			const string invalidPropertyName = "Test";
			const string expectedErrorMessage = "Expected";

			sut.ModelState.AddModelError(invalidPropertyName, expectedErrorMessage);

			var response = await sut.PostAsync(new NewCategory());
			var actual = response.Result as BadRequestObjectResult;

			Assert.That(actual, Is.Not.Null, nameof(response.Result));

			var actualError = (SerializableError)actual.Value;

			Assert.That(actualError.Count, Is.EqualTo(sut.ModelState.ErrorCount), nameof(sut.ModelState.ErrorCount));
			Assert.That(actualError.TryGetValue(invalidPropertyName, out var actualErrorMessages), Is.True, "PropertyName");
			Assert.That(string.Join(string.Empty, (string[])actualErrorMessages), Is.EqualTo(expectedErrorMessage), "ErrorMessage");
		}
	}
}