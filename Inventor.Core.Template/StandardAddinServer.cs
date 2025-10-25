using ExtrabbitCode.Inventor.Core.Template.UI;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using ExtrabbitCode.Inventor.Core.Template.Helper;
using ExtrabbitCode.Inventor.Core.Template.Models;
using log4net;

// #if (ui == "wpfui")
using Wpf.Ui.Appearance;
// #endif

namespace ExtrabbitCode.Inventor.Core.Template;

[ProgId("Inventor.Core.Template.StandardAddInServer")]
[Guid(Globals.AddInClientId)]
public class StandardAddInServer : ApplicationAddInServer
{
    private UserInterfaceEvents? _uiEvents;
    private List<RibbonPanel> _ribbonPanels = [];
    private List<RibbonTab> _ribbonTabs = [];
    private List<CommandControl> _buttons = [];
    private List<ButtonDefinition> _buttonDefinitions = [];

    public static ApplicationEvents? InvAppEvents{ get; set; }

    private ButtonDefinition? _defaultButton;
    private ButtonDefinition? _info;

    private static readonly ILog Logger = LogManagerAddin.GetLogger(typeof(StandardAddInServer));

    public UserInterfaceEvents? UiEvents
    {
        [MethodImpl(MethodImplOptions.Synchronized)]
        get => _uiEvents;

        [MethodImpl(MethodImplOptions.Synchronized)]
        set
        {
            if (_uiEvents != null)
            {
                _uiEvents.OnResetRibbonInterface -= UiEventsOnResetRibbonInterface;
            }

            _uiEvents = value;
            if (_uiEvents != null)
            {
                _uiEvents.OnResetRibbonInterface += UiEventsOnResetRibbonInterface;
            }
        }
    }

    /// <summary>
    /// This method is called by Inventor when it loads the AddIn. The AddInSiteObject provides access to the Inventor Application object. The FirstTime flag indicates if the AddIn is loaded for the first time. However, with the introduction of the ribbon this argument is always true.
    /// </summary>
    /// <param name="addInSiteObject">The add in site object.</param>
    /// <param name="firstTime">if set to <c>true</c> [first time].</param>
    // ReSharper disable once CA1725
#pragma warning disable CA1725 // Parameter names should match base declaration
    public void Activate(ApplicationAddInSite addInSiteObject, bool firstTime)
#pragma warning restore CA1725 // Parameter names should match base declaration
    {
        ArgumentNullException.ThrowIfNull(addInSiteObject);

        try
        {
            Logger.Debug("Addin InventorTemplate Activated");

            Globals.InvApp = addInSiteObject.Application;
            Globals.InvApplicationAddInSite = addInSiteObject;
            UiEvents = Globals.InvApp.UserInterfaceManager.UserInterfaceEvents;
            InvAppEvents = Globals.InvApp.ApplicationEvents;
            InvAppEvents.OnApplicationOptionChange += InvAppEvents_OnApplicationOptionChange;

            ThemeManager themeManager = Globals.InvApp.ThemeManager;
            Globals.ActiveTheme = themeManager.ActiveTheme;
            string themeName = Globals.ActiveTheme.Name;
            Logger.Debug("Inventor ThemeManager ActiveTheme: " + themeName);

            // #if (ui == "wpfui")
            ApplicationTheme appTheme = themeName == InventorThemeConstants.LightTheme
                ? ApplicationTheme.Light
                : ApplicationTheme.Dark;

            // Force initialize Wpf.Ui before any dialog opens
            ApplicationThemeManager.Apply(appTheme);
            // #endif

            _info = UiDefinitionHelper.CreateButton("Info", "ExtrabbitCode.Inventor.Core.Template.Info", @"UI\ButtonResources\Info", themeName);
            _defaultButton = UiDefinitionHelper.CreateButton("DefaultButton", "ExtrabbitCode.Inventor.Core.Template.DefaultButton", @"UI\ButtonResources\DefaultButton", themeName);
            _buttonDefinitions.Add(_info);
            _buttonDefinitions.Add(_defaultButton);

            if (firstTime)
            {
                AddToUserInterface();
            }
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException(
                @"Unexpected failure during activation of the add-in 'InventorTemplate'.",
                ex
            );
        }
    }

