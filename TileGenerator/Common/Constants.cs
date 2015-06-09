//-----------------------------------------------------------------------
// <copyright file="Constants.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation 2011. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Microsoft.Research.Wwt.TileGenerator
{
    /// <summary>
    /// Constants class
    /// </summary>
    public static class Constants
    {
        /// <summary>
        /// Output folder structure.
        /// </summary>
        public const string OutputFolderStructure = "{0}_{1}_{2}";

        /// <summary>
        /// Output folder date format.
        /// </summary>
        public const string OutputFolderDateFormat = "yyyyddMM-HHmmss";

        /// <summary>
        /// Latitude max value
        /// </summary>
        public const double LatitudeMaxValue = 90;

        /// <summary>
        /// Latitude min value
        /// </summary>
        public const double LatitudeMinValue = -90;

        /// <summary>
        /// Longitude max value
        /// </summary>
        public const double LongitudeMaxValue = 180;

        /// <summary>
        /// Longitude min value
        /// </summary>
        public const double LongitudeMinValue = -180;

        /// <summary>
        /// Top Left Latitude
        /// </summary>
        public const double TopLeftLatitude = 90;

        /// <summary>
        /// Top Left Longitude
        /// </summary>
        public const double TopLeftLongitude = -180;

        /// <summary>
        /// Bottom Right Latitude
        /// </summary>
        public const double BottomRightLatitude = -90;

        /// <summary>
        /// Bottom Right Longitude
        /// </summary>
        public const double BottomRightLongitude = 180;

        /// <summary>
        /// Image jpg format.
        /// </summary>
        public const string JpgImageFormat = "jpg";

        /// <summary>
        /// Tif image format.
        /// </summary>
        public const string TifImageFormat = "tif";

        /// <summary>
        /// Timer interval.
        /// </summary>
        public const int TimerInterval = 500;

        /// <summary>
        /// Input image path
        /// </summary>
        public const string InputImagePath = "InputImagePath";

        /// <summary>
        /// Top left latitude
        /// </summary>
        public const string TopLeftLatitudeProp = "TopLeftLatitude";

        /// <summary>
        /// Top Left Longitude
        /// </summary>
        public const string TopLeftLongitudeProp = "TopLeftLongitude";

        /// <summary>
        /// Bottom Right Latitude
        /// </summary>
        public const string BottomRightLatitudeProp = "BottomRightLatitude";

        /// <summary>
        /// Bottom Right Longitude
        /// </summary>
        public const string BottomRightLongitudeProp = "BottomRightLongitude";

        /// <summary>
        /// Line break
        /// </summary>
        public const string LineBreak = "\r\n";

        /// <summary>
        /// Multiplier for thumbnail estimated time
        /// </summary>
        public const int ThumbnailMultiplier = 2;

        /// <summary>
        /// Processed tiles percentage
        /// </summary>
        public const int ProcessedTilesPercentage = 70;

        /// <summary>
        /// Tile time multiplier
        /// </summary>
        public const int TileTimeMultiplier = 2;
    }
}
