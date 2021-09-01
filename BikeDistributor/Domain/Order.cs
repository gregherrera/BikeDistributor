using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BikeDistributor.Domain
{
    public class Order
    {
        private const double TaxRate = .0725d;

		//Extending Percentages
		private const double TenPercentage = .10d;
		private const double NinePercentage = .9d;
		private const double EightPercentage = .8d;
		private const double SevenPercentage = .7d;
		private const double SixPercentage = .6d;

		public Order(int orderId, string company)
        {
            Company = company;
            OrderId = orderId;
			Lines = new List<OrderLine>();
		}

        public string Company { get; private set; }

        public IList<OrderLine> Lines { get; private set; } 

		public int OrderId { get; private set; }

        public void AddLine(OrderLine line)
        {
            Lines.Add(line);
        }

		public string Receipt()
		{
			var totalAmount = 0d;
			var result = new StringBuilder($"Order Receipt for {Company}{Environment.NewLine}");

			foreach (var line in Lines)
			{
				var thisAmount = 0d;
				switch (line.Bike.Price)
				{
					case Bike.OneThousand:
						if (line.Quantity >= 25)
							thisAmount += line.Quantity * line.Bike.Price * TenPercentage;
						else
							thisAmount += line.Quantity * line.Bike.Price;
						break;
					case Bike.TwoThousand:
						if (line.Quantity >= 20)
							thisAmount += line.Quantity * line.Bike.Price * NinePercentage;
						else
							thisAmount += line.Quantity * line.Bike.Price;
						break;
					case Bike.ThreeThousand:
						if (line.Quantity >= 15)
							thisAmount += line.Quantity * line.Bike.Price * EightPercentage;
						else
							thisAmount += line.Quantity * line.Bike.Price;
						break;
					case Bike.FourThousand:
						if (line.Quantity >= 10)
							thisAmount += line.Quantity * line.Bike.Price * SevenPercentage;
						else
							thisAmount += line.Quantity * line.Bike.Price;
						break;
					case Bike.FiveThousand:
						if (line.Quantity >= 5)
							thisAmount += line.Quantity * line.Bike.Price * SixPercentage;
						else
							thisAmount += line.Quantity * line.Bike.Price;
						break;
				}
				result.AppendLine($"\t{line.Quantity} x {line.Bike.Brand} {line.Bike.Model} = {thisAmount:C}");
				totalAmount += thisAmount;
			}
			result.AppendLine($"Sub-Total: {totalAmount:C}");
			var tax = totalAmount * TaxRate;
			result.AppendLine($"Tax: {tax:C}");
			result.Append($"Total: {totalAmount + tax:C}");
			return result.ToString();
		}

		public string HtmlReceipt()
		{
			var totalAmount = 0d;
			var result = new StringBuilder($"<html><body><h1>Order Receipt for {Company}</h1>");
			if (Lines.Count > 0)
			{
				result.Append("<ul>");
				foreach (var line in Lines)
				{
					var thisAmount = 0d;
					switch (line.Bike.Price)
					{
						case Bike.OneThousand:
							if (line.Quantity >= 25)
								thisAmount += line.Quantity * line.Bike.Price * TenPercentage;
							else
								thisAmount += line.Quantity * line.Bike.Price;
							break;
						case Bike.TwoThousand:
							if (line.Quantity >= 20)
								thisAmount += line.Quantity * line.Bike.Price * NinePercentage;
							else
								thisAmount += line.Quantity * line.Bike.Price;
							break;
						case Bike.ThreeThousand:
							if (line.Quantity >= 15)
								thisAmount += line.Quantity * line.Bike.Price * EightPercentage;
							else
								thisAmount += line.Quantity * line.Bike.Price;
							break;
						case Bike.FourThousand:
							if (line.Quantity >= 10)
								thisAmount += line.Quantity * line.Bike.Price * SevenPercentage;
							else
								thisAmount += line.Quantity * line.Bike.Price;
							break;
						case Bike.FiveThousand:
							if (line.Quantity >= 5)
								thisAmount += line.Quantity * line.Bike.Price * SixPercentage;
							else
								thisAmount += line.Quantity * line.Bike.Price;
							break;
					}
					result.Append($"<li>{line.Quantity} x {line.Bike.Brand} {line.Bike.Model} = {thisAmount:C}</li>");
					totalAmount += thisAmount;
				}
				result.Append("</ul>");
			}
			result.Append($"<h3>Sub-Total: {totalAmount:C}</h3>");
			var tax = totalAmount * TaxRate;
			result.Append($"<h3>Tax: {tax:C}</h3>");
			result.Append($"<h2>Total: {totalAmount + tax:C}</h2>");
			result.Append("</body></html>");
			return result.ToString();
		}
	}
}
