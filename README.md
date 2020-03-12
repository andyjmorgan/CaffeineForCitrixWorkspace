# Caffeine For Citrix Workspace

http://andrewmorgan.ie/2012/07/caffeine-for-citrix-receiver/

Remember this Citrix utility i wrote 8 years ago? no? Me neither, or at least I wouldnt, if you would stop emailing me about it.

In light of COVID-19 and peoples desire not to actually work, to appear active in skype for business / teams, etc. I've re-written it and i'm putting the whole project up here so you can have it and do more with it yourself if you like.

# You'll need to do three things:

1: Create the following two, DWORD values in the following key:

HKEY_LOCAL_MACHINE\SOFTWARE\WOW6432Node\Citrix\ICA Client\CCM

AllowLiveMonitoring: Dword: 1
AllowSimulationAPI: Dword: 1

2: Download the utility:

https://github.com/andyjmorgan/CaffeineForCitrixWorkspace/tree/master/Binary

3: Run it, then go back to doing nothing safe in the knowledge that you shouldnt get a screensaver prompt again!

if you want to tune the keystroke it sends, see the exe.config file
if you want to tune the frequency, see the exe.config file.

