using Sitecore.Commerce.Core;
using Sitecore.Commerce.Plugin.Carts;
using Sitecore.Commerce.Plugin.Catalog;
using Sitecore.Framework.Conditions;
using Sitecore.Framework.Pipelines;
using System;
using System.Linq;
using System.Threading.Tasks;
using XCentium.Commerce.Plugin.GiftWrap.Components;
using XCentium.Commerce.Plugin.GiftWrap.Pipelines.Arguments;

namespace XCentium.Commerce.Plugin.GiftWrap.Pipelines.Blocks
{
    /// <summary>
    /// The pipeline block to validate that the relevant sellable item is configured to allow gift box.
    /// And add/modify CartLineGiftBoxComponent based on IsGiftBox attribute.
    /// </summary>
    [PipelineDisplayName("GiftWrap.Block.UpdateCartLineGiftBox")]
    public class UpdateCartLineGiftBoxBlock : PipelineBlock<CartLineGiftBoxArgument, Cart, CommercePipelineExecutionContext>
    {
        private readonly IGetSellableItemPipeline _getSellableItemPipeline;

        public UpdateCartLineGiftBoxBlock(IGetSellableItemPipeline getSellableItemPipeline)
        {
            _getSellableItemPipeline = getSellableItemPipeline;
        }

        public override async Task<Cart> Run(CartLineGiftBoxArgument cartLineGiftBoxArgument, CommercePipelineExecutionContext context)
        {
            Condition.Requires(cartLineGiftBoxArgument).IsNotNull($"{Name}: The cartLineGiftBoxArgument cannot be null");


            var cart = cartLineGiftBoxArgument.Cart;
            var cartLine = cart.Lines.FirstOrDefault(l => l.Id.Equals(cartLineGiftBoxArgument.CartLineId, StringComparison.OrdinalIgnoreCase));

            if (cartLine != null)
            {
                var giftBoxComponent = cartLine.GetComponent<CartLineGiftBoxComponent>();

                if (cartLineGiftBoxArgument.IsGiftBox)
                {
                    if (await IsGiftBoxAllowed(cartLine.ItemId, context))
                    {
                        giftBoxComponent.IsGiftBox = cartLineGiftBoxArgument.IsGiftBox; // set to true

                        context.CommerceContext.AddModel(new LineUpdated(cartLine.Id));
                    }
                    else
                    {
                        await context.CommerceContext.AddMessage(context.GetPolicy<KnownResultCodes>().Error,
                                                                 "InvalidGiftWrapConfiguration",
                                                                 new object[] { cartLine.ItemId },
                                                                 $"Sellable item {cartLine.ItemId} is not configured for gift wrap.");

                        giftBoxComponent.IsGiftBox = false;
                    }
                }
                else
                {
                    giftBoxComponent.IsGiftBox = cartLineGiftBoxArgument.IsGiftBox; // set to false

                    context.CommerceContext.AddModel(new LineUpdated(cartLine.Id));
                }
            }
            else
            {
                await context.CommerceContext.AddMessage(context.GetPolicy<KnownResultCodes>().Error,
                                                         "CartLineNotFound",
                                                         new object[] { cartLineGiftBoxArgument.CartLineId },
                                                         $"Cart line {cartLineGiftBoxArgument.CartLineId} was not found.");
            }

            return cart;
        }

        private async Task<bool> IsGiftBoxAllowed(string itemId, CommercePipelineExecutionContext context)
        {
            var productArgument = ProductArgument.FromItemId(itemId);

            if (productArgument.IsValid())
            {
                var sellableItem = await _getSellableItemPipeline.Run(productArgument, context);

                if (sellableItem != null)
                {
                    return sellableItem.GetComponent<SellableItemGiftBoxComponent>().AllowGiftBox;
                }
            }

            return false;
        }
    }
}
