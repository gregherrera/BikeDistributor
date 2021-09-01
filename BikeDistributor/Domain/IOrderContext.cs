#if NET472
	using System.Data.Entity;
#elif NETCOREAPP3_1
	using Microsoft.EntityFrameworkCore;
#endif

namespace BikeDistributor.Domain
{
	/// <summary>
	/// This interface is for implement a particular DbSet version in the context class
	/// </summary>
	public interface IOrderContext
	{
		DbSet<Order> Orders { get; set; }
	}
}
