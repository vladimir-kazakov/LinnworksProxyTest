namespace ProxyApi
{
	public interface ILinnworksWebApiFactory
	{
		IWebApi Create(string authenticationToken);
	}
}