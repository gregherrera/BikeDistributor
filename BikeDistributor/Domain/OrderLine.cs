namespace BikeDistributor.Domain
{
    public class OrderLine
    {
		public OrderLine(Bike bike, int quantity)
        {
			Bike = bike;
            Quantity = quantity;
        }

		public Bike Bike { get; private set; }
        public int Quantity { get; private set; }
	}
}