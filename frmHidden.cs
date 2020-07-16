using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CaffeineV2
{
    public partial class frmHidden : Form
    {
        private const string CCMRegPath = @"SOFTWARE\WOW6432Node\Citrix\ICA Client\CCM";

        public frmHidden()
        {
            InitializeComponent();
        }

        private bool CheckCCMRegistryKeys()
        {
            List<string> ccmValues = new List<string>();
            ccmValues.Add("AllowSimulationAPI");
            ccmValues.Add("AllowLiveMonitoring");
            using (var baseKey = Microsoft.Win32.RegistryKey.OpenBaseKey(Microsoft.Win32.RegistryHive.LocalMachine, Microsoft.Win32.RegistryView.Registry32))
            {
                var ccmKey = baseKey.OpenSubKey(CCMRegPath);
                if (ccmKey != null)
                {
                    foreach (string Value in ccmValues)
                    {
                        if ((int)ccmKey.GetValue(Value, -1) != 1)
                        {
                            return false;
                        }
                    }
                    return true;

                }
            }
            return false;
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
                icaClient.CloseEnumHandle(enumHandle);
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occured while registering the ICA CCM Object", "Error on Startup", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Close();
            }

            if (!CheckCCMRegistryKeys())
            {
                MessageBox.Show(@"The CCM registry DWORDS (AllowSimulationAPI & AllowLiveMonitoring) are missing from HKLM:\" + CCMRegPath, "Missing CCM values detected on Startup", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Close();
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
                Debug.WriteLine("Found {0} sessions", sessionCount);
                int itemCount = 0;

                while (itemCount < sessionCount)
                {
                    try
                    {
                        Debug.WriteLine("Sending keepalive to session: {0}", itemCount);
                        string session = icaClient.GetEnumNameByIndex(enumHandle, itemCount);
                        WFICALib.ICAClient icaSession = new WFICALib.ICAClient();
                        icaSession.SetProp("OutputMode", "1");
                        icaSession.StartMonitoringCCMSession(session, true);
                        icaSession.Session.Keyboard.SendKeyDown(Properties.Settings.Default.KeyValue);//(int)Keys.B);
                        icaSession.StopMonitoringCCMSession(session);
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine("Keepalive to session: {0} failed: {1}", itemCount, ex.ToString());
                    }
                    itemCount += 1;

                }
                icaClient.CloseEnumHandle(enumHandle);
            }
            catch(Exception ex)
            {
                Debug.WriteLine("Timer tick exception: {0}", ex.ToString());
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
            frmAbout fa = new frmAbout();
            fa.Show();
        }

        private void tsmiExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}


