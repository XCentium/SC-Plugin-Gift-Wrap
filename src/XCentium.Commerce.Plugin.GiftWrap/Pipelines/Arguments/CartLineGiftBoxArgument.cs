using Sitecore.Commerce.Plugin.Carts;
using Sitecore.Framework.Conditions;

namespace XCentium.Commerce.Plugin.GiftWrap.Pipelines.Arguments
{
    /// <summary>
    /// Cart line gift box argument used by IUpdateCartLineGiftBoxPipeline pipeline. 
    /// </summary>
    public class CartLineGiftBoxArgument : CartArgument
    {
        public string CartLineId { get; set; }

        public bool IsGiftBox { get; set; }

        public CartLineGiftBoxArgument(Cart cart, string cartLineId, bool isGiftBox) 
            : base(cart)
        {
            Condition.Requires(cartLineId).IsNotNullOrEmpty("The cart line id can not be null");
            CartLineId = cartLineId;
            IsGiftBox = isGiftBox;
        }

    }
}
