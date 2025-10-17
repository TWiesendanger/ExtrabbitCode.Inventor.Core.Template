using System;
using System.Reflection;
using System.Windows.Forms;

namespace ExtrabbitCode.Inventor.Core.Template.UI.Dialog;

public partial class FrmInfo : Form
{
    public FrmInfo()
    {
        InitializeComponent();
    }

    protected override void OnShown(EventArgs e)
    {
        base.OnShown(e);

        txtVersion.Text = Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? "0.0.0";
        string changeLog = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"\Resources\versionhistory.txt";
        txtChangeLog.Lines = System.IO.File.ReadAllLines(changeLog);
    }

    private void frmInfo_Load(object sender, EventArgs e)
    {

    }
}