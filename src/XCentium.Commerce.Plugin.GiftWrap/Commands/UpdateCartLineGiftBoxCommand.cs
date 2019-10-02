using System;
using System.Linq;
using System.Threading.Tasks;
using Sitecore.Commerce.Core;
using Sitecore.Commerce.Core.Commands;
using Sitecore.Commerce.Plugin.Carts;
using XCentium.Commerce.Plugin.GiftWrap.Pipelines;
using XCentium.Commerce.Plugin.GiftWrap.Pipelines.Arguments;

namespace XCentium.Commerce.Plugin.GiftWrap.Commands
{
    /// <summary>
    /// The commerce command to execute IUpdateCartLineGiftBoxPipeline pipeline for a cart line.
    /// </summary>
    public class UpdateCartLineGiftBoxCommand : CommerceCommand
    {
        private readonly IUpdateCartLineGiftBoxPipeline _updateCartLineGiftBoxPipeline;
        private readonly IFindEntityPipeline _findEntityPipeline;

        public UpdateCartLineGiftBoxCommand(IServiceProvider serviceProvider,
                                            IUpdateCartLineGiftBoxPipeline updateCartLineGiftBoxPipeline, 
                                            IFindEntityPipeline findEntityPipeline) 
            : base(serviceProvider)
        {
            _updateCartLineGiftBoxPipeline = updateCartLineGiftBoxPipeline;
            _findEntityPipeline = findEntityPipeline;
        }

        /// <summary>
        /// Process UpdateCartLineGiftBoxCommand commerce command.
        /// </summary>
        /// <param name="commerceContext">Commerce context.</param>
        /// <param name="cartId">Cart Id.</param>
        /// <param name="cartLineId">Cart line Id.</param>
        /// <param name="isGiftBox">Cart line gift box boolean attribute.</param>
        /// <returns></returns>
        public async Task<Cart> Process(CommerceContext commerceContext, string cartId, string cartLineId, bool isGiftBox)
        {
            try
            {
                Cart cartResult = null;

                using (CommandActivity.Start(commerceContext, this))
                {
                    // find Cart entity
                    var findCartArgument = new FindEntityArgument(typeof(Cart), cartId, false);
                    var pipelineContext = new CommercePipelineExecutionContextOptions(commerceContext);
                    cartResult = await _findEntityPipeline.Run(findCartArgument, pipelineContext) as Cart;

                    var validationErrorCode = commerceContext.GetPolicy<KnownResultCodes>().ValidationError;

                    if (cartResult == null)
                    {
                        await pipelineContext.CommerceContext.AddMessage(validationErrorCode, 
                                                                         "EntityNotFound", 
                                                                         new object[] { cartId }, 
                                                                         $"Cart {cartId} was not found.");
                    }
                    else if (!cartResult.Lines.Any(l => l.Id == cartLineId))
                    {
                        await pipelineContext.CommerceContext.AddMessage(validationErrorCode,
                                                                         "CartLineNotFound",
                                                                         new object[] { cartLineId },
                                                                         $"Cart line {cartLineId} was not found.");
                    }
                    else
                    {
                        var arg = new CartLineGiftBoxArgument(cartResult, cartLineId, isGiftBox);
                        cartResult = await _updateCartLineGiftBoxPipeline.Run(arg, pipelineContext);
                    }

                    return cartResult;
                }
            }
            catch (Exception ex)
            {
                return await Task.FromException<Cart>(ex);
            }
        }
    }
}