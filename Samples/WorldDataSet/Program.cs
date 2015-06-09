//---------------------------------------------------------------------
// <copyright file="Program.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation 2011. All rights reserved.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using Microsoft.Research.Wwt.Sdk.Core;

namespace Microsoft.Research.Wwt.Sdk.Samples
{
    /// <summary>
    /// This sample demonstrates how to create image tiles for the entire world.
    /// For this purpose, let's use NOAA ETOPO1 XYZ dataset available here - http://www.ngdc.noaa.gov/mgg/global/global.html.
    /// The application can be invoked as,
    /// WorldDataSet /Input="Input file" /Projection="Projection Type - Mercator or Toast"
    ///              /ColorMap="Color Map File" /ColorMapOrientation="Horizontal or Vertical [/OutputDir="Output Directory"]"
    /// Input - Path to NOAA ETOPO1 XYZ dataset.
    /// Projection - Mercator or Toast.
    /// ColorMap - Path of color map file used. This is the lookup table used to associate color to values.
    /// ColorMapOrientation - Horizontal, if the color map should be looked up horizontally. Vertical, otherwise.
    /// [OutputDir] - Folder path where output should be generated.
    /// </summary>
    /// <remarks>
    /// Dataset used is http://www.ngdc.noaa.gov/mgg/global/relief/ETOPO1/data/bedrock/grid_registered/xyz/ETOPO1_Bed_g_int.xyz.gz
    /// Use a standard gz uncompressing utility to get the XYZ file out.
    /// </remarks>
    public static class Program
    {
        public static void Main(string[] args)
        {
            // Read, parse and validate commandline arguments.
            CommandLineArguments cmdLine = Program.Initialize(args);
            if (cmdLine != null)
            {
                try
                {
                    // Build the image pyramid.
                    ProcessEquirectangularGrid(cmdLine.Input, cmdLine.OutputDir, cmdLine.ColorMap, cmdLine.ColorMapOrientationAsEnum, cmdLine.ProjectionAsEnum);
                }
                catch (OutOfMemoryException ex)
                {
                    Trace.TraceError("{0}: " + ex.Message, DateTime.Now.TimeOfDay.ToString(@"hh\:mm\:ss", CultureInfo.InvariantCulture));
                }
                catch (Exception ex)
                {
                    Trace.TraceError("{0}: " + ex.Message, DateTime.Now.TimeOfDay.ToString(@"hh\:mm\:ss", CultureInfo.InvariantCulture));
                }
            }
        }

        /// <summary>
        /// Processes the input equirectangular dataset.
        /// </summary>
        /// <param name="inputFile">
        /// Input File path.
        /// </param>
        /// <param name="outputDir">
        /// Output directory where pyramid is generated.
        /// </param>
        /// <param name="colorMapPath">
        /// Map of color map file.
        /// </param>
        /// <param name="orientation">
        /// Orientation of color map file.
        /// </param>
        /// <param name="projection">
        /// Projection type.
        /// </param>
        public static void ProcessEquirectangularGrid(string inputFile, string outputDir, string colorMapPath, ColorMapOrientation orientation, ProjectionTypes projection)
        {
            Trace.TraceInformation("{0}: Reading input dataset...", DateTime.Now.TimeOfDay.ToString(@"hh\:mm\:ss", CultureInfo.InvariantCulture));
            
            // Get grid details from the input file
            var dataGridDetails = DataGridHelper.LoadFromFile(inputFile);

            // Populate the data grid using grid details
            var dataGrid = new DataGrid(dataGridDetails.Data, true);

            // Build a equirectangular projection grid map using the data grid and boundary details
            var equirectangularGridMap = new EquirectangularGridMap(dataGrid, dataGridDetails.Boundary);

            // Build a color map using equirectangular projection grid map 
            var dataColorMap = new DataColorMap(colorMapPath, equirectangularGridMap, orientation, dataGridDetails.MinimumThreshold, dataGridDetails.MaximumThreshold);

            // Define an instance of ITileCreator to create image tiles.
            ITileCreator imageTileCreator = TileCreatorFactory.CreateImageTileCreator(dataColorMap, projection, outputDir);

            Trace.TraceInformation("{0}: Building base and parent levels...", DateTime.Now.TimeOfDay.ToString(@"hh\:mm\:ss", CultureInfo.InvariantCulture));
            TileGenerator tileGenerator = new TileGenerator(imageTileCreator);
            int zoomLevels = 5;
            tileGenerator.Generate(zoomLevels);

            string fileName = Path.GetFileNameWithoutExtension(inputFile);

            // Generate Thumbnail Images.
            Trace.TraceInformation("{0}: Building Thumbnail image..", DateTime.Now.TimeOfDay.ToString(@"hh\:mm\:ss", CultureInfo.InvariantCulture));
            ImageTileSerializer tileSerializer = new ImageTileSerializer(TileHelper.GetDefaultImageTilePathTemplate(outputDir), ImageFormat.Png);
            string thumbnailFile = Path.Combine(outputDir, fileName + ".jpeg");
            TileHelper.GenerateThumbnail(tileSerializer.GetFileName(0, 0, 0), 96, 45, thumbnailFile, ImageFormat.Jpeg);

            // Create and save WTML file.
            Trace.TraceInformation("{0}: Generating WTML file...", DateTime.Now.TimeOfDay.ToString(@"hh\:mm\:ss", CultureInfo.InvariantCulture));
            string textureTilePath = WtmlCollection.GetWtmlTextureTilePath(TileHelper.GetDefaultImageTilePathTemplate(outputDir), ImageFormat.Png.ToString());
            WtmlCollection wtmlCollection = new WtmlCollection(fileName, thumbnailFile, textureTilePath, zoomLevels, projection);
            wtmlCollection.IsElevationModel = false;
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
            const string ErrorMessage = "Incorrect arguments...\nUsage: WorldDataSet "
                + "/Input=<Input file name> "
                + "/Projection=<Projection Type - Mercator or Toast> "
                + "/ColorMap=<Color Map File> /ColorMapOrientation=<Horizontal or Vertical>"
                + "[/OutputDir=<Output Directory>] ";

            if (args == null || args.Length < 4)
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

            if (!string.IsNullOrEmpty(cmdLine.ColorMapOrientation))
            {
                ColorMapOrientation tmp;
                if (!Enum.TryParse(cmdLine.ColorMapOrientation, true, out tmp))
                {
                    Trace.TraceError("{0}: " + ErrorMessage, DateTime.Now.TimeOfDay.ToString(@"hh\:mm\:ss", CultureInfo.InvariantCulture));
                    return null;
                }
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
                            case "COLORMAP":
                                this.ColorMap = pair.Value;
                                break;
                            case "COLORMAPORIENTATION":
                                this.ColorMapOrientation = pair.Value;
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
            /// Gets or sets the color map file path.
            /// </summary>
            public string ColorMap { get; set; }

            /// <summary>
            /// Gets or sets the color map orientation.
            /// </summary>
            public string ColorMapOrientation { get; set; }

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

            /// <summary>
            /// Gets the color map orientation as an enumeration value.
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
