//---------------------------------------------------------------------
// <copyright file="IElevationMap.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation 2011. All rights reserved.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Research.Wwt.Sdk.Core
{
    /// <summary>
    /// Defines a mapping from coordinates in 2D-space (lat,long) to an elevation.
    /// </summary>
    public interface IElevationMap
    {
        /// <summary>
        /// Gets the elevation corresponding to the longitude and latitude.
        /// </summary>
        /// <param name="longitude">
        /// Longitude (X-axis) value.
        /// </param>
        /// <param name="latitude">
        /// Latitude (Y-axis) value.
        /// </param>
        /// <returns>
        /// Elevation value.
        /// </returns>
        short GetElevation(double longitude, double latitude);
    }
}
