using Sitecore.Commerce.Core;
using Sitecore.Commerce.Plugin.Carts;
using Sitecore.Commerce.Plugin.Pricing;
using Sitecore.Framework.Conditions;
using Sitecore.Framework.Pipelines;
using System.Linq;
using System.Threading.Tasks;
using XCentium.Commerce.Plugin.GiftWrap.Components;
using XCentium.Commerce.Plugin.GiftWrap.Policies;

namespace XCentium.Commerce.Plugin.GiftWrap.Pipelines.Blocks
{
    /// <summary>
    /// The pipeline block to calculate/add cart level adjustment for the total gift box cost.
    /// </summary>
    [PipelineDisplayName("GiftWrap.Block.CalculateCartGiftBoxAdjustment")]
    public class CalculateCartGiftBoxAdjustmentBlock : PipelineBlock<Cart, Cart, CommercePipelineExecutionContext>
    {
        public CalculateCartGiftBoxAdjustmentBlock()
        {
        }

        public override Task<Cart> Run(Cart cart, CommercePipelineExecutionContext context)
        {
            Condition.Requires(cart).IsNotNull($"{Name}: The cart cannot be null");

            // Clear gift box fee adjustment
            var existingGiftBoxAdjustment = cart.Adjustments.SingleOrDefault(a => a.Name.Equals(Constants.Adjustments.GiftBox.Name));
            if (existingGiftBoxAdjustment != null)
            {
                cart.Adjustments.Remove(existingGiftBoxAdjustment);
            }

            decimal giftBoxAdjustmentAmount = 0;
            var currencyCode = context.CommerceContext.CurrentCurrency();
            var giftBoxPolicy = context.GetPolicy<GiftBoxFeeAdjustmentPolicy>();

            // TODO: is additional logic needed for CartSubLineComponents?
            foreach (var cartLine in cart.Lines.Where(l => l != null))
            {
                var giftBoxComponet = cartLine.GetComponent<CartLineGiftBoxComponent>();

                if (giftBoxComponet.IsGiftBox)
                {
                    giftBoxAdjustmentAmount += giftBoxPolicy.FeeAmount * cartLine.Quantity;
                }
            }

            // Add gift box fee adjustment
            var giftBoxAdjustment = new CartLevelAwardedAdjustment()
            {
                Name = Constants.Adjustments.GiftBox.Name,
                DisplayName = Constants.Adjustments.GiftBox.Name,
                Adjustment = new Money(currencyCode, giftBoxAdjustmentAmount),
                AdjustmentType = Constants.Adjustments.GiftBox.AdjustmentType,
                AwardingBlock = Name,
                IsTaxable = giftBoxPolicy.IsTaxable
            };
            cart.Adjustments.Add(giftBoxAdjustment);

            return Task.FromResult(cart);
        }
    }
}
