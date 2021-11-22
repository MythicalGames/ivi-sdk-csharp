namespace Mythical.Game.IviSdkCSharp.Model
{
    public class IviIssuedItem
    {
        public string? GameInventoryId { get; set; }
        public string? ItemName { get; set; }
        public string? GameItemTypeId { get; set; }
        public decimal AmountPaid { get; set; }
        public string? Currency { get; set; }
        public IviMetadata? Metadata { get; set; }
    }
}
