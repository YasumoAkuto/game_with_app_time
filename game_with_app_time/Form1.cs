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
using System.IO;

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
        private static extern uint GetWindowThreadProcessId(IntPtr hWnd, out int lpdwProcessId);

        private string beforeForegroundWindowApp = "t";
        private DateTime beforeDT;


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
            Process current = Process.GetCurrentProcess();//起動しているこのプロセスのことを得る


            MessageBox.Show(current.ProcessName);

            //フォルダが存在するかどうか
            if (Directory.Exists(@"c:\RecordTime\"))
            {

            }
            else
            {
               　//フォルダがない時にディレクトリを作成
                Directory.CreateDirectory(@"c:\RecordTime\");
            }

            StreamWriter sw = new StreamWriter(@"c:\RecordTime\time.txt");
            sw.WriteLine("test");
            sw.Close();
            
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            
        }

        delegate void DelegateProcess();

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            var Timer = new Stopwatch();
           

            while (!backgroundWorker1.CancellationPending)
            {

                /*                Process currentProcess = Process.GetCurrentProcess();
                                Console.WriteLine(currentProcess.ProcessName);*/
                StringBuilder sb = new StringBuilder(65535);//65535に特に意味はない
                GetWindowText(GetForegroundWindow(), sb, 65535);
                Console.WriteLine(sb);
                string st = sb.ToString();
                Console.WriteLine(st);
                int processid;
                GetWindowThreadProcessId(GetForegroundWindow(), out processid);
                st = processid.ToString();
                Process pro = Process.GetProcessById(processid);

                //nowForegroundWindowApp = pro.MainModule.FileVersionInfo.ProductName;
                //今のプロセスの名前を取得
                string nowForegroundWindowApp = pro.ProcessName;
                if(beforeForegroundWindowApp != nowForegroundWindowApp)//前のアクティブ状態のアプリと同じアプリがアクティブかどうか
                {
                    Timer.Stop();//計測停止
                    StreamWriter sw = new StreamWriter(@"c:\RecordTime\time.txt",true);
                    sw.WriteLine(beforeDT.ToString() + " " + beforeForegroundWindowApp + " " + Timer.Elapsed);
                    sw.Close();
                    Timer.Restart();
                }
                beforeForegroundWindowApp = pro.ProcessName;
                beforeDT = DateTime.Now;

                
                if (this.InvokeRequired)
                {
                    //DelegateProcess process = new DelegateProcess(ChangeLabelText);
                    this.Invoke(new Action<string>(this.ChangeLabelText), beforeForegroundWindowApp);
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
