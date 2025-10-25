using System.Xml.Linq;
using WixSharp;
using File = WixSharp.File;

namespace WixInstaller;

public class Program
{
    public static void Main(string[] args)
    {
        Compiler.AllowNonRtfLicense = true;

        string installerProjectDir = Directory.GetCurrentDirectory();
        Console.WriteLine($"CurrentDirectory '{installerProjectDir}'.");
        string solutionDir = Path.GetFullPath(Path.Combine(installerProjectDir, ".."));
        Console.WriteLine($"SolutionDirectory '{solutionDir}'.");
        string buildConfiguration = GetBuildConfiguration();
        Console.WriteLine(buildConfiguration);

        string csprojPath = $@"{solutionDir}\ExtrabbitCode.Inventor.Core.Template\ExtrabbitCode.Inventor.Core.Template.csproj";
        Console.WriteLine(csprojPath);

        string appVersion = GetVersionFromCsproj(csprojPath);
        string appName = $"ExtrabbitCode.Inventor.Core.Template {appVersion}";
        Console.WriteLine(appVersion);
        const string company = "YOUR COMPANY";
        const string appUrl = "YOUR COMPANY URL";

        string addInFile = Path.Combine(solutionDir, "ExtrabbitCode.Inventor.Core.Template", "Addin", "ExtrabbitCode.Inventor.Core.Template.addin");
        Console.WriteLine("AddInFile: " + addInFile);

        string binaryFiles = Path.Combine(solutionDir, "ExtrabbitCode.Inventor.Core.Template", @"bin\Release\*.*");
        Console.WriteLine("BinaryFiles: " + binaryFiles);

        Project project = new Project(appName,
            new Dir(@"C:\ProgramData\Autodesk\Inventor Addins",
                new File(addInFile)
                {
                    Permissions =
                    [
                        new FilePermission("Everyone", GenericPermission.All)
                    ]
                }),
            new Dir(@"C:\ProgramData\ExtrabbitCode.Inventor.Core.Template",
                new Files(binaryFiles))
            {
                Permissions = new[]
                {
                    new DirPermission("Everyone", GenericPermission.All)
                }
            }
        )
        {
            GUID = new Guid("d6b86a56-4b70-4028-8179-108b910edadd"),
            Version = new Version(appVersion),
            ControlPanelInfo =
            {
                Manufacturer = company,
                HelpLink = appUrl,
                ProductIcon = @"resources\app.ico"
            },
            LicenceFile = @"resources\License.txt",
            UI = WUI.WixUI_ProgressOnly,
            OutFileName = $"ExtrabbitCode.Inventor.Core.Template_{appVersion}",
            Platform = Platform.x64
        };

        project.BuildMsi();
    }

    public static string GetVersionFromCsproj(string csprojPath)
    {
        XDocument doc = XDocument.Load(csprojPath);
        XElement? versionElement = doc.Descendants("AssemblyVersion").FirstOrDefault();
        return versionElement?.Value ?? "1.0.0";
    }

    static string GetBuildConfiguration()
    {
#if DEBUG
        return "Debug";
#else
        return "Release";
#endif
    }
}