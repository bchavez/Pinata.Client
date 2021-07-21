[![Build status](https://ci.appveyor.com/api/projects/status/tp8iyv60j05q8hfl/branch/master?svg=true)](https://ci.appveyor.com/project/bchavez/pinata-client) [![Nuget](https://img.shields.io/nuget/v/Pinata.Client.svg)](https://www.nuget.org/packages/Pinata.Client/) [![Users](https://img.shields.io/nuget/dt/Pinata.Client.svg)](https://www.nuget.org/packages/Pinata.Client/) <img src="https://raw.githubusercontent.com/bchavez/Pinata.Client/master/Docs/pinata.png" align='right' />

Pinata.Client for .NET/C# Library
======================

Project Description
-------------------
A .NET implementation for [Pinata Cloud](https://pinata.cloud/documentation).

[1]:https://docs.microsoft.com/en-us/mem/configmgr/core/plan-design/security/enable-tls-1-2-client
[2]:https://docs.microsoft.com/en-us/dotnet/framework/network-programming/tls
#### Minimum Requirements
* **.NET Standard 2.0** or later
* **.NET Framework 4.5** or later
* **TLS 1.2** or later

#### Crypto Tip Jar
<a href="https://commerce.coinbase.com/checkout/53604209-ec9d-47b1-9b81-b3ea04a6f8c6"><img src="https://raw.githubusercontent.com/bchavez/Pinata.Client/master/Docs/tipjar.png" /></a>



### Download & Install
**Nuget Package [Pinata.Client](https://www.nuget.org/packages/Pinata.Client/)**

```powershell
Install-Package Pinata.Client
```

Getting Started
------

```csharp
var config = new Config
   {
      ApiKey = "2981f1eb1813daf...",
      ApiSecret = "42281fa28de32fe3c..."
   };

var client = new PinataClient(config);

var html = @"
<html>
   <head>
      <title>Hello IPFS!</title>
   </head>
   <body>
      <h1>Hello World</h1>
   </body>
</html>
";

var metadata = new PinataMetadata // optional
   {
      KeyValues =
         {
            {"Author", "Brian Chavez"}
         }
   };

var options = new PinataOptions(); // optional

options.CustomPinPolicy.AddOrUpdateRegion("NYC1", desiredReplicationCount: 1);

var response = await this.client.Pinning.PinFileToIpfsAsync(content =>
      {
         var file = new StringContent(html, Encoding.UTF8, MediaTypeNames.Text.Html);

         content.AddPinataFile(file, "index.html");
      },
   metadata,
   options);

if( response.IsSuccess )
{
   //File uploaded to Pinata Cloud and can be accessed on IPFS!
   var hash = response.IpfsHash; // QmR9HwzakHVr67HFzzgJHoRjwzTTt4wtD6KU4NFe2ArYuj
}
```

Now your file can be accessed over [IPFS](https://ipfs.io/) and accessed via [Cloudflare's IPFS gateway](https://cloudflare-ipfs.com/)!

```
https://cloudflare-ipfs.com/ipfs/QmR9HwzakHVr67HFzzgJHoRjwzTTt4wtD6KU4NFe2ArYuj
```

Find more examples [here](https://github.com/bchavez/Pinata.Client/blob/master/Source/Pinata.Client.Tests/IntegrationTests/IntegrationTest.cs).

-------


Magic! Easy peasy! **Happy file sharing!** :tada:


Reference
---------
* [Pinata Documentation](https://pinata.cloud/documentation)


Building
--------
* Download the source code.
* Run `build.cmd`.

Upon successful build, the results will be in the `\__compile` directory. If you want to build NuGet packages, run `build.cmd pack` and the NuGet packages will be in `__package`.



Contributors
---------
Created by [Brian Chavez](http://www.bitarmory.com).

A big thanks to GitHub and all contributors!

---

*Note: This application/third-party library is not directly supported by Pinata Technologies, Inc. Pinata Technologies, Inc. makes no claims about this application/third-party library.  This application/third-party library is not endorsed or certified by Pinata Technologies, Inc.*
