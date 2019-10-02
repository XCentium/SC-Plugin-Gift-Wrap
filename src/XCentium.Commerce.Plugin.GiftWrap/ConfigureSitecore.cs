using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Sitecore.Commerce.Core;
using Sitecore.Commerce.EntityViews;
using Sitecore.Commerce.Plugin.Carts;
using Sitecore.Commerce.Plugin.Catalog;
using Sitecore.Framework.Configuration;
using Sitecore.Framework.Pipelines.Definitions.Extensions;
using XCentium.Commerce.Plugin.GiftWrap.Pipelines;
using XCentium.Commerce.Plugin.GiftWrap.Pipelines.Blocks;

namespace XCentium.Commerce.Plugin.GiftWrap
{
    /// <summary>
    /// Configure the plugin pipelines.
    /// Add IUpdateCartLineGiftBoxPipeline pipeline.
    /// Modify IGetEntityViewPipeline, IPopulateEntityViewActionsPipeline, IDoActionPipeline and ICalculateCartPipeline pipelines.
    /// </summary>
    public class ConfigureSitecore : IConfigureSitecore
    {
        public void ConfigureServices(IServiceCollection services)
        {
            var assembly = Assembly.GetExecutingAssembly();
            services.RegisterAllPipelineBlocks(assembly);

            services.Sitecore().Pipelines(config => config
                .AddPipeline<IUpdateCartLineGiftBoxPipeline, UpdateCartLineGiftBoxPipeline>(configure =>
                {
                    configure.Add<UpdateCartLineGiftBoxBlock>()
                             .Add<ICalculateCartPipeline>()
                             .Add<PersistCartBlock>();
                }, "main", 1000)
                .ConfigurePipeline<IConfigureServiceApiPipeline>(configure =>
                {
                    configure.Add<ConfigureServiceApiBlock>();
                })
                .ConfigurePipeline<IGetEntityViewPipeline>(configure =>
                {
                    configure.Add<GetSellableItemGiftWrapViewBlock>().After<GetSellableItemDetailsViewBlock>();
                })
                .ConfigurePipeline<IPopulateEntityViewActionsPipeline>(c =>
                {
                    c.Add<PopulateSellableItemGiftWrapActionsBlock>().After<InitializeEntityViewActionsBlock>();
                })
                .ConfigurePipeline<IDoActionPipeline>(configure =>
                {
                    configure.Add<DoActionEditSellableItemGiftWrapBlock>().After<ValidateEntityVersionBlock>();
                })
                .ConfigurePipeline<ICalculateCartPipeline>(configure =>
                {
                    configure.Add<CalculateCartGiftBoxAdjustmentBlock>().After<CalculateCartSubTotalsBlock>();
                }, "main", 1001));

            services.RegisterAllCommands(assembly);
        }
    }
}