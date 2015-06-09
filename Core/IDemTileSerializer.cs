//-----------------------------------------------------------------------
// <copyright file="IDemTileSerializer.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation 2011. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Microsoft.Research.Wwt.Sdk.Core
{
    /// <summary>
    /// Defines the mechanism for serializing a DEM tile.
    /// </summary>
    public interface IDemTileSerializer
    {
        /// <summary>
        /// Serializes the DEM tile object to a storage system.
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
        void Serialize(short[] tile, int level, int tileX, int tileY);

        /// <summary>
        /// Deserializes a DEM tile object from storage system.
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
        /// Deserialized tile.
        /// </returns>
        short[] Deserialize(int level, int tileX, int tileY);
    }
}