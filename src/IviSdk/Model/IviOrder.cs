using System;
using System.Collections.Generic;
using Ivi.Proto.Api.Order.Payment;
using Ivi.Proto.Common.Order;
namespace Mythical.Game.IviSdkCSharp.Model
{
    public class IviOrder
    {
        public string? OrderId { get; set; }
        public string? BuyerPlayerId { get; set; }
        public decimal Tax { get; set; }
        public decimal Total { get; set; }
        public OrderState? OrderStatus { get; set; }
        public IviAddress? Address { get; set; }
        public Dictionary<string, object>? Metadata { get; set; }
        public string? CreatedBy { get; set; }
        public DateTimeOffset CreatedTimestamp { get; set; }
        public string? RequestIp { get; set; }
        public string? EnvironmentId { get; set; }
        public PaymentProviderId? PaymentProviderId { get; set; }
        public List<IviItemTypeOrder>? PurchasedItems { get; set; }
        public string? ListingId { get; set; }
        public IIviPaymentProviderOrder? PaymentProviderData { get; set; }
        public string? StoreId { get; set; }
    }
}
