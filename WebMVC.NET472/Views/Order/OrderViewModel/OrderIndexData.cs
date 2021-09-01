using System.Collections.Generic;

namespace WebMVC.NET472.Views.Order.OrderViewModel
{
	public class OrderIndexData
	{
		public IList<BikeDistributor.Domain.Order> Orders { get; set; }
		public IList<BikeDistributor.Domain.OrderLine> Lines { get; set; }
		public string Receipt { get; set; }
	}
}