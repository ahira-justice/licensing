using System.IO;
using Licensing.Exceptions;

namespace Licensing.Constants {
    internal static class LicensePublicKey {
        private static string _keyString;

        public static string KeyString {
            get {
                if (string.IsNullOrEmpty(_keyString))
                    throw new LicensePublicKeyNotDefinedException();

                return _keyString;
            }
            set {
                // Set your custom public key here
                _keyString = "";
            }
        }

        public static string FileDirectory {
            get {
                return Directory.GetCurrentDirectory() + @"\public_key.pem";
            }
        }
    }
}
