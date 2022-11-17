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
    public partial class Form2 : Form
    {
        // Declare the event
        public event ApplyEventHandler Apply;

        public Form2()
        {
            InitializeComponent();
        }

        private void Form2_Load(object sender, EventArgs e)
        {

        }

        public int MyInteger { get; set; }
        public string MyString { get; set; }

        public void SetString(string label)
        {
            MyString = label;
            label1.Text = MyString;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                MyInteger = int.Parse(textBox1.Text);
            }catch (Exception except)
            {
                foreach(char c in textBox1.Text)
                {
                    MyInteger += (int)c;
                }
            }
            
            if (Apply != null) Apply(this, new ApplyEventArgs(this.MyInteger, this.MyString));
        }
    }
}
