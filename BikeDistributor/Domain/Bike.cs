namespace BikeDistributor.Domain
{
    public class Bike
    {
        public const int OneThousand = 1000;
        public const int TwoThousand = 2000;
        public const int FiveThousand = 5000;

		#region New Prices
		//Extended to suply more bikes at new prices
		public const int ThreeThousand = 3000;
		public const int FourThousand = 4000;
		#endregion

		public Bike(string brand, string model, int price)
		{
			Brand = brand;
			Model = model;
			Price = price;
		}

		public string Brand { get; private set; }
		public string Model { get; private set; }
		public int Price { get; private set; }
	}
}
