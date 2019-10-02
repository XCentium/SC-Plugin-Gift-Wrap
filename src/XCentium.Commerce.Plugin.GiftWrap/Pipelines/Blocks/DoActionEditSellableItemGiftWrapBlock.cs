using Sitecore.Commerce.Core;
using Sitecore.Commerce.EntityViews;
using Sitecore.Commerce.Plugin.Catalog;
using Sitecore.Framework.Conditions;
using Sitecore.Framework.Pipelines;
using System;
using System.Linq;
using System.Threading.Tasks;
using XCentium.Commerce.Plugin.GiftWrap.Components;
using XCentium.Commerce.Plugin.GiftWrap.Policies;

namespace XCentium.Commerce.Plugin.GiftWrap.Pipelines.Blocks
{
    /// <summary>
    /// The pipeline block to handle Gift Wrap entity edit action in BizFX.
    /// And set SellableItemGiftBoxComponent AllowGiftBox property for a sellable item.
    /// </summary>
    [PipelineDisplayName("GiftWrap.Block.DoActionPersistSellableItemGiftBox")]
    public class DoActionEditSellableItemGiftWrapBlock : PipelineBlock<EntityView, EntityView, CommercePipelineExecutionContext>
    {
        private readonly IPersistEntityPipeline _persistEntityPipeline;

        public DoActionEditSellableItemGiftWrapBlock(IPersistEntityPipeline persistEntityPipeline)
        {
            _persistEntityPipeline = persistEntityPipeline;
        }

        public override async Task<EntityView> Run(EntityView entityView, CommercePipelineExecutionContext context)
        {
            Condition.Requires(entityView).IsNotNull($"{Name}: The entityView cannot be null.");

            var giftWrapActions = context.GetPolicy<KnownSellableItemGiftWrapActionsPolicy>();

            if (string.IsNullOrEmpty(entityView.Action) ||
                !entityView.Action.Equals(giftWrapActions.EditGiftWrap, StringComparison.OrdinalIgnoreCase))
            {
                return entityView;
            }

            var sellableItem = context.CommerceContext.GetObject<SellableItem>(i => i.Id == entityView.EntityId);
            if (sellableItem != null)
            {
                var giftBoxComponent = sellableItem.GetComponent<SellableItemGiftBoxComponent>();
                var allowGiftBoxProperty = entityView.Properties.FirstOrDefault(p => p.Name.Equals(nameof(SellableItemGiftBoxComponent.AllowGiftBox), StringComparison.OrdinalIgnoreCase));

                //if (allowGiftBoxProperty?.Value?.Equals(Constants.AvailableValues.Yes, StringComparison.OrdinalIgnoreCase) ?? false)
                if (allowGiftBoxProperty != null & bool.TryParse(allowGiftBoxProperty.Value, out var allowGiftBoxBool))
                {
                    giftBoxComponent.AllowGiftBox = allowGiftBoxBool;
                }
                else
                {
                    giftBoxComponent.AllowGiftBox = false;
                }

                await _persistEntityPipeline.Run(new PersistEntityArgument(sellableItem), context);

            }

            return entityView;
        }
    }
}
