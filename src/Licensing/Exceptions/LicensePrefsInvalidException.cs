using System;

namespace Licensing.Exceptions {
    public class LicensePrefsInvalidException : Exception {
        public LicensePrefsInvalidException() : base("LicensePrefs file is invalid or corrupted") { }
    }
}
