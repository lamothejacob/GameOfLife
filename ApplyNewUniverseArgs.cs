using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameOfLife
{
    public delegate void ApplyUniverseHandler(object sender, ApplyNewUniverseArgs e);

    public class ApplyNewUniverseArgs
    {
        int myHeight;
        int myWidth;
        public int MyHeight
        {
            get { return myHeight; }
            set { myHeight = value; }
        }
        public int MyWidth
        {
            get { return myWidth; }
            set { myWidth = value; }
        }

        public ApplyNewUniverseArgs(int myWidth, int myHeight)
        {
            this.myHeight = myHeight;
            this.myWidth = myWidth;
        }
    }
}
