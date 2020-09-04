using System;

namespace Licensing.Models {

    [Serializable]
    public class LicenseKeyModel {

        public string Signature { get; }
        public string Data { get; }
        public string ActivatedAt { get; set; }
        public int Expiry { get; }

        public LicenseKeyModel() { }

        public LicenseKeyModel(string signature, string data, int expiry) {
            Signature = signature;
            Data = data;
            Expiry = expiry;
        }

    }
    
}
