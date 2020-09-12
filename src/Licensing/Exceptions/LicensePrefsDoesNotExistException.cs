using System;

namespace Licensing.Exceptions {
    public class LicensePrefsDoesNotExistException : Exception {
        public LicensePrefsDoesNotExistException() : base("LicensePrefs file does not exist") { }
    }
}
