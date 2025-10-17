using System;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
using ExtrabbitCode.Inventor.Core.Template.Helper;
using ExtrabbitCode.Inventor.Core.Template.UI.Dialog;

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
                MessageBox.Show(@"Default message.", @"Default title");
                return;
            case "InventorTemplateInfo":
                var infoDlg = new FrmInfo();
                infoDlg.ShowDialog(new WindowWrapper((IntPtr)Globals.InvApp.MainFrameHWND));
                return;
            default:
                return;
        }
    }
}