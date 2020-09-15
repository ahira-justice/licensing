using System;
using Licensing.Enums;

namespace Licensing.Models {

    [Serializable]
    public class LicenseKeyDataModel {

        public string Email { get; set; }
        public double EpochTime { get; set; }
        public int Expiry { get; set; }
        public SubscriptionTier Subscription { get; set; }

    }
}
