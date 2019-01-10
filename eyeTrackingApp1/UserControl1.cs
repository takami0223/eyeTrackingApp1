﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Tobii.Interaction;
using Tobii.Interaction.Framework;

namespace eyeTrackingApp1
{
    public partial class UserControl1 : UserControl
    {
        public UserControl1()
        {
            InitializeComponent();
        }

        private void UserControl1_Load(object sender, EventArgs e)
        {
            /*-------------------------*/
            /* ボタンを成型（三角ver） */
            /*-------------------------*/

            /*チェンジボタンを三角に成型*/
            Change.SetBounds(Change.Location.X, 0, 130, 130);
            Point[] change_points =
            {
                new Point(0,0),
                new Point(130,0),
                new Point(0,130)
            };
            byte[] change_types =
            {
                (byte) System.Drawing.Drawing2D.PathPointType.Line,
                (byte) System.Drawing.Drawing2D.PathPointType.Line,
                (byte) System.Drawing.Drawing2D.PathPointType.Line
            };
            System.Drawing.Drawing2D.GraphicsPath path = new System.Drawing.Drawing2D.GraphicsPath(change_points, change_types);
            Change.Region = new Region(path);
            
            /*チェンジボタンに表示する画像*/
            var image = Properties.Resources.woman_icon2;//Image.FromFile(@"C:\Users\takami\OneDrive\実験関連\woman_icon2.png");
            Change.BackgroundImage = image;

            /*---------------------*/
            /*ボタンを成型（丸ver）*/
            /*---------------------*/

            /*シャツボタンを丸く成型*/
            
            Shirt.SetBounds(Shirt.Location.X, Shirt.Top, 301, 301);
            path = new System.Drawing.Drawing2D.GraphicsPath();
            path.AddEllipse(new System.Drawing.Rectangle(10, 10, 280, 280));//(5, 5, 290, 290));
            Shirt.Region = new Region(path);
            
            /*ズボンボタンを丸く成型*/
            
            Pant.SetBounds(Pant.Location.X, Pant.Top, 301, 301);
            path = new System.Drawing.Drawing2D.GraphicsPath();
            path.AddEllipse(new System.Drawing.Rectangle(10, 10, 280, 280));
            Pant.Region = new Region(path);
            
            /*ワンピースボタンを丸く成型*/
            
            Shoes.SetBounds(Shoes.Location.X, Shoes.Top, 301, 301);
            path = new System.Drawing.Drawing2D.GraphicsPath();
            path.AddEllipse(new System.Drawing.Rectangle(10, 10, 280, 280));
            Shoes.Region = new Region(path);

            /*全てボタンを丸く成型*/
            All.SetBounds(All.Location.X, All.Top, 301, 301);
            path = new System.Drawing.Drawing2D.GraphicsPath();
            path.AddEllipse(new System.Drawing.Rectangle(10, 10, 280, 280));
            All.Region = new Region(path);

            /*データ追加ボタンを丸く成型*/

            Add_data.SetBounds(Add_data.Location.X, Add_data.Top, 130, 130);
            path = new System.Drawing.Drawing2D.GraphicsPath();
            path.AddEllipse(new System.Drawing.Rectangle(5, 5, 120, 120));//(10, 10, 110, 110));
            Add_data.Region = new Region(path);
            
            /*好みのデータボタンを丸く成型*/
            
            Human.SetBounds(Human.Location.X, Human.Top, 130, 130);
            path = new System.Drawing.Drawing2D.GraphicsPath();
            path.AddEllipse(new System.Drawing.Rectangle(5,5,120,120));//(15, 15, 100, 100));
            Human.Region = new Region(path);

            /*設定ボタンを丸く成型*/

            Setting.SetBounds(Setting.Location.X, Setting.Top, 130, 130);
            path = new System.Drawing.Drawing2D.GraphicsPath();
            path.AddEllipse(new System.Drawing.Rectangle(5, 5, 120, 120));
            Setting.Region = new Region(path);

            /*-------------------------*/
            /*ボタンを成型（五角形ver）*/
            /*-------------------------*/
            /*
            Shirt.SetBounds(Shirt.Location.X, Shirt.Top, 301, 301);
            Point[] shirt_points =
            {
                new Point(0,0),
                new Point(301,0),
                new Point(301,150),
                new Point(150,301),
                new Point(0,301)
            };
            byte[] shirt_types =
            {
                (byte) System.Drawing.Drawing2D.PathPointType.Line,
                (byte) System.Drawing.Drawing2D.PathPointType.Line,
                (byte) System.Drawing.Drawing2D.PathPointType.Line,
                (byte) System.Drawing.Drawing2D.PathPointType.Line,
                (byte) System.Drawing.Drawing2D.PathPointType.Line
            };
            path = new System.Drawing.Drawing2D.GraphicsPath(shirt_points, shirt_types);
            Shirt.Region = new Region(path);
            */
        }

        Random r = new Random();

        /* 見られた場所、時間を管理する構造体 */
        public struct Gaze : IComparable
        {
            public int y;
            public int x;
            public int count;

            public int CompareTo(object obj)
            {
                return ((Gaze)obj).count - count;
            }
        }

        /* 見られた画像、割合を管理する構造体 */
        public struct Image_Proportion : IComparable
        {
            public Image image;
            public int count;
            public double proportion;

            public int CompareTo(object obj)
            {
                return ((Image_Proportion)obj).count - count;
            }
        }

