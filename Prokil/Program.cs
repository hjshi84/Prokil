using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Prokil
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            
            Application.Run(new Form1());
            
            System.Threading.Mutex mutex = new System.Threading.Mutex(false, "ThisShouldOnlyRunOnce");
            bool Running = !mutex.WaitOne(0, false);
            if (!Running)
                Application.Run(new Form1());
            else
                MessageBox.Show("程序已启动！");
        
         }
    }
}
