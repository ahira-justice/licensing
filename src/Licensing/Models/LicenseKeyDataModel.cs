using System;

namespace Licensing.Models {

    [Serializable]
    public class LicenseKeyDataModel {

        public string Email { get; set; }
        public float EpochTime { get; set; }
        public PermissionsModel Permissions { get; set; }

    }
}
