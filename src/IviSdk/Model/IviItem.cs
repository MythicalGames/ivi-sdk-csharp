using System;
using Ivi.Proto.Common.Item;

namespace Mythical.Game.IviSdkCSharp.Model
{
    public class IviItem
    {
        private string _gameInventoryId;
        private string _gameItemTypeId;
        private long _dGoodsId;
        private string _itemName;
        private string _playerId;
        private string _ownerSidechainAccount;
        private int _serialNumber;
        private string _currencyBase;
        private string _metadataUri;
        private string _trackingId;
        private IviMetadata _metadata;
        private ItemState _itemState;
        private DateTime _createdTimestamp;
        private DateTime _updatedTimestamp;

        public IviItem(string gameInventoryId, string gameItemTypeId, long dGoodsId, string itemName, string playerId, string ownerSidechainAccount, int serialNumber, string currencyBase, string metadataUri, string trackingId, IviMetadata metadata, ItemState itemState, DateTime createdTimestamp, DateTime updatedTimestamp)
        {
            this._gameInventoryId = gameInventoryId;
            this._gameItemTypeId = gameItemTypeId;
            this._dGoodsId = dGoodsId;
            this._itemName = itemName;
            this._playerId = playerId;
            this._ownerSidechainAccount = ownerSidechainAccount;
            this._serialNumber = serialNumber;
            this._currencyBase = currencyBase;
            this._metadataUri = metadataUri;
            this._trackingId = trackingId;
            this._metadata = metadata;
            this._itemState = itemState;
            this._createdTimestamp = createdTimestamp;
            this._updatedTimestamp = updatedTimestamp;
        }
    }
}