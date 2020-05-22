namespace ProxyApi.Models
{
	using System.ComponentModel.DataAnnotations;

	public class NewCategory
	{
		[Required(AllowEmptyStrings = false)]
		public string Name { get; set; }
	}
}