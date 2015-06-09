//-----------------------------------------------------------------------
// <copyright file="Constants.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation 2011. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Microsoft.Research.Wwt.Sdk.Core
{
    /// <summary>
    /// Constant values.
    /// </summary>
    public static class Constants
    {
        /// <summary>
        /// Size of each individual image tile.
        /// </summary>
        public const int TileSize = 256;

        /// <summary>
        /// Default image extension.
        /// </summary>
        public const string DefaultImageExtension = "png";

        /// <summary>
        /// Constant representing radius of the earth.
        /// </summary>
        public const double EarthRadius = 6378137;

        /// <summary>
        /// Default meter value per pixel.
        /// </summary>
        public const double BaseMetersPerPixel = 156543;

        /// <summary>
        /// Offset meter value.
        /// </summary>
        public const double OffsetMeters = 20037508;

        /// <summary>
        /// Minimum longitude value. 
        /// </summary>
        public const double MinimumMercatorLongitude = -180.0;

        /// <summary>
        /// Maximum longitude value.
        /// </summary>
        public const double MaximumMercatorLongitude = 180.0;

        /// <summary>
        /// Minimum latitude value. Used in reverse order.
        /// </summary>
        public const double MinimumMercatorLatitude = 85.05112878;

        /// <summary>
        /// Maximum latitude value. Used in reverse order.
        /// </summary>
        public const double MaximumMercatorLatitude = -85.05112878;

        /// <summary>
        /// Maximum H value.
        /// </summary>
        public const int MaxHvalue = 36;

        /// <summary>
        /// Maximum V value.
        /// </summary>
        public const int MaxVvalue = 18;

        /// <summary>
        /// Default Output Path.
        /// </summary>
        public const string DefaultOutputPath = "{0}\\Microsoft Research\\WWT SDK\\Output";

        /// <summary>
        /// Number of bytes used per pixel value.
        /// </summary>
        public const int BytesPerPixel = 4;

        /// <summary>
        /// Top level pyramid plate.
        /// </summary>
        public const string TopLevelPlate = "L0X0Y0.plate";

        /// <summary>
        /// Base level pyramid plate format.
        /// </summary>
        public const string BaseLevelPlateFormat = "L{0}X{1}Y{2}.plate";

        /// <summary>
        /// Top level DEM plate.
        /// </summary>
        public const string DEMTopLevelPlate = "DL0X0Y0.plate";

        /// <summary>
        /// Base level DEM plate format.
        /// </summary>
        public const string DEMBaseLevelPlateFormat = "DL{0}X{1}Y{2}.plate";

        /// <summary>
        /// WTML files search pattern.
        /// </summary>
        public const string WTMLSearchPattern = "*.wtml";

        /// <summary>
        /// Image set path.
        /// </summary>
        public const string ImageSetPath = "//ImageSet";

        /// <summary>
        /// WTML tile levels.
        /// </summary>
        public const string WTMLTileLevel = "TileLevels";
    }
}
