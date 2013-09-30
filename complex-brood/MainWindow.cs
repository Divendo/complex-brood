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
            spinnerScale.Value = spinnerDiameter.Value / mandelDisplay.Width;
            comboColorPalette.SelectedIndex = 0;
            comboLocations.SelectedIndex = 0;

            // Initial calculation for `mandelDisplay`
            mandelDisplay.SetMandelNumberToColorFunc(MandelNumberConverters.BlackWhite);
            recalcMandel();
        }

        /// <summary>Keeps track in which window state the last recalculation was done</summary>
        private FormWindowState lastRecalcState;

        /// <summary>Recalculate the mandelbrot for the current settings</summary>
        private void recalcMandel()
        {
            lastRecalcState = WindowState;
            mandelDisplay.SetArea(areaFromSpinners());
            mandelDisplay.Recalc();
        }

        /// <summary>Creates a MandelAreaArgs object from the current spinner values</summary>
        /// <returns>The created MandelAreaArgs</returns>
        private MandelAreaArgs areaFromSpinners()
        {
            return new MandelAreaArgs(Decimal.ToDouble(spinnerCenterX.Value),
                            Decimal.ToDouble(spinnerCenterY.Value),
                            Decimal.ToDouble(spinnerScale.Value),
                            Decimal.ToInt32(spinnerMaxIterations.Value),
                            mandelDisplay.Width, mandelDisplay.Height);
        }

        // Click handler for `btnGo`
        private void btnGo_Click(object sender, EventArgs e)
        { recalcMandel(); }

        // ValueChanged handler for `spinnerCenterX`
        private void spinnerCenterX_ValueChanged(object sender, EventArgs e)
        { mandelDisplay.SetArea(areaFromSpinners()); }

        // ValueChanged handler for `spinnerCenterY`
        private void spinnerCenterY_ValueChanged(object sender, EventArgs e)
        { mandelDisplay.SetArea(areaFromSpinners()); }

        // ValueChanged handler for `spinnerScale`
        private void spinnerScale_ValueChanged(object sender, EventArgs e)
        {
            // No check on the validity of the value is needed, since `spinnerScale.Value * mandelDisplay.Width` is always a valid value for `spinnerDiameter`
            spinnerDiameter.Value = spinnerScale.Value * mandelDisplay.Width;
            mandelDisplay.SetArea(areaFromSpinners());
        }

        // ValueChanged handler for `spinnerDiameter`
        private void spinnerDiameter_ValueChanged(object sender, EventArgs e)
        {
            spinnerScale.Value = Math.Max(spinnerScale.Minimum, Math.Min(spinnerScale.Maximum, spinnerDiameter.Value / mandelDisplay.Width));
            mandelDisplay.SetArea(areaFromSpinners());
        }

        // ResizeEnd handler
        private void MainWindow_ResizeEnd(object sender, EventArgs e)
        {
            // Recalculate the mandelbrot
            recalcMandel();
        }

        // Resize handler
        private void MainWindow_Resize(object sender, EventArgs e)
        {
            // Keep the same diameter
            spinnerScale.Value = Math.Max(spinnerScale.Minimum, Math.Min(spinnerScale.Maximum, spinnerDiameter.Value / mandelDisplay.Width));

            // Make sure that the right area is passed to the mandelDisplay
            // Because if only the height is being adjusted, the value of spinnerScale doesn't change
            mandelDisplay.SetArea(areaFromSpinners());

            // If our state changes, we need to recalculate (the ResizeEnd event will not fire then)
            if(lastRecalcState != WindowState && WindowState != FormWindowState.Minimized)
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
                spinnerCenterX.Value = Math.Max(spinnerCenterX.Minimum, Math.Min(spinnerCenterX.Maximum, spinnerCenterX.Value - (mea.X - lastDragX) * spinnerScale.Value));
                spinnerCenterY.Value = Math.Max(spinnerCenterY.Minimum, Math.Min(spinnerCenterY.Maximum, spinnerCenterY.Value + (mea.Y - lastDragY) * spinnerScale.Value));

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
                spinnerCenterX.Value = Math.Max(spinnerCenterX.Minimum, Math.Min(spinnerCenterX.Maximum, new Decimal(Decimal.ToDouble(spinnerCenterX.Value) - scale * mandelDisplay.Width / 2 + scale / 2 + mea.X * scale)));
                spinnerCenterY.Value = Math.Max(spinnerCenterY.Minimum, Math.Min(spinnerCenterY.Maximum, new Decimal(Decimal.ToDouble(spinnerCenterY.Value) + scale * mandelDisplay.Height / 2 - scale / 2 - mea.Y * scale)));

                // Zoom in
                spinnerScale.Value = Math.Max(spinnerScale.Minimum, Math.Min(spinnerScale.Maximum, new Decimal(scale / 2)));
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

        // SelectedIndexChanged handler for `comboColorPalette`
        private void comboColorPalette_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch(comboColorPalette.SelectedIndex)
            {
                case 0:
                    mandelDisplay.SetMandelNumberToColorFunc(MandelNumberConverters.BlackWhite);
                break;
                case 1:
                    mandelDisplay.SetMandelNumberToColorFunc(MandelNumberConverters.EvenOdd);
                break;
                case 2:
                    mandelDisplay.SetMandelNumberToColorFunc(MandelNumberConverters.RedBlue);
                break;
                case 3:
                    mandelDisplay.SetMandelNumberToColorFunc(MandelNumberConverters.WaveLength);
                break;
                case 4:
                    mandelDisplay.SetMandelNumberToColorFunc(MandelNumberConverters.ThreeChannels);
                break;
                case 5:
                    mandelDisplay.SetMandelNumberToColorFunc(MandelNumberConverters.ColorWheel);
                break;
            }
        }

        // SelectedIndexChanged handler for `comboLocations`
        private void comboLocations_SelectedIndexChanged(object sender, EventArgs e)
        {
            // The location data, sorted the same way as the combobox
            double[][] locationData = new double[][] {
                /*            Center X      Center Y        Hor. diameter   */
                new double[] {0,            0,              4.0},
                new double[] {-0.743643135, 0.131825963,    0.000014628},
                new double[] {-0.74364990,  0.13188204,     0.00073801},
                new double[] {-0.74364085,  0.13182733,     0.00012068},
                new double[] {-0.7435669,   0.1314023,      0.0022878},
                new double[] {-0.743644786, 0.1318252536,   0.0000029336},
                new double[] {-0.732532474, 0.216417977,    0.003984000} };

            // Set the new values to the spinners
            // No checks on the validity of the values is needed, since all values are garuanteed to be valid
            spinnerCenterX.Value = new Decimal(locationData[comboLocations.SelectedIndex][0]);
            spinnerCenterY.Value = new Decimal(locationData[comboLocations.SelectedIndex][1]);
            spinnerDiameter.Value = new Decimal(locationData[comboLocations.SelectedIndex][2]);
        }
    }
}