        /* 一つのパネルに表示される画像一枚ずつのプロパティを管理する構造体 */
        public struct Image_Property
        {
            public Image image;
            public int category;
            public int color;
            public int count;
        }

        /*------------------------------------------*/
        /*                 ※重要※                 */
        /*                                          */
        /*[メインパス][カテゴリ名][色]を格納した配列*/
        /*------------------------------------------*/
        enum Category
        {
            Shirt,
            Pant,
            Shoes,

            MAX_CATEGORY
        }

        enum Color
        {
            Red,
            Blue,
            Green,
            White,
            Black,
            Other,

            MAX_COLOR
        }

        static string MAIN_PATH = @"C:\Users\takami\OneDrive\実験関連\数理情報工学\画像認識\photo_out";
        static string[] CATEGORY_NAME = new string[(int)Category.MAX_CATEGORY] {"シャツ", "ズボン", "シューズ" };
        static string[] COLOR_NAME = new string[(int)Color.MAX_COLOR] { "赤", "青", "緑", "白", "黒", "その他" };

        /* ウィンドウサイズ */
        static int MAX_WIDTH = 1920;
        static int MAX_HEIGHT = 1080;

        /* 分割比 */
        static int WIDTH_RATIO = 240;//320;
        static int HEIGHT_RATIO = 270;

        /* 一つのボックスサイズ */
        static int BOX_HEIGHT = MAX_HEIGHT / HEIGHT_RATIO; //4
        static int BOX_WIDTH = MAX_WIDTH / WIDTH_RATIO;   //6
        

        /* 各カテゴリーの画像ファイル名配列 */
        string[][] shirt_files =
        {
            Directory.GetFiles(Path.Combine(MAIN_PATH, CATEGORY_NAME[(int)Category.Shirt], COLOR_NAME[(int)Color.Red]), "*.jpg", SearchOption.AllDirectories),
            Directory.GetFiles(Path.Combine(MAIN_PATH, CATEGORY_NAME[(int)Category.Shirt], COLOR_NAME[(int)Color.Blue]), "*.jpg", SearchOption.AllDirectories),
            Directory.GetFiles(Path.Combine(MAIN_PATH, CATEGORY_NAME[(int)Category.Shirt], COLOR_NAME[(int)Color.Green]), "*.jpg", SearchOption.AllDirectories),
            Directory.GetFiles(Path.Combine(MAIN_PATH, CATEGORY_NAME[(int)Category.Shirt], COLOR_NAME[(int)Color.White]), "*.jpg", SearchOption.AllDirectories),
            Directory.GetFiles(Path.Combine(MAIN_PATH, CATEGORY_NAME[(int)Category.Shirt], COLOR_NAME[(int)Color.Black]), "*.jpg", SearchOption.AllDirectories),
            Directory.GetFiles(Path.Combine(MAIN_PATH, CATEGORY_NAME[(int)Category.Shirt], COLOR_NAME[(int)Color.Other]), "*.jpg", SearchOption.AllDirectories)
        };

        string[][] pant_files =
        {
            Directory.GetFiles(Path.Combine(MAIN_PATH, CATEGORY_NAME[(int)Category.Pant], COLOR_NAME[(int)Color.Red]), "*.jpg", SearchOption.AllDirectories),
            Directory.GetFiles(Path.Combine(MAIN_PATH, CATEGORY_NAME[(int)Category.Pant], COLOR_NAME[(int)Color.Blue]), "*.jpg", SearchOption.AllDirectories),
            Directory.GetFiles(Path.Combine(MAIN_PATH, CATEGORY_NAME[(int)Category.Pant], COLOR_NAME[(int)Color.Green]), "*.jpg", SearchOption.AllDirectories),
            Directory.GetFiles(Path.Combine(MAIN_PATH, CATEGORY_NAME[(int)Category.Pant], COLOR_NAME[(int)Color.White]), "*.jpg", SearchOption.AllDirectories),
            Directory.GetFiles(Path.Combine(MAIN_PATH, CATEGORY_NAME[(int)Category.Pant], COLOR_NAME[(int)Color.Black]), "*.jpg", SearchOption.AllDirectories),
            Directory.GetFiles(Path.Combine(MAIN_PATH, CATEGORY_NAME[(int)Category.Pant], COLOR_NAME[(int)Color.Other]), "*.jpg", SearchOption.AllDirectories)
        };

        string[][] shoes_files =
        {
            Directory.GetFiles(Path.Combine(MAIN_PATH, CATEGORY_NAME[(int)Category.Shoes], COLOR_NAME[(int)Color.Red]), "*.jpg", SearchOption.AllDirectories),
            Directory.GetFiles(Path.Combine(MAIN_PATH, CATEGORY_NAME[(int)Category.Shoes], COLOR_NAME[(int)Color.Blue]), "*.jpg", SearchOption.AllDirectories),
            Directory.GetFiles(Path.Combine(MAIN_PATH, CATEGORY_NAME[(int)Category.Shoes], COLOR_NAME[(int)Color.Green]), "*.jpg", SearchOption.AllDirectories),
            Directory.GetFiles(Path.Combine(MAIN_PATH, CATEGORY_NAME[(int)Category.Shoes], COLOR_NAME[(int)Color.White]), "*.jpg", SearchOption.AllDirectories),
            Directory.GetFiles(Path.Combine(MAIN_PATH, CATEGORY_NAME[(int)Category.Shoes], COLOR_NAME[(int)Color.Black]), "*.jpg", SearchOption.AllDirectories),
            Directory.GetFiles(Path.Combine(MAIN_PATH, CATEGORY_NAME[(int)Category.Shoes], COLOR_NAME[(int)Color.Other]), "*.jpg", SearchOption.AllDirectories)
        };

