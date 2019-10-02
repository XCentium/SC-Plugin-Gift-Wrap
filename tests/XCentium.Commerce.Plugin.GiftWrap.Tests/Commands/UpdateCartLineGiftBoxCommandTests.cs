using Moq;
using Sitecore.Commerce.Core;
using Sitecore.Commerce.Plugin.Carts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XCentium.Commerce.Plugin.GiftWrap.Commands;
using XCentium.Commerce.Plugin.GiftWrap.Pipelines;
using XCentium.Commerce.Plugin.GiftWrap.Pipelines.Arguments;
using Xunit;

namespace XCentium.Commerce.Plugin.GiftWrap.Tests.Commands
{
    public class UpdateCartLineGiftBoxCommandTests
    {
        private Mock<IServiceProvider> _serviceProviderMock;
        private Mock<IUpdateCartLineGiftBoxPipeline> _updateCartLineGiftBoxPipelineMock;
        private Mock<IFindEntityPipeline> _findEntityPipelineMock;
        private CommerceContext _commerceContextFake;

        private UpdateCartLineGiftBoxCommand _command;

        private const string TestCartId = "Cart01";
        private const string TestCartLineId = "CartLine1";
        private const string TestSellableItemId = "catatlog|some-item-1|001";

        public UpdateCartLineGiftBoxCommandTests()
        {
            _serviceProviderMock = new Mock<IServiceProvider>();
            _updateCartLineGiftBoxPipelineMock = new Mock<IUpdateCartLineGiftBoxPipeline>();
            _findEntityPipelineMock = new Mock<IFindEntityPipeline>();
            _commerceContextFake = FakeCommercePipelineExecutionContext.CreatePipelineExecutionContext().CommerceContext;

            _command = new UpdateCartLineGiftBoxCommand(_serviceProviderMock.Object, 
                                                        _updateCartLineGiftBoxPipelineMock.Object, 
                                                        _findEntityPipelineMock.Object);
        }

        [Fact]
        public async Task Process_GiftBoxUpdateForValidCartLine_ShouldReturnUpdatedCart()
        {
            _findEntityPipelineMock
                .Setup(p => p.Run(It.IsAny<FindEntityArgument>(), It.IsAny<CommercePipelineExecutionContextOptions>()))
                .Returns(Task.FromResult((CommerceEntity)CreateInputCart(TestCartLineId)));
            _updateCartLineGiftBoxPipelineMock
                .Setup(p => p.Run(It.IsAny<CartLineGiftBoxArgument>(), It.IsAny<CommercePipelineExecutionContextOptions>()))
                .Returns(Task.FromResult(new Cart()));

            var outputCart = await _command.Process(_commerceContextFake, TestCartId, TestCartLineId, true);

            Assert.NotNull(outputCart);
        }

        [Fact]
        public async Task Process_GiftBoxUpdateForNonexistentCartLine_ShouldReturnValidationErrorMessage()
        {
            _findEntityPipelineMock
                .Setup(p => p.Run(It.IsAny<FindEntityArgument>(), It.IsAny<CommercePipelineExecutionContextOptions>()))
                .Returns(Task.FromResult((CommerceEntity)CreateInputCart("WrongCartLine1")));

            var outputCart = await _command.Process(_commerceContextFake, TestCartId, TestCartLineId, true);

            var errorMessage = _commerceContextFake.GetMessage(m => m.Code == new KnownResultCodes().ValidationError);
            Assert.NotNull(errorMessage);
        }

        private Cart CreateInputCart(string cartLineId)
        {
            var cart = new Cart(TestCartId);
            var cartLine = new CartLineComponent()
            {
                 Id = cartLineId,
                 ItemId = TestSellableItemId,
                 Quantity = 1
            };
            cart.Lines.Add(cartLine);
            return cart;

        }
    }
}
