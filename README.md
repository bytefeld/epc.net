# Bytefeld EPC

`Bytefeld EPC` is an EPC [Electronic Product Code](https://en.wikipedia.org/wiki/Electronic_Product_Code) Encoder/Decoder library written in C#, with support for .NET Framework 3.5 and .NET Standard 1.4

It aims to support the latest [EPC TAG Data Standard](http://www.gs1.org/gsmp/kc/epcglobal/tds/) with focus on id and tag uri handling.


## Install

```Powershell
Install-Package bytefeld.epc -Version 0.5.0-beta2
```

## Usage

Use the EpcTag class as an convenient entry point to the library.

Converting from binary tag into an EPC tag uri:

```csharp
string uri = EpcTag.FromBinary("302D28B329B0F6C000000001").ToString();
// uri is "epc:tag:sgtin-96:1.311112347.0987.1"
```

Converting from uri to binary representation:

```csharp
string tag = EpcTag.FromUri("epc:tag:sgtin-96:1.311112347.0987.1").ToBinary();
// tag is "302D28B329B0F6C000000001"
```

Accessing properties of a typed tag:

```csharp
var tag = EpcTag.FromBinary<Sgtin96>("302D28B329B0F6C000000001");
string customerPrefix = tag.CustomerPrefix;
```


## Supported Tags

The following tags are currently supported by Bytefeld EPC:

* SGTIN-96
* SGTIN-198
* SSCC-96

The following tags are currently not supported:

* SGLN-96
* GRAI-96
* GRAI-170
* GSRN-96
* GDTI-96
* GDTI-113
* GID-96
* DOD
* ADI
