using System;
using System.Windows.Forms;

namespace ExtrabbitCode.Inventor.Core.Template.Helper;

///<summary> This class is used to wrap a Win32 hWnd as a .Net IWind32Window class.
/// This is primarily used for parenting a dialog to the Inventor window.
/// This provides the expected behavior when the Inventor window is collapsed
/// myForm.Show(New WindowWrapper(invApp.MainFrameHWND))
/// </summary>
public class WindowWrapper : IWin32Window
{
    private readonly IntPtr _hwnd;

    /// <summary>
    /// Creates a new instance of the window wrapper class
    /// </summary>
    /// <param name="windowHandle">Represents the window handle necessary to obtain the IWin32Window</param>
    public WindowWrapper(IntPtr windowHandle)
    {
        _hwnd = windowHandle;
    }

    /// <summary>
    /// Creates a new instance of the window wrapper class
    /// </summary>
    /// <param name="windowHandle">Represents the window handle number necessary to obtain the IWin32Window</param>
    public WindowWrapper(int windowHandle)
    {
        _hwnd = new IntPtr(windowHandle);
    }

    public IntPtr Handle => _hwnd;
}