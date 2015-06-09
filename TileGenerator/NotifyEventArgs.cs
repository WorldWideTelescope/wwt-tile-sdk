// <copyright file="NotifyEventArgs.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation 2010. All rights reserved.
// </copyright>
//---------------------------------------------------------------------------

using System;
namespace Microsoft.Research.Wwt.TileGenerator
{
    /// <summary>
    /// This class holds all details which we need to notify.
    /// </summary>
    public class NotifyEventArgs : EventArgs
    {
        /// <summary>
        /// Gets or sets the step details.
        /// </summary>
        public PyramidGenerationSteps Step { get; set; }

        /// <summary>
        /// Gets or sets the status of the step.
        /// </summary>
        public PyramidGenerationStatus Status { get; set; }
    }
}
