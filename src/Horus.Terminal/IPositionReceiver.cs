using Horus.Terminal.Models;

namespace Horus.Terminal
{
    interface IPositionReceiver
    {
        Task ReceivePositions(Action<IEnumerable<ClosedPosition>> on_receive, CancellationToken token);
    }
}
