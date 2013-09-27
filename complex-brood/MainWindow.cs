using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace complex_brood
{
    public partial class MainWindow : Form
    {
        public MainWindow()
        {
            // Initialize the layout
            InitializeComponent();

            // Make sure the the diameter spinner has the right value
            spinnerDiameter.Value = spinnerScale.Value * mandelDisplay.Width;

            // Initial calculation for `mandelDisplay`
            recalcMandel();
        }

        /// <summary>Keeps track in which window state the last recalculation was done</summary>
        private FormWindowState lastRecalcState;

        /// <summary>Recalculate the mandelbrot for the current settings</summary>
        private void recalcMandel()
        {
            lastRecalcState = WindowState;

            mandelDisplay.SetArea(new MandelAreaArgs(Decimal.ToDouble(spinnerCenterX.Value),
                                        Decimal.ToDouble(spinnerCenterY.Value),
                                        Decimal.ToDouble(spinnerScale.Value),
                                        Decimal.ToInt32(spinnerMaxIterations.Value),
                                        mandelDisplay.Width, mandelDisplay.Height));
            mandelDisplay.Recalc();
        }

        // Click handler for `btnGo`
        private void btnGo_Click(object sender, EventArgs e)
        { recalcMandel(); }

        // ValueChanged handler for `spinnerScale`
        private void spinnerScale_ValueChanged(object sender, EventArgs e)
        { spinnerDiameter.Value = spinnerScale.Value * mandelDisplay.Width; }

        // ValueChanged handler for `spinnerDiameter`
        private void spinnerDiameter_ValueChanged(object sender, EventArgs e)
        { spinnerScale.Value = spinnerDiameter.Value / mandelDisplay.Width; }

        // ResizeEnd handler
        private void MainWindow_ResizeEnd(object sender, EventArgs e)
        {
            // Keep the same diameter
            spinnerScale.Value = spinnerDiameter.Value / mandelDisplay.Width;
            recalcMandel();
        }

        // Resize handler
        private void MainWindow_Resize(object sender, EventArgs e)
        {
            // Keep the same diameter
            spinnerScale.Value = spinnerDiameter.Value / mandelDisplay.Width;

            // If our state changes, we need to recalculate (the ResizeEnd event will not fire then)
            if(lastRecalcState != WindowState && WindowState != FormWindowState.Minimized)
                recalcMandel();
        }

        // MouseClick handler for `mandelDisplay`
        private void mandelDisplay_MouseClick(object sender, MouseEventArgs mea)
        {
            // Set the new center
            double scale = Decimal.ToDouble(spinnerScale.Value);
            spinnerCenterX.Value = new Decimal(Decimal.ToDouble(spinnerCenterX.Value) - scale * mandelDisplay.Width / 2 + scale / 2 + mea.X * scale);
            spinnerCenterY.Value = new Decimal(Decimal.ToDouble(spinnerCenterY.Value) + scale * mandelDisplay.Height / 2 - scale / 2 - mea.Y * scale);

            // Zoom in
            spinnerScale.Value = new Decimal(scale / 2);

            // Recalculate the image
            recalcMandel();
        }

        /// <summary>The last x-coordinate of the mouse if the mouse button is down, -1 otherwise</summary>
        int lastDragX = -1;
        /// <summary>The last y-coordinate of the mouse if the mouse button is down, -1 otherwise</summary>
        int lastDragY = -1;
        /// <summary>Whether the mandelbrot was dragged</summary>
        bool dragged = false;

        private void mandelDisplay_MouseDown(object sender, MouseEventArgs mea)
        {
            // If we're not pressing the left button, we're not interested
            if(mea.Button != System.Windows.Forms.MouseButtons.Left) return;

            // Start dragging
            lastDragX = mea.X;
            lastDragY = mea.Y;
        }

        private void mandelDisplay_MouseMove(object sender, MouseEventArgs mea)
        {
            if(lastDragX > 0 && lastDragY > 0)
            {
                // Translate the mandelbrot area
                spinnerCenterX.Value -= (mea.X - lastDragX) * spinnerScale.Value;
                spinnerCenterY.Value += (mea.Y - lastDragY) * spinnerScale.Value;

                // Add a translation to the current mandelbrot image, for visual feedback
                mandelDisplay.Translate(mea.X - lastDragX, mea.Y - lastDragY);

                // Remember the current mouse position
                lastDragX = mea.X;
                lastDragY = mea.Y;

                // The mandelbrot was dragged
                dragged = true;
            }
        }

        private void mandelDisplay_MouseUp(object sender, MouseEventArgs mea)
        {
            // If we're not pressing the left button, we're not interested
            if(mea.Button != System.Windows.Forms.MouseButtons.Left) return;

            // If we haven't dragged the mandelbrot, it was a click
            if(!dragged)
            {
                // Set the new center
                double scale = Decimal.ToDouble(spinnerScale.Value);
                spinnerCenterX.Value = new Decimal(Decimal.ToDouble(spinnerCenterX.Value) - scale * mandelDisplay.Width / 2 + scale / 2 + mea.X * scale);
                spinnerCenterY.Value = new Decimal(Decimal.ToDouble(spinnerCenterY.Value) + scale * mandelDisplay.Height / 2 - scale / 2 - mea.Y * scale);

                // Zoom in
                spinnerScale.Value = new Decimal(scale / 2);
            }

            // Recalculate the image
            recalcMandel();

            // Reset the variables to their initial state
            lastDragX = -1;
            lastDragY = -1;
            dragged = false;
        }

        private void mandelDisplay_MouseLeave(object sender, EventArgs e)
        {
            // Reset the variables to their initial state
            lastDragX = -1;
            lastDragY = -1;
            dragged = false;
        }
    }
}
