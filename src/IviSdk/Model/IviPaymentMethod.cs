namespace Mythical.Game.IviSdkCSharp.Model
{
    public class IviPaymentMethod
    {
        public string CardType { get; set; } = string.Empty;
        public string Token { get; set; } = string.Empty;
        public string MaskedNumber { get; set; } = string.Empty;
        public string ExpirationMonth { get; set; } = string.Empty;
        public string ExpirationYear { get; set; } = string.Empty;
        public string LastFour { get; set; } = string.Empty;
        public bool DefaultOption { get; set; }
        public IviAddress? Address { get; set; }
    }
}
