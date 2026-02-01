using System.Diagnostics;
using System.Text;
using System.Xml;

namespace NeoBundle
{
    public class AppBuilder(BundleAppTask task)
    {
        public string AppDirectory => Path.Combine(task.PublishDir, $"{task.AppName}.app");
        public string DmgPath => Path.Combine(task.PublishDir, $"{task.AppName}.dmg");
        private string ContentsDirectory => Path.Combine(AppDirectory, "Contents");
        private string MacosDirectory => Path.Combine(ContentsDirectory, "MacOS");
        private string ResourcesDirectory => Path.Combine(ContentsDirectory, "Resources");

        public AppBuilder Build()
        {
            if (Directory.Exists(AppDirectory))
                Directory.Delete(AppDirectory, true);
            
            Directory.CreateDirectory(AppDirectory);
            Directory.CreateDirectory(ContentsDirectory);
            Directory.CreateDirectory(MacosDirectory);
            Directory.CreateDirectory(ResourcesDirectory);
            return this;
        }
        
        public AppBuilder Bundle()
        {
            CopyIcon(
                new (task.OutDir),
                new (ResourcesDirectory));
            CopyFiles(
                new (task.PublishDir),
                new (MacosDirectory),
                new (AppDirectory));
            return this;
        }

        private void CopyFiles(DirectoryInfo source, DirectoryInfo target, DirectoryInfo exclude)
        {
            Directory.CreateDirectory(target.FullName);

            foreach (var fileInfo in source.GetFiles())
            {
                var path = Path.Combine(target.FullName, fileInfo.Name);
                fileInfo.CopyTo(path,true);
            }

            foreach (var sourceSubDir in source.GetDirectories())
            {
                if (sourceSubDir.FullName == exclude.FullName) continue;
                var targetSubDir = target.CreateSubdirectory(sourceSubDir.Name);
                CopyFiles(sourceSubDir, targetSubDir, exclude);
            }
        }

        private void CopyIcon(DirectoryInfo source, DirectoryInfo destination)
        {
            var iconName = task.IconFile;
            if (string.IsNullOrWhiteSpace(iconName))
                return;
            
            var sourcePath = Path.Combine(source.FullName, iconName);
            var targetPath = Path.Combine(destination.FullName, Path.GetFileName(iconName));
            var sourceFile = new FileInfo(sourcePath);
            
            if (!sourceFile.Exists) return;
            sourceFile.CopyTo(targetPath);
        }
        
        public AppBuilder CreatePlist()
        {
            var infoPlistPath = Path.Combine(ContentsDirectory, "Info.plist");

            var parts = task.Version.Split('.');
            var shortVersion = parts.Length <= 2 ? task.Version : $"{parts[0]}.{parts[1]}";

            using (var writer = XmlWriter.Create(infoPlistPath, new()
                   {
                       Indent = true,
                       Encoding = Encoding.UTF8,
                       OmitXmlDeclaration = false
                   }))
            {
                writer.WriteDocType("plist", "-//Apple//DTD PLIST 1.0//EN",
                    "http://www.apple.com/DTDs/PropertyList-1.0.dtd", null);

                writer.WriteStartElement("plist");
                writer.WriteAttributeString("version", "1.0");
                writer.WriteStartElement("dict");

                WriteKeyValuePair(writer, "CFBundleExecutable", task.AppName);
                WriteKeyValuePair(writer, "CFBundleIdentifier",
                    $"com.{task.Authors.ToLowerInvariant()}.{task.AppName.ToLowerInvariant()}");
                WriteKeyValuePair(writer, "CFBundleName", task.AppName);
                WriteKeyValuePair(writer, "CFBundleVersion", task.Version);
                WriteKeyValuePair(writer, "CFBundleShortVersionString", shortVersion);
                WriteKeyValuePair(writer, "CFBundlePackageType", "APPL");
                WriteKeyValuePair(writer, "LSApplicationCategoryType", "public.app-category.productivity");
                WriteKeyValuePair(writer, "CFBundleIconFile", Path.GetFileName(task.IconFile));
                WriteKeyValuePair(writer, "NSHumanReadableCopyright", task.Copyright);

                writer.WriteEndElement();
                writer.WriteEndElement();
            }
            return this;
        }

        private void WriteKeyValuePair(XmlWriter writer, string key, string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return;
            writer.WriteElementString("key", key);
            writer.WriteElementString("string", value);
        }

        public AppBuilder CreateDmg()
        {
            StartProcess("/bin/ln", $"-s /Applications \"{Path.Combine(AppDirectory, "Applications")}\"");
            
            StartProcess("/usr/bin/hdiutil", $"create -volname \"{task.AppName}\" -srcfolder \"{AppDirectory}\" -ov -format UDZO \"{DmgPath}\"");
            return this;
        }
        
        private void StartProcess(string fileName, string arguments)
        {
            using var proc = Process.Start(new ProcessStartInfo
            {
                FileName = fileName,
                Arguments = arguments,
                UseShellExecute = false,
                CreateNoWindow = true
            });
            proc?.WaitForExit();
        }
    }
}