using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;

namespace CaffeineV2
{
    public partial class frmHidden : Form
    {
        private const string CCMRegPath = @"SOFTWARE\Citrix\ICA Client\CCM";

        public frmHidden()
        {
            InitializeComponent();
        }

        private static bool CheckCCMRegistryKeys()
        {
            List<string> ccmValues = new List<string> {"AllowSimulationAPI", "AllowLiveMonitoring"};

            var view = Environment.Is64BitProcess
                ? Microsoft.Win32.RegistryView.Registry64
                : Microsoft.Win32.RegistryView.Registry32;
            using (var baseKey = Microsoft.Win32.RegistryKey.OpenBaseKey(Microsoft.Win32.RegistryHive.LocalMachine, view))
            using (var ccmKey = baseKey.OpenSubKey(CCMRegPath))
            {
                return ccmKey != null && ccmValues.All(value => ccmKey.GetValue(value) is int enabled && enabled == 1);
            }
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            tmrTick.Interval = Properties.Settings.Default.TimerIntervalSeconds * 1000;

            try
            {
                Debug.WriteLine("Testing ICA client object");
                var icaClient = new WFICALib.ICAClient();
                var version = icaClient.Version;
                var enumHandle = icaClient.EnumerateCCMSessions();
                var sessionCount = icaClient.GetEnumNameCount(enumHandle);
                Debug.WriteLine($"ICA client object creation successful: Version {version} / Sessions {sessionCount}");
                icaClient.CloseEnumHandle(enumHandle);
            }
            catch (Exception ex)
            {
                string architecture = Environment.Is64BitProcess ? "64-bit" : "32-bit";
                MessageBox.Show($"Unable to create the ICA CCM object. This build requires {architecture} Citrix Workspace. Check that the installed client matches.", "Error on Startup", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Debug.WriteLine($"An error occurred while registering the ICA CCM Object: {ex}");
                this.Close();
                return;
            }

            if (!CheckCCMRegistryKeys())
            {
                MessageBox.Show("The CCM registry values AllowSimulationAPI and AllowLiveMonitoring must both be DWORD 1. Run caffeinev2_fix.ps1 as Administrator, then restart the app.", "Missing CCM values detected on Startup", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Close();
                return;
            }
            Debug.WriteLine("Starting Timer");

            tmrTick.Start();
            this.Visible = false;
        }

        private void tmrTick_Tick(object sender, EventArgs e)
        {
            try
            {
                Debug.WriteLine("Timer Ticking");

                var icaClient = new WFICALib.ICAClient();
                var enumHandle = icaClient.EnumerateCCMSessions();
                var sessionCount = icaClient.GetEnumNameCount(enumHandle);
                Debug.WriteLine($"Found {sessionCount} sessions");
                int itemCount = 0;

                while (itemCount < sessionCount)
                {
                    try
                    {
                        Debug.WriteLine("Sending keepalive to session: {0}", itemCount);
                        string session = icaClient.GetEnumNameByIndex(enumHandle, itemCount);
                        var icaSession = new WFICALib.ICAClient();
                        icaSession.SetProp("OutputMode", "1");
                        icaSession.StartMonitoringCCMSession(session, true);
                        icaSession.Session.Keyboard.SendKeyDown(Properties.Settings.Default.KeyValue);//(int)Keys.B);
                        icaSession.StopMonitoringCCMSession(session);
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"Keepalive to session: {itemCount} failed: {ex}");
                    }
                    itemCount += 1;

                }

                icaClient.CloseEnumHandle(enumHandle);
            }
            catch(Exception ex)
            {
                Debug.WriteLine($"Timer tick exception: {ex}");
            }
        }

        private void frmHidden_Shown(object sender, EventArgs e)
        {
            this.Hide();
        }

        private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {
        }

        private void tsmiAbout_Click(object sender, EventArgs e)
        {
            var fa = new frmAbout();
            fa.Show();
        }

        private void tsmiExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