        /* パネルに表示される画像一枚に対する詳細データリスト */
        List<Image_Property> image_property = new List<Image_Property>();

        /* パネル一面での上位三枚の詳細データ */
        List<Image_Property> top_image_property = new List<Image_Property>();
        //Image[,] img = new Image[BOX_HEIGHT, BOX_WIDTH];

        /* 画面が更新された回数と更新終了フラグ */
        int panel_change_count = 0;
        bool change_finish_flag = false;

        /* 各カテゴリでの割合管理変数 */
        public static List<Image_Proportion> image_proportion_shirt = new List<Image_Proportion>();
        public static List<Image_Proportion> image_proportion_pant = new List<Image_Proportion>();
        public static List<Image_Proportion> image_proportion_shoes = new List<Image_Proportion>();
        public static List<Image_Proportion> image_proportion_all = new List<Image_Proportion>();
        static double change_shirt_image_count = 0;
        static double change_pant_image_count = 0;
        static double change_shoes_image_count = 0;
        static double change_image_count = 0;

        /*--------------------------------------*/
        /* 測定の度にリセットすべき変数の初期化 */
        /*--------------------------------------*/
        private void Init_Variable()
        {
            top_image_property.Clear();
            panel_change_count = 0;
            change_finish_flag = false;
        }

        /*------------------------------------------*/
        /* 各カテゴリのボタンをクリックした際の関数 */
        /*------------------------------------------*/
        private void Shirt_Click(object sender, EventArgs e)
        {
            Cursor.Hide();
            textBox_Log.ResetText();

            /*コントロ－ル（パネル）を生成*/
            Panel panel = new Panel();
            panel.Location = new Point(0, 0);
            panel.Size = new System.Drawing.Size(MAX_WIDTH, MAX_HEIGHT);
            panel.Paint += new PaintEventHandler(this.Shirt_Panel_Paint);
            Controls.Add(panel);
            panel.BringToFront();

            for(int i = 0; i < 4; i++)
            //while(!change_finish_flag)
            {
                writeLog("["+panel_change_count+"]回目");
                panel.Refresh();
                Eye_Tracking(image_proportion_shirt, image_proportion_all, panel);
                panel_change_count++;
            }

            Init_Variable();

            panel.Hide(); //コントロール（パネル）を消去
            Cursor.Show();
        }

        private void Pant_Click(object sender, EventArgs e)
        {
            Cursor.Hide();
            textBox_Log.ResetText();

            /*コントロ－ル（パネル）を生成*/
            Panel panel = new Panel();
            panel.Location = new Point(0, 0);
            panel.Size = new System.Drawing.Size(MAX_WIDTH, MAX_HEIGHT);
            panel.Paint += new PaintEventHandler(this.Pant_Panel_Paint);
            Controls.Add(panel);
            panel.BringToFront();

            for(int i = 0; i < 3; i++)
            {
                panel.Refresh();
                Eye_Tracking(image_proportion_pant, image_proportion_all, panel);
            }
            
            panel.Hide(); //コントロール（パネル）を消去

            Cursor.Show();
        }

        private void Shoes_Click(object sender, EventArgs e)
        {
            Cursor.Hide();
            textBox_Log.ResetText();

            /*コントロ－ル（パネル）を生成*/
            Panel panel = new Panel();
            panel.Location = new Point(0, 0);
            panel.Size = new System.Drawing.Size(MAX_WIDTH, MAX_HEIGHT);
            panel.Paint += new PaintEventHandler(this.Shoes_Panel_Paint);
            Controls.Add(panel);
            panel.BringToFront();

            for (int i = 0; i < 3; i++)
            {
                panel.Refresh();
                Eye_Tracking(image_proportion_shoes, image_proportion_all, panel);
            }

            panel.Hide(); //コントロール（パネル）を消去

            Cursor.Show();
        }

        private void All_Click(object sender, EventArgs e)
        {
            Cursor.Hide();
            textBox_Log.ResetText();

            /*コントロ－ル（パネル）を生成*/
            Panel panel = new Panel();
            panel.Location = new Point(0, 0);
            panel.Size = new System.Drawing.Size(MAX_WIDTH, MAX_HEIGHT);
            panel.Paint += new PaintEventHandler(this.All_Panel_Paint);
            Controls.Add(panel);
            panel.BringToFront();

            for (int i = 0; i < 3; i++)
            {
                panel.Refresh();
                Eye_Tracking(image_proportion_all, image_proportion_all, panel);
            }

            panel.Hide(); //コントロール（パネル）を消去

            Cursor.Show();
        }


