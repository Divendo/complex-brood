using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace complex_brood
{
    /// <summary>A class that holds a bunch of static functions to convert mandelnumbers to colors</summary>
    class MandelNumberConverters
    {
        /// <summary>A simple black and white display</summary>
        public static Color BlackWhite(int n, int maxN)
        {
            if(n == maxN)
                return Color.Black;

            return Color.FromArgb(255 - n % 256, 255 - n % 256, 255 - n % 256);
        }

        /// <summary>Picks a color depending on whether n is even (blue) or odd (red)</summary>
        public static Color EvenOdd(int n, int maxN)
        {
            if(n == maxN) return Color.Black;

            return n % 2 == 1 ? Color.Red : Color.Blue;
        }

        /// <summary>The red and blue channels depend on the ratio mandelnumber / maximum mandelnumber</summary>
        public static Color RedBlue(int n, int maxN)
        {
            if(n == maxN) return Color.Black;

            return Color.FromArgb(255 * n / maxN, 0, 255 * (maxN - n) / maxN);
        }

        /// <summary>Creates a color by converting the mandelnumber to a wave length</summary>
        public static Color WaveLength(int n, int maxN)
        {
            if(n == maxN) return Color.Black;

            double wave = 380.0 + 480.0 * n / maxN;
            double r = 0.0;
            double g = 0.0;
            double b = 0.0;

            if(wave >= 380.0 && wave <= 440.0)
            {
                r = -1.0 * (wave - 440.0) / (440.0 - 380.0);
                b = 1.0;
            }
            else if(wave >= 440.0 && wave <= 490.0)
            {
                g = (wave - 440.0) / (490.0 - 440.0);
                b = 1.0;
            }
            else if(wave >= 490.0 && wave <= 510.0)
            {
                g = 1.0;
                b = -1.0 * (wave - 510.0) / (510.0 - 490.0);
            }
            else if(wave >= 510.0 && wave <= 580.0)
            {
                r = (wave - 510.0) / (580.0 - 510.0);
                g = 1.0;
            }
            else if(wave >= 580.0 && wave <= 645.0)
            {
                r = 1.0;
                g = -1.0 * (wave - 645.0) / (645.0 - 580.0);
            }
            else if(wave >= 645.0 && wave <= 780.0)
                r = 1.0;

            double s = 1.0;
            if(wave > 700.0)
                s = 0.3 + 0.7 * (780.0 - wave) / (780.0 - 700.0);
            else if(wave <  420.0)
                s = 0.3 + 0.7 * (wave - 380.0) / (420.0 - 380.0);

            r = Math.Pow(r * s, 0.8);
            g = Math.Pow(g * s, 0.8);
            b = Math.Pow(b * s, 0.8);
            return Color.FromArgb((int) (r * 255), (int) (g * 255), (int) (b * 255));
        }

        /// <summary>Divides each mandelnumber into 3 channels: red, green and blue</summary>
        public static Color ThreeChannels(int n, int maxN)
        {
            if(n == maxN) return Color.Black;

            double x = n;
            double cubicRoot = Math.Pow(maxN, 1.0 / 3);

            double b = x - Math.Floor(x / cubicRoot) * cubicRoot;
            x = Math.Floor(x / cubicRoot);
            double g = x - Math.Floor(x / cubicRoot) * cubicRoot;
            x = Math.Floor(x / cubicRoot);
            double r = x;

            return Color.FromArgb((int) Math.Floor(255 * r / cubicRoot), (int) Math.Floor(255 * g / cubicRoot), (int) Math.Floor(255 * b / cubicRoot));
        }

        /// <summary>Picks a color on a color wheel (with blue on top, green on 1/3 and red on 2/3)</summary>
        public static Color ColorWheel(int n, int maxN)
        {
            if(n == maxN) return Color.Black;

            int angle = n % 510;

            int r = angle <= 85 ? 85 - angle : (angle <= 340 ? angle - 85 : 595 - angle);
            int g = angle <= 170 ? 85 + angle : (angle <= 425 ? 425 - angle : angle - 425);
            int b = Math.Abs(255 - angle);

            return Color.FromArgb(r, g, b);
        }
    }
}