    public void Deactivate()
    {
        ReleaseButtons();
        ReleaseRibbonPanels();
        ReleaseRibbonTabs();
        ReleaseAppEvents();

        if (_uiEvents != null)
        {
            _uiEvents.OnResetRibbonInterface -= UiEventsOnResetRibbonInterface;
            _uiEvents = null;
        }
    }

    private void ReleaseAppEvents()
    {
        if (InvAppEvents is null)
        {
            return;
        }

        try
        {
            InvAppEvents.OnApplicationOptionChange -= InvAppEvents_OnApplicationOptionChange;
        }
        catch (COMException ex)
        {
            Logger.Debug("COMException while releasing application events.", ex);
        }
        catch (InvalidComObjectException ex)
        {
            Logger.Debug("InvalidComObjectException while releasing application events.", ex);
        }
        finally
        {
            InvAppEvents = null;
        }
    }

    private void ReleaseRibbonTabs()
    {
        if (_ribbonTabs.Count == 0)
        {
            return;
        }

        foreach (RibbonTab tab in _ribbonTabs)
        {
            try
            {
                tab.Delete();
                Marshal.ReleaseComObject(tab);
            }
            catch (COMException ex)
            {
                Logger.Debug("COMException releasing RibbonTab.", ex);
            }
            catch (InvalidComObjectException ex)
            {
                Logger.Debug("InvalidComObjectException releasing RibbonTab.", ex);
            }
        }

        _ribbonTabs.Clear();
    }

    private void ReleaseRibbonPanels()
    {
        if (_ribbonPanels.Count == 0)
        {
            return;
        }

        foreach (RibbonPanel panel in _ribbonPanels)
        {
            try
            {
                panel.Delete();
                Marshal.ReleaseComObject(panel);
            }
            catch (COMException ex)
            {
                Logger.Debug("COMException releasing RibbonPanel.", ex);
            }
            catch (InvalidComObjectException ex)
            {
                Logger.Debug("InvalidComObjectException releasing RibbonPanel.", ex);
            }
        }

        _ribbonPanels.Clear();
    }

    private void ReleaseButtons()
    {
        if (_buttonDefinitions.Count == 0 && _buttons.Count == 0)
        {
            return;
        }

        try
        {
            foreach (ButtonDefinition buttonDefinition in _buttonDefinitions)
            {
                try
                {
                    buttonDefinition.Delete();
                    Marshal.ReleaseComObject(buttonDefinition);
                }
                catch (COMException ex)
                {
                    Logger.Debug("COMException releasing ButtonDefinition.", ex);
                }
                catch (InvalidComObjectException ex)
                {
                    Logger.Debug("InvalidComObjectException releasing ButtonDefinition.", ex);
                }
            }

            foreach (CommandControl commandControl in _buttons)
            {
                try
                {
                    commandControl.Delete();
                    Marshal.ReleaseComObject(commandControl);
                }
                catch (COMException ex)
                {
                    Logger.Debug("COMException releasing CommandControl.", ex);
                }
                catch (InvalidComObjectException ex)
                {
                    Logger.Debug("InvalidComObjectException releasing CommandControl.", ex);
                }
            }
        }
        finally
        {
            _buttonDefinitions.Clear();
            _buttons.Clear();
        }
    }

    public object? Automation => null;
#pragma warning disable CA1725 // Parameter names should match base declaration
    public void ExecuteCommand(int commandId) { }
#pragma warning restore CA1725 // Parameter names should match base declaration

