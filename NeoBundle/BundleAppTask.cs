using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using Microsoft.Build.Framework;
using Task = Microsoft.Build.Utilities.Task;

namespace NeoBundle;

[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
[SuppressMessage("ReSharper", "AutoPropertyCanBeMadeGetOnly.Global")]
// ReSharper disable once ClassNeverInstantiated.Global
public class BundleAppTask : Task
{
    public string PublishDir { get; set; }
    public string OutDir { get; set; }
    public string AppName { get; set; }
    public string Version { get; set; }
    public string Copyright { get; set; }
    public string IconFile { get; set; }
    public string Authors { get; set; } = "example";

    public override bool Execute()
    {
#if RELEASE
        if(!RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            Log.LogError("App bundling is only supported on macOS.");
            return false;
        }
#endif
        
        var builder = new AppBuilder(this)
            .Build()
            .Bundle()
            .CreatePlist()
            .CreateDmg();
        
        Log.LogMessage(MessageImportance.High,
            $"{AppName}.app -> {builder.AppDirectory}");
        Log.LogMessage(MessageImportance.High,
            $"{AppName}.dmg -> {builder.DmgPath}");
        return true;
    }
}