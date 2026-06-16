# How to use this template

This is a Template for writting dotnet core based Inventor Addins. Version 2025 and higher. The complete documentation can be found here: [ExtrabbitCode Inventor Core Template Documentation](https://github.com/TWiesendanger/ExtrabbitCode.Inventor.Core.Template)

A short overview is contained in this readme.

## Install

Use this command in your terminal:

`dotnet new install ExtrabbitCode.Inventor.Core.Template@1.4.0`

## UI

Dialogs can be built with **Modern UI** (the default, via [ExtrabbitCode.Inventor.ModernUi](https://www.nuget.org/packages/ExtrabbitCode.Inventor.ModernUi/) — [docs](https://modernui.extrabbitcode.com/)), **wpfui** or **winforms**. Choose the option with the `--ui` parameter (`modernui` / `wpfui` / `winforms`) or in the Visual Studio creation dialog.

## Change Inventor Version

To change the inventor version, you need to edit the project file. Look for `<InventorVersion>$(InventorVersion)</InventorVersion>` and change it to the desired version. Even better you look for the `Directory.Build.props` file in the root folder and change it there. This way all projects will use the same version.

### Use iLogic

To use iLogic dll's, you need to edit the project file. You can add `<UseILogic>true</UseILogic>`.

## Analyzer

The analyzer can be pretty strict if `TreatWarningAsErrors` is used. It forces you to use best practises and avoid common mistakes. If it is to strict for you, feel free to edit the ruleset file called `.editorconfig` in the root folder. By default using `var` is not allowed. If you want to change this, just edit the file and change the corresponding rule.

If you want to change this, you can do this by changing this line from the project to `true`:

```xml
<TreatWarningsAsErrors>true</TreatWarningsAsErrors>
```

## Isolation

Inventor Addins are loaded into the same process as Inventor. This can lead to some dependency conflicts. To avoid this, the template uses a technique called isolation. This means that the addin is loaded into a separate AppDomain. This way, the addin can have its own dependencies without affecting Inventor or other addins.

Before you might have used another logger version than autodesk is using. This could even happen after a dot update.
This is now using a separate AppDomain, so you can use any version of the logger you want without worrying about conflicts.