using System.Linq;

namespace BikeDistributor.Infrastructure
{
	public interface IUnitOfWork
	{
		IQueryable<T> Query<T>();
		OrderContext Context { get; }
		OrderService OrderService();
	}

	public class UnitOfWork : IUnitOfWork
	{
		public UnitOfWork()
		{
			context = new OrderContext();
		}

		public UnitOfWork(OrderContext context)
		{
			this.context = context;
		}

		public IQueryable<T> Query<T>()
		{
			//You do not need to implement this method for the coding exercise
			throw new System.NotImplementedException();
		}

		public OrderService OrderService()
		{
			if (_orderService == null)
			{
				_orderService = new OrderService(context);
			}
			return _orderService;
		}
		/// <summary>
		/// Context property to gain access to the order DbSet to test using Moq
		/// </summary>
		public OrderContext Context
		{
			get { return context; }
		}

		OrderContext context = null;
		OrderService _orderService = null;
	}
}