        /*-------------------------------------------------*/
        /* 視線の測定、ランキングと割合を取得（メイン関数）*/
        /*-------------------------------------------------*/
        private void Eye_Tracking(List<Image_Proportion> proportion, List<Image_Proportion> all_proportion, Panel panel)
        {
            /*アイトラッカー関連*/
            var host = new Host();
            var gazePointDataStream = host.Streams.CreateGazePointDataStream();
            int point;
            Gaze[] gaze = new Gaze[BOX_HEIGHT * BOX_WIDTH];

            /*どのカテゴリの測定がされているかフラグを立てておく*/
            int category_flag = 0;
            bool same = false;

            if (proportion == image_proportion_shirt)
            {
                category_flag = (int)Category.Shirt;
            }
            else if (proportion == image_proportion_pant)
            {
                category_flag = (int)Category.Pant;
            }
            else if (proportion == image_proportion_shoes)
            {
                category_flag = (int)Category.Shoes;
            }
            else if (proportion == all_proportion)
            {
                category_flag = (int)Category.MAX_CATEGORY;
                same = true;
            }

            top_image_property.Clear();

            /*座標と見られた時間の初期化*/
            for (int i = 0; i < BOX_HEIGHT * BOX_WIDTH; i++)
            {
                gaze[i].y = 0;
                gaze[i].x = 0;
                gaze[i].count = 0;
            }

            //System.Threading.Thread.Sleep(5000); //測定無しの状態で画像を見せるため

            gazePointDataStream.GazePoint((gazePointX, gazePointY, ts) =>
            {
                if (gazePointX >= 0 && gazePointY >= 0 && gazePointX <= MAX_WIDTH && gazePointY <= MAX_HEIGHT)
                {
                    int y = (int)(gazePointY / HEIGHT_RATIO);
                    int x = (int)(gazePointX / WIDTH_RATIO);

                    point = (y * (BOX_WIDTH - 1)) + (y + x);
                    gaze[point].y = y;
                    gaze[point].x = x;
                    gaze[point].count++;
                }
            });

            System.Threading.Thread.Sleep(15000); //測定時間

            //System.Threading.Thread.Sleep(5000);

            host.DisableConnection();

            Array.Sort(gaze); //countに関してソートされる

            /*各カテゴリ単位で見られた時間の総和を求める(割合算出に使用)*/
            for (int a = 0; a < BOX_HEIGHT * BOX_WIDTH; a++)
            {
                if (gaze[a].count != 0)
                {
                    switch (category_flag)
                    {
                        case (int)Category.Shirt:
                            change_shirt_image_count += gaze[a].count;
                            break;

                        case (int)Category.Pant:
                            change_pant_image_count += gaze[a].count;
                            break;

                        case (int)Category.Shoes:
                            change_shoes_image_count += gaze[a].count;
                            break;
                    }
                    change_image_count += gaze[a].count;
                }
            }

            /*見られた画像の割合を更新*/
            Data_Registration(gaze, proportion, category_flag);
            if (!same) Data_Registration(gaze, all_proportion, (int)Category.MAX_CATEGORY);

            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox2.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox3.SizeMode = PictureBoxSizeMode.Zoom;

            /*トップ3位を登録*/
            for (int num = 0; num < 10; num++)
            {
                Image_Property tmpDate = image_property[Get_Point(gaze, num)];
                tmpDate.count = gaze[num].count;
                image_property[Get_Point(gaze, num)] = tmpDate;
                top_image_property.Add(image_property[Get_Point(gaze, num)]);
                //writeLog("color : " + top_image_property[num].color + " [" + top_image_property[num].count + "]");
            }
            //Finish_Gudge();

            pictureBox1.Image = top_image_property[0].image;
            pictureBox2.Image = top_image_property[1].image;
            pictureBox3.Image = top_image_property[2].image;

            //writeLog("カテゴリ : " + top_image_property[0].category);
            //writeLog("カラー : " + top_image_property[0].color);

            /*三週目以降、終了条件のチェック*/
            //if (panel_change_count >= 3) Finish_Gudge();

        }
        
        private void Data_Registration(Gaze[] gaze, List<Image_Proportion> image_proportion, int category_flag)
        {
            double count = 0;
            
            switch(category_flag)
            {
                case (int)Category.Shirt:
                    count = change_shirt_image_count;
                    break;

                case (int)Category.Pant:
                    count = change_pant_image_count;
                    break;

                case (int)Category.Shoes:
                    count = change_shoes_image_count;
                    break;

                case (int)Category.MAX_CATEGORY:
                    count = change_image_count;
                    break;
            }

            for (int a = 0; a < BOX_HEIGHT * BOX_WIDTH; a++)
            {
                if (gaze[a].count != 0)
                {
                    bool check = false;

                    for (int b = 0; b < image_proportion.Count; b++)
                    {
                        Image_Proportion tmpData = image_proportion[b];

                        /* もし過去に同じ画像が登録されていたら、「カウント」と「割合」を更新 */
                        if (Image_Compare(image_property[Get_Point(gaze, a)].image, tmpData.image))
                        {
                            //System.Threading.Thread.Sleep(100);
                            tmpData.count += gaze[a].count;
                            tmpData.proportion = (tmpData.count / count) * 100;
                            image_proportion[b] = tmpData;
                            check = true;
                        }

                        /* 増加するchange_image_countに合わせて「割合」のみ更新 */
                        else
                        {
                            tmpData.proportion = (tmpData.count / count) * 100;
                            image_proportion[b] = tmpData;
                        }
                    }

                    /*もし初めて見る画像だったら、「画像」「カウント」「割合」を登録*/
                    if (!check)
                    {
                        image_proportion.Add(new Image_Proportion()
                        {
                            image = image_property[Get_Point(gaze, a)].image,
                            count = gaze[a].count,
                            proportion = (gaze[a].count / count) * 100
                        });
                    }
                }
            }

            image_proportion.Sort();

            /*
            for (int b = 0; b < image_proportion.Count; b++)
            {
                string s1 = "" +image_proportion[b].count + " : " + image_proportion[b].proportion + "%";
                writeLog(s1);
            }
            */
            /*
            string s2 = "change_image_count : " + count;
            writeLog(s2);
            string s3 = "image_proportion.Count : " + image_proportion.Count;
            writeLog(s3);
            */
        }

