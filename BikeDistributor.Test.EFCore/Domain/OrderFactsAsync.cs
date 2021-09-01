using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using BikeDistributor.Infrastructure;
using BikeDistributor.Helper;
using Moq.EntityFrameworkCore;
using Moq;

namespace BikeDistributor.Domain
{
	public class OrderFactsAsync
	{
		private static readonly Bike Defy = new Bike("Giant", "Defy 1", Bike.OneThousand);
		private static readonly Bike Elite = new Bike("Specialized", "Venge Elite", Bike.TwoThousand);
		private static readonly Bike DuraAce = new Bike("Specialized", "S-Works Venge Dura-Ace", Bike.FiveThousand);
		private static readonly Bike Arktos = new Bike("Alchemy", "Arktos", Bike.FourThousand);
		private static readonly Bike ZigZag = new Bike("All City", "Zig Zag 105", Bike.ThreeThousand);
		private static readonly Bike Infinito = new Bike("Bianchi", "Infinito CV", Bike.TwoThousand);

		[Fact]
		public async Task Get_All_Orders_By_Mountain_Bike_Shop_Company_Name_Async()
		{
			#region Arrange
			//Arrange
			List<Order> orderList = new List<Order>();

			//Instance first order to Mountain Bike Shop company
			Order order = new Order(1, "Mountain Bike Shop");
			order.AddLine(new OrderLine(Defy, 2));
			order.AddLine(new OrderLine(Infinito, 1));
			orderList.Add(order);			

			//Instance second order to Mountain Bike Shop
			order = new Order(2, "Mountain Bike Shop");
			order.AddLine(new OrderLine(Elite, 3));
			order.AddLine(new OrderLine(Arktos, 2));
			orderList.Add(order);

			//Instance first order to Valley Bike Shop
			order = new Order(3, "Valley Bike Shop");
			order.AddLine(new OrderLine(ZigZag, 3));
			orderList.Add(order);

			//Instance third order to Valley Bike Shop
			order = new Order(4, "Mountain Bike Shop");
			order.AddLine(new OrderLine(ZigZag, 1));
			orderList.Add(order);

			//Instance first order to Valley Bike Shop
			order = new Order(5, "Rock Bike Shop");
			order.AddLine(new OrderLine(DuraAce, 5));
			orderList.Add(order);
			#endregion

			#region Act
			var mockSet = MoqExtension.ToDbSet<Order>(orderList);

			var mockContext = new Mock<OrderContext>();
			mockContext.Setup(c => c.Orders).Returns(mockSet);

			var Uow = new UnitOfWork(mockContext.Object);

			var MountainBikeShopOrders = await Uow.Context.Orders.Where(x => x.Company.Equals("Mountain Bike Shop")).ToListAsync();
			#endregion

			#region Assert
			//Assert
			//This time I'll use FluentAssertions.Should
			MountainBikeShopOrders.Should().HaveCount(3);
			#endregion
		}

		[Fact]
		public async Task Get_Order_With_OrderId_Equal_Four_Async()
		{
			#region Assert
			//Arrange
			List<Order> orderList = new List<Order>();

			Order order = new Order(1, "Mountain Bike Shop");
			order.AddLine(new OrderLine(Defy, 2));
			order.AddLine(new OrderLine(Infinito, 1));
			orderList.Add(order);

			order = new Order(2, "Beach Bike Shop");
			order.AddLine(new OrderLine(Elite, 3));
			order.AddLine(new OrderLine(Arktos, 2));
			orderList.Add(order);

			order = new Order(3, "Valley Bike Shop");
			order.AddLine(new OrderLine(ZigZag, 3));
			orderList.Add(order);

			order = new Order(4, "Alabama Bike Shop");
			order.AddLine(new OrderLine(ZigZag, 1));
			orderList.Add(order);

			order = new Order(5, "Arkansas Bike Shop");
			order.AddLine(new OrderLine(DuraAce, 5));
			orderList.Add(order);
			#endregion

			#region Act
			//Act
			var mockSet = MoqExtension.ToDbSet<Order>(orderList);

			var mockContext = new Mock<OrderContext>();
			mockContext.Setup(c => c.Orders).Returns(mockSet);

			var Uow = new UnitOfWork(mockContext.Object);

			var AlabamaBikeShopOrder = await Uow.Context.Orders.Where(x => x.OrderId == 4).FirstOrDefaultAsync();
			#endregion

			#region Assert
			//Assert
			//This time I'll use XUnit.Assert
			Assert.Equal("Alabama Bike Shop", AlabamaBikeShopOrder.Company);
			#endregion
		}

