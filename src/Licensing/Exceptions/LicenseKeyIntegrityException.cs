using System;

namespace Licensing.Exceptions {
    public class LicenseKeyIntegrityException : Exception {
        public LicenseKeyIntegrityException() : base("License key signature or data integrity compromised") { }
    }
}
