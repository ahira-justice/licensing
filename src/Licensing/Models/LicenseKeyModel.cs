namespace Licensing.Models {

    public class LicenseKeyModel {

        public string Signature { get; set; }
        public string RawData { get; set; }
        public LicenseKeyDataModel Data { get; set; }

    }

}
