//-----------------------------------------------------------------------
// <copyright file="DataGridDetails.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation 2011. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using Microsoft.Research.Wwt.Sdk.Core;

namespace Microsoft.Research.Wwt.Sdk.Samples
{
    /// <summary>
    /// Stores data grid details.
    /// </summary>
    public class DataGridDetails
    {
        /// <summary>
        /// Initializes a new instance of the DataGridDetails class.
        /// </summary>
        public DataGridDetails(double[][] data, int gridWidth, int gridHeight, Boundary boundary, double minimumThreshold, double maximumThreshold)
        {
            this.Data = data;
            this.Height = gridHeight;
            this.Width = gridWidth;
            this.Boundary = boundary;
            this.MinimumThreshold = minimumThreshold;
            this.MaximumThreshold = maximumThreshold;
        }

        /// <summary>
        /// Gets or sets grid data.
        /// </summary>
        public double[][] Data { get; set; }

        /// <summary>
        /// Gets or sets input grid boundary.
        /// </summary>
        public Boundary Boundary { get; set; }

        /// <summary>
        /// Gets or sets the Minimum threshold value.
        /// </summary>
        public double MinimumThreshold { get; set; }

        /// <summary>
        /// Gets or sets the maximum threshold value.
        /// </summary>
        public double MaximumThreshold { get; set; }

        /// <summary>
        /// Gets or sets the grid width.
        /// </summary>
        public int Width { get; set; }

        /// <summary>
        /// Gets or sets the grid height.
        /// </summary>
        public int Height { get; set; }
    }
}
