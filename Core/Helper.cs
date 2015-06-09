//-----------------------------------------------------------------------
// <copyright file="Helper.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation 2011. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;

namespace Microsoft.Research.Wwt.Sdk.Core
{
    /// <summary>
    /// SDK helper class with utility methods.
    /// </summary>
    public static class Helper
    {
        /// <summary>
        /// Minimum latitude value.
        /// </summary>
        private const double MinLatitude = -90;

        /// <summary>
        /// Maximum latitude value.
        /// </summary>
        private const double MaxLatitude = 90;

        /// <summary>
        /// Minimum longitude value. 
        /// </summary>
        private const double MinLongitude = -180;

        /// <summary>
        /// Maximum longitude value.
        /// </summary>
        private const double MaxLongitude = 180;

        /// <summary>
        /// Computes the X tile for given longitude and zoom level.
        /// </summary>
        /// <param name="longitude">
        /// Longitude value.
        /// </param>
        /// <param name="level">
        /// Zoom level.
        /// </param>
        /// <returns>
        /// X tile computed.
        /// </returns>
        /// <remarks>
        /// Formula picked up from http://msdn.microsoft.com/en-us/library/bb259689.aspx.
        /// </remarks>
        public static int GetMercatorXTileFromLongitude(double longitude, int level)
        {
            longitude = Math.Max(Constants.MinimumMercatorLongitude, longitude);
            longitude = Math.Min(Constants.MaximumMercatorLongitude, longitude);
            return (int)(((longitude + 180) / 360) * Math.Pow(2, level));
        }

        /// <summary>
        /// Computes the Y tile for given latitude and zoom level.
        /// </summary>
        /// <param name="latitude">
        /// Latitude value.
        /// </param>
        /// <param name="level">
        /// Zoom level.
        /// </param>
        /// <returns>
        /// Y tile computed.
        /// </returns>
        /// <remarks>
        /// Formula picked up from http://msdn.microsoft.com/en-us/library/bb259689.aspx.
        /// </remarks>
        public static int GetMercatorYTileFromLatitude(double latitude, int level)
        {
            latitude = Math.Max(Constants.MaximumMercatorLatitude, latitude);
            latitude = Math.Min(Constants.MinimumMercatorLatitude, latitude);
            double sinLat = Math.Sin(Helper.DegreesToRadians(latitude));
            double value = Math.Log((1 + sinLat) / (1 - sinLat));
            value = 0.5 - (value / (4 * Math.PI));
            return (int)(value * Math.Pow(2, level));
        }

        /// <summary>
        /// Computes the absolute meter value of the latitude for a specified zoom value.
        /// </summary>
        /// <param name="meters">
        /// The relative latitude meter value within a tile.
        /// </param>
        /// <param name="level">
        /// Current zoom level.
        /// </param>
        /// <returns>Absolute meter value.</returns>
        public static double AbsoluteMetersToLatitudeAtZoom(int meters, int level)
        {
            double metersPerPixel = Helper.MetersPerPixel(level);
            double metersY = (double)Constants.OffsetMeters - (double)meters * metersPerPixel;

            return (Helper.RadiansToDegrees(Math.PI / 2 - 2 * Math.Atan(Math.Exp(0 - metersY / Constants.EarthRadius))));
        }

        /// <summary>
        /// Converts degree value into radian.
        /// </summary>
        /// <param name="degrees">
        /// Value in angle.
        /// </param>
        /// <returns>
        /// Converted radian value in double.
        /// </returns>
        public static double DegreesToRadians(double degrees)
        {
            return (degrees * Math.PI / 180.0);
        }

        /// <summary>
        /// Converts radian value into angle.
        /// </summary>
        /// <param name="rad">
        /// Value in radian.
        /// </param>
        /// <returns>
        /// Converted degree value.
        /// </returns>
        public static double RadiansToDegrees(double rad)
        {
            return (rad * 180.0 / Math.PI);
        }

