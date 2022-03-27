using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Horus.Terminal.Models
{
    readonly struct ClosedPosition
    {
        internal string AgentId { get; init; }
        internal string ExchangeName { get; init; }
        internal string QuoteName { get; init; }
        internal string Currency { get; init; }
        internal DateTime DateOfBuy { get; init; }
        internal DateTime DateOfSell { get; init; }
        internal double BuyPrice { get; init; }
        internal double SellPrice { get; init; }
        internal double Amount { get; init; }
    }
}
