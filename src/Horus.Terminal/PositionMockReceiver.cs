using Horus.Terminal.Models;

namespace Horus.Terminal
{
    class PositionMockReceiver : IPositionReceiver
    {
        readonly Queue<ClosedPosition> positions = new Queue<ClosedPosition>();
        readonly Random rand = new Random();
        readonly List<MockExchangeStream> second_fake_streams = new List<MockExchangeStream>
        {
            new MockExchangeStream("BINANCE FUTURES", "BTC", "USD", 30000, 0.1)
        };
        readonly List<MockExchangeStream> minute_fake_streams = new List<MockExchangeStream>
        {
            new MockExchangeStream("BINANCE FUTURES", "BTC", "USD", 30000, 0.1)
        };

        public Task ReceivePositions(Action<IEnumerable<ClosedPosition>> on_receive, CancellationToken token)
        {
            return Task.Run(() =>
            {
                ushort ticker = 0;
                while(!token.IsCancellationRequested)
                {
                    GenerateNextTicks(ref ticker);

                    var new_position = FakeMarketAction(ticker);

                    if(new_position.HasValue)
                    {
                        positions.Enqueue(new_position.Value);
                        if (positions.Count > 10)
                            positions.Dequeue();

                        on_receive(positions);
                    }

                    Task.Delay(1000).Wait();
                    ticker++;
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

                ticker = 0;
            }
        }

        ClosedPosition? FakeMarketAction(ushort ticker)
        {
            var second_size = second_fake_streams.Count;
            var minute_size = minute_fake_streams.Count;

            var should_publish = rand.Next(1, 5) == 5;

            if (!should_publish)
                return null;

            var take_minute = rand.Next(1, 60) == 60;

            if(take_minute)
            {
                var index = rand.Next(0, minute_size - 1);
                var stream = minute_fake_streams.ElementAt(index);
                return stream.CloseMockPosition();
            }
            else
            {
                var index = rand.Next(0, second_size - 1);
                var stream = second_fake_streams.ElementAt(index);
                return stream.CloseMockPosition();
            }
        }
    }
}
