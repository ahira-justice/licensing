# licensing

licensing is a .NET Standard class library for licensing and managing permissions in desktop applications.

## Licensing Scheme

The license key is produced by the accompanying backend service. The license key scheme prescribed for this project is based on the principles of asymmetric cryptography, signature and verification. The license key comprises of two parts; a data string, and a RSA private key signature of that data. The public key is to be shipped with the software and is included with this project.

Read an overview of the licensing scheme [here](https://blog.ahirajustice.com/licensing-a-desktop-application-and-the-cryptography-that-powers-it-ckb12u7tw038fl6s18szyyglx).

## Data

The license data will be in csv format and encoded as a base 64 string.

```sh
email, epochTime, expiry, subscriptionTier
```

The `email` and `epochTime` values are of little use on the client, but they serve to provide a data string unique to each license. The `epochTime` value is the time of license purchase/creation. The `expiry` value is also crucial to the implementation of the licensing scheme. Any subsequent values added on are to convey information about the user's license and make decisions within the application. For this project, I have included `subscriptionTier`. The subscription tier value is an integer representing the different access levels or tiers of the application's usage. There is no restriction on what this value can be, as long as it matches the `Licensing.Enums.SubscriptionTier` Enum values.

You can extend this implementation to include values unique to your use case. For this, you will need to modify `Licensing.Models.LicenseKeyDataModel` and `Licensing.LicenseChecker.ParseLicenseKeyData()`. You can also extend the subscription tier information by modifying `Licensing.Enums.SubscriptionTier`,

## Signature

The license signature is also encoded as a base 64 string.

Verifying this signature against the data is what confers validity on the license key. You verify the license by making a call to `Licensing.LicenseChecker.IsServerLicenseValid()` or `Licensing.LicenseChecker.IsClientLicenseValid()`. The former accepts as a parameter, the license key from the server [[1](#1.-fetch-license-key-from-server)], while the latter accepts the license stored to persistent storage. Both methods have a `bool` return type.

## Expiry

The expiry value is an integer representing the number of months a license is valid for. If you have a different use case, say weeks or years, you could modify the `Licensing.LicenseChecker.IsLicenseExpired()` method to use one of `Licensing.LicenseChecker.Year`, `Licensing.LicenseChecker.Month`, or `Licensing.LicenseChecker.Week` constant properties.

To check the expiration status of a license, make a call to `Licensing.LicenseChecker.IsLicenseExpired()`. This method takes the license stored to persistent storage as a parameter.

## Persistence

Use the `Licensing.LicenseChecker.ParseLicenseKeyPrefs()` method to convert your server gotten license key into a `Licensing.Models.LicenseKeyPrefsModel`[[2](#2.-one-time-internet-connection)] object which can be serialized to disk and stores the license key data on the user machine.

`Licensing.LicenseCore` provides static methods for saving and retrieving the license key to and from disk.

## Exceptions

There is a collection of custom exception classes in `Licensing.Exceptions`. These exceptions serve to report on the unique type of error and allows your application more robust exception handling.

How you handle these exceptions and where is entirely up to you.

## Constants

You are to set the RSA public key key-string in `Licensing.Constants.LicensePublicKey.Initialize()`. This is best done as a string assignment. You could alternatively set up a config or env file that feeds into this assignment. Protection of the public key is not priority, or even practical. This project is meant for client side deployment and anyone with time and a decompiler can extract that key-string[[3](#3.-cracks-and-keygens)]. 

## Example

#### License Key Data

```sh
"user@example.com, 1600163621.4321098, 12, 2"
```

#### Code Snippet

```sh
using System;
using Licensing;
using Licensing.Models;

var licenseKey = new LicenseKeyModel {
    Signature="license-signature-string-fkwPBfjdcwQmspfokGDWewdFGfkmclwdfopewgihVJnHGempofdiDSewfSFh",
    Data="license-data-string-sfnDdljfe3GbwoK9sLmfJiurhbgfbDSunw"
};

Console.WriteLine(LicenseChecker.IsServerLicenseValid(licenseKey));

var licenseKeyPrefs = LicenseChecker.ParseLicenseKeyPrefs(licenseKey);

LicenseCore.SaveLicensePrefs(licenseKeyPrefs);
var prefs = LicenseCore.GetCurrentLicensePrefs();

Console.WriteLine(prefs.Data.Email);
Console.WriteLine(prefs.Data.EpochTime);
Console.WriteLine(prefs.Data.Subscription.ToString());
Console.WriteLine(prefs.Data.Expiry);

Console.WriteLine(LicenseChecker.IsClientLicenseValid(prefs));
Console.WriteLine(LicenseChecker.IsLicenseExpired(prefs));
```

#### Output

```sh
True
user@example.com
1600163621.4321098
Plus
12
True
False
```

## Usage

Download the repo as zip. Extract the contents into your solution directory. Add the licensing project to your solution.

```sh
$ dotnet sln <SLN_FILE> add src/Licensing/Licensing.csproj
```

## Dependencies

licensing uses the [Bouncy Castle](https://www.bouncycastle.org/csharp/index.html) cryptography library. A DLL is included with this repo in the /lib directory.

## Footnotes

#### 1. Fetch License Key from Server

You could deliver the license key to the client through two channels. One channel would be over http/https through a web API. The other is as a plain text license file containing the signature and data downloaded from the server.

Your choice will depend on your constraints and use cases. Either way, you will need to implement your own wrapper to retrieve the license key and convert it to an object of `Licensing.Models.LicenseKeyModel`.

#### 2. One-time Internet Connection

In certain scenarios, there is a lag between the time a license is purchased and when it is first activated. If you are only interested in the purchase date, then `Licensing.Models.LicenseKeyPrefsModel.Data`'s `EpochTime` property holds that information. You can convert this value to `System.DateTime` and assign it to `Licensing.Models.LicenseKeyPrefsModel.ActivatedAt` before calling `Licensing.LicenseCore.SaveLicensePrefs()`.

In other scenarios, you might be interested in the activation date of the license instead. For instance, where a reseller program is in place. In this case, you would want the validity of a license to begin at activation and not at purchase. You could set up a one-time internet connection to your server for this purpose. One way would be to store the "ActivatedAt" value for each license on the server. Subsequent use of the same license would have an attached "ActivatedAt" value and you will be able to implement validity properly across devices.

Now, this server "ActivatedAt" value is not private key signed and could be subject to tampering. It is up to you to establish a secure channel of communication between your server and the client. 

#### 3. Cracks and Keygens

You can set up a system that allows updating the asymmetric key pair for each different version of your application. The private keys of all the different versions will be managed by the server, and the corresponding public keys deployed with each respective version. It is important that you get this match right. It is also important that you protect your private key(s). Anyone with a private key can generate limitless amounts of license keys for that version of your software (keygen).

Your application can be cracked if decompiled and your access control statements are bypassed. I recommend looking into obfuscation of critical code sections to make the process a little more difficult.

## Further Reading

Read this [article](https://build-system.fman.io/generating-license-keys) by Michael Herrmann about generating license keys on the server side.
