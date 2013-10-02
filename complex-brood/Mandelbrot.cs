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

        /// <summary>A dictionary of booleans indicating for which ranges the work has finished.
        /// The booleans are stored as follows: workFinished[range.Begin] = true (finished) or false (unfinished)</summary>
        private Dictionary<int, bool> workFinished;

        /// <summary>Starts calculating the mandelnumbers for the given area of the mandelbrot set.</summary>
        /// <param name="args">The area to calculate</param>
        public void Calculate(MandelAreaArgs args)
        {
            lock(locker)
            {
                // Abort any old workers
                Abort();

                // Make sure the x axis lies in the middel of a pixel
                args.CenterOnXAxis();

                // Create a new worker
                worker = new Worker(args);
                worker.OnWorkFinished = WorkFinished;

                // Determine which indexes should be calculated and which can be determined using the symmetry of the mandelbrot set
                int startIndex = 0;
                int endIndex = args.CalcArea.Width * args.CalcArea.Height;
                if(args.CalcAreaTop() > 0.0 && args.CalcAreaBottom() < 0.0)
                {
                    if(args.CalcAreaTop() >= -args.CalcAreaBottom())
                    {
                        // `endIndex` = `rows of pixels above the x-axis including the x-axis` * `width in pixels`
                        endIndex = (int) Math.Min(endIndex, Math.Round((args.CalcAreaTop() / args.Scale + 1) * args.CalcArea.Width));
                    }
                    else
                    {
                        // `startIndex` = `rows of pixels above the x-axis excluding the x-axis` * `width in pixels`
                        startIndex = (int) Math.Round((args.CalcAreaTop() / args.Scale) * args.CalcArea.Width);
                    }
                }

                // Queue the work items
                workFinished = new Dictionary<int, bool>((int) Math.Ceiling(((double) endIndex - startIndex) / WORK_PER_THREAD));
                for(int i = startIndex; i < endIndex; i += WORK_PER_THREAD)
                {
                    workFinished[i] = false;
                    ThreadPool.QueueUserWorkItem(new WaitCallback(worker.Work), new Worker.WorkerArgs(i, Math.Min(i + WORK_PER_THREAD, endIndex)));
                }
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
        /// <param name="args">The area for which the calculation was done</param>
        /// <param name="mandelNumbers">An array containing the mandelnumbers, stored as follows: mandelNumbers[x + y * width] = mandelnumber for (x, y)</param>
        public delegate void OnCalculationDoneHandler(MandelAreaArgs args, int[] mandelNumbers);
        /// <summary>The handler that is to be called when the calculation is done</summary>
        private OnCalculationDoneHandler onCalculationDoneHandler;
        /// <summary>Property to set or get the current OnCalculationDoneHandler</summary>
        public OnCalculationDoneHandler OnCalculationDone
        {
            get { return onCalculationDoneHandler; }
            set { onCalculationDoneHandler = value; }
        }
        /// <summary>Calls the OnCalculationDoneHandler</summary>
        /// <param name="args">The area for which the calculation was done that is to be passed to the OnCalculationDoneHandler</param>
        /// <param name="mandelNumbers">The array of mandelnumbers that is to passed to the OnCalculationDoneHandler</param>
        private void CallOnCalculationDone(MandelAreaArgs args, int[] mandelNumbers)
        {
            if(onCalculationDoneHandler != null)
                BeginInvoke(onCalculationDoneHandler, new object[] { args, mandelNumbers });
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
                workFinished[args.Begin] = true;

                // Check if all work is done
                bool allWorkFinished = true;
                foreach(bool finished in workFinished.Values)
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
                    CallOnCalculationDone(w.Args, w.MandelNumbers);
                    worker = null;
                }
            }
        }

        /// <summary>This class does the actual calculating of the mandelnumbers</summary>
        private class Worker
        {
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

            /// <summary>The array in which the results are stored, the results are stored as follows: mandelNumbers[x + y * width] = mandelnumber for (x, y).
            /// Note that here x, y are in pixel coordinates where x and y are relative to the top-left coordinates of `mandelAreaArgs.CalcArea`.
            /// Furthermore, width here is the same as `mandelAreaArgs.CalcArea.Width`</summary>
            private int[] mandelNumbers;
            /// <summary>The area that is to be calculated</summary>
            private MandelAreaArgs mandelAreaArgs;
            /// <summary>Whether or not we're aborted</summary>
            private bool aborted = false;

            /// <summary>Constructs a Worker to calculate the mandelnumbers for a certain area</summary>
            /// <param name="args">The area that is to be calculated</param>
            public Worker(MandelAreaArgs args)
            {
                mandelNumbers = new int[args.CalcArea.Width * args.CalcArea.Height];
                mandelAreaArgs = args;
            }

            /// <summary>Get the calculated mandel numbers</summary>
            public int[] MandelNumbers { get { return mandelNumbers; } }

            /// <summary>Aborts all work for this Worker</summary>
            public void Abort() { aborted = true; }

            /// <summary>Returns whether the work in this Worker has been aborted or not</summary>
            public bool Aborted { get { return aborted; } }

            /// <summary>Returns the MandelAreaArgs that was given to this Worker to calculate</summary>
            public MandelAreaArgs Args { get { return mandelAreaArgs; } }

            /// <summary>Calculates a part of the mandelNumbers array</summary>
            /// <param name="args">A WorkerArgs instance describing which part of the mandelNumbers array should be calculated</param>
            public void Work(Object args)
            {
                // Convert `args` to a WorkerArgs that we can use
                WorkerArgs workerArgs = (WorkerArgs) args;

                // Calculate the left and top coordinates in mandelbrot coordinates
                double left = mandelAreaArgs.CalcAreaLeft();
                double top = mandelAreaArgs.CalcAreaTop();

                // Go through all indexes that should be calculated
                for(int i = workerArgs.Begin; !aborted && i < workerArgs.End; ++i)
                {
                    // Calculate the real (`cx`) and imaginary (`cy`) part of the current pixel
                    double cx = left + (i % mandelAreaArgs.CalcArea.Width) * mandelAreaArgs.Scale;
                    double cy = top - (i / mandelAreaArgs.CalcArea.Width) * mandelAreaArgs.Scale;

                    // The formula of the mandelbrot is:
                    // Z(n+1) = Z(n)^2 + C, with Z and C both complex and Z(0) = 0 and C = the current pixel
                    // Since Z(1) = C, we can start there
                    int iter = 1;
                    double zx = cx;
                    double zy = cy;

                    // Calculate the mandelnumber for the current C
                    double zxTemp = 0.0;
                    while(!aborted && zx * zx + zy * zy <= 4 && iter < mandelAreaArgs.MaxIterations)
                    {
                        zxTemp = zx;
                        zx = zx * zx - zy * zy + cx;
                        zy = 2 * zxTemp * zy + cy;
                        ++iter;
                    }

                    // Store the calculated mandelnumber
                    mandelNumbers[i] = iter;

                    // Check if a symmetrical mandelnumber should be stored
                    if(cy != 0.0)
                    {
                        // pixelY = (top - -cy) / scale
                        int pixelY = (int) Math.Round((top + cy) / mandelAreaArgs.Scale);

                        // Calculate te symmetrical index
                        int symIndex = pixelY * mandelAreaArgs.CalcArea.Width + i % mandelAreaArgs.CalcArea.Width;

                        // Check if the symmetrical index is in range, and if it is we store the symmetrical mandelnumber
                        if(symIndex > 0 && symIndex < mandelNumbers.Length)
                            mandelNumbers[symIndex] = iter;
                    }
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
