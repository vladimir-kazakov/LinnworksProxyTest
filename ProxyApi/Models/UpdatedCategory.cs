namespace ProxyApi.Models
{
	using System.ComponentModel.DataAnnotations;

	public class UpdatedCategory
	{
		[Required(AllowEmptyStrings = false)]
		public string Name { get; set; }
	}
}