namespace Horus.Terminal.Models
{
    readonly struct ClosedPosition
    {
        public string AgentId { get; init; }
        public string ExchangeName { get; init; }
        public string QuoteName { get; init; }
        public string Currency { get; init; }
        public DateTime DateOfBuy { get; init; }
        public DateTime DateOfSell { get; init; }
        public double BuyPrice { get; init; }
        public double SellPrice { get; init; }
        public double Amount { get; init; }
    }
}
