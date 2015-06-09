//-----------------------------------------------------------------------
// <copyright file="IRegionMask.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation 2011. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Microsoft.Research.Wwt.Sdk.Samples
{
    /// <summary>
    /// This interface defines methods for validating the latitude and longitude.
    /// </summary>
    public interface IRegionMask
    {
        /// <summary>
        /// Validates the given geo-location.
        /// </summary>
        /// <param name="longitude">Longitude in degrees (in [-180; 180] range).</param>
        /// <param name="latitude">Latitude in degrees (in [-90; 90] range).</param>
        /// <param name="horizontalAxis">The horizontal cell number.</param>
        /// <param name="verticalAxis">The vertical cell number.</param>
        /// <returns>Valid or not.</returns>
        bool IsBound(double longitude, double latitude, int horizontalAxis, int verticalAxis);
    }
}
