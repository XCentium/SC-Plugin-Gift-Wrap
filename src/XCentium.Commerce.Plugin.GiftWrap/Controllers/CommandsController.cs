using System;
using System.Threading.Tasks;
using System.Web.Http.OData;
using Microsoft.AspNetCore.Mvc;
using Sitecore.Commerce.Core;
using XCentium.Commerce.Plugin.GiftWrap.Commands;
using static XCentium.Commerce.Plugin.GiftWrap.Constants;

namespace XCentium.Commerce.Plugin.GiftWrap.Controllers
{
    /// <summary>
    /// OData commerce controller for the action to update cart line gift box attribute.
    /// </summary>
    public class CommandsController : CommerceController
    {
        public CommandsController(IServiceProvider serviceProvider, CommerceEnvironment globalEnvironment)
            : base(serviceProvider, globalEnvironment)
        {
        }

        [HttpPut]
        [Route(Actions.UpdateCartLineGiftBox.Route)]
        public async Task<IActionResult> UpdateCartLineGiftBox([FromBody] ODataActionParameters value)
        {
            if (!ModelState.IsValid)
            {
                return new BadRequestObjectResult(ModelState);
            }

            if (!value.ContainsKey(Actions.UpdateCartLineGiftBox.CartId) || string.IsNullOrEmpty(value[Actions.UpdateCartLineGiftBox.CartId].ToString()) ||
                !value.ContainsKey(Actions.UpdateCartLineGiftBox.CartLineId) || string.IsNullOrEmpty(value[Actions.UpdateCartLineGiftBox.CartLineId].ToString()) ||
                !value.ContainsKey(Actions.UpdateCartLineGiftBox.IsGiftBox))
            {
                return new BadRequestObjectResult(value);
            }

            var cartId = value[Actions.UpdateCartLineGiftBox.CartId].ToString();
            var cartLineId = value[Actions.UpdateCartLineGiftBox.CartLineId].ToString();
            var isGiftBox = (bool)value[Actions.UpdateCartLineGiftBox.IsGiftBox];

            var command = Command<UpdateCartLineGiftBoxCommand>();
            var commandResult = await command.Process(CurrentContext, cartId, cartLineId, isGiftBox);

            return new ObjectResult(command);
        }
    }
}

