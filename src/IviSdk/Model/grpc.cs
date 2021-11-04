using System.Collections.Generic;
using System.ComponentModel;
using System.ServiceModel;
using System.Threading.Tasks;
using Google.Protobuf;
using Google.Protobuf.WellKnownTypes;
using ProtoBuf;
using ProtoBuf.Grpc;
using ProtoBuf.Meta;
using ProtoBuf.Serializers;

namespace Mythical.Game.IviSdkCSharp
{
    [ProtoBuf.ProtoContract]
    public partial class Metadata
    {

        static Metadata()
        {
            var metatype = RuntimeTypeModel.Default?.Add<Struct>(false);
            metatype!.SerializerType = typeof(MythicalStructSerializer);
        }
        [ProtoBuf.ProtoMember(1, Name = @"name")]
        [DefaultValue("")]
        public string Name { get; set; } = "";

        [ProtoBuf.ProtoMember(2, Name = @"description")]
        [DefaultValue("")]
        public string Description { get; set; } = "";

        [ProtoBuf.ProtoMember(3, Name = @"image")]
        [DefaultValue("")]
        public string Image { get; set; } = "";

        [ProtoBuf.ProtoMember(4, Name = @"properties")]
        [DefaultValue("")]
        internal string PropertiesJson { get; set; } = "";

        public string Properties { get; set; } = "";
    }

    internal class MythicalStructSerializer : ISerializer<Struct>
    {
        SerializerFeatures ISerializer<Struct>.Features => SerializerFeatures.CategoryScalar | SerializerFeatures.WireTypeString;

        Struct ISerializer<Struct>.Read(ref ProtoReader.State state, Struct value)
            => Struct.Parser.ParseJson(state.ReadString()!); 

        void ISerializer<Struct>.Write(ref ProtoWriter.State state, Struct value)
            => state.WriteString(JsonFormatter.Default.Format(value)); 
    }

    [ProtoBuf.ProtoContract]
    public partial class ItemType
    {        
        [ProtoBuf.ProtoMember(1, Name = @"game_item_type_id")]
        [DefaultValue("")]
        public string GameItemTypeId { get; set; } = "";

        [ProtoBuf.ProtoMember(2, Name = @"max_supply")]
        public int MaxSupply { get; set; }

        [ProtoBuf.ProtoMember(3, Name = @"current_supply")]
        public int CurrentSupply { get; set; }

        [ProtoBuf.ProtoMember(4, Name = @"issued_supply")]
        public int IssuedSupply { get; set; }

        [ProtoBuf.ProtoMember(5, Name = @"issuer")]
        [DefaultValue("")]
        public string Issuer { get; set; } = "";

        [ProtoBuf.ProtoMember(6, Name = @"issue_time_span")]
        public int IssueTimeSpan { get; set; }

        [ProtoBuf.ProtoMember(7, Name = @"category")]
        [DefaultValue("")]
        public string Category { get; set; } = "";

        [ProtoBuf.ProtoMember(8, Name = @"token_name")]
        [DefaultValue("")]
        public string TokenName { get; set; } = "";

        [ProtoBuf.ProtoMember(9, Name = @"fungible")]
        public bool Fungible { get; set; }

        [ProtoBuf.ProtoMember(10, Name = @"burnable")]
        public bool Burnable { get; set; }

        [ProtoBuf.ProtoMember(11, Name = @"transferable")]
        public bool Transferable { get; set; }

        [ProtoBuf.ProtoMember(12, Name = @"finalized")]
        public bool Finalized { get; set; }

        [ProtoBuf.ProtoMember(13, Name = @"sellable")]
        public bool Sellable { get; set; }

        [ProtoBuf.ProtoMember(14, Name = @"base_uri")]
        [DefaultValue("")]
        public string BaseUri { get; set; } = "";

        [ProtoBuf.ProtoMember(15, Name = @"tracking_id")]
        [DefaultValue("")]
        public string TrackingId { get; set; } = "";

        [ProtoBuf.ProtoMember(16, Name = @"agreement_ids")]
        [System.Obsolete]
        public List<string> AgreementIds { get; } = new();

        [ProtoBuf.ProtoMember(17, Name = @"item_type_state")]
        public ItemTypeState ItemTypeState { get; set; }

        [ProtoBuf.ProtoMember(18, Name = @"created_timestamp")]
        public long CreatedTimestamp { get; set; }

        [ProtoBuf.ProtoMember(19, Name = @"updated_timestamp")]
        public long UpdatedTimestamp { get; set; }

        [ProtoBuf.ProtoMember(20, Name = @"metadata")]
        public Metadata? Metadata { get; set; }

    }

    [ProtoBuf.ProtoContract]
    public partial class ItemTypes
    {        
        [ProtoBuf.ProtoMember(1)]
        public List<ItemType> item_types { get; } = new();

    }

    [ProtoBuf.ProtoContract]
    public partial class CreateItemTypeRequest
    {        
        [ProtoBuf.ProtoMember(1, Name = @"environment_id")]
        [DefaultValue("")]
        internal string EnvironmentId { get; set; } = "";

        [ProtoBuf.ProtoMember(2, Name = @"token_name")]
        [DefaultValue("")]
        public string TokenName { get; set; } = "";

        [ProtoBuf.ProtoMember(3, Name = @"category")]
        [DefaultValue("")]
        public string Category { get; set; } = "";

        [ProtoBuf.ProtoMember(4, Name = @"max_supply")]
        public int MaxSupply { get; set; }

        [ProtoBuf.ProtoMember(5, Name = @"issue_time_span")]
        public int IssueTimeSpan { get; set; }

        [ProtoBuf.ProtoMember(6, Name = @"burnable")]
        public bool Burnable { get; set; }

        [ProtoBuf.ProtoMember(7, Name = @"transferable")]
        public bool Transferable { get; set; }

        [ProtoBuf.ProtoMember(8, Name = @"sellable")]
        public bool Sellable { get; set; }

        [ProtoBuf.ProtoMember(9, Name = @"agreement_ids")]
        [System.Obsolete]
        public List<string> AgreementIds { get; } = new();

        [ProtoBuf.ProtoMember(10, Name = @"metadata")]
        public Metadata? Metadata { get; set; }

        [ProtoBuf.ProtoMember(11, Name = @"game_item_type_id")]
        [DefaultValue("")]
        public string GameItemTypeId { get; set; } = "";

    }

    [ProtoBuf.ProtoContract]
    public partial class CreateItemResponse
    {
        [ProtoBuf.ProtoMember(1, Name = @"tracking_id")]
        [DefaultValue("")]
        public string TrackingId { get; set; } = "";

        [ProtoBuf.ProtoMember(2, Name = @"game_item_type_id")]
        [DefaultValue("")]
        public string GameItemTypeId { get; set; } = "";

        [ProtoBuf.ProtoMember(3, Name = @"item_type_state")]
        public ItemTypeState ItemTypeState { get; set; }

    }

    [ProtoBuf.ProtoContract]
    public partial class GetItemTypesRequest
    {
        [ProtoBuf.ProtoMember(1, Name = @"environment_id")]
        [DefaultValue("")]
        public string EnvironmentId { get; set; } = "";

        [ProtoBuf.ProtoMember(2, Name = @"game_item_type_ids")]
        public List<string> GameItemTypeIds { get; } = new();

    }

    [ProtoBuf.ProtoContract]
    public partial class FreezeItemTypeRequest
    {
        [ProtoBuf.ProtoMember(1, Name = @"environment_id")]
        [DefaultValue("")]
        public string EnvironmentId { get; set; } = "";

        [ProtoBuf.ProtoMember(2, Name = @"game_item_type_id")]
        [DefaultValue("")]
        public string GameItemTypeId { get; set; } = "";

    }

    [ProtoBuf.ProtoContract]
    public partial class FreezeItemTypeResponse
    {        
        [ProtoBuf.ProtoMember(1, Name = @"tracking_id")]
        [DefaultValue("")]
        public string TrackingId { get; set; } = "";

        [ProtoBuf.ProtoMember(2, Name = @"item_type_state")]
        public ItemTypeState ItemTypeState { get; set; }

    }

    [ProtoBuf.ProtoContract]
    public partial class UpdateItemTypeMetadataPayload
    {        
        [ProtoBuf.ProtoMember(1, Name = @"environment_id")]
        [DefaultValue("")]
        public string EnvironmentId { get; set; } = "";

        [ProtoBuf.ProtoMember(2, Name = @"game_item_type_id")]
        [DefaultValue("")]
        public string GameItemTypeId { get; set; } = "";

