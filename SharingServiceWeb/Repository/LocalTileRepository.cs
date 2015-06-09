//-----------------------------------------------------------------------
// <copyright file="LocalTileRepository.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation 2011. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security;
using System.ServiceModel;
using System.Web.Hosting;
using System.Xml;
using System.Xml.XPath;
using Microsoft.Research.Wwt.Sdk.Core;

namespace Microsoft.Research.Wwt.SharingService.Web
{
    /// <summary>
    /// This class is responsible for the local tile repository.
    /// </summary>
    public class LocalTileRepository : ITileRepository
    {
        #region Private Properties

        private string pyramidLocation;

        #endregion

        /// <summary>
        /// Gets or sets pyramid location
        /// </summary>
        /// <returns>Pyramid folder location</returns>
        public string PyramidLocation
        {
            get
            {
                if (string.IsNullOrWhiteSpace(pyramidLocation))
                {
                    pyramidLocation = ConfigurationManager.AppSettings[Constants.TileServiceInputDirectory];
                    if (string.IsNullOrWhiteSpace(pyramidLocation))
                    {
                        pyramidLocation = TileHelper.DefaultOutputDirectory;
                    }
                }
                return pyramidLocation;
            }
            set
            {
                pyramidLocation = value;
            }
        }

        /// <summary>
        /// Gets all pyramid data for the service. 
        /// </summary>
        /// <returns>Details for all pyramids.</returns>
        public Collection<Pyramid> GetPyramidDetails()
        {
            Collection<Pyramid> imageDetails = new Collection<Pyramid>();
            try
            {
                if (!string.IsNullOrWhiteSpace(this.PyramidLocation))
                {
                    DirectoryInfo directory = new DirectoryInfo(this.PyramidLocation);

                    if (directory.Exists)
                    {
                        foreach (DirectoryInfo dirInfo in directory.GetDirectories())
                        {
                            FileInfo file = dirInfo.GetFiles(Constants.WTMLSearchPattern).FirstOrDefault();
                            FileInfo plateFile = dirInfo.GetFiles(Constants.PlateFileSearchPattern).FirstOrDefault();
                            DirectoryInfo pyramidFolder = dirInfo.GetDirectories(Constants.PyramidFolder).FirstOrDefault();
                            DirectoryInfo platesFolder = dirInfo.GetDirectories(Constants.PlatesFolder).FirstOrDefault();
                            if (file != null && (plateFile != null || pyramidFolder != null || platesFolder != null))
                            {
                                var wtmlDetails = GetWtmlDetails(file.FullName);
                                if (wtmlDetails != null)
                                {
                                    wtmlDetails.DateCreated = file.CreationTime;
                                    imageDetails.Add(new Pyramid
                                    {
                                        Id = dirInfo.Name,
                                        Name = dirInfo.Name,
                                        WtmlDetails = wtmlDetails
                                    });
                                }
                            }
                        }
                    }
                }
                else
                {
                    // Throw a fault exception which will be handled by the tile service sample aspx page.
                    throw new FaultException(Properties.Resources.PyramidPathNotSet);
                }
            }
            catch (SecurityException ex)
            {
                ErrorHandler.LogException(ex);
                throw new FaultException(ex.Message);
            }
            catch (UnauthorizedAccessException ex)
            {
                ErrorHandler.LogException(ex);
                throw new FaultException(ex.Message);
            }
            catch (ArgumentException ex)
            {
                ErrorHandler.LogException(ex);
                throw new FaultException(ex.Message);
            }
            catch (IOException ex)
            {
                ErrorHandler.LogException(ex);
                throw new FaultException(ex.Message);
            }
            catch (FaultException)
            {
                throw;
            }
            return imageDetails;
        }

