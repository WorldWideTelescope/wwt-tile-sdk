//---------------------------------------------------------------------------
// <copyright file="MercatorTileCreator.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation 2010. All rights reserved.
// </copyright>
//---------------------------------------------------------------------------

using System;
using System.Drawing;

namespace Microsoft.Research.Wwt.Sdk.Core
{
    /// <summary>
    /// Creates image tiles in Mercator projection.
    /// </summary>
    public class MercatorTileCreator : ITileCreator
    {
        /// <summary>
        /// Initializes a new instance of the MercatorTileCreator class.
        /// </summary>
        /// <param name="map">
        /// Color map.
        /// </param>
        /// <param name="serializer">
        /// Tile serializer.
        /// </param>
        public MercatorTileCreator(IColorMap map, IImageTileSerializer serializer)
        {
            if (map == null)
            {
                throw new ArgumentNullException("map");
            }

            if (serializer == null)
            {
                throw new ArgumentNullException("serializer");
            }

            this.ColorMap = map;
            this.TileSerializer = serializer;
        }

        /// <summary>
        /// Gets the projection type for this tile creator.
        /// </summary>
        public ProjectionTypes ProjectionType
        {
            get { return ProjectionTypes.Mercator; }
        }

        /// <summary>
        /// Gets the IColorMap instance bound to this tile creator.
        /// </summary>
        public IColorMap ColorMap { get; private set; }

        /// <summary>
        /// Gets the IImageTileSeralizer instance bound to this tile creator.
        /// </summary>
        public IImageTileSerializer TileSerializer { get; private set; }

        /// <summary>
        /// Creates the tile specified by level.
        /// </summary>
        /// <param name="level">
        /// Zoom level.
        /// </param>
        /// <param name="tileX">
        /// X tile coordinate.
        /// </param>
        /// <param name="tileY">
        /// Y tile coordinate.
        /// </param>
        public void Create(int level, int tileX, int tileY)
        {
            // Pre-compute tables to map from pixel position to global (longitude, latitude).
            // Map size at this level.
            double tileSize = (double)(((long)Constants.TileSize) << level);

            // Compute longitudes across the tile (px = 0 at left edge);
            double[] longitudes = new double[Constants.TileSize];
            long pixelX = checked(tileX * Constants.TileSize);
            for (int px = 0; px < Constants.TileSize; px++)
            {
                double x = ((pixelX + px) + 0.5) / tileSize;
                longitudes[px] = 360.0 * x - 180.0;
            }

            // Compute latitudes across the tile (py = 0 at top edge).
            double[] latitudes = new double[Constants.TileSize];
            long pixelY = checked(tileY * Constants.TileSize);
            for (int py = 0; py < Constants.TileSize; py++)
            {
                double y = 0.5 - (((pixelY + py) + 0.5) / tileSize);
                latitudes[py] = 90.0 - (360.0 * Math.Atan(Math.Exp(-y * 2.0 * Math.PI)) / Math.PI);
            }

            int[] colors = new int[Constants.TileSize * Constants.TileSize];
            bool hasData = false;
            int position = -1;
            for (int py = 0; py < Constants.TileSize; py++)
            {
                for (int px = 0; px < Constants.TileSize; px++)
                {
                    // Map geo location to an ARGB value.
                    Color color = this.ColorMap.GetColor(longitudes[px], latitudes[py]);

                    // Store and update bit indicating whether actual data is present or not.
                    position++;
                    colors[position] = color.ToArgb();
                    if (hasData == false)
                    {
                        hasData = (color != Color.Transparent);
                    }
                }
            }

            if (hasData)
            {
                TileHelper.ToBitmap(level, tileX, tileY, colors, this.TileSerializer);
            }

            colors = null;
            longitudes = null;
            latitudes = null;
        }

        /// <summary>
        /// Aggregates lower level image tiles to construct upper level tiles.
        /// </summary>
        /// <param name="level">
        /// Zoom level.
        /// </param>
        /// <param name="tileX">
        /// X coordinate.
        /// </param>
        /// <param name="tileY">
        /// Y coordinate.
        /// </param>
        public void CreateParent(int level, int tileX, int tileY)
        {
            TileHelper.CreateParent(level, tileX, tileY, this.TileSerializer);
        }
    }
}