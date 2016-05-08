using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Text.RegularExpressions;

namespace Prokil
{
    public partial class Form3 : Form
    {
        public Form3()
        {
            InitializeComponent();
            try
            {

                FileStream aFile = new FileStream(GetAppPath() + "Log.pkl", FileMode.Open);
                StreamReader sr = new StreamReader(aFile);
                string a = sr.ReadToEnd();
                textBox1.Text = a;
            }
            catch
            {

            }
        }
        private static string GetAppPath()
        {
            string AppPath = "";
            Match m = Regex.Match(
                System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase.Replace("/", @"\"),
                        @"(?<=file:\\\\\\|^)(?:[^\\]+)?(\\[^\\]+)+(?=\\[^\\]+)");
            if (!m.Success) throw new Exception("Get execute path error.Can not run on this system.");
            else AppPath = m.Value + @"\";
            return AppPath;
        }
    }
}
