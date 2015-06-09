//------------------------------------------------------------------------
// <copyright file="MultiTileCreator.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation 2011. All rights reserved.
// </copyright>
//------------------------------------------------------------------------

using System;
using System.Collections.Generic;

namespace Microsoft.Research.Wwt.Sdk.Core
{
    /// <summary>
    /// Encapsulates different tile creator instances.
    /// </summary>
    public class MultiTileCreator : ITileCreator
    {
        /// <summary>
        /// Collection of tile creation instances.
        /// </summary>
        private ICollection<ITileCreator> tileCreators;

        /// <summary>
        /// Initializes a new instance of the MultiTileCreator class.
        /// </summary>
        /// <param name="creators">
        /// Collection of tile creator instances.
        /// </param>
        /// <param name="type">
        /// Desired projection type.
        /// </param>
        public MultiTileCreator(ICollection<ITileCreator> creators, ProjectionTypes type)
        {
            if (creators == null)
            {
                throw new ArgumentNullException("creators");
            }

            this.tileCreators = creators;
            this.ProjectionType = type;
        }

        /// <summary>
        /// Gets the ProjectionType.
        /// </summary>
        public ProjectionTypes ProjectionType { get; private set; }

        /// <summary>
        /// Calls CreatePyramid on each of the encapsulated tile creator instances.
        /// </summary>
        /// <param name="level">
        /// Level at which the base image has to be built.
        /// </param>
        /// <param name="tileX">
        /// X tile coordinate.
        /// </param>
        /// <param name="tileY">
        /// Y tile coordinate.
        /// </param>
        public void Create(int level, int tileX, int tileY)
        {
            foreach (var creator in this.tileCreators)
            {
                creator.Create(level, tileX, tileY);
            }
        }

        /// <summary>
        /// Build the base image at the specified level.
        /// </summary>
        /// <param name="level">
        /// Level at which the base image has to be built.
        /// </param>
        /// <param name="tileX">
        /// X tile coordinate.
        /// </param>
        /// <param name="tileY">
        /// Y tile coordinate.
        /// </param>
        public void CreateParent(int level, int tileX, int tileY)
        {
            foreach (var creator in this.tileCreators)
            {
                creator.CreateParent(level, tileX, tileY);
            }
        }
    }
}
