//-----------------------------------------------------------------------
// <copyright file="IImageTileSerializer.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation 2011. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Drawing;

namespace Microsoft.Research.Wwt.Sdk.Core
{
    /// <summary>
    /// Defines the mechanism for serializing a tile.
    /// </summary>
    public interface IImageTileSerializer
    {
        /// <summary>
        /// Serializes the Image tile object to a storage system.
        /// </summary>
        /// <param name="tile">
        /// Bitmap to be saved.
        /// </param>
        /// <param name="level">
        /// Zoom level.
        /// </param>
        /// <param name="tileX">
        /// X coordinate of the tile.
        /// </param>
        /// <param name="tileY">
        /// Y coordinate of the tile.
        /// </param>
        void Serialize(Bitmap tile, int level, int tileX, int tileY);

        /// <summary>
        /// Deserializes a Image tile object from storage system.
        /// </summary>
        /// <param name="level">
        /// Zoom level.
        /// </param>
        /// <param name="tileX">
        /// X coordinate of the tile.
        /// </param>
        /// <param name="tileY">
        /// Y coordinate of the tile.
        /// </param>
        /// <returns>
        /// Deserialized bitmap.
        /// </returns>
        Bitmap Deserialize(int level, int tileX, int tileY);
    }
}