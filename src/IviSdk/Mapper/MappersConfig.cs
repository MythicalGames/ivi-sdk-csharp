﻿using System.Xml.Serialization;
using Google.Protobuf.WellKnownTypes;
using Ivi.Proto.Api.Itemtype;
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
        }
    }
}