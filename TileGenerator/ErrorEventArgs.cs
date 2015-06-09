// <copyright file="ErrorEventArgs.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation 2010. All rights reserved.
// </copyright>
//---------------------------------------------------------------------------

using System;

namespace Microsoft.Research.Wwt.TileGenerator
{
    /// <summary>
    /// This class defines the properties for error event args.
    /// </summary>
    public class ErrorEventArgs : EventArgs
    {
        /// <summary>
        /// Gets or sets the error of the operation. 
        /// </summary>
        public Exception Error { get; set; }
    }
}
