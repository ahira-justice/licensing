﻿using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Licensing.Constants;
using Licensing.Enums;
using Licensing.Exceptions;
using Licensing.Models;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.OpenSsl;
using Org.BouncyCastle.Security;

namespace Licensing {

    public static class LicenseChecker {

        // Overwrite with your chosen key size
        private static int KeySize { get => 2048; }
        private static int Month { get => 30; }
        private static string SignatureHashAlgorithm { get => "SHA256"; }

        private static void Initialize() {
            LicensePublicKey.Initialize();
            var keyFile = File.CreateText(LicensePublicKey.FileDirectory);
            var keyString = LicensePublicKey.KeyString;

            keyFile.Write(keyString);
            keyFile.Close();
        }

        private static RSAParameters GetPublicKeyFromPemFile(string filePath) {
            try {
                using TextReader publicKeyTextReader = new StringReader(File.ReadAllText(filePath));
                RsaKeyParameters publicKeyParam = (RsaKeyParameters) new PemReader(publicKeyTextReader).ReadObject();
                RSAParameters rsaParams = DotNetUtilities.ToRSAParameters(publicKeyParam);

                return rsaParams;
            }
            catch (Exception) {
                throw new PemFileInvalidException();
            }
        }

        private static bool VerifyDataIntegrity(string signature, string data) {
            try {
                var signatureBytes = Convert.FromBase64String(signature);
                var dataBytes = Convert.FromBase64String(data);

                using RSACryptoServiceProvider RSA = new RSACryptoServiceProvider(KeySize);
                RSA.ImportParameters(GetPublicKeyFromPemFile(LicensePublicKey.FileDirectory));
                return RSA.VerifyData(dataBytes, CryptoConfig.MapNameToOID(SignatureHashAlgorithm), signatureBytes);
            }
            catch (Exception) {
                throw new LicenseKeyDataIntegrityException();
            }
        }

        private static DateTime ParseISODate(string value) {
            var dateArray = value.Split("-").Select(n => Convert.ToInt32(n)).ToArray();
            return new DateTime(dateArray[0], dateArray[1], dateArray[2]);
        }

        public static LicenseKeyDataModel ParseLicenseKeyData(LicenseKeyModel licenseKey) {
            if (!IsServerLicenseValid(licenseKey))
                throw new LicenseKeyDataIntegrityException();

            var dataBytes = Convert.FromBase64String(licenseKey.RawData);
            var textData = Encoding.UTF8.GetString(dataBytes);
            var splittedTextData = textData.Split(", ");

            return new LicenseKeyDataModel {
                    Email = splittedTextData[0],
                    EpochTime = float.Parse(splittedTextData[1]),
                    Subscription = (SubscriptionTier) Int32.Parse(splittedTextData[2]),
                    Expiry = Int32.Parse(splittedTextData[3])
            };
        }

        public static bool IsClientLicenseValid(LicenseKeyPrefsModel licenseKeyPrefs) {
            Initialize();
            var result = VerifyDataIntegrity(licenseKeyPrefs.Signature, licenseKeyPrefs.RawData);
            File.Delete(LicensePublicKey.FileDirectory);
            return result;
        }

        public static bool IsServerLicenseValid(LicenseKeyModel licenseKey) {
            Initialize();
            var result = VerifyDataIntegrity(licenseKey.Signature, licenseKey.RawData);
            File.Delete(LicensePublicKey.FileDirectory);
            return result;
        }

        public static bool IsClientLicenseExpired(LicenseKeyPrefsModel licenseKeyPrefs) {
            var expiryDate = ParseISODate(licenseKeyPrefs.ActivatedAt) + TimeSpan.FromDays(Month * licenseKeyPrefs.Data.Expiry);

            if (DateTime.Now > expiryDate)
                return true;

            return false;
        }

        public static bool IsServerLicenseExpired(LicenseKeyModel licenseKey, DateTime now) {
            var expiryDate = now + TimeSpan.FromDays(Month * licenseKey.Data.Expiry);

            if (DateTime.Now > expiryDate)
                return true;

            return false;
        }
    }

}
