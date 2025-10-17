namespace ExtrabbitCode.Inventor.Core.Template;

public static class Globals
{
    /// <summary>Holds a reference to the app (inventor)</summary>
    public static Application InvApp;
    /// <summary>Holds a reference to the addin.</summary>
    public static ApplicationAddInSite InvApplicationAddInSite;
    /// <summary>This is the guid used through the whole addin. This was generated during the creation of the addin. Make sure it is unique. There cannot be another addin having the same guid.</summary>
    public const string AddInClientId = "d6b86a56-4b70-4028-8179-108b910edadd";
    /// <summary>Holds a reference to the active app them. Can be dark theme or light theme.</summary>
    public static Theme ActiveTheme;
}