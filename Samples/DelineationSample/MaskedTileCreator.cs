//---------------------------------------------------------------------------
// <copyright file="MaskedTileCreator.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation 2010. All rights reserved.
// </copyright>
//---------------------------------------------------------------------------

using System;
using System.Drawing;
using Microsoft.Research.Wwt.Sdk.Core;
using Microsoft.Research.Wwt.Sdk.Utilities;

namespace Microsoft.Research.Wwt.Sdk.Samples
{
    /// <summary>
    /// Creates image tiles in Toast projection.
    /// </summary>
    public class MaskedTileCreator : ITileCreator
    {
        /// <summary>
        /// Initializes a new instance of the MaskedTileCreator class.
        /// </summary>
        /// <param name="map">
        /// Color map.
        /// </param>
        /// <param name="serializer">
        /// Tile serializer.
        /// </param>
        /// <param name="lookAtOutsideSurface">
        /// True if the projection is from outside the sphere, False if it's from inside.
        /// </param>
        public MaskedTileCreator(IColorMap map, IImageTileSerializer serializer, bool lookAtOutsideSurface)
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
            this.LookAtOutsideSurface = lookAtOutsideSurface;
        }

        /// <summary>
        /// Gets the projection type for this tile creator.
        /// </summary>
        public ProjectionTypes ProjectionType
        {
            get { return ProjectionTypes.Toast; }
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
        /// Gets or sets the serializer to look up reference tiles for delineating.
        /// </summary>
        public IImageTileSerializer ReferenceTileSerializer { get; set; }

        /// <summary>
        /// Gets a value indicating whether the spherical TOAST image being constructed 
        /// is being looked at from outside of the sphere.
        /// </summary>
        public bool LookAtOutsideSurface { get; private set; }

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
            // Map to convert from pixel position to global (longitude, latitude).
            OctTileMap tileMap = new OctTileMap(level, tileX, tileY);

            int[] colors = new int[Constants.TileSize * Constants.TileSize];
            bool hasData = false;
            int position = -1;
            for (int pixelY = 0; pixelY < Constants.TileSize; pixelY++)
            {
                for (int pixelX = 0; pixelX < Constants.TileSize; pixelX++)
                {
                    // Map pixel (u, v) position to (longitude, latitude).
                    double u = (0.5 + pixelX) / ((double)Constants.TileSize);
                    double v = (0.5 + pixelY) / ((double)Constants.TileSize);
                    Vector2d location = tileMap.PointToRaDec(new Vector2d(u, v));
                    double latitude = location.Y;
                    double longitude = location.X;
                    if (this.LookAtOutsideSurface)
                    {
                        longitude -= 180.0;
                    }

                    // Map geo location to an ARGB value.
                    Color color = this.ColorMap.GetColor(longitude, latitude);

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
                TileHelper.ToBitmap(level, tileX, tileY, colors, this.TileSerializer, this.ReferenceTileSerializer);
            }
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
