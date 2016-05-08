using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Microsoft.Win32;

namespace Prokil
{
    
    public partial class Form2 : Form
    {

        Form1 parrentform;
        public  bool isadd=false;

        public Form2(Form1 f1)
        {
            parrentform = f1;
            InitializeComponent();
        }
        internal void LoadForm2(string str1,string str2)
        {
            this.textBox1.Text = str1;
            this.textBox2.Text = str2;
        }

        private void button1_Click(object sender, EventArgs e)
        {
           try
                {
                    int x=Convert.ToInt16(textBox2.Text.Trim());
                    if (!isadd)
                    {
                        parrentform.getcorr(this.textBox1.Text, x.ToString(),x.ToString());

                    }
                    else if (isadd)
                    {
                        parrentform.getadd(this.textBox1.Text, x.ToString(), x.ToString());

                    }
                    this.Close();
                }
                catch 
                {
                    MessageBox.Show("出现错误","ERROR");
                    textBox2.Text = "3600";
                }
            
         }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
