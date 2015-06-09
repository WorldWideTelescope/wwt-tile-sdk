//-----------------------------------------------------------------------
// <copyright file="IColorMap.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation 2011. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Drawing;

namespace Microsoft.Research.Wwt.Sdk.Core
{
    /// <summary>
    /// Map used to generate color pixels corresponding to longitude and latitude coordinates.
    /// </summary>
    public interface IColorMap
    {
        /// <summary>
        /// Gets color corresponding to the longitude and latitude.
        /// </summary>
        /// <param name="longitude">
        /// Longitude (X-axis) value.
        /// </param>
        /// <param name="latitude">
        /// Latitude (Y-axis) value.
        /// </param>
        /// <returns>
        /// Color pixel.
        /// </returns>
        Color GetColor(double longitude, double latitude);
    }
}
