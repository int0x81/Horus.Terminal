﻿using Horus.Terminal;
using Horus.Terminal.Models;

ushort[] COLUMN_WIDTHS = new ushort[8] { 18, 12, 18, 18, 20, 20, 16, 4 };
const ushort AMOUNT_COLUMNS = 30; 

Queue<ClosedPosition> positions = new();

static void WriteColor(string message, ConsoleColor color)
{
    Console.ForegroundColor = color;
    Console.Write(message);
    Console.ResetColor();
}

static void RenderColumn(string text, ushort width, ConsoleColor color = ConsoleColor.White, bool padding_right = false)
{
    string aligned;

    if (text.Length > width)
        aligned = text.Substring(0, width - 1);
    else
        aligned = text;

    string padded;
    if(padding_right)
        padded = aligned.PadRight(width, ' ');
    else
        padded = aligned.PadLeft(width, ' ');

    WriteColor(padded, color);
}

static Tuple<string, ConsoleColor, string> DefineIndicators(double relative_return)
{
    string symbol;
    ConsoleColor color;
    string extra;
    if (relative_return > 0)
    {
        symbol = "\u2197";
        color = ConsoleColor.Green;
        extra = relative_return > 0.01 ? "🔥" : "";
    }
    else if (relative_return < 0)
    {
        symbol = "\u2198";
        color = ConsoleColor.Red;
        extra = relative_return < -0.01 ? "💀" : "";
    }
    else
    {
        symbol = "\u27A1";
        color = ConsoleColor.White;
        extra = "";
    }

    return new Tuple<string, ConsoleColor, string>(symbol, color, extra);
}

static double Round(double val)
{
    return double.Parse(val.ToString("0.00"));
}

static string CreateHoldingTimeDisplay(TimeSpan span)
{
    if(5 < span.TotalDays)
        return $"{span.TotalDays.ToString("0")} days";

    if (5 < span.TotalHours)
        return $"{span.TotalHours.ToString("0")} hours";

    if (1 < span.TotalMinutes)
        return $"{span.TotalMinutes.ToString("0")} minutes";

    if (1 < span.TotalSeconds)
        return $"{span.TotalSeconds.ToString("0")} seconds";

    return "< 1 second";
}

string RenderCurrencyName(string currency_name)
{
    const ushort CURRENCY_NAME_MAX_LENGTH = 4;
    return currency_name.PadLeft(CURRENCY_NAME_MAX_LENGTH);
}

void DisplayHeader()
{
    RenderColumn("Exchange", COLUMN_WIDTHS[0]);
    RenderColumn("Quote", COLUMN_WIDTHS[1]);
    RenderColumn("Buy", COLUMN_WIDTHS[2]);
    RenderColumn("Sell", COLUMN_WIDTHS[3]);
    RenderColumn("Absolute Return", COLUMN_WIDTHS[4]);
    RenderColumn("Relative Return", COLUMN_WIDTHS[5]);
    RenderColumn("Holding Time", COLUMN_WIDTHS[6]);
    RenderColumn("", COLUMN_WIDTHS[7]);
    Console.WriteLine();
    foreach (var col in COLUMN_WIDTHS)
    {
        for(int i = 0; i < col; i++)
            Console.Write("-");
    }
    Console.WriteLine();
}

void DisplayClosedPosition(ClosedPosition position)
{
    var absolute_buy_in = position.Amount * position.BuyPrice;
    var relative_return = position.SellPrice / position.BuyPrice - 1;
    var absolute_return = absolute_buy_in * (relative_return + 1) - absolute_buy_in;
    var holding_time = CreateHoldingTimeDisplay(position.DateOfSell - position.DateOfBuy);
    var indicators = DefineIndicators(relative_return);

    RenderColumn(position.ExchangeName, COLUMN_WIDTHS[0]);
    RenderColumn(position.QuoteName, COLUMN_WIDTHS[1]);
    RenderColumn($"{Round(position.BuyPrice)} {RenderCurrencyName(position.Currency)}", COLUMN_WIDTHS[2]);
    RenderColumn($"{Round(position.SellPrice)} {RenderCurrencyName(position.Currency)}", COLUMN_WIDTHS[3]);
    RenderColumn($"{Round(absolute_return)} {RenderCurrencyName(position.Currency)} {indicators.Item1}", COLUMN_WIDTHS[4], indicators.Item2);
    RenderColumn($"{Round(relative_return * 100)}% {indicators.Item1}", COLUMN_WIDTHS[5], indicators.Item2);
    RenderColumn($"{holding_time}", COLUMN_WIDTHS[6]);
    RenderColumn(indicators.Item3, COLUMN_WIDTHS[7]);
    Console.WriteLine();
}

void RenderTable(ClosedPosition new_position)
{
    Console.Clear();
    DisplayHeader();

    positions.Enqueue(new_position);

    if (positions.Count > AMOUNT_COLUMNS)
        positions.Dequeue();

    foreach(var pos in positions)
        DisplayClosedPosition(pos);
}

Console.OutputEncoding = System.Text.Encoding.Unicode;

var receiver = new PositionMockReceiver();
var token_source = new CancellationTokenSource();
var task = receiver.ReceivePositions(RenderTable, token_source.Token);

Console.ReadKey();
token_source.Cancel();
await task;