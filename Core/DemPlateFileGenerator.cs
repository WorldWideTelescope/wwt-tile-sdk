//-----------------------------------------------------------------------
// <copyright file="DemPlateFileGenerator.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation 2011. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.IO;

namespace Microsoft.Research.Wwt.Sdk.Core
{
    /// <summary>
    /// Class used to generate DEM plate file.
    /// </summary>
    public class DemPlateFileGenerator : IDemPlateFileGenerator
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DemPlateFileGenerator"/> class.
        /// </summary>
        /// <param name="filePath">
        /// Path of the plate file.
        /// </param>
        /// <param name="levels">
        /// Number of levels.
        /// </param>
        public DemPlateFileGenerator(string filePath, int levels)
        {
            this.PlateFilePath = filePath;
            this.Levels = levels;
        }

        /// <summary>
        /// Gets the plate file path.
        /// </summary>
        public string PlateFilePath { get; private set; }

        /// <summary>
        /// Gets the number of levels of the plate file.
        /// </summary>
        public int Levels { get; private set; }

        /// <summary>
        /// Gets the number of processed tiles for the level in context.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Need to implement this as part of interface IDemPlateFileGenerator.")]
        public long TilesProcessed { get; private set; }

        /// <summary>
        /// This function is used to create the plate file from already generated DEM pyramid.
        /// </summary>
        /// <param name="serializer">
        /// DEM tile serializer to retrieve the tile.
        /// </param>
        public void CreateFromDemTile(IDemTileSerializer serializer)
        {
            PlateFile currentPlate = new PlateFile(this.PlateFilePath, this.Levels);
            currentPlate.Create();

            for (int level = 0; level <= Levels; level++)
            {
                // Number of tiles in each direction at this level
                int n = (int)Math.Pow(2, level);

                // Add each tile to the plate file.
                for (int indexY = 0; indexY < n; indexY++)
                {
                    for (int indexX = 0; indexX < n; indexX++)
                    {
                        if (serializer != null)
                        {
                            short[] data = serializer.Deserialize(level, indexX, indexY);
                            if (data != null)
                            {
                                using (MemoryStream ms = new MemoryStream())
                                {
                                    ms.Seek(0, SeekOrigin.Begin);
                                    BinaryWriter bw = new BinaryWriter(ms);
                                    foreach (short value in data)
                                    {
                                        bw.Write(value);
                                    }

                                    currentPlate.AddStream(ms, level, indexX, indexY);
                                }

                                data = null;
                            }
                        }
                    }
                }
            }

            // Update the header and close the file stream.
            currentPlate.UpdateHeaderAndClose();
        }
    }
}