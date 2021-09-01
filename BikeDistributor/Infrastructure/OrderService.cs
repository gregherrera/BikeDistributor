using BikeDistributor.Domain;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace BikeDistributor.Infrastructure
{
	public class OrderService
	{
		private readonly OrderContext context = null;

		public OrderService(OrderContext context)
		{
			this.context = context;
		}

		public void Add(Order order)
		{
			context.Orders.Add(order);
		}

		public void Delete(Order order)
		{
			context.Orders.Remove(order);
		}

		public Order FindById(int id)
		{
			return this.context.Orders.FirstOrDefault(x => x.OrderId == id);
		}

		public string TextReceipt(int id)
		{
			return context.Orders.FirstOrDefault(x => x.OrderId == id).Receipt();
		}

		public string HtmlReceipt(int id)
		{
			return context.Orders.FirstOrDefault(x => x.OrderId == id).HtmlReceipt();
		}

		public IQueryable<Order> GetAll()
		{
			return context.Orders.AsQueryable();
		}

		public IQueryable<Order> Find(Expression<Func<Order, bool>> predicate)
		{
			return context.Orders.AsQueryable().Where(predicate);
		}
	}
}
