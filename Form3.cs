using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GameOfLife
{
    public partial class Form3 : Form
    {
        public event ApplyUniverseHandler Apply;

        public Form3()
        {
            InitializeComponent();
        }

        public int MyHeight { get; set; }
        public int MyWidth { get; set; }
        private void button2_Click(object sender, EventArgs e)
        {
            MyWidth = int.Parse(textBox1.Text);
            MyHeight = int.Parse(textBox2.Text);

            if (Apply != null) Apply(this, new ApplyNewUniverseArgs(this.MyWidth, this.MyHeight));
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }
    }
}
