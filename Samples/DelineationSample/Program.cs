//-----------------------------------------------------------------------
// <copyright file="Program.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation 2011. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.IO;
using Microsoft.Research.Wwt.Sdk.Core;
using Microsoft.Research.Wwt.Sdk.Utilities;
using Microsoft.Test.CommandLineParsing;

namespace Microsoft.Research.Wwt.Sdk.Samples
{
    /// <summary>
    /// This sample demonstrates how to delineate an image map using geographical region shape files.
    /// For this purpose, let's use NOAA ETOPO1 dataset and delineate the continent boundaries using a continent shape file from ESRI.
    /// NOAA data is available here - http://www.ngdc.noaa.gov/mgg/global/global.html.
    /// Shape files are available here - http://pubs.usgs.gov/of/2006/1187/basemaps/continents/continent_faq.htm.
    /// The application can be invoked as,
    /// DelineationSample /Input="Input file" /ColorMapOrientation "Horizontal or Vertical" /ColorMap="Color Map File"
    ///                    /ShapeFilePath="Shape file path" /RegionHvMapPath="Region map path" /ShapeKey="string key used to access shape file"
    ///                    /MaxLevel="Number of levels of tiles to be built."
    ///                    [/OutputDir="Output Directory"]
    /// Input - Path to NOAA ETOPO1 dataset.
    /// ColorMap - Path of color map file used. This is the lookup table used to associate color to values.
    /// ColorMapOrientation - Horizontal, if the color map should be looked up horizontally. Vertical, otherwise.
    /// ShapeFilePath - Path to the shape file.
    /// RegionHvMapPath - H and V values for the desired regions. Similar to the "RegionHVMap.txt" in this project.
    /// ShapeKey - The string key used to retrieve shapes from the shape file. The key for continents shapefile is "continent".
    /// MaxLevel - Maximum number of levels of pyramids desired.
    /// [OutputDir] - Folder path where output should be generated.
    /// </summary>
    /// <remarks>
    /// Note that ShapeFile and RegionHvMap are tightly coupled. This sample uses a continents shape file and associated values in RegionHvMapPath.
    /// If a different shape file is used, RegionHvMap.txt should be updated accordingly.
    /// </remarks>
    public class Program
    {
        /// <summary>
        /// Entry point.
        /// </summary>
        /// <param name="args">
        /// Command line arguments.
        /// </param>
        public static void Main(string[] args)
        {
            // Read, parse and validate commandline arguments.
            CommandLineArguments cmdLine = Program.Initialize(args);
            if (cmdLine != null)
            {
                try
                {
                    // Build image pyramid from shape files.
                    string referenceImagePath = Path.Combine(cmdLine.OutputDir, "ReferenceImageTiles");
                    Program.GenerateReferenceImageTiles(cmdLine.ShapeFilePath, cmdLine.RegionHvMapPath, cmdLine.ShapeKey, referenceImagePath, cmdLine.MaxLevel);

                    // Build NOAA tiles and use shape tile files to delineate them.
                    Program.DelineateImage(cmdLine.Input, cmdLine.OutputDir, referenceImagePath, cmdLine.ColorMap, cmdLine.ColorMapOrientationAsEnum, cmdLine.MaxLevel);
                }
                catch (OutOfMemoryException ex)
                {
                    Trace.TraceError("{0}: " + ex.Message, DateTime.Now.TimeOfDay.ToString(@"hh\:mm\:ss"));
                }
                catch (Exception ex)
                {
                    Trace.TraceError("{0}: " + ex.Message, DateTime.Now.TimeOfDay.ToString(@"hh\:mm\:ss"));
                }
            }
        }

        /// <summary>
        /// Generates reference image tiles.
        /// </summary>
        /// <param name="shapeFilePath">Shape file path.</param>
        /// <param name="regionHvMapPath">Region HV map file path.</param>
        /// <param name="shapeKey">Shape key used.</param>
        /// <param name="outputDir">Output directory where the reference image tiles have to be stored.</param>
        /// <param name="maxLevel">Maximum level of the pyramid.</param>
        private static void GenerateReferenceImageTiles(string shapeFilePath, string regionHvMapPath, string shapeKey, string outputDir, int maxLevel)
        {
            // Create output folder.
            if (!Directory.Exists(outputDir))
            {
                Directory.CreateDirectory(outputDir);
            }

            // Define Color map.
            var shapeGrid = new ShapeValueMap(shapeFilePath, regionHvMapPath, shapeKey);
            var colorMap = new ShapeColorMap(shapeGrid);

            // Define serializer.
            var tileSerializer = new ImageTileSerializer(TileHelper.GetDefaultImageTilePathTemplate(outputDir), ImageFormat.Png);

            // Create and configure the tile creator.
            var tileCreator = new ShapeTileCreator(tileSerializer, colorMap, ProjectionTypes.Toast, maxLevel);

            // Generate shape file tiles.
            Trace.TraceInformation("{0}: Creating the reference image tiles..", DateTime.Now.TimeOfDay.ToString(@"hh\:mm\:ss"));
            var tileGenerator = new TileGenerator(tileCreator);
            tileGenerator.Generate(maxLevel);
            Trace.TraceInformation("{0}: Reference Image generation completed.", DateTime.Now.TimeOfDay.ToString(@"hh\:mm\:ss"));
        }

        /// <summary>
        /// Perform delineation for the specified region.
        /// </summary>
        /// <param name="inputGrid">Input data set.</param>
        /// <param name="outputDir">Output directory where the delineationated image tiles have to be persisted.</param>
        /// <param name="referenceImagePath">Reference image tiles folder path.</param>
        /// <param name="colorMapPath">Color map file path.</param>
        /// <param name="orientation">Color orientation.</param>
        /// <param name="maxLevel">Maximum level of the pyramid.</param>
        private static void DelineateImage(string inputGrid, string outputDir, string referenceImagePath, string colorMapPath, ColorMapOrientation orientation, int maxLevel)
        {
            Trace.TraceInformation("{0}: Starting delineation..", DateTime.Now.TimeOfDay.ToString(@"hh\:mm\:ss"));
            Trace.TraceInformation("{0}: Reading dataset..", DateTime.Now.TimeOfDay.ToString(@"hh\:mm\:ss"));
            ImageFormat imageFormat = ImageFormat.Png;

            // Read and parse the input dataset.
            var equirectangularGrid = new EquirectangularGrid(inputGrid);
            equirectangularGrid.MaximumLevelsOfDetail = maxLevel;

            // Define a color map to lookup color pixel for double values.
            var colorMap = new EquirectangularColorMap(colorMapPath, equirectangularGrid, orientation);

            // Define serialization mechanism for storing/retrieving reference tiles.
            var referenceTileSerializer = new ImageTileSerializer(TileHelper.GetDefaultImageTilePathTemplate(referenceImagePath), ImageFormat.Png);

            // Define an instance of ITielCreator to create the tiles.
            var tileSerializer = new ImageTileSerializer(TileHelper.GetDefaultImageTilePathTemplate(outputDir), ImageFormat.Png);
            var imageTileCreator = new MaskedTileCreator(colorMap, tileSerializer, true);

            // Start building the image tiles.
            var tileGenerator = new TileGenerator(imageTileCreator);
            imageTileCreator.ReferenceTileSerializer = referenceTileSerializer;
            tileGenerator.Generate(maxLevel);

            string fileName = Path.GetFileNameWithoutExtension(inputGrid);

            // Generate Thumbnail Images.
            string thumbnailFile = Path.Combine(outputDir, fileName + ".jpeg");
            TileHelper.GenerateThumbnail(tileSerializer.GetFileName(0, 0, 0), 96, 45, thumbnailFile);

            // Create and save WTML file.
            string textureTilePath = WtmlCollection.GetWtmlTextureTilePath(TileHelper.GetDefaultImageTilePathTemplate(outputDir), imageFormat.ToString());
            WtmlCollection wtmlCollection = new WtmlCollection(fileName, thumbnailFile, textureTilePath, equirectangularGrid.MaximumLevelsOfDetail, ProjectionTypes.Toast);
            string path = Path.Combine(outputDir, fileName + ".wtml");
            wtmlCollection.Save(path);
            Trace.TraceInformation("{0}: Collection successfully generated.", DateTime.Now.TimeOfDay.ToString(@"hh\:mm\:ss"));
        }

        /// <summary>
        /// Parses the command line arguments and initializes the object.
        /// </summary>
        /// <param name="args">
        /// Command line arguments.
        /// </param>
        /// <returns>
        /// Parsed command line arguments.
        /// </returns>
        private static CommandLineArguments Initialize(string[] args)
        {
            const string ErrorMessage = "Incorrect arguments..\nUsage: DelineationSample /Input=<Input file> " +
                "/ColorMap=<Color Map File> /ColorMapOrientation <Horizontal or Vertical> " +
                "/ShapeFilePath=<Shape file path> /RegionHvMapPath=<Region map path> /ShapeKey=<string key used to access shape file> " +
                "/MaxLevel=<Number of levels of tiles to be built.> [/OutputDir=<Output Directory>]";

            if (args == null || args.Length < 6)
            {
                Trace.TraceError("{0}: " + ErrorMessage, DateTime.Now.TimeOfDay.ToString(@"hh\:mm\:ss"));
                return null;
            }

            CommandLineArguments cmdLine = new CommandLineArguments();
            CommandLineParser.ParseArguments(cmdLine, args);
            if (string.IsNullOrWhiteSpace(cmdLine.Input) || string.IsNullOrWhiteSpace(cmdLine.ColorMap))
            {
                Trace.TraceError("{0}: " + ErrorMessage, DateTime.Now.TimeOfDay.ToString(@"hh\:mm\:ss"));
                return null;
            }

            if (string.IsNullOrWhiteSpace(cmdLine.OutputDir))
            {
                string outputFolderName = string.Format("{0}_{1}", Path.GetFileNameWithoutExtension(cmdLine.Input), DateTime.Now.ToString("yyyyddMM-HHmmss"));
                cmdLine.OutputDir = Path.Combine(TileHelper.GetDefaultOutputDirectory(), outputFolderName);
                Trace.TraceInformation("{0}: Output directory {1}", DateTime.Now.TimeOfDay.ToString(@"hh\:mm\:ss"), cmdLine.OutputDir);
            }

            if (!Path.IsPathRooted(cmdLine.Input))
            {
                cmdLine.Input = Path.GetFullPath(cmdLine.Input);
            }

            if (!Path.IsPathRooted(cmdLine.OutputDir))
            {
                cmdLine.OutputDir = Path.GetFullPath(cmdLine.OutputDir);
            }

            if (!Path.IsPathRooted(cmdLine.ColorMap))
            {
                cmdLine.ColorMap = Path.GetFullPath(cmdLine.ColorMap);
            }

            if (!string.IsNullOrEmpty(cmdLine.ColorMapOrientation))
            {
                ColorMapOrientation orientation;
                if (!Enum.TryParse(cmdLine.ColorMapOrientation, true, out orientation))
                {
                    Trace.TraceError("{0}: " + ErrorMessage, DateTime.Now.TimeOfDay.ToString(@"hh\:mm\:ss"));
                    return null;
                }
            }

            return cmdLine;
        }

        /// <summary>
        /// Encapsulates the command line arguments used.
        /// </summary>
        public class CommandLineArguments
        {
            /// <summary>
            /// Gets or sets the Input image path.
            /// </summary>
            public string Input { get; set; }

            /// <summary>
            /// Gets or sets the Output pyramid directory.
            /// </summary>
            public string OutputDir { get; set; }

            /// <summary>
            /// Gets or sets the color map file path.
            /// </summary>
            public string ColorMap { get; set; }

            /// <summary>
            /// Gets or sets the color map orientation.
            /// </summary>
            public string ColorMapOrientation { get; set; }

            /// <summary>
            /// Gets or sets the Shape file path.
            /// </summary>
            public string ShapeFilePath { get; set; }

            /// <summary>
            /// Gets or sets the Region Map path.
            /// </summary>
            public string RegionHvMapPath { get; set; }

            /// <summary>
            /// Gets or sets the Shape key used.
            /// </summary>
            public string ShapeKey { get; set; }

            /// <summary>
            /// Gets or sets the Maximum level of details.
            /// </summary>
            public int MaxLevel { get; set; }

            /// <summary>
            /// Gets the color map orientation as Enum.
            /// </summary>
            public ColorMapOrientation ColorMapOrientationAsEnum
            {
                get
                {
                    return (ColorMapOrientation)Enum.Parse(typeof(ColorMapOrientation), this.ColorMapOrientation, true);
                }
            }
        }
    }
}