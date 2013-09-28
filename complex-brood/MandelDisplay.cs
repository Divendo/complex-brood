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
        MandelNumberToColor mandelNumberToColor;

        public MandelDisplay()
        {
            // Initialize the control
            InitializeComponent();

            // Initialiaze some fields
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

        /// <summary>Sets the area of the mandelbrot that is to be shown</summary>
        /// <param name="mea">The area of the mandelbrot that is to be shown</param>
        public void SetArea(MandelAreaArgs mea)
        {
            currArea = mea;
            currArea.CenterOnXAxis();
            Invalidate();
        }

        /// <summary>Recalculates the mandelbrot for the current size and area</summary>
        public void Recalc()
        {
            // Start the new calculation
            mandelbrot.Calculate(currArea);
            calculating = true;

            // Redraw the screen
            Invalidate();
        }

        public void SetMandelNumberToColorFunc(MandelNumberToColor f)
        { mandelNumberToColor = f; }
            
            
        /// <summary>Handler for when `mandelbrot` is done calculating</summary>
        private void onMandelbrotDoneHandler(MandelAreaArgs args, int[] result)
        {
            // Remember the area for which the calculation has been done
            calcedArea = args;
            mandelNumbers = result;

            // We're done calculating
            calculating = false;

            // Render the bitmap
            renderBitmap();
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
            {
                img = new Bitmap(calcedArea.PxWidth, calcedArea.PxHeight);
                Graphics.FromImage(img).FillRectangle(Brushes.White, 0, 0, calcedArea.PxWidth, calcedArea.PxHeight);
            }

            // Draw the calculated pixels on the bitmap
            for(int x = 0; x < calcedArea.CalcArea.Width; ++x)
                for(int y = 0; y < calcedArea.CalcArea.Height; ++y)
                    img.SetPixel(calcedArea.CalcArea.X + x, calcedArea.CalcArea.Y + y, mandelNumberToColor(mandelNumbers[x + y * calcedArea.CalcArea.Width], calcedArea.MaxIterations));

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
               /* GraphicsState state = pea.Graphics.Save();
                pea.Graphics.TranslateTransform((float) ((calcedArea.CenterX - currArea.CenterX) / calcedArea.Scale), (float) ((currArea.CenterY - calcedArea.CenterY) / calcedArea.Scale));
                float scaleFactor = (float) (calcedArea.Scale / currArea.Scale);
                pea.Graphics.ScaleTransform(scaleFactor, scaleFactor);

                /*double factor = Width / ((double) img.Width);
                int w = (int) Math.Round(factor * img.Width);
                int h = (int) Math.Round(factor * img.Height);
                pea.Graphics.DrawImage(img, (Width - w) / 2, (Height - h) / 2, w, h);*/
                //pea.Graphics.DrawImage(img, 0, 0);
                //pea.Graphics.DrawImage(img, new Rectangle(0, 0, Width, Height), new Rectangle(-50, -50, img.Width + 100, img.Height + 100), GraphicsUnit.Pixel);

                //pea.Graphics.Restore(state);
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
