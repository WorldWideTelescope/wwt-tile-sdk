//-----------------------------------------------------------------------
// <copyright file="IProjectionGridMap.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation 2011. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Microsoft.Research.Wwt.Sdk.Core
{
    /// <summary>
    /// Represents a functionality which will map latitude and longitude values into UV or XY co-ordinates of the grid 
    /// based on the projection. The interface abstracts the projection conversion logic required to convert longitude
    /// and latitude values to UV or XY co-ordinates of the grid.
    /// </summary>
    public interface IProjectionGridMap
    {
        /// <summary>
        /// Gets grid boundary for the projection map
        /// </summary>
        Boundary InputBoundary { get; }

        /// <summary>
        /// Gets source grid for the projection map
        /// </summary>
        IGrid InputGrid { get; }

        /// <summary>
        /// Gets the value contained at the specified longitude and latitude.
        /// </summary>
        /// <param name="longitude">
        /// Longitude value.
        /// </param>
        /// <param name="latitude">
        /// Latitude value.
        /// </param>
        /// <returns>
        /// Value contained at the specified longitude and latitude.
        /// </returns>
        double GetValue(double longitude, double latitude);

        /// <summary>
        /// Gets the index of longitude along X-axis.
        /// </summary>
        /// <param name="longitude">
        /// Longitude value.
        /// </param>
        /// <returns>
        /// Index along X-axis.
        /// </returns>
        int GetXIndex(double longitude);

        /// <summary>
        /// Gets the index of longitude along Y-axis.
        /// </summary>
        /// <param name="latitude">
        /// Latitude value.
        /// </param>
        /// <returns>
        /// Index along Y-axis.
        /// </returns>
        int GetYIndex(double latitude);

        /// <summary>
        /// Checks if the given longitude-latitude pair is within the range of this dataset.
        /// </summary>
        /// <param name="longitude">
        /// Longitude value.
        /// </param>
        /// <param name="latitude">
        /// Latitude value.
        /// </param>
        /// <returns>
        /// True if the given longitude-latitude pair is within the range of this dataset, False otherwise.
        /// </returns>
        bool IsInRange(double longitude, double latitude);
    }
}
