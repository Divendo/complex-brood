using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
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
        /// <summary>The Mandelbrot instance that takes care of calculating the mandelnumbers</summary>
        private Mandelbrot mandelbrot;

        /// <summary>The area for which the mandelbrot should be calculated</summary>
        private MandelAreaArgs currArea;

        /// <summary>The area that was last calculated a mandelbrot for.
        /// Note that here CalcArea means the area that the mandelbrot is calculated for.</summary>
        private MandelAreaArgs calcedArea;

        /// <summary>The graphical representation of the current part of the mandelbrot</summary>
        private Bitmap img;

        /// <summary>The horizontal translation of `img`</summary>
        private int imgTransX;

        /// <summary>The vertical translation of `img`</summary>
        private int imgTransY;

        /// <summary>Whether we're currently calculating or not</summary>
        private bool calculating;

        public MandelDisplay()
        {
            // Initialize the control
            InitializeComponent();

            // Initialiaze some fields
            img = null;
            imgTransX = 0;
            imgTransY = 0;
            calculating = false;

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

        /// <summary>Translates the current mandelbrot images by the given amount of pixels.
        /// This translation is reset when the mandelbrot is recalculated.</summary>
        /// <param name="dx">The amount of pixels to translate horizontally</param>
        /// <param name="dy">The amount of pixels to translate vertically</param>
        public void Translate(int dx, int dy)
        {
            imgTransX += dx;
            imgTransY += dy;
            Invalidate();
        }

        // Handler for when `mandelbrot` is done calculating
        private void onMandelbrotDoneHandler(MandelAreaArgs args, int[] result)
        {
            // If necessary: create a new bitmap
            if(img == null || img.Width != args.PxWidth || img.Height != args.PxHeight)
            {
                img = new Bitmap(args.PxWidth, args.PxHeight);
                Graphics.FromImage(img).FillRectangle(Brushes.White, 0, 0, args.PxWidth, args.PxHeight);
            }

            // Draw the calculated pixels on the bitmap
            for(int x = 0; x < args.CalcArea.Width; ++x)
                for(int y = 0; y < args.CalcArea.Height; ++y)
                    img.SetPixel(args.CalcArea.X + x, args.CalcArea.Y + y, getColor(result[x + y * args.CalcArea.Width]));

            // Reset the translation for img
            imgTransX = 0;
            imgTransY = 0;

            // We're done calculating
            calculating = false;

            // Remember the area for which the calculation has been done
            calcedArea = args;

            // Redraw the screen
            Invalidate();
        }

        // Simple function to assign a color to a mandelnumber
        // Will later be replaced by some kind of delegate system, so that different color palettes can be used
        private Color getColor(int mandelNumber)
        {
            if(mandelNumber == 1000)
                return Color.Black;

            return Color.FromArgb(255 - mandelNumber % 256, 255 - mandelNumber % 256, 255 - mandelNumber % 256);
        }

        // Handler for the Paint event
        private void paint(object sender, PaintEventArgs pea)
        {
            // If there is no image, there is nothing to do
            if(img == null) return;

            // Draw a white background
            pea.Graphics.FillRectangle(Brushes.White, 0, 0, Width, Height);

            // Draw the image (scale it to fit)
            double factor = Width / ((double) img.Width);
            int w = (int) Math.Round(factor * img.Width);
            int h = (int) Math.Round(factor * img.Height);
            pea.Graphics.DrawImage(img, imgTransX + (Width - w) / 2, imgTransY + (Height - h) / 2, w, h);

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
