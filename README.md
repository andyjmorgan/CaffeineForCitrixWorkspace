# Caffeine for Citrix Workspace

Sends a configurable keystroke to Citrix sessions at a fixed interval. This fork provides x86 and x64 builds of [Andrew Morgan's utility](https://github.com/andyjmorgan/CaffeineForCitrixWorkspace).

## Choose a build

Match the **installed Citrix Workspace client**, even on 64-bit Windows.

| Citrix Workspace | Download | Executable |
| --- | --- | --- |
| 32-bit | [x86 package](https://github.com/efnikolas/CaffeineForCitrixWorkspace/raw/master/Binary/CaffeineForCitrixWorkspace-x86.zip) | `CaffeineV2-x86.exe` |
| 64-bit | [x64 package](https://github.com/efnikolas/CaffeineForCitrixWorkspace/raw/master/Binary/CaffeineForCitrixWorkspace-x64.zip) | `CaffeineV2-x64.exe` |

Each build loads the Citrix ICA COM component of the same architecture.

## Requirements

- Windows with Citrix Workspace installed locally.
- .NET Framework 4.8.1.
- Administrator access for the registry setup script.

## Setup

1. Download the matching package above and extract it.
2. Open PowerShell **as Administrator**, change to the extracted folder, and run:

   ```powershell
   .\caffeinev2_fix.ps1
   ```

3. Check that the script reports `1` for both `AllowLiveMonitoring` and `AllowSimulationAPI`.
4. Run the extracted executable. Keep its `.exe.config` file beside it.

The app runs in the notification area. Right-click its icon and choose **Exit** to stop it.

## Registry setup

Both packages include the same `caffeinev2_fix.ps1`. It requires an elevated PowerShell session and sets `AllowLiveMonitoring` and `AllowSimulationAPI` to DWORD `1` in both registry views on 64-bit Windows:

| View | Key |
| --- | --- |
| 64-bit | `HKLM\SOFTWARE\Citrix\ICA Client\CCM` |
| 32-bit | `HKLM\SOFTWARE\WOW6432Node\Citrix\ICA Client\CCM` |

On 32-bit Windows it writes only the 32-bit view, under `HKLM\SOFTWARE\Citrix\ICA Client\CCM`. Each executable checks the view matching its process architecture.

The script supports Windows PowerShell 5.1 and PowerShell 7. It enables the CCM APIs; it does not install or register the Citrix ICA COM component.

## Configuration

Exit the app before editing the matching `CaffeineV2-x86.exe.config` or `CaffeineV2-x64.exe.config`, then restart it.

| Setting | Default | Meaning |
| --- | --- | --- |
| `KeyValue` | `126` | Windows Forms key code; `126` is F15. |
| `TimerIntervalSeconds` | `120` | Seconds between keystrokes. |

## Build

On a 64-bit Windows machine, install Visual Studio or Build Tools with MSBuild and the .NET Framework 4.8.1 targeting pack. Citrix Workspace supplies `WFICALib.dll`; the project looks for it in the installed client's `ICA Client` folder under Program Files or Program Files (x86).

Run this from the repository folder to build and package both architectures:

```powershell
.\build.ps1
```

The script finds MSBuild through PATH or the Visual Studio installer. You can supply its location with `-MSBuildPath`.

To build one architecture from Developer PowerShell:

```powershell
MSBuild.exe .\CaffeineV2.sln /t:Rebuild /p:Configuration=Release /p:Platform=x86
MSBuild.exe .\CaffeineV2.sln /t:Rebuild /p:Configuration=Release /p:Platform=x64
```

Executables and configs are written to `bin\x86\Release` and `bin\x64\Release`. `build.ps1` writes the ZIP packages to `Binary`. Both builds embed the COM type definitions from `WFICALib.dll`. To use a copy outside the installation folder, pass `/p:CitrixInteropPath="C:\path\to\WFICALib.dll"` to MSBuild.

## Startup errors

If the app cannot create the ICA CCM object, check that the executable matches the installed Citrix client's architecture. The registry script cannot fix an architecture mismatch or a missing COM component.

If the app reports missing CCM values, run `caffeinev2_fix.ps1` as Administrator and restart the app.
