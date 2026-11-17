#Requires -Version 5.1
#Requires -RunAsAdministrator

$ErrorActionPreference = 'Stop'
$path = 'SOFTWARE\Citrix\ICA Client\CCM'
$views = @([Microsoft.Win32.RegistryView]::Registry32)
if ([Environment]::Is64BitOperatingSystem) {
    $views += [Microsoft.Win32.RegistryView]::Registry64
}

foreach ($view in $views) {
    $baseKey = [Microsoft.Win32.RegistryKey]::OpenBaseKey(
        [Microsoft.Win32.RegistryHive]::LocalMachine, $view)
    $key = $null
    try {
        $key = $baseKey.CreateSubKey($path)
        $key.SetValue('AllowLiveMonitoring', 1, [Microsoft.Win32.RegistryValueKind]::DWord)
        $key.SetValue('AllowSimulationAPI', 1, [Microsoft.Win32.RegistryValueKind]::DWord)
        [pscustomobject]@{
            RegistryView = $view
            AllowLiveMonitoring = $key.GetValue('AllowLiveMonitoring')
            AllowSimulationAPI = $key.GetValue('AllowSimulationAPI')
        }
    }
    finally {
        if ($null -ne $key) { $key.Dispose() }
        $baseKey.Dispose()
    }
}
