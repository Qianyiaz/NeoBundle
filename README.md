# NeoBundle

Command-line interface tools for bundling .NET projects into MacOS applications (.app)

### Installation

Install MSBuild task via NuGet package: `NeoBundle`

[![NuGet](https://img.shields.io/nuget/v/NeoBundle.svg)](https://www.nuget.org/packages/NeoBundle/)

```xml
<PackageReference Condition="$(RuntimeIdentifier.StartsWith('osx'))" Include="NeoBundle" Version="*"/>
```

### Properties

Define properties to override default bundle values

```xml
<PropertyGroup> <!-- All can be empty -->
    <Version>1.0.2.8</Version>
    <Authors>Qianyiaz</Authors>
    <Copyright>By Qianyiaz</Copyright>
    <ApplicationIcon>app.icns</ApplicationIcon> <!-- Will be copied from output directory -->
</PropertyGroup>

<ItemGroup>
    <None Include="app.icns" CopyToPublishDirectory="Always" />
</ItemGroup>
```

More info: https://developer.apple.com/library/archive/documentation/CoreFoundation/Conceptual/CFBundles/BundleTypes/BundleTypes.html
