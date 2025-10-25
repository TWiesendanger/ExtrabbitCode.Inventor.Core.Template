using ExtrabbitCode.Inventor.Core.Template.Helper;
//#if (ui == "wpfui")
using ExtrabbitCode.Inventor.Core.Template.Models;
using ExtrabbitCode.Inventor.Core.Template.UI.Dialog;
using System;
using System.Runtime.CompilerServices;
using System.Windows;
//#endif
using log4net;
using Wpf.Ui.Appearance;


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
                //#if (ui == "wpfui")
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

    private static void SetDialogTheme(Window dialog)
    {
        ApplicationTheme theme = Globals.ActiveTheme.Name == InventorThemeConstants.LightTheme
            ? ApplicationTheme.Light
            : ApplicationTheme.Dark;

        ApplicationThemeManager.Apply(dialog);
        ApplicationThemeManager.Apply(theme);
    }
}