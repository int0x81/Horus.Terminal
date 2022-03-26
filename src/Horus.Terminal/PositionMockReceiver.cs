using Horus.Terminal.Models;

namespace Horus.Terminal
{
    class PositionMockReceiver : IPositionReceiver
    {
        readonly Queue<ClosedPosition> positions = new Queue<ClosedPosition>();

        public Task ReceivePositions(Action<IEnumerable<ClosedPosition>> on_receive, CancellationToken token)
        {
            return Task.Run(() =>
            {
                while(!token.IsCancellationRequested)
                {
                    var new_position = GenerateMockPosition();

                    positions.Enqueue(new_position);
                    if (positions.Count > 10)
                        positions.Dequeue();

                    on_receive(positions);

                    Task.Delay(new Random().Next(600, 5000)).Wait();
                }
            }, token);
        }

        string[] FakeExchangeList = new string[10]
        {
            "EUREX",
            "BINANCE FUTURES"
        };

        string[] FakeCryptoList = new string[10]
        {
            "BTC",
            "ETH",
            "CDN",
            "KTZ",
            "IOTA",
        };

        string[] FakeStockIndexList = new string[10]
        {
            "FDAX",
        };

        ClosedPosition GenerateMockPosition()
        {
            return new ClosedPosition
            {
                ExchangeName = "BINANCE FUTURES",
                QuoteName = "BTC",
                Currency = "EUR",
                BuyPrice = 78.547,
                SellPrice = 83.652555,
                Amount = 10,
                DateOfBuy = DateTime.UtcNow.AddMinutes(-5),
                DateOfSell = DateTime.UtcNow,
            };
        }
    }
}
