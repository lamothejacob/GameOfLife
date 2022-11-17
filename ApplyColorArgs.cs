using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Threading.Tasks;

namespace GameOfLife
{
    public delegate void ApplyColorHandler(object sender, ApplyColorArgs e);

    public class ApplyColorArgs
    {
        Color backgroundColor = Color.White;
        Color gridColor = Color.Black;
        Color cellColor = Color.Gray;

        public Color CellColor
        {
            get { return cellColor; }
            set { cellColor = value; }
        }

        public Color GridColor
        {
            get { return gridColor; }
            set { gridColor = value; }
        }

        public Color BackgroundColor
        {
            get { return backgroundColor; }
            set { backgroundColor = value; }
        }

        public ApplyColorArgs(Color backgroundColor, Color gridColor, Color cellColor)
        {
            this.backgroundColor = backgroundColor;
            this.cellColor = cellColor;
            this.gridColor = gridColor;
        }
    }
}
