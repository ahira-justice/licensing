using System;

namespace Licensing.Models {

    [Serializable]
    public class LicenseKeyPrefsModel {

        public string Signature { get; set; }
        public string RawData { get; set; }
        public LicenseKeyDataModel Data { get; set; }
        public DateTime ActivatedAt { get; set; }

    }

}
