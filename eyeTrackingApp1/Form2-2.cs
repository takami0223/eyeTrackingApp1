using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace eyeTrackingApp1
{
    public partial class Form2_2 : Form
    {
        /*--------------------------------*/
        /*コンボボックスのデータ関連クラス*/
        /*--------------------------------*/

        public static bool[] color_flag = new bool[(int)(UserControl2.Color.MAX_COLOR)] { true, true, true, true, true, true };
        static int past_index = 1; //前の選択を記憶

        public class CmbObject
        {
            public int width_ratio { get; set; }
            public int height_ratio { get; set; }
            public int image_count { get; set; }
            public string Value { get; set; }

            /*コンストラクタ*/
            public CmbObject(int width_ratio, int height_ratio, int image_count, string Value)
            {
                this.width_ratio = width_ratio;
                this.height_ratio = height_ratio;
                this.image_count = image_count;
                this.Value = Value;
            }
        }

        public Form2_2()
        {
            InitializeComponent();
            Opacity = 0;

            /*データを追加(横1920x縦1080)*/
            List<CmbObject> src = new List<CmbObject>();
            //src.Add(new CmbObject(320, 270, 24, "320x270")); //6x4y
            src.Add(new CmbObject(128, 135, 120, "128x135")); //15x8t*
            src.Add(new CmbObject(240, 270, 32, "240x270")); //8x4t**
            src.Add(new CmbObject(320, 360, 18, "320x360")); //6x3t*
            src.Add(new CmbObject(480, 540, 8, "480x540")); //4x2t*
            
            comboBox1.DataSource = src;
            comboBox1.DisplayMember = "Value";
            comboBox1.SelectedIndex = past_index; //再描画の際、前に選択したものを選択
                                                  //comboBox1.ValueMember = "";

            checkBox1.Checked = color_flag[0];
            checkBox2.Checked = color_flag[1];
            checkBox3.Checked = color_flag[2];
            checkBox4.Checked = color_flag[3];
            checkBox5.Checked = color_flag[4];
            checkBox6.Checked = color_flag[5];


        }

        private void Form2_Load(object sender, EventArgs e)
        {
            Animator.Animate(150, (frame, frequency) =>
            {
                if (!Visible || IsDisposed) return false;
                Opacity = (double)frame / frequency;
                return true;
            });
        }
        
        /*----------------------------------------------*/
        /*メインフォームにコンボボックスの値を返す構造体*/
        /*----------------------------------------------*/

        public Ratio_Size ReturnValue;

        public struct Ratio_Size
        {
            public int width_ratio;
            public int height_ratio;
            public int image_count;
        }
        
        /*他フォームに値を返す関数*/
        static public Ratio_Size ShowMiniForm()
        {
            Form2_2 f = new Form2_2();
            f.ShowDialog();
            Ratio_Size receiveText = f.ReturnValue;
            f.Dispose();

            return receiveText;
        }

        /*コンボボックスの値を構造体に代入*/
        private void button1_Click(object sender, EventArgs e)
        {
            // 返すコンボボックスの値を設定
            CmbObject obj = (CmbObject)comboBox1.SelectedItem;
            this.ReturnValue.width_ratio = obj.width_ratio;
            this.ReturnValue.height_ratio = obj.height_ratio;
            this.ReturnValue.image_count = obj.image_count;
            past_index = comboBox1.SelectedIndex;

            // 返すカラーフラグを設定
            color_flag[0] = checkBox1.Checked;
            color_flag[1] = checkBox2.Checked;
            color_flag[2] = checkBox3.Checked;
            color_flag[3] = checkBox4.Checked;
            color_flag[4] = checkBox5.Checked;
            color_flag[5] = checkBox6.Checked;

            if (color_flag[0] || color_flag[1] || color_flag[2] || color_flag[3] || color_flag[4] || color_flag[5])
            this.Close();
        }

        /*--------------------------------------------------*/
        /*コンボボックスの値が変更された時のイベントパンドラ*/
        /*--------------------------------------------------*/

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            CmbObject obj = (CmbObject)comboBox1.SelectedItem;
            label_width_ratio.Text = Convert.ToString(obj.width_ratio);
            label_height_ratio.Text = Convert.ToString(obj.height_ratio);
            label_image_count.Text = Convert.ToString(obj.image_count);

            pictureBox1.Width = obj.width_ratio;
            pictureBox1.Height = obj.height_ratio;
        }
    }
}

    
