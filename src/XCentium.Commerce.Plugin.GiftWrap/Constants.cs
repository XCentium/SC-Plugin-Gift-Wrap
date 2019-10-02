namespace XCentium.Commerce.Plugin.GiftWrap
{
    /// <summary>
    /// Constants used by the gift wrap plugin.
    /// </summary>
    public static class Constants
    {
        public static class Actions
        {
            public static class UpdateCartLineGiftBox
            {
                public const string Name = "UpdateCartLineGiftBox";
                public const string Route = "UpdateCartLineGiftBox()";
                public const string CartId = "cartId";
                public const string CartLineId = "cartLineId";
                public const string IsGiftBox = "isGiftBox";
            }
        }

        public static class Adjustments
        {
            public static class GiftBox
            {
                public const string Name = "GiftBoxFee";
                public const string AdjustmentType = "GiftBox";
            }
        }

        public static class AvailableValues
        {
            public static string Yes = "Yes";
            public static string No = "No";
        }
    }
}