        private void Finish_Gudge()
        {
            int[] color_count = new int[(int)Color.MAX_COLOR]; //各カラーのランキング入りした数

            color_count = Color_Count();
            int[] temp = new int[(int)Color.MAX_COLOR];

            temp = color_count;
            Array.Sort(temp);
            Array.Reverse(temp);

            foreach (int x in temp) writeLog("[" + x + "]");
            writeLog("" + (temp[0] - temp[1]));

            if (temp[0] - temp[1] >= 200)
            {
                change_finish_flag = true;
            }
        }

        private int Get_Point(Gaze[] gaze, int set)
        {
            int point = (gaze[set].y * (BOX_WIDTH - 1)) + (gaze[set].y + gaze[set].x);
            return point;
        }
        
        private bool Image_Compare(Image image1, Image image2)
        {
            Bitmap img1 = (Bitmap)image1;
            Bitmap img2 = (Bitmap)image2;

            //高さが違えばfalse
            if (img1.Width != img2.Width || img1.Height != img2.Height) return false;
            //BitmapData取得
            BitmapData bd1 = img1.LockBits(new System.Drawing.Rectangle(0, 0, img1.Width, img1.Height), System.Drawing.Imaging.ImageLockMode.ReadOnly, img1.PixelFormat);
            BitmapData bd2 = img2.LockBits(new System.Drawing.Rectangle(0, 0, img2.Width, img2.Height), System.Drawing.Imaging.ImageLockMode.ReadOnly, img2.PixelFormat);
            //スキャン幅が違う場合はfalse
            if (bd1.Stride != bd2.Stride)
            {
                //ロックを解除
                img1.UnlockBits(bd1);
                img2.UnlockBits(bd2);
                return false;
            }
            int bsize = bd1.Stride * img1.Height;
            byte[] byte1 = new byte[bsize];
            byte[] byte2 = new byte[bsize];
            //バイト配列にコピー
            Marshal.Copy(bd1.Scan0, byte1, 0, bsize);
            Marshal.Copy(bd2.Scan0, byte2, 0, bsize);
            //ロックを解除
            img1.UnlockBits(bd1);
            img2.UnlockBits(bd2);

            //MD5ハッシュを取る
            System.Security.Cryptography.MD5CryptoServiceProvider md5 =
                new System.Security.Cryptography.MD5CryptoServiceProvider();
            byte[] hash1 = md5.ComputeHash(byte1);
            byte[] hash2 = md5.ComputeHash(byte2);
            
            //ハッシュを比較
            return hash1.SequenceEqual(hash2);
        }

        /*
        private void Add_data_Click(object sender, EventArgs e)
        {

            //Process p = Process.Start("EXPLORER.EXE",@"C:\Users\takami\OneDrive\実験関連\数理情報工学\画像認識\photo_test");
            //System.Threading.Thread.Sleep(5000);
           
            ProcessStartInfo startInfo = new ProcessStartInfo("Explorer.exe", @"C:\Users\takami\OneDrive\実験関連\数理情報工学\画像認識\photo_test");
            startInfo.WindowStyle = ProcessWindowStyle.Minimized;
            startInfo.Arguments = "shell64.dll,Control_RunDLL inetcpl.cpl";
            Process p = Process.Start(startInfo);
            p.WaitForExit();
            
            ProcessStartInfo processInfo = new ProcessStartInfo();
            processInfo.FileName = "explorer.exe";
            processInfo.Arguments = @"C:\Users\takami\OneDrive\実験関連\数理情報工学\画像認識\photo_test";
            processInfo.UseShellExecute = true;

            try
            {
                Process p = Process.Start(processInfo);
                p.WaitForExit();
                System.Threading.Thread.Sleep(1000);
            }
            catch (Exception ex)
            {
                writeLog(ex.ToString());
            }
            
            ProcessStartInfo processInfo = new ProcessStartInfo();
            processInfo.FileName = @"C:\Users\takami\Anaconda3\pythonw.exe";
            processInfo.Arguments = @"C:\Users\takami\OneDrive\実験関連\数理情報工学\画像認識\photo_test";
            //processInfo.Verb = "runas";
            processInfo.UseShellExecute = true;
            
            try
            {
                Process p = Process.Start(processInfo);

                writeLog("play");
                p.WaitForExit();
                //System.Threading.Thread.Sleep(1000);
            }
            catch (Exception ex)
            {
                writeLog(ex.ToString());
            }
            
            Process p = new Process();
            p.StartInfo.FileName = @"C:\Users\takami\OneDrive\実験関連\数理情報工学\画像認識\photo_test";
            //p.StartInfo.UseShellExecute = false;
            p.Start();
            //System.Threading.Thread.Sleep(1000);
            
            while (p.MainWindowHandle == IntPtr.Zero && p.HasExited == false)
            {
                System.Threading.Thread.Sleep(1);
                p.Refresh();
            }
            IntPtr hMainWindow = p.MainWindowHandle;
            
            //p.WaitForExit();
            
            while (true)
            {
                
                if (p.HasExited == false)
                {
                    System.Threading.Thread.Sleep(500);
                    //p.WaitForExit(5000);
                }
                else
                {
                    writeLog("break");
                    break;
                }
                
                if (p.CloseMainWindow())
                {
                    writeLog("break");
                    break;
                }
                else
                {
                    writeLog("contine");
                    System.Threading.Thread.Sleep(1000);
                }
            }
            p.Close();
            p.Dispose();
            
            //p.WaitForInputIdle();
            //p.WaitForExit();
            //writeLog("clear");
            
            System.Diagnostics.Process.Start(@"C:\Users\takami\OneDrive\実験関連\数理情報工学\画像認識\photo_test");
            IntPtr hWnd;
            while (true)
            {
                System.Threading.Thread.Sleep(500);

                hWnd = FindForm(null)
            }
            

            
            // Pythonスクリプト実行エンジン
            ScriptEngine engine = Python.CreateEngine();

            // 実行するPythonのソースを指定
            ScriptSource source = engine.CreateScriptSourceFromFile(@"C:\Users\takami\OneDrive\実験関連\数理情報工学\画像認識\fashion_test.py");

            // 実行
            source.Execute();
            
            //---------
            
            var engine = Python.CreateEngine();
            var searchPaths = engine.GetSearchPaths();
            ScriptRuntime py = Python.CreateRuntime();
            searchPaths.Add(@"C:\Users\takami\Anaconda3\Lib");
            py.UseFile(@"C:\Users\takami\OneDrive\実験関連\数理情報工学\画像認識\fashion_test.py");
            
            //------------------
        }
        */