    private void AddToUserInterface()
    {
        Ribbon idwRibbon = Globals.InvApp.UserInterfaceManager.Ribbons["Drawing"];
        Ribbon iptRibbon = Globals.InvApp.UserInterfaceManager.Ribbons["Part"];
        Ribbon iamRibbon = Globals.InvApp.UserInterfaceManager.Ribbons["Assembly"];
        Ribbon ipnRibbon = Globals.InvApp.UserInterfaceManager.Ribbons["Presentation"];

        RibbonTab tabIdw = UiDefinitionHelper.SetupTab("ExtrabbitCode.Inventor.Core.Template", "ExtrabbitCode.Inventor.Core.Template", idwRibbon);
        RibbonTab tabIpt = UiDefinitionHelper.SetupTab("ExtrabbitCode.Inventor.Core.Template", "ExtrabbitCode.Inventor.Core.Template", iptRibbon);
        RibbonTab tabIam = UiDefinitionHelper.SetupTab("ExtrabbitCode.Inventor.Core.Template", "ExtrabbitCode.Inventor.Core.Template", iamRibbon);
        RibbonTab tabIpn = UiDefinitionHelper.SetupTab("ExtrabbitCode.Inventor.Core.Template", "ExtrabbitCode.Inventor.Core.Template", ipnRibbon);
        _ribbonTabs.Add(tabIdw);
        _ribbonTabs.Add(tabIpt);
        _ribbonTabs.Add(tabIam);
        _ribbonTabs.Add(tabIpn);

        RibbonPanel infoIdw = UiDefinitionHelper.SetupPanel("Info", "Info", tabIdw);
        RibbonPanel infoIpt = UiDefinitionHelper.SetupPanel("Info", "Info", tabIpt);
        RibbonPanel infoIam = UiDefinitionHelper.SetupPanel("Info", "Info", tabIam);
        RibbonPanel infoIpn = UiDefinitionHelper.SetupPanel("Info", "Info", tabIpn);
        _ribbonPanels.Add(infoIdw);
        _ribbonPanels.Add(infoIpt);
        _ribbonPanels.Add(infoIam);
        _ribbonPanels.Add(infoIpn);

        RibbonPanel addinPanelIdw = UiDefinitionHelper.SetupPanel("AddinCommands", "AddinCommands", tabIdw);
        RibbonPanel addinPanelIpt = UiDefinitionHelper.SetupPanel("AddinCommands", "AddinCommands", tabIpt);
        RibbonPanel addinPanelIam = UiDefinitionHelper.SetupPanel("AddinCommands", "AddinCommands", tabIam);
        RibbonPanel addinPanelIpn = UiDefinitionHelper.SetupPanel("AddinCommands", "AddinCommands", tabIpn);
        _ribbonPanels.Add(addinPanelIdw);
        _ribbonPanels.Add(addinPanelIpt);
        _ribbonPanels.Add(addinPanelIam);
        _ribbonPanels.Add(addinPanelIpn);

        if (_defaultButton != null)
        {
            CommandControl defaultButtonIdw = addinPanelIdw.CommandControls.AddButton(_defaultButton, true);
            CommandControl defaultButtonIpt = addinPanelIpt.CommandControls.AddButton(_defaultButton, true);
            CommandControl defaultButtonIam = addinPanelIam.CommandControls.AddButton(_defaultButton, true);
            CommandControl defaultButtonIpn = addinPanelIpn.CommandControls.AddButton(_defaultButton, true);
            _buttons.Add(defaultButtonIdw);
            _buttons.Add(defaultButtonIpt);
            _buttons.Add(defaultButtonIam);
            _buttons.Add(defaultButtonIpn);
        }
        if (_info != null)
        {
            CommandControl infoButtonIdw = infoIdw.CommandControls.AddButton(_info, true);
            CommandControl infoButtonIpt = infoIpt.CommandControls.AddButton(_info, true);
            CommandControl infoButtonIam = infoIam.CommandControls.AddButton(_info, true);
            CommandControl infoButtonIpn = infoIpn.CommandControls.AddButton(_info, true);
            _buttons.Add(infoButtonIdw);
            _buttons.Add(infoButtonIpt);
            _buttons.Add(infoButtonIam);
            _buttons.Add(infoButtonIpn);
        }
    }

    private void UiEventsOnResetRibbonInterface(NameValueMap context)
    {
        AddToUserInterface();
    }

    private void InvAppEvents_OnApplicationOptionChange(EventTimingEnum beforeOrAfter, NameValueMap context, out HandlingCodeEnum handlingCode)
    {
        if (beforeOrAfter == EventTimingEnum.kAfter)
        {
            ThemeManager themeManager = Globals.InvApp.ThemeManager;
            Theme activeTheme = themeManager.ActiveTheme;
            string theme = activeTheme.Name;

            if (Globals.ActiveTheme.Name != theme) //check if theme has changed
            {
                Deactivate();
                Activate(Globals.InvApplicationAddInSite, true);
            }
        }

        handlingCode = HandlingCodeEnum.kEventNotHandled;
    }
}