using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Timers;
using System.Diagnostics;
using Microsoft.Win32;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml;
//using KProtectProcess;

namespace Prokil
{


    public partial class Form1 : Form
    {

        private string code="secret";
        XmlTextWriter writer;
        public string pn, pt;

        private List<Valueitem> ConList=new List<Valueitem>();

        private bool canXMLWRuse=true;
        private static bool needcontrol;

        public Form1()
        {
            InitializeComponent();
            CheckForIllegalCrossThreadCalls = false;
            needcontrol = !checkBox1.Checked;

            System.Timers.Timer detect = new System.Timers.Timer();
            detect.Interval = 1000;
            detect.Elapsed += detecting;
            
            SetAutoRun();
            
            if (File.Exists(GetAppPath() + "cpg.xml"))
            {
                listView1.Items.Clear();
                Readdata();
                
            }
            
            RDConInfo();
            
            if (needcontrol)
            {
                startcontrol();
            }
            
            detect.Start();
            this.Hide();
            
            //KProtectProcess.KProcess.SelfProtection();

        }

        public void detecting(object source, ElapsedEventArgs e)
        {
            try
            {

                Process[] p = Process.GetProcesses();

                foreach (ListViewItem m in listView1.Items)
                {
                    foreach (Process i in p)
                    {
                        if (i.ProcessName.Equals(m.Text))
                        {
                            m.SubItems[1].Text = (int.Parse(m.SubItems[1].Text) - 1).ToString();

                            if (int.Parse(m.SubItems[1].Text) <= 0)
                            {
                                m.SubItems[1].Text = "0";
                                KillProcess(m.SubItems[0].Text);
                            }

                            break;
                        }

                    }
                }

                if (canXMLWRuse) ControlXMLWR();

               // 这段是用文本文件保存控制的进程和时间信息，考虑改成XML文件格式
                /*if ((DateTime.Now.Minute <= 60) & (DateTime.Now.Second <= 60)) 
                {


                    FileStream aFile = new FileStream(GetAppPath() + "Log.pkl", FileMode.OpenOrCreate);
                    
                    StreamWriter sw = new StreamWriter(aFile);

                    sw.WriteLine(DateTime.Now.Date.ToShortDateString());
                    sw.WriteLine( DateTime.Now.TimeOfDay);

                    foreach (ListViewItem m in listView1.Items)
                    {
                        sw.WriteLine(m.Text+" "+m.SubItems[1].Text+" "+m.SubItems[2].Text);
                    }

                    sw.Close();
                }*/
                
                
            }
            catch(Exception ex)
            {
                MessageBox.Show("程序出现错误！请关闭重开！"+ex.ToString(),"ERROR");
            }
        }



        /// <summary>
        /// 关闭进程
        /// </summary>
        /// <param name="processName">进程名</param>
        private void KillProcess(string processName)
        {
            Process[] myproc = Process.GetProcesses();
            foreach (Process item in myproc)
            {
                if (item.ProcessName == processName)
                {
                    item.Kill();
                }
            }
        }


        private void 删除ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (int i in listView1.SelectedIndices)
            {
                listView1.Items.RemoveAt(i);
            }
        }

        private void 修改ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form2 fm = new Form2(this);
            
