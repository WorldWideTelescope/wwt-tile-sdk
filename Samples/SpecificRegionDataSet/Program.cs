//---------------------------------------------------------------------
// <copyright file="Program.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation 2011. All rights reserved.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using Microsoft.Research.Wwt.Sdk.Core;

namespace Microsoft.Research.Wwt.Sdk.Samples
{
    /// <summary>
    /// This sample demonstrates how to create image tiles for a specific region of the world.
    /// It also shows how to add elevation model to visualize in 3-D.
    /// For this purpose, let's use Lower Glacier Bay, Alaska dataset.
    /// Data is available here - http://geopubs.wr.usgs.gov/open-file/of02-391/gb-data.html.
    /// The application can be invoked as,
    /// SpecificRegionDataSet /Input="Input file" /Projection="Projection Type - Mercator or Toast [/OutputDir="Output Directory"]"
    /// Input - Path to Glacier Bay XYZ dataset.
    /// Projection - Mercator or Toast.
    /// [OutputDir] - Folder path where output should be generated.
    /// </summary>
    /// <remarks>
    /// Install Sharing Service which is part of Worldwide Telescope SDK to create the web server for retrieving DEM tiles.
    /// Dataset used is http://geopubs.wr.usgs.gov/open-file/of02-391/data/bathy.zip.
    /// Use a standard gz uncompressing utility to get the XYZ file out.
    /// </remarks>
    public static class Program
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
                    // Create the image pyramid.
                    ProcessEquirectangularGrid(cmdLine.Input, cmdLine.OutputDir, cmdLine.ProjectionAsEnum);
                }
                catch (OutOfMemoryException ex)
                {
                    Trace.TraceError("{0}: " + ex.Message, DateTime.Now.TimeOfDay.ToString(@"hh\:mm\:ss", CultureInfo.InvariantCulture));
                }
                catch (Exception ex)
                {
                    Trace.TraceError("{0}: " + ex.ToString(), DateTime.Now.TimeOfDay.ToString(@"hh\:mm\:ss", CultureInfo.InvariantCulture));
                }
            }
        }

        /// <summary>
        /// Processes the input equirectangular dataset.
        /// </summary>
        /// <param name="inputGrid">
        /// Input image path.
        /// </param>
        /// <param name="outputDir">
        /// Output directory where pyramid is generated.
        /// </param>
        /// <param name="projection">
        /// Projection type.
        /// </param>
        private static void ProcessEquirectangularGrid(string inputGrid, string outputDir, ProjectionTypes projection)
        {
            Trace.TraceInformation("{0}: Reading dataset..", DateTime.Now.TimeOfDay.ToString(@"hh\:mm\:ss", CultureInfo.InvariantCulture));
            ImageFormat imageFormat = ImageFormat.Png;

            // Read and parse glacier bay dataset.
            // Define a color map with relief shading implemented.
            var datagridDetails = DataGridHelper.LoadFromFile(inputGrid);

            // Build a data grid using the input data set
            var dataGrid = new DataGrid(datagridDetails.Data, false);

            // Build the grid map for equirectangular projection using the data grid and boundary co-ordinates
            var equirectangularGridMap = new EquirectangularGridMap(dataGrid, datagridDetails.Boundary);

            // Build the color map using equirectangular projection grid map
            var dataColorMap = new ShadedReliefColorMap(equirectangularGridMap);

            var maximumLevelsOfDetail = 15;

            // Define an instance of ITileCreator to create image tiles.
            ITileCreator imageTileCreator = TileCreatorFactory.CreateImageTileCreator(dataColorMap, projection, outputDir);

            // Define an instance of ITileCreator to create DEM tiles. 
            // Define serialization mechanism for storing and retrieving DEM tiles.
            ITileCreator demTileCreator = TileCreatorFactory.CreateDemTileCreator(dataColorMap, projection, outputDir);

            // MultiTile creator encapsulates image and DEM tile creators.
            var multiTileCreator = new MultiTileCreator(new Collection<ITileCreator>() { imageTileCreator, demTileCreator }, projection);

            // Define boundary for the region.
            var boundary = new Boundary(datagridDetails.Boundary.Left, datagridDetails.Boundary.Top, datagridDetails.Boundary.Right, datagridDetails.Boundary.Bottom);
            if (projection == ProjectionTypes.Toast)
            {
                boundary.Left += 180.0;
                boundary.Right += 180.0;
            }

            // Generate base tiles and fill up the pyramid.
            var tileGenerator = new TileGenerator(multiTileCreator);
            tileGenerator.Generate(maximumLevelsOfDetail, boundary);

            // Path of Mercator and Toast DEM tile server.
            const string MercatorDemTilePath = @"http://(web server address)?Q={0},{1},{2},Mercator,dem2178";
            const string ToastDemTilePath = @"http://(web server address)?Q={0},{1},{2},Toast,dem1033";

            string fileName = Path.GetFileNameWithoutExtension(inputGrid);

            // Generate Thumbnail Images.
            Trace.TraceInformation("{0}: Building Thumbnail image..", DateTime.Now.TimeOfDay.ToString(@"hh\:mm\:ss", CultureInfo.InvariantCulture));
            ImageTileSerializer tileSerializer = new ImageTileSerializer(TileHelper.GetDefaultImageTilePathTemplate(outputDir), ImageFormat.Png);
            string thumbnailFile = Path.Combine(outputDir, fileName + ".jpeg");
            TileHelper.GenerateThumbnail(tileSerializer.GetFileName(0, 0, 0), 96, 45, thumbnailFile, ImageFormat.Jpeg);

            // Create and save WTML file.
            string textureTilePath = WtmlCollection.GetWtmlTextureTilePath(TileHelper.GetDefaultImageTilePathTemplate(outputDir), imageFormat.ToString());
            var inputBoundary = new Boundary(datagridDetails.Boundary.Left, datagridDetails.Boundary.Top, datagridDetails.Boundary.Right, datagridDetails.Boundary.Bottom);
            WtmlCollection wtmlCollection = new WtmlCollection(fileName, thumbnailFile, textureTilePath, maximumLevelsOfDetail, projection, inputBoundary);
            wtmlCollection.ZoomLevel = 0.2;
            wtmlCollection.IsElevationModel = true;
            wtmlCollection.DemTilePath = projection == ProjectionTypes.Mercator ? MercatorDemTilePath : ToastDemTilePath;
            string path = Path.Combine(outputDir, fileName + ".wtml");
            wtmlCollection.Save(path);
            Trace.TraceInformation("{0}: Collection successfully generated.", DateTime.Now.TimeOfDay.ToString(@"hh\:mm\:ss", CultureInfo.InvariantCulture));
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
            const string ErrorMessage = "Incorrect arguments...\nUsage: SpecificRegionDataSet "
                + "/Input=<Input file> "
                + "/Projection=<Projection Type - Mercator or Toast>"
                + "[/OutputDir=<Output Directory>] ";

            if (args == null || args.Length < 2)
            {
                Trace.TraceError("{0}: " + ErrorMessage, DateTime.Now.TimeOfDay.ToString(@"hh\:mm\:ss", CultureInfo.InvariantCulture));
                return null;
            }

            CommandLineArguments cmdLine = new CommandLineArguments(args);

            if (string.IsNullOrEmpty(cmdLine.Input))
            {
                Trace.TraceError("{0}: " + ErrorMessage, DateTime.Now.TimeOfDay.ToString(@"hh\:mm\:ss", CultureInfo.InvariantCulture));
                return null;
            }

            if (!string.IsNullOrEmpty(cmdLine.Projection))
            {
                ProjectionTypes tmp;
                if (!Enum.TryParse(cmdLine.Projection, true, out tmp))
                {
                    Trace.TraceError("{0}: " + ErrorMessage, DateTime.Now.TimeOfDay.ToString(@"hh\:mm\:ss", CultureInfo.InvariantCulture));
                    return null;
                }
            }
            else
            {
                Trace.TraceError("{0}: " + ErrorMessage, DateTime.Now.TimeOfDay.ToString(@"hh\:mm\:ss", CultureInfo.InvariantCulture));
                return null;
            }

            if (string.IsNullOrWhiteSpace(cmdLine.OutputDir))
            {
                string outputFolderName = string.Format(CultureInfo.InvariantCulture, "{0}_{1}_{2}", Path.GetFileNameWithoutExtension(cmdLine.Input), cmdLine.Projection, DateTime.Now.ToString("yyyyddMM-HHmmss", CultureInfo.InvariantCulture));
                cmdLine.OutputDir = Path.Combine(TileHelper.DefaultOutputDirectory, outputFolderName);
                Trace.TraceInformation("{0}: Output directory {1}", DateTime.Now.TimeOfDay.ToString(@"hh\:mm\:ss", CultureInfo.InvariantCulture), cmdLine.OutputDir);
            }

            if (!Path.IsPathRooted(cmdLine.Input))
            {
                cmdLine.Input = Path.GetFullPath(cmdLine.Input);
            }

            if (!Path.IsPathRooted(cmdLine.OutputDir))
            {
                cmdLine.OutputDir = Path.GetFullPath(cmdLine.OutputDir);
            }

            return cmdLine;
        }

        /// <summary>
        /// Encapsulates the command line arguments used.
        /// </summary>
        internal class CommandLineArguments
        {
            /// <summary>
            /// Initializes a new instance of the CommandLineArguments class.
            /// </summary>
            /// <param name="args">arguments parameter</param>
            public CommandLineArguments(string[] args)
            {
                if (args != null)
                {
                    var argsCollection = new Dictionary<string, string>();
                    //// Load arguments into a collection
                    foreach (string arg in args)
                    {
                        if (arg.IndexOf('=') > 0)
                        {
                            var argument = arg.TrimStart(new char[2] { '/', '-' });
                            var argumentKey = argument.Substring(0, argument.IndexOf('='));
                            var argumentValue = argument.Substring(argument.IndexOf('=') + 1);
                            if (!argsCollection.ContainsKey(argumentKey))
                            {
                                argsCollection.Add(argumentKey.ToUpperInvariant(), argumentValue);
                            }
                        }
                    }

                    //// Set properties based on collection key value
                    foreach (KeyValuePair<string, string> pair in argsCollection)
                    {
                        switch (pair.Key)
                        {
                            case "INPUT":
                                this.Input = pair.Value;
                                break;
                            case "OUTPUTDIR":
                                this.OutputDir = pair.Value;
                                break;
                            case "PROJECTION":
                                this.Projection = pair.Value;
                                break;
                        }
                    }
                }
            }

            /// <summary>
            /// Gets or sets the Input image path.
            /// </summary>
            public string Input { get; set; }

            /// <summary>
            /// Gets or sets the Output pyramid directory.
            /// </summary>
            public string OutputDir { get; set; }

            /// <summary>
            /// Gets or sets the projection.
            /// </summary>
            public string Projection { get; set; }

            /// <summary>
            /// Gets the projection type as an enumeration value.
            /// </summary>
            public ProjectionTypes ProjectionAsEnum
            {
                get
                {
                    return (ProjectionTypes)Enum.Parse(typeof(ProjectionTypes), this.Projection, true);
                }
            }
        }
    }
}