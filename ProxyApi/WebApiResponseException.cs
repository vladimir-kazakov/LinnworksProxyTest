namespace ProxyApi
{
	using System;
	using System.Net;

	public class WebApiResponseException : Exception
	{
		public HttpStatusCode StatusCode { get; }

		public WebApiResponseException(HttpStatusCode statusCode, string message) : base(message)
		{
			StatusCode = statusCode;
		}
	}
}