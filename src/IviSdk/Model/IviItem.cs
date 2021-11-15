using System;
using System.Collections.Generic;
using Ivi.Proto.Api.Order.Payment;
using Ivi.Proto.Common.Item;
using Ivi.Proto.Common.Order;
namespace Mythical.Game.IviSdkCSharp.Model
{
    public class IviFraudResult
    {
        public int FraudScore { get; set; }
        public string? FraudOmniscore { get; set; }
    }
}
namespace Mythical.Game.IviSdkCSharp.Model
{
    public class IviIssuedItem
    {
        public string? GameInventoryId { get; set; }
        public string? ItemName { get; set; }
        public string? GameItemTypeId { get; set; }
        public decimal? AmountPaid { get; set; }
        public string? Currency { get; set; }
        public IviMetadata? Metadata { get; set; }
    }
}
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
namespace Mythical.Game.IviSdkCSharp.Model
{
    public class IviAddress
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? AddressLine1 { get; set; }
        public string? AddressLine2 { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? PostalCode { get; set; }
        public string? CountryName { get; set; }
        public string? CountryIsoAlpha2 { get; set; }
    }
}
namespace Mythical.Game.IviSdkCSharp.Model
{
    public class IviOrder
    {
        public string? OrderId { get; set; }
        public string? BuyerPlayerId { get; set; }
        public string? Tax { get; set; }
        public string? Total { get; set; }
        public OrderState? OrderStatus { get; set; }
        public IviAddress? Address { get; set; }
        public Dictionary<string, object>? Metadata { get; set; }
        public string? CreatedBy { get; set; }
        public long CreatedTimestamp { get; set; }
        public string? RequestIp { get; set; }
        public string? EnvironmentId { get; set; }
        public PaymentProviderId? PaymentProviderId { get; set; }
        public List<IviItemTypeOrder>? PurchasedItems { get; set; }
        public string? ListingId { get; set; }
        public IIviPaymentProviderOrder? PaymentProviderData { get; set; }
        public string? StoreId { get; set; }
    }
}
namespace Mythical.Game.IviSdkCSharp.Model
{
    public interface IIviPaymentProviderOrder
    {
    }
}
namespace Mythical.Game.IviSdkCSharp.Model
{
    public class IviBitpayOrder : IIviPaymentProviderOrder
    {
        public Dictionary<string, object>? Invoice { get; set; }
    }
}
namespace Mythical.Game.IviSdkCSharp.Model
{
    public class IviItemTypeOrder
    {
        public List<string>? GameInventoryIds { get; set; }
        public string? ItemName { get; set; }
        public string? GameItemTypeId { get; set; }
        public string? AmountPaid { get; set; }
        public string? Currency { get; set; }
        public IviMetadata? Metadata { get; set; }
    }
}
namespace Mythical.Game.IviSdkCSharp.Model
{
    public class IviItem
    {
        public string GameInventoryId { get; }
        public string GameItemTypeId { get; }
        public long DGoodsId { get; }
        public string ItemName { get; }
        public string PlayerId { get; }
        public string OwnerSidechainAccount { get; }
        public int SerialNumber { get; }
        public string CurrencyBase { get; }
        public string MetadataUri { get; }
        public string TrackingId { get; }
        public IviMetadata Metadata { get; }
        public ItemState State { get; }
        public DateTime CreatedTimestamp { get; }
        public DateTime UpdatedTimestamp { get; }

        public IviItem(string gameInventoryId, string gameItemTypeId, long dGoodsId, string itemName, string playerId, string ownerSidechainAccount, int serialNumber, string currencyBase, string metadataUri, string trackingId, IviMetadata metadata, ItemState itemState, DateTime createdTimestamp, DateTime updatedTimestamp)
        {
            this.GameInventoryId = gameInventoryId;
            this.GameItemTypeId = gameItemTypeId;
            this.DGoodsId = dGoodsId;
            this.ItemName = itemName;
            this.PlayerId = playerId;
            this.OwnerSidechainAccount = ownerSidechainAccount;
            this.SerialNumber = serialNumber;
            this.CurrencyBase = currencyBase;
            this.MetadataUri = metadataUri;
            this.TrackingId = trackingId;
            this.Metadata = metadata;
            this.State = itemState;
            this.CreatedTimestamp = createdTimestamp;
            this.UpdatedTimestamp = updatedTimestamp;
        }
    }
}