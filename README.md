# Caffeine for Citrix Workspace (x64)

Sends a configurable keystroke to Citrix sessions at a fixed interval. This fork builds the original [Andrew Morgan utility](https://github.com/andyjmorgan/CaffeineForCitrixWorkspace) for 64-bit Citrix Workspace.

## Requirements

- 64-bit Windows and 64-bit Citrix Workspace installed locally.
- .NET Framework 4.8.1 for the packaged build.
- Administrator access to run the registry setup script.

The x64 executable requires the 64-bit Citrix ICA COM component. The registry script enables the CCM APIs; it does not install or register that component.

## Setup

1. [Download the x64 package](https://github.com/efnikolas/CaffeineForCitrixWorkspace/raw/master/Binary/CaffeineForCitrixWorkspace-x64.zip) and extract it.
2. Open PowerShell **as Administrator**, change to the extracted folder, and run:

   ```powershell
   .\caffeinev2_fix.ps1
   ```

3. Check that the script reports `1` for both `AllowLiveMonitoring` and `AllowSimulationAPI`.
4. Run `CaffeineV2-x64.exe`. Keep `CaffeineV2-x64.exe.config` beside it.

The app runs in the notification area. Right-click its icon and choose **Exit** to stop it.

## Registry setup

`caffeinev2_fix.ps1` requires an elevated PowerShell session and sets these DWORD values to `1`:

- `AllowLiveMonitoring`
- `AllowSimulationAPI`

It writes both registry views on 64-bit Windows:

| View | Key |
| --- | --- |
| 64-bit | `HKLM\SOFTWARE\Citrix\ICA Client\CCM` |
| 32-bit | `HKLM\SOFTWARE\WOW6432Node\Citrix\ICA Client\CCM` |

The script refuses to run without Administrator rights. It supports Windows PowerShell 5.1 and PowerShell 7.

## Configuration

Exit the app before editing `CaffeineV2-x64.exe.config`, then restart it.

| Setting | Default | Meaning |
| --- | --- | --- |
| `KeyValue` | `126` | Windows Forms key code; `126` is F15. |
| `TimerIntervalSeconds` | `120` | Seconds between keystrokes. |

## Build

Install Visual Studio or its Build Tools with MSBuild and the .NET Framework 4.8.1 targeting pack. The project references the interop library from the local 64-bit Workspace installation at `C:\Program Files\Citrix\ICA Client\WFICALib.dll`.

From a Developer PowerShell window in the repository folder:

```powershell
MSBuild.exe .\CaffeineV2.csproj /t:Rebuild /p:Configuration=Release /p:TargetFrameworkVersion=v4.8.1 /p:AssemblyName=CaffeineV2-x64 /p:OutputPath=bin\x64\Release\ /p:PostBuildEvent=
```

The executable and its config are written to `bin\x64\Release`. The empty `PostBuildEvent` disables the upstream packaging command, which requires 7-Zip.

## Startup error

If the app reports an error registering the ICA CCM object, check that the installed Citrix Workspace client is 64-bit. The x64 build cannot load a 32-bit ICA COM component, and changing the CCM registry values does not resolve an architecture mismatch.
