# Caffeine For Citrix Workspace

This fork targets 64-bit Citrix Workspace.

http://andrewmorgan.ie/2012/07/caffeine-for-citrix-receiver/

Remember this Citrix utility i wrote 8 years ago? no? Me neither, or at least I wouldnt, if you would stop emailing me about it.

In light of COVID-19, constant pestering and peoples desire to avoid work, I've re-written it and i'm putting the whole project up here so you can have it. Or do more with it yourself if you like.

# You'll need to do four things:

1: .NET Framework 4.8.1 for the included x64 build

2: Open PowerShell as Administrator and run `./caffeinev2_fix.ps1` (included in the download).
The script sets `AllowLiveMonitoring` and `AllowSimulationAPI` to DWORD `1` under
`HKLM\SOFTWARE\Citrix\ICA Client\CCM` in both the 32-bit and 64-bit registry views.

3: Download the utility:

https://github.com/efnikolas/CaffeineForCitrixWorkspace/raw/master/Binary/CaffeineForCitrixWorkspace-x64.zip

4: Run it!

Then go back to doing nothing safe in the knowledge that you shouldnt get a screensaver prompt again!

if you want to tune the keystroke it sends, see the exe.config file
if you want to tune the frequency, see the exe.config file.

