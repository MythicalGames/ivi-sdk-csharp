using Ivi.Proto.Api.Item;
using Ivi.Proto.Api.Itemtype;
using Ivi.Proto.Api.Player;
using Ivi.Proto.Common.Sort;

#nullable disable

namespace Mythical.Game.IviSdkCSharp.Config
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "used for code generation")]
    internal class IviModelsGenerationConfig
    {
        private Item Item { get; set; }
        private ItemType ItemType { get; set; }
        private IVIPlayer Player { get; set; }
        private SortOrder SortOrder { get; set; }
    }
}