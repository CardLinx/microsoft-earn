//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
// // <summary>
//   The error rate based logger.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Lomo.Logging
{
    using System;
    using System.Collections.Generic;
    using System.Threading;

    /// <summary>
    /// The error rate based logger.
    /// </summary>
    public class OperationRateLogger
    {
        #region private members 

        /// <summary>
        /// The min error count.
        /// </summary>
        private readonly int minErrorCount;

        /// <summary>
        /// The min failure rate.
        /// </summary>
        private readonly double minFailureRate;

        /// <summary>
        /// The measurement event.
        /// </summary>
        private readonly Action<long, long> measurementEvent;

        /// <summary>
        /// The window timer.
        /// </summary>
        private readonly Timer windowTimer;

        /// <summary>
        /// The slices queue.
        /// </summary>
        private readonly Queue<WindowSlice> slicesQueue = new Queue<WindowSlice>();

        /// <summary>
        /// The current slice error count.
        /// </summary>
        private long currentSliceErrorCount;

        /// <summary>
        /// The current slice success count.
        /// </summary>
        private long currentSliceSuccessCount;

        /// <summary>
        /// The error count.
        /// </summary>
        private long partialErrorCount;

        /// <summary>
        /// The success count.
        /// </summary>
        private long partialSuccessCount;

        #endregion

        #region Constructor
        
        /// <summary>
        /// Initializes a new instance of the <see cref="OperationRateLogger"/> class.
        /// </summary>
        /// <param name="measurementWindow"> The measurement window. </param>
        /// <param name="minErrorCount"> The min error count before rising an error.
        /// </param> <param name="minFailureRate"> The min failure rate before rising an error. </param>
        /// <param name="measurementEvent">a callback event for listeners for measurement operation. this action should be very fast!!!</param>
        /// <exception cref="ArgumentNullException">measurement windows or minElapsedTime are null </exception>
        /// <exception cref="ArgumentException"> minElapsedTime is greater then measurementWindow
        /// </exception>
        public OperationRateLogger(TimeSpan measurementWindow, int minErrorCount = 10, double minFailureRate = 0.001, Action<long, long> measurementEvent = null)
        {
            if (measurementWindow == null)
            {
                throw new ArgumentNullException("measurementWindow");
            }

            if (measurementWindow < TimeSpan.FromMinutes(10) || measurementWindow > TimeSpan.FromDays(1))
            {
                throw new ArgumentException("measurment windows must be between 10 minutes to 24 hours", "measurementWindow");
            }

            // Calculate slice window
            var sliceWindow = new TimeSpan(measurementWindow.Ticks / 60);
            
            if (sliceWindow < TimeSpan.FromMinutes(1))
            {
                sliceWindow = TimeSpan.FromMinutes(1);
            }
            
            // calculate number of slices
            var slicesNum = (int)Math.Ceiling((double)measurementWindow.Ticks / sliceWindow.Ticks);
            
            // Populate the queue with empty slices
            for (int i = 0; i < slicesNum; i++)
            {
                this.slicesQueue.Enqueue(new WindowSlice { ErrorCount = 0, SuccessCount = 0 });
            }

            this.minErrorCount = minErrorCount;
            this.minFailureRate = minFailureRate;
            this.measurementEvent = measurementEvent;
            this.windowTimer = new Timer(this.WindowTimerElapsed, null, TimeSpan.Zero, sliceWindow);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the error count.
        /// </summary>
        private long ErrorCount
        {
            get
            {
                return this.partialErrorCount + this.currentSliceErrorCount;
            }
        }

        /// <summary>
        /// Gets the success count.
        /// </summary>
        private long SuccessCount
        {
            get
            {
                return this.partialSuccessCount + this.currentSliceSuccessCount;
            }
        }

        #endregion

        /// <summary>
        /// The success.
        /// </summary>
        public void Success()
        {
            Interlocked.Increment(ref this.currentSliceSuccessCount);
            this.RaiseCallback(this.SuccessCount, this.ErrorCount);
        }

        /// <summary> operation Failure log. If the allowed thresholds passed a error log  will be written otherwise an information log.</summary>
        /// <param name="eventId"> event id  </param>
        /// <param name="message">
        /// The message.
        /// </param>
        public void Failure(int eventId, string message)
        {
            this.Failure(eventId, null, message);
        }

        /// <summary> operation Failure log. If the allowed thresholds passed a error log  will be written otherwise an information log.</summary>
        /// <param name="eventId"> The event id. </param>
        /// <param name="exception"> The exception. </param>
        /// <param name="message"> The message. </param>
        public void Failure(int eventId, Exception exception, string message)
        {
            Interlocked.Increment(ref this.currentSliceErrorCount);

            long tempErrorCount = this.ErrorCount;
            long tempSuccessCount = this.SuccessCount;

            double faliureRate = tempErrorCount == 0 ? 0 : ((double)this.ErrorCount / (tempSuccessCount + tempErrorCount));

            string msgNew = string.Format("{0}; ErrorCount={1}; ErrorRate={2};", message, tempErrorCount, faliureRate);

            if (tempErrorCount >= this.minErrorCount && faliureRate >= this.minFailureRate)
            {
                Log.Error(eventId, exception, msgNew);
            }
            else
            {
                msgNew = string.Format("{0};Error={1}", msgNew, exception);
                Log.Info(eventId, msgNew);
            }

            this.RaiseCallback(tempSuccessCount, tempErrorCount);
        }

        /// <summary>
        /// The measurement callback.
        /// </summary>
        /// <param name="successCount">
        /// The success Count.
        /// </param>
        /// <param name="errorCount">
        /// The error Count.
        /// </param>
        private void RaiseCallback(long successCount, long errorCount)
        {
            if (this.measurementEvent != null)
            {
                this.measurementEvent(errorCount, errorCount + successCount);
            }
        }

        /// <summary>
        /// The window timer elapsed.
        /// </summary>
        /// <param name="state">
        /// The sender.
        /// </param>
        private void WindowTimerElapsed(object state)
        {
            long tempSliceErrorCount = Interlocked.Exchange(ref this.currentSliceErrorCount, 0);
            long tempSliceSuccessCount = Interlocked.Exchange(ref this.currentSliceSuccessCount, 0);
            this.partialSuccessCount += tempSliceSuccessCount;
            this.partialErrorCount += tempSliceErrorCount;

            var sliceToAdd = new WindowSlice
                                         {
                                             ErrorCount = tempSliceErrorCount,
                                             SuccessCount = tempSliceSuccessCount
                                         };
            this.slicesQueue.Enqueue(sliceToAdd);
            WindowSlice sliceToRemove = this.slicesQueue.Dequeue();
            this.partialErrorCount -= sliceToRemove.ErrorCount;
            this.partialSuccessCount -= sliceToRemove.SuccessCount;
        }

        /// <summary>
        /// The window slice.
        /// </summary>
        public class WindowSlice
        {
            /// <summary>
            /// Gets or sets the success count.
            /// </summary>
            public long SuccessCount { get; set; }

            /// <summary>
            /// Gets or sets the error count.
            /// </summary>
            public long ErrorCount { get; set; }
        }
    }
}