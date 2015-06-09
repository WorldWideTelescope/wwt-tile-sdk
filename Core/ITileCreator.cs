//-----------------------------------------------------------------------
// <copyright file="ITileCreator.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation 2011. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Microsoft.Research.Wwt.Sdk.Core
{
    /// <summary>
    /// Creates tiles for a given level.
    /// </summary>
    public interface ITileCreator
    {
        /// <summary>
        /// Gets the ProjectionType.
        /// </summary>
        ProjectionTypes ProjectionType { get; }

        /// <summary>
        /// Creates base tiles and the pyramid.
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
        void Create(int level, int tileX, int tileY);

        /// <summary>
        /// Fills up the pyramid starting at specified level.
        /// </summary>
        /// <param name="level">
        /// Base level of the pyramid.
        /// </param>
        /// <param name="tileX">
        /// X tile coordinate.
        /// </param>
        /// <param name="tileY">
        /// Y tile coordinate.
        /// </param>
        void CreateParent(int level, int tileX, int tileY);
    }
}
