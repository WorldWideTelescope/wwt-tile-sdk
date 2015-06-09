//-----------------------------------------------------------------------
// <copyright file="DataColorMap.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation 2011. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Microsoft.Research.Wwt.Sdk.Core
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Linq;

    /// <summary>
    /// Represents color map used to generate color pixels corresponding to longitude and latitude coordinates 
    /// based on the projection and the source grid
    /// </summary>
    public class DataColorMap : IColorMap
    {
        /// <summary>
        /// Grid map for Data.
        /// </summary>
        private IProjectionGridMap projectionGridMap;

        /// <summary>
        /// List of colors used for mapping.
        /// </summary>
        private List<Color> colors;

        /// <summary>
        /// Minimum threshold value for color.
        /// </summary>
        private double minimumThreshold;

        /// <summary>
        /// Maximum threshold value for color.
        /// </summary>
        private double maximumThreshold;

        /// <summary>
        /// Initializes a new instance of the DataColorMap class.
        /// </summary>
        /// <param name="colorMapFile">
        /// Color map file path. This file will be used as Index of colors.
        /// </param>
        /// <param name="projectionGridMap">
        /// Input image grid map for specified projection.
        /// </param>
        /// <param name="orientation">
        /// Represents the orientation of the color map. 
        ///     For example : Vertical or Horizontal.
        /// </param>
        /// <param name="minimumValue">
        /// Represents minimum threshold value for color.
        /// </param>
        /// <param name="maximumValue">
        /// Represents maximum threshold value for color.
        /// </param>
        public DataColorMap(string colorMapFile, IProjectionGridMap projectionGridMap, ColorMapOrientation orientation, double minimumValue, double maximumValue)
        {
            if (string.IsNullOrEmpty(colorMapFile))
            {
                throw new ArgumentNullException("colorMapFile");
            }

            if (projectionGridMap == null)
            {
                throw new ArgumentNullException("projectionGridMap");
            }

            this.projectionGridMap = projectionGridMap;
            this.minimumThreshold = minimumValue;
            this.maximumThreshold = maximumValue;
            try
            {
                using (Bitmap bitmap = new Bitmap(Bitmap.FromFile(colorMapFile)))
                {
                    int samplingPoint = bitmap.Width / 2;
                    int numberOfColors = bitmap.Height;
                    if (orientation == ColorMapOrientation.Horizontal)
                    {
                        samplingPoint = bitmap.Height / 2;
                        numberOfColors = bitmap.Width;
                    }

                    this.colors = new List<Color>(numberOfColors);

                    if (orientation == ColorMapOrientation.Vertical)
                    {
                        for (int i = 0; i < numberOfColors; i++)
                        {
                            this.colors.Add(bitmap.GetPixel(samplingPoint, numberOfColors - i - 1));
                        }
                    }
                    else if (orientation == ColorMapOrientation.Horizontal)
                    {
                        for (int i = 0; i < numberOfColors; i++)
                        {
                            this.colors.Add(bitmap.GetPixel(numberOfColors - i - 1, samplingPoint));
                        }
                    }
                }
            }
            catch
            {
                throw new InvalidOperationException("Error reading the color map file. Check the color map and orientation.");
            }
        }

        /// <summary>
        /// Gets the color of pixel at specified longitude and latitude values.
        /// </summary>
        /// <param name="longitude">
        /// Longitude value for which the pixel color has to be retrieved.
        /// </param>
        /// <param name="latitude">
        /// Latitude value for which the pixel color has to be retrieved.
        /// </param>
        /// <returns>
        /// Return the color which is represented by longitude and latitude.
        /// </returns>
        public Color GetColor(double longitude, double latitude)
        {
            double value = this.projectionGridMap.GetValue(longitude, latitude);
            if (double.IsNaN(value))
            {
                return Color.Transparent;
            }

            if (value < this.minimumThreshold)
            {
                return this.colors.FirstOrDefault();
            }

            if (value > this.maximumThreshold)
            {
                return this.colors.LastOrDefault();
            }

            int colorIndex = (int)((this.colors.Count - 1) * (value - this.minimumThreshold) / (this.maximumThreshold - this.minimumThreshold));
            colorIndex = Math.Max(0, colorIndex);
            colorIndex = Math.Min(this.colors.Count, colorIndex);
            return this.colors[colorIndex];
        }
    }
}
