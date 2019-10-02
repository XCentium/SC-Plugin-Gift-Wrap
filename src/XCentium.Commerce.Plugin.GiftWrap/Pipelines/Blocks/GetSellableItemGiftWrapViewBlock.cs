using Sitecore.Commerce.Core;
using Sitecore.Commerce.EntityViews;
using Sitecore.Commerce.Plugin.Catalog;
using Sitecore.Framework.Conditions;
using Sitecore.Framework.Pipelines;
using System;
using System.Threading.Tasks;
using XCentium.Commerce.Plugin.GiftWrap.Components;
using XCentium.Commerce.Plugin.GiftWrap.Policies;

namespace XCentium.Commerce.Plugin.GiftWrap.Pipelines.Blocks
{
    /// <summary>
    /// The pipeline block to render Gift Wrap child entity view in BizFX.
    /// </summary>
    [PipelineDisplayName("GiftWrap.Block.GetSellableItemGiftBoxView")]
    public class GetSellableItemGiftWrapViewBlock : PipelineBlock<EntityView, EntityView, CommercePipelineExecutionContext>
    {
        public GetSellableItemGiftWrapViewBlock(ViewCommander viewCommander)
        {
        }

        public override Task<EntityView> Run(EntityView entityView, CommercePipelineExecutionContext context)
        {
            Condition.Requires(entityView).IsNotNull($"{Name}: The entityView cannot be null.");

            var entityViewArgument = context.CommerceContext.GetObject<EntityViewArgument>();

            var catalogViews = context.GetPolicy<KnownCatalogViewsPolicy>();

            var giftWrapViews = context.GetPolicy<KnownSellableItemGiftWrapViewsPolicy>();
            var giftWrapActions = entityView.GetPolicy<KnownSellableItemGiftWrapActionsPolicy>();

            var isConnectSellableItemView = entityView.Name.Equals(catalogViews.ConnectSellableItem, StringComparison.OrdinalIgnoreCase);

            // TODO: what about variation view?
            if (string.IsNullOrEmpty(entityViewArgument?.ViewName) ||
                (!entityViewArgument.ViewName.Equals(catalogViews.Master, StringComparison.OrdinalIgnoreCase) &&
                 /*!entityViewArgument.ViewName.Equals(catalogViews.Details, StringComparison.OrdinalIgnoreCase) &&*/
                 !entityViewArgument.ViewName.Equals(giftWrapViews.GiftWrap, StringComparison.OrdinalIgnoreCase) &&
                 !isConnectSellableItemView))
            {
                return Task.FromResult(entityView);
            }

            // Only proceed if the current entity is a sellable item
            if (!(entityViewArgument.Entity is SellableItem))
            {
                return Task.FromResult(entityView);
            }

            var sellableItem = (SellableItem)entityViewArgument.Entity;
            var targetView = entityView;

            // Check if the edit action was requested
            var isEditGiftWrapAction = !string.IsNullOrEmpty(entityView.Action) && entityView.Action.Equals(giftWrapActions.EditGiftWrap, StringComparison.OrdinalIgnoreCase);
            var isMasterView = string.IsNullOrEmpty(entityView.Action) && entityViewArgument.ViewName.Equals(catalogViews.Master, StringComparison.OrdinalIgnoreCase);
            if (!isEditGiftWrapAction)
            {
                // Create a new view and add it to the current entity view.
                targetView = new EntityView
                {
                    Name = context.GetPolicy<KnownSellableItemGiftWrapViewsPolicy>().GiftWrap,
                    DisplayName = "Gift Wrap",
                    EntityId = entityView.EntityId,
                    EntityVersion = entityView.EntityVersion
                };
                entityView.ChildViews.Add(targetView);
            }


            if (sellableItem != null && (isConnectSellableItemView || isEditGiftWrapAction || isMasterView))
            {
                var giftBoxComponent = sellableItem?.GetComponent<SellableItemGiftBoxComponent>();

                targetView.Properties.Add(new ViewProperty
                {
                    Name = nameof(SellableItemGiftBoxComponent.AllowGiftBox),
                    DisplayName = "Allow Gift Box",
                    IsRequired = false,
                    IsReadOnly = !isEditGiftWrapAction,
                    RawValue = GetAllowGiftBoxValue(giftBoxComponent.AllowGiftBox, isConnectSellableItemView, !isEditGiftWrapAction)
                    //RawValue = giftBoxComponent.AllowGiftBox
                    //Policies = new List<Policy>()
                    //{
                    //    GetAvailableSelectionsPolicy()
                    //}
                });
            }

            return Task.FromResult(entityView);
        }

        private object GetAllowGiftBoxValue(bool allowGiftBox, bool isConnectSellableItemView, bool isReadOnly)
        {
            if (!isConnectSellableItemView && isReadOnly)
            {
                return allowGiftBox == true ? Constants.AvailableValues.Yes : Constants.AvailableValues.No;
            }

            return allowGiftBox;
        }

        //private Policy GetAvailableSelectionsPolicy()
        //{
        //    return new AvailableSelectionsPolicy(new List<Selection>()
        //    {
        //        new Selection()
        //        {
        //            Name = Constants.AvailableValues.Yes,
        //            DisplayName = Constants.AvailableValues.Yes
        //        },
        //        new Selection()
        //        {
        //            Name = Constants.AvailableValues.No,
        //            DisplayName = Constants.AvailableValues.No
        //        },
        //    });
        //}
    }
}
