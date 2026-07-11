using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

namespace ExtrabbitCode.Inventor.Core.Template.UI;

public static class UiDefinitionHelper
{
    public static ButtonDefinition CreateButton(string displayText, string internalName, string iconPath, string theme)
    {
        UiButton myButton = new()
        {
            Bd = CreateButtonDefinition(displayText, internalName, "", iconPath, theme)
        };
        return myButton.Bd;
    }

    public static ButtonDefinition CreateButtonDefinition(string displayName, string internalName,
        string toolTip, string iconFolder, string theme)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(displayName);
        ArgumentException.ThrowIfNullOrWhiteSpace(internalName);

        ButtonDefinition? controlDefs = Globals.InvApp.CommandManager.ControlDefinitions
            .Cast<object>()
            .OfType<ButtonDefinition>()
            .FirstOrDefault(b =>
                string.Equals(b.InternalName, internalName, StringComparison.OrdinalIgnoreCase));

        if (controlDefs is not null)
        {
            throw new InvalidOperationException(
                $"A command already exists with the internal name '{internalName}'. " +
                "Each add-in must have a unique internal name.");
        }

        (IPictureDisp? iPicDisp16X16, IPictureDisp? iPicDisp32X32) = GetButtonIcons(iconFolder, theme);

        try
        {
            CommandManager cmdMgr = Globals.InvApp.CommandManager;
            ButtonDefinition? buttonDef = cmdMgr.ControlDefinitions.AddButtonDefinition(
                displayName,
                internalName,
                CommandTypesEnum.kShapeEditCmdType,
                Globals.AddInClientId,
                string.Empty,
                toolTip,
                iPicDisp16X16,
                iPicDisp32X32
            );

            return buttonDef;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Failed to create button definition.", ex);
        }
    }

    public static (IPictureDisp? iPicDisp16X16, IPictureDisp? iPicDisp32X32) GetButtonIcons(string iconFolder, string theme)
    {
        IPictureDisp? iPicDisp16X16 = LoadEmbeddedIcon(iconFolder, "16x16", theme);
        IPictureDisp? iPicDisp32X32 = LoadEmbeddedIcon(iconFolder, "32x32", theme);
        return (iPicDisp16X16, iPicDisp32X32);
    }

    /// <summary>
    ///     Loads a button icon that is embedded in the assembly as a raw PNG resource and converts it
    ///     into the <see cref="IPictureDisp" /> that the Inventor API requires.
    /// </summary>
    /// <remarks>
    ///     The PNG bytes are read straight from the manifest resource stream, so there is no
    ///     <c>BinaryFormatter</c>-based resource deserialization involved (unlike storing a typed
    ///     <see cref="System.Drawing.Bitmap" /> in a .resx). That keeps icon loading working on both
    ///     .NET 8 and .NET 10, where <c>BinaryFormatter</c> has been removed. Resources are matched by
    ///     name suffix, so the exact root namespace of the assembly does not matter.
    /// </remarks>
    private static IPictureDisp? LoadEmbeddedIcon(string iconFolder, string size, string theme)
    {
        if (string.IsNullOrWhiteSpace(iconFolder))
        {
            return null;
        }

        // e.g. "UI\ButtonResources\Info" + "16x16" + "DarkTheme" -> "UI.ButtonResources.Info.16x16DarkTheme.png"
        string suffix = iconFolder.Replace('\\', '.').Replace('/', '.').Trim('.') + $".{size}{theme}.png";

        Assembly assembly = Assembly.GetExecutingAssembly();
        string? resourceName = Array.Find(
            assembly.GetManifestResourceNames(),
            name => name.EndsWith(suffix, StringComparison.OrdinalIgnoreCase));

        if (resourceName is null)
        {
            return null;
        }

        try
        {
            using Stream stream = assembly.GetManifestResourceStream(resourceName)!;
            using System.Drawing.Bitmap bitmap = new(stream);
            return ConvertImage.ConvertImageToIPictureDisp(bitmap);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException(
                $@"Unable to load the embedded button icon ""{resourceName}"".", ex);
        }
    }

    public static RibbonTab SetupTab(string displayName, string internalName, Ribbon? invRibbon)
    {
        ArgumentNullException.ThrowIfNull(invRibbon);

        RibbonTab? ribbonTab = invRibbon.RibbonTabs
            .Cast<RibbonTab>()
            .FirstOrDefault(t => string.Equals(t.InternalName, internalName, StringComparison.OrdinalIgnoreCase));

        if (ribbonTab is not null)
        {
            return ribbonTab;
        }

        try
        {
            ribbonTab = invRibbon.RibbonTabs.Add(displayName, internalName, Globals.AddInClientId);
            return ribbonTab;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException(
                $"Failed to create ribbon tab '{internalName}' in ribbon '{invRibbon.InternalName}'.", ex);
        }
    }

    public static RibbonPanel SetupPanel(string displayName, string internalName, RibbonTab? ribbonTab)
    {
        ArgumentNullException.ThrowIfNull(ribbonTab);

        RibbonPanel? ribbonPanel = ribbonTab.RibbonPanels
            .Cast<RibbonPanel>()
            .FirstOrDefault(p =>
                string.Equals(p.InternalName, internalName, StringComparison.OrdinalIgnoreCase));

        if (ribbonPanel is not null)
        {
            return ribbonPanel;
        }

        try
        {
            ribbonPanel = ribbonTab.RibbonPanels.Add(displayName, internalName, Globals.AddInClientId);
            return ribbonPanel;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException(
                $"Failed to create ribbon panel '{internalName}' in tab '{ribbonTab.InternalName}'.",
                ex);
        }
    }
}

///<summary>
/// Class used to convert bitmaps and icons between their .Net native types
/// and an IPictureDisp object which is what the Inventor API requires.
/// </summary>
public class ConvertImage() : AxHost("59EE46BA-677D-4d20-BF10-8D8067CB8B32")
{
    public static IPictureDisp ConvertImageToIPictureDisp(System.Drawing.Image image)
    {
        try
        {
            return (IPictureDisp)GetIPictureFromPicture(image);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException(
                "Failed to convert image to IPictureDisp. The input image might be invalid or unsupported.",
                ex);
        }
    }
}