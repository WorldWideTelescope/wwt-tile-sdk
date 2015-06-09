//-----------------------------------------------------------------------
// <copyright file="BlueMarbleApp.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation 2011. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Microsoft.Research.Wwt.Sdk.Core;

namespace Microsoft.Research.Wwt.Sdk.Samples
{
    /// <summary>
    /// BlueMarble sample demonstrates how to visualize Equirectangular images in WWT.
    /// The input can be either a single equirectangular image or multiple images with each image covering a portion of the world.
    /// The application can be invoked as,
    /// BlueMarbleApp /Input="Input file" /Projection="Projection Type - Mercator or Toast" [/OutputDir="Output Directory"].
    ///     [/InputBoundary="Top Left Latitude, Top Left Longitude, Bottom Right Latitude, Bottom Right Longitude"]
    /// BlueMarble images can be downloaded from http://visibleearth.nasa.gov/view_rec.php?id=2429.
    /// </summary>
    /// <remarks>
    /// Single BlueMarble image can be downloaded from http://eoimages.gsfc.nasa.gov/ve//7100/world.topo.bathy.200401.3x5400x2700.jpg
    /// Multipart BlueMarble images can be downloaded from,
    /// http://eoimages.gsfc.nasa.gov/ve//7100/world.topo.bathy.200401.3x21600x21600.A1.jpg
    /// http://eoimages.gsfc.nasa.gov/ve//7100/world.topo.bathy.200401.3x21600x21600.A2.jpg
    /// http://eoimages.gsfc.nasa.gov/ve//7100/world.topo.bathy.200401.3x21600x21600.B1.jpg
    /// http://eoimages.gsfc.nasa.gov/ve//7100/world.topo.bathy.200401.3x21600x21600.B2.jpg
    /// http://eoimages.gsfc.nasa.gov/ve//7100/world.topo.bathy.200401.3x21600x21600.C1.jpg
    /// http://eoimages.gsfc.nasa.gov/ve//7100/world.topo.bathy.200401.3x21600x21600.C2.jpg
    /// http://eoimages.gsfc.nasa.gov/ve//7100/world.topo.bathy.200401.3x21600x21600.D1.jpg
    /// http://eoimages.gsfc.nasa.gov/ve//7100/world.topo.bathy.200401.3x21600x21600.D2.jpg
    /// Details about how the multipart image is composed can be seen here - http://earthobservatory.nasa.gov/Features/BlueMarble/bmng.pdf
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
            // Read, parse and validate command line arguments.
            CommandLineArguments cmdLine = Program.Initialize(args);
            if (cmdLine != null)
            {
                try
                {
                    if (cmdLine.Input.Trim().EndsWith(".xml", StringComparison.CurrentCultureIgnoreCase))
                    {
                        if (cmdLine.InputBoundaryAsBoundary != null)
                        {
                            ProcessMultipartEquirectangularImage(
                                cmdLine.Input,
                                cmdLine.OutputDir,
                                cmdLine.ProjectionAsEnum,
                                cmdLine.InputBoundaryAsBoundary ?? new Boundary(-180, -90, 180, 90));
                        }
                        else
                        {
                            // If the input is an XML file, it's a configuration file for multiple equirectangular images.
                            ProcessMultipartEquirectangularImage(cmdLine.Input, cmdLine.OutputDir, cmdLine.ProjectionAsEnum);
                        }
                    }
                    else
                    {
                        if (cmdLine.InputBoundaryAsBoundary != null)
                        {
                            ProcessEquirectangularImageSpecificRegion(
                                cmdLine.Input,
                                cmdLine.OutputDir,
                                cmdLine.ProjectionAsEnum,
                                cmdLine.InputBoundaryAsBoundary ?? new Boundary(-180, -90, 180, 90));
                        }
                        else
                        {
                            // Otherwise, let's try to process it as a single image.
                            ProcessEquirectangularImage(cmdLine.Input, cmdLine.OutputDir, cmdLine.ProjectionAsEnum);
                        }
                    }
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
        /// Processes the input equirectangular image.
        /// </summary>
        /// <param name="inputImage">
        /// Input image path.
        /// </param>
        /// <param name="outputDir">
        /// Output directory where pyramid is generated.
        /// </param>
        /// <param name="projection">
        /// Projection type.
        /// </param>
        private static void ProcessEquirectangularImage(string inputImage, string outputDir, ProjectionTypes projection)
        {
            Trace.TraceInformation("{0}: Reading image..", DateTime.Now.TimeOfDay.ToString(@"hh\:mm\:ss", CultureInfo.InvariantCulture));
            ImageFormat imageFormat = ImageFormat.Png;

            // Set the grid boundaries
            var imageBoundary = new Boundary(-180, -90, 180, 90);

            // Build an image grid using the input image
            var imageGrid = new ImageGrid(inputImage, true);

            // Build the grid map for equirectangular projection using the image grid and boundary co-ordinates
            var equirectangularGridMap = new EquirectangularGridMap(imageGrid, imageBoundary);

            // Build the color map using equirectangular projection grid map
            var imageColorMap = new ImageColorMap(equirectangularGridMap);

            var maximumLevelsOfDetail = TileHelper.CalculateMaximumLevel(imageGrid.Height, imageGrid.Width, imageBoundary);

            // Define ITileCreator instance for creating image tiles.
            ITileCreator tileCreator = TileCreatorFactory.CreateImageTileCreator(imageColorMap, projection, outputDir);

            // Define plumbing for looping through all the tiles to be created for base image and pyramid.
            var tileGenerator = new TileGenerator(tileCreator);

            // Start building base image and the pyramid.
            Trace.TraceInformation("{0}: Building base and parent levels...", DateTime.Now.TimeOfDay.ToString(@"hh\:mm\:ss", CultureInfo.InvariantCulture));
            tileGenerator.Generate(maximumLevelsOfDetail);

            string fileName = Path.GetFileNameWithoutExtension(inputImage);

            // Generate Plate file.
            Trace.TraceInformation("{0}: Building Plate file...", DateTime.Now.TimeOfDay.ToString(@"hh\:mm\:ss", CultureInfo.InvariantCulture));
            ImageTileSerializer pyramid = new ImageTileSerializer(TileHelper.GetDefaultImageTilePathTemplate(outputDir), ImageFormat.Png);
            PlateFileGenerator plateGenerator = new PlateFileGenerator(
                Path.Combine(outputDir, fileName + ".plate"),
                maximumLevelsOfDetail,
                ImageFormat.Png);
            plateGenerator.CreateFromImageTile(pyramid);

            // Generate Thumbnail Images.
            Trace.TraceInformation("{0}: Building Thumbnail image..", DateTime.Now.TimeOfDay.ToString(@"hh\:mm\:ss", CultureInfo.InvariantCulture));
            string thumbnailFile = Path.Combine(outputDir, fileName + ".jpeg");
            TileHelper.GenerateThumbnail(inputImage, 96, 45, thumbnailFile, ImageFormat.Jpeg);

            // Get the path of image tiles created and save it in WTML file.
            Trace.TraceInformation("{0}: Building WTML file..", DateTime.Now.TimeOfDay.ToString(@"hh\:mm\:ss", CultureInfo.InvariantCulture));
            string pyramidPath = WtmlCollection.GetWtmlTextureTilePath(TileHelper.GetDefaultImageTilePathTemplate(outputDir), imageFormat.ToString());

            // Create and save WTML collection file.
            WtmlCollection wtmlCollection = new WtmlCollection(fileName, thumbnailFile, pyramidPath, maximumLevelsOfDetail, projection);
            string path = Path.Combine(outputDir, fileName + ".wtml");
            wtmlCollection.Save(path);
            Trace.TraceInformation("{0}: Collection successfully generated.", DateTime.Now.TimeOfDay.ToString(@"hh\:mm\:ss", CultureInfo.InvariantCulture));
        }

        /// <summary>
        /// Processes the input equirectangular image.
        /// </summary>
        /// <param name="inputImage">
        /// Input image path.
        /// </param>
        /// <param name="outputDir">
        /// Output directory where pyramid is generated.
        /// </param>
        /// <param name="projection">
        /// Projection type.
        /// </param>
        /// <param name="inputBoundary">
        /// Input image boundary.
        /// </param>
        private static void ProcessEquirectangularImageSpecificRegion(string inputImage, string outputDir, ProjectionTypes projection, Boundary inputBoundary)
        {
            Trace.TraceInformation("{0}: Reading image..", DateTime.Now.TimeOfDay.ToString(@"hh\:mm\:ss", CultureInfo.InvariantCulture));
            ImageFormat imageFormat = ImageFormat.Png;

            // Check if the image is circular.
            double longitudeDelta = inputBoundary.Right - inputBoundary.Left;
            bool circular = (360.0 - longitudeDelta) < 0.000001;

            // Build an image grid using the input image
            var imageGrid = new ImageGrid(inputImage, circular);

            // Build the grid map for equirectangular projection using the image grid and boundary co-ordinates
            var equirectangularGridMap = new EquirectangularGridMap(imageGrid, inputBoundary);

            // Build the color map using equirectangular projection grid map
            var imageColorMap = new ImageColorMap(equirectangularGridMap);

            var maximumLevelsOfDetail = TileHelper.CalculateMaximumLevel(imageGrid.Height, imageGrid.Width, inputBoundary);

            // Define ITileCreator instance for creating image tiles.
            ITileCreator tileCreator = TileCreatorFactory.CreateImageTileCreator(imageColorMap, projection, outputDir);

            // Define plumbing for looping through all the tiles to be created for base image and pyramid.
            var tileGenerator = new TileGenerator(tileCreator);

            // Define bounds of the image. Image is assumed to cover the entire world.
            // If not, change the coordinates accordingly.
            // For Mercator projection, longitude spans from -180 to +180 and latitude from 90 to -90.
            Boundary gridBoundary = new Boundary(inputBoundary.Left, inputBoundary.Top, inputBoundary.Right, inputBoundary.Bottom);
            if (projection == ProjectionTypes.Toast)
            {
                // For Toast projection, longitude spans from 0 to +360 and latitude from 90 to -90.
                gridBoundary.Left += 180;
                gridBoundary.Right += 180;
            }

            // Start building base image and the pyramid.
            Trace.TraceInformation("{0}: Building base and parent levels...", DateTime.Now.TimeOfDay.ToString(@"hh\:mm\:ss", CultureInfo.InvariantCulture));
            tileGenerator.Generate(maximumLevelsOfDetail, gridBoundary);

            string fileName = Path.GetFileNameWithoutExtension(inputImage);

            // Generate Plate file.
            Trace.TraceInformation("{0}: Building Plate file...", DateTime.Now.TimeOfDay.ToString(@"hh\:mm\:ss", CultureInfo.InvariantCulture));
            ImageTileSerializer pyramid = new ImageTileSerializer(TileHelper.GetDefaultImageTilePathTemplate(outputDir), ImageFormat.Png);
            PlateFileGenerator plateGenerator = new PlateFileGenerator(
                Path.Combine(outputDir, fileName + ".plate"),
                maximumLevelsOfDetail,
                ImageFormat.Png);
            plateGenerator.CreateFromImageTile(pyramid);

            // Generate Thumbnail Images.
            Trace.TraceInformation("{0}: Building Thumbnail image..", DateTime.Now.TimeOfDay.ToString(@"hh\:mm\:ss", CultureInfo.InvariantCulture));
            string thumbnailFile = Path.Combine(outputDir, fileName + ".jpeg");
            TileHelper.GenerateThumbnail(inputImage, 96, 45, thumbnailFile, ImageFormat.Jpeg);

            // Get the path of image tiles created and save it in WTML file.
            Trace.TraceInformation("{0}: Building WTML file..", DateTime.Now.TimeOfDay.ToString(@"hh\:mm\:ss", CultureInfo.InvariantCulture));
            string pyramidPath = WtmlCollection.GetWtmlTextureTilePath(TileHelper.GetDefaultImageTilePathTemplate(outputDir), imageFormat.ToString());

            // Create and save WTML collection file.
            WtmlCollection wtmlCollection = new WtmlCollection(fileName, thumbnailFile, pyramidPath, maximumLevelsOfDetail, projection, inputBoundary);
            string path = Path.Combine(outputDir, fileName + ".wtml");
            wtmlCollection.Save(path);
            Trace.TraceInformation("{0}: Collection successfully generated.", DateTime.Now.TimeOfDay.ToString(@"hh\:mm\:ss", CultureInfo.InvariantCulture));
        }

        /// <summary>
        /// Processes a list of input image tiles and generates pyramid for level N.
        /// </summary>
        /// <param name="inputFilePath">Xml file with list of image tile information.</param>
        /// <param name="outputDir">Output directory where the image tiles of pyramid has to be stored.</param>
        /// <param name="projection">Projection to be used.</param>
        private static void ProcessMultipartEquirectangularImage(string inputFilePath, string outputDir, ProjectionTypes projection)
        {
            ImageFormat imageFormat = ImageFormat.Png;

            Trace.TraceInformation("{0}: Reading image..", DateTime.Now.TimeOfDay.ToString(@"hh\:mm\:ss", CultureInfo.InvariantCulture));

            // Get the list of equirectangular images input.
            string[,] imageTiles = Program.GetInputImageList(inputFilePath);

            // Set the grid boundaries
            var imageBoundary = new Boundary(-180, -90, 180, 90);

            // Build an image grid using the input images
            var imageGrid = new ImageGrid(imageTiles, true);

            // Build the grid map for equirectangular projection using the image grid and boundary co-ordinates
            var equirectangularGridMap = new EquirectangularGridMap(imageGrid, imageBoundary);

            // Build the color map using equirectangular projection grid map
            var imageColorMap = new ImageColorMap(equirectangularGridMap);

            var maximumLevelsOfDetail = TileHelper.CalculateMaximumLevel(imageGrid.Height, imageGrid.Width, imageBoundary);

            // Define ITileCreator instance for creating image tiles.
            ITileCreator tileCreator = TileCreatorFactory.CreateImageTileCreator(imageColorMap, projection, outputDir);

            // Define plumbing for looping through all the tiles to be created for base image and pyramid.
            var tileGenerator = new TileGenerator(tileCreator);

            // Start building base image and the pyramid.
            Trace.TraceInformation("{0}: Building base and parent levels...", DateTime.Now.TimeOfDay.ToString(@"hh\:mm\:ss", CultureInfo.InvariantCulture));
            tileGenerator.Generate(maximumLevelsOfDetail);

            string fileName = Path.GetFileNameWithoutExtension(inputFilePath);

            // Generate Plate file.
            Trace.TraceInformation("{0}: Building Plate file...", DateTime.Now.TimeOfDay.ToString(@"hh\:mm\:ss", CultureInfo.InvariantCulture));
            ImageTileSerializer pyramid = new ImageTileSerializer(TileHelper.GetDefaultImageTilePathTemplate(outputDir), ImageFormat.Png);
            PlateFileGenerator plateGenerator = new PlateFileGenerator(
                Path.Combine(outputDir, fileName + ".plate"),
                maximumLevelsOfDetail,
                ImageFormat.Png);
            plateGenerator.CreateFromImageTile(pyramid);

            // Generate Thumbnail Images.
            Trace.TraceInformation("{0}: Building Thumbnail image..", DateTime.Now.TimeOfDay.ToString(@"hh\:mm\:ss", CultureInfo.InvariantCulture));
            string thumbnailFile = Path.Combine(outputDir, fileName + ".jpeg");
            TileHelper.GenerateThumbnail(pyramid.GetFileName(0, 0, 0), 96, 45, thumbnailFile, ImageFormat.Jpeg);

            // Get the path of image tiles created and save it in WTML file.
            Trace.TraceInformation("{0}: Building WTML file..", DateTime.Now.TimeOfDay.ToString(@"hh\:mm\:ss", CultureInfo.InvariantCulture));
            string pyramidPath = WtmlCollection.GetWtmlTextureTilePath(TileHelper.GetDefaultImageTilePathTemplate(outputDir), imageFormat.ToString());

            // Create and save WTML collection file.
            WtmlCollection wtmlCollection = new WtmlCollection(fileName, thumbnailFile, pyramidPath, maximumLevelsOfDetail, projection);
            string path = Path.Combine(outputDir, fileName + ".wtml");
            wtmlCollection.Save(path);
            Trace.TraceInformation("{0}: Collection successfully generated.", DateTime.Now.TimeOfDay.ToString(@"hh\:mm\:ss", CultureInfo.InvariantCulture));
        }

        /// <summary>
        /// Processes a list of input image tiles and generates pyramid for level N.
        /// </summary>
        /// <param name="inputFilePath">Xml file with list of image tile information.</param>
        /// <param name="outputDir">Output directory where the image tiles of pyramid has to be stored.</param>
        /// <param name="projection">Projection to be used.</param>
        /// <param name="inputBoundary">Input image boundary.</param>
        private static void ProcessMultipartEquirectangularImage(string inputFilePath, string outputDir, ProjectionTypes projection, Boundary inputBoundary)
        {
            ImageFormat imageFormat = ImageFormat.Png;

            Trace.TraceInformation("{0}: Reading image..", DateTime.Now.TimeOfDay.ToString(@"hh\:mm\:ss", CultureInfo.InvariantCulture));

            // Get the list of equirectangular images input.
            string[,] imageTiles = Program.GetInputImageList(inputFilePath);

            // Check if the image is circular.
            double longitudeDelta = inputBoundary.Right - inputBoundary.Left;
            bool circular = (360.0 - longitudeDelta) < 0.000001;

            // Build an image grid using the input images
            var imageGrid = new ImageGrid(imageTiles, circular);

            // Build the grid map for equirectangular projection using the image grid and boundary co-ordinates
            var equirectangularGridMap = new EquirectangularGridMap(imageGrid, inputBoundary);

            // Build the color map using equirectangular projection grid map
            var imageColorMap = new ImageColorMap(equirectangularGridMap);

            var maximumLevelsOfDetail = TileHelper.CalculateMaximumLevel(imageGrid.Height, imageGrid.Width, inputBoundary);

            // Define ITileCreator instance for creating image tiles.
            ITileCreator tileCreator = TileCreatorFactory.CreateImageTileCreator(imageColorMap, projection, outputDir);

            // Define bounds of the image. Image is assumed to cover the entire world.
            // If not, change the coordinates accordingly.
            // For Mercator projection, longitude spans from -180 to +180 and latitude from 90 to -90.
            Boundary gridBoundary = new Boundary(inputBoundary.Left, inputBoundary.Top, inputBoundary.Right, inputBoundary.Bottom);
            if (projection == ProjectionTypes.Toast)
            {
                // For Toast projection, longitude spans from 0 to +360 and latitude from 90 to -90.
                gridBoundary.Left += 180;
                gridBoundary.Right += 180;
            }

            // Define plumbing for looping through all the tiles to be created for base image and pyramid.
            var tileGenerator = new TileGenerator(tileCreator);

            // Start building base image and the pyramid.
            Trace.TraceInformation("{0}: Building base and parent levels...", DateTime.Now.TimeOfDay.ToString(@"hh\:mm\:ss", CultureInfo.InvariantCulture));
            tileGenerator.Generate(maximumLevelsOfDetail, gridBoundary);

            string fileName = Path.GetFileNameWithoutExtension(inputFilePath);

            // Generate Plate file.
            Trace.TraceInformation("{0}: Building Plate file...", DateTime.Now.TimeOfDay.ToString(@"hh\:mm\:ss", CultureInfo.InvariantCulture));
            ImageTileSerializer pyramid = new ImageTileSerializer(TileHelper.GetDefaultImageTilePathTemplate(outputDir), ImageFormat.Png);
            PlateFileGenerator plateGenerator = new PlateFileGenerator(
                Path.Combine(outputDir, fileName + ".plate"),
                maximumLevelsOfDetail,
                ImageFormat.Png);
            plateGenerator.CreateFromImageTile(pyramid);

            // Generate Thumbnail Images.
            Trace.TraceInformation("{0}: Building Thumbnail image..", DateTime.Now.TimeOfDay.ToString(@"hh\:mm\:ss", CultureInfo.InvariantCulture));
            string thumbnailFile = Path.Combine(outputDir, fileName + ".jpeg");
            TileHelper.GenerateThumbnail(pyramid.GetFileName(0, 0, 0), 96, 45, thumbnailFile, ImageFormat.Jpeg);

            // Get the path of image tiles created and save it in WTML file.
            Trace.TraceInformation("{0}: Building WTML file..", DateTime.Now.TimeOfDay.ToString(@"hh\:mm\:ss", CultureInfo.InvariantCulture));
            string pyramidPath = WtmlCollection.GetWtmlTextureTilePath(TileHelper.GetDefaultImageTilePathTemplate(outputDir), imageFormat.ToString());

            // Create and save WTML collection file.
            WtmlCollection wtmlCollection = new WtmlCollection(fileName, thumbnailFile, pyramidPath, maximumLevelsOfDetail, projection, inputBoundary);
            string path = Path.Combine(outputDir, fileName + ".wtml");
            wtmlCollection.Save(path);
            Trace.TraceInformation("{0}: Collection successfully generated.", DateTime.Now.TimeOfDay.ToString(@"hh\:mm\:ss", CultureInfo.InvariantCulture));
        }

        /// <summary>
        /// Parses the input xml file and returns an array of input image file paths.
        /// Input XML should be in the format of M X N format where 
        ///     N = number of images in the horizontal direction
        ///     M = number of images in the vertical direction
        /// </summary>
        /// <param name="xmlFilePath">
        /// Xml file with list of input image tile details.
        /// </param>
        /// <returns>
        /// Two dimensional array of image filename.
        /// </returns>
        private static string[,] GetInputImageList(string xmlFilePath)
        {
            if (!File.Exists(xmlFilePath))
            {
                throw new FileNotFoundException("Invalid Input xml file");
            }

            string[,] imageList = null;

            XDocument xmlDoc = null;
            try
            {
                // Read and parse all the input images into ImageTile objects.
                xmlDoc = XDocument.Load(xmlFilePath);
                var rows = xmlDoc.Elements("Regions").Elements("Row");

                int rowCount = 0;
                int columnCount = 0;
                foreach (XElement row in rows)
                {
                    var regions = row.Elements("Region");
                    if (rowCount == 0)
                    {
                        imageList = new string[rows.Count(), regions.Count()];
                    }

                    columnCount = 0;
                    foreach (XElement region in regions)
                    {
                        imageList[rowCount, columnCount] = region.Element("ImagePath").Value;
                        columnCount++;
                    }

                    rowCount++;
                }
            }
            catch
            {
                throw;
            }
            finally
            {
                xmlDoc = null;
            }

            return imageList;
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
            const string ErrorMessage =
            "Incorrect arguments..\nUsage: BlueMarbleApp /Input=<Input file>"
            + " /Projection=<Projection Type - Mercator or Toast>  [/OutputDir=<Output Directory>]"
            + " [/InputBoundary=<Top Left Latitude, Top Left Longitude, Bottom Right Latitude, Bottom Right Longitude>]";

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
                ProjectionTypes projection;
                if (!Enum.TryParse(cmdLine.Projection, true, out projection))
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
                            case "INPUTBOUNDARY":
                                this.InputBoundary = pair.Value;
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
            /// Gets or sets the input boundary.
            /// </summary>
            public string InputBoundary { get; set; }

            /// <summary>
            /// Gets the input boundary.
            /// </summary>
            public Boundary InputBoundaryAsBoundary
            {
                get
                {
                    return ParseBoundary(this.InputBoundary);
                }
            }

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
            /// Converts the string representation of the boundary to Boundary object.
            /// </summary>
            /// <param name="boundary">
            /// String representation of the boundary
            /// </param>
            /// <returns>
            /// An object of type Boundary.
            /// </returns>
            private static Boundary ParseBoundary(string boundary)
            {
                if (string.IsNullOrWhiteSpace(boundary))
                {
                    return null;
                }

                string[] points = boundary.Split(',');
                if (points.Length != 4)
                {
                    return null;
                }

                return new Boundary(
                    double.Parse(points[1], CultureInfo.InvariantCulture),
                    double.Parse(points[2], CultureInfo.InvariantCulture),
                    double.Parse(points[3], CultureInfo.InvariantCulture),
                    double.Parse(points[0], CultureInfo.InvariantCulture));
            }
        }
    }
}