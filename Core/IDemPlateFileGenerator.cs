//-----------------------------------------------------------------------
// <copyright file="IDemPlateFileGenerator.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation 2011. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Microsoft.Research.Wwt.Sdk.Core
{
    /// <summary>
    /// Interface for DEM plate file generator.
    /// </summary>
    public interface IDemPlateFileGenerator
    {
        /// <summary>
        /// Gets the number of processed tiles for the level in context.
        /// </summary>
        long TilesProcessed { get; }

        /// <summary>
        /// This function is used to create the plate file from already generated DEM pyramid.
        /// </summary>
        /// <param name="serializer">
        /// DEM tile serializer to retrieve the tile.
        /// </param>
        void CreateFromDemTile(IDemTileSerializer serializer);
    }
}
