using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Forms;

namespace complex_brood
{
    /// <summary>
    /// A class that can calculate the mandelnumbers for a given area of the mandelbrot set.
    /// The class uses multiple threads to do so.
    /// </summary>
    class Mandelbrot : Control
    {
        /// <summary>The maximum amount of pixels each thread should calculate</summary>
        private const int WORK_PER_THREAD = 1000;

        /// <summary>The worker that's currently active</summary>
        private Worker worker = null;

        /// <summary>Worker threads results reporting synchronizes on this object</summary>
        private Object locker = new Object();

        /// <summary>An array of booleans indicating for which ranges the work has finished.
        /// The booleans are stored as follows: workFinished[range.Begin / WORK_PER_THREAD] = true (finished) or false (unfinished)</summary>
        private bool[] workFinished;

        /// <summary>Starts calculating the mandelnumbers for the given area of the mandelbrot set.</summary>
        /// <param name="centerX">The x-coordinate around which the area that is to be calculated is centred</param>
        /// <param name="centerY">The y-coordinate around which the area that is to be calculated is centred</param>
        /// <param name="scale">The scale of each pixel</param>
        /// <param name="maxIterations">The maximum amount of iterations before a point is considered to be part of the mandelbrot set</param>
        /// <param name="pxWidth">The amount of horizontal pixels</param>
        /// <param name="pxHeight">The amount of vertical pixels</param>
        public void Calculate(double centerX, double centerY, double scale, int maxIterations, int pxWidth, int pxHeight)
        {
            lock(locker)
            {
                // Abort any old workers and create a new one
                Abort();
                worker = new Worker(centerX - scale * pxWidth / 2 + scale / 2, centerY - scale * pxHeight / 2 + scale / 2, scale, maxIterations, pxWidth, pxHeight);
                worker.OnWorkFinished = WorkFinished;

                // Reset `workFinished`
                workFinished = new bool[(int) Math.Ceiling(((double) pxWidth * pxHeight) / WORK_PER_THREAD)];
                for(int i = 0; i < workFinished.Length; ++i)
                    workFinished[i] = false;

                // Queue the threads
                for(int i = 0; i < pxWidth * pxHeight; i += WORK_PER_THREAD)
                    ThreadPool.QueueUserWorkItem(new WaitCallback(worker.Work), new Worker.WorkerArgs(i, Math.Min(i + WORK_PER_THREAD, pxWidth * pxHeight)));
            }
        }