        [ProtoBuf.ProtoMember(3, Name = @"metadata")]
        public Metadata? Metadata { get; set; }

    }

    [ProtoBuf.ProtoContract]
    public enum ItemTypeState
    {
        [ProtoBuf.ProtoEnum(Name = @"PENDING_CREATE")]
        PendingCreate = 0,
        [ProtoBuf.ProtoEnum(Name = @"CREATED")]
        Created = 1,
        [ProtoBuf.ProtoEnum(Name = @"PENDING_FREEZE")]
        PendingFreeze = 2,
        [ProtoBuf.ProtoEnum(Name = @"FROZEN")]
        Frozen = 3,
        [ProtoBuf.ProtoEnum(Name = @"SOLD_OUT")]
        SoldOut = 4,
        [ProtoBuf.ProtoEnum(Name = @"EXPIRED")]
        Expired = 5,
        [ProtoBuf.ProtoEnum(Name = @"FAILED")]
        Failed = 6,
        [ProtoBuf.ProtoEnum(Name = @"UPDATED_METADATA")]
        UpdatedMetadata = 7,
    }

    [ServiceContract(Name = @"ivi.rpc.api.itemtype.ItemTypeService")]
    public partial interface IItemTypeService
    {
        ValueTask<CreateItemResponse> CreateItemTypeAsync(CreateItemTypeRequest value, CallContext context = default);
        ValueTask<ItemTypes> GetItemTypesAsync(GetItemTypesRequest value, CallContext context = default);
        ValueTask<FreezeItemTypeResponse> FreezeItemTypeAsync(FreezeItemTypeRequest value, CallContext context = default);
        ValueTask UpdateItemTypeMetadataAsync(UpdateItemTypeMetadataPayload value, CallContext context = default);
    }
    
        [ProtoBuf.ProtoContract()]
    public partial class Subscribe
    {
        [ProtoBuf.ProtoMember(1, Name = @"environment_id")]
        [DefaultValue("")]
        public string EnvironmentId { get; set; } = "";
    }

    [ProtoBuf.ProtoContract()]
    public partial class ItemTypeStatusUpdate
    {
        [ProtoBuf.ProtoMember(1, Name = @"game_item_type_id")]
        [DefaultValue("")]
        public string GameItemTypeId { get; set; } = "";

        [ProtoBuf.ProtoMember(2, Name = @"current_supply")]
        public int CurrentSupply { get; set; }

        [ProtoBuf.ProtoMember(3, Name = @"issued_supply")]
        public int IssuedSupply { get; set; }

        [ProtoBuf.ProtoMember(4, Name = @"base_uri")]
        [DefaultValue("")]
        public string BaseUri { get; set; } = "";

        [ProtoBuf.ProtoMember(5, Name = @"issue_time_span")]
        public int IssueTimeSpan { get; set; }

        [ProtoBuf.ProtoMember(6, Name = @"tracking_id")]
        [DefaultValue("")]
        public string TrackingId { get; set; } = "";

        [ProtoBuf.ProtoMember(7, Name = @"item_type_state")]
        public ItemTypeState ItemTypeState { get; set; }

    }

    [ProtoBuf.ProtoContract()]
    public partial class ItemTypeStatusConfirmRequest
    {
        [ProtoBuf.ProtoMember(1, Name = @"environment_id")]
        [DefaultValue("")]
        public string EnvironmentId { get; set; } = "";

        [ProtoBuf.ProtoMember(2, Name = @"game_item_type_id")]
        [DefaultValue("")]
        public string GameItemTypeId { get; set; } = "";

        [ProtoBuf.ProtoMember(3, Name = @"tracking_id")]
        [DefaultValue("")]
        public string TrackingId { get; set; } = "";

        [ProtoBuf.ProtoMember(4, Name = @"item_type_state")]
        public ItemTypeState ItemTypeState { get; set; }

    }

    [ServiceContract(Name = @"ivi.rpc.streams.itemtype.ItemTypeStatusStream")]
    public partial interface IItemTypeStatusStream
    {
        IAsyncEnumerable<ItemTypeStatusUpdate> ItemTypeStatusStreamAsync(Subscribe value, CallContext context = default);
        ValueTask ItemTypeStatusConfirmationAsync(ItemTypeStatusConfirmRequest value, CallContext context = default);
    }
    
}

