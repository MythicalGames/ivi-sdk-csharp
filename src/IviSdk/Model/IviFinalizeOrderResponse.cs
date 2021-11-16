using System.Collections.Generic;
using Ivi.Proto.Common.Order;
namespace Mythical.Game.IviSdkCSharp.Model
{
    public class IviFinalizeOrderResponse
    {
        public bool Success { get; set; }
        public OrderState? OrderStatus { get; set; }
        public string? PaymentInstrumentType { get; set; }
        public string? TransactionId { get; set; }
        public List<IviIssuedItem>? PendingIssuedItems { get; set; }
        public IviFraudResult? FraudScore { get; set; }
        public string? ProcessorResponse { get; set; }
    }
}