        /// <summary>
        /// Gets tile image for the specified tile id.
        /// </summary>
        /// <param name="id">Tile details id.</param>
        /// <param name="level">Level of the image.</param>
        /// <param name="x">X axis image.</param>
        /// <param name="y">Y axis image.</param>
        /// <returns>Tile image for the specified image.</returns>
        public Stream GetTileImage(string id, int level, int x, int y)
        {
            Stream outputStream = null;
            try
            {
                string pyramidFolderPath = Path.Combine(this.PyramidLocation, id, Constants.PyramidFolder);
                string platesFolderPath = Path.Combine(this.PyramidLocation, id, Constants.PlatesFolder);
                DirectoryInfo pyramidFolder = new DirectoryInfo(pyramidFolderPath);
                DirectoryInfo platesFolder = new DirectoryInfo(platesFolderPath);

                if (pyramidFolder.Exists)
                {
                    string pyramidPath = Path.Combine(this.PyramidLocation, id, Constants.TileImagePath);

                    string filename = string.Format(CultureInfo.InvariantCulture, pyramidPath, level, x, y, ImageFormat.Png.ToString());

                    if (File.Exists(filename))
                    {
                        using (Bitmap bmp = new Bitmap(filename, false))
                        {
                            if (bmp != null)
                            {
                                outputStream = new MemoryStream();
                                bmp.Save(outputStream, ImageFormat.Png);
                                outputStream.Seek(0, SeekOrigin.Begin);
                            }
                        }
                    }
                }
                else if (platesFolder.Exists)
                {
                    outputStream = PlateFileHelper.GetTileFromMultiplePlates(level, x, y, platesFolder.FullName);
                }
                else
                {
                    // If Pyramid folder is not available, then try to read from plate file.
                    FileInfo plateFile = pyramidFolder.Parent.GetFiles(Constants.PlateFileSearchPattern).FirstOrDefault();
                    if (plateFile != null && plateFile.Exists)
                    {
                        PlateFile plateFileInstance = new PlateFile(plateFile.FullName, level);
                        outputStream = plateFileInstance.GetFileStream(level, x, y);
                    }
                }
            }
            catch (ArgumentException ex)
            {
                ErrorHandler.LogException(ex);
                throw new FaultException(ex.Message);
            }
            catch (IOException ex)
            {
                ErrorHandler.LogException(ex);
                throw new FaultException(ex.Message);
            }
            catch (ObjectDisposedException ex)
            {
                ErrorHandler.LogException(ex);
                throw new FaultException(ex.Message);
            }
            return outputStream;
        }

        /// <summary>
        /// Gets Dem for the specified tile id.
        /// </summary>
        /// <param name="id">Tile details id.</param>
        /// <param name="level">Level of the image.</param>
        /// <param name="x">X axis image.</param>
        /// <param name="y">Y axis image.</param>
        /// <returns>Dem for the specified image.</returns>
        public Stream GetDem(string id, int level, int x, int y)
        {
            Stream outputStream = null;
            try
            {
                string pyramidFolderPath = Path.Combine(this.PyramidLocation, id, Constants.PyramidFolder);
                string demPlatesFolderPath = Path.Combine(this.PyramidLocation, id, Constants.DEMPlatesFolder);
                DirectoryInfo pyramidFolder = new DirectoryInfo(pyramidFolderPath);
                DirectoryInfo platesFolder = new DirectoryInfo(demPlatesFolderPath);

                if (pyramidFolder.Exists)
                {
                    string pyramidPath = Path.Combine(this.PyramidLocation, id, Constants.DemTilePath);

                    string filename = string.Format(CultureInfo.InvariantCulture, pyramidPath, level, x, y, Constants.DemExtension);
                    if (File.Exists(filename))
                    {
                        byte[] data = null;
                        using (Stream s = File.OpenRead(filename))
                        {
                            int length = (int)s.Length;
                            data = new byte[length];
                            s.Read(data, 0, length);
                            outputStream = new MemoryStream(data);
                        }
                    }
                }
                else if (platesFolder.Exists)
                {
                    // Get from multiple plates
                    outputStream = PlateFileHelper.GetDEMTileFromMultiplePlates(level, x, y, platesFolder.FullName);
                }
                else
                {
                    // If Pyramid folder is not available, then try to read from plate file.
                    FileInfo plateFile = pyramidFolder.Parent.GetFiles(Constants.PlateFileSearchPattern).FirstOrDefault();
                    if (plateFile != null && plateFile.Exists)
                    {
                        PlateFile plateFileInstance = new PlateFile(plateFile.FullName, level);
                        outputStream = plateFileInstance.GetFileStream(level, x, y);
                    }
                }
            }
            catch (ArgumentException ex)
            {
                ErrorHandler.LogException(ex);
                throw new FaultException(ex.Message);
            }
            catch (IOException ex)
            {
                ErrorHandler.LogException(ex);
                throw new FaultException(ex.Message);
            }
            catch (ObjectDisposedException ex)
            {
                ErrorHandler.LogException(ex);
                throw new FaultException(ex.Message);
            }

            return outputStream;
        }

        /// <summary>
        /// Gets thumbnail image for the pyramid id.
        /// </summary>
        /// <param name="id">Pyramid Id.</param>
        /// <param name="name">WTML Name of the Pyramid.</param>
        /// <returns>Image in the stream.</returns>
        public Stream GetThumbnailImage(string id, string name)
        {
            string pyramidPath = Path.Combine(this.PyramidLocation, id, Constants.ThumbNailImagePath);
            string filename = string.Format(CultureInfo.InvariantCulture, pyramidPath, name, ImageFormat.Jpeg);

            MemoryStream memoryStream = null;
            try
            {
                if (!File.Exists(filename))
                {
                    filename = HostingEnvironment.ApplicationPhysicalPath + Constants.DefaultImage;
                }
                using (Bitmap bmp = new Bitmap(filename, false))
                {
                    if (bmp != null)
                    {
                        memoryStream = new MemoryStream();
                        bmp.Save(memoryStream, ImageFormat.Jpeg);
                        memoryStream.Seek(0, SeekOrigin.Begin);
                    }
                }
            }
            catch (ArgumentException ex)
            {
                ErrorHandler.LogException(ex);
                throw new FaultException(ex.Message);
            }
            catch (IOException ex)
            {
                ErrorHandler.LogException(ex);
                throw new FaultException(ex.Message);
            }
            catch (ObjectDisposedException ex)
            {
                ErrorHandler.LogException(ex);
                throw new FaultException(ex.Message);
            }
            return memoryStream;
        }

