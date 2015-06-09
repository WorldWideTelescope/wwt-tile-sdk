//-----------------------------------------------------------------------
// <copyright file="ErrorHandler.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation 2011. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.Research.Wwt.SharingService.Web
{
    /// <summary>
    /// Class is used to log exception in the log file.
    /// </summary>
    public static class ErrorHandler
    {
        /// <summary>
        /// The trace source instance.
        /// </summary>
        private static TraceSource tracesource = new TraceSource("TileService");

        /// <summary>
        /// Constructs and logs the exception message.
        /// </summary>
        /// <param name="exception">The general exception object.</param>        
        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Consume any exception while logging.")]
        internal static void LogException(Exception exception)
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
