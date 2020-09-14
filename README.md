# licensing

licensing is a .NET Standard class library for licensing and managing permissions in desktop applications.

## Licensing Scheme

The license key is produced by the accompanying backend service. The license key scheme prescribed for this project is based on the principles of asymmetric cryptography, and signature and verification. The license key comprises of two parts; a data string, and a RSA private key signature of that data. The public key is to be shipped with the software by including it in this project.

Read an overview of the licensing scheme [here](https://blog.ahirajustice.com/licensing-a-desktop-application-and-the-cryptography-that-powers-it-ckb12u7tw038fl6s18szyyglx).

## Data

The license data will be in csv format and encoded as a base 64 string.

```sh
email, epochTime, expiry, subscriptionTier
```

The `email` and `epochTime` values are of little use on the client, but they serve to provide a data string unique to each license. The `expiry` value is also crucial to the implementation of the licensing scheme. Any subsequent values added on are to convey information about the user's license and make decisions within the application. For this project, I have included `subscriptionTier`. The subscription tier value is an integer representing the different access levels or tiers of the application's usage. There is no restriction on what this value can be, as long as it matches the `Licensing.Enums.SubscriptionTier` Enum values.

You can extend this implementation to include values unique to your use case. For this, you will need to modify `Licensing.Models.LicenseKeyDataModel` and `Licensing.LicenseChecker.ParseLicenseKeyData()`. You can also extend the subscription tier information by modifying `Licensing.Enums.SubscriptionTier`,

## Signature

The license signature is also encoded as a base 64 string.

Verifying this signature against the data is what confers validity on the license key. You verify the license by making a call to `Licensing.LicenseChecker.IsServerLicenseValid()` or `Licensing.LicenseChecker.IsClientLicenseValid()`. The former accepts as a parameter, the license key from the server [[1](#1.-fetch-license-key-from-server)], while the latter accepts the license stored to persistent storage. Both methods have a `bool` return type that signifies an answer to the question posed in the method name.

## Expiry

The expiry value is an integer representing the number of months a license is valid for. If you have a different use case, say weeks or years, you could modify the `Licensing.LicenseChecker.IsLicenseExpired()` method to use one of `Licensing.LicenseChecker.Year`, `Licensing.LicenseChecker.Month`, or `Licensing.LicenseChecker.Week` constant properties.

To check the expiration status of a license, make a call to `Licensing.LicenseChecker.IsLicenseExpired()`. This method takes the license stored to persistent storage as a parameter.

## Persistence

Use the `Licensing.LicenseChecker.ParseLicenseKeyPrefs()` method to convert your server gotten license key into a `Licensing.Models.LicenseKeyPrefsModel` object which can be serialized to disk and stores the license key data on the user machine.

`Licensing.LicenseCore` provides static methods for saving and retrieving the license key to and from disk.

## Example

```sh
var licenseKey = new LicenseKeyModel {
    Signature="license-signature-string",
    Data="license-data-string"
};

System.Console.WriteLine(LicenseChecker.IsServerLicenseValid(licenseKey));

var licenseKeyPrefs = LicenseChecker.ParseLicenseKeyPrefs(licenseKey);

LicenseCore.SaveLicensePrefs(licenseKeyPrefs);
var prefs = LicenseCore.GetCurrentLicensePrefs();

System.Console.WriteLine(prefs.Data.Email);
System.Console.WriteLine(prefs.Data.EpochTime);
System.Console.WriteLine(prefs.Data.Subscription);
System.Console.WriteLine(prefs.Data.Expiry);

System.Console.WriteLine(LicenseChecker.IsClientLicenseValid(prefs));
System.Console.WriteLine(LicenseChecker.IsLicenseExpired(prefs));
```

## Usage

Download the repo as zip. Extract the contents into your solution directory. Add the licensing project to your solution.

```sh
$ dotnet sln <SLN_FILE> add src/Licensing/Licensing.csproj
```

## Footnote

#### 1. Fetch License Key from Server

You could deliver the license key to the client through two channels. One channel would be over http/https through a web API. The other is as a plain text license file containing the signature and data downloaded from the server.

Your choice will depend on your constraints and use cases. Either way, you will need to implement your own wrapper to retrieve the license key and convert it to an object of `Licensing.Models.LicenseKeyModel`.

#### 2. One-time Internet Connection

## Further Reading

Read this [article](https://build-system.fman.io/generating-license-keys) by Michael Herrmann about generating license keys on the server side.
