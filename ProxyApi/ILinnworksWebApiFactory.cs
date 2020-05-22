namespace ProxyApi
{
	public interface ILinnworksWebApiFactory
	{
		ILinnworksWebApi Create(string authenticationToken);
	}
}