//-----------------------------------------------------------------------
// <copyright file="PlateFileGenerator.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation 2011. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace Microsoft.Research.Wwt.Sdk.Core
{
    /// <summary>
    /// Class used to generate plate file.
    /// </summary>
    public class PlateFileGenerator : IPlateFileGenerator
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PlateFileGenerator"/> class.
        /// </summary>
        /// <param name="filePath">
        /// Path of the plate file.
        /// </param>
        /// <param name="levels">
        /// Number of levels.
        /// </param>
        /// <param name="format">
        /// Image Format of the tiles.
        /// </param>
        public PlateFileGenerator(string filePath, int levels, ImageFormat format)
        {
            this.PlateFilePath = filePath;
            this.Levels = levels;
            this.Format = format;
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
        /// Gets or sets the image format for tiles. By default the image format is png.
        /// </summary>
        public ImageFormat Format { get; set; }

        /// <summary>
        /// Gets the number of processed tiles for the level in context.
        /// </summary>
        public long TilesProcessed { get; private set; }

        /// <summary>
        /// This function is used to create the plate file from already generated pyramids.
        /// </summary>
        /// <param name="serializer">
        /// Image tile serializer to retrieve the tile.
        /// </param>
        public void CreateFromImageTile(IImageTileSerializer serializer)
        {
            TilesProcessed = 0;
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
                        TilesProcessed++;
                        if (serializer != null)
                        {
                            Bitmap tile = serializer.Deserialize(level, indexX, indexY);
                            if (tile != null)
                            {
                                using (MemoryStream ms = new MemoryStream())
                                {
                                    ms.Seek(0, SeekOrigin.Begin);
                                    tile.Save(ms, ImageFormat.Png);
                                    currentPlate.AddStream(ms, level, indexX, indexY);
                                }

                                tile.Dispose();
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