using System;
using System.Runtime.CompilerServices;
using ExtrabbitCode.Inventor.Core.Template.Helper;
using ExtrabbitCode.Inventor.Core.Template.UI.Dialog;
using log4net;
//#if (ui == "wpfui" || ui == "modernui")
using ExtrabbitCode.Inventor.Core.Template.Models;
using System.Windows;
//#endif
//#if (ui == "modernui")
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using ExtrabbitCode.Inventor.ModernUi;
using ModernTheme = ExtrabbitCode.Inventor.ModernUi.Theme;
//#endif
//#if (ui == "wpfui")
using Wpf.Ui.Appearance;
//#endif


namespace ExtrabbitCode.Inventor.Core.Template.UI;

public class UiButton
{
    private ButtonDefinition? _bd;
    private static readonly ILog Logger = LogManagerAddin.GetLogger(typeof(UiButton));

    public ButtonDefinition? Bd
    {
        [MethodImpl(MethodImplOptions.Synchronized)]
        get => _bd;

        [MethodImpl(MethodImplOptions.Synchronized)]
        set
        {
            if (_bd != null)
            {
                _bd.OnExecute -= ButtonOnExecute;
            }

            _bd = value;
            if (_bd != null)
            {
                _bd.OnExecute += ButtonOnExecute;
            }
        }
    }

    private void ButtonOnExecute(NameValueMap context)
    {
        if (Bd is null)
        {
            Logger.Error("ButtonOnExecute invoked, but Bd is null.");
            return;
        }

        switch (Bd.InternalName)
        {
            case "ExtrabbitCode.Inventor.Core.Template.DefaultButton":
                Logger.Debug("Default Button was pressed.");
                System.Windows.Forms.MessageBox.Show(@"Default message.", @"Default title");
                return;
            case "ExtrabbitCode.Inventor.Core.Template.Info":
                Logger.Info("Templatebutton pressed");
                //#if (ui == "modernui")
                ShowInfoDialog();
                //#elif (ui == "wpfui")
                InfoDialog infoDialog = new();
                SetDialogTheme(infoDialog);
                infoDialog.ShowDialog();
                //#elif (ui == "winforms")
                using (FrmInfo infoDlg = new())
                {
                    infoDlg.ShowDialog(new WindowWrapper((IntPtr) Globals.InvApp.MainFrameHWND));
                }
                //#endif
                return;
            default:
                return;
        }
    }

    //#if (ui == "modernui")
    /// <summary>
    ///     Shows the info dialog using the ExtrabbitCode Modern UI library. The theme and font are read
    ///     from Inventor and applied window-scoped, so the dialog matches Inventor without any
    ///     process-global UI state.
    /// </summary>
    private static void ShowInfoDialog()
    {
        ModernTheme theme = Globals.ActiveTheme.Name == InventorThemeConstants.LightTheme
            ? ModernTheme.Light
            : ModernTheme.Dark;

        FontOptions font = FontOptions.FromInventor(
            Globals.InvApp.GeneralOptions.TextAppearance,
            Globals.InvApp.GeneralOptions.TextSize);

        ModernWindow window = new(theme, font: font)
        {
            Title = "Info ExtrabbitCode.Inventor.Core.Template",
            Icon = new BitmapImage(new Uri(
                "pack://application:,,,/ExtrabbitCode.Inventor.Core.Template;component/Resources/appIcon.png")),
            Content = new InfoView(),
            Width = 800,
            Height = 450,
        };

        // Own the dialog to Inventor's main window so it stays on top of Inventor.
        _ = new WindowInteropHelper(window) { Owner = new IntPtr(Globals.InvApp.MainFrameHWND) };
        window.ShowDialog();
    }
    //#endif

    //#if (ui == "wpfui")
    private static void SetDialogTheme(Window dialog)
    {
        ApplicationTheme theme = Globals.ActiveTheme.Name == InventorThemeConstants.LightTheme
            ? ApplicationTheme.Light
            : ApplicationTheme.Dark;

        ApplicationThemeManager.Apply(dialog);
        ApplicationThemeManager.Apply(theme);
    }
    //#endif
}
