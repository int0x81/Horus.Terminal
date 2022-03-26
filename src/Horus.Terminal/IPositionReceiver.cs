using Horus.Terminal.Models;

namespace Horus.Terminal
{
    interface IPositionReceiver
    {
        Task ReceivePositions(Action<ClosedPosition> on_receive, CancellationToken token);
    }
}
