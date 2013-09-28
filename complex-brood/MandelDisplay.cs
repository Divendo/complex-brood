using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace complex_brood
{
    /// <summary>Class that takes care of displaying the mandelbrot image</summary>
    public partial class MandelDisplay : Control
    {
        /// <summary>Functions that convert mandelnumbers to colors should have this form</summary>
        /// <param name="n">The mandelnumber</param>
        /// <param name="maxN">The maximum mandelnumber, that is if the mandelnumber equals this value it is said to part of the mandelbrot set</param>
        /// <returns>The color that belongs to the mandelnumber</returns>
        public delegate Color MandelNumberToColor(int n, int maxN);

        /// <summary>The Mandelbrot instance that takes care of calculating the mandelnumbers</summary>
        private Mandelbrot mandelbrot;

        /// <summary>The area for which the mandelbrot should be calculated</summary>
        private MandelAreaArgs currArea;

        /// <summary>The area that was last calculated a mandelbrot for.
        /// Note that here CalcArea means the area that the mandelbrot is calculated for.</summary>
        private MandelAreaArgs calcedArea;

        /// <summary>The mandelnumbers that have been calculated for calcedArea, stored as follows: mandelNumbers[x + y * width] = mandelnumber for (x, y).
        /// Note that here x, y are in pixel coordinates where x and y are relative to the top-left coordinates of `calcedArea.CalcArea`.
        /// Furthermore, width here is the same as `calcedArea.CalcArea.Width`</summary>
        private int[] mandelNumbers;

        /// <summary>The graphical representation of the current part of the mandelbrot</summary>
        private Bitmap img;

        /// <summary>Whether we're currently calculating or not</summary>
        private bool calculating;

        /// <summary>The current function for converting mandelnumbers to colors</summary>
        private MandelNumberToColor mandelNumberToColor;

        /// <summary>An array of colors, each for the corresponding mandelnumber</summary>
        private Color[] colorMap;

        /// <summary>Whether or not we expect the outcome of the next calculation to be the whole area</summary>
        private bool expectWholeArea;

        public MandelDisplay()
        {
            // Initialize the control
            InitializeComponent();

            // Initialiaze some fields
            currArea = new MandelAreaArgs();
            calcedArea = new MandelAreaArgs();
            mandelNumbers = null;
            img = null;
            calculating = false;
            mandelNumberToColor = defaultMandelNumberToColor;

            // Create and attach the mandelbrot calculator
            mandelbrot = new Mandelbrot();
            mandelbrot.OnCalculationDone = onMandelbrotDoneHandler;
            this.Controls.Add(mandelbrot);

            // Connect the event handlers
            this.Paint += this.paint;

            // Some properties of the control
            this.ResizeRedraw = true;
        }

        /// <summary>Sets the area of the mandelbrot that is to be shown (note that the CalcArea property is ignored)</summary>
        /// <param name="mea">The area of the mandelbrot that is to be shown</param>
        public void SetArea(MandelAreaArgs mea)
        {
            currArea = mea;
            currArea.CalcArea = new Rectangle(0, 0, currArea.PxWidth, currArea.PxHeight);
            currArea.CenterOnXAxis();
            currArea.CenterOnYAxis();
            Invalidate();
        }

        /// <summary>Recalculates the mandelbrot for the current size and area</summary>
        public void Recalc()
        {
            // Start the new calculation
            if(mandelNumbers != null && currArea.Scale == calcedArea.Scale && currArea.MaxIterations == calcedArea.MaxIterations)
            {
                if(currArea.CenterX == calcedArea.CenterX && currArea.CenterY == calcedArea.CenterY && currArea.PxWidth == calcedArea.PxWidth && currArea.PxHeight == calcedArea.PxHeight)
                    renderBitmap();
                else
                {
                    // Calculate the translation
                    int transX = (int) Math.Round((calcedArea.CenterX - currArea.CenterX) / calcedArea.Scale);
                    int transY = (int) Math.Round((currArea.CenterY - calcedArea.CenterY) / calcedArea.Scale);

                    // Check if the already calculated area and the area that is to be calculated overlap
                    if(calcedArea.CalcArea.X + transX < currArea.PxWidth && calcedArea.CalcArea.Y + transY < currArea.PxHeight &&
                       calcedArea.CalcArea.X + transX + calcedArea.CalcArea.Width > 0 && calcedArea.CalcArea.Y + transY + calcedArea.CalcArea.Height > 0)
                    {
                        // Calculate the overlapping area (in the pixel coordinates of currArea)
                        Rectangle overlap = new Rectangle(calcedArea.CalcArea.X + transX, calcedArea.CalcArea.Y + transY, calcedArea.CalcArea.Width, calcedArea.CalcArea.Height);
                        overlap.Intersect(new Rectangle(0, 0, currArea.PxWidth, currArea.PxHeight));

                        // Create a new array of mandelnumbers
                        int[] tmpMandelNumbers = new int[overlap.Width * overlap.Height];
                        for(int x = 0; x < overlap.Width; ++x)
                            for(int y = 0; y < overlap.Height; ++y)
                                tmpMandelNumbers[x + y * overlap.Width] = mandelNumbers[x + overlap.X - transX + (y + overlap.Y - transY) * calcedArea.CalcArea.Width];

                        // Set the calcedArea and mandelNumber to the new values overlapping values
                        calcedArea = currArea;
                        calcedArea.CalcArea = overlap;
                        mandelNumbers = tmpMandelNumbers;

                        // Start filling the gaps
                        calcPart();

                        // Create a new bitmap of the right size and copy the old one to it
                        Bitmap newImg = new Bitmap(calcedArea.PxWidth, calcedArea.PxHeight);
                        Graphics.FromImage(newImg).FillRectangle(Brushes.White, 0, 0, calcedArea.PxWidth, calcedArea.PxHeight);
                        Graphics.FromImage(newImg).DrawImage(img, transX, transY);
                        img = newImg;
                    }
                    else
                    {
                        // Just start a regular calculation if they don't overlap
                        expectWholeArea = true;
                        mandelbrot.Calculate(currArea);
                        calculating = true;
                    }
                }
            }
            else
            {
                expectWholeArea = true;
                mandelbrot.Calculate(currArea);
                calculating = true;
            }

            // Redraw the screen
            Invalidate();
        }

        /// <summary>Calculates a part that hasn't been calculated yet according to `calcedArea`</summary>
        /// <returns>True if a new calculation is started, otherwise false</returns>
        private bool calcPart()
        {
            // First fill the bands on top or below the calculated area
            // That leaves bigger vertical bands, which can use the symmetry of the mandelbrot to boost their speed
            if(calcedArea.CalcArea.Y > 0)
            {
                MandelAreaArgs area = calcedArea;
                area.CalcArea = new Rectangle(calcedArea.CalcArea.X, 0, calcedArea.CalcArea.Width, calcedArea.CalcArea.Y);
                expectWholeArea = false;
                mandelbrot.Calculate(area);
                calculating = true;
            }
            else if(calcedArea.CalcArea.Height < calcedArea.PxHeight)
            {
                MandelAreaArgs area = calcedArea;
                area.CalcArea = new Rectangle(calcedArea.CalcArea.X, calcedArea.CalcArea.Bottom, calcedArea.CalcArea.Width, calcedArea.PxHeight - calcedArea.CalcArea.Bottom);
                expectWholeArea = false;
                mandelbrot.Calculate(area);
                calculating = true;
            }
            else if(calcedArea.CalcArea.X > 0)
            {
                MandelAreaArgs area = calcedArea;
                area.CalcArea = new Rectangle(0, 0, calcedArea.CalcArea.X, calcedArea.PxHeight);
                expectWholeArea = false;
                mandelbrot.Calculate(area);
                calculating = true;
            }
            else if(calcedArea.CalcArea.Width < calcedArea.PxWidth)
            {
                MandelAreaArgs area = calcedArea;
                area.CalcArea = new Rectangle(calcedArea.CalcArea.Right, 0, calcedArea.PxWidth - calcedArea.CalcArea.Right, calcedArea.PxHeight);
                expectWholeArea = false;
                mandelbrot.Calculate(area);
                calculating = true;
            }
            else
                calculating = false;

            // Redraw the screen
            Invalidate();

            // Return whether we've started a new calculation
            return calculating;
        }

        public void SetMandelNumberToColorFunc(MandelNumberToColor f)
        {
            // Remember the function
            mandelNumberToColor = f;

            // Calculate the color map
            colorMap = new Color[calcedArea.MaxIterations + 1];
            for(int i = 0; i <= calcedArea.MaxIterations; ++i)
                colorMap[i] = mandelNumberToColor(i, calcedArea.MaxIterations);
        }


        /// <summary>Handler for when `mandelbrot` is done calculating</summary>
        private void onMandelbrotDoneHandler(MandelAreaArgs args, int[] result)
        {
            if(expectWholeArea)
            {
                // Remember the area for which the calculation has been done
                calcedArea = args;
                mandelNumbers = result;

                // Recalculate the color map if the maximum amount of iterations has changed
                if(colorMap.Length != calcedArea.MaxIterations + 1)
                {
                    colorMap = new Color[calcedArea.MaxIterations + 1];
                    for(int i = 0; i <= calcedArea.MaxIterations; ++i)
                        colorMap[i] = mandelNumberToColor(i, calcedArea.MaxIterations);
                }

                // We're done calculating
                calculating = false;

                // Render the bitmap
                renderBitmap();
            }
            else
            {
                // Calculate the new calcedArea.CalcArea
                Rectangle newCalcArea = new Rectangle(Math.Min(calcedArea.CalcArea.X, args.CalcArea.X),
                                                     Math.Min(calcedArea.CalcArea.Y, args.CalcArea.Y),
                                                     1, 1);
                newCalcArea.Width = Math.Max(calcedArea.CalcArea.Right, args.CalcArea.Right) - newCalcArea.X;
                newCalcArea.Height = Math.Max(calcedArea.CalcArea.Bottom, args.CalcArea.Bottom) - newCalcArea.Y;

                // Copy the new mandelnumbers together with the current mandelnumbers to a new array
                int[] tmpMandelNumbers = new int[newCalcArea.Width * newCalcArea.Height];
                for(int x = 0; x < newCalcArea.Width; ++x)
                {
                    for(int y = 0; y < newCalcArea.Height; ++y)
                    {
                        if(args.CalcArea.Contains(newCalcArea.X + x, newCalcArea.Y + y))
                            tmpMandelNumbers[x + y * newCalcArea.Width] = result[newCalcArea.X + x - args.CalcArea.X + (newCalcArea.Y + y - args.CalcArea.Y) * args.CalcArea.Width];
                        else
                            tmpMandelNumbers[x + y * newCalcArea.Width] = mandelNumbers[newCalcArea.X + x - calcedArea.CalcArea.X + (newCalcArea.Y + y - calcedArea.CalcArea.Y) * calcedArea.CalcArea.Width];
                    }
                }

                // Store the new result
                mandelNumbers = tmpMandelNumbers;
                calcedArea.CalcArea = newCalcArea;

                // Fill the next gap, or render the bitmap (if we've filled all gaps)
                if(!calcPart())
                    renderBitmap();
            }
        }

        /// <summary>Renders a bitmap from the currently calculated data and stores it in `img`</summary>
        private void renderBitmap()
        {
            // If there are no results (yet), we throw away the bitmap we have and stop here
            if(mandelNumbers == null)
            {
                img = null;
                Invalidate();
                return;
            }

            // If necessary: create a new bitmap
            if(img == null || img.Width != calcedArea.PxWidth || img.Height != calcedArea.PxHeight)
                img = new Bitmap(calcedArea.PxWidth, calcedArea.PxHeight);

            // Clear the bitmap
            Graphics.FromImage(img).FillRectangle(Brushes.White, 0, 0, calcedArea.PxWidth, calcedArea.PxHeight);

            // Draw the calculated pixels on the bitmap
            for(int x = 0; x < calcedArea.CalcArea.Width; ++x)
                for(int y = 0; y < calcedArea.CalcArea.Height; ++y)
                    img.SetPixel(calcedArea.CalcArea.X + x, calcedArea.CalcArea.Y + y, colorMap[mandelNumbers[x + y * calcedArea.CalcArea.Width]]);
            
            // Redraw the screen
            Invalidate();
        }

        // Simple function to assign a color to a mandelnumber
        private Color defaultMandelNumberToColor(int mandelNumber, int max)
        {
            if(mandelNumber == max)
                return Color.Black;

            return Color.FromArgb(255 - mandelNumber % 256, 255 - mandelNumber % 256, 255 - mandelNumber % 256);
        }

        // Handler for the Paint event
        private void paint(object sender, PaintEventArgs pea)
        {
            // Draw a white background
            pea.Graphics.FillRectangle(Brushes.White, 0, 0, Width, Height);

            // Draw the image, if we have one (scale it to fit)
            if(img != null)
            {
                RectangleF src = new RectangleF((float) ((currArea.Left() - calcedArea.Left()) / calcedArea.Scale),
                                                (float) ((calcedArea.Top() - currArea.Top()) / calcedArea.Scale),
                                                (float) (currArea.PxWidth * currArea.Scale / calcedArea.Scale),
                                                (float) (currArea.PxHeight * currArea.Scale / calcedArea.Scale));
                pea.Graphics.DrawImage(img, new RectangleF(0, 0, Width, Height), src, GraphicsUnit.Pixel);
            }

            // Show whether we're still calculating or not
            if(calculating)
            {
                Font font = new Font("arial", 10.0f);
                SizeF textSize = pea.Graphics.MeasureString("Rekenen...", font);
                pea.Graphics.FillRectangle(Brushes.Black, 0, 0, textSize.Width + 10, textSize.Height + 10);
                pea.Graphics.DrawString("Rekenen...", font, Brushes.White, 5, 5);
            }
        }
    }
}
