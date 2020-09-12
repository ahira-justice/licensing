using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Licensing.Exceptions;
using Licensing.Models;

namespace Licensing {

    public static class LicenseCore {

        private static string _preferenceDirectory;

        private static string BaseDirectory { get; set; }
        private static string AppDirectory { get; set; }
        private static string LicensePrefsFileName { get; set; }
        private static string LicensePrefsDirectory {
            get => _preferenceDirectory;
            set {
                if (!Directory.Exists(value)) {
                    Directory.CreateDirectory(value);
                    _preferenceDirectory = value;
                }
                else if (Directory.Exists(value)) {
                    _preferenceDirectory = value;
                }
            }
        }

        private static void Initialize() {
            BaseDirectory = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            AppDirectory = @"\App";

            LicensePrefsDirectory = BaseDirectory + AppDirectory + @"\Data";
            LicensePrefsFileName = LicensePrefsDirectory + @"\LicensePrefs.bin";
        }

        public static LicenseKeyPrefsModel GetCurrentLicensePrefs() {
            Initialize();

            try {
                using Stream openFileStream = File.OpenRead(LicensePrefsFileName);
                BinaryFormatter deserializer = new BinaryFormatter();

                var licensePrefs = (LicenseKeyPrefsModel) deserializer.Deserialize(openFileStream);
                openFileStream.Close();

                return licensePrefs;
            }
            catch (Exception ex) when(ex is FileNotFoundException) {
                throw new LicensePrefsDoesNotExistException();
            }
            catch (Exception) {
                throw new LicensePrefsInvalidException();
            }
        }

        public static void SaveLicensePrefs(LicenseKeyPrefsModel value) {
            Initialize();
            Stream saveFileStream = File.Create(LicensePrefsFileName);
            BinaryFormatter serializer = new BinaryFormatter();

            serializer.Serialize(saveFileStream, value);
            saveFileStream.Close();
        }

    }

}
