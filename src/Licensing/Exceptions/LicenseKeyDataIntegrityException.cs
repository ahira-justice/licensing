using System;

namespace Licensing.Exceptions {
    public class LicenseKeyDataIntegrityException : Exception {
        public LicenseKeyDataIntegrityException() : base("LicenseKeyData integrity compromised") { }
    }
}
