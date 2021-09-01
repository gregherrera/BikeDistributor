using BikeDistributor.Domain;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace BikeDistributor.Infrastructure
{
	//This class is used with In-Memory Database (Enumerable, List)
	//It is provided to be used for all the job required by this project
	//It has its own implementation of IDbAsyncEnumerable depending of the target framework: NET472 or NETCOREAPP3_1

#if NET472
	using System.Data.Entity;
	using System.Data.Entity.Infrastructure;

	public class OrderContext : IOrderContext
	{
		public OrderContext()
		{
			this.Orders = new TestDbSet<Order>();
		}

		public virtual DbSet<Order> Orders { get; set; }
	}

	public class TestDbSet<TEntity> : DbSet<TEntity>, IQueryable<TEntity>, IEnumerable<TEntity>, IDbAsyncEnumerable<TEntity>
			where TEntity : class
	{
		ObservableCollection<TEntity> _data;
		IQueryable _query;

		public TestDbSet()
		{
			_data = new ObservableCollection<TEntity>();
			_query = _data.AsQueryable();
		}

		public override TEntity Add(TEntity item)
		{
			_data.Add(item);
			return item;
		}

		public override TEntity Remove(TEntity item)
		{
			_data.Remove(item);
			return item;
		}

		public override TEntity Attach(TEntity item)
		{
			_data.Add(item);
			return item;
		}

		public override TEntity Create()
		{
			return Activator.CreateInstance<TEntity>();
		}

		public override TDerivedEntity Create<TDerivedEntity>()
		{
			return Activator.CreateInstance<TDerivedEntity>();
		}

		public override ObservableCollection<TEntity> Local
		{
			get { return _data; }
		}

		Type IQueryable.ElementType
		{
			get { return _query.ElementType; }
		}

		Expression IQueryable.Expression
		{
			get { return _query.Expression; }
		}

		IQueryProvider IQueryable.Provider
		{
			get { return new TestDbAsyncQueryProvider<TEntity>(_query.Provider); }
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return _data.GetEnumerator();
		}

		IEnumerator<TEntity> IEnumerable<TEntity>.GetEnumerator()
		{
			return _data.GetEnumerator();
		}

		IDbAsyncEnumerator<TEntity> IDbAsyncEnumerable<TEntity>.GetAsyncEnumerator()
		{
			return new TestDbAsyncEnumerator<TEntity>(_data.GetEnumerator());
		}
	}

	internal class TestDbAsyncQueryProvider<TEntity> : IDbAsyncQueryProvider
	{
		private readonly IQueryProvider _inner;

		internal TestDbAsyncQueryProvider(IQueryProvider inner)
		{
			_inner = inner;
		}

		public IQueryable CreateQuery(Expression expression)
		{
			return new TestDbAsyncEnumerable<TEntity>(expression);
		}

		public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
		{
			return new TestDbAsyncEnumerable<TElement>(expression);
		}

		public object Execute(Expression expression)
		{
			return _inner.Execute(expression);
		}

		public TResult Execute<TResult>(Expression expression)
		{
			return _inner.Execute<TResult>(expression);
		}

		public Task<object> ExecuteAsync(Expression expression, CancellationToken cancellationToken)
		{
			return Task.FromResult(Execute(expression));
		}

		public Task<TResult> ExecuteAsync<TResult>(Expression expression, CancellationToken cancellationToken)
		{
			return Task.FromResult(Execute<TResult>(expression));
		}
	}

	internal class TestDbAsyncEnumerable<T> : EnumerableQuery<T>, IDbAsyncEnumerable<T>, IQueryable<T>
	{
		public TestDbAsyncEnumerable(IEnumerable<T> enumerable)
			: base(enumerable)
		{ }

		public TestDbAsyncEnumerable(Expression expression)
			: base(expression)
		{ }

		public IDbAsyncEnumerator<T> GetAsyncEnumerator()
		{
			return new TestDbAsyncEnumerator<T>(this.AsEnumerable().GetEnumerator());
		}

		IDbAsyncEnumerator IDbAsyncEnumerable.GetAsyncEnumerator()
		{
			return GetAsyncEnumerator();
		}

		IQueryProvider IQueryable.Provider
		{
			get { return new TestDbAsyncQueryProvider<T>(this); }
		}
	}

	internal class TestDbAsyncEnumerator<T> : IDbAsyncEnumerator<T>
	{
		private readonly IEnumerator<T> _inner;

		public TestDbAsyncEnumerator(IEnumerator<T> inner)
		{
			_inner = inner;
		}

		public void Dispose()
		{
			_inner.Dispose();
		}

		public Task<bool> MoveNextAsync(CancellationToken cancellationToken)
		{
			return Task.FromResult(_inner.MoveNext());
		}

		public T Current
		{
			get { return _inner.Current; }
		}

		object IDbAsyncEnumerator.Current
		{
			get { return Current; }
		}
	}
}
#elif NETCOREAPP3_1
	using Microsoft.EntityFrameworkCore;
	using Microsoft.EntityFrameworkCore.Query;
	using Microsoft.EntityFrameworkCore.ChangeTracking;
	public class OrderContext : IOrderContext
	{
		public OrderContext()
		{
			this.Orders = new TestDbSet<Order>();
		}

		public virtual DbSet<Order> Orders { get; set; }
	}

	public class TestDbSet<TEntity> : DbSet<TEntity>, IQueryable<TEntity>, IEnumerable<TEntity>, IAsyncEnumerable<TEntity>
			where TEntity : class
	{
		ObservableCollection<TEntity> _data;
		IQueryable _query;

		public TestDbSet()
		{
			_data = new ObservableCollection<TEntity>();
			_query = _data.AsQueryable();
		}

		public override EntityEntry<TEntity> Add(TEntity entity)
		{
			_data.Add(entity);

			return null;
		}

		public override EntityEntry<TEntity> Remove(TEntity item)
		{
			_data.Remove(item);

			return null;
		}

		public override EntityEntry<TEntity> Attach(TEntity item)
		{
			_data.Add(item);

			return null;
		}

		Type IQueryable.ElementType
		{
			get { return _query.ElementType; }
		}

		Expression IQueryable.Expression
		{
			get { return _query.Expression; }
		}

		IQueryProvider IQueryable.Provider
		{
			get { return new TestAsyncQueryProvider<TEntity>(_query.Provider); }
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return _data.GetEnumerator();
		}

		IEnumerator<TEntity> IEnumerable<TEntity>.GetEnumerator()
		{
			return _data.GetEnumerator();
		}

		public IAsyncEnumerator<TEntity> GetAsyncEnumerator(CancellationToken cancellationToken = default)
		{
			return new TestAsyncEnumerator<TEntity>(this.AsEnumerable().GetEnumerator());
		}

		IAsyncEnumerator<TEntity> IAsyncEnumerable<TEntity>.GetAsyncEnumerator(CancellationToken cancellationToken)
		{
			return GetAsyncEnumerator(cancellationToken);
		}
	}

	internal class TestAsyncQueryProvider<TEntity> : IAsyncQueryProvider
	{
		private readonly IQueryProvider _inner;

		internal TestAsyncQueryProvider(IQueryProvider inner)
		{
			_inner = inner;
		}

		public IQueryable CreateQuery(Expression expression)
		{
			return new TestAsyncEnumerable<TEntity>(expression);
		}

		public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
		{
			return new TestAsyncEnumerable<TElement>(expression);
		}

		public object Execute(Expression expression)
		{
			return _inner.Execute(expression);
		}

		public TResult Execute<TResult>(Expression expression)
		{
			return _inner.Execute<TResult>(expression);
		}

		public IAsyncEnumerable<TResult> ExecuteAsync<TResult>(Expression expression)
		{
			return new TestAsyncEnumerable<TResult>(expression);
		}

		public TResult ExecuteAsync<TResult>(Expression expression, CancellationToken cancellationToken)
		{
			return Execute<TResult>(expression);
		}
	}

	// Async enumerable for unit testing
	internal class TestAsyncEnumerable<T> : EnumerableQuery<T>, IAsyncEnumerable<T>, IQueryable<T>
	{
		public TestAsyncEnumerable(IEnumerable<T> enumerable)
			: base(enumerable)
		{ }

		public TestAsyncEnumerable(Expression expression)
			: base(expression)
		{ }

		public IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default)
		{
			return new TestAsyncEnumerator<T>(this.AsEnumerable().GetEnumerator());
		}

		IAsyncEnumerator<T> IAsyncEnumerable<T>.GetAsyncEnumerator(CancellationToken cancellationToken)
		{
			return GetAsyncEnumerator(cancellationToken);
		}

		IQueryProvider IQueryable.Provider
		{
			get { return new TestAsyncQueryProvider<T>(this); }
		}
	}

	// Async enumerator for unit testing
	internal class TestAsyncEnumerator<T> : IAsyncEnumerator<T>
	{
		private readonly IEnumerator<T> _inner;

		public TestAsyncEnumerator(IEnumerator<T> inner)
		{
			_inner = inner;
		}

		public T Current
		{
			get
			{
				return _inner.Current;
			}
		}

		public ValueTask<bool> MoveNextAsync()
		{
			return new ValueTask<bool>(_inner.MoveNext());
		}

		public ValueTask DisposeAsync()
		{
			_inner.Dispose();
			return new ValueTask();
		}
	}
}
#endif