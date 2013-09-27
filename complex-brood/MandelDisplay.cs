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

        /// <summary>The graphical representation of the current part of the mandelbrot</summary>
        private Bitmap img;

        public MandelDisplay()
        {
            // Initialize the control
            InitializeComponent();

            // Initialiaze some fields
            img = null;

            // Create and attach the mandelbrot calculator
            mandelbrot = new Mandelbrot();
            mandelbrot.OnCalculationDone = onMandelbrotDoneHandler;
            this.Controls.Add(mandelbrot);

            // Connect the event handlers
            this.Paint += this.paint;

            // Some properties of the control
            this.ResizeRedraw = true;
        }

        /// <summary>Recalculates the mandelbrot for the current size and area</summary>
        public void Recalc()
        {
            mandelbrot.Calculate(new MandelAreaArgs(0, 0, 4.0 / Width, 1000, Width, Height));
        }

        // Handler for when `mandelbrot` is done calculating
        private void onMandelbrotDoneHandler(MandelAreaArgs args, int[] result)
        {
            // If necessary: create a new bitmap
            if(img == null || img.Width != args.PxWidth || img.Height != args.PxHeight)
            {
                img = new Bitmap(args.PxWidth, args.PxHeight);
                Graphics.FromImage(img).FillRectangle(Brushes.Red, 0, 0, args.PxWidth, args.PxHeight);
            }

            // Draw the calculated pixels on the bitmap
            for(int x = 0; x < args.CalcArea.Width; ++x)
                for(int y = 0; y < args.CalcArea.Height; ++y)
                    img.SetPixel(args.CalcArea.X + x, args.CalcArea.Y + y, getColor(result[x + y * args.CalcArea.Width]));

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
            pea.Graphics.DrawImage(img, (Width - w) / 2, (Height - h) / 2, w, h);
        }
    }
}
