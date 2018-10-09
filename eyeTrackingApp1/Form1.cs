using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Windows.Forms;
using Tobii.Interaction;
using Tobii.Interaction.Framework;
using IronPython.Hosting;
using Microsoft.Scripting.Hosting;

namespace eyeTrackingApp1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            Opacity = 0;
            //this.Load += new EventHandler(Form1_Load);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Animator.Animate(500, (frame, frequency) =>
             {
                 if (!Visible || IsDisposed) return false;
                 Opacity = (double)frame / frequency;
                 return true;
             });
            Next(typeof(UserControl1));
        }

        public void Next(Type t)
        {
            if (t == null)
            {
                Close();
                return;
            }
            else
            {
                this.Controls.Clear();
                var uc = Activator.CreateInstance(t) as UserControl;
                this.Controls.Add(uc);
            }
        }

        public void Text_Mens()
        {
            this.Text = "視線による好みの推定(メンズ)";
        }

        public void Text_Ladies()
        {
            this.Text = "視線による好みの推定(レディース)";
        }

    } 
}
