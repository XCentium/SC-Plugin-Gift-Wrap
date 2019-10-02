using Sitecore.Commerce.Core;

namespace XCentium.Commerce.Plugin.GiftWrap.Policies
{
    public class GiftBoxFeeAdjustmentPolicy : Policy
    {
        public decimal FeeAmount { get; set; }

        public bool IsTaxable { get; set; }

        public GiftBoxFeeAdjustmentPolicy()
        {
            FeeAmount = 4.25M;
            IsTaxable = false;
        }
    }
}
