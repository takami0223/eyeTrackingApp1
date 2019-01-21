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
    public partial class Form_PanelTopImg_Womans : Form
    {
        public Form_PanelTopImg_Womans()
        {
            InitializeComponent();
            Opacity = 0;
        }

        private void Form_PanelTopImg_Load(object sender, EventArgs e)
        {
            Animator.Animate(150, (frame, frequency) =>
            {
                if (!Visible || IsDisposed) return false;
                Opacity = (double)frame / frequency;
                return true;
            });

            List<List<List<Image>>> panel_top_image = UserControl2.panel_top_image;
            //if (panel_top_image.Count != 0) No_data_label1.Visible = false;
            Control_Generation(panel_top_image);
        }

        private void Control_Generation(List<List<List<Image>>> panel_top_image)
        {
            for (int tab = 0; tab < panel_top_image.Count; tab++)
            {
                TabPage tp = new TabPage("測定" + (tabControl1.TabPages.Count+1));
                tabControl1.TabPages.Add(tp);

                Panel panel = new Panel();
                panel.Name = "panel" + tab.ToString();
                panel.BackColor = Color.White;
                panel.Size = new Size(770, 430);//(780, 400);//768
                panel.AutoScroll = true;

                ShapeContainer canvas = new ShapeContainer();
                //canvas.Parent = panel;

                /* 順位のラベル */
                for (int i = 0; i < panel_top_image[tab].Count; i++)
                {
                    for (int j = 0; j < 3; j++)
                    {
                        Label label1 = new Label();
                        label1.Name = "Label1_" + j.ToString();
                        label1.Location = new Point(155 + (200 * j), 40 + (220 * i));
                        label1.Font = new Font("MS UI Gothic", 20);
                        label1.AutoSize = true;
                        label1.Text = string.Format((j + 1).ToString() + "位");
                        if (j == 0) label1.ForeColor = Color.Gold;
                        else if (j == 1) label1.ForeColor = Color.Silver;
                        else if (j == 2) label1.ForeColor = Color.IndianRed;
                        panel.Controls.Add(label1);
                    }
                }

                for (int count = 0; count < panel_top_image[tab].Count; count++)
                {
                    for (int img = 0; img < panel_top_image[tab][count].Count; img++)
                    {
                        //ShapeContainer canvas = new ShapeContainer();
                        canvas.Parent = panel;
                        
                        /* パネルにピクチャーボックスと色縁を作成 */
                        PictureBox pictureBox = new PictureBox();
                        pictureBox.Name = "TextBox" + count.ToString() + "_" + img.ToString();
                        pictureBox.Location = new Point(150 + (200 * img), 60 + (220 * count));
                        pictureBox.Size = new Size(140, 170);
                        pictureBox.SizeMode = PictureBoxSizeMode.Zoom;
                        pictureBox.Image = panel_top_image[tab][count][img];
                        panel.Controls.Add(pictureBox);

                        if (img <= 2)
                        {
                            LineShape lineShape1 = new LineShape();
                            LineShape lineShape2 = new LineShape();
                            LineShape lineShape3 = new LineShape();
                            LineShape lineShape4 = new LineShape();

                            lineShape1.Parent = canvas;
                            lineShape2.Parent = canvas;
                            lineShape3.Parent = canvas;
                            lineShape4.Parent = canvas;
                            lineShape1.Name = "LineShape1_" + count.ToString();
                            lineShape2.Name = "LineShape2_" + count.ToString();
                            lineShape3.Name = "LineShape3_" + count.ToString();
                            lineShape4.Name = "LineShape4_" + count.ToString();

                            lineShape1.BorderWidth = 3;
                            lineShape2.BorderWidth = 3;
                            lineShape3.BorderWidth = 3;
                            lineShape4.BorderWidth = 3;

                            lineShape1.StartPoint = new Point(150 + (200 * img), 55 + (220 * count));
                            lineShape1.EndPoint = new Point(290 + (200 * img), 55 + (220 * count));

                            lineShape2.StartPoint = new Point(290 + (200 * img), 55 + (220 * count));
                            lineShape2.EndPoint = new Point(290 + (200 * img), 235 + (220 * count));

                            lineShape3.StartPoint = new Point(290 + (200 * img), 235 + (220 * count));
                            lineShape3.EndPoint = new Point(150 + (200 * img), 235 + (220 * count));

                            lineShape4.StartPoint = new Point(150 + (200 * img), 235 + (220 * count));
                            lineShape4.EndPoint = new Point(150 + (200 * img), 55 + (220 * count));

                            if (img == 0)
                            {
                                lineShape1.BorderColor = Color.Gold;
                                lineShape2.BorderColor = Color.Gold;
                                lineShape3.BorderColor = Color.Gold;
                                lineShape4.BorderColor = Color.Gold;
                            }
                            else if (img == 1)
                            {
                                lineShape1.BorderColor = Color.Silver;
                                lineShape2.BorderColor = Color.Silver;
                                lineShape3.BorderColor = Color.Silver;
                                lineShape4.BorderColor = Color.Silver;
                            }
                            else if (img == 2)
                            {
                                lineShape1.BorderColor = Color.IndianRed;
                                lineShape2.BorderColor = Color.IndianRed;
                                lineShape3.BorderColor = Color.IndianRed;
                                lineShape4.BorderColor = Color.IndianRed;
                            }
                        }
                        

                    }
                    /* パネルにラベルを作成 */
                    Label label4 = new Label();
                    label4.Name = "Label" + count.ToString();
                    label4.Location = new Point(20, 130 + (220 * count));
                    label4.Font = new Font("MS UI Gothic", 30);
                    label4.AutoSize = true;
                    label4.Text = string.Format("{0}回目", count + 1);
                    panel.Controls.Add(label4);
                    
                    /* 区切り線 */
                    /*
                    LineShape lineShape5 = new LineShape();
                    lineShape5.Parent = canvas;
                    lineShape5.Name = "LineShape5_" + count.ToString();
                    lineShape5.BorderStyle = System.Drawing.Drawing2D.DashStyle.Dash;
                    lineShape5.BorderWidth = 3;
                    lineShape5.StartPoint = new Point(30, 55 + (220 * (count+1)));
                    lineShape5.EndPoint = new Point(700, 55 + (220 * (count+1)));
                    */
                }

                tabControl1.TabPages[tab].Controls.Add(panel);
                //canvas.Parent = panel;
            }
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
    }
}
