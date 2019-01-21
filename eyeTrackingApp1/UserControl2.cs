using System;
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
    public partial class UserControl2 : UserControl
    {
        public UserControl2()
        {
            InitializeComponent();
        }

        private void UserControl2_Load(object sender, EventArgs e)
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

            /*性別変更ボタンに表示する画像*/
            var image = Properties.Resources.mens_icon2;//Image.FromFile(@"C:\Users\takami\OneDrive\実験関連\mens_icon2.png");
            Change.BackgroundImage = image;

            /*---------------------*/
            /*ボタンを成型（丸ver）*/
            /*---------------------*/

            /*ワンピースボタンを丸く成型*/
            Onepiece.SetBounds(Onepiece.Location.X, Onepiece.Top, 301, 301);
            path = new System.Drawing.Drawing2D.GraphicsPath();
            path.AddEllipse(new System.Drawing.Rectangle(10, 10, 280, 280));
            Onepiece.Region = new Region(path);

            /*スカートボタンを丸く成型*/
            Skirt.SetBounds(Skirt.Location.X, Skirt.Top, 301, 301);
            path = new System.Drawing.Drawing2D.GraphicsPath();
            path.AddEllipse(new System.Drawing.Rectangle(10, 10, 280, 280));
            Skirt.Region = new Region(path);

            /*ヒールボタンを丸く成型*/
            Heal.SetBounds(Heal.Location.X, Heal.Top, 301, 301);
            path = new System.Drawing.Drawing2D.GraphicsPath();
            path.AddEllipse(new System.Drawing.Rectangle(10, 10, 280, 280));
            Heal.Region = new Region(path);

            /*全てボタンを丸く成型*/
            All.SetBounds(All.Location.X, All.Top, 301, 301);
            path = new System.Drawing.Drawing2D.GraphicsPath();
            path.AddEllipse(new System.Drawing.Rectangle(10, 10, 280, 280));
            All.Region = new Region(path);

            /*各パネル結果ボタンを丸く成型*/
            Panel_Eye.SetBounds(Panel_Eye.Location.X, Panel_Eye.Top, 301, 301);
            path = new System.Drawing.Drawing2D.GraphicsPath();
            path.AddEllipse(new System.Drawing.Rectangle(10, 10, 280, 280));
            Panel_Eye.Region = new Region(path);

            /*データ追加ボタンを丸く成型*/

            Add_data.SetBounds(Add_data.Location.X, Add_data.Top, 130, 130);
            path = new System.Drawing.Drawing2D.GraphicsPath();
            path.AddEllipse(new System.Drawing.Rectangle(5, 5, 120, 120));//(10, 10, 110, 110));
            Add_data.Region = new Region(path);

            /*好みのデータボタンを丸く成型*/

            Human.SetBounds(Human.Location.X, Human.Top, 130, 130);
            path = new System.Drawing.Drawing2D.GraphicsPath();
            path.AddEllipse(new System.Drawing.Rectangle(5, 5, 120, 120));//(15, 15, 100, 100));
            Human.Region = new Region(path);

            /*設定ボタンを丸く成型*/

            Setting.SetBounds(Setting.Location.X, Setting.Top, 130, 130);
            path = new System.Drawing.Drawing2D.GraphicsPath();
            path.AddEllipse(new System.Drawing.Rectangle(5, 5, 120, 120));
            Setting.Region = new Region(path);

            /* 通知アイコンを丸く成型 */
            Notice1.SetBounds(Notice1.Location.X, Notice1.Top, 70, 70);
            path = new System.Drawing.Drawing2D.GraphicsPath();
            path.AddEllipse(new System.Drawing.Rectangle(4, 4, 61, 61));
            Notice1.Region = new Region(path);

            Notice2.SetBounds(Notice2.Location.X, Notice2.Top, 70, 70);
            path = new System.Drawing.Drawing2D.GraphicsPath();
            path.AddEllipse(new System.Drawing.Rectangle(4, 4, 61, 61));
            Notice2.Region = new Region(path);

        }

        Random r = new Random();

        /*見られた場所、時間を管理する構造体*/
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

        /*見られた画像、割合を管理する構造体*/
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

        /*一つのパネルに表示される画像一枚ずつのプロパティを管理する構造体*/
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
        public enum Category
        {
            Onepiece,
            Skirt,
            Heal,

            MAX_CATEGORY
        }

        public enum Color
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
        static string[] CATEGORY_NAME = new string[(int)Category.MAX_CATEGORY] { "ワンピース", "スカート", "ヒール" };
        static string[] COLOR_NAME = new string[(int)Color.MAX_COLOR] { "赤", "青", "緑", "白", "黒", "その他" };


        /*ウィンドウサイズ*/
        static int MAX_WIDTH = 1920;
        static int MAX_HEIGHT = 1080;

        /*分割比*/
        static int WIDTH_RATIO = 240;
        static int HEIGHT_RATIO = 270;

        /*一つのボックスサイズ*/
        static int BOX_HEIGHT = MAX_HEIGHT / HEIGHT_RATIO; //4
        static int BOX_WIDTH = MAX_WIDTH / WIDTH_RATIO;   //6

        /* 測定に使用するカラーの有無 */
        public static bool[] color_flag = new bool[(int)Color.MAX_COLOR] { true, true, true, true, true, true };
        List<int> color_index = new List<int>();

        /* 各カテゴリーの画像ファイル名配列 */
        string[][] onepiece_files =
        {
            Directory.GetFiles(Path.Combine(MAIN_PATH,CATEGORY_NAME[(int)Category.Onepiece],COLOR_NAME[(int)Color.Red]), "*.jpg", System.IO.SearchOption.AllDirectories),
            Directory.GetFiles(Path.Combine(MAIN_PATH,CATEGORY_NAME[(int)Category.Onepiece],COLOR_NAME[(int)Color.Blue]), "*.jpg", System.IO.SearchOption.AllDirectories),
            Directory.GetFiles(Path.Combine(MAIN_PATH,CATEGORY_NAME[(int)Category.Onepiece],COLOR_NAME[(int)Color.Green]), "*.jpg", System.IO.SearchOption.AllDirectories),
            Directory.GetFiles(Path.Combine(MAIN_PATH,CATEGORY_NAME[(int)Category.Onepiece],COLOR_NAME[(int)Color.White]), "*.jpg", System.IO.SearchOption.AllDirectories),
            Directory.GetFiles(Path.Combine(MAIN_PATH,CATEGORY_NAME[(int)Category.Onepiece],COLOR_NAME[(int)Color.Black]), "*.jpg", System.IO.SearchOption.AllDirectories),
            Directory.GetFiles(Path.Combine(MAIN_PATH,CATEGORY_NAME[(int)Category.Onepiece],COLOR_NAME[(int)Color.Other]), "*.jpg", System.IO.SearchOption.AllDirectories)
        };

        string[][] skirt_files =
        {
            Directory.GetFiles(Path.Combine(MAIN_PATH,CATEGORY_NAME[(int)Category.Skirt],COLOR_NAME[(int)Color.Red]), "*.jpg", System.IO.SearchOption.AllDirectories),
            Directory.GetFiles(Path.Combine(MAIN_PATH,CATEGORY_NAME[(int)Category.Skirt],COLOR_NAME[(int)Color.Blue]), "*.jpg", System.IO.SearchOption.AllDirectories),
            Directory.GetFiles(Path.Combine(MAIN_PATH,CATEGORY_NAME[(int)Category.Skirt],COLOR_NAME[(int)Color.Green]), "*.jpg", System.IO.SearchOption.AllDirectories),
            Directory.GetFiles(Path.Combine(MAIN_PATH,CATEGORY_NAME[(int)Category.Skirt],COLOR_NAME[(int)Color.White]), "*.jpg", System.IO.SearchOption.AllDirectories),
            Directory.GetFiles(Path.Combine(MAIN_PATH,CATEGORY_NAME[(int)Category.Skirt],COLOR_NAME[(int)Color.Black]), "*.jpg", System.IO.SearchOption.AllDirectories),
            Directory.GetFiles(Path.Combine(MAIN_PATH,CATEGORY_NAME[(int)Category.Skirt],COLOR_NAME[(int)Color.Other]), "*.jpg", System.IO.SearchOption.AllDirectories)
        };
        string[][] heal_files =
        {
            Directory.GetFiles(Path.Combine(MAIN_PATH,CATEGORY_NAME[(int)Category.Heal],COLOR_NAME[(int)Color.Red]), "*.jpg", System.IO.SearchOption.AllDirectories),
            Directory.GetFiles(Path.Combine(MAIN_PATH,CATEGORY_NAME[(int)Category.Heal],COLOR_NAME[(int)Color.Blue]), "*.jpg", System.IO.SearchOption.AllDirectories),
            Directory.GetFiles(Path.Combine(MAIN_PATH,CATEGORY_NAME[(int)Category.Heal],COLOR_NAME[(int)Color.Green]), "*.jpg", System.IO.SearchOption.AllDirectories),
            Directory.GetFiles(Path.Combine(MAIN_PATH,CATEGORY_NAME[(int)Category.Heal],COLOR_NAME[(int)Color.White]), "*.jpg", System.IO.SearchOption.AllDirectories),
            Directory.GetFiles(Path.Combine(MAIN_PATH,CATEGORY_NAME[(int)Category.Heal],COLOR_NAME[(int)Color.Black]), "*.jpg", System.IO.SearchOption.AllDirectories),
            Directory.GetFiles(Path.Combine(MAIN_PATH,CATEGORY_NAME[(int)Category.Heal],COLOR_NAME[(int)Color.Other]), "*.jpg", System.IO.SearchOption.AllDirectories)
        };

        /*パネルに表示される画像一枚に対する詳細データリスト*/
        List<Image_Property> image_property = new List<Image_Property>();

        /*パネル一面での上位三枚の詳細データ*/
        List<Image_Property> top_image_property = new List<Image_Property>();

        /* 画面が更新された回数と更新終了フラグ */
        int panel_change_count = 0;
        bool change_finish_flag = false;

        /*各カテゴリでの割合管理変数*/
        public static List<Image_Proportion> image_proportion_onepiece = new List<Image_Proportion>();
        public static List<Image_Proportion> image_proportion_skirt = new List<Image_Proportion>();
        public static List<Image_Proportion> image_proportion_heal = new List<Image_Proportion>();
        public static List<Image_Proportion> image_proportion_all = new List<Image_Proportion>();
        static double change_onepiece_image_count = 0;
        static double change_skirt_image_count = 0;
        static double change_heal_image_count = 0;
        static double change_image_count = 0;

        /* 各パネルでの上位3位を記録 */
        public static List<List<List<Image>>> panel_top_image = new List<List<List<Image>>>();
        static int tracking_count = 0;

        /*--------------*/
        /* 測定前の処理 */
        /*--------------*/
        private void Before_Process()
        {
            Cursor.Hide();
            textBox_Log.ResetText();

            Notice1.Visible = true;
            Notice2.Visible = true;

            panel_change_count = 0;
            change_finish_flag = false;
            panel_top_image.Add(new List<List<Image>>());

            for (int i = 0; i < (int)Color.MAX_COLOR; i++)
            {
                color_index.Add(i);
                if (!color_flag[i]) color_index.Remove(i);
            }
        }

        private Panel Set_Panel(int category)
        {
            /*コントロ－ル（パネル）を生成*/
            Panel panel = new Panel();
            panel.Location = new Point(0, 0);
            panel.Size = new System.Drawing.Size(MAX_WIDTH, MAX_HEIGHT);
            switch (category)
            {
                case (int)Category.Onepiece:
                    panel.Paint += new PaintEventHandler(this.Onepiece_Panel_Paint);
                    break;

                case (int)Category.Skirt:
                    panel.Paint += new PaintEventHandler(this.Skirt_Panel_Paint);
                    break;

                case (int)Category.Heal:
                    panel.Paint += new PaintEventHandler(this.Heal_Panel_Paint);
                    break;

                case (int)Category.MAX_CATEGORY:
                    panel.Paint += new PaintEventHandler(this.All_Panel_Paint);
                    break;
            }
            Controls.Add(panel);
            panel.Focus();
            panel.BringToFront();

            return panel;
        }

        private void Onepiece_Click(object sender, EventArgs e)
        {
            /*
            Cursor.Hide();
            textBox_Log.ResetText();
            */
            /*コントロ－ル（パネル）を生成*/
            /*
            Panel panel = new Panel();
            panel.Location = new Point(0, 0);
            panel.Size = new System.Drawing.Size(MAX_WIDTH, MAX_HEIGHT);
            panel.Paint += new PaintEventHandler(this.Onepiece_Panel_Paint);
            Controls.Add(panel);
            panel.BringToFront();
            */

            Before_Process();
            Panel panel = Set_Panel((int)Category.Onepiece);

            //for (int i = 0; i < 3; i++)
            while(!change_finish_flag)
            {
                writeLog("       [" + panel_change_count + "]回目");
                panel.Refresh();
                Eye_Tracking(image_proportion_onepiece, image_proportion_all, panel);
                panel_change_count++;
            }
            Controls.Remove(panel);
            After_Process();
        }

        private void Skirt_Click(object sender, EventArgs e)
        {
            Before_Process();
            Panel panel = Set_Panel((int)Category.Skirt);

            //for (int i = 0; i < 3; i++)
            while(!change_finish_flag)
            {
                writeLog("       [" + panel_change_count + "]回目");
                panel.Refresh();
                Eye_Tracking(image_proportion_skirt, image_proportion_all, panel);
                panel_change_count++;
            }

            Controls.Remove(panel);
            After_Process();
        }

        private void Heal_Click(object sender, EventArgs e)
        {
            Before_Process();
            Panel panel = Set_Panel((int)Category.Heal);

            //for (int i = 0; i < 3; i++)
            while(!change_finish_flag)
            {
                writeLog("       [" + panel_change_count + "]回目");
                panel.Refresh();
                Eye_Tracking(image_proportion_heal, image_proportion_all, panel);
                panel_change_count++;
            }

            Controls.Remove(panel);
            After_Process();
        }

        private void All_Click(object sender, EventArgs e)
        {
            Before_Process();
            Panel panel = Set_Panel((int)Category.MAX_CATEGORY);

            //for (int i = 0; i < 3; i++)
            while(!change_finish_flag)
            {
                writeLog("       [" + panel_change_count + "]回目");
                panel.Refresh();
                Eye_Tracking(image_proportion_all, image_proportion_all, panel);
                panel_change_count++;
            }

            Controls.Remove(panel);
            After_Process();
        }

        /*--------------*/
        /* 測定後の処理 */
        /*--------------*/
        private void After_Process()
        {
            top_image_property.Clear();
            color_index.Clear();
            //panel_change_count = 0;
            //change_finish_flag = false;
            tracking_count++;

            Cursor.Show();
        }

        /*-------------------------------------------------*/
        /* 視線の測定、ランキングと割合を取得（メイン関数）*/
        /*-------------------------------------------------*/
        private void Eye_Tracking(List<Image_Proportion> proportion, List<Image_Proportion> all_proportion, Panel panel)
        {
            /* アイトラッカー関連 */
            var host = new Host();
            var gazePointDataStream = host.Streams.CreateGazePointDataStream();
            int point;
            Gaze[] gaze = new Gaze[BOX_HEIGHT * BOX_WIDTH];

            /* どのカテゴリの測定がされているかフラグを立てておく */
            int category_flag = 0;
            bool same = false;

            if (proportion == image_proportion_onepiece)
            {
                category_flag = (int)Category.Onepiece;
            }
            else if (proportion == image_proportion_skirt)
            {
                category_flag = (int)Category.Skirt;
            }
            else if (proportion == image_proportion_heal)
            {
                category_flag = (int)Category.Heal;
            }
            else if (proportion == all_proportion)
            {
                category_flag = (int)Category.MAX_CATEGORY;
                same = true;
            }

            top_image_property.Clear();

            for (int i = 0; i < BOX_HEIGHT * BOX_WIDTH; i++)
            {
                gaze[i].y = 0;
                gaze[i].x = 0;
                gaze[i].count = 0;
            }

            System.Threading.Thread.Sleep(5000); //測定無しの状態で画像を見せるため

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

            System.Threading.Thread.Sleep(15000);

            host.DisableConnection();

            Array.Sort(gaze); //countに関してソートされる

            /*各カテゴリ単位で見られた時間の総和を求める(割合算出に使用)*/
            for (int a = 0; a < BOX_HEIGHT * BOX_WIDTH; a++)
            {
                if (gaze[a].count != 0)
                {
                    switch (category_flag)
                    {
                        case (int)Category.Onepiece:
                            change_onepiece_image_count += gaze[a].count;
                            break;

                        case (int)Category.Skirt:
                            change_skirt_image_count += gaze[a].count;
                            break;

                        case (int)Category.Heal:
                            change_heal_image_count += gaze[a].count;
                            break;
                    }
                    change_image_count += gaze[a].count;
                }
            }

            Data_Registration(gaze, proportion, category_flag);
            if (!same) Data_Registration(gaze, all_proportion, (int)Category.MAX_CATEGORY);

            /*
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox2.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox3.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox1.Image = image_property[Get_Point(gaze, 0)].image;  //最も見られた画像を出力
            pictureBox2.Image = image_property[Get_Point(gaze, 1)].image;  //2位
            pictureBox3.Image = image_property[Get_Point(gaze, 2)].image;  //3位
            writeLog("カテゴリ : " + image_property[Get_Point(gaze, 0)].category);
            writeLog("カラー : " + image_property[Get_Point(gaze, 0)].color);
            */
            /*トップ3位を登録*/
            /*
            top_image_property.Add(image_property[0]);
            top_image_property.Add(image_property[1]);
            top_image_property.Add(image_property[2]);
            */
            /*トップ10位を登録*/
            for (int num = 0; num < 10; num++)
            {
                if (num < BOX_HEIGHT * BOX_WIDTH)
                {
                    Image_Property tmpDate = image_property[Get_Point(gaze, num)];
                    tmpDate.count = gaze[num].count;
                    image_property[Get_Point(gaze, num)] = tmpDate;
                    top_image_property.Add(image_property[Get_Point(gaze, num)]);
                    //writeLog("color : " + top_image_property[num].color + " [" + top_image_property[num].count + "]");
                }
            }

            /* 各パネルでの上位3枚を保存 */
            panel_top_image[tracking_count].Add(new List<Image>());
            for (int num = 0; num < 3; num++)
            {
                panel_top_image[tracking_count][panel_change_count].Add(top_image_property[num].image);
            }

            /*三週目以降、終了条件のチェック*/
            if (panel_change_count >= 3) Finish_Gudge();
        }

        private void Data_Registration(Gaze[] gaze, List<Image_Proportion> image_proportion, int category_flag)
        {
            double count = 0;

            switch (category_flag)
            {
                case (int)Category.Onepiece:
                    count = change_onepiece_image_count;
                    break;

                case (int)Category.Skirt:
                    count = change_skirt_image_count;
                    break;

                case (int)Category.Heal:
                    count = change_heal_image_count;
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
            int[] category_count = new int[(int)Category.MAX_CATEGORY];
            int[] color_count = new int[(int)Color.MAX_COLOR]; //各カラーのランキング入りした数

            writeLog("   [gudge]");
            category_count = Category_Count();
            color_count = Color_Count();

            int[] temp_category = new int[(int)Category.MAX_CATEGORY];
            category_count.CopyTo(temp_category, 0);
            Array.Sort(temp_category);
            Array.Reverse(temp_category);

            int[] temp_color = new int[(int)Color.MAX_COLOR];
            color_count.CopyTo(temp_color, 0);
            Array.Sort(temp_color);
            Array.Reverse(temp_color);

            //foreach (int x in temp) writeLog("[" + x + "]");
            writeLog("final-gudge:" + (temp_color[0] - temp_color[1]));

            if ((temp_category[0] - temp_category[1] >= 100) && (temp_color[0] - temp_color[1] >= 200))
            {
                change_finish_flag = true;
            }
        }
        /*
        void Data_Registration(Gaze[] gaze, List<Image_Proportion> image_proportion)
        {
            for (int a = 0; a < BOX_HEIGHT * BOX_WIDTH; a++)
            {
                if (gaze[a].count != 0)
                {
                    bool check = false;

                    for (int b = 0; b < image_proportion.Count; b++)
                    {
                        Image_Proportion tmpData = image_proportion[b];

                        //もし過去に同じ画像が登録されていたら、「カウント」と「割合」を更新
                        if (Image_Compare(image_property[Get_Point(gaze, a)].image, tmpData.image))
                        {
                            //System.Threading.Thread.Sleep(100);
                            tmpData.count += gaze[a].count;
                            tmpData.proportion = (tmpData.count / change_image_count) * 100;
                            image_proportion[b] = tmpData;
                            check = true;
                        }

                        //増加するchange_image_countに合わせて「割合」のみ更新
                        else
                        {
                            tmpData.proportion = (tmpData.count / change_image_count) * 100;
                            image_proportion[b] = tmpData;
                        }
                    }

                    //もし初めて見る画像だったら、「画像」「カウント」「割合」を登録
                    if (!check)
                    {
                        image_proportion.Add(new Image_Proportion()
                        {
                            image = image_property[Get_Point(gaze, a)].image,
                            count = gaze[a].count,
                            proportion = (gaze[a].count / change_image_count) * 100,
                        });
                    }
                    
                }
            }

            image_proportion.Sort();
            
            string s2 = "change_image_count : " + change_image_count;
            writeLog(s2);
            string s3 = "image_proportion.Count : " + image_proportion.Count;
            writeLog(s3);
        }
        */
        int Get_Point(Gaze[] gaze, int set)
        {
            int point = (gaze[set].y * (BOX_WIDTH - 1)) + (gaze[set].y + gaze[set].x);
            return point;
        }

        bool Image_Compare(Image image1, Image image2)
        {
            /*
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
            */
            Bitmap img1 = (Bitmap)image1;
            Bitmap img2 = (Bitmap)image2;

            //高さが違えばfalse
            if (img1.Width != img2.Width || img1.Height != img2.Height) return false;

            try
            {
                //BitmapData取得
                BitmapData bd1 = img1.LockBits(new System.Drawing.Rectangle(0, 0, img1.Width, img1.Height), System.Drawing.Imaging.ImageLockMode.ReadOnly, img1.PixelFormat);
                BitmapData bd2 = img2.LockBits(new System.Drawing.Rectangle(0, 0, img2.Width, img2.Height), System.Drawing.Imaging.ImageLockMode.ReadOnly, img2.PixelFormat);

                try
                {
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
                    //img1.UnlockBits(bd1);
                    //img2.UnlockBits(bd2);

                    //MD5ハッシュを取る
                    System.Security.Cryptography.MD5CryptoServiceProvider md5 =
                        new System.Security.Cryptography.MD5CryptoServiceProvider();
                    byte[] hash1 = md5.ComputeHash(byte1);
                    byte[] hash2 = md5.ComputeHash(byte2);

                    //ハッシュを比較
                    return hash1.SequenceEqual(hash2);
                }
                catch
                {
                    img1.UnlockBits(bd1);
                    img2.UnlockBits(bd2);
                    throw;
                }
                finally
                {
                    //writeLog("通過1");
                    img1.UnlockBits(bd1);
                    img2.UnlockBits(bd2);
                }
            }
            catch
            {
                writeLog("通過2");
                img1.Dispose();
                img2.Dispose();
                throw;
            }
            img1.Dispose();
            img2.Dispose();
            return false;
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

        /*----------------------*/
        /* パネルに画像を並べる */
        /*----------------------*/
        private int[] Category_Count()
        {
            int[] category_count = new int[(int)Category.MAX_CATEGORY];

            for (int index = 0; index < top_image_property.Count(); index++)
            {
                switch (top_image_property[index].category)
                {
                    case (int)Category.Onepiece:
                        category_count[(int)Category.Onepiece] += top_image_property[index].count;
                        break;

                    case (int)Category.Skirt:
                        category_count[(int)Category.Skirt] += top_image_property[index].count;
                        break;

                    case (int)Category.Heal:
                        category_count[(int)Category.Heal] += top_image_property[index].count;
                        break;
                }
            }
            //デバッグ用
            double d_total = 0;
            foreach (int a in category_count)
            {
                d_total += a;
            }
            foreach (int a in category_count)
            {
                string text = $"{(a / d_total) * 100:F2}";
                writeLog("c-[" + a + "] " + text + "%");
            }

            /* 1位以外も選ばれるために補正（補正値に関して要検討） */
            int max = 0;
            int max_index = 0;

            for (int i = 0; i < (int)Category.MAX_CATEGORY; i++)
            {
                if (max < category_count[i])
                {
                    max = category_count[i];
                    max_index = i;
                }
            }
            //writeLog("max_index : " + max_index);

            /* 1位と2位の差に応じて補正値を設定 */
            int[] temp = new int[(int)Category.MAX_CATEGORY];
            category_count.CopyTo(temp, 0);
            Array.Sort(temp);
            Array.Reverse(temp);

            int diff = 0;
            diff = (temp[0] - temp[1]);
            //foreach (int a in temp) writeLog("{" + a + "}");
            writeLog("c-diff :" + diff);

            for (int i = 0; i < (int)Category.MAX_CATEGORY; i++)
            {
                if (i != max_index)
                {
                    //int diff = color_count[max_index] - color_count[i];

                    if (diff >= 300) category_count[i] += 90;//120;//70;//140;         ~300
                    else if (diff >= 200) category_count[i] += 60;//80;//50;//120; 299~200
                    else if (diff >= 100) category_count[i] += 30;//50;//30;//80;  199~100
                    else if (diff >= 50) category_count[i] += 10;//30;//10;//30;    99~50
                }
            }

            //デバッグ用
            d_total = 0;
            foreach (int a in category_count)
            {
                d_total += a;
            }
            foreach (int a in category_count)
            {
                string text = $"{(a / d_total) * 100:F2}";
                writeLog("c-[[" + a + "]] " + text + "%");
            }

            return category_count;
        }

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
            //デバッグ用
            double d_total = 0;
            foreach (int a in color_count)
            {
                d_total += a;
            }
            foreach (int a in color_count)
            {
                string text = $"{(a / d_total) * 100:F2}";
                writeLog("[" + a + "] " + text + "%");
            }

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
            //writeLog("max_index : " + max_index);

            /* 1位と2位の差に応じて補正値を設定 */
            int[] temp = new int[(int)Color.MAX_COLOR];
            color_count.CopyTo(temp, 0);
            Array.Sort(temp);
            Array.Reverse(temp);
            int diff = 0;
            diff = (temp[0] - temp[1]);
            //foreach (int a in temp) writeLog("{" + a + "}");
            writeLog("diff :" + diff);

            for (int i = 0; i < (int)Color.MAX_COLOR; i++)
            {
                if (i != max_index && color_flag[i])
                {
                    //int diff = color_count[max_index] - color_count[i];

                    if (diff >= 300) color_count[i] += 90;//120;//70;//140;         ~300
                    else if (diff >= 200) color_count[i] += 60;//80;//50;//120; 299~200
                    else if (diff >= 100) color_count[i] += 30;//50;//30;//80;  199~100
                    else if (diff >= 50) color_count[i] += 10;//30;//10;//30;    99~50
                }
            }

            //デバッグ用
            d_total = 0;
            foreach (int a in color_count)
            {
                d_total += a;
            }
            foreach (int a in color_count)
            {
                string text = $"{(a / d_total) * 100:F2}";
                writeLog("[[" + a + "]] " + text + "%");
            }
            return color_count;
        }

        private int Select_Index(int[] count_origin)
        {
            int total = 0;
            foreach (int a in count_origin) total += a;

            int rand = r.Next(total);

            int temp = 0;
            int index = 0;

            foreach (int b in count_origin)
            {
                temp += b;
                if (rand < temp)
                {
                    break;
                }
                index++;
            }
            return index;
        }

        private void Panel_Paint(string[][] files, int select_category, PaintEventArgs e)
        {
            int select_color; //どの色
            int select_img;   //どの画像
            int[] color_count = new int[(int)Color.MAX_COLOR]; //各カラーのランキング入りした数

            image_property.Clear();
            writeLog("image_propertyリセット");

            /* 2週目以降、前回のカラーランキングを前もって集計 */
            if (panel_change_count > 0)
            {
                color_count = Color_Count();
                //foreach (int a in color_count) writeLog("[[" + a + "]]");
            }

            /* 並べる画像を抽選 */
            for (int i = 0; i < BOX_HEIGHT; i++)
            {
                for (int j = 0; j < BOX_WIDTH; j++)
                {
                    int point = (i * (BOX_WIDTH - 1)) + (i + j);

                    /* 初回のみ */
                    if (panel_change_count == 0)
                    {
                        //select_color = r.Next((int)Color.MAX_COLOR);
                        select_color = color_index[r.Next(color_index.Count)];
                        select_img = r.Next(files[select_color].Length);

                        image_property.Add(new Image_Property()
                        {
                            image = Image.FromFile(files[select_color][select_img]),
                            category = select_category,
                            color = select_color,
                            count = 0
                        });
                    }
                    /* 2週目以降 */
                    else
                    {
                        select_color = Select_Index(color_count);
                        select_img = r.Next(files[select_color].Length);

                        image_property.Add(new Image_Property()
                        {
                            image = Image.FromFile(files[select_color][select_img]),
                            category = select_category,
                            color = select_color,
                            count = 0
                        });
                    }
                    e.Graphics.DrawImage(image_property[point].image, j * WIDTH_RATIO, i * HEIGHT_RATIO, WIDTH_RATIO, HEIGHT_RATIO);
                }
            }
        }

        void Onepiece_Panel_Paint(object sender, PaintEventArgs e)
        {
            Panel_Paint(onepiece_files, (int)Category.Onepiece, e);
        }

        private void Skirt_Panel_Paint(object sender, PaintEventArgs e)
        {
            Panel_Paint(skirt_files, (int)Category.Skirt, e);
        }

        private void Heal_Panel_Paint(object sender, PaintEventArgs e)
        {
            Panel_Paint(heal_files, (int)Category.Heal, e);
        }

        private void All_Panel_Paint(object sender, PaintEventArgs e)
        {
            int select_category;
            int select_color;
            int select_img;
            int[] category_count = new int[(int)Category.MAX_CATEGORY];
            int[] color_count = new int[(int)Color.MAX_COLOR]; //各カラーのランキング入りした数

            image_property.Clear();

            /* 2週目以降、前回のカラーランキングを前もって集計 */
            if (panel_change_count > 0)
            {
                category_count = Category_Count();
                color_count = Color_Count();
            }

            for (int i = 0; i < BOX_HEIGHT; i++)
            {
                for (int j = 0; j < BOX_WIDTH; j++)
                {
                    int point = (i * (BOX_WIDTH - 1)) + (i + j);

                    /* 初回のみ */
                    if (panel_change_count == 0)
                    {
                        select_category = r.Next((int)Category.MAX_CATEGORY);

                        switch (select_category)
                        {
                            case (int)Category.Onepiece: //ワンピースの場合

                                //select_color = r.Next((int)Color.MAX_COLOR);
                                select_color = color_index[r.Next(color_index.Count)];
                                select_img = r.Next(onepiece_files[select_color].Length);

                                image_property.Add(new Image_Property()
                                {
                                    image = Image.FromFile(onepiece_files[select_color][select_img]),
                                    category = (int)Category.Onepiece,
                                    color = select_color
                                });
                                break;

                            case (int)Category.Skirt: //スカートの場合

                                //select_color = r.Next((int)Color.MAX_COLOR);
                                select_color = color_index[r.Next(color_index.Count)];
                                select_img = r.Next(skirt_files[select_color].Length);

                                image_property.Add(new Image_Property()
                                {
                                    image = Image.FromFile(skirt_files[select_color][select_img]),
                                    category = (int)Category.Skirt,
                                    color = select_color
                                });
                                break;

                            case (int)Category.Heal: //ヒールの場合

                                //select_color = r.Next((int)Color.MAX_COLOR);
                                select_color = color_index[r.Next(color_index.Count)];
                                select_img = r.Next(heal_files.Length);

                                image_property.Add(new Image_Property()
                                {
                                    image = Image.FromFile(heal_files[select_color][select_img]),
                                    category = (int)Category.Heal,
                                    color = select_color
                                });
                                break;
                        }
                    }
                    /* 2週目以降 */
                    else
                    {
                        select_category = Select_Index(category_count);

                        switch (select_category)
                        {
                            case (int)Category.Onepiece: //ワンピースの場合

                                select_color = Select_Index(color_count);
                                select_img = r.Next(onepiece_files[select_color].Length);

                                image_property.Add(new Image_Property()
                                {
                                    image = Image.FromFile(onepiece_files[select_color][select_img]),
                                    category = (int)Category.Onepiece,
                                    color = select_color
                                });
                                break;

                            case (int)Category.Skirt: //スカートの場合

                                select_color = Select_Index(color_count);
                                select_img = r.Next(skirt_files[select_color].Length);

                                image_property.Add(new Image_Property()
                                {
                                    image = Image.FromFile(skirt_files[select_color][select_img]),
                                    category = (int)Category.Skirt,
                                    color = select_color
                                });
                                break;

                            case (int)Category.Heal: //ヒールの場合

                                select_color = Select_Index(color_count);
                                select_img = r.Next(heal_files.Length);

                                image_property.Add(new Image_Property()
                                {
                                    image = Image.FromFile(heal_files[select_color][select_img]),
                                    category = (int)Category.Heal,
                                    color = select_color
                                });
                                break;
                        }
                    }
                    e.Graphics.DrawImage(image_property[point].image, j * WIDTH_RATIO, i * HEIGHT_RATIO, WIDTH_RATIO, HEIGHT_RATIO);
                }
            }
        }

        /*----------------------------*/
        /*メンズとレディースを変更する*/
        /*----------------------------*/
        private void Panel_Eye_Click(object sender, EventArgs e)
        {
            Form_PanelTopImg_Womans fpanel = new Form_PanelTopImg_Womans();
            Notice1.Visible = false;
            fpanel.ShowDialog();
        }

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
                Process p = Process.Start(@"C:\Users\takami\OneDrive\実験関連\数理情報工学\画像認識\dist\fashion_test\fashion_test.exe");
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
            (Parent as Form1).Text_Mens();
            (Parent as Form1).Next(typeof(UserControl1));
        }
        
        private void Human_Click(object sender, EventArgs e)
        {
            Form3_2 f3 = new Form3_2();
            f3.ShowDialog();
        }

        private void Setting_Click(object sender, EventArgs e)
        {
            Form2_2.Ratio_Size receiveText = Form2_2.ShowMiniForm();

            /*値を取得*/
            WIDTH_RATIO = receiveText.width_ratio;
            HEIGHT_RATIO = receiveText.height_ratio;

            /*取得した値をもとに更新*/
            BOX_HEIGHT = MAX_HEIGHT / HEIGHT_RATIO;
            BOX_WIDTH = MAX_WIDTH / WIDTH_RATIO;
            //img = new Image[BOX_HEIGHT, BOX_WIDTH];
            writeLog("" + BOX_WIDTH + "x" + BOX_HEIGHT);

            color_flag = Form2_2.color_flag;
            for (int i = 0; i < (int)Color.MAX_COLOR; i++)
            {
                writeLog("" + color_flag[i]);

            }
        }

        private void Wally_game_Click(object sender, EventArgs e)
        {
            Process.Start(@"C:\Users\takami\source\repos\worry_game\worry_game\bin\Debug\worry_game.exe");

        }

        private void Face_game_Click(object sender, EventArgs e)
        {
            Process.Start(@"C:\Users\takami\source\repos\faceLike_mens_game\faceLike_mens_game\bin\Debug\faceLike_mens_game.exe");

        }

        private void Data_Delete_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("データを削除しますか?", "確認", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button2);
            if (result == DialogResult.OK)
            {
                //writeLog("データを削除");
                image_proportion_onepiece.Clear();
                image_proportion_skirt.Clear();
                image_proportion_heal.Clear();
                image_proportion_all.Clear();
                panel_top_image.Clear();
                tracking_count = 0;

                Notice1.Visible = false;
                Notice2.Visible = false;

            }
            else if (result == DialogResult.Cancel)
            {
                //writeLog("キャンセル");
            }
        }

        /*------------*/
        /* ログを出力 */
        /*------------*/
        private void writeLog(string logText)
        {
            textBox_Log.SelectionStart = textBox_Log.Text.Length;
            textBox_Log.SelectionLength = 0;
            textBox_Log.SelectedText = /* "[" + DateTime.Now.ToString() + "]" + */ logText + "\r\n";
        }
        
    }
}
