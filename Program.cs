using System;
using System.Threading;
using System.Windows.Forms;

namespace CaffeineV2
{
    public static class Program
    {
        private static Mutex m_Mutex;

        [STAThread]
        public static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            m_Mutex = new Mutex(true, "CaffeineForWorkspaceMutex", out bool createdNew);

            if (createdNew)
            {
                Application.Run(new frmHidden());
            }
            else
            {
                MessageBox.Show("The application is already running.", Application.ProductName,
                    MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }
    }
}
