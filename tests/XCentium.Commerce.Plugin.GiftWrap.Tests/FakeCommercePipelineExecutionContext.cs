using Microsoft.Extensions.Logging;
using Moq;
using Sitecore.Commerce.Core;
using Sitecore.Framework.Pipelines;

namespace XCentium.Commerce.Plugin.GiftWrap.Tests
{
    public class FakeCommercePipelineExecutionContext
    {
        public static CommercePipelineExecutionContext CreatePipelineExecutionContext()
        {
            return new CommercePipelineExecutionContext(CreatePipelineExecutionContextOptions(), new Mock<ILogger>().Object);
        }

        private static IPipelineExecutionContextOptions CreatePipelineExecutionContextOptions()
        {
            return new CommercePipelineExecutionContextOptions(CreateCommerceContext());
        }

        private static CommerceContext CreateCommerceContext()
        {
            var context = new CommerceContext(new Mock<ILogger>().Object, null);
            context.Environment = new CommerceEnvironment();
            return context;
        }
    }
}
