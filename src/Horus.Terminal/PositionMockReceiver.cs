using Horus.Terminal.Models;

namespace Horus.Terminal
{
    class PositionMockReceiver : IPositionReceiver
    {
        readonly Random rand = new Random();
        readonly List<MockExchangeStream> second_fake_streams = new()
        {
            new MockExchangeStream("BINANCE FUTURES", "ETH", "USDT", 30000, 0.1),
        };
        readonly List<MockExchangeStream> minute_fake_streams = new()
        {
            new MockExchangeStream("EUREX", "FDAX", "EUR", 14000, 100),
            new MockExchangeStream("BINANCE SPOT", "IOTA", "USD", 0.4, 0.5),
            new MockExchangeStream("XETRA", "SIE", "EUR", 126, 50)
        };

        public Task ReceivePositions(Action<ClosedPosition> on_receive, CancellationToken token)
        {
            return Task.Run(() =>
            {
                ushort ticker = 0;
                while(!token.IsCancellationRequested)
                {
                    GenerateNextTicks(ref ticker);

                    var new_position = FakeMarketAction();

                    if(new_position.HasValue)
                        on_receive(new_position.Value);
                    
                    Task.Delay(1000).Wait();

                    ticker++;
                    if (ticker == 60)
                        ticker = 0;
                }
            }, token);
        }

        void GenerateNextTicks(ref ushort ticker)
        {
            foreach(var fake_stream in second_fake_streams)
                fake_stream.NextTick();

            if(ticker == 59)
            {
                foreach (var fake_stream in minute_fake_streams)
                    fake_stream.NextTick();
            }
        }

        ClosedPosition? FakeMarketAction()
        {
            var second_size = second_fake_streams.Count;
            var minute_size = minute_fake_streams.Count;

            var should_publish = rand.Next(1, 6) == 5;

            if (!should_publish)
                return null;

            var take_minute = rand.Next(1, 60) > 60 - Math.Floor(minute_size / (double)second_size + 1);

            if(take_minute)
            {
                var index = rand.Next(0, minute_size);
                var stream = minute_fake_streams.ElementAt(index);
                return stream.CloseMockPosition();
            }
            else
            {
                var index = rand.Next(0, second_size);
                var stream = second_fake_streams.ElementAt(index);
                return stream.CloseMockPosition();
            }
        }
    }
}