        /// <summary>
        /// Get WTML file for the pyramid.
        /// </summary>
        /// <param name="id">Pyramid id.</param>
        /// <param name="name">Name of the WTML file.</param>
        /// <returns>WTML file for the pyramid.</returns>
        public IXPathNavigable GetWtmlFile(string id, string name)
        {
            XmlDocument xmlDoc = null;
            try
            {
                string dir = Path.Combine(this.PyramidLocation, id);
                DirectoryInfo dirInfo = new DirectoryInfo(dir);
                string fileName = string.Format(CultureInfo.InvariantCulture, Constants.WTMLFile, name);
                FileInfo file = dirInfo.GetFiles(fileName).FirstOrDefault();
                if (file != null)
                {
                    xmlDoc = new XmlDocument();
                    xmlDoc.Load(file.FullName);
                }
            }
            catch (DirectoryNotFoundException ex)
            {
                ErrorHandler.LogException(ex);
                throw new FaultException(ex.Message);
            }
            catch (SecurityException ex)
            {
                ErrorHandler.LogException(ex);
                throw new FaultException(ex.Message);
            }
            catch (UnauthorizedAccessException ex)
            {
                ErrorHandler.LogException(ex);
                throw new FaultException(ex.Message);
            }
            catch (ArgumentException ex)
            {
                ErrorHandler.LogException(ex);
                throw new FaultException(ex.Message);
            }
            catch (IOException ex)
            {
                ErrorHandler.LogException(ex);
                throw new FaultException(ex.Message);
            }

            return xmlDoc as IXPathNavigable;
        }

        /// <summary>
        /// Gets WTML details.
        /// </summary>
        /// <param name="filePath">File Path.</param>
        /// <returns>WTML collection from the xml.</returns>
        private static WTMLCollection GetWtmlDetails(string filePath)
        {
            WTMLCollection wtmlDetails = null;
            XmlDocument xmlDoc = new XmlDocument();
            try
            {
                xmlDoc.Load(filePath);
                XmlNode imageSet = xmlDoc.SelectSingleNode(Constants.ImageSetPath);
                if (imageSet != null)
                {
                    wtmlDetails = new WTMLCollection();
                    if (imageSet.Attributes[Constants.WTMLElevationModelAttribute] != null)
                    {
                        bool elevationModel;
                        if (bool.TryParse(imageSet.Attributes[Constants.WTMLElevationModelAttribute].Value, out elevationModel))
                        {
                            wtmlDetails.IsElevationModel = elevationModel;
                        }
                    }
                    if (imageSet.Attributes[Constants.WTMLName] != null)
                    {
                        wtmlDetails.Name = imageSet.Attributes[Constants.WTMLName].Value;
                    }
                    if (imageSet.Attributes[Constants.WTMLTileLevel] != null)
                    {
                        wtmlDetails.Levels = imageSet.Attributes[Constants.WTMLTileLevel].Value;
                    }
                    if (imageSet.Attributes[Constants.WTMLProjection] != null)
                    {
                        wtmlDetails.ProjectType = imageSet.Attributes[Constants.WTMLProjection].Value;
                    }
                    if (imageSet.Attributes[Constants.DEMUrl] != null)
                    {
                        wtmlDetails.IsDemEnabled = true;
                    }
                    if (imageSet.SelectSingleNode(Constants.Credits) != null)
                    {
                        wtmlDetails.Credit = imageSet.SelectSingleNode(Constants.Credits).InnerText;
                    }
                    if (imageSet.SelectSingleNode(Constants.CreditURL) != null)
                    {
                        wtmlDetails.CreditPath = imageSet.SelectSingleNode(Constants.CreditURL).InnerText;
                    }
                }
            }
            catch (XmlException ex)
            {
                ErrorHandler.LogException(ex);
                throw new FaultException(ex.Message);
            }
            catch (IOException ex)
            {
                ErrorHandler.LogException(ex);
                throw new FaultException(ex.Message);
            }
            catch (SecurityException ex)
            {
                ErrorHandler.LogException(ex);
                throw new FaultException(ex.Message);
            }
            return wtmlDetails;
        }
    }
}
