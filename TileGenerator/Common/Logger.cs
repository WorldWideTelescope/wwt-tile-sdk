//-----------------------------------------------------------------------
// <copyright file="Logger.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation 2011. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
using System;
using System.Diagnostics;

namespace Microsoft.Research.Wwt.TileGenerator
{
    public static class Logger
    {
        /// <summary>
        /// The trace source instance.
        /// </summary>
        private static TraceSource tracesource = new TraceSource("TileGenerator");

        /// <summary>
        /// Constructs and logs the exception message.
        /// </summary>
        /// <param name="exception">The Exception object.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Consume any exception while logging.")]
        public static void LogException(Exception exception)
        {
            if (exception != null)
            {
                try
                {
                    string traceMessage = DateTime.Now + " : " + exception.Message;
                    if (exception.InnerException != null)
                    {
                        traceMessage += " : " + exception.InnerException.Message;
                    }

                    tracesource.TraceEvent(TraceEventType.Error, exception.GetHashCode(), traceMessage);
                }
                catch (Exception)
                {
                    // Consume any exception while logging as it cannot be logged any more.
                }
            }
        }
    }
}