		[Fact]
		public async Task Get_TextReceipt_With_SixPercent_Async()
		{
			#region Assert
			//Arrange
			const string ResultStatementSixPercent = @"Order Receipt for Alabama Bike Shop
	5 x Specialized S-Works Venge Dura-Ace = $15,000.00
Sub-Total: $15,000.00
Tax: $1,087.50
Total: $16,087.50";

			List<Order> orderList = new List<Order>();

			Order order = new Order(1, "Alabama Bike Shop");
			order.AddLine(new OrderLine(DuraAce, 5));
			orderList.Add(order);
			#endregion

			#region Act
			//Act
			var mockSet = MoqExtension.ToDbSet<Order>(orderList);

			var mockContext = new Mock<OrderContext>();
			mockContext.Setup(c => c.Orders).Returns(mockSet);

			var Uow = new UnitOfWork(mockContext.Object);

			var AlabamaBikeShopOrder = await Uow.Context.Orders.Where(x => x.OrderId == 1).FirstOrDefaultAsync();

			string textReceipt = AlabamaBikeShopOrder.Receipt();
			#endregion

			#region Assert
			//Assert
			//This time I'll use FluentAssertions.Should
			textReceipt.Should().Be(ResultStatementSixPercent);
			#endregion
		}

		[Fact]
		public async Task Get_TextRecipt_For_OrderId_Equal_Three_Async()
		{
			#region Arrange
			//Arrange
			const string ResultStatementValleyBikeShopTextReceipt = @"Order Receipt for Valley Bike Shop
	3 x All City Zig Zag 105 = $9,000.00
Sub-Total: $9,000.00
Tax: $652.50
Total: $9,652.50";

			List<Order> orderList = new List<Order>();

			Order order = new Order(1, "Mountain Bike Shop");
			order.AddLine(new OrderLine(Defy, 2));
			order.AddLine(new OrderLine(Infinito, 1));

			orderList.Add(order);

			order = new Order(2, "Beach Bike Shop");
			order.AddLine(new OrderLine(Elite, 3));
			order.AddLine(new OrderLine(Arktos, 2));
			orderList.Add(order);

			order = new Order(3, "Valley Bike Shop");
			order.AddLine(new OrderLine(ZigZag, 3));
			orderList.Add(order);

			order = new Order(4, "Alabama Bike Shop");
			order.AddLine(new OrderLine(ZigZag, 1));
			orderList.Add(order);

			order = new Order(5, "Arkansas Bike Shop");
			order.AddLine(new OrderLine(DuraAce, 5));
			orderList.Add(order);
			#endregion

			#region Act
			//Act
			var mockSet = MoqExtension.ToDbSet<Order>(orderList);

			var mockContext = new Mock<OrderContext>();
			mockContext.Setup(c => c.Orders).Returns(mockSet);

			var Uow = new UnitOfWork(mockContext.Object);

			var ValleyBikeShopTextReceipt = await Uow.Context.Orders.Where(x => x.OrderId == 3).FirstOrDefaultAsync();

			string textReceipt = ValleyBikeShopTextReceipt.Receipt();
			#endregion

			#region Assert
			//Assert
			//This time I'll use XUnit.Assert
			Assert.Equal(ResultStatementValleyBikeShopTextReceipt, textReceipt);
			#endregion
		}

		[Fact]
		public async Task Get_HtmlReceipt_For_Arktos_Bike_Async()
		{
			#region Arrange
			//Arrange			
			const string ResultStatementHtmlReceipt = "<html><body><h1>Order Receipt for Miami Bike Shop</h1><ul><li>8 x Alchemy Arktos = $32,000.00</li></ul><h3>Sub-Total: $32,000.00</h3><h3>Tax: $2,320.00</h3><h2>Total: $34,320.00</h2></body></html>";
			
			List<Order> orderList = new List<Order>();

			Order order = new Order(1, "Miami Bike Shop");
			order.AddLine(new OrderLine(Arktos, 8));
			orderList.Add(order);

			//To convert the data to IQueryable to execute Async actions
			var data = orderList.AsQueryable();
			#endregion

			#region Act
			//Act
			var mockSet = MoqExtension.ToDbSet<Order>(orderList);

			var mockContext = new Mock<OrderContext>();
			mockContext.Setup(c => c.Orders).Returns(mockSet);

			var Uow = new UnitOfWork(mockContext.Object);

			var ValleyBikeShopTextReceipt = await Uow.Context.Orders.Where(x => x.OrderId == 1).FirstOrDefaultAsync();

			string textReceipt = ValleyBikeShopTextReceipt.Receipt();

			string HtmlReceipt = ValleyBikeShopTextReceipt.HtmlReceipt();
			#endregion

			#region Assert
			//Assert
			//This time I'll use FluentAssertions.Should
			HtmlReceipt.Should().Be(ResultStatementHtmlReceipt);
			#endregion
		}
	}
}
