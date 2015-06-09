//---------------------------------------------------------------------
// <copyright file="TileCreatorFactory.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation 2011. All rights reserved.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Drawing.Imaging;
using System.IO;

namespace Microsoft.Research.Wwt.Sdk.Core
{
    /// <summary>
    /// This class can be used for creating a tile creator.
    /// </summary>
    public static class TileCreatorFactory
    {
        /// <summary>
        /// Creates a DEM tile creator instance for the specified projection type.
        /// </summary>
        /// <param name="map">
        /// Elevation map used.
        /// </param>
        /// <param name="projectionType">
        /// Projection type desired.
        /// </param>
        /// <param name="serializer">
        /// Tile serializer instance.
        /// </param>
        /// <returns>
        /// ITileCreator instance.
        /// </returns>
        public static ITileCreator CreateDemTileCreator(IElevationMap map, ProjectionTypes projectionType, IDemTileSerializer serializer)
        {
            if (map == null)
            {
                throw new ArgumentNullException("map");
            }

            if (serializer == null)
            {
                throw new ArgumentNullException("serializer");
            }

            switch (projectionType)
            {
                case ProjectionTypes.Mercator:
                    return new MercatorDemTileCreator(map, serializer);

                default:
                    return new ToastDemTileCreator(map, serializer);
            }
        }

        /// <summary>
        /// Creates a DEM tile creator instance for the specified projection type.
        /// </summary>
        /// <param name="map">
        /// Elevation map used.
        /// </param>
        /// <param name="projectionType">
        /// Projection type desired.
        /// </param>
        /// <param name="path">
        /// Location where the tiles should be serialized.
        /// </param>
        /// <returns>
        /// ITileCreator instance.
        /// </returns>
        public static ITileCreator CreateDemTileCreator(IElevationMap map, ProjectionTypes projectionType, string path)
        {
            if (map == null)
            {
                throw new ArgumentNullException("map");
            }

            if (path == null)
            {
                throw new ArgumentNullException("path");
            }

            IDemTileSerializer serializer = new DemTileSerializer(Path.Combine(path, @"Pyramid\{0}\{1}\DL{0}X{1}Y{2}.dem"));
            return CreateDemTileCreator(map, projectionType, serializer);
        }

        /// <summary>
        /// Creates an image tile creator instance for the specified projection type.
        /// </summary>
        /// <param name="map">
        /// Color map used.
        /// </param>
        /// <param name="projectionType">
        /// Projection type desired.
        /// </param>
        /// <param name="serializer">
        /// Tile serializer instance.
        /// </param>
        /// <returns>
        /// ITileCreator instance.
        /// </returns>
        public static ITileCreator CreateImageTileCreator(IColorMap map, ProjectionTypes projectionType, IImageTileSerializer serializer)
        {
            if (map == null)
            {
                throw new ArgumentNullException("map");
            }

            if (serializer == null)
            {
                throw new ArgumentNullException("serializer");
            }

            switch (projectionType)
            {
                case ProjectionTypes.Mercator:
                    return new MercatorTileCreator(map, serializer);

                default:
                    return new ToastTileCreator(map, serializer);
            }
        }

        /// <summary>
        /// Creates a DEM tile creator instance for the specified projection type.
        /// </summary>
        /// <param name="map">
        /// Color map used.
        /// </param>
        /// <param name="projectionType">
        /// Projection type desired.
        /// </param>
        /// <param name="path">
        /// Location where the tiles should be serialized.
        /// </param>
        /// <returns>
        /// ITileCreator instance.
        /// </returns>
        public static ITileCreator CreateImageTileCreator(IColorMap map, ProjectionTypes projectionType, string path)
        {
            if (map == null)
            {
                throw new ArgumentNullException("map");
            }

            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentNullException("path");
            }

            IImageTileSerializer serializer = new ImageTileSerializer(TileHelper.GetDefaultImageTilePathTemplate(path), ImageFormat.Png);
            return CreateImageTileCreator(map, projectionType, serializer);
        }
    }
}
