using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace complex_brood
{
    /// <summary>Struct that describes the area of the mandelbrot that is to be calculated or that is calculated</summary>
    struct MandelAreaArgs
    {
        /// <summary>The x-coordinate around which the area is centred</summary>
        private double centerX;
        /// <summary>The y-coordinate around which the area is centred</summary>
        private double centerY;
        /// <summary>The size of each pixel</summary>
        private double scale;
        /// <summary>The maximum amount of iterations before a point is considered to be part of the mandelbrot set</summary>
        private int maxIterations;
        /// <param name="pxWidth">The amount of horizontal pixels</param>
        private int pxWidth;
        /// <param name="pxWidth">The amount of vertical pixels</param>
        private int pxHeight;
        /// <summary>The area that is to be calculated (in pixel coordinates)</summary>
        private Rectangle calcArea;

        /// <summary>Constructs an instance of MandelAreaArgs</summary>
        /// <param name="cx">The x-coordinate around which the area is centred</param>
        /// <param name="cy">The y-coordinate around which the area is centred</param>
        /// <param name="s">The size of each pixel</param>
        /// <param name="maxIter">The maximum amount of iterations before a point is considered to be part of the mandelbrot set</param>
        /// <param name="w">The amount of horizontal pixels</param>
        /// <param name="h">The amount of vertical pixels</param>
        public MandelAreaArgs(double cx = 0.0, double cy = 0.0, double s = 0.01, int maxIter = 500, int w = 400, int h = 400)
        {
            centerX = cx;
            centerY = cy;
            scale = s;
            maxIterations = maxIter;
            pxWidth = w;
            pxHeight = h;
            calcArea = new Rectangle(0, 0, w, h);
        }

        /// <summary>The x-coordinate around which the area is centred</summary>
        public double CenterX
        { get { return centerX; } set { centerX = value; } }

        /// <summary>The y-coordinate around which the area is centred</summary>
        public double CenterY
        { get { return centerY; } set { centerY = value; } }

        /// <summary>The size of each pixel</summary>
        public double Scale
        { get { return scale; } set { scale = value; } }

        /// <summary>The total width of the area (in mandelbrot coordinates)</summary>
        public double Diameter
        {
            get { return scale * pxWidth; }
            set { scale = value / pxWidth; }
        }

        /// <summary>The maximum amount of iterations before a point is considered to be part of the mandelbrot set</summary>
        public int MaxIterations
        { get { return maxIterations; } set { maxIterations = value; } }

        /// <param name="pxWidth">The amount of horizontal pixels</param>
        public int PxWidth
        { get { return pxWidth; } set { pxWidth = value; } }

        /// <param name="pxWidth">The amount of vertical pixels</param>
        public int PxHeight
        { get { return pxHeight; } set { pxHeight = value; } }

        /// <summary>The area that is to be calculated (in pixel coordinates)</summary>
        public Rectangle CalcArea
        { get { return calcArea; } set { calcArea = value; } }

        /// <summary>Calculates the x-coordinate of the left side of the area that is to be calculated in mandelbrot coordinates</summary>
        /// <returns>The x-coordinate of the left side of the area that is to be calculated in mandelbrot coordinates</returns>
        public double CalcAreaLeft()
        { return centerX - scale * pxWidth / 2 + scale / 2 + calcArea.X * scale; }

        /// <summary>Calculates the y-coordinate of the top of the area that is to be calculated in mandelbrot coordinates</summary>
        /// <returns>The y-coordinate of the top of the area that is to be calculated in mandelbrot coordinates</returns>
        public double CalcAreaTop()
        { return centerY + scale * pxHeight / 2 - scale / 2 - calcArea.Y * scale; }

        /// <summary>Calculates the y-coordinate of the bottom of the area that is to be calculated in mandelbrot coordinates</summary>
        /// <returns>The y-coordinate of the bottom of the area that is to be calculated in mandelbrot coordinates</returns>
        public double CalcAreaBottom()
        { return centerY - scale * pxHeight / 2 + scale / 2 + (pxHeight - calcArea.Bottom) * scale; }
    }
}
