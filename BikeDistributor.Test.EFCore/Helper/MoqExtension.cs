using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace BikeDistributor.Helper
{
	public static class MoqExtension
	{
		public static DbSet<T> ToDbSet<T>(this List<T> sourceList) where T : class
		{
			var queryable = sourceList.AsQueryable();
			var dbSet = new Mock<DbSet<T>>();

			dbSet.As<IAsyncEnumerable<T>>()
				 .Setup(m => m.GetAsyncEnumerator(It.IsAny<CancellationToken>()))
				 .Returns(new MockAsyncEnumerator<T>(queryable.GetEnumerator()));

			dbSet.As<IQueryable<T>>()
				.Setup(m => m.Provider)
				.Returns(new MockAsyncQueryProvider<T>(queryable.Provider));

			dbSet.As<IQueryable<T>>().Setup(m => m.Expression).Returns(queryable.Expression);
			dbSet.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(queryable.ElementType);
			dbSet.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(queryable.GetEnumerator());
			return dbSet.Object;
		}
	}
}
