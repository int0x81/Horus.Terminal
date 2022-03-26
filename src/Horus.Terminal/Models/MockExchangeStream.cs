namespace Horus.Terminal.Models
{
    class MockExchangeStream
    {
        readonly Random rand = new();

        readonly double _amount;
        readonly double _price_orientation;
        readonly double _lower_bound;
        readonly double _upper_bound;
        

        readonly string exchange_name;
        readonly string quote_name;
        readonly string currency_name;
        DateTime last_buy_date;
        double last_buy_in;
        double current_price;

        internal MockExchangeStream(string exchange, string quote, 
            string currency, double start_price, double amount)
        {
            _amount = amount;
            _price_orientation = start_price;
            _lower_bound = start_price * 0.5;
            _upper_bound = start_price * 1.5;
            exchange_name = exchange;
            quote_name = quote;
            currency_name = currency;
            last_buy_date = DateTime.UtcNow;
            last_buy_in = start_price;
        }

        internal void NextTick()
        {
            var price_change = rand.NextDouble() * (_price_orientation * 0.005);
            price_change += price_change * 1.5 - price_change;

            current_price += price_change;

            if (current_price < _lower_bound)
                current_price += Math.Abs(price_change * 1.5);

            if (current_price > _upper_bound)
                current_price += price_change * -1.5;

        }

        internal ClosedPosition CloseMockPosition()
        {
            var now = DateTime.UtcNow;
            var closed_position = new ClosedPosition()
            {
                ExchangeName = exchange_name,
                QuoteName = quote_name,
                Currency = currency_name,
                DateOfBuy = last_buy_date,
                DateOfSell = now,
                Amount = _amount,
                BuyPrice = last_buy_in,
                SellPrice = current_price
            };
            last_buy_date = now;
            last_buy_in = current_price;

            return closed_position;
        }
    }
}
