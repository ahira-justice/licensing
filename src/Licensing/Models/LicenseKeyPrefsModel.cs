using System;

namespace Licensing.Models {

    [Serializable]
    public class LicenseKeyPrefsModel {

        public string Signature { get; set; }
        public string RawData { get; set; }
        public LicenseKeyDataModel Data { get; set; }
        public string ActivatedAt { get; set; }
        public int Expiry { get; set; }

    }

}