            foreach (int i in listView1.SelectedIndices)
            {
                pn = listView1.Items[i].Text;
                pt = listView1.Items[i].SubItems[1].Text;
                
            }
            fm.LoadForm2(pn, pt);
            fm.Show();
        }

        public void getcorr(string name, string time,string time1)
        {
            int snumber = 0;
            foreach (ListViewItem i in listView1.Items)
            {
                
                if ((i.Text.Equals(name))&(isNumberic(time))&(isNumberic(time1)))
                {
                    listView1.Items[snumber].SubItems[0].Text = name;
                    listView1.Items[snumber].SubItems[1].Text = time;
                    listView1.Items[snumber].SubItems[2].Text = time1;
                    listView1.Refresh();   
                    
                }
                snumber++;
            }          
      
        }

        public void getadd(string name, string time,string time1)
        {
            foreach (ListViewItem i in listView1.Items)
            {
                if ((i.Text.Equals(name)) & (isNumberic(time)) & (isNumberic(time1)))
                {
                    getcorr(name, time, time1);
                    return;
                }
            }
            if ((isNumberic(time)) & (isNumberic(time1)))
            {
                ListViewItem li = new ListViewItem();
                li.SubItems.Clear();
                li.SubItems[0].Text = name;
                li.SubItems.Add(time);
                li.SubItems.Add(time1);
                listView1.Items.Add(li);
                listView1.Refresh();
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (textBox1.Text.Equals(code))
            {
                tabControl1.Enabled = true;
            }
            else
            {
                tabControl1.Enabled = false;
            }

        }

        private void 添加ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form2 fm = new Form2(this);
            fm.isadd = true;
            fm.Show();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel=true;
            textBox1.Text = "";
            this.Hide();
       
        }

        private void Form1_SizeChanged(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
            {
                //this.Visible = false; //这个也可以
                textBox1.Text = "";
                this.Hide();               
                
            }
        }

        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            this.Show();
            
        }

        public static bool SetAutoRun()
        {
            
            try
            {
                string starupPath = Application.ExecutablePath;

                RegistryKey runKey = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", true);
                

                runKey.SetValue("Prokil", starupPath);

                runKey.Close();
                MessageBox.Show("成功");

            }

            catch
            {
                MessageBox.Show("失败");
                return false;

            }

            return true;

        }

        private void Readdata()
        {


            try
            {
                XmlTextReader reader = new XmlTextReader(GetAppPath() + "cpg.xml");
                //reader.WhitespaceHandling = WhitespaceHandling.None;
                string a, b, c;
                while (reader.Read())
                {
                    if ((reader.NodeType == XmlNodeType.Element) & (reader.Name == "CPG"))
                    {
                        reader.ReadToFollowing("process");
                        reader.Read();
                        a = reader.Value;
                        reader.ReadToFollowing("lefttime");
                        reader.Read();
                        b = reader.Value;
                        reader.ReadToFollowing("totaltime");
                        reader.Read();
                        c = reader.Value;

                        getadd(a, b, c);

                    }
                }

                reader.Close();
            }
            catch
            {
                return;
            }

            //这段是读取文本文件的配置信息稍后考虑用XML文件
            /*try
            {
                
                FileStream aFile = new FileStream(GetAppPath() + "Log.pkl", FileMode.Open);
                StreamReader sr = new StreamReader(aFile);

                string ss = sr.ReadLine();
                if (ss.Equals(DateTime.Now.Date.ToShortDateString()))
                {
                    ss = sr.ReadLine();
                    ss = sr.ReadLine();

                    
                    listView1.Items.Clear();

                    while (ss != null)
                    {
                        //MessageBox.Show(ss.Substring(0, ss.IndexOf(" "))+"!");
                        //MessageBox.Show(ss.Substring(ss.IndexOf(" ") , ss.Length - ss.IndexOf(" "))+"!");
                                               
                        getadd(ss.Substring(0,ss.IndexOf(" ")),ss.Substring(1+ss.IndexOf(" "),ss.LastIndexOf(" ")-ss.IndexOf(" ")-1),ss.Substring(ss.LastIndexOf(" ")+1,ss.Length-ss.LastIndexOf(" ")-1));

                        ss = sr.ReadLine();
                    
                    }
                    
                }

                sr.Close();
            }
            catch (IOException ex)
            {
                Console.WriteLine("An IOException has been thrown!");
                Console.WriteLine(ex.ToString());
                Console.ReadLine();
                return;
            }
             */
        }

        public static string GetAppPath()
        {
            string AppPath = "";
            Match m = Regex.Match(
                System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase.Replace("/", @"\"),
                        @"(?<=file:\\\\\\|^)(?:[^\\]+)?(\\[^\\]+)+(?=\\[^\\]+)");
            if (!m.Success) throw new Exception("Get execute path error.Can not run on this system.");
            else AppPath = m.Value + @"\";
            return AppPath;
        }

        public void ControlXMLWR()
        {
            canXMLWRuse = false;
            writer = new XmlTextWriter(GetAppPath()+"cpg.xml", null);
            writer.Formatting = Formatting.Indented;  //缩进格式
            writer.Indentation = 4;

            writer.WriteStartDocument();

            writer.WriteStartElement("ALLCPG");

            int i=0;

            foreach (ListViewItem m in listView1.Items)
            {
                writer.WriteStartElement("CPG");
                writer.WriteStartAttribute("ID", null);
                writer.WriteString(i.ToString());
                writer.WriteEndAttribute();

                writer.WriteStartElement("process");
                writer.WriteString(m.Text);
                writer.WriteEndElement();

                writer.WriteStartElement("lefttime");
                writer.WriteString(m.SubItems[1].Text);
                writer.WriteEndElement();

                writer.WriteStartElement("totaltime");
                writer.WriteString(m.SubItems[2].Text);
                writer.WriteEndElement();

                writer.WriteEndElement();

                i++;
                //sw.WriteLine(m.Text + " " + m.SubItems[1].Text + " " + m.SubItems[2].Text);
            }


            
            writer.Flush();
            writer.Close();
            canXMLWRuse = true;
        }

        private void listView1_MouseDown_1(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                if (listView1.SelectedIndices.Count == 0)
                {
                    修改ToolStripMenuItem.Enabled = false;
                    删除ToolStripMenuItem.Enabled = false;
                }
                else
                {
                    删除ToolStripMenuItem.Enabled = true;
                    修改ToolStripMenuItem.Enabled = true;
                }
                contextMenuStrip1.Show(this.listView1, new Point(e.X, e.Y));

            }
        }

        private void monthCalendar1_DateSelected(object sender, DateRangeEventArgs e)
        {
            
            Point p = monthCalendar1.PointToClient(MousePosition);
            contextMenuStrip2.Show(this.monthCalendar1,p);
        }

        private void TimeStart_Click(object sender, EventArgs e)
        {            
            textBox2.Text = monthCalendar1.SelectionStart.ToShortDateString();
        }

        private void TimeEnd_Click(object sender, EventArgs e)
        {
            try
            {
                DateTime a=new DateTime(1900,1,1);
                if (textBox2.Text != null)
                {a = DateTime.Parse(textBox2.Text); }
                if (monthCalendar1.SelectionStart < a)
                {
                    textBox3.Text = a.ToShortDateString();
                    textBox2.Text = monthCalendar1.SelectionStart.ToShortDateString();
                }
                else
                {
                    textBox3.Text = monthCalendar1.SelectionStart.ToShortDateString();
                }
            }
            catch
            {

            }
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if ((textBox2.Text != null) & (textBox3.Text != null) & (textBox4.Text != null))
            {
                wrConInfo();

            }
        }

        private void wrConInfo()
        {
            try
            {
                
                string[] ConInfo;
                ConInfo = textBox4.Text.Split(';');
                //FileStream aFile = new FileStream(GetAppPath() + "Log.pkl", FileMode.OpenOrCreate,FileAccess.ReadWrite);

                StreamWriter sw = new StreamWriter(GetAppPath() + "Log.pkl",true);
               
                sw.WriteLine(textBox2.Text);
                sw.WriteLine(textBox3.Text);
                foreach (string i in ConInfo)
                {  
                    sw.WriteLine(i);                    
                }
                sw.WriteLine("END");
                sw.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }

        }

        public void RDConInfo()
        {
            try
            {

                FileStream aFile = new FileStream(GetAppPath() + "Log.pkl", FileMode.Open);
                StreamReader sr = new StreamReader(aFile);
                
                string ft,tt,nt;
                ft = sr.ReadLine();
                
                while (ft!= null)
                {
                    tt = sr.ReadLine();
                    nt = sr.ReadLine();
                    string n,t;

                    while(!nt.Equals("END"))
                    {
                        n = nt.Substring(0, nt.IndexOf(' '));
                        t = nt.Remove(0, nt.IndexOf(' ')+1);
                        Valueitem v = new Valueitem(ft, tt,n,t);
                     
                        ConList.Add(v);
                        nt = sr.ReadLine();
                        
                    }
                    ft = sr.ReadLine();
                    
                }
                
                

                sr.Close();
            }
            catch (IOException ex)
            {
                Console.WriteLine("An IOException has been thrown!");
                Console.WriteLine(ex.ToString());
                Console.ReadLine();
                
            }
        }


        private void startcontrol()
        {
            foreach (ListViewItem i in listView1.Items)
            {
                foreach (Valueitem m in ConList)
                {
                    
                    if ((DateTime.Now<=DateTime.Parse(m.totime))&(DateTime.Now>=DateTime.Parse(m.fromtime)))
                    {
                        if (i.Text.Equals(m.name))
                        {
                            getcorr(m.name, m.time, m.time);

                        }
                    }
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Form3 fm = new Form3();
            fm.Show();
        }

        protected bool isNumberic(string message)
        {
            System.Text.RegularExpressions.Regex rex =
            new System.Text.RegularExpressions.Regex(@"^\d+$");
            
            if (rex.IsMatch(message))
            {
                
                return true;
            }
            else
                return false;
        }

        //Point p=new Point(e.X,e.Y);
        //    contextMenuStrip2.Show(this.monthCalendar1,p);

    }
}
