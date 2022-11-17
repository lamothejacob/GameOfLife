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
    public partial class Form4 : Form
    {
        // Declare the event
        public event ApplyColorHandler Apply;

        public Form4()
        {
            InitializeComponent();

            //Iterates and lists all valid colors
            int count = 1;
            foreach(KnownColor color in Enum.GetValues(typeof(KnownColor)))
            {
                if(count > 27 && count < Enum.GetValues(typeof(KnownColor)).Length - 6)
                {
                    Color c = Color.FromName(Enum.GetName(typeof(KnownColor), color));
                    listBox1.Items.Add(c);
                    listBox2.Items.Add(c);
                    listBox3.Items.Add(c);
                }
                
                count++;
            }
        }

        public Color BackgroundColor { get; set; }
        public Color GridColor { get; set; }
        public Color CellColor { get; set; }

        private void button2_Click(object sender, EventArgs e)
        {
            //Applies the selections from the list boxes
            BackgroundColor = (Color)listBox1.SelectedItem;
            GridColor = (Color)listBox2.SelectedItem;
            CellColor = (Color)listBox3.SelectedItem;
            
            if (Apply != null) Apply(this, new ApplyColorArgs(this.BackgroundColor, this.GridColor, this.CellColor));
        }
    }
}
