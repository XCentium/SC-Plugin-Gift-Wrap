using System.Threading.Tasks;
using Microsoft.AspNetCore.OData.Builder;
using Sitecore.Commerce.Core;
using Sitecore.Commerce.Core.Commands;
using Sitecore.Framework.Conditions;
using Sitecore.Framework.Pipelines;
using static XCentium.Commerce.Plugin.GiftWrap.Constants;

namespace XCentium.Commerce.Plugin.GiftWrap
{
    /// <summary>
    /// Configure OData model for UpdateCartLineGiftBox action.
    /// </summary>
    public class ConfigureServiceApiBlock : PipelineBlock<ODataConventionModelBuilder, ODataConventionModelBuilder, CommercePipelineExecutionContext>
    {
        public override Task<ODataConventionModelBuilder> Run(ODataConventionModelBuilder modelBuilder, CommercePipelineExecutionContext context)
        {
            Condition.Requires(modelBuilder).IsNotNull($"{Name}: The argument cannot be null.");

            var configuration = modelBuilder.Action(Actions.UpdateCartLineGiftBox.Name);
            configuration.Parameter<string>(Actions.UpdateCartLineGiftBox.CartId);
            configuration.Parameter<string>(Actions.UpdateCartLineGiftBox.CartLineId);
            configuration.Parameter<bool>(Actions.UpdateCartLineGiftBox.IsGiftBox);
            configuration.ReturnsFromEntitySet<CommerceCommand>("Commands");

            return Task.FromResult(modelBuilder);
        }
    }
}
