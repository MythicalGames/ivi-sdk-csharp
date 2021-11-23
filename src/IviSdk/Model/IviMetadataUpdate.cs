namespace Mythical.Game.IviSdkCSharp.Model;

public class IviMetadataUpdate
{
    public string GameInventoryId { get; set; }
    public IviMetadata Metadata { get; set; }

    public IviMetadataUpdate(string gameInventoryId, IviMetadata metadata)
    {
        GameInventoryId = gameInventoryId;
        Metadata = metadata;
    }
}