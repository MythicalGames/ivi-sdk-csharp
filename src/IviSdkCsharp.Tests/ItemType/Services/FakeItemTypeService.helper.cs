using Mythical.Game.IviSdkCSharp;

namespace IviSdkCsharp.Tests.ItemType.Services
{
    public partial class FakeItemTypeService
    {
        public const string GameItemTypeIdNew = "Kicker";
        public const string GameItemTypeIdExisting = "Quarterback";
        public const string GameItemTypeIdNotFound = "Not found";
        public const string GameItemTypeIdThrow = "Should throw";

        private static ItemTypes _defaultItemTypes;

        public static ItemTypes DefaultItemTypes
        {
            get
            {
                if (_defaultItemTypes == null)
                {
                    _defaultItemTypes = new ItemTypes();
                    _defaultItemTypes.item_types.Add(new Mythical.Game.IviSdkCSharp.ItemType {GameItemTypeId = GameItemTypeIdExisting, Category = "Player", BaseUri = "", CurrentSupply = 100});
                    _defaultItemTypes.item_types.Add(new Mythical.Game.IviSdkCSharp.ItemType {GameItemTypeId = "Kicker", Category = "Player", BaseUri = "", CurrentSupply = 100});
                    _defaultItemTypes.item_types.Add(new Mythical.Game.IviSdkCSharp.ItemType {GameItemTypeId = "Coach", Category = "Person", BaseUri = "", CurrentSupply = 100});
                }

                return _defaultItemTypes;
            }
        }
    }
}