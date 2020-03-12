using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CaffeineV2
{
    static class Program
    {
        private static Mutex m_Mutex;
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            bool createdNew;
            m_Mutex = new Mutex(true, "CaffeineForWorkspaceMutex", out createdNew);
            if (createdNew)
            {
                Application.Run(new frmHidden());


            }
            else
                MessageBox.Show("The application is already running.", Application.ProductName,
                  MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
        }
    }
}