        /*--------------------------*/
        /* パネルに画像を並べる関数 */
        /*--------------------------*/

        private int[] Color_Count()
        {
            int[] color_count = new int[(int)Color.MAX_COLOR];

            for (int index = 0; index < top_image_property.Count(); index++)
            {
                switch (top_image_property[index].color)
                {
                    case (int)Color.Red:
                        color_count[(int)Color.Red] += top_image_property[index].count;
                        break;

                    case (int)Color.Blue:
                        color_count[(int)Color.Blue] += top_image_property[index].count;
                        break;

                    case (int)Color.Green:
                        color_count[(int)Color.Green] += top_image_property[index].count;
                        break;

                    case (int)Color.White:
                        color_count[(int)Color.White] += top_image_property[index].count;
                        break;

                    case (int)Color.Black:
                        color_count[(int)Color.Black] += top_image_property[index].count;
                        break;

                    case (int)Color.Other:
                        color_count[(int)Color.Other] += top_image_property[index].count;
                        break;
                }
            }
            foreach (int a in color_count) writeLog("[" + a + "]");

            /* 1位以外も選ばれるために補正（補正値に関して要検討） */
            int max = 0;
            int max_index = 0;

            for (int i = 0; i < (int)Color.MAX_COLOR; i++)
            {
                if (max < color_count[i])
                {
                    max = color_count[i];
                    max_index = i;
                }
            }
            writeLog("max_index : " + max_index);
            
            for (int i = 0; i < (int)Color.MAX_COLOR; i++)
            {
                if (i != max_index)
                {
                    if (color_count[max_index] - color_count[i] >= 300) color_count[i] += 200;
                    else if (color_count[max_index] - color_count[i] >= 100) color_count[i] += 80;
                    else if (color_count[max_index] - color_count[i] >= 50) color_count[i] += 30;
                }
            }

            return color_count;
        }

        private int Color_Index(int[] color_count)
        {
            int total = 0;
            foreach (int a in color_count) total += a;

            int rand = r.Next(total);

            int temp = 0;
            int index = 0;

            foreach (int b in color_count)
            {
                temp += b;
                if (rand < temp)
                {
                    break;
                    //return index;
                }
                index++;
            }
            return index;
        }

        private void Shirt_Panel_Paint(object sender, PaintEventArgs e)
        {
            int select_color; //どの色
            int select_img;   //どの画像
            int[] color_count = new int[(int)Color.MAX_COLOR]; //各カラーのランキング入りした数
            
            image_property.Clear();
            
            /* 2週目以降、前回のカラーランキングを前もって集計 */
            if(panel_change_count > 0)
            {
                color_count = Color_Count();
                foreach (int a in color_count) writeLog("[[" + a + "]]");
            }

            /* 並べる画像を抽選 */
            for(int i = 0; i < BOX_HEIGHT; i++)
            {
                for(int j = 0; j < BOX_WIDTH; j++)
                {
                    int point = (i * (BOX_WIDTH - 1)) + (i + j);

                    /* 初回のみ */
                    if (panel_change_count == 0)
                    {
                        select_color = r.Next((int)Color.MAX_COLOR);
                        select_img = r.Next(shirt_files[select_color].Length);

                        image_property.Add(new Image_Property()
                        {
                            image = Image.FromFile(shirt_files[select_color][select_img]),
                            category = (int)Category.Shirt,
                            color = select_color,
                            count = 0
                        });
                    }
                    /* 2週目以降 */
                    else
                    {
                        select_color = Color_Index(color_count);
                        select_img = r.Next(shirt_files[select_color].Length);

                        image_property.Add(new Image_Property()
                        {
                            image = Image.FromFile(shirt_files[select_color][select_img]),
                            category = (int)Category.Shirt,
                            color = select_color,
                            count = 0
                        });
                    }
                    e.Graphics.DrawImage(image_property[point].image, j * WIDTH_RATIO, i * HEIGHT_RATIO, WIDTH_RATIO, HEIGHT_RATIO);
                }
            }
        }

