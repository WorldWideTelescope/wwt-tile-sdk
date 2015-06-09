//-----------------------------------------------------------------------
// <copyright file="MultiplePlateFileGenerator.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation 2011. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;

namespace Microsoft.Research.Wwt.Sdk.Core
{
    /// <summary>
    /// Class used to generate multiple plate files.
    /// </summary>
    public class MultiplePlateFileGenerator : IPlateFileGenerator
    {
        /// <summary>
        /// Gets the path of the folder where the plates will be stored.
        /// </summary>
        private string platesFolderPath;

        /// <summary>
        /// Gets the number of levels of the plate file.
        /// </summary>
        private int maxLevels;

        /// <summary>
        /// Multiple plate file details.
        /// </summary>
        private MultiplePlateFileDetails plateFileDetails;

        /// <summary>
        /// Gets or sets the image format for tiles. By default the image format is png.
        /// </summary>
        private ImageFormat format;

        /// <summary>
        /// Initializes a new instance of the <see cref="MultiplePlateFileGenerator"/> class.
        /// </summary>
        /// <param name="folderPath">
        /// Path of the plate file.
        /// </param>
        /// <param name="levels">
        /// Number of levels.
        /// </param>
        /// <param name="format">
        /// Image Format of the tiles.
        /// </param>
        public MultiplePlateFileGenerator(string folderPath, int levels, ImageFormat format)
        {
            this.platesFolderPath = folderPath;
            this.maxLevels = levels;
            this.format = format;
            this.plateFileDetails = new MultiplePlateFileDetails(this.maxLevels);
        }

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
            System.IO.Directory.CreateDirectory(System.IO.Path.Combine(platesFolderPath, this.plateFileDetails.MinOverlappedLevel.ToString(CultureInfo.InvariantCulture)));

            TilesProcessed = 0;
            CreateBaseLevelPlateFiles(serializer);
            CreateTopLevelPlateFile(serializer);
        }

        /// <summary>
        /// This function is used to generate the base level plate files.
        /// </summary>
        /// <param name="serializer">
        /// Image tile serializer to retrieve the tile.
        /// </param>
        private void CreateBaseLevelPlateFiles(IImageTileSerializer serializer)
        {
            int maxPlate = (int)Math.Pow(2, this.plateFileDetails.MinOverlappedLevel);

            Parallel.For(
                0,
                maxPlate,
                yPlate =>
                {
                    for (int xPlate = 0; xPlate < maxPlate; xPlate++)
                    {
                        // Get plate file name base don the X and Y coordinates of the plate file.
                        string plateFilePath = System.IO.Path.Combine(this.platesFolderPath, this.plateFileDetails.MinOverlappedLevel.ToString(CultureInfo.InvariantCulture), string.Format(CultureInfo.InvariantCulture, Constants.BaseLevelPlateFormat, this.plateFileDetails.MinOverlappedLevel, xPlate, yPlate));

                        // Create the plate file instance.
                        PlateFile currentPlate = new PlateFile(plateFilePath, this.plateFileDetails.LevelsPerPlate);
                        currentPlate.Create();

                        for (int level = this.plateFileDetails.LevelsPerPlate - 1; level >= 0; level--)
                        {
                            // Get the number of tiles for the given level index.
                            int numberOftiles = (int)Math.Pow(2, level);

                            // Calculate the start and end index of X axis.
                            int xStart = xPlate * numberOftiles;

                            // Calculate the start and end index of Y axis.
                            int yStart = yPlate * numberOftiles;

                            // Add each tile to the plate file.
                            for (int yIndex = 0; yIndex < numberOftiles; yIndex++)
                            {
                                for (int xIndex = 0; xIndex < numberOftiles; xIndex++)
                                {
                                    TilesProcessed++;
                                    if (serializer != null)
                                    {
                                        Bitmap tile = serializer.Deserialize((this.maxLevels - (this.plateFileDetails.LevelsPerPlate - 1) + level), xStart + xIndex, yStart + yIndex);
                                        if (tile != null)
                                        {
                                            using (MemoryStream ms = new MemoryStream())
                                            {
                                                ms.Seek(0, SeekOrigin.Begin);
                                                tile.Save(ms, format);
                                                currentPlate.AddStream(ms, level, xIndex, yIndex);
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
                });
        }

        /// <summary>
        /// This function is used to generate the top level plate file.
        /// </summary>
        /// <param name="serializer">
        /// Image tile serializer to retrieve the tile.
        /// </param>
        private void CreateTopLevelPlateFile(IImageTileSerializer serializer)
        {
            string plateFilePath = System.IO.Path.Combine(this.platesFolderPath, Constants.TopLevelPlate);
            PlateFile currentPlate = new PlateFile(plateFilePath, this.maxLevels);
            currentPlate.Create();

            for (int level = 0; level <= this.plateFileDetails.MaxOverlappedLevel; level++)
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
                                    tile.Save(ms, format);
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