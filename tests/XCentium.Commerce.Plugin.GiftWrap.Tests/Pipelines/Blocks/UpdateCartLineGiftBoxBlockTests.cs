using Moq;
using Sitecore.Commerce.Core;
using Sitecore.Commerce.Plugin.Carts;
using Sitecore.Commerce.Plugin.Catalog;
using System;
using System.Linq;
using System.Threading.Tasks;
using XCentium.Commerce.Plugin.GiftWrap.Components;
using XCentium.Commerce.Plugin.GiftWrap.Pipelines.Arguments;
using XCentium.Commerce.Plugin.GiftWrap.Pipelines.Blocks;
using Xunit;

namespace XCentium.Commerce.Plugin.GiftWrap.Tests.Pipelines.Blocks
{
    public class UpdateCartLineGiftBoxBlockTests
    {
        private CommercePipelineExecutionContext _pipelineContextFake;
        private Mock<IGetSellableItemPipeline> _getSellableItemPipelineMock;
        private UpdateCartLineGiftBoxBlock _pipelineBlock;

        private const string TestSellableItemId = "catatlog|some-item-1|001";

        public UpdateCartLineGiftBoxBlockTests()
        {
            _pipelineContextFake = FakeCommercePipelineExecutionContext.CreatePipelineExecutionContext();
            _getSellableItemPipelineMock = new Mock<IGetSellableItemPipeline>();
            _pipelineBlock = new UpdateCartLineGiftBoxBlock(_getSellableItemPipelineMock.Object);
        }

        [Fact]
        public async Task Run_UpdateIsGiftBoxToTrueForValidItem_ShouldReturnCartLineWithIsGiftBoxEqualTrue()
        {
            var inputCart = new Cart();
            var cartLineId = Guid.NewGuid().ToString();
            var cartLine = new CartLineComponent()
            {
                Id = cartLineId,
                ItemId = TestSellableItemId,
                Quantity = 1
            };
            cartLine.SetComponent(new CartLineGiftBoxComponent() { IsGiftBox = false });
            inputCart.Lines.Add(cartLine);
            var cartLineGiftBoxArgument = new CartLineGiftBoxArgument(inputCart, cartLineId, true);
            var sellableItemValidForGiftWrap = new SellableItem();
            sellableItemValidForGiftWrap.AddComponent(null, new SellableItemGiftBoxComponent() { AllowGiftBox = true });
            _getSellableItemPipelineMock
                .Setup(p => p.Run(It.IsAny<ProductArgument>(), It.IsAny<CommercePipelineExecutionContext>()))
                .Returns(Task.FromResult(sellableItemValidForGiftWrap));

            var outputCart = await _pipelineBlock.Run(cartLineGiftBoxArgument, _pipelineContextFake);

            var cartLineGiftBoxComponent = outputCart.Lines
                                            .SingleOrDefault(l => l.Id == cartLineId)
                                            .GetComponent<CartLineGiftBoxComponent>();
            Assert.True(cartLineGiftBoxComponent.IsGiftBox);
        }

        [Fact]
        public async Task Run_UpdateIsGiftBoxToTrueForNotValidItem_ShouldReturnErrorMessage()
        {
            var inputCart = new Cart();
            var cartLineId = Guid.NewGuid().ToString();
            var cartLine = new CartLineComponent()
            {
                Id = cartLineId,
                ItemId = TestSellableItemId,
                Quantity = 1
            };
            cartLine.SetComponent(new CartLineGiftBoxComponent() { IsGiftBox = false });
            inputCart.Lines.Add(cartLine);
            var cartLineGiftBoxArgument = new CartLineGiftBoxArgument(inputCart, cartLineId, true);
            var sellableItemNotValidForGiftWrap = new SellableItem();
            sellableItemNotValidForGiftWrap.AddComponent(null, new SellableItemGiftBoxComponent() { AllowGiftBox = false });
            _getSellableItemPipelineMock
                .Setup(p => p.Run(It.IsAny<ProductArgument>(), It.IsAny<CommercePipelineExecutionContext>()))
                .Returns(Task.FromResult(sellableItemNotValidForGiftWrap));

            var outputCart = await _pipelineBlock.Run(cartLineGiftBoxArgument, _pipelineContextFake);

            var cartLineGiftBoxComponent = outputCart.Lines
                                            .SingleOrDefault(l => l.Id == cartLineId)
                                            .GetComponent<CartLineGiftBoxComponent>();
            var errorMessage = _pipelineContextFake.CommerceContext.GetMessage(m => m.Code == new KnownResultCodes().Error);
            Assert.False(cartLineGiftBoxComponent.IsGiftBox);
            Assert.NotNull(errorMessage);
        }

        [Fact]
        public async Task Run_UpdateIsGiftBoxToFalse_ShouldReturnCartLineWithIsGiftBoxEqualFalse()
        {
            var inputCart = new Cart();
            var cartLineId = Guid.NewGuid().ToString();
            var cartLine = new CartLineComponent()
            {
                Id = cartLineId,
                ItemId = TestSellableItemId,
                Quantity = 1
            };
            cartLine.SetComponent(new CartLineGiftBoxComponent() { IsGiftBox = true });
            inputCart.Lines.Add(cartLine);
            var cartLineGiftBoxArgument = new CartLineGiftBoxArgument(inputCart, cartLineId, false);

            var outputCart = await _pipelineBlock.Run(cartLineGiftBoxArgument, _pipelineContextFake);

            var cartLineGiftBoxComponent = outputCart.Lines
                                            .SingleOrDefault(l => l.Id == cartLineId)
                                            .GetComponent<CartLineGiftBoxComponent>();
            Assert.False(cartLineGiftBoxComponent.IsGiftBox);
        }
    }
}
