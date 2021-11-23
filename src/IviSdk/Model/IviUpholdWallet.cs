using System.Collections.Generic;

namespace Mythical.Game.IviSdkCSharp.Model;

public class IviUpholdWallet
{
    public string UpholdId { get; set; } = string.Empty;
    public string UpholdState { get; set; } = string.Empty;
    public string UpholdExternalCardId { get; set; } = string.Empty;
    public IviUpholdBalance Balance { get; set; } = new();
    public List<IviUpholdCard> Cards { get; set; } = new();
    public string Email { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public Dictionary<string, object> Verifications { get; set; } = new();
    public string BirthDate { get; set; } = string.Empty;
    public IviAddress Address { get; set; } = new();
}