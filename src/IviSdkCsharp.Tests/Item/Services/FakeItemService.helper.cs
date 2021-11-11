using System;
using System.Collections.Generic;
using System.Text.Json;
using Ivi.Proto.Api.Item;
using Ivi.Proto.Common;
using Ivi.Proto.Common.Sort;
using Mapster;
using Mythical.Game.IviSdkCSharp.Config;
using Mythical.Game.IviSdkCSharp.Model;

namespace IviSdkCsharp.Tests.Item.Services
{
    public partial class FakeItemService
    {
        public const string GameInventoryIdExisting = "ItemIdWizardStaff";
        public const string GameInventoryIdBurning = "BurningId";
        public const string GameInventoryIdIssueId = "IssueId";
        public const string GameInventoryIdPendingIssue = "PendingIssue";
        public const string GameInventoryIdPendingListed = "PendingListed";
        public const string GameInventoryIdListed = "Listed";
        public const string GameInventoryIdPendingTransfer = "PendingTrasfer";
        public const string GameInventoryIdTransferred = "Transferred";
        public const string GameInventoryIdPendingSale = "PendingSale";
        public const string GameInventoryIdPendingBurned = "PendingBurned";
        public const string GameInventoryIdBurned = "Burned";
        public const string GameInventoryIdFailed = "Failed";
        public const string GameInventoryIdPendingCloseListing = "PendingCloseListing ";
        public const string GameInventoryIdListingClosed = "ListingClosed";
        public const string GameInventoryIdUpdatedMetadata = "UpdatedMetadata";

        public const string GameInventoryIdNotFound = "Not found";
        public const string GameInventoryIdThrow = "Should throw";

        public static readonly UpdateItemMetadataResponse IssuedMetaData = new IviMetadata("thing", "imgUrl", "randomeNameUpdated", new Dictionary<string, object>()).Adapt<UpdateItemMetadataResponse>();

        public record GetExpectedUpdateItemMetadataResponse();

        public record GetItemsExpectedRequest(DateTimeOffset createdTimestamp, int pageSize, SortOrder sortOrder);
     
        public static readonly GetItemsExpectedRequest GetItemsExpectedRequestData =
            new (DateTimeOffset.UtcNow, 256, SortOrder.Desc);
        
        private static bool IsDefaultRequest(GetItemsRequest request)
        {
            if (request.EnvironmentId != IviConfiguration.EnvironmentId) return false;
            var createdTimestampDiff = (long) request.CreatedTimestamp -
                                       GetItemsExpectedRequestData.createdTimestamp.ToUnixTimeSeconds();
            if (Math.Abs(createdTimestampDiff) > 1) return false;
            if (request.PageSize != GetItemsExpectedRequestData.pageSize) return false;
            if (request.SortOrder != GetItemsExpectedRequestData.sortOrder) return false;
            return true;
        }
        
        private static readonly Lazy<Items> _defaultItems = new(() => JsonSerializer.Deserialize<Items>(
            @"
{ ""Items_"": [
  {
    ""GameInventoryId"": ""ItemIdOne"",
    ""GameItemTypeId"": ""5ad49ec2-45b2-4ce4-a863-8527802039c0"",
    ""DGoodsId"": ""4cd9e0ed-218a-4923-81e1-a09d1e74d330"",
    ""ItemName"": ""Sword"",
    ""PlayerId"": ""9dfc3127-5dd9-411b-aa2a-c48e4e789d2f"",
    ""OwnerSidechainAccount"": ""eadea1f3-4b03-4d6f-94f0-95110e9a43e8"",
    ""SerialNumber"": ""65db2b33-a0d3-43ec-854b-bf58cf826bb6"",
    ""CurrencyBase"": ""fcfb8c5a-7926-489f-a9b8-818e5200d0ab"",
    ""MetadataUri"": ""f4566c16-49c0-4688-8942-765c6cd088a0"",
    ""TrackingId"": ""06a5aa90-ad6a-4289-bedc-da98670bed70"",
    ""Metadata"": {
        ""Name"": ""fugiat"",
        ""Description"": ""Elit consequat occaecat adipisicing id. Adipisicing aliquip irure non quis duis qui excepteur nulla aliqua ullamco exercitation exercitation. Irure sint ex cillum sit enim laboris officia nisi eiusmod deserunt deserunt. Elit quis ipsum eu labore non irure. Ea magna exercitation cupidatat in cupidatat Lorem.\r\n"",
        ""Image"": ""sunt"",
        ""Properties"": {
            ""thing1"": {
                ""id"": 0,
                ""name"": ""Leola Mcdowell""
            }
        }
    },
    ""State"": ""0"",
    ""CreatedTimestamp"": 1609459200,
    ""UpdatedTimestamp"": ""2020-10-27T11:08:42 +05:00""
  },
  {
    ""GameInventoryId"": ""ItemIdTwo"",
    ""GameItemTypeId"": ""5be0c326-25b5-4b03-8793-bc0ba80005a8"",
    ""DGoodsId"": ""81f00e37-3c40-4043-8d5b-14a7c763160a"",
    ""ItemName"": ""Axe"",
    ""PlayerId"": ""184a1b26-2a9b-41b4-835f-8b2bae353b8c"",
    ""OwnerSidechainAccount"": ""9b9819d6-a80a-4e6f-8896-788a3e89446a"",
    ""SerialNumber"": ""767b8a15-6a51-4baa-a7c3-b02371afe50c"",
    ""CurrencyBase"": ""612173cc-a1fd-4042-977e-79a5f41542ea"",
    ""MetadataUri"": ""fa228b91-1f0b-439c-af14-90deba30da82"",
    ""TrackingId"": ""07e45e07-675d-4628-b135-ed23a7360254"",
    ""Metadata"": {
        ""Name"": ""veniam"",
        ""Description"": ""Voluptate ea labore excepteur veniam commodo minim ullamco ex eu fugiat occaecat ad duis deserunt. Tempor id esse est laboris nulla elit dolor labore velit. Incididunt sit sunt laborum in id veniam. Irure ullamco cillum irure ullamco magna sit veniam anim sunt sint fugiat amet.\r\n"",
        ""Image"": ""exercitation"",
        ""Properties"": {
            ""thing2"": {
                ""id"": 1,
                ""name"": ""Fitzgerald Peck""
            }
        }
    },
    ""State"": ""1"",
    ""CreatedTimestamp"": 1577836800,
    ""UpdatedTimestamp"": ""2015-07-31T02:58:54 +05:00""
  },
  {
    ""GameInventoryId"": ""ItemIdThree"",
    ""GameItemTypeId"": ""ba98899c-6878-4f0f-8d2b-3e881fec7146"",
    ""DGoodsId"": ""1c6d5f5d-71e3-41f4-81ab-35ca2fc03a79"",
    ""ItemName"": ""Dagger"",
    ""PlayerId"": ""2f8d7d75-934a-4ead-945d-964318738c00"",
    ""OwnerSidechainAccount"": ""d92439fb-20ac-4d8e-b06b-a2a3dd0a84ed"",
    ""SerialNumber"": ""109c0da5-5216-4c37-a1ea-cfb6d749f870"",
    ""CurrencyBase"": ""4cb94596-9338-483e-83e2-ccaef9f1dde5"",
    ""MetadataUri"": ""2583c1b2-3df0-421d-ae27-4662dcc9059c"",
    ""TrackingId"": ""0ab4ffd5-1fb8-41bb-b22a-778fd9f07604"",
    ""Metadata"": {
        ""Name"": ""nostrud"",
        ""Description"": ""Eu reprehenderit laborum cillum nisi dolor. Fugiat eiusmod minim elit sit ut ad elit deserunt elit magna. Anim sunt veniam labore dolor culpa occaecat. Aliqua amet Lorem irure officia commodo reprehenderit do nulla nisi cillum irure aliquip voluptate. Ad dolore exercitation eu velit dolor laborum.\r\n"",
        ""Image"": ""proident"",
        ""Properties"": {
            ""thingy3"": {
                ""id"": 2,
                ""name"": ""Helen Dickerson""
            }
        }
    },
    ""State"": ""2"",
    ""CreatedTimestamp"": 1546300800,
    ""UpdatedTimestamp"": ""2018-05-11T05:16:18 +05:00""
  },
  {
    ""GameInventoryId"": ""ItemIdFour"",
    ""GameItemTypeId"": ""beeb327b-4149-4c67-b741-f01934532c76"",
    ""DGoodsId"": ""e868aecf-cb47-47b2-9973-df62a3e60aa3"",
    ""ItemName"": ""War Hammer"",
    ""PlayerId"": ""1c5750b4-8c33-4948-98b2-eaf1853e351c"",
    ""OwnerSidechainAccount"": ""28c21557-393c-4f55-8e25-fcaeb659c705"",
    ""SerialNumber"": ""39815502-2d12-4b2a-9feb-1a4a589c4cab"",
    ""CurrencyBase"": ""7a5fc54b-b9f7-41bd-81ca-5999a5b2cead"",
    ""MetadataUri"": ""ea01e5d8-a5a1-4bd0-a7fe-87d2375c3754"",
    ""TrackingId"": ""4d77d221-d1ea-4d7e-8a70-0bedb0a4d4ea"",
    ""Metadata"": {
        ""Name"": ""magna"",
        ""Description"": ""Commodo commodo commodo nisi exercitation et magna velit amet elit do Lorem aliquip proident. Et nulla aute aute ea cupidatat. Anim id qui voluptate do nulla magna. Esse aliqua non minim labore ullamco elit qui elit. Pariatur id dolore id exercitation.\r\n"",
        ""Image"": ""velit"",
        ""Properties"": {
            ""thing 4"": {
                ""id"": 3,
                ""name"": ""Wood Walton""
            }
        }
    },
    ""State"": ""3"",
    ""CreatedTimestamp"": 1514764800,
    ""UpdatedTimestamp"": ""2016-08-08T01:16:01 +05:00""
  },
  {
    ""GameInventoryId"": ""ItemIdFive"",
    ""GameItemTypeId"": ""34a04532-0cce-448a-95b3-bf47696c9785"",
    ""DGoodsId"": ""7aaf25c2-bc6b-4fb3-a317-86d8216943cb"",
    ""ItemName"": ""Sythe"",
    ""PlayerId"": ""84d99e74-8b5d-4b45-85c8-28b7a299a6e2"",
    ""OwnerSidechainAccount"": ""9f514fc7-c36f-4bc1-b18e-7e673fdd4ef3"",
    ""SerialNumber"": ""0abd7998-ed8b-4cb3-8f2d-cc82306dd0a3"",
    ""CurrencyBase"": ""a6125de4-5f0e-4621-a20f-7683f8dc5e62"",
    ""MetadataUri"": ""76301a89-b16f-4451-8416-89e38be9a228"",
    ""TrackingId"": ""a16aa6c8-a522-4caf-9387-4e2271245743"",
    ""Metadata"": {
        ""Name"": ""qui"",
        ""Description"": ""Deserunt labore ut ullamco ad culpa ad aute. Eiusmod voluptate ut ut deserunt ea proident ex sint. Nostrud ullamco tempor commodo nulla et eu veniam tempor duis. Nulla mollit commodo esse consequat et ad. Dolor velit in excepteur dolor nostrud. Irure amet excepteur commodo deserunt reprehenderit Lorem labore dolore mollit dolore.\r\n"",
        ""Image"": ""commodo"",
        ""Properties"": {
            ""thing 5"": {
                ""id"": 4,
                ""name"": ""Peters Higgins""
            }
        }
    },
    ""State"": ""4"",
    ""CreatedTimestamp"": 1483228800,
    ""UpdatedTimestamp"": ""2015-02-10T04:41:04 +06:00""
  },
  {
    ""GameInventoryId"": ""ItemIdSix"",
    ""GameItemTypeId"": ""b1f59087-5635-4336-8b1a-0fd201a112ff"",
    ""DGoodsId"": ""ee447cd1-c775-4692-bd01-fc09c96976d7"",
    ""ItemName"": ""Spear"",
    ""PlayerId"": ""f9fc1982-fcf8-4715-bbd6-df02b392e083"",
    ""OwnerSidechainAccount"": ""ca3eb6ee-4f0d-44ee-b9d1-ec759a2cf251"",
    ""SerialNumber"": ""acb38a39-c8f2-4ee5-92dd-108762c92dd9"",
    ""CurrencyBase"": ""a4e0a7ab-fa20-4ed9-9f6b-fd15e7ddddfe"",
    ""MetadataUri"": ""aca44c10-789f-4d9f-81c6-5f44ccd98715"",
    ""TrackingId"": ""b03e306d-e5a7-4183-8cd3-f9d959a7d87e"",
    ""Metadata"": {
        ""Name"": ""magnalana"",
        ""Description"": ""Fugiat aliquip sunt nisi qui exercitation labore quis anim. Qui non do laborum minim eiusmod adipisicing sint elit pariatur. In deserunt adipisicing dolor aliquip ullamco nisi aliqua enim ad consequat. Tempor aliquip magna aliquip cupidatat Lorem cupidatat culpa exercitation non pariatur nisi non ex qui. Non consectetur enim eiusmod nisi eu exercitation pariatur ea dolore exercitation ullamco voluptate excepteur sint. Non ex incididunt aliquip irure culpa velit officia adipisicing in ad mollit. Enim do culpa mollit minim.\r\n"",
        ""Image"": ""proident"",
        ""Properties"": {
            ""thing 6"": {
                ""id"": 5,
                ""name"": ""Kelley Pacheco""
            }
        }
    },
    ""State"": ""5"",
    ""CreatedTimestamp"": 1451606400,
    ""UpdatedTimestamp"": ""2019-07-21T09:23:42 +05:00""
  }
]}"));
        
        public static  Items DefaultItems
        {
            get
            {
                var result = new Items();
                result.Items_.AddRange(_defaultItems.Value.Items_);
                return result;
            }
        }
    }
}