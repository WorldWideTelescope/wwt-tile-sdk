//-----------------------------------------------------------------------
// <copyright file="IPlateFileGenerator.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation 2011. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Microsoft.Research.Wwt.Sdk.Core
{
    /// <summary>
    /// Interface for plate file generator.
    /// </summary>
    public interface IPlateFileGenerator
    {
        /// <summary>
        /// Gets the number of processed tiles for the level in context.
        /// </summary>
        long TilesProcessed { get; }

        /// <summary>
        /// This function is used to create the plate file from already generated pyramids.
        /// </summary>
        /// <param name="serializer">
        /// Image tile serializer to retrieve the tile.
        /// </param>
        void CreateFromImageTile(IImageTileSerializer serializer);
    }
}
