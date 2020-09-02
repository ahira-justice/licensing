using System;

namespace Licensing.Exceptions {
    public class LicensePublicKeyNotDefinedException : Exception {
        public LicensePublicKeyNotDefinedException() : base("License public key is undefined. Assign a value to License.Constants.LicensePublicKey.KeyString") { }
    }
}
