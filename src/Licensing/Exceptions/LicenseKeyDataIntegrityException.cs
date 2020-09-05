using System;

namespace Licensing.Exceptions {
    public class LicenseKeyDataIntegrityException : Exception {
        public LicenseKeyDataIntegrityException() : base("License key data integrity compromised") { }
    }
}
