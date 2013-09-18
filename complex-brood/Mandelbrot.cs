using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace complex_brood
{
    /// <summary>
    /// A class that can calculate the mandelnumbers for a given area of the mandelbrot set.
    /// The class uses multiple threads to do so.
    /// </summary>
    class Mandelbrot : Control
    {
        /// <summary>Starts calculating the mandelnumbers for the given area of the mandelbrot set.</summary>
        /// <param name="centerX">The x-coordinate around which the area that is to be calculated is centred</param>
        /// <param name="centerY">The y-coordinate around which the area that is to be calculated is centred</param>
        /// <param name="scale">The scale of each pixel</param>
        /// <param name="maxIterations">The maximum amount of iterations before a point is considered to be part of the mandelbrot set</param>
        /// <param name="pxWidth">The amount of horizontal pixels</param>
        /// <param name="pxHeight">The amount of vertical pixels</param>
        public void Calculate(double centerX, double centerY, double scale, int maxIterations, int pxWidth, int pxHeight)
        {
        }

        /// <summary>The type of the method that is to be called when the mandelnumbers have been calculated</summary>
        /// <param name="mandelNumbers">An array containing the mandelnumbers, stored as follows: mandelNumbers[x + y * width] = mandelnumber for (x, y)</param>
        public delegate void OnCalculationDoneHandler(int[] mandelNumbers);

        /// <summary>The handler that is to be called when the calculation is done</summary>
        private OnCalculationDoneHandler onCalculationDoneHandler;

        /// <summary>Property to set or get the current OnCalculationDoneHandler</summary>
        public OnCalculationDoneHandler OnCalculationDone
        {
            get { return onCalculationDoneHandler; }
            set { onCalculationDoneHandler = value; }
        }

        /// <summary>Calls the OnCalculationDoneHandler</summary>
        /// <param name="mandelNumbers">The array of mandelnumbers that is to passed to the OnCalculationDoneHandler</param>
        private void onCalculationDone(int[] mandelNumbers)
        {
            if(onCalculationDoneHandler != null)
                this.Invoke(onCalculationDoneHandler, new object[]{ mandelNumbers });
        }
    }
}
