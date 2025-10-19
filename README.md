# NeoBundle

Command-line interface tools for bundling .NET projects into MacOS applications (.app)

### Installation

Install MSBuild task via NuGet package: `NeoBundle`

[![NuGet](https://img.shields.io/nuget/v/NeoBundle.svg)](https://www.nuget.org/packages/NeoBundle/)

```
<PackageReference Include="NeoBundle" Version="*"/>
```

### Using the tool

```
dotnet publish -r osx-x64 
```

### Properties

Define properties to override default bundle values

```
<PropertyGroup>
    <Version>1.0.2.8</Version>
    <Authors>Qianyiaz</Authors>
    <Copyright>By Qianyiaz</Copyright>
    <ApplicationIcon>app.icns</ApplicationIcon> <!-- Will be copied from output directory -->
</PropertyGroup>
```

More info: https://developer.apple.com/library/archive/documentation/CoreFoundation/Conceptual/CFBundles/BundleTypes/BundleTypes.html
