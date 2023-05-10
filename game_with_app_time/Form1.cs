using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace game_with_app_time
{
    public partial class Form1 : Form
    {

        [System.Runtime.InteropServices.DllImport("kernel32.dll")]
        private static extern bool AllocConsole();

        [DllImport("user32.dll")]
        public static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll", EntryPoint = "GetWindowText", CharSet = CharSet.Auto)]
        public static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

        public Form1()
        {
                                                                       
            InitializeComponent();
            AllocConsole();
            backgroundWorker1.RunWorkerAsync();
            Console.WriteLine("hogehoge");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            
            Process[] localAll = Process.GetProcesses();
            Process current = Process.GetCurrentProcess();


            MessageBox.Show(current.ProcessName);
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            
        }

        delegate void DelegateProcess();

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            
            while (!backgroundWorker1.CancellationPending)
            {

                /*                Process currentProcess = Process.GetCurrentProcess();
                                Console.WriteLine(currentProcess.ProcessName);*/
                StringBuilder sb = new StringBuilder(65535);//65535に特に意味はない
                GetWindowText(GetForegroundWindow(), sb, 65535);
                Console.WriteLine(sb);
                string st = sb.ToString();
                Console.WriteLine(st);
                uint processid;
                GetWindowThreadProcessId(GetForegroundWindow(), out processid);
                st = processid.ToString();
                Process pro = Process.GetProcessById((int)processid);
                
                
                if (this.InvokeRequired)
                {
                    //DelegateProcess process = new DelegateProcess(ChangeLabelText);
                    this.Invoke(new Action<string>(this.ChangeLabelText), pro.ProcessName);
                }
            }
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            backgroundWorker1.CancelAsync();
            Application.DoEvents();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void ChangeLabelText(string tex)
        {
            label1.Text = tex;
            return;
        }
    }
}
