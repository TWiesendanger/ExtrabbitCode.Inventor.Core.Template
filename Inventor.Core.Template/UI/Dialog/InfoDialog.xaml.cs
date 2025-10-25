using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using ExtrabbitCode.Inventor.Core.Template.UI.ViewModels;
using Wpf.Ui.Appearance;

namespace ExtrabbitCode.Inventor.Core.Template.UI.Dialog;

public partial class InfoDialog
{
    public InfoDialog()
    {
        InitializeComponent();
        DataContext = new InfoDialogViewModel();

        WindowStartupLocation = WindowStartupLocation.CenterOwner;

        IntPtr ownerHandle = new(Globals.InvApp.MainFrameHWND);
        new WindowInteropHelper(this).Owner = ownerHandle;


        ApplicationThemeManager.Apply(this);

        WindowStartupLocation = WindowStartupLocation.CenterOwner;
    }

    protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
    {
        base.OnMouseLeftButtonDown(e);

        DragMove();
    }
}