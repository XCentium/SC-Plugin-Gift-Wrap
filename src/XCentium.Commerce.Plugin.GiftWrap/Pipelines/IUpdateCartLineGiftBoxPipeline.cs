using Sitecore.Commerce.Core;
using Sitecore.Commerce.Plugin.Carts;
using Sitecore.Framework.Pipelines;
using XCentium.Commerce.Plugin.GiftWrap.Pipelines.Arguments;

namespace XCentium.Commerce.Plugin.GiftWrap.Pipelines
{
    /// <summary>
    /// The commerce pipeline to handle cart line gift box attribute logic.
    /// </summary>
    [PipelineDisplayName("GiftWrap.Pipeline.UpdateCartLineGiftBox")]
    public interface IUpdateCartLineGiftBoxPipeline : IPipeline<CartLineGiftBoxArgument, Cart, CommercePipelineExecutionContext>
    {
    }
}
