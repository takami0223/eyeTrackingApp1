using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.VisualBasic.PowerPacks;

namespace eyeTrackingApp1
{
    public partial class Form3_1 : Form
    {
        public Form3_1()
        {
            InitializeComponent();
            Opacity = 0;
        }

        private void Form3_Load(object sender, EventArgs e)
        {

            Animator.Animate(150, (frame, frequency) =>
            {
                if (!Visible || IsDisposed) return false;
                Opacity = (double)frame / frequency;
                return true;
            });

            /*-----------------------------------------*/
            /*tabPage1(Shirt)での表示パネルを生成*/
            /*-----------------------------------------*/
            List<UserControl1.Image_Proportion> image_proportion1 = UserControl1.image_proportion_shirt;
            if (image_proportion1.Count != 0) No_data_label1.Visible = false;
            Control_Generation(image_proportion1, panel1);

            /*-----------------------------------------*/
            /*tabPage2(Pant)での表示パネルを生成*/
            /*-----------------------------------------*/
            List<UserControl1.Image_Proportion> image_proportion2 = UserControl1.image_proportion_pant;
            if (image_proportion2.Count != 0) No_data_label2.Visible = false;
            Control_Generation(image_proportion2, panel2);

            /*-----------------------------------------*/
            /*tabPage3(Shoes)での表示パネルを生成*/
            /*-----------------------------------------*/
            List<UserControl1.Image_Proportion> image_proportion3 = UserControl1.image_proportion_shoes;
            if (image_proportion3.Count != 0) No_data_label3.Visible = false;
            Control_Generation(image_proportion3, panel3);

            /*-----------------------------------------*/
            /*tabPage4(All_Content)での表示パネルを生成*/
            /*-----------------------------------------*/
            List<UserControl1.Image_Proportion> image_proportion4 = UserControl1.image_proportion_all;
            if (image_proportion4.Count != 0) No_data_label4.Visible = false;
            Control_Generation(image_proportion4, panel4);
            
        }

        private void Control_Generation(List<UserControl1.Image_Proportion> image_proportion, Panel panel)
        {
            for (int i = 0; i < image_proportion.Count; i++)
            {

                ShapeContainer canvas = new ShapeContainer();
                canvas.Parent = panel;

                /*パネルにピクチャーボックスと色縁を作成*/
                PictureBox pictureBox = new PictureBox();
                pictureBox.Name = "TextBox" + i.ToString();
                pictureBox.Location = new Point(50, 30 + (220 * i));
                pictureBox.Size = new Size(140, 170);
                pictureBox.SizeMode = PictureBoxSizeMode.Zoom;
                pictureBox.Image = image_proportion[i].image;
                panel.Controls.Add(pictureBox);

                if (i <= 2)
                {
                    LineShape lineShape1 = new LineShape();
                    LineShape lineShape2 = new LineShape();
                    LineShape lineShape3 = new LineShape();
                    LineShape lineShape4 = new LineShape();

                    lineShape1.Parent = canvas;
                    lineShape2.Parent = canvas;
                    lineShape3.Parent = canvas;
                    lineShape4.Parent = canvas;
                    lineShape1.Name = "LineShape1_" + i.ToString();
                    lineShape2.Name = "LineShape2_" + i.ToString();
                    lineShape3.Name = "LineShape3_" + i.ToString();
                    lineShape4.Name = "LineShape4_" + i.ToString();
                    
                    lineShape1.BorderWidth = 3;
                    lineShape2.BorderWidth = 3;
                    lineShape3.BorderWidth = 3;
                    lineShape4.BorderWidth = 3;

                    lineShape1.StartPoint = new Point(50, 25 + (220 * i));
                    lineShape1.EndPoint = new Point(190, 25 + (220 * i));

                    lineShape2.StartPoint = new Point(190, 25 + (220 * i));
                    lineShape2.EndPoint = new Point(190, 205 + (220 * i));

                    lineShape3.StartPoint = new Point(190, 205 + (220 * i));
                    lineShape3.EndPoint = new Point(50, 205 + (220 * i));

                    lineShape4.StartPoint = new Point(50, 205 + (220 * i));
                    lineShape4.EndPoint = new Point(50, 25 + (220 * i));

                    if (i == 0)
                    {
                        lineShape1.BorderColor = Color.Gold;
                        lineShape2.BorderColor = Color.Gold;
                        lineShape3.BorderColor = Color.Gold;
                        lineShape4.BorderColor = Color.Gold;
                    }
                    else if (i == 1)
                    {
                        lineShape1.BorderColor = Color.Silver;
                        lineShape2.BorderColor = Color.Silver;
                        lineShape3.BorderColor = Color.Silver;
                        lineShape4.BorderColor = Color.Silver;
                    }
                    else if (i == 2)
                    {
                        lineShape1.BorderColor = Color.IndianRed;
                        lineShape2.BorderColor = Color.IndianRed;
                        lineShape3.BorderColor = Color.IndianRed;
                        lineShape4.BorderColor = Color.IndianRed;
                    }
                }

                /*パネルにラベルを作成*/
                Label label = new Label();
                label.Name = "Label" + i.ToString();
                label.Location = new Point(250, 100 + (220 * i));
                label.Font = new Font("MS UI Gothic", 30);
                label.AutoSize = true;
                label.Text = string.Format("{0:0.00}%", image_proportion[i].proportion);
                panel.Controls.Add(label);

                /*パネルにラインを作成*/
                LineShape lineShape5 = new LineShape();
                LineShape lineShape6 = new LineShape();

                lineShape5.Parent = canvas;
                lineShape5.Name = "LineShape5_" + i.ToString();
                lineShape5.StartPoint = new Point(207, 113 + (220 * i));
                lineShape5.EndPoint = new Point(248, 146 + (220 * i));

                lineShape6.Parent = canvas;
                lineShape6.Name = "LineShape6_" + i.ToString();
                lineShape6.StartPoint = new Point(248, 146 + (220 * i));
                lineShape6.EndPoint = new Point(382, 146 + (220 * i));
            }
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void tabPage1_Click(object sender, EventArgs e)
        {
            this.Text = "あなたの記録(シャツのみ)";
            this.AutoScrollPosition = new Point(0, 0);
        }

        private void tabPage2_Click(object sender, EventArgs e)
        {
            this.Text = "あなたの記録(ズボンのみ)";
        }

        private void tabPage3_Click(object sender, EventArgs e)
        {
            this.Text = "あなたの記録(靴のみ)";
        }

        private void tabPage4_Click(object sender, EventArgs e)
        {
            this.Text = "あなたの記録(すべての記録)";
        }
    }
}
