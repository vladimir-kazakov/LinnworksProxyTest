namespace ProxyApi
{
	using System;
	using System.Net;

	public class WebApiKnownException : Exception
	{
		public HttpStatusCode StatusCode { get; }

		public WebApiKnownException(HttpStatusCode statusCode, string message) : base(message)
		{
			StatusCode = statusCode;
		}
	}
}