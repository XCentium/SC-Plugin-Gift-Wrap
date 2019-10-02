using Sitecore.Commerce.Core;
using Sitecore.Commerce.EntityViews;
using Sitecore.Framework.Conditions;
using Sitecore.Framework.Pipelines;
using System;
using System.Threading.Tasks;
using XCentium.Commerce.Plugin.GiftWrap.Policies;

namespace XCentium.Commerce.Plugin.GiftWrap.Pipelines.Blocks
{
    /// <summary>
    /// The pipeline block to render Gift Wrap entity view actions in BizFX.
    /// </summary>
    [PipelineDisplayName("GiftWrap.Block.PopulateSellableItemGiftBoxActions")]
    public class PopulateSellableItemGiftWrapActionsBlock : PipelineBlock<EntityView, EntityView, CommercePipelineExecutionContext>
    {
        public PopulateSellableItemGiftWrapActionsBlock()
        {
        }

        public override Task<EntityView> Run(EntityView entityView, CommercePipelineExecutionContext context)
        {
            Condition.Requires(entityView).IsNotNull($"{Name}: The entityView cannot be null.");

            var viewsPolicy = context.GetPolicy<KnownSellableItemGiftWrapViewsPolicy>();

            if (string.IsNullOrEmpty(entityView?.Name) || !entityView.Name.Equals(viewsPolicy.GiftWrap, StringComparison.OrdinalIgnoreCase))
            {
                return Task.FromResult(entityView);
            }

            var actionPolicy = entityView.GetPolicy<ActionsPolicy>();

            actionPolicy.Actions.Add(
            new EntityActionView
            {
                Name = context.GetPolicy<KnownSellableItemGiftWrapActionsPolicy>().EditGiftWrap,
                DisplayName = "Edit Gift Wrap Options",
                Description = "Edit the Gift Wrap Options of a Sellable Item",
                IsEnabled = true,
                EntityView = entityView.Name,
                Icon = "edit"
            });

            return Task.FromResult(entityView);
        }
    }
}
