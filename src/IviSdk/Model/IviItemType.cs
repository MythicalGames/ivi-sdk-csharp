using System;
using System.Collections.Generic;
using Ivi.Proto.Common.Itemtype;

namespace Mythical.Game.IviSdkCSharp.Model
{
    public class IviItemType
    {
        private string _gameItemTypeId;
        private int _maxSupply;
        private int _currentSupply;
        private int _issuedSupply;
        private string _issuer;
        private int _issueTimeSpan;
        private string _category;
        private string _tokenName;
        private bool _fungible;
        private bool _burnable;
        private bool _transferable;
        private bool _finalized;
        private bool _sellable;
        private string _baseUri;
        private List<Guid> _agreementIds;
        private string _trackingId;
        private IviMetadata _metadata;
        private DateTime _createdTimestamp;
        private DateTime _updatedTimestamp;
        private ItemTypeState _itemTypeState;

        public IviItemType(string gameItemTypeId, int maxSupply, int currentSupply, int issuedSupply, string issuer,
            int issueTimeSpan, string category, string tokenName, bool fungible, bool burnable, bool transferable,
            bool finalized, bool sellable, string baseUri, List<Guid> agreementIds, string trackingId,
            IviMetadata metadata, DateTime createdTimestamp, DateTime updatedTimestamp, ItemTypeState itemTypeState)
        {
            _gameItemTypeId = gameItemTypeId;
            _maxSupply = maxSupply;
            _currentSupply = currentSupply;
            _issuedSupply = issuedSupply;
            _issuer = issuer;
            _issueTimeSpan = issueTimeSpan;
            _category = category;
            _tokenName = tokenName;
            _fungible = fungible;
            _burnable = burnable;
            _transferable = transferable;
            _finalized = finalized;
            _sellable = sellable;
            _baseUri = baseUri;
            _agreementIds = agreementIds;
            _trackingId = trackingId;
            _metadata = metadata;
            _createdTimestamp = createdTimestamp;
            _updatedTimestamp = updatedTimestamp;
            _itemTypeState = itemTypeState;
        }
    }
}
