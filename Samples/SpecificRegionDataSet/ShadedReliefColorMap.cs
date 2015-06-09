//----------------------------------------------------------------------------
// <copyright file="ShadedReliefColorMap.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation 2011. All rights reserved.
// </copyright>
//----------------------------------------------------------------------------

using System;
using System.Drawing;
using Microsoft.Research.Wwt.Sdk.Core;

namespace Microsoft.Research.Wwt.Sdk.Samples
{
    /// <summary>
    /// A color map with relief shading for equirectangular data.
    /// </summary>
    public class ShadedReliefColorMap : IColorMap, IElevationMap
    {
        /// <summary>
        /// Equirectangular projection grid map.
        /// </summary>
        private IProjectionGridMap projectionGridMap;

        /// <summary>
        /// Initializes a new instance of the ShadedReliefColorMap class.
        /// </summary>
        /// <param name="projectionGridMap">Input image grid.</param>
        public ShadedReliefColorMap(IProjectionGridMap projectionGridMap)
        {
            if (projectionGridMap == null)
            {
                throw new ArgumentNullException("projectionGridMap");
            }

            this.projectionGridMap = projectionGridMap;
        }

        /// <summary>
        /// Gets the pixel color at specified longitude and latitude values.
        /// </summary>
        /// <param name="longitude">Longitude value for which the pixel color has to be retrieved.</param>
        /// <param name="latitude">Latitude value for which the pixel color has to be retrieved.</param>
        /// <returns>Return the color of the pixel.</returns>
        public Color GetColor(double longitude, double latitude)
        {
            double reliefShadedValue = double.NaN;
            if (this.projectionGridMap.IsInRange(longitude, latitude))
            {
                // Get i and j index for the given longitude-latitude pair.
                int i = this.projectionGridMap.GetXIndex(longitude);
                int j = this.projectionGridMap.GetYIndex(latitude);

                // Grid map returns -1 in case the index is not found
                if (i < 0 || j < 0)
                {
                    return Color.Transparent;
                }
                else
                {
                    int i1 = Math.Max(0, i - 1);
                    int j1 = Math.Max(0, j - 1);

                    // Get values at (i, j) and (i1, j1).
                    double value = this.projectionGridMap.InputGrid.GetValueAt(i, j);
                    double valueDash = this.projectionGridMap.InputGrid.GetValueAt(i1, j1);

                    // Use the delta between value and valueDash as relief shaded value.
                    reliefShadedValue = value - valueDash;
                }
            }

            if (double.IsNaN(reliefShadedValue) || reliefShadedValue == 0)
            {
                return Color.Transparent;
            }

            reliefShadedValue = 127 * Math.Abs(reliefShadedValue) / 5;
            int colorCode = (int)((127 + reliefShadedValue) % 256);
            return Color.FromArgb(colorCode, colorCode, colorCode);
        }

        /// <summary>
        ///  Retrieves the elevation of a pixel at specific latitude and longitude value.
        /// </summary>
        /// <param name="longitude">Longitude value for which the elevation has to be retrieved.</param>
        /// <param name="latitude">Latitude value for which the elevation has to be retrieved.</param>
        /// <returns>Returns the elevation.</returns>
        public short GetElevation(double longitude, double latitude)
        {
            double value = this.projectionGridMap.GetValue(longitude, latitude);
            if (double.IsNaN(value))
            {
                return 0;
            }

            return Convert.ToInt16(value);
        }
    }
}