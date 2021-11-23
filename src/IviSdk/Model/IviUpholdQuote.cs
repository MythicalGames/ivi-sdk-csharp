using System;

namespace Mythical.Game.IviSdkCSharp.Model;

public class IviUpholdQuote
{
    public string QuoteId { get; set; } = string.Empty;
    public decimal RequestedAmount { get; set; }
    public string RequestedCurrency { get; set; } = string.Empty;
    public decimal QuotedAmount { get; set; }
    public string QuotedCurrency { get; set; } = string.Empty;
    public decimal NormalizedQuotedAmount { get; set; }
    public decimal ConversionFee { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public long ExpireInMillis { get; set; }
}