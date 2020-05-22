namespace ProxyApi
{
	using System.Security.Principal;

	public class AuthenticationTokenIdentity : IIdentity
	{
		public string AuthenticationType => "Token";

		public bool IsAuthenticated => !string.IsNullOrWhiteSpace(Name);

		public string Name { get; }

		public AuthenticationTokenIdentity(string token)
		{
			Name = token;
		}
	}
}