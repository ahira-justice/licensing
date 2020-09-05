using System;

namespace Licensing.Models {

    [Serializable]
    public class PermissionsModel {

        public SubscriptionTier Subscription { get; set; }

        public PermissionsModel(int subscriptionTier) {
            Subscription = (SubscriptionTier) subscriptionTier;
        }

    }

    [Serializable]
    public enum SubscriptionTier {

        Free = 1,
        Plus = 2,
        Pro = 3

    }

}
