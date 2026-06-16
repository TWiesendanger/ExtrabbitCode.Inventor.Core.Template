using System.Windows.Controls;
using ExtrabbitCode.Inventor.Core.Template.UI.ViewModels;

namespace ExtrabbitCode.Inventor.Core.Template.UI.Dialog;

/// <summary>
///     Content shown inside the Modern UI info dialog. Hosted in a <c>ModernWindow</c>, so its
///     standard WPF controls are themed automatically (see <c>UiButton</c>).
/// </summary>
public partial class InfoView : UserControl
{
    public InfoView()
    {
        InitializeComponent();
        DataContext = new InfoDialogViewModel();
    }
}
