namespace Mythical.Game.IviSdkCSharp.Model
{
    public class IviUpholdCard
    {
        public string UpholdId { get; set; } = string.Empty;
        public string Currency { get; set; } = string.Empty;
        public decimal Balance { get; set; }
        public string NormalizedCurrency { get; set; } = string.Empty;
        public decimal NormalizedBalance { get; set; }
    }
}
