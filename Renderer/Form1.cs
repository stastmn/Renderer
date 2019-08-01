using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Renderer
{
    public partial class Form1 : Form
    {
        public void Render(ref Bitmap bmp)
        {
            
            pictureBox1.Image = bmp;
            
        }

        Bitmap _bmp;
        public Form1()
        {
            InitializeComponent();
            
        }
        public Form1(Bitmap bmp)
        {
            InitializeComponent();
             _bmp = bmp;

            /*
            var w = new System.Windows.Window();
            UserControl1 u = new UserControl1(  this);
            w.Content = u;
            w.Show();
            */
            
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            pictureBox1.Image = _bmp;
        }
        
        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void Form1_Enter(object sender, EventArgs e)
        {
            pictureBox1.Image = _bmp;
        }
    }
}
