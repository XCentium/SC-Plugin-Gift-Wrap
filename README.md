# Sitecore Commerce Gift Wrap Plugin

The plugin allows to add gift wrapping functionality to your Sitecore Commerce implementation.
- This integrates with and extends commerce Carts plugin functionality by adding gift box `CartLevelAwardedAdjustment` to a `Cart` object.
- The plugin includes back-end functionality that can be integrated with your Sitecore components using `Sitecore.Commerce.ServiceProxy`.
- Developed for Commerce 9 Update-3; however, Sitecore Commerce NuGet packages can be updated to match your installed version.

## Sponsor 

This plugin was sponsored and created by XCentium.

## How to Install

1. Copy `XCentium.Commerce.Plugin.GiftWrap` and `Sitecore.Commerce.ServiceProxy` to your Sitecore Commerce Engine solution and add as projects.
2. Add `XCentium.Commerce.Plugin.GiftWrap` as a dependency to your `Sitecore.Commerce.Engine` project.
3. If your Sitecore Commerce Engine solution includes other custom plugins that modify ServiceProxy then re-execute the update of Connected Services inside of `Sitecore.Commerce.ServiceProxy` project.
4. Bootstrap and deploy the Sitecore Commerce Engine.

## How to Use

### Configure Sellable Items in BizFX

When working in Merchandising sectiong of BizFX, you can configure gift wrapping for a sellable item.  
  
![alt text](https://github.com/XCentium/SC-Plugin-Gift-Wrap/blob/master/images/bizfx-gift-wrap.png)

### UpdateCartLineGiftBox Commerce Command

An example of using UpdateCartLineGiftBox ServiceProxy command to add gift wrapping to a cart line.
```csharp
var cartId = "Some-Cart-Id";
var sellableItemId = "Some-Sellable-Item-Id";

var addLineResult = Proxy.DoCommand(container.AddCartLine(cartId, sellableItemId, 1));

var lineAdded = (LineAdded)addLineResult.Models.Where(m => m.GetType() == typeof(LineAdded)).FirstOrDefault();

var lineGiftBoxResult = Proxy.DoCommand(container.UpdateCartLineGiftBox(cartId, lineAdded.LineId, true));
```

### GiftBoxFee Cart Adjustment

When working with a `Cart` object, `Adjustments` collection will contain `GiftBoxFee` adjustment. Cart totals will be calculated to include the adjustment as well.
```json
{
    "AdjustmentType":"GiftBox",
    "Adjustment": {
        "CurrencyCode":"USD",
        "Amount":4.25
    },
    "AwardingBlock":"GiftWrap.Block.CalculateCartGiftBoxAdjustment",
    "IsTaxable":false,
    "IncludeInGrandTotal":true,
    "Name":"GiftBoxFee",
    "DisplayName":"GiftBoxFee"
}
```

### BizFX Order Details

Order details Adjustments section will contain GiftBox adjustment.  
  
![alt text](https://github.com/XCentium/SC-Plugin-Gift-Wrap/blob/master/images/bizfx-order-details.png)

## Note

- If you have any questions, comment or need us to help install, extend or adapt this plugin to your needs, do not hesitate to reachout to us at XCentium.