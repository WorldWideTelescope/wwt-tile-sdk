//-----------------------------------------------------------------------
// <copyright file="TileRepositoryFactory.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation 2011. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Microsoft.Research.Wwt.SharingService.Web
{
    /// <summary>
    /// Class decides on the instance of Tile repository.
    /// </summary>
    public static class TileRepositoryFactory
    {
        /// <summary>
        /// Creates a repository instance.
        /// </summary>
        /// <returns>Tile repository instance.</returns>
        public static ITileRepository Create()
        {
            return new LocalTileRepository(); 
        }
    }
}
