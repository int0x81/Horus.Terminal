namespace Horus.Terminal.Models
{
    class MockExchangeStream
    {
        readonly Random rand = new();

        readonly string agent_id = Guid.NewGuid().ToString().Substring(0,4);
        readonly double fixed_amount;
        readonly double max_range;
        readonly double lower_bound;
        readonly double upper_bound;

        readonly string exchange_name;
        readonly string quote_name;
        readonly string currency_name;
        DateTime last_buy_date;
        double last_buy_in;
        double current_price;

        internal MockExchangeStream(string exchange, string quote, 
            string currency, double start_price, double amount)
        {
            fixed_amount = amount;
            max_range = start_price * 0.005;
            lower_bound = start_price * 0.5;
            upper_bound = start_price * 1.5;
            exchange_name = exchange;
            quote_name = quote;
            currency_name = currency;
            last_buy_date = DateTime.UtcNow;
            last_buy_in = start_price;
            current_price = start_price;
        }

        internal void NextTick()
        {
            var level = rand.NextDouble();
            var direction = rand.NextDouble() < 0.5;
            var price_change = direction ? level * max_range : level * max_range * -1;

            current_price += price_change;

            if (current_price < lower_bound)
                current_price += Math.Abs(price_change * 1.5);

            if (current_price > upper_bound)
                current_price += price_change * -1.5;

        }

        internal ClosedPosition CloseMockPosition()
        {
            var now = DateTime.UtcNow;
            var closed_position = new ClosedPosition()
            {
                AgentId = agent_id,
                ExchangeName = exchange_name,
                QuoteName = quote_name,
                Currency = currency_name,
                DateOfBuy = last_buy_date,
                DateOfSell = now,
                Amount = fixed_amount,
                BuyPrice = last_buy_in,
                SellPrice = current_price
            };
            last_buy_date = now;
            last_buy_in = current_price;

            return closed_position;
        }
    }
}
