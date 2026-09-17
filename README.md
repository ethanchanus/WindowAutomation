# WindowAutomation Plugin for Loupedeck

WindowAutomation is a Loupedeck plugin for automating interactions with native Windows applications. It locates any window or control by title, class name, or UI Automation ID (optionally scoped by parent, previous, or next sibling), then applies an action to it:

- Set focus on a window or control
- Select a ComboBox/ListBox item by index or text
- Click a button, or check/uncheck a checkbox
- Click a toolbar button by name or tooltip
- Select a tab by name

This lets you chain multiple commands into complex, repeatable mouse-and-keyboard sequences for controls like Tabs, TextBoxes, ListBoxes, ComboBoxes, and Toolbar menus in any Windows application. It also includes a dial adjustment to resize the active window directly from the Loupedeck device.

## Screenshot
<img width="824" height="1050" alt="image" src="https://github.com/user-attachments/assets/f3f4f7af-550d-4eb9-9ffc-74823a1e0009" />
<img width="759" height="734" alt="image" src="https://github.com/user-attachments/assets/247e9f70-c229-47cd-9d21-2aba0e74d62a" />



## Building the Plugin

1. Go to the `sln` folder.
2. Run the following command to generate the `lplug4` plugin package:

   ```
   msbuild ..\WindowAutomationPlugin\src\WindowAutomationPlugin\WindowAutomationPlugin.csproj /t:Deploy /p:Configuration=Release /p:SolutionDir="$(Resolve-Path .)\" /m
   ```

3. The plugin is generated and saved to `..\out\WindowAutomation.lplug4`.

## Installing the Plugin

To install the plugin (`lplug4` file), either import it from the Loupedeck configuration tool (see the [Loupedeck Add-on Manager instructions](https://support.loupedeck.com/add-on-manager.html)), or use the following command:

```
LoupedeckPluginTool.exe install -path=..\out\WindowAutomation.lplug4
```

Installation extracts the `lplug4` package to `%LOCALAPPDATA%\Loupedeck\Plugins\WindowAutomation`.

## Note

To find a Windows control's exact attributes (name, class name, UI Automation ID, parent/sibling class name, etc), inspect it with one of these tools:

- **Spy++** — included with Visual Studio, under `Common7\Tools`.
<img width="752" height="414" alt="image" src="https://github.com/user-attachments/assets/cfa40dab-c49c-4b64-a828-2b4271b4a2d0" />
 
- **Inspect.exe** — a UI Automation diagnostic tool included with the Windows SDK.
<img width="642" height="390" alt="image" src="https://github.com/user-attachments/assets/6c506b13-2cc5-49fe-b78d-b5b62a4b607f" />

## License

Apache-2.0

## Third-Party Notices

- [Win32Interop.WinHandles](https://github.com/zastrowm/Win32Interop.WinHandles) — Apache-2.0
