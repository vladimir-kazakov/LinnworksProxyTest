﻿namespace ProxyApi.Tests.Unit
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

		private ILinnworksWebApi fakeWebApi;

		[SetUp]
		public void BeforeEachTest()
		{
			fakeWebApi = Substitute.For<ILinnworksWebApi>();

			var fakeWebApiFactory = Substitute.For<ILinnworksWebApiFactory>();

			fakeWebApiFactory.Create(Arg.Any<string>()).Returns(fakeWebApi);

			var claims = new[]
			{
				new Claim(ProxyClaimTypes.AuthenticationToken, "0d3ed01c-e2f5-471e-aa3d-40c40a520ed1")
			};

			var identify = new ClaimsIdentity(claims, "Token");

			sut = new CategoriesController(fakeWebApiFactory)
			{
				ControllerContext = new ControllerContext
				{
					HttpContext = new DefaultHttpContext
					{
						User = new ClaimsPrincipal(identify)
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
		public void Controller_RequiresAuthenticationToken()
		{
			var actual = sut.GetType().GetAttribute<RequireAuthenticationTokenAttribute>();

			Assert.That(actual, Is.Not.Null);
		}

		[Test]
		public void Controller_HandlesWebApiKnownExceptions()
		{
			var filters = sut.GetType().GetAttributes<ServiceFilterAttribute>();

			var actual = filters.Where(f => f.ServiceType == typeof(WebApiKnownExceptionHandler));

			Assert.That(actual.Count(), Is.EqualTo(1));
		}

		[Test]
		public async Task Get_Normally_ReturnsExistingCategories()
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
		public async Task Put_Normally_UpdatesExistingCategory()
		{
			var expected = new Entities.Category
			{
				CategoryId = Guid.Parse("96e2799e-e0e5-49e7-a44b-3858ce69d9f1"),
				CategoryName = "Updated"
			};

			var response = await sut.PutAsync(expected.CategoryId, new UpdatedCategory { Name = expected.CategoryName });
			var actual = response as NoContentResult;

			Assert.That(actual, Is.Not.Null);

			await fakeWebApi.Received(1).UpdateCategoryAsync(Arg.Is<Entities.Category>(actual =>
				actual.CategoryId == expected.CategoryId &&
				actual.CategoryName == expected.CategoryName));
		}

		[Test]
		public async Task Delete_Normally_DeletesExistingCategory()
		{
			var expectedCategoryId = Guid.Parse("4a34cf09-6711-4aea-ab92-8ddcd9edbd4c");

			var response = await sut.DeleteAsync(expectedCategoryId);
			var actual = response as NoContentResult;

			Assert.That(actual, Is.Not.Null);

			await fakeWebApi.Received(1).DeleteCategoryAsync(expectedCategoryId);
		}
	}
}