using Sitecore.Commerce.Core;

namespace XCentium.Commerce.Plugin.GiftWrap.Policies
{
    public class KnownSellableItemGiftWrapActionsPolicy : Policy
    {
        public string EditGiftWrap { get; set; } = nameof(EditGiftWrap);
    }
}
