using Sitecore.Commerce.Core;

namespace XCentium.Commerce.Plugin.GiftWrap.Components
{
    /// <summary>
    /// Cart line component to store cart line gift box boolean attribute.
    /// </summary>
    public class CartLineGiftBoxComponent : Component
    {
        public bool IsGiftBox { get; set; }
    }
}
