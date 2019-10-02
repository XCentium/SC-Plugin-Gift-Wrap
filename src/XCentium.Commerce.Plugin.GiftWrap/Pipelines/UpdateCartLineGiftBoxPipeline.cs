using Microsoft.Extensions.Logging;
using Sitecore.Commerce.Core;
using Sitecore.Commerce.Plugin.Carts;
using Sitecore.Framework.Pipelines;
using XCentium.Commerce.Plugin.GiftWrap.Pipelines.Arguments;

namespace XCentium.Commerce.Plugin.GiftWrap.Pipelines
{
    /// <summary>
    /// The commerce pipeline to handle cart line gift box attribute logic.
    /// </summary>
    public class UpdateCartLineGiftBoxPipeline : CommercePipeline<CartLineGiftBoxArgument, Cart>, IUpdateCartLineGiftBoxPipeline
    {
        public UpdateCartLineGiftBoxPipeline(IPipelineConfiguration<IUpdateCartLineGiftBoxPipeline> configuration, ILoggerFactory loggerFactory) 
            : base(configuration, loggerFactory)
        {
        }
    }
}
