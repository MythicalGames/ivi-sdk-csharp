using Ivi.Proto.Api.Itemtype;

namespace IviSdkCsharp.Tests.ItemType.Services
{
    public partial class FakeItemTypeService
    {
        public const string GameItemTypeIdNew = "Kicker";
        public const string GameItemTypeIdExisting = "Quarterback";
        public const string GameItemTypeIdNotFound = "Not found";
        public const string GameItemTypeIdThrow = "Should throw";
        public const string GameItemTypeIdFreeze = "Freeze this one";

        private static ItemTypes _defaultItemTypes;

        public static ItemTypes DefaultItemTypes
        {
            get
            {
                if (_defaultItemTypes == null)
                {
                    _defaultItemTypes = new ItemTypes();
                    _defaultItemTypes.ItemTypes_.Add(new Ivi.Proto.Api.Itemtype.ItemType(){GameItemTypeId = GameItemTypeIdExisting, Category = "Player", BaseUri = "", CurrentSupply = 100});
                    _defaultItemTypes.ItemTypes_.Add(new Ivi.Proto.Api.Itemtype.ItemType(){GameItemTypeId = "Kicker", Category = "Player", BaseUri = "", CurrentSupply = 100});
                    _defaultItemTypes.ItemTypes_.Add(new Ivi.Proto.Api.Itemtype.ItemType(){GameItemTypeId = "Coach", Category = "Person", BaseUri = "", CurrentSupply = 100});
                }

                return _defaultItemTypes;
            }
        }
    }
}