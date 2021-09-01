using Moq;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;

namespace BikeDistributor.Helper
{
	public class MoqExtension
	{
		public static Mock<DbSet<T>> ToDbSet<T>(IQueryable<T> testData) where T : class
		{
			var mockSet = new Mock<DbSet<T>>();

			mockSet.As<IDbAsyncEnumerable<T>>()
				.Setup(x => x.GetAsyncEnumerator())
				.Returns(new TestDbAsyncEnumerator<T>(testData.GetEnumerator()));

			mockSet.As<IQueryable<T>>()
				.Setup(m => m.Provider)
				.Returns(new TestDbAsyncQueryProvider<T>(testData.Provider));

			mockSet.As<IQueryable<T>>().Setup(m => m.Expression).Returns(testData.Expression);
			mockSet.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(testData.ElementType);
			mockSet.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(testData.GetEnumerator());

			return mockSet;
		}
	}
}
