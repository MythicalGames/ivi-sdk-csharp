using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using Google.Protobuf.Collections;
using Google.Protobuf.WellKnownTypes;
using Ivi.Proto.Api.Itemtype;
using Ivi.Proto.Api.Order;
using Ivi.Proto.Common;
using Mapster;
using Mythical.Game.IviSdkCSharp.Model;

namespace Mythical.Game.IviSdkCSharp.Mapper
{
    public class MappersConfig
    {
        public static void RegisterMappings()
        {
            TypeAdapterConfig<IviItemType, CreateItemTypeRequest>.NewConfig()
                .Map(dest => dest.GameItemTypeId, src => src.GameItemTypeId)
                .Map(dest => dest.TokenName, src => src.TokenName)
                .Map(dest => dest.Category, src => src.Category)
                .Map(dest => dest.MaxSupply, src => src.MaxSupply)
                .Map(dest => dest.IssueTimeSpan, src => src.IssueTimeSpan)
                .Map(dest => dest.Burnable, src => src.Burnable)
                .Map(dest => dest.Transferable, src => src.Transferable)
                .Map(dest => dest.Sellable, src => src.Sellable)
                .Map(dest => dest.Metadata, src => new Metadata())
                .AfterMapping((src, dest) =>
                {
                     src.Metadata.Adapt(dest.Metadata);
                })
                .Compile(); 
            
            TypeAdapterConfig<IviMetadata, Metadata>.NewConfig()
                .Map(dest => dest.Name, src => src.Name)
                .Map(dest => dest.Description, src => src.Description)
                .Map(dest => dest.Image, src => src.Image) 
                .Map(dest => dest.Properties, src => new Struct() { Fields = { } }) 
                .AfterMapping((src, dest) =>
                {
                    foreach (var metadataProperty in src.Properties)
                    {
                        dest.Properties.Fields.Add(metadataProperty.Key, Value.ForString(metadataProperty.Value.ToString()));
                    }
                }).Compile();

            TypeAdapterConfig<List<IviItemTypeOrder>, ItemTypeOrders>.NewConfig()
                .AfterMapping((src, dest) =>
                {
                    foreach (var item in src) dest.PurchasedItems.Add(item.Adapt<ItemTypeOrder>());
                })
                .Compile();

            TypeAdapterConfig<string, decimal>.NewConfig()
                .MapWith(src => src.ToDecimal())
                .Compile();

            TypeAdapterConfig<Dictionary<string, object>, Struct>.NewConfig()
                .MapWith(src => src.ToProtoStruct())
                .Compile();

            TypeAdapterConfig<Struct, Dictionary<string, object>>.NewConfig()
                .MapWith(src => src.ToDictionary())
                .Compile();

            TypeAdapterConfig<long, DateTimeOffset>.NewConfig()
                .MapWith(src => DateTimeOffset.FromUnixTimeMilliseconds(src))
                .Compile();

            TypeAdapterConfig<DateTimeOffset, long>.NewConfig()
                .MapWith(src => src.ToUnixTimeMilliseconds())
                .Compile();


            TypeAdapterConfig<IviItemTypeOrder, ItemTypeOrder>.NewConfig()
                .AfterMapping((src, dest) =>
                {
                    dest.GameInventoryIds.Add(src.GameInventoryIds);
                })
                .Compile();

            TypeAdapterConfig<PaymentProviderOrderProto, IIviPaymentProviderOrder?>.NewConfig()
                .ConstructUsing(src => src.ProviderCase == PaymentProviderOrderProto.ProviderOneofCase.Bitpay ? src.Bitpay.Adapt<IviBitpayOrder>() : null)
                .Compile();

            TypeAdapterConfig<ItemTypeOrder, IviItemTypeOrder>.NewConfig()
                .AfterMapping((src, dest) =>
                {
                    dest.GameInventoryIds = new List<string>(src.GameInventoryIds);
                })
                .Compile();

            TypeAdapterConfig<Order, IviOrder>.NewConfig()
                .AfterMapping((src, dest) =>
                {
                    if (src.PurchasedItems != null)
                    {
                        dest.PurchasedItems = src.PurchasedItems.PurchasedItems.Select(o => o.Adapt<IviItemTypeOrder>()).ToList();
                    }
                })
                .Compile();

            TypeAdapterConfig<FinalizeOrderAsyncResponse, IviFinalizeOrderResponse>.NewConfig()
                .AfterMapping((src, dest) =>
                {
                    if (src.PendingIssuedItems != null)
                    {
                        dest.PendingIssuedItems = src.PendingIssuedItems.PurchasedItems.Select(o => o.Adapt<IviIssuedItem>()).ToList();
                    }
                })
                .Compile();
        }
    }
}
