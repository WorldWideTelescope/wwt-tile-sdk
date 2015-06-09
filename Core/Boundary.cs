//---------------------------------------------------------------------------
// <copyright file="Boundary.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation 2011. All rights reserved.
// </copyright>
//---------------------------------------------------------------------------

namespace Microsoft.Research.Wwt.Sdk.Core
{
    /// <summary>
    /// Class which holds bounding co-ordinates.
    /// </summary>
    public class Boundary
    {
        /// <summary>
        /// Initializes a new instance of the Boundary class.
        /// </summary>
        /// <param name="left">Left position.</param>
        /// <param name="top">Top position.</param>
        /// <param name="right">Right position.</param>
        /// <param name="bottom">Bottom position.</param>
        public Boundary(double left, double top, double right, double bottom)
        {
            this.Left = left;
            this.Right = right;
            this.Top = top;
            this.Bottom = bottom;
        }

        /// <summary>
        /// Gets or sets the left position.
        /// </summary>
        public double Left { get; set; }
       
        /// <summary>
        /// Gets or sets the Top position.
        /// </summary>
        public double Top { get; set; }

        /// <summary>
        /// Gets or sets the Right position.
        /// </summary>
        public double Right { get; set; }
       
        /// <summary>
        /// Gets or sets the Bottom position.
        /// </summary>
        public double Bottom { get; set; }
    }
}