        /// <summary>
        /// Calculates number of meters denoted by a single pixel at specified zoom level.
        /// </summary>
        /// <param name="zoom">
        /// Zoom level for which the computation has to be carried out.
        /// </param>
        /// <returns>
        /// Number of meters in floating point value.
        /// </returns>
        public static double MetersPerPixel(int zoom)
        {
            return (Constants.BaseMetersPerPixel / (double)(1 << zoom));
        }

        /// <summary>
        /// Converts the latitude and longitude value to H V coordinates.
        /// </summary>
        /// <param name="lon">
        /// Longitude value.
        /// </param>
        /// <param name="lat">
        /// Latitude value.
        /// </param>
        /// <returns>
        /// Array holding H and V coordinates.
        /// </returns>
        public static int[] LatLonToHV(double lon, double lat)
        {
            double[] xy = LatLonToSinXY(lon, lat);
            double w = 10.0; // size of a cell in degrees.
            double h = Math.Floor((xy[0] + 180.0) / w);
            double v = Math.Floor((90.0 - xy[1]) / w);
            return new int[] { (int)h, (int)v };
        }

        /// <summary>
        /// Determines the map width and height (in pixels) at a specified level
        /// of detail.
        /// </summary>
        /// <param name="levelOfDetail">
        /// Level of detail, from 1 (lowest detail) to 23 (highest detail).
        /// </param>
        /// <returns>
        /// The map width and height in pixels.
        /// </returns>
        public static uint MapSize(int levelOfDetail)
        {
            return (uint)256 << levelOfDetail;
        }

        /// <summary>
        /// Converts a point from latitude/longitude WGS-84 coordinates (in degrees)
        /// into pixel XY coordinates at a specified level of detail.
        /// </summary>
        /// <param name="latitude">
        /// Latitude of the point, in degrees.
        /// </param>
        /// <param name="longitude">
        /// Longitude of the point, in degrees.
        /// </param>
        /// <param name="levelOfDetail">
        /// Level of detail, from 1 (lowest detail) to 23 (highest detail).
        /// </param>
        /// <param name="pixelX">
        /// Output parameter receiving the X coordinate in pixels.
        /// </param>
        /// <param name="pixelY">
        /// Output parameter receiving the Y coordinate in pixels.
        /// </param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "3#", Justification = "We need to return 2 variables.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "4#", Justification = "We need to return 2 variables.")]
        public static void LatLongToPixelXY(double latitude, double longitude, int levelOfDetail, out int pixelX, out int pixelY)
        {
            latitude = Clip(latitude, MinLatitude, MaxLatitude);
            longitude = Clip(longitude, MinLongitude, MaxLongitude);

            double x = (longitude + 180) / 360;
            double sinLatitude = Math.Sin(latitude * Math.PI / 180);
            double y = 0.5 - Math.Log((1 + sinLatitude) / (1 - sinLatitude)) / (4 * Math.PI);

            uint mapSize = MapSize(levelOfDetail);
            pixelX = (int)Clip(x * mapSize + 0.5, 0, mapSize - 1);
            pixelY = (int)Clip(y * mapSize + 0.5, 0, mapSize - 1);
        }

        /// <summary>
        /// Clips a number to the specified minimum and maximum values.
        /// </summary>
        /// <param name="n">
        /// The number to clip.
        /// </param>
        /// <param name="minValue">
        /// Minimum allowable value.
        /// </param>
        /// <param name="maxValue">
        /// Maximum allowable value.
        /// </param>
        /// <returns>
        /// The clipped value.
        /// </returns>
        private static double Clip(double n, double minValue, double maxValue)
        {
            return Math.Min(Math.Max(n, minValue), maxValue);
        }

        /// <summary>
        /// Compute sinusoidal projection of point with given latitude and longitude.
        /// </summary>
        /// <param name="lon">
        /// Longitude in degrees.
        /// </param>
        /// <param name="lat">
        /// Latitude in degrees.
        /// </param>
        /// <returns>
        /// Coordinates(x,y) as a double array.
        /// </returns>
        private static double[] LatLonToSinXY(double lon, double lat)
        {
            double x = lon * Math.Cos(DegreesToRadians(lat));
            double y = lat;
            return new double[] { x, y };
        }
    }
}