//-----------------------------------------------------------------------
// <copyright file="IGrid.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation 2011. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Microsoft.Research.Wwt.Sdk.Core
{
    /// <summary>
    /// Represents the data grid for the source data. The interface abstracts the source data and defines API that 
    /// can return the value when provided with UV co-ordinates or XY co-ordinates.
    /// </summary>
    public interface IGrid
    {
        /// <summary>
        /// Gets the Height for the grid
        /// </summary>
        int Height { get; }

        /// <summary>
        /// Gets the Width for the grid
        /// </summary>
        int Width { get; }

        /// <summary>
        /// Gets the value for the specified u and v co-ordinates in the grid.
        /// </summary>
        /// <param name="u">
        /// U co-ordinate.
        /// </param>
        /// <param name="v">
        /// V co-ordinate.
        /// </param>
        /// <returns>
        /// Value for the specified u and v co-ordinates in the grid.
        /// </returns>
        double GetValue(double u, double v);

        /// <summary>
        /// Gets the value contained at the specified i and j index.
        /// </summary>
        /// <param name="i">
        /// Index along X-axis.
        /// </param>
        /// <param name="j">
        /// Index along Y-axis.
        /// </param>
        /// <returns>
        /// Value contained at the specified i and j index.
        /// </returns>
        double GetValueAt(int i, int j);

        /// <summary>
        /// Gets the X index value for the specified u co-ordinate in the grid
        /// </summary>
        /// <param name="u">
        /// U co-ordinate
        /// </param>
        /// <returns>
        /// Index along X-axis
        /// </returns>
        int GetXIndex(double u);

        /// <summary>
        /// Gets the Y index value for the specified v co-ordinate in the grid
        /// </summary>
        /// <param name="v">
        /// V co-ordinate
        /// e</param>
        /// <returns>
        /// Index along Y axis
        /// </returns>
        int GetYIndex(double v);
    }
}