        private void Pant_Panel_Paint(object sender, PaintEventArgs e)
        {
            /*
            image_property.Clear();

            for (int i = 0; i < BOX_HEIGHT; i++)
            {
                for (int j = 0; j < BOX_WIDTH; j++)
                {
                    int point = (i * (BOX_WIDTH - 1)) + (i + j);

                    image_property.Add(new Image_Property()
                    {
                        image = Image.FromFile(pant_files[pant_count]),
                        category = (int)Category.Pant,
                        color = COLOR_RED,
                    });

                    pant_count++;
                    if (pant_count >= pant_files.Length)
                    {
                        pant_count = 0;
                    }

                    e.Graphics.DrawImage(image_property[point].image, j * WIDTH_RATIO, i * HEIGHT_RATIO, WIDTH_RATIO, HEIGHT_RATIO);
                }
            }
            */
            int select_color; //どの色
            int select_img; //どの画像

            image_property.Clear();

            for (int i = 0; i < BOX_HEIGHT; i++)
            {
                for (int j = 0; j < BOX_WIDTH; j++)
                {
                    int point = (i * (BOX_WIDTH - 1)) + (i + j);

                    /*初回のみ*/
                    if (top_image_property.Count == 0)
                    {
                        select_color = r.Next((int)Color.MAX_COLOR);
                        select_img = r.Next(pant_files[select_color].Length);
                        //Color color_str = (Color)Enum.ToObject(typeof(Color), select_color);

                        image_property.Add(new Image_Property()
                        {
                            image = Image.FromFile(pant_files[select_color][select_img]),
                            category = (int)Category.Pant,
                            color = select_color
                            //color = color_str.ToString()
                        });
                    }
                    /*2週目以降*/
                    else
                    {
                        select_color = r.Next((int)Color.MAX_COLOR);
                        select_img = r.Next(pant_files[select_color].Length);
                        //Color color_str = (Color)Enum.ToObject(typeof(Color), select_color);

                        image_property.Add(new Image_Property()
                        {
                            image = Image.FromFile(pant_files[select_color][select_img]),
                            category = (int)Category.Pant,
                            color = select_color
                            //color = color_str.ToString()
                        });
                    }

                    e.Graphics.DrawImage(image_property[point].image, j * WIDTH_RATIO, i * HEIGHT_RATIO, WIDTH_RATIO, HEIGHT_RATIO);
                }
            }
        }

        private void Shoes_Panel_Paint(object sender, PaintEventArgs e)
        {
            /*
            image_property.Clear();

            for (int i = 0; i < BOX_HEIGHT; i++)
            {
                for (int j = 0; j < BOX_WIDTH; j++)
                {
                    int point = (i * (BOX_WIDTH - 1)) + (i + j);

                    image_property.Add(new Image_Property()
                    {
                        image = Image.FromFile(shoes_files[shoes_count]),
                        category = (int)Category.Shoes,
                        color = COLOR_RED,
                    });

                    shoes_count++;
                    if (shoes_count >= shoes_files.Length)
                    {
                        shoes_count = 0;
                    }

                    e.Graphics.DrawImage(image_property[point].image, j * WIDTH_RATIO, i * HEIGHT_RATIO, WIDTH_RATIO, HEIGHT_RATIO);
                }
            }
            */
            int select_color; //どの色
            int select_img; //どの画像

            image_property.Clear();

            for (int i = 0; i < BOX_HEIGHT; i++)
            {
                for (int j = 0; j < BOX_WIDTH; j++)
                {
                    int point = (i * (BOX_WIDTH - 1)) + (i + j);

                    /*初回のみ*/
                    if (top_image_property.Count == 0)
                    {
                        select_color = r.Next((int)Color.MAX_COLOR);
                        select_img = r.Next(shoes_files[select_color].Length);
                        //Color color_str = (Color)Enum.ToObject(typeof(Color), select_color);

                        image_property.Add(new Image_Property()
                        {
                            image = Image.FromFile(shoes_files[select_color][select_img]),
                            category = (int)Category.Shoes,
                            color = select_color
                            //color = color_str.ToString()
                        });
                    }
                    /*2週目以降*/
                    else
                    {
                        select_color = r.Next((int)Color.MAX_COLOR);
                        select_img = r.Next(shoes_files[select_color].Length);
                        //Color color_str = (Color)Enum.ToObject(typeof(Color), select_color);

                        image_property.Add(new Image_Property()
                        {
                            image = Image.FromFile(shoes_files[select_color][select_img]),
                            category = (int)Category.Shoes,
                            color = select_color
                            //color = color_str.ToString()
                        });
                    }

                    e.Graphics.DrawImage(image_property[point].image, j * WIDTH_RATIO, i * HEIGHT_RATIO, WIDTH_RATIO, HEIGHT_RATIO);
                }
            }
        }

