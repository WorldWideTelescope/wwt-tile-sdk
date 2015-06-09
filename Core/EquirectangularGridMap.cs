//-----------------------------------------------------------------------
// <copyright file="EquirectangularGridMap.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation 2011. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;

namespace Microsoft.Research.Wwt.Sdk.Core
{
    /// <summary>
    /// Represents a functionality which will map latitude and longitude values into 
    /// UV or XY co-ordinates of the grid based on Equirectangular projection.
    /// </summary>
    public class EquirectangularGridMap : IProjectionGridMap
    {
        /// <summary>
        /// Gets minimum longitude.
        /// </summary>
        private double minimumLongitude;

        /// <summary>
        /// Gets maximum latitude.
        /// </summary>
        private double maximumLatitude;

        /// <summary>
        /// Gets longitude delta.
        /// </summary>
        private double longitudeDelta;

        /// <summary>
        /// Gets latitude delta.
        /// </summary>
        private double latitudeDelta;

        /// <summary>
        /// Initializes a new instance of the EquirectangularGridMap class.
        /// </summary>
        /// <param name="inputGrid">
        /// Input grid.
        /// </param>
        /// <param name="boundary">
        /// Input boundary co-ordinates
        /// </param>
        public EquirectangularGridMap(IGrid inputGrid, Boundary boundary)
        {
            if (inputGrid == null)
            {
                throw new ArgumentNullException("inputGrid");
            }

            if (boundary == null)
            {
                throw new ArgumentNullException("boundary");
            }

            longitudeDelta = boundary.Right - boundary.Left;
            if (longitudeDelta > 360.0)
            {
                throw new ArgumentException("Longitude range must be less than 360 degrees.", "boundary");
            }

            if (longitudeDelta <= 0.0)
            {
                throw new ArgumentException("Longitudes must be increasing from left to right.", "boundary");
            }

            latitudeDelta = boundary.Top - boundary.Bottom;
            if (latitudeDelta >= 0.0)
            {
                throw new ArgumentException("Latitudes must be increasing from top to bottom.", "boundary");
            }

            if (latitudeDelta < -180.0)
            {
                throw new ArgumentException("Latitude range must be less than 180 degrees.", "boundary");
            }

            this.minimumLongitude = boundary.Left;
            this.maximumLatitude = boundary.Bottom;
            this.InputGrid = inputGrid;
            this.InputBoundary = boundary;
        }

        /// <summary>
        /// Gets the Input grid used as a source for Grid map.
        /// </summary>
        public IGrid InputGrid { get; private set; }

        /// <summary>
        /// Gets the input boundary.
        /// </summary>
        public Boundary InputBoundary { get; private set; }

        /// <summary>
        /// Gets the value corresponding to the longitude and latitude.
        /// </summary>
        /// <param name="longitude">
        /// Longitude (X-axis) value.
        /// </param>
        /// <param name="latitude">
        /// Latitude (Y-axis) value.
        /// </param>
        /// <returns>
        /// Value corresponding to the longitude and latitude.
        /// </returns>
        public double GetValue(double longitude, double latitude)
        {
            if (this.IsInRange(longitude, latitude))
            {
                double u = (longitude - minimumLongitude) / longitudeDelta;
                double v = (latitude - maximumLatitude) / latitudeDelta;
                return this.InputGrid.GetValue(u, v);
            }

            return double.NaN;
        }

        /// <summary>
        /// Gets the index of longitude along X-axis.
        /// </summary>
        /// <param name="longitude">
        /// Longitude (X-axis) value.
        /// </param>
        /// <returns>
        /// Index along X-axis.
        /// </returns>
        public int GetXIndex(double longitude)
        {
            return this.InputGrid.GetXIndex(((longitude - minimumLongitude) / longitudeDelta));
        }

        /// <summary>
        /// Gets the index of latitude along Y-axis.
        /// </summary>
        /// <param name="latitude">
        /// latitude (Y-axis) value.
        /// </param>
        /// <returns>
        /// Index along Y-axis.
        /// </returns>
        public int GetYIndex(double latitude)
        {
            return this.InputGrid.GetYIndex((latitude - maximumLatitude) / latitudeDelta);
        }

        /// <summary>
        /// Checks if the given longitude-latitude pair is within the range of this dataset based on
        /// the projection
        /// </summary>
        /// <param name="longitude">
        /// Longitude (X-axis) value.
        /// </param>
        /// <param name="latitude">
        /// Latitude (Y-axis) value.
        /// </param>
        /// <returns>
        /// True if the given longitude-latitude pair is within the range of this dataset, False otherwise.
        /// </returns>
        public bool IsInRange(double longitude, double latitude)
        {
            if (longitude < this.InputBoundary.Left || longitude > this.InputBoundary.Right || latitude < this.InputBoundary.Top || latitude > this.InputBoundary.Bottom)
            {
                return false;
            }

            return true;
        }
    }
}
