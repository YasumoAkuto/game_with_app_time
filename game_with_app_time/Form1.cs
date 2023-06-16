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

/*        [System.Runtime.InteropServices.DllImport("kernel32.dll")]
        private static extern bool AllocConsole();*/

        [DllImport("user32.dll")]
        public static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll", EntryPoint = "GetWindowText", CharSet = CharSet.Auto)]
        public static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern uint GetWindowThreadProcessId(IntPtr hWnd, out int lpdwProcessId);

        private string beforeForegroundWindowApp = "t";
        private DateTime beforeDT;
        private List<AppData> appData = new List<AppData>();
        private string comboAppName = "";
        private int comboAppIndex = -1;
        private List<string> registeredComboBox = new List<string>();
        private AppData registerdAppData = new AppData();
        private string folderPath = System.IO.Directory.GetCurrentDirectory();

        //アプリごとのデータを登録するクラス
        class AppData
        {
            private string appName;
            private TimeSpan totalTime;


            /// <summary>
            /// アプリケーションの名前
            /// </summary>
            public string AppName
            {
                get { return appName; }
                set { appName = value; }
            }

            /// <summary>
            /// アプリケーションの総起動時間
            /// </summary>
            public TimeSpan TotalTime
            {
                get { return totalTime; }
                set { totalTime = value; }
            }
        }


        public Form1()
        {                                                    
            InitializeComponent();
            //AllocConsole();

            //フォルダが存在するかどうか
            System.IO.Directory.SetCurrentDirectory(folderPath);
            if (Directory.Exists(@"RecordTime\")) { }
            else
            {
               　//フォルダがない時にディレクトリを作成
                Directory.CreateDirectory(@"RecordTime\");
            }

            LoadAppData();

            backgroundWorker1.RunWorkerAsync();


            Application.ApplicationExit += new EventHandler(this.Application_ApplicationExit);



        }

        private void button1_Click(object sender, EventArgs e)
        {
            if(comboBox1.SelectedItem != null)
            {
                registerdAppData.AppName = comboBox1.SelectedItem.ToString();
                label2.Text = registerdAppData.AppName;
            }

/*            if(textBox1.Text != "")
            {
                bool nameFlag = false;
                for(int i = 0; i < appData.Count; i++)
                {
                    if(appData[i].AppName == textBox1.Text)
                    {
                        comboAppName = textBox1.Text;
                        comboAppIndex = i;
                        nameFlag = true;
                    }
                }
                if(nameFlag == true)
                {
                    MessageBox.Show("登録完了。");
                }
                else
                {
                    MessageBox.Show("そのアプリ名はありません。");
                }

            }
            else
            {
                MessageBox.Show("何も入力されていません。");
            }

            if(comboAppName != "")
            {
                label3.Text = appData[comboAppIndex].TotalTime.ToString();
            }*/
            
/*            Process[] localAll = Process.GetProcesses();
            Process current = Process.GetCurrentProcess();//起動しているこのプロセスのことを得る*/


            //MessageBox.Show(current.ProcessName);
            //MessageBox.Show(appData[1].TotalTime.TotalSeconds.ToString());


/*            StreamWriter sw = new StreamWriter(@"c:\RecordTime\time.txt");
            sw.WriteLine("test");
            sw.Close();*/
            
        }

        delegate void DelegateProcess();

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            var Timer = new Stopwatch();
           
            //Update的な感じで使うために必要な手順
            while (!backgroundWorker1.CancellationPending)
            {

                /*                Process currentProcess = Process.GetCurrentProcess();
                                Console.WriteLine(currentProcess.ProcessName);*/
                StringBuilder sb = new StringBuilder(65535);//65535に特に意味はない
                GetWindowText(GetForegroundWindow(), sb, 65535);
                string st = sb.ToString();
                int processid;
                GetWindowThreadProcessId(GetForegroundWindow(), out processid);
                st = processid.ToString();
                Process pro = Process.GetProcessById(processid);

                //nowForegroundWindowApp = pro.MainModule.FileVersionInfo.ProductName;
                //今のプロセスの名前を取得
                string nowForegroundWindowApp = pro.ProcessName;
                if(beforeForegroundWindowApp != nowForegroundWindowApp)//前のアクティブ状態のアプリと同じアプリがアクティブかどうか
                {
                    
                    //時間計測
                    Timer.Stop();//計測停止
                    Directory.SetCurrentDirectory(folderPath);
                    StreamWriter sw = new StreamWriter(@"RecordTime\time.txt",true);
                    sw.WriteLine(beforeDT.ToString() + "," + beforeForegroundWindowApp + "," + Timer.Elapsed);
                    TimeSpan span = Timer.Elapsed;
                    sw.Close();
                    Timer.Restart();

                    //データの中に既にアプリケーションの名前が登録されているのかどうかを確認
                    bool flag = false;
                    //データのインデックスを確認するためのint
                    int cnt = -1;
                    for (int i = 0; i < appData.Count; i++)
                    {
                        if(appData[i].AppName == beforeForegroundWindowApp)
                        {
                            flag = true;
                            cnt = i;
                        }
                    }

                    if(flag == false)//データにアプリケーションが保存されていない場合
                    {
                        //アプリケーションの登録
                        AppData test = new AppData();
                        test.AppName = beforeForegroundWindowApp;
                        test.TotalTime = span;
                        appData.Add(test);
                    }
                    else
                    {
                        if(cnt >= 0)
                        {
                            appData[cnt].TotalTime = appData[cnt].TotalTime + span;
                        }
                    }

                    if (this.InvokeRequired)
                    {
                        this.Invoke(new Action<string>(this.AddComboBox), beforeForegroundWindowApp);
                    }
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

        private void AddComboBox(string text)
        {
            bool comboFlag = false;
            for(int i = 0; i < registeredComboBox.Count; i++)
            {
                if (registeredComboBox[i] == text)
                {
                    comboFlag = true;
                }
            }
            if(comboFlag == false)
            {
                registeredComboBox.Add(text);
                comboBox1.Items.Add(text);
            }
            return;
        }

        private void LoadAppData()
        {
            Directory.SetCurrentDirectory(folderPath);
            try
            {
                StreamReader sr = new StreamReader(@"RecordTime\TotalTime.txt");
                while (sr.Peek() > -1)
                {
                    string s = sr.ReadLine();
                    string[] s_array = s.Split(',');
                    AppData y = new AppData();
                    y.AppName = s_array[0];
                    y.TotalTime = TimeSpan.Parse(s_array[1]);
                    appData.Add(y);
                }
            }
            catch(FileNotFoundException e)
            {
                return;
            }
            
        }

        private void SaveAppData()
        {
            Directory.SetCurrentDirectory(folderPath);

            StreamWriter sw = new StreamWriter(@"RecordTime\TotalTime.txt");
            for(int i = 0; i < appData.Count; i++)
            {
                sw.WriteLine(appData[i].AppName + "," + appData[i].TotalTime.ToString());
            }
            sw.Close();
        }

        private void Application_ApplicationExit(object sender, EventArgs e)
        {
            SaveAppData();
            Directory.SetCurrentDirectory(folderPath);
            StreamWriter sw = new StreamWriter(@"RecordTime\time.txt", true);
            sw.WriteLine("exit");
            sw.Close();
            Application.ApplicationExit -= new EventHandler(this.Application_ApplicationExit);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (comboAppName != "")
            {
                label3.Text = appData[comboAppIndex].TotalTime.ToString();
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selectedItem = comboBox1.SelectedItem.ToString();
            for(int i = 0; i < appData.Count; i++)
            {
                if(appData[i].AppName == selectedItem)
                {
                    comboAppIndex = i;
                    comboAppName = selectedItem;
                    label3.Text = appData[i].TotalTime.ToString();
                }
            }
        }
    }
}
