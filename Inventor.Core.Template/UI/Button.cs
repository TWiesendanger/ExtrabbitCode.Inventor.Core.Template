using ExtrabbitCode.Inventor.Core.Template.Helper;
using ExtrabbitCode.Inventor.Core.Template.UI.Dialog;
using System;
using System.Runtime.CompilerServices;
//#if (ui == "wpfui")
using ExtrabbitCode.Inventor.Core.Template.Models;
using System.Windows;
using Wpf.Ui.Appearance;
//#endif
using System.Windows.Forms;


namespace ExtrabbitCode.Inventor.Core.Template.UI;

public class UiButton
{
    private ButtonDefinition _bd;

    public ButtonDefinition Bd
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
        switch (Bd.InternalName)
        {
            case "InventorTemplateDefaultButton":
                System.Windows.Forms.MessageBox.Show(@"Default message.", @"Default title");
                return;
            case "InventorTemplateInfo":
                //#if (ui == "wpfui")
                InfoDialog infoDialog = new InfoDialog();
                SetDialogTheme(infoDialog);
                infoDialog.ShowDialog();
                return;
                //#elif (ui == "winforms")
                FrmInfo infoDlg = new FrmInfo();
                infoDlg.ShowDialog(new WindowWrapper((IntPtr)Globals.InvApp.MainFrameHWND));
                //#endif

                return;
            default:
                return;
        }
    }

    private static void SetDialogTheme(Window dialog)
    {
        var theme = Globals.ActiveTheme != null && Globals.ActiveTheme.Name == InventorThemeConstants.LightTheme
            ? ApplicationTheme.Light
            : ApplicationTheme.Dark;

        ApplicationThemeManager.Apply(dialog);
        ApplicationThemeManager.Apply(theme);
    }
}