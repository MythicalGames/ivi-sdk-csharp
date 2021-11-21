namespace Mythical.Game.IviSdkCSharp.Model
{
    public class IviWallet
    {
        public string AccountId { get; set; } = string.Empty;
        public IviUpholdWallet Uphold { get; set; } = new();
    }
}