        /// <summary>Aborts the currently running calculation, if one is running at the moment.
        /// Note that the currently running calculation will not report the results it has calculated so far.</summary>
        public void Abort()
        {
            lock(locker)
            {
                if(worker != null)
                {
                    worker.Abort();
                    worker = null;
                }
            }
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
        private void CallOnCalculationDone(int[] mandelNumbers)
        {
            if(onCalculationDoneHandler != null)
                BeginInvoke(onCalculationDoneHandler, new object[] { mandelNumbers });
        }

        /// <summary>Called when Worker.Work() finishes its work on a thread</summary>
        /// <param name="w">The worker that reports the results</param>
        /// <param name="args">The range of indexes for which the work is finished</param>
        private void WorkFinished(Worker w, Worker.WorkerArgs args)
        {
            lock(locker)
            {
                // If the worker has been aborted, we stop here
                if(w.Aborted) return;

                // Mark the work as done
                workFinished[args.Begin / WORK_PER_THREAD] = true;

                // Check if all work is done
                bool allWorkFinished = true;
                foreach(bool finished in workFinished)
                {
                    if(!finished)
                    {
                        allWorkFinished = false;
                        break;
                    }
                }

                // If all work is done, report so to the main thread
                if(allWorkFinished)
                {
                    CallOnCalculationDone(w.MandelNumbers);
                    worker = null;
                }
            }
        }

        /// <summary>This class does the actual calculating of the mandelnumbers</summary>
        private class Worker
        {
            /// <summary>The array in which the results are stored, the results are stored as follows: mandelNumbers[x + y * width] = mandelnumber for (x, y)</summary>
            private int[] mandelNumbers;
            /// <summary>The leftmost x-coordinate (in mandelbrot coordinates) of the area that is to be calculated</summary>
            private double left;
            /// <summary>The topmost y-coordinate (in mandelbrot coordinates) of the area that is to be calculated</summary>
            private double top;
            /// <summary>The scale of each pixel</summary>
            private double scale;
            /// <summary>The maximum amount of iterations before a point is considered to be part of the mandelbrot set</summary>
            private int maxIterations;
            /// <summary>The width of the area in pixels</summary>
            private int pxWidth;
            /// <summary>Whether or not we're aborted</summary>
            private bool aborted = false;

            /// <summary>Describes the half-open range [begin, end) of indexes in the mandelNumbers array that should be calculated by the Work() method</summary>
            public struct WorkerArgs
            {
                /// <summary>The left side of the half-open range (inclusive)</summary>
                private int begin;
                /// <summary>The right side of the half-open range (exclusive)</summary>
                private int end;

                /// <summary>Constructor</summary>
                /// <param name="b">The left side of the half-open range (inclusive)</param>
                /// <param name="e">The right side of the half-open range (exclusive)</param>
                public WorkerArgs(int b, int e)
                { begin = b; end = e; }

                /// <summary>The left side of the half-open range (inclusive)</summary>
                public int Begin { get { return begin; } }
                /// <summary>The right side of the half-open range (exclusive)</summary>
                public int End { get { return end; } }
            }

            /// <summary>Constructs a Worker to calculate the mandelnumbers for a certain area</summary>
            /// <param name="x">The leftmost x-coordinate (in mandelbrot coordinates) of the area that is to be calculated</param>
            /// <param name="y">The topmost y-coordinate (in mandelbrot coordinates) of the area that is to be calculated</param>
            /// <param name="s">The scale of each pixel</param>
            /// <param name="maxIter">The maximum amount of iterations before a point is considered to be part of the mandelbrot set</param>
            /// <param name="w">The width in pixels</param>
            /// <param name="h">The height in pixels</param>
            public Worker(double x, double y, double s, int maxIter, int w, int h)
            {
                mandelNumbers = new int[w * h];
                left = x;
                top = y;
                scale = s;
                maxIterations = maxIter;
                pxWidth = w;
            }

            /// <summary>Get the calculated mandel numbers</summary>
            public int[] MandelNumbers { get { return mandelNumbers; } }

            /// <summary>Aborts all work for this Worker</summary>
            public void Abort()
            { aborted = true; }

            /// <summary>Returns whether the work in this Worker has been aborted or not</summary>
            public bool Aborted { get { return aborted; } }

            /// <summary>Calculates a part of the mandelNumbers array</summary>
            /// <param name="args">A WorkerArgs instance describing which part of the mandelNumbers array should be calculated</param>
            public void Work(Object args)
            {
                // Convert `args` to a WorkerArgs that we can use
                WorkerArgs workerArgs = (WorkerArgs) args;

                // Go through all indexes that should be calculated
                for(int i = workerArgs.Begin; !aborted && i < workerArgs.End; ++i)
                {
                    // Calculate the real (`cx`) and imaginary (`cy`) part of the current pixel
                    double cx = left + (i % pxWidth) * scale;
                    double cy = top + (i / pxWidth) * scale;

                    // The formula of the mandelbrot is:
                    // Z(n+1) = Z(n)^2 + C, with Z and C both complex and Z(0) = 0 and C = the current pixel
                    // Since Z(1) = C, we can start there
                    int iter = 1;
                    double zx = cx;
                    double zy = cy;

                    // Calculate the mandelnumber for the current C
                    double zxTemp = 0.0;
                    while(!aborted && zx * zx + zy * zy <= 4 && iter < maxIterations)
                    {
                        zxTemp = zx;
                        zx = zx * zx - zy * zy + cx;
                        zy = 2 * zxTemp * zy + cy;
                        ++iter;
                    }

                    // Store the calculated mandelnumber
                    mandelNumbers[i] = iter;
                }

                // We're done calculating, notify our master
                if(!aborted)
                    CallOnWorkFinishedHandler(workerArgs);
            }

            /// <summary>The type of the method that is to be called when the Work() method finishes its work</summary>
            /// <param name="w">The worker that reports that the work has finished (i.e. `this`)</param>
            /// <param name="args">The range of indexes in the mandelNumbers array for which the work is finished</param>
            public delegate void OnWorkFinishedHandler(Worker w, WorkerArgs args);
            /// <summary>The handler that is to be called when the Work() method finishes its work</summary>
            private OnWorkFinishedHandler onWorkFinishedHandler;
            /// <summary>Property to set or get the current OnWorkFinishedHandler</summary>
            public OnWorkFinishedHandler OnWorkFinished
            {
                get { return onWorkFinishedHandler; }
                set { onWorkFinishedHandler = value; }
            }
            /// <summary>Calls the OnWorkFinishedHandler</summary>
            /// <param name="args">The range that is passed to the OnWorkFinishedHandler</param>
            private void CallOnWorkFinishedHandler(WorkerArgs args)
            {
                if(onWorkFinishedHandler != null)
                    onWorkFinishedHandler(this, args);
            }
        }
    }
}
