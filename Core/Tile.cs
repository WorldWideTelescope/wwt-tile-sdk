//-----------------------------------------------------------------------
// <copyright file="Tile.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation 2011. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Microsoft.Research.Wwt.Sdk.Core
{
    /// <summary>
    /// Class holding tile coordinates.
    /// </summary>
    public class Tile
    {
        /// <summary>
        /// Initializes a new instance of the Tile class.
        /// </summary>
        /// <param name="tileX">X coordinate of the tile.</param>
        /// <param name="tileY">Y coordinate of the tile.</param>
        public Tile(int tileX, int tileY)
        {
            this.X = tileX;
            this.Y = tileY;
        }

        /// <summary>
        /// Gets the X value of a tile.
        /// </summary>
        public int X { get; private set; }

        /// <summary>
        /// Gets the Y value of a tile.
        /// </summary>
        public int Y { get; private set; }
    }
}
