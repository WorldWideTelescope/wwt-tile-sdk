//-----------------------------------------------------------------------
// <copyright file="PlateFileHelper.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation 2011. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Xml;

namespace Microsoft.Research.Wwt.Sdk.Core
{
    /// <summary>
    /// This is an help class for plate file.
    /// </summary>
    public static class PlateFileHelper
    {
        /// <summary>
        /// This function is used to retrieve the tile from the multiple plates.
        /// </summary>
        /// <param name="level">Level of the image.</param>
        /// <param name="x">X axis image.</param>
        /// <param name="y">Y axis image.</param>
        /// <param name="platesFolder">Multiple plates folder.</param>
        /// <returns>Tile image for the specified image.</returns>
        public static Stream GetTileFromMultiplePlates(int level, int x, int y, string platesFolder)
        {
            DirectoryInfo platesDirectory = new DirectoryInfo(platesFolder);
            Stream outputStream = null;
            if (platesDirectory != null && platesDirectory.Exists)
            {
                outputStream = GetTileFromMultiplePlates(level, x, y, platesDirectory, Constants.TopLevelPlate, Constants.BaseLevelPlateFormat);
            }

            return outputStream;
        }

        /// <summary>
        /// This function is used to retrieve the tile from the multiple plates.
        /// </summary>
        /// <param name="level">Level of the DEM Tile.</param>
        /// <param name="x">X axis DEM Tile.</param>
        /// <param name="y">Y axis DEM Tile.</param>
        /// <param name="demPlatesFolder">Multiple DEM plates folder.</param>
        /// <returns>DEM tile data as stream.</returns>
        public static Stream GetDEMTileFromMultiplePlates(int level, int x, int y, string demPlatesFolder)
        {
            DirectoryInfo demPlatesDirectory = new DirectoryInfo(demPlatesFolder);
            Stream outputStream = null;
            if (demPlatesDirectory != null && demPlatesDirectory.Exists)
            {
                outputStream = GetTileFromMultiplePlates(level, x, y, demPlatesDirectory, Constants.DEMTopLevelPlate, Constants.DEMBaseLevelPlateFormat);
            }

            return outputStream;
        }

        /// <summary>
        /// This function is used to retrieve the tile from the multiple plates.
        /// </summary>
        /// <param name="level">Level of the image.</param>
        /// <param name="x">X axis image.</param>
        /// <param name="y">Y axis image.</param>
        /// <param name="platesFolder">Multiple plates folder.</param>
        /// <param name="topLevelPlateFilename">Top level plate file name.</param>
        /// <param name="plateFilenameFormat">Format for plate file name.</param>
        /// <returns>Tile image for the specified image.</returns>
        private static Stream GetTileFromMultiplePlates(int level, int x, int y, DirectoryInfo platesFolder, string topLevelPlateFilename, string plateFilenameFormat)
        {
            Stream outputStream = null;
            int maxLevel = GetMaxLevel(platesFolder);

            // TODO: Need to check on how to get the details on how many levels are there in a plate.
            int levelsPerPlate = 8;

            // Get the levels break up from Plates folder structure.
            List<int> plateLevels = GetPlateLevels(platesFolder);

            // Sort the plate levels. This is make sure that we can do binary search.
            plateLevels.Sort();

            // Get the index of level of the plate files (folder name) where the plate file which contains the tile for level, x and y could be found.
            // BinarySearch = The zero-based index of item in the sorted List<T>, if item is found; otherwise, a negative number that is the bitwise 
            //      complement of the index of the next element that is larger than item or, if there is no larger element, the bitwise complement of Count.
            int plateLevelIndex = plateLevels.BinarySearch(level);
            plateLevelIndex = plateLevelIndex < 0 ? ~plateLevelIndex - 1 : plateLevelIndex;

            if (plateLevelIndex <= 0)
            {
                // This means that we need to get data from top level plate file.
                PlateFile plateFileInstance = new PlateFile(Path.Combine(platesFolder.FullName, topLevelPlateFilename), levelsPerPlate);
                outputStream = plateFileInstance.GetFileStream(level, x, y);
            }
            else if (level <= maxLevel)
            {
                // Get the minimum level number (which will be the Folder name) from the plate level index.
                int minLevel = plateLevels[plateLevelIndex];

                // Level in the local plate file.
                int plateLevel = level - minLevel;

                int powLevDiff = (int)Math.Pow(2, plateLevel);
                int plateIndexX = x / powLevDiff;
                int plateIndexY = y / powLevDiff;
                string filename = string.Format(CultureInfo.InvariantCulture, plateFilenameFormat, minLevel, plateIndexX, plateIndexY);

                int tileIndexX = x % powLevDiff;
                int tileIndexY = y % powLevDiff;

                PlateFile plateFileInstance = new PlateFile(Path.Combine(platesFolder.FullName, minLevel.ToString(CultureInfo.InvariantCulture), filename), levelsPerPlate);
                outputStream = plateFileInstance.GetFileStream(plateLevel, tileIndexX, tileIndexY);
            }

            return outputStream;
        }

        /// <summary>
        /// Gets max level from WTML.
        /// </summary>
        /// <param name="platesFolder">Plates Folder.</param>
        /// <returns>Max level of the pyramid</returns>
        private static int GetMaxLevel(DirectoryInfo platesFolder)
        {
            int maxLevel = 0;
            try
            {
                FileInfo plateFile = platesFolder.Parent.GetFiles(Constants.WTMLSearchPattern).FirstOrDefault();

                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(plateFile.FullName);
                XmlNode imageSet = xmlDoc.SelectSingleNode(Constants.ImageSetPath);
                if (imageSet != null)
                {
                    if (imageSet.Attributes[Constants.WTMLTileLevel] != null)
                    {
                        maxLevel = int.Parse(imageSet.Attributes[Constants.WTMLTileLevel].Value, CultureInfo.InvariantCulture);
                    }
                }
            }
            catch (IOException)
            {
                // Ignore all exceptions.
            }
            catch (System.Xml.XmlException)
            {
                // Ignore all exceptions.
            }
            catch (ArgumentException)
            {
                // Ignore all exceptions.
            }
            catch (UnauthorizedAccessException)
            {
                // Ignore all exceptions.
            }
            catch (NotSupportedException)
            {
                // Ignore all exceptions.
            }
            catch (System.Security.SecurityException)
            {
                // Ignore all exceptions.
            }
            return maxLevel;
        }

        /// <summary>
        /// Get the plate levels break up from Plates folder structure
        /// </summary>
        /// <param name="platesFolder">Plates Folder.</param>
        /// <returns>Returns plate levels break up.</returns>
        private static List<int> GetPlateLevels(DirectoryInfo platesFolder)
        {
            List<int> plateLevels = new List<int>();
            plateLevels.Add(0);
            foreach (DirectoryInfo plates in platesFolder.GetDirectories())
            {
                int level = 0;

                if (int.TryParse(plates.Name, out level))
                {
                    plateLevels.Add(level);
                }
            }

            return plateLevels;
        }
    }
}
