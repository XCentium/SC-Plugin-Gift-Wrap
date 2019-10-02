using Sitecore.Commerce.Core;
using Sitecore.Commerce.Plugin.Carts;
using System;
using System.Linq;
using System.Threading.Tasks;
using XCentium.Commerce.Plugin.GiftWrap.Components;
using XCentium.Commerce.Plugin.GiftWrap.Pipelines.Blocks;
using XCentium.Commerce.Plugin.GiftWrap.Policies;
using Xunit;

namespace XCentium.Commerce.Plugin.GiftWrap.Tests.Pipelines.Blocks
{
    public class CalculateCartGiftBoxAdjustmentBlockTests
    {
        private CommercePipelineExecutionContext _pipelineContextFake;
        private CalculateCartGiftBoxAdjustmentBlock _pipelineBlock;

        public CalculateCartGiftBoxAdjustmentBlockTests()
        {
            _pipelineContextFake = FakeCommercePipelineExecutionContext.CreatePipelineExecutionContext();
            _pipelineBlock = new CalculateCartGiftBoxAdjustmentBlock();
        }

        [Fact]
        public async Task Run_EmptyCart_ShouldReturnCartWithZeroGitfBoxFee()
        {
            var inputCart = new Cart();

            var outputCart = await _pipelineBlock.Run(inputCart, _pipelineContextFake);

            var giftBoxAdjustment = outputCart.Adjustments.SingleOrDefault(a => a.Name == Constants.Adjustments.GiftBox.Name);
            Assert.NotNull(giftBoxAdjustment);
            Assert.True(giftBoxAdjustment.Adjustment.Amount == 0);
        }

        [Fact]
        public async Task Run_CartWithOneGiftBoxItem_ShouldReturnCartWithNonZeroGitfBoxFee()
        {
            var expectedFeeAmount = new GiftBoxFeeAdjustmentPolicy().FeeAmount;
            var inputCart = new Cart();
            var cartLine1 = new CartLineComponent()
            {
                Id = Guid.NewGuid().ToString(),
                ItemId = "some-item-1",
                Quantity = 1
            };
            cartLine1.SetComponent(new CartLineGiftBoxComponent() { IsGiftBox = true });
            inputCart.Lines.Add(cartLine1);

            var outputCart = await _pipelineBlock.Run(inputCart, _pipelineContextFake);

            var giftBoxAdjustment = outputCart.Adjustments.SingleOrDefault(a => a.Name == Constants.Adjustments.GiftBox.Name);
            Assert.NotNull(giftBoxAdjustment);
            Assert.True(giftBoxAdjustment.Adjustment.Amount == expectedFeeAmount);
        }

        [Fact]
        public async Task Run_CartWithTwoGiftBoxItems_ShouldReturnCartWithNonZeroGitfBoxFee()
        {
            var expectedFeeAmount = new GiftBoxFeeAdjustmentPolicy().FeeAmount * 2;
            var inputCart = new Cart();
            var cartLine1 = new CartLineComponent()
            {
                Id = Guid.NewGuid().ToString(),
                ItemId = "some-item-1",
                Quantity = 1
            };
            cartLine1.SetComponent(new CartLineGiftBoxComponent() { IsGiftBox = true });
            inputCart.Lines.Add(cartLine1);
            var cartLine2 = new CartLineComponent()
            {
                Id = Guid.NewGuid().ToString(),
                ItemId = "some-item-2",
                Quantity = 1
            };
            cartLine2.SetComponent(new CartLineGiftBoxComponent() { IsGiftBox = true });
            inputCart.Lines.Add(cartLine2);

            var outputCart = await _pipelineBlock.Run(inputCart, _pipelineContextFake);

            var giftBoxAdjustment = outputCart.Adjustments.SingleOrDefault(a => a.Name == Constants.Adjustments.GiftBox.Name);
            Assert.NotNull(giftBoxAdjustment);
            Assert.True(giftBoxAdjustment.Adjustment.Amount == expectedFeeAmount);
        }

        [Fact]
        public async Task Run_CartWithOneGiftBoxItemAndQuantityOfTwo_ShouldReturnCartWithNonZeroGitfBoxFee()
        {
            var expectedFeeAmount = new GiftBoxFeeAdjustmentPolicy().FeeAmount * 2;
            var inputCart = new Cart();
            var cartLine1 = new CartLineComponent()
            {
                Id = Guid.NewGuid().ToString(),
                ItemId = "some-item-1",
                Quantity = 2
            };
            cartLine1.SetComponent(new CartLineGiftBoxComponent() { IsGiftBox = true });
            inputCart.Lines.Add(cartLine1);

            var outputCart = await _pipelineBlock.Run(inputCart, _pipelineContextFake);

            var giftBoxAdjustment = outputCart.Adjustments.SingleOrDefault(a => a.Name == Constants.Adjustments.GiftBox.Name);
            Assert.NotNull(giftBoxAdjustment);
            Assert.True(giftBoxAdjustment.Adjustment.Amount == expectedFeeAmount);
        }
    }
}
