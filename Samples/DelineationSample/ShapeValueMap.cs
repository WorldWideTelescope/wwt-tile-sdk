//-----------------------------------------------------------------------
// <copyright file="ShapeValueMap.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation 2011. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using Microsoft.Research.Wwt.Sdk.Core;
using Microsoft.Research.Wwt.Sdk.Utilities;

namespace Microsoft.Research.Wwt.Sdk.Samples
{
    /// <summary>
    /// Maps a goe-location (latitude and longitude) to a shape value.
    /// </summary>
    public class ShapeValueMap : IValueMap
    {
        /// <summary>
        ///  Modis validator.
        /// </summary>
        private IRegionMask validator;

        /// <summary>
        /// Initializes a new instance of the ShapeValueMap class.
        /// </summary>
        /// <param name="shapeFilePath">Shape file path.</param>
        /// <param name="regionHvmapPath">Region map file path.</param>
        /// <param name="shapeKey">Shape key used.</param>
        public ShapeValueMap(string shapeFilePath, string regionHvmapPath, string shapeKey)
        {
            if (string.IsNullOrWhiteSpace(shapeFilePath))
            {
                throw new ArgumentNullException("shapeFilePath");
            }

            if (string.IsNullOrWhiteSpace(regionHvmapPath))
            {
                throw new ArgumentNullException("regionHvmapPath");
            }

            this.validator = new RegionMask(shapeFilePath, regionHvmapPath, shapeKey);
        }

        /// <summary>
        /// Return a shape value for a given longitude and latitude value.
        /// </summary>
        /// <param name="longitude">Longitude value.</param>
        /// <param name="latitude">Latitude value.</param>
        /// <returns>Shape value in double.</returns>
        public double GetValueAt(double longitude, double latitude)
        {
            // Validate range of lat and lon
            if ((latitude < -90.0) || (latitude > 90.0))
            {
                return double.NaN;
            }

            if ((longitude < 0.0) || (longitude > 360.0))
            {
                return double.NaN;
            }

            // Offset longitude: [0; 360] -> [-180; 180]
            longitude -= 180.0;

            // Map geo-location to variable value
            double value = double.NaN;
            int[] hv = Helper.LatLonToHV(longitude, latitude);

            // Check if the geo-location is valid or not.
            value = (this.validator != null && this.validator.IsBound(longitude, latitude, hv[0], hv[1])) ? 1 : 0;

            return value;
        }
    }
}
