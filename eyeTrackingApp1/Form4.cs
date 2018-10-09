using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace eyeTrackingApp1
{
    public partial class Form4 : Form
    {
        public Form4()
        {
            InitializeComponent();
            Opacity = 0;
            //progress_check();
        }

        private void Form4_Load(object sender, EventArgs e)
        {
            Animator.Animate(150, (frame, frequency) =>
            {
                if (!Visible || IsDisposed) return false;
                Opacity = (double)frame / frequency;
                return true;
            });
        }

        private void Form4_Shown(object sender, EventArgs e)
        {
            //progress_check();
            

        }

        public void progress_check()
        {
            Process p = Process.Start(@"C:\Users\takami\OneDrive\実験関連\数理情報工学\画像認識\dist\fashion_test\fashion_test.exe");
            
            /*
            p.WaitForExit(10000);
            if (p.HasExited)
            {
                writeLog("終了");
                textBox_Log.Refresh();
            }
            else
            {
                writeLog("まだ");
                textBox_Log.Refresh();
            }
            */
            //p.WaitForExit(10000);


            //fm4.label1.Text = "処理中...";

            int max_filecount = Directory.GetFiles(@"C:\Users\takami\OneDrive\実験関連\数理情報工学\画像認識\photo_test", "*", SearchOption.AllDirectories).Length;
            progressBar1.Maximum = max_filecount;
            progressBar1.Minimum = 0;
            progressBar1.Value = 0;

            while (!p.HasExited)
            {
                int fileCount = Directory.GetFiles(@"C:\Users\takami\OneDrive\実験関連\数理情報工学\画像認識\photo_test", "*", SearchOption.AllDirectories).Length;
                int newCount = max_filecount - fileCount;

                if (newCount >= max_filecount / 10)
                    label1.Text = "10%";
                if (newCount >= max_filecount / 30)
                    label1.Text = "30%";
                if (newCount >= max_filecount / 50)
                    label1.Text = "50%";
                if (newCount >= max_filecount / 70)
                    label1.Text = "70%";
                if (newCount >= max_filecount / 90)
                    label1.Text = "90%";

                progressBar1.Value = newCount;
                label1.Refresh();
                label1.Refresh();
                progressBar1.Refresh();
                //Application.DoEvents();
            }
            Close();
            p.Close();
            p.Dispose();
        }

    }
}
