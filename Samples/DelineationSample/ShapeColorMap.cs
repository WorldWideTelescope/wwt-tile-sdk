//-----------------------------------------------------------------------
// <copyright file="ShapeColorMap.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation 2011. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Drawing;
using Microsoft.Research.Wwt.Sdk.Core;

namespace Microsoft.Research.Wwt.Sdk.Samples
{
    /// <summary>
    /// Class that maps a color value for x and y coordinates.
    /// </summary>
    public class ShapeColorMap : IColorMap
    {
        /// <summary>
        /// Color value map source.
        /// </summary>
        private IValueMap valueSource;

        /// <summary>
        /// Initializes a new instance of the ShapeColorMap class.
        /// </summary>
        /// <param name="source">Color map source.</param>
        public ShapeColorMap(IValueMap source)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }

            this.valueSource = source;
        }

        /// <summary>
        /// Returns a color value for x and y coordinate value.
        /// </summary>
        /// <param name="x">Longitude value.</param>
        /// <param name="y">Latitude value.</param>
        /// <returns>Color value.</returns>
        public Color GetColor(double x, double y)
        {
            Color color = Color.Transparent;
            try
            {
                double v = this.valueSource.GetValueAt(x, y);

                if (v == 0)
                {
                   color = Color.Black;
                }
                else
                {
                    color = Color.White;
                }
            }
            catch
            {
            }

            return color;
        }
    }
}
