using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GameOfLife
{
    public partial class Form1 : Form
    {
        // The universe array
        static bool[,] universe = new bool[20,10];
        bool[,] scratchpad = new bool[universe.GetLength(0), universe.GetLength(1)];

        // Drawing colors
        Color gridColor = Color.Black;
        Color cellColor = Color.Gray;
        Color backgroundColor = Color.White;

        // The Timer class
        Timer timer = new Timer();

        // Generation count
        int generations = 0;
        int livingCells = 0;
        int currentSeed = 0;

        bool start = false;
        bool seeNeighbors = true;
        bool boundary = false;
        bool gridOn = true;
        bool toggleHUD = true;

        public Form1()
        {
            InitializeComponent();

            load();

            //Initialize HUD Element
            toolStripStatusLabel3.Text = "Universe Size: " + universe.GetLength(0) + " x " + universe.GetLength(1);

            // Setup the timer
            timer.Interval = 100; // milliseconds
            timer.Tick += Timer_Tick;
            if (start)
            {
                timer.Enabled = true; // start timer running
            }
        }

        // Calculate the next generation of cells
        private void NextGeneration()
        {
            //Reset living cell count
            livingCells = 0;

            //Update Scratchpad
            for (int y = 0; y < universe.GetLength(1); y++)
            {
                for(int x = 0; x < universe.GetLength(0); x++)
                {
                    switch (GetNeighbors(x, y))
                    {
                        case 0:
                        case 1:
                            scratchpad[x, y] = false;
                            break;
                        case 2:
                            scratchpad[x,y] = universe[x, y];
                            if(scratchpad[x, y])
                            {
                                livingCells++;
                            }
                            break;
                        case 3:
                            scratchpad[x, y] = true;
                            livingCells++;
                            break;
                        default:
                            scratchpad[x, y] = false;
                            break;
                    }
                }
            }

            //Update the universe array
            bool[,] temp = universe;
            universe = scratchpad;
            scratchpad = temp;

            // Increment generation count
            generations++;

            // Update status strip
            toolStripStatusLabel1.Text = "Living Cells = " + livingCells.ToString();
            toolStripStatusLabelGenerations.Text = "Generations = " + generations.ToString();

            //Redraw the graphics panel
            graphicsPanel1.Invalidate();
        }

        //Counts and returns neighbors within the universe array
        private int GetNeighbors(int x1, int y1){
            int xCheck = 0, yCheck = 0, neighbors = 0;

            for (int y2 = y1 - 1; y2 <= y1 + 1; y2++)
            {
                for (int x2 = x1 - 1; x2 <= x1 + 1; x2++)
                {
                    //Ignore the current position
                    if(!(x2 == x1 && y1 == y2))
                    {
                        xCheck = x2;
                        yCheck = y2;

                        //Counts neighbors if boundary is Torodial
                        if (!boundary)
                        {
                            //Loop around the map on the x axis
                            if (x2 >= universe.GetLength(0))
                            {
                                xCheck -= universe.GetLength(0);
                            }
                            else if (x2 < 0)
                            {
                                xCheck += universe.GetLength(0);
                            }

                            //Loop around the map on the y axis
                            if (y2 >= universe.GetLength(1))
                            {
                                yCheck -= universe.GetLength(1);
                            }
                            else if (y2 < 0)
                            {
                                yCheck += universe.GetLength(1);
                            }

                            if (universe[xCheck, yCheck])
                            {
                                neighbors++;
                            }
                        }
                        //Counts neighbors if the boundary is finite
                        else if (!(x2 < 0 || y2 < 0) && !(x2 >= universe.GetLength(0) || y2 >= universe.GetLength(1)))
                        {
                            if (universe[xCheck, yCheck])
                            {
                                neighbors++;
                            }
                        }
                    }
                }
            }

            return neighbors;
        }

        // The event called by the timer every Interval milliseconds.
        private void Timer_Tick(object sender, EventArgs e)
        {
            NextGeneration();
        }

        private void graphicsPanel1_Paint(object sender, PaintEventArgs e)
        {
            // Calculate the width and height of each cell in pixels
            // CELL WIDTH = WINDOW WIDTH / NUMBER OF CELLS IN X
            int cellWidth = graphicsPanel1.ClientSize.Width / universe.GetLength(0);
            // CELL HEIGHT = WINDOW HEIGHT / NUMBER OF CELLS IN Y
            int cellHeight = graphicsPanel1.ClientSize.Height / universe.GetLength(1);

            //Adjust Background color
            graphicsPanel1.BackColor = backgroundColor;

            // A Pen for drawing the grid lines (color, width)
            Pen gridPen;

            if (gridOn)
            {
                gridPen = new Pen(gridColor, 1);
            }
            else
            {
                gridPen = new Pen(backgroundColor, 1);
            }

            // A Brush for filling living cells interiors (color)
            Brush cellBrush = new SolidBrush(cellColor);

            // Iterate through the universe in the y, top to bottom
            for (int y = 0; y < universe.GetLength(1); y++)
            {
                // Iterate through the universe in the x, left to right
                for (int x = 0; x < universe.GetLength(0); x++)
                {
                    // A rectangle to represent each cell in pixels
                    Rectangle cellRect = Rectangle.Empty;
                    cellRect.X = x * cellWidth;
                    cellRect.Y = y * cellHeight;
                    cellRect.Width = cellWidth;
                    cellRect.Height = cellHeight;

                    // Fill the cell with a brush if alive
                    if (universe[x, y] == true)
                    {
                        e.Graphics.FillRectangle(cellBrush, cellRect);
                    }

                    // Outline the cell with a pen
                    e.Graphics.DrawRectangle(gridPen, cellRect.X, cellRect.Y, cellRect.Width, cellRect.Height);

                    //Displays neighbor count
                    if (seeNeighbors)
                    {
                        e.Graphics.DrawString(GetNeighbors(x, y).ToString(), Font, Brushes.Tomato, cellRect.X + cellWidth / 3, cellRect.Y + cellHeight / 3);
                    }
                }
            }

            // Cleaning up pens and brushes
            gridPen.Dispose();
            cellBrush.Dispose();
        }

        private void graphicsPanel1_MouseClick(object sender, MouseEventArgs e)
        {
            // If the left mouse button was clicked
            if (e.Button == MouseButtons.Left)
            {
                // Calculate the width and height of each cell in pixels
                int cellWidth = graphicsPanel1.ClientSize.Width / universe.GetLength(0);
                int cellHeight = graphicsPanel1.ClientSize.Height / universe.GetLength(1);

                // Calculate the cell that was clicked in
                // CELL X = MOUSE X / CELL WIDTH
                int x = e.X / cellWidth;
                // CELL Y = MOUSE Y / CELL HEIGHT
                int y = e.Y / cellHeight;

                // Toggle the cell's state
                universe[x, y] = !universe[x, y];

                //Update living cells
                if(universe[x, y])
                {
                    livingCells++;
                }
                else
                {
                    livingCells--;
                }

                //Updates tool strip
                toolStripStatusLabel1.Text = "Living Cells = " + livingCells.ToString();

                //The seed has been modified and is no longer the current seed
                currentSeed = 0;

                // Tell Windows you need to repaint
                graphicsPanel1.Invalidate();
            }
        }

        //Randomizes the Universe Array
        private void RandomizeUniverse(int seed = 0)
        {
            //If seed is 0 then apply a new seed based on system time
            if(seed == 0)
            {
                seed = Environment.TickCount;
            }

            //Update the currentSeed variable
            currentSeed = seed;

            //Instantiate a random variable with the seed
            Random rand = new Random(seed);

            //Populate the universe array
            for (int y = 0; y < universe.GetLength(1); y++)
            {
                for (int x = 0; x < universe.GetLength(0); x++)
                {
                    if(rand.Next(0,3) == 0)
                    {
                        universe[x, y] = true;

                        //Increment living cells
                        livingCells++;
                    }
                    else { 
                        universe[x, y] = false;
                    }
                }
            }

            //Update Tool Strip
            toolStripStatusLabel1.Text = "Living Cells = " + livingCells.ToString();

            //Redraw the graphics panel
            graphicsPanel1.Invalidate();
        }

        //Start the game
        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            start = true;
            timer.Enabled = start;
        }

        //Pause the game
        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            start = false;
            timer.Enabled = start;
        }

        //Increment the game by one tick
        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            NextGeneration();
        }

        //CLear and Reset the Universe
        private void toolStripButton4_Click(object sender, EventArgs e)
        {
            //Update the universe array
            universe = new bool[universe.GetLength(0), universe.GetLength(1)];
            toolStripStatusLabel3.Text = "Universe Size: " + universe.GetLength(0) + " x " + universe.GetLength(1);

            //Reset the timer
            start = false;
            timer.Enabled = start;

            // Reset generation count, cell count, and currentSeed
            generations = 0;
            livingCells = 0;
            currentSeed = 0;

            // Update status strip generations
            toolStripStatusLabelGenerations.Text = "Generations = " + generations.ToString();
            toolStripStatusLabel1.Text = "Living Cells = " + livingCells.ToString();

            //Redraw the graphics panel
            graphicsPanel1.Invalidate();
        }

        //Start the game
        private void startToolStripMenuItem_Click(object sender, EventArgs e)
        {
            toolStripButton1_Click(sender, e);
        }

        //Pause the game
        private void pauseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            toolStripButton2_Click(sender, e);
        }

        //Increment the game by one tick
        private void continueToolStripMenuItem_Click(object sender, EventArgs e)
        {
            toolStripButton3_Click(sender, e);
        }

        //Clear and reset the Universe
        private void clearToolStripMenuItem_Click(object sender, EventArgs e)
        {
            toolStripButton4_Click(sender, e);
        }

        //Randomize the universe with no seed input
        private void noSeedToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            toolStripButton4_Click(sender, e);
            RandomizeUniverse();
        }

        //Randomize the universe with a seed input from a dialog box
        private void withSeedToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            int seed = 0;
            toolStripButton4_Click(sender, e);

            Form2 dlg = new Form2();

            dlg.Apply += new ApplyEventHandler(dlg_Apply);

            dlg.MyInteger = 0;
            dlg.SetString("Enter Seed: ");

            if (DialogResult.OK == dlg.ShowDialog())
            {
                seed = dlg.MyInteger;

                RandomizeUniverse(seed);
            }
        }

        void dlg_Apply(object sender, ApplyEventArgs e)
        {
            // Retrieve the event arguements
            int x = e.MyInteger;
            string s = e.MyString;
        }

        void dlg_Apply(object sender, ApplyNewUniverseArgs e)
        {
            // Retrieve the event arguements
            int x = e.MyWidth;
            int y = e.MyHeight;
        }

        void dlg_Apply(object sender, ApplyColorArgs e)
        {
            // Retrieve the event arguements
            Color x = e.BackgroundColor;
            Color y = e.GridColor;
            Color z = e.CellColor;
        }

        //Toggles the grid on or off
        private void toggleGridToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Toggles the grid on and off
            gridOn = !gridOn;

            //Redraws the graphics panel
            graphicsPanel1.Invalidate();
        }

        //Toggles whether you see the neighbor count or not
        private void toggleNeighborsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            seeNeighbors = !seeNeighbors;

            //Redraws the graphics panel
            graphicsPanel1.Invalidate();
        }

        //Closes the application
        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        //Adjusts the time in milliseconds between ticks, for the timer
        private void timingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int milliseconds = 0;
            //Clears and resets the universe
            toolStripButton4_Click(sender, e);

            Form2 dlg = new Form2();

            dlg.Apply += new ApplyEventHandler(dlg_Apply);

            dlg.MyInteger = 0;
            dlg.SetString("Enter Milliseconds: ");

            if (DialogResult.OK == dlg.ShowDialog())
            {
                milliseconds = dlg.MyInteger;

                //Sets the timer interval to the inputed miliseconds
                timer.Interval = milliseconds;
                timer.Tick += Timer_Tick;
            }
        }

        //Adjusts the size of the universe
        private void sizeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int width = 0, height = 0;
            toolStripButton4_Click(sender, e);

            Form3 dlg = new Form3();

            dlg.Apply += new ApplyUniverseHandler(dlg_Apply);

            dlg.MyHeight = 0;
            dlg.MyWidth = 0;

            if (DialogResult.OK == dlg.ShowDialog())
            {
                width = dlg.MyWidth;
                height = dlg.MyHeight;

                //Sets the universe to the new width and height
                universe = new bool[width, height];
                scratchpad = new bool[width, height];

                //Update HUD Element
                toolStripStatusLabel3.Text = "Universe Size: " + universe.GetLength(0) + " x " + universe.GetLength(1);

                //Redraws the graphics panel
                graphicsPanel1.Invalidate();
            }
        }

        //Resets the universe
        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            toolStripButton4_Click(sender, e);
        }

        //Opens a file and adjusts the universe's size to compensate
        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Resets the game
            toolStripButton4_Click(sender, e);
            
            //Creates a new open file dialog
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "All Files|*.*|Cells|*.cells";
            dlg.FilterIndex = 2;

            if (DialogResult.OK == dlg.ShowDialog())
            {
                StreamReader reader = new StreamReader(dlg.FileName);

                //Variables to adjust the size of the universe array
                int maxWidth = 0;
                int maxHeight = 0;

                while (!reader.EndOfStream)
                {
                    string row;

                    //Keeps grabbing the next line until its not a comment
                    do
                    {
                        row = reader.ReadLine();
                    } while (row.StartsWith("!"));

                    //Increments the height of the array
                    maxHeight += 1;

                    if(row.Length > maxWidth)
                    {
                        //Updates the width of the array
                        maxWidth = row.Length;
                    }
                }

                //Updates the universe to the proper dimensions
                universe = new bool[maxWidth, maxHeight];
                scratchpad = new bool[maxWidth, maxHeight];

                //Resets the input stream to the beginning of the file
                reader.BaseStream.Seek(0, SeekOrigin.Begin);

                int yPos = 0;
                while (!reader.EndOfStream)
                {
                    string row;

                    //Keeps grabbing the next line until its not a comment
                    do
                    {
                        row = reader.ReadLine();
                    } while (row.StartsWith("!"));

                    //Populates the current row of the array
                    for (int xPos = 0; xPos < row.Length; xPos++)
                    {
                        if(row[xPos] == 'O')
                        {
                            universe[xPos, yPos] = true;
                        }
                        else
                        {
                            universe[xPos, yPos] = false;
                        }
                    }

                    yPos++;
                }

                //Closes the reader
                reader.Close();

                //Redraws the new array
                graphicsPanel1.Invalidate();
            }
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Creates a save file dialog
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.Filter = "All Files|*.*|Cells|*.cells";
            dlg.FilterIndex = 2; dlg.DefaultExt = "cells";

            if (DialogResult.OK == dlg.ShowDialog())
            {
                StreamWriter writer = new StreamWriter(dlg.FileName);

                //Comments whether or not this is a custom creation or seed based
                if(currentSeed == 0)
                {
                    writer.WriteLine("!Custom Map");
                }
                else
                {
                    writer.WriteLine("!Seed: " + currentSeed.ToString());
                }
                
                //Comments the current generation and living cells at the time of saving
                writer.WriteLine("!Generation: " + generations.ToString());
                writer.WriteLine("!Living Cell Count: " + livingCells.ToString());

                for (int y = 0; y < universe.GetLength(1); y++)
                {
                    String currentRow = string.Empty;

                    //Places an 'O' for living cells and '.' for dead cells
                    for (int x = 0; x < universe.GetLength(0); x++)
                    {
                        if (universe[x, y])
                        {
                            currentRow += "O";
                        }
                        else
                        {
                            currentRow += ".";
                        }
                    }

                    //Writes the current line to the save file
                    writer.WriteLine(currentRow);
                }

                //Closes the file writer
                writer.Close();
            }
        }

        //Imports a file (Opens it without paying attention to data outside the proper range)
        private void importToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Creates an open file dialog
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "All Files|*.*|Cells|*.cells";
            dlg.FilterIndex = 2;

            if (DialogResult.OK == dlg.ShowDialog())
            {
                StreamReader reader = new StreamReader(dlg.FileName);

                int yPos = 0;
                while (!reader.EndOfStream && yPos < universe.GetLength(1))
                {
                    string row;

                    //Keeps grabbing the next line until its not a comment
                    do
                    {
                        row = reader.ReadLine();
                    } while (row.StartsWith("!"));

                    //Overwrites data until going over either the range of the universe, or range of the file being read
                    for (int xPos = 0; xPos < row.Length && xPos < universe.GetLength(0); xPos++)
                    {
                        if (row[xPos] == 'O')
                        {
                            universe[xPos, yPos] = true;
                        }
                        else
                        {
                            universe[xPos, yPos] = false;
                        }
                    }

                    yPos++;
                }

                //Closes the file reader
                reader.Close();

                //Redraws the graphics panel
                graphicsPanel1.Invalidate();
            }
        }

        private void fileToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        //Toggles the boundary between Torodial and Finite
        private void toggleBoundaryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Toggle the boundary between Torodial and Finite
            boundary = !boundary;

            //Update HUD
            if (!boundary)
            {
                toolStripStatusLabel2.Text = "Boundary Type: Torodial";
            }
            else
            {
                toolStripStatusLabel2.Text = "Boundary Type: Finite";
            }

            //Redraw panel
            graphicsPanel1.Invalidate();
        }

        //Uses a dialog menu to change colors within the game
        private void changeColorsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Color bg = Color.White, g = Color.Black, c = Color.Gray;

            Form4 dlg = new Form4();

            dlg.Apply += new ApplyColorHandler(dlg_Apply);

            dlg.BackgroundColor = bg;
            dlg.GridColor = g;
            dlg.CellColor = c;

            if (DialogResult.OK == dlg.ShowDialog())
            {
                bg = dlg.BackgroundColor;
                g = dlg.GridColor;
                c = dlg.CellColor;

                //Sets the universe to the new width and height
                backgroundColor = bg;
                gridColor = g;
                cellColor = c;

                //Redraws the graphics panel
                graphicsPanel1.Invalidate();
            }
        }

        private void toggleHUDToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Toggle HUD
            toggleHUD = !toggleHUD;

            //Set Visibility
            statusStrip1.Visible = toggleHUD;
        }

        //Saves necessary settings
        private void save()
        {
            Properties.Settings.Default.BackgroundColor = backgroundColor;
            Properties.Settings.Default.GridColor = gridColor;
            Properties.Settings.Default.CellColor = cellColor;
            Properties.Settings.Default.TimerInterval = timer.Interval;
            Properties.Settings.Default.UniverseWidth = universe.GetLength(0);
            Properties.Settings.Default.UniverseHeight = universe.GetLength(1);

            Properties.Settings.Default.Save();
        }

        //Loads settings from memory
        private void load()
        {
            Properties.Settings.Default.Reload();

            //Reload past values
            backgroundColor = Properties.Settings.Default.BackgroundColor;
            gridColor = Properties.Settings.Default.GridColor;
            cellColor = Properties.Settings.Default.CellColor;

            Console.WriteLine(backgroundColor + " , " + gridColor + " , " + cellColor);

            if (Properties.Settings.Default.TimerInterval == 0)
            {
                reset();
                load();
            }
            else
            {
                timer.Interval = Properties.Settings.Default.TimerInterval;
            }

            universe = new bool[Properties.Settings.Default.UniverseWidth, Properties.Settings.Default.UniverseHeight];

            //Redraw graphics panel
            graphicsPanel1.Invalidate();
        }

        private void reset()
        {
            //Set values to default values
            backgroundColor = Color.White;
            gridColor = Color.Black;
            cellColor = Color.Gray;
            timer.Interval = 100;
            universe = new bool[20, 10];

            save();

            //Redraw graphics panel
            graphicsPanel1.Invalidate();
        }

        private void saveToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            save();
        }

        private void reloadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            load();
        }

        private void resetToolStripMenuItem_Click(object sender, EventArgs e)
        {
            reset();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            save();
        }
        private void startToolStripMenuItem1_Click_1(object sender, EventArgs e)
        {
            toolStripButton1_Click(sender, e);
        }

        private void stopToolStripMenuItem_Click(object sender, EventArgs e)
        {
            toolStripButton2_Click(sender, e);
        }

        private void nextGenerationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            toolStripButton3_Click(sender, e);
        }

        private void clearToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            toolStripButton4_Click(sender, e);
        }

        private void hideToolStripMenuItem_Click(object sender, EventArgs e)
        {
            toggleHUDToolStripMenuItem_Click(sender, e);
        }
    }
}
