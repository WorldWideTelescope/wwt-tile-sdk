//-----------------------------------------------------------------------
// <copyright file="ImageColorMap.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation 2011. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Microsoft.Research.Wwt.Sdk.Core
{
    using System;
    using System.Drawing;

    /// <summary>
    /// Implements the IColorMap interface for the image data type by taking a dependency on the IProjectionGridMap
    /// </summary>
    public class ImageColorMap : IColorMap
    {
        /// <summary>
        /// Grid map for the specified projection.
        /// </summary>
        private IProjectionGridMap projectionGridMap;

        /// <summary>
        /// Initializes a new instance of the ImageColorMap class.
        /// </summary>
        /// <param name="projectionGridMap">projection grid map.</param>
        public ImageColorMap(IProjectionGridMap projectionGridMap)
        {
            if (projectionGridMap == null)
            {
                throw new ArgumentNullException("projectionGridMap");
            }

            this.projectionGridMap = projectionGridMap;
        }

        /// <summary>
        /// Gets the pixel color at specified longitude and latitude values.
        /// This will use projection grid map to get the color value
        /// </summary>
        /// <param name="longitude">Longitude value for which the pixel color has to be retrieved.</param>
        /// <param name="latitude">Latitude value for which the pixel color has to be retrieved.</param>
        /// <returns>Return the color of the pixel.</returns>
        public Color GetColor(double longitude, double latitude)
        {
            double value = this.projectionGridMap.GetValue(longitude, latitude);
            if (double.IsNaN(value))
            {
                return Color.Transparent;
            }

            return Color.FromArgb(Convert.ToInt32(value));
        }
    }
}
