namespace ProxyApi
{
	using Microsoft.AspNetCore.Mvc;
	using Microsoft.AspNetCore.Mvc.Filters;

	public class WebApiKnownExceptionHandler : IExceptionFilter
	{
		public void OnException(ExceptionContext context)
		{
			if (context.Exception is WebApiKnownException ex)
			{
				context.Result = new ObjectResult(ex.Message)
				{
					StatusCode = (int)ex.StatusCode
				};
			}
		}
	}
}