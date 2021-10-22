using System;
using System.Text.Json;
using Ivi.Proto.Api.Player;
using Ivi.Proto.Common.Sort;
using IviSdkCsharp.Config;

namespace IviSdkCsharp.Tests.Host.Services
{
    public partial class PlayerServiceImplementation
    {
        public const string PlayerIdExisting = "Ninja";
        public const string PlayerIdNotFound = "Not found";
        public const string PlayerIdThrow = "Should throw";
        
        public record GetPlayersExpectedRequest(DateTimeOffset createdTimestamp, int pageSize, SortOrder sortOrder);
     
        public static readonly GetPlayersExpectedRequest GetPlayersExpectedRequestData =
            new (DateTimeOffset.UtcNow, 13579, SortOrder.Desc);
        
        private static bool IsDefaultRequest(GetPlayersRequest request)
        {
            if (request.EnvironmentId != IviConfiguration.EnvironmentId) return false;
            var createdTimestampDiff = (long) request.CreatedTimestamp -
                                       GetPlayersExpectedRequestData.createdTimestamp.ToUnixTimeSeconds();
            if (Math.Abs(createdTimestampDiff) > 1) return false;
            if (request.PageSize != GetPlayersExpectedRequestData.pageSize) return false;
            if (request.SortOrder != GetPlayersExpectedRequestData.sortOrder) return false;
            return true;
        }

        private static readonly Lazy<IVIPlayer[]> _defaultPlayers = new(() => JsonSerializer.Deserialize<IVIPlayer[]>(
            @"
[
  {
    ""PlayerId"": ""molestias"",
    ""Email"": ""Nigel_Zboncak@example.com"",
    ""DisplayName"": ""Nigel Zboncak"",
    ""SidechainAccountName"": ""CreatedTimestamp: 1/1/2021"",
    ""TrackingId"": ""500"",
    ""PlayerState"": 0,
    ""CreatedTimestamp"": 1609459200
  },
  {
    ""PlayerId"": ""enim"",
    ""Email"": ""Coralie_OKon@example.com"",
    ""DisplayName"": ""Coralie O\u0027Kon"",
    ""SidechainAccountName"": ""CreatedTimestamp: 1/1/2020"",
    ""TrackingId"": ""501"",
    ""PlayerState"": 1,
    ""CreatedTimestamp"": 1577836800
  },
  {
    ""PlayerId"": ""est"",
    ""Email"": ""Yolanda78@example.com"",
    ""DisplayName"": ""Yolanda Pfeffer"",
    ""SidechainAccountName"": ""CreatedTimestamp: 1/1/2019"",
    ""TrackingId"": ""502"",
    ""PlayerState"": 2,
    ""CreatedTimestamp"": 1546300800
  },
  {
    ""PlayerId"": ""atque"",
    ""Email"": ""Lue_Halvorson67@example.com"",
    ""DisplayName"": ""Lue Halvorson"",
    ""SidechainAccountName"": ""CreatedTimestamp: 1/1/2018"",
    ""TrackingId"": ""503"",
    ""PlayerState"": 1,
    ""CreatedTimestamp"": 1514764800
  },
  {
    ""PlayerId"": ""iure"",
    ""Email"": ""Sheila63@example.com"",
    ""DisplayName"": ""Sheila Ondricka"",
    ""SidechainAccountName"": ""CreatedTimestamp: 1/1/2017"",
    ""TrackingId"": ""504"",
    ""PlayerState"": 0,
    ""CreatedTimestamp"": 1483228800
  },
  {
    ""PlayerId"": ""ducimus"",
    ""Email"": ""Alena.Windler@example.com"",
    ""DisplayName"": ""Alena Windler"",
    ""SidechainAccountName"": ""CreatedTimestamp: 1/1/2016"",
    ""TrackingId"": ""505"",
    ""PlayerState"": 0,
    ""CreatedTimestamp"": 1451606400
  },
  {
    ""PlayerId"": ""ex"",
    ""Email"": ""Cornell5@example.com"",
    ""DisplayName"": ""Cornell Marks"",
    ""SidechainAccountName"": ""CreatedTimestamp: 1/1/2015"",
    ""TrackingId"": ""506"",
    ""PlayerState"": 1,
    ""CreatedTimestamp"": 1420070400
  }
]"));

        public static IVIPlayers DefaultPlayers
        {
            get
            {
                var result = new IVIPlayers();
                result.IviPlayers.AddRange(_defaultPlayers.Value);
                return result;
            }
        }
    }
}