﻿using Microsoft.EntityFrameworkCore.Query;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace BikeDistributor.Helper
{
	internal class MockAsyncQueryProvider<TEntity> : IAsyncQueryProvider
	{
		private readonly IQueryProvider _inner;

		internal MockAsyncQueryProvider(IQueryProvider inner)
		{
			_inner = inner;
		}

		public IQueryable CreateQuery(Expression expression)
		{
			return new MockAsyncEnumerable<TEntity>(expression);
		}

		public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
		{
			return new MockAsyncEnumerable<TElement>(expression);
		}

		public object Execute(Expression expression)
		{
			return _inner.Execute(expression);
		}

		public TResult Execute<TResult>(Expression expression)
		{
			return _inner.Execute<TResult>(expression);
		}

		public TResult ExecuteAsync<TResult>(Expression expression, CancellationToken cancellationToken = default)
		{
			var expectedResultType = typeof(TResult).GetGenericArguments()[0];
			var executionResult = typeof(IQueryProvider)
					 .GetMethod(name: nameof(IQueryProvider.Execute), genericParameterCount: 1, types: new[] { typeof(Expression) })
					 .MakeGenericMethod(expectedResultType)
					 .Invoke(this, new[] { expression });

			return (TResult)typeof(Task)
				.GetMethod(nameof(Task.FromResult))
				?.MakeGenericMethod(expectedResultType)
				.Invoke(null, new[] { executionResult });
		}
	}
	internal class MockAsyncEnumerable<T> : EnumerableQuery<T>, IAsyncEnumerable<T>, IQueryable<T>
	{
		IQueryProvider IQueryable.Provider => new MockAsyncQueryProvider<T>(this);

		public MockAsyncEnumerable(IEnumerable<T> enumerable) : base(enumerable) { }
		public MockAsyncEnumerable(Expression expression) : base(expression) { }

		public IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default)
		{
			return new MockAsyncEnumerator<T>(this.AsEnumerable().GetEnumerator());
		}
	}
	internal class MockAsyncEnumerator<T> : IAsyncEnumerator<T>
	{
		private readonly IEnumerator<T> _inner;

		public T Current => _inner.Current;

		public MockAsyncEnumerator(IEnumerator<T> inner)
		{
			_inner = inner;
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