using System;
using System.Collections.Generic;
using Ivi.Proto.Common.Item;

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
        public string MetadataUri { get; }
        public string TrackingId { get; }
        public IviMetadata Metadata { get; }
        public ItemState State { get; }
        public DateTimeOffset CreatedTimestamp { get; }
        public DateTimeOffset UpdatedTimestamp { get; }

        public IviItem()
        {
            GameInventoryId = "";
            GameItemTypeId = "";
            ItemName = "";
            PlayerId = "";
            OwnerSidechainAccount = "";
            MetadataUri = "";
            TrackingId = "";
            Metadata = new IviMetadata("", "", "", new Dictionary<string, object>());
        }

        public IviItem(string gameInventoryId, string gameItemTypeId, long dGoodsId, string itemName, string playerId, string ownerSidechainAccount, int serialNumber, string metadataUri, string trackingId, IviMetadata metadata, ItemState itemState, DateTimeOffset createdTimestamp, DateTimeOffset updatedTimestamp)
        {
            this.GameInventoryId = gameInventoryId;
            this.GameItemTypeId = gameItemTypeId;
            this.DGoodsId = dGoodsId;
            this.ItemName = itemName;
            this.PlayerId = playerId;
            this.OwnerSidechainAccount = ownerSidechainAccount;
            this.SerialNumber = serialNumber;
            this.MetadataUri = metadataUri;
            this.TrackingId = trackingId;
            this.Metadata = metadata;
            this.State = itemState;
            this.CreatedTimestamp = createdTimestamp;
            this.UpdatedTimestamp = updatedTimestamp;
        }
    }
}