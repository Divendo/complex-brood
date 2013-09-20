/***************************** LICENSE - GPLv3 ******************************
*                                                                           *
*   complex-brood, a mandelbrot set viewer in C#                            *
*   Copyright (C) 2013  Mark Kamsma and Cas Widdershoven                    *
*                                                                           *
*   This program is free software: you can redistribute it and/or modify    *
*   it under the terms of the GNU General Public License as published by    *
*   the Free Software Foundation, either version 3 of the License, or       *
*   (at your option) any later version.                                     *
*                                                                           *
*   This program is distributed in the hope that it will be useful,         *
*   but WITHOUT ANY WARRANTY; without even the implied warranty of          *
*   MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the           *
*   GNU General Public License for more details.                            *
*                                                                           *
*   You should have received a copy of the GNU General Public License       *
*   along with this program.  If not, see {http://www.gnu.org/licenses/}.   *
*                                                                           *
*****************************************************************************/

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
    public partial class TestWindow : Form
    {
        private const int max = 1000;
        private int calcW = 0;
        private int calcH = 0;
        private int lastCalcW = 0;
        private int lastCalcH = 0;

        private int startTicks = 0;
        private int deltaTicks = 0;

        public TestWindow()
        {
            InitializeComponent();

            this.Paint += paint;
            this.Resize += resize;
            this.ClientSize = new Size(400, 400);

            // The Mandelbrot is a control, so it should be added to the window
            this.Controls.Add(mandelbrot);
            // Set the handler for when a calculation is done
            mandelbrot.OnCalculationDone = OnMandelbrotDoneHandler;

            // Start the initial calculation
            recalc();
        }

        Mandelbrot mandelbrot = new Mandelbrot();
        int[] mandelNumbers = null;

        public void recalc()
        {
            calcW = ClientSize.Width;
            calcH = ClientSize.Height;
            startTicks = Environment.TickCount;

            // Start the calculation
            mandelbrot.Calculate(0, 0, 4.0 / calcW, max, calcW, calcH);
        }

        void OnMandelbrotDoneHandler(int[] result)
        {
            deltaTicks = Environment.TickCount - startTicks;
            mandelNumbers = result;
            lastCalcW = calcW;
            lastCalcH = calcH;
            Invalidate();
        }

        public void paint(object o, PaintEventArgs pea)
        {
            if(mandelNumbers == null) return;

            Bitmap bmp = new Bitmap(lastCalcW, lastCalcH);
            for(int y = 0; y < lastCalcH; ++y)
                for(int x = 0; x < lastCalcW; ++x)
                    bmp.SetPixel(x, y, getColor(mandelNumbers[x + y * lastCalcW]));
            pea.Graphics.DrawImage(bmp, 0, 0);

            pea.Graphics.DrawString("Calculation time: " + deltaTicks.ToString() + "ms", new Font("Arial", 12), Brushes.Black, new PointF(10, 10));
        }

        public void resize(object o, EventArgs ea)
        { recalc(); }

        public Color getColor(int mandelNumber)
        {
            if(mandelNumber == max)
                return Color.Black;

            return Color.FromArgb(255 - mandelNumber % 256, 255 - mandelNumber % 256, 255 - mandelNumber % 256);
        }
    }
}