        private void All_Panel_Paint(object sender, PaintEventArgs e)
        {
            int select_category;
            int select_color;
            int select_img;

            image_property.Clear();

            for (int i = 0; i < BOX_HEIGHT; i++)
            {
                for (int j = 0; j < BOX_WIDTH; j++)
                {
                    int point = (i * (BOX_WIDTH - 1)) + (i + j);

                    select_category = r.Next((int)Category.MAX_CATEGORY);

                    switch (select_category)
                    {
                        case (int)Category.Shirt : //シャツの場合

                            select_color = r.Next((int)Color.MAX_COLOR);
                            select_img = r.Next(shirt_files[select_color].Length);
                            //Color color_str = (Color)Enum.ToObject(typeof(Color), select_color);

                            image_property.Add(new Image_Property()
                            {
                                image = Image.FromFile(shirt_files[select_color][select_img]),
                                category = (int)Category.Shirt,
                                color = select_color
                                //color = color_str.ToString()
                            });
                            //img[i, j] = Image.FromFile(shirt_files[select_img]);
                            break;
                        case (int)Category.Pant : //ズボンの場合

                            select_color = r.Next((int)Color.MAX_COLOR);
                            select_img = r.Next(pant_files[select_color].Length);

                            image_property.Add(new Image_Property()
                            {
                                image = Image.FromFile(pant_files[select_color][select_img]),
                                category = (int)Category.Pant,
                                color = select_color
                                //color = COLOR_RED,
                            });
                            //img[i, j] = Image.FromFile(pant_files[select_img]);
                            break;
                        case (int)Category.Shoes : //シューズの場合

                            select_color = r.Next((int)Color.MAX_COLOR);
                            select_img = r.Next(shoes_files.Length);

                            image_property.Add(new Image_Property()
                            {
                                image = Image.FromFile(shoes_files[select_color][select_img]),
                                category = (int)Category.Shoes,
                                color = select_color
                                //color = COLOR_RED,
                            });
                            //img[i, j] = Image.FromFile(onepiece_files[select_img]);
                            break;
                    }
                    e.Graphics.DrawImage(image_property[point].image, j * WIDTH_RATIO, i * HEIGHT_RATIO, WIDTH_RATIO, HEIGHT_RATIO);
                    //e.Graphics.DrawImage(img[i, j], j * WIDTH_RATIO, i * HEIGHT_RATIO, WIDTH_RATIO, HEIGHT_RATIO);

                }
            }
        }

        /*----------------------------------------------*/
        /* その他、ボタンやゲームをクリックした際の関数 */
        /*----------------------------------------------*/
        private void Add_data_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();

            ofd.InitialDirectory = @"C:\Users\takami\OneDrive\実験関連\数理情報工学\画像認識\photo_test";
            ofd.Filter = "JPGファイル(*.jpg)|*.jpg";
            ofd.FilterIndex = 1;
            ofd.Multiselect = true;

            ofd.Title = "カテゴリ別に分けたい画像ファイルを選択してください";

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                foreach (string fn in ofd.FileNames)
                {
                    File.Copy(fn, @"C:\Users\takami\OneDrive\実験関連\数理情報工学\画像認識\photo_test\" + Path.GetFileName(fn), true);
                    //writeLog("@\"" + fn + "\"");
                    writeLog(Path.GetFileName(fn));
                }

                //Form4 fm4 = new Form4();
                //fm4.ShowDialog();
                //fm4.progress_check();
                
                
                
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
                    if (newCount == max_filecount)
                        label6.Text = "100%";
                    if (newCount >= max_filecount / 10)
                        label6.Text = "90%";
                    if (newCount >= max_filecount / 30)
                        label6.Text = "70%";
                    if (newCount >= max_filecount / 50)
                        label6.Text = "50%";
                    if (newCount >= max_filecount / 70)
                        label6.Text = "30%";
                    if (newCount >= max_filecount / 90)
                        label6.Text = "10%";

                    progressBar1.Value = newCount;
                    //fm4.label1.Refresh();
                    label6.Refresh();
                    progressBar1.Refresh();
                    Application.DoEvents();
                    writeLog("まだ");
                    textBox_Log.Refresh();
                }
                //fm4.Close();
                p.Close();
                p.Dispose();
                
            }
        }

        private void Change_Click(object sender, EventArgs e)
        {
            (Parent as Form1).Text_Ladies();
            (Parent as Form1).Next(typeof(UserControl2));
        }
        
        private void Human_Click(object sender, EventArgs e)
        {
            Form3_1 f3 = new Form3_1();
            f3.ShowDialog();
        }
        
        private void Setting_Click(object sender, EventArgs e)
        {
            Form2.Ratio_Size receiveText = Form2.ShowMiniForm();

            /*値を取得*/
            WIDTH_RATIO = receiveText.width_ratio;
            HEIGHT_RATIO = receiveText.height_ratio;

            /*取得した値をもとに更新*/
            BOX_HEIGHT = MAX_HEIGHT / HEIGHT_RATIO;
            BOX_WIDTH = MAX_WIDTH / WIDTH_RATIO;
            writeLog("" + BOX_WIDTH + "x" + BOX_HEIGHT);
        }

        private void Wally_game_Click(object sender, EventArgs e)
        {
            Process.Start(@"C:\Users\takami\source\repos\worry_game\worry_game\bin\Debug\worry_game.exe");
        }

        private void Face_game_Click(object sender, EventArgs e)
        {
            Process.Start(@"C:\Users\takami\source\repos\faceLike_mens_game\faceLike_mens_game\bin\Debug\faceLike_mens_game.exe");
        }
        
        /*------------*/
        /* ログを出力 */
        /*------------*/
        private void writeLog(string logText)
        {
            //textBox_Log.ResetText();
            textBox_Log.SelectionStart = textBox_Log.Text.Length;
            textBox_Log.SelectionLength = 0;
            textBox_Log.SelectedText = /* "[" + DateTime.Now.ToString() + "]" + */ logText + "\r\n";
        }
    }
}
