using System.Diagnostics.CodeAnalysis;
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
        new AppBuilder(this)
            .Build()
            .Bundle()
            .CreatePlist();
        Log.LogMessage(MessageImportance.High,
            $"{AppName}.app -> {Path.Combine(Path.GetFullPath(PublishDir), $"{AppName}.app")}");
        return true;
    }
}