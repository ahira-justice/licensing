using System;
using Licensing.Enums;

namespace Licensing.Models {

    [Serializable]
    public class LicenseKeyDataModel {

        public string Email { get; set; }
        public float EpochTime { get; set; }
        public SubscriptionTier Subscription { get; set; }
        public int Expiry { get; set; }

    }
}
