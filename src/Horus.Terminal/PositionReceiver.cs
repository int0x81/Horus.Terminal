using Horus.Terminal.Models;
using Microsoft.AspNetCore.SignalR.Client;
using System.Text.Json;

namespace Horus.Terminal
{
    internal class PositionReceiver : IPositionReceiver, IAsyncDisposable
    {
        readonly HubConnection connection;
        public PositionReceiver()
        {
            connection = new HubConnectionBuilder()
                .WithUrl("http://localhost:5000/positions")
                .Build();
        }

        public Task ReceivePositions(Action<ClosedPosition> on_receive, CancellationToken token)
        {
            connection.On<string>("receiveposition", message =>
            {
                var position = JsonSerializer.Deserialize<ClosedPosition>(message);

                on_receive(position);
            });

            return connection.StartAsync(token);
        }

        public ValueTask DisposeAsync()
        {
            return connection.DisposeAsync();
        }
    }
}
