namespace ProxyApi
{
	using System;
	using System.Net;
	using System.Security.Claims;
	using System.Threading.Tasks;
	using Microsoft.AspNetCore.Mvc;
	using Microsoft.AspNetCore.Mvc.Filters;

	public class AuthenticationActionFilter : IAsyncActionFilter
	{
		public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
		{
			if (!context.HttpContext.Request.Headers.TryGetValue(HttpHeaderName.Authorization, out var values))
			{
				context.Result = new UnauthorizedObjectResult(
					"To access this web API, provide an authentication token " +
					$"(a GUID in the {Guid.Empty} format) " +
					$"through the {HttpHeaderName.Authorization} HTTP request header.");

				return;
			}

			var authenticationToken = string.Join(string.Empty, values);

			if (!Guid.TryParse(authenticationToken, out _))
			{
				context.Result = new ObjectResult(
					$"The value of the {HttpHeaderName.Authorization} HTTP request header " +
					$"does not represent a valid authentication token (a GUID in the {Guid.Empty} format).")
				{
					StatusCode = (int)HttpStatusCode.BadRequest
				};

				return;
			}

			context.HttpContext.User = new ClaimsPrincipal(new AuthenticationTokenIdentity(authenticationToken));

			await next();
		}
	}
}