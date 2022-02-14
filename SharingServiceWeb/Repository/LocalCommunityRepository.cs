//-----------------------------------------------------------------------
// <copyright file="LocalCommunityRepository.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation 2011. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security;
using System.Security.Permissions;
using System.ServiceModel;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using Microsoft.Research.Wwt.Sdk.Core;
using Microsoft.Research.Wwt.SharingService.Web.Properties;

namespace Microsoft.Research.Wwt.SharingService.Web
{
    /// <summary>
    /// Handles all the requests made for retrieving details about communities which are stored locally.
    /// </summary>
    public class LocalCommunityRepository : ICommunityRepository
    {
        #region Private Properties

        /// <summary>
        /// Cache dependency for the community folder. This will make sure that the cached payload XMLs are up to date and any changes to the 
        /// community folder will delete the payload XML.
        /// </summary>
        private static CommunityFolderCacheDependency communityFolderCacheDependency;

        /// <summary>
        /// Location of the Community folder
        /// </summary>
        private string communityLocation;

        /// <summary>
        /// Days from today on which if files are modified, then file to be added in Latest folder.
        /// </summary>
        private int communityServiceLatestFileDays;

        #endregion

        /// <summary>
        /// Initializes a new instance of the LocalCommunityRepository class.
        /// </summary>
        public LocalCommunityRepository()
        {
            // Instantiate the community folder dependency cache for the community folder location.
            if (communityFolderCacheDependency == null && Directory.Exists(this.CommunityLocation))
            {
                communityFolderCacheDependency = new CommunityFolderCacheDependency(this.CommunityLocation);
            }
        }

        /// <summary>
        /// Gets Community location
        /// </summary>
        /// <returns>Community folder location</returns>
        public string CommunityLocation
        {
            get
            {
                if (string.IsNullOrWhiteSpace(communityLocation))
                {
                    communityLocation = ConfigurationManager.AppSettings[Constants.CommunityServiceInputDirectory];
                }

                return communityLocation;
            }
        }

        /// <summary>
        /// Gets the days from today on which if files are modified, then file to be added in Latest folder.
        /// </summary>
        /// <returns>Days from today on which if files are modified, then file to be added in Latest folder.</returns>
        public int CommunityServiceLatestFileDays
        {
            get
            {
                if (communityServiceLatestFileDays == 0)
                {
                    if (!Int32.TryParse(ConfigurationManager.AppSettings[Constants.CommunityServiceLatestFileDays], out communityServiceLatestFileDays))
                    {
                        // Default value is 30.
                        communityServiceLatestFileDays = 30;
                    }
                }

                return communityServiceLatestFileDays;
            }
        }

        /// <summary>
        /// Gets all community details for the service. 
        /// </summary>
        /// <returns>Details for all communities.</returns>
        public Collection<Community> GetAllCommunites()
        {
            Collection<Community> communityDetails = new Collection<Community>();
            try
            {
                if (!string.IsNullOrWhiteSpace(this.CommunityLocation))
                {
                    DirectoryInfo directory = new DirectoryInfo(this.CommunityLocation);

                    if (directory.Exists)
                    {
                        // Get all the directories excluding hidden, system and reparse point directories.
                        List<DirectoryInfo> directories = directory.GetDirectories().Where(
                                e => !e.Attributes.HasFlag(FileAttributes.Hidden | FileAttributes.System | FileAttributes.ReparsePoint)).ToList();
                        foreach (DirectoryInfo dirInfo in directories)
                        {
                            string description = string.Empty;

                            // Get thumbnail relative file path, if any image file with name Thumbnail exists.
                            string thumbnailFile = dirInfo.GetThumbnailFilePath(this.CommunityLocation);

                            // If any description file is there in the community folder, read the contents also.
                            FileInfo[] descriptionFile = dirInfo.GetFiles(Constants.DescriptionFileName);
                            if (descriptionFile.Length > 0)
                            {
                                using (StreamReader streamReader = new StreamReader(descriptionFile[0].OpenRead()))
                                {
                                    description = streamReader.ReadToEnd();
                                }
                            }

                            communityDetails.Add(new Community
                            {
                                Id = dirInfo.Name,
                                Description = description,
                                Thumbnail = thumbnailFile
                            });
                        }
                    }
                }
                else
                {
                    // Throw a fault exception which will be handled by the community service sample aspx page.
                    throw new FaultException(Resources.CommunityPathNotSet);
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

            return communityDetails;
        }

        /// <summary>
        /// Gets the signup details for the given community.
        /// </summary>
        /// <param name="communityId">Community for which signup xml to be fetched</param>
        /// <returns>Signup object with details.</returns>
        public SignUp GetSignUpDetail(string communityId)
        {
            SignUp signup = null;

            try
            {
                string communityDirectory = Path.Combine(this.CommunityLocation, communityId);
                DirectoryInfo communityDirectoryInfo = new DirectoryInfo(communityDirectory);

                // Get thumbnail relative file path, if any image file with name Thumbnail exists.
                string thumbnailFile = communityDirectoryInfo.GetThumbnailFilePath(this.CommunityLocation);

                signup = new SignUp(communityDirectoryInfo.Name, thumbnailFile);
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

            return signup;
        }

        /// <summary>
        /// Gets the Payload WTML file details for the given community.
        /// </summary>
        /// <param name="communityId">Community for which payload xml to be fetched</param>
        /// <returns>Payload object with details</returns>
        [PermissionSetAttribute(SecurityAction.LinkDemand, Name = "FullTrust")] 
        public Folder GetPayloadDetails(string communityId)
        {
            Folder rootFolder = null;

            try
            {
                string communityFolder = Path.Combine(this.CommunityLocation, communityId);
                string payloadFilePath = string.Format(CultureInfo.CurrentCulture, "{0}\\{1}_payload.xml", communityFolder, communityId);
                DirectoryInfo dirInfo = new DirectoryInfo(communityFolder);

                // Delete if any payload files are pending to be deleted, because cache dependency could not delete last time.
                for (int i = communityFolderCacheDependency.FilesToBeDeleted.Count - 1; i >= 0; i--)
                {
                    try
                    {
                        string filePath = communityFolderCacheDependency.FilesToBeDeleted[i];
                        File.Delete(filePath);
                        communityFolderCacheDependency.FilesToBeDeleted.Remove(filePath);
                    }
                    catch (IOException ex)
                    {
                        ErrorHandler.LogException(ex);
                    }
                }

                if (File.Exists(payloadFilePath))
                {
                    try
                    {
                        using (Stream stream = File.OpenRead(payloadFilePath))
                        {
                            // IsLocal property of the Place object needs to be serialized so that while
                            // De-serialized and processed for URL rewriting, IsLocal property can be used.
                            XmlAttributeOverrides overrides = new XmlAttributeOverrides();
                            XmlAttributes attributes = new XmlAttributes();
                            attributes.XmlIgnore = false;
                            overrides.Add(typeof(Place), "IsLocal", attributes);

                            XmlSerializer serializer = new XmlSerializer(typeof(Folder), overrides);
                            XmlReader reader = new XmlTextReader(stream);
                            rootFolder = (Folder)serializer.Deserialize(reader);
                        }
                    }
                    catch (IOException ex)
                    {
                        ErrorHandler.LogException(ex);
                    }
                }
                else
                {
                    // Get thumbnail relative file path, if any image file with name Thumbnail exists.
                    string thumbnailFile = dirInfo.GetThumbnailFilePath(this.CommunityLocation);

                    // Creating the root community folder object.
                    rootFolder = new Folder(dirInfo.Name, null);
                    rootFolder.Thumbnail = thumbnailFile;

                    // By default, two folder with name "All Tours" and "Latest" will be added to all communities.
                    // "All Tours" will be having all the tours which are present in the community anywhere in the folder structure.
                    // "Latest" will be having all the tours, WTMLs and Places/Links which are modified in last n days and present 
                    // in the community anywhere in the folder structure.
                    Folder allToursFolder = new Folder(Constants.AllToursFolder, rootFolder);
                    Folder latestFolder = new Folder(Constants.LatestFolder, rootFolder);

                    // Loop through all the folder contents recursively.
                    ProcessFolderItems(dirInfo, rootFolder);

                    // This will make sure that there are not events fired while creating/overwriting the payload XML file.
                    communityFolderCacheDependency.CommunityFolderWatcher.EnableRaisingEvents = false;

                    SavePayloadToCache(payloadFilePath, rootFolder);
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
            finally
            {
                // If the community folder watcher is not enabled for events, enable them.
                if (!communityFolderCacheDependency.CommunityFolderWatcher.EnableRaisingEvents)
                {
                    communityFolderCacheDependency.CommunityFolderWatcher.EnableRaisingEvents = true;
                }
            }

            return rootFolder;
        }

        /// <summary>
        /// Gets the file identified by the given id from the given communities folder.
        /// </summary>
        /// <param name="id">Id of the file</param>
        /// <returns>File stream.</returns>
        public Stream GetFile(string id)
        {
            MemoryStream memoryStream = null;
            string filename = Path.Combine(this.CommunityLocation, id);

            try
            {
                if (File.Exists(filename))
                {
                    byte[] data = null;
                    using (Stream s = File.OpenRead(filename))
                    {
                        int length = (int)s.Length;
                        data = new byte[length];
                        s.Read(data, 0, length);
                        memoryStream = new MemoryStream(data);
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
        /// Gets tile image.
        /// </summary>
        /// <param name="id">ID for specified Pyramid.</param>
        /// <param name="level">Level for the image.</param>
        /// <param name="x">X axis of the image.</param>
        /// <param name="y">Y axis of the image.</param>
        /// <returns>Stream for the tile image for the specified level, x and y axis.</returns>
        public Stream GetTile(string id, int level, int x, int y)
        {
            Stream outputStream = null;
            try
            {
                string pyramidFolderPath = Path.Combine(this.CommunityLocation, id, Constants.PyramidFolder);
                string platesFolderPath = Path.Combine(this.CommunityLocation, id, Constants.PlatesFolder);
                DirectoryInfo pyramidFolder = new DirectoryInfo(pyramidFolderPath);
                DirectoryInfo platesFolder = new DirectoryInfo(platesFolderPath);

                if (pyramidFolder.Exists)
                {
                    string pyramidPath = Path.Combine(this.CommunityLocation, id, Constants.TileImagePath);

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
                string pyramidFolderPath = Path.Combine(this.CommunityLocation, id, Constants.PyramidFolder);
                string demPlatesFolderPath = Path.Combine(this.CommunityLocation, id, Constants.DEMPlatesFolder);
                DirectoryInfo pyramidFolder = new DirectoryInfo(pyramidFolderPath);
                DirectoryInfo platesFolder = new DirectoryInfo(demPlatesFolderPath);

                if (pyramidFolder.Exists)
                {
                    string pyramidPath = Path.Combine(this.CommunityLocation, id, Constants.DemTilePath);

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
        /// Adds the place object to the folder with the given name, URL, thumbnail. If the link file is latest, then 
        /// Place will be added the "Latest" folder also.
        /// </summary>
        /// <param name="name">Name of the link</param>
        /// <param name="url">Url for the link</param>
        /// <param name="thumbnail">Thumbnail image of the link</param>
        /// <param name="latestFile">Is the link file latest?</param>
        /// <param name="folder">Folder to which the link belongs to</param>
        /// <param name="modifiedDate">Links file modified date</param>
        private static void AddPlaceElementToFolder(string name, string url, string thumbnail, bool latestFile, Folder folder, DateTime modifiedDate)
        {
            Place place = new Place(name, url, thumbnail);
            place.Parent = folder;
            place.ModifiedDate = modifiedDate;

            // Add the Place to the "Latest" folder which is the second children of RootFolder only if the file is modified with
            // the specified date in the configuration file.
            if (latestFile)
            {
                Place existingPlace = folder.RootFolder.Children[1].Links.FirstOrDefault(e => (e.Name == place.Name && e.Thumbnail == place.Thumbnail));

                // Make sure the same Link is not added already which could be there in some other folder. Links will be 
                // identified same based on their name and thumbnail.
                // If the Link already exists, replace the current Link if it is the latest one.
                if (existingPlace == null)
                {
                    folder.RootFolder.Children[1].Links.Add(place);
                }
                else if (place.ModifiedDate > existingPlace.ModifiedDate)
                {
                    folder.RootFolder.Children[1].Links.Remove(existingPlace);
                    folder.RootFolder.Children[1].Links.Add(place);
                }
            }
        }

        /// <summary>
        /// Parses tour file and gets the Tour portion of the XML text from it.
        /// </summary>
        /// <param name="file">Tour file object</param>
        /// <returns>Tour xml section</returns>
        private static XmlDocument ParseTourFile(FileInfo file)
        {
            XmlDocument tourFile = new XmlDocument();

            try
            {
                ////  Format of the tour file (.wtt) header. 
                ////  Initial section of the tour file will contain the below XML. After that, it will have tour xml content followed by
                ////  binary stream having the tour file contents. Header XML will have size of each file and their offset address in the stream.

                ////  <?xml version='1.0' encoding='UTF-8'?>
                ////  <FileCabinet HeaderSize="0x000003c2">
                ////      <Files>
                ////          <File Name="TC tOUR.wwtxml" Size="9579" Offset="0" />
                ////          <File Name="4d70857b-722d-4739-b9f7-e2fa72df2fc0\86858e99-e41a-428e-bf07-5fb7f488488e.thumb.png" Size="1587" Offset="9579" />
                ////          <File Name="4d70857b-722d-4739-b9f7-e2fa72df2fc0\e743a6db-1f50-4812-95e1-c7f73fa17010.thumb.png" Size="1375" Offset="11166" />
                ////          <File Name="4d70857b-722d-4739-b9f7-e2fa72df2fc0\ed6474d0-7244-42ca-9f7c-4031c085a03f.thumb.png" Size="1617" Offset="12541" />
                ////          <File Name="4d70857b-722d-4739-b9f7-e2fa72df2fc0\9747fe35-d9eb-41b0-86b7-82856d2a1488.thumb.png" Size="8778" Offset="14158" />
                ////          <File Name="4d70857b-722d-4739-b9f7-e2fa72df2fc0\aa2bca68-b1f9-46a4-92f2-11d572f6a1ab.thumb.png" Size="3166" Offset="22936" />
                ////          <File Name="4d70857b-722d-4739-b9f7-e2fa72df2fc0\4902a150-35fc-45e6-bcee-1aae81a5df7b.png" Size="777835" Offset="26102" />
                ////      </Files>
                ////  </FileCabinet>

                using (FileStream fileStream = file.OpenRead())
                {
                    // Reading the HeaderSize attribute's value from the above header xml.
                    byte[] buffer = new byte[256];
                    fileStream.Read(buffer, 0, 255);
                    string data = Encoding.UTF8.GetString(buffer);
                    int start = data.IndexOf("0x", StringComparison.Ordinal);

                    // If file is corrupted or invalid format, ignore this tour file.
                    if (start != -1)
                    {
                        // Get the header xml size.
                        int headerSize = Convert.ToInt32(data.Substring(start, 10), 16);
                        fileStream.Seek(0, SeekOrigin.Begin);

                        // Read the header XML from the tour file (.wtt). Using the header size read the header xml stream and load XML dom.
                        buffer = new byte[headerSize];
                        fileStream.Read(buffer, 0, headerSize);
                        data = Encoding.UTF8.GetString(buffer);
                        XmlDocument doc = new XmlDocument();
                        doc.LoadXml(data);

                        // Get the FileCabinet element.
                        XmlNode cab = doc["FileCabinet"];

                        if (cab != null)
                        {
                            // Get all the File elements.
                            XmlNode files = cab["Files"];

                            if (files != null && files.ChildNodes.Count > 0)
                            {
                                // First File element is for tour xml, get the size of the tour xml.
                                int fileSize = Convert.ToInt32(files.ChildNodes[0].Attributes["Size"].Value, CultureInfo.CurrentCulture);

                                using (MemoryStream stream = new MemoryStream())
                                {
                                    // Read the tour xml stream and load the Tour XML in XML dom.
                                    buffer = new byte[fileSize];
                                    fileStream.Seek(headerSize, SeekOrigin.Begin);
                                    if (fileStream.Read(buffer, 0, fileSize) == fileSize)
                                    {
                                        stream.Write(buffer, 0, fileSize);
                                        stream.Seek(0, SeekOrigin.Begin);
                                        tourFile.Load(stream);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (XmlException ex)
            {
                // Consume any Xml Exception.
                ErrorHandler.LogException(ex);
            }
            catch (IOException ex)
            {
                // Consume any IO Exception.
                ErrorHandler.LogException(ex);
            }

            return tourFile;
        }

        /// <summary>
        /// Saves the payload Folder in to Cache.
        /// </summary>
        /// <param name="payloadFilePath">Payload file path for the community</param>
        /// <param name="folder">Root folder object which needs to cached</param>
        /// <returns>True if saved to cache, false otherwise</returns>
        private static bool SavePayloadToCache(string payloadFilePath, Folder folder)
        {
            bool savedToCache = false;

            try
            {
                if (folder != null)
                {
                    using (FileStream fileStream = new FileStream(payloadFilePath, FileMode.Create, FileAccess.Write))
                    {
                        // IsLocal property of the Place object needs to be serialized so that while
                        // De-serialized and processed for URL rewriting, IsLocal property can be used.
                        XmlAttributeOverrides overrides = new XmlAttributeOverrides();
                        XmlAttributes attributes = new XmlAttributes();
                        attributes.XmlIgnore = false;
                        overrides.Add(typeof(Place), "IsLocal", attributes);

                        XmlTextWriter xmlTextWriter = new XmlTextWriter(fileStream, ASCIIEncoding.Unicode);
                        XmlSerializer xmlSerializer = new XmlSerializer(typeof(Folder), overrides);
                        xmlSerializer.Serialize(xmlTextWriter, folder);
                        savedToCache = true;
                    }
                }
            }
            catch (IOException ex)
            {
                ErrorHandler.LogException(ex);
            }
            catch (UnauthorizedAccessException ex)
            {
                ErrorHandler.LogException(ex);
            }

            return savedToCache;
        }

        /// <summary>
        /// Loops through all the items and adds them in to the payload xml. And also recursively loops through all the folders and do the same.
        /// </summary>
        /// <param name="currentDirectory">Current folder getting processed</param>
        /// <param name="folder">Current folder object</param>
        private void ProcessFolderItems(DirectoryInfo currentDirectory, Folder folder)
        {
            bool containOnlySingleWTMLFile = true;

            // Get all the directories excluding hidden, system and reparse point directories.
            List<DirectoryInfo> directories = currentDirectory.GetDirectories().Where(
                    e => !e.Attributes.HasFlag(FileAttributes.Hidden) & !e.Attributes.HasFlag(FileAttributes.System) & !e.Attributes.HasFlag(FileAttributes.ReparsePoint)).ToList();
            foreach (DirectoryInfo childDirectory in directories)
            {
                // 1. If any pyramid/plate/DEM plate folders are present, ignore them since they will be used only by WTML.
                // 2. In the root community folder, if any folder with name "All Tours" or "Latest", ignore them also.
                if ((childDirectory.Name.Equals(Constants.PyramidFolder, StringComparison.OrdinalIgnoreCase) ||
                        childDirectory.Name.Equals(Constants.PlatesFolder, StringComparison.OrdinalIgnoreCase) ||
                        childDirectory.Name.Equals(Constants.DEMPlatesFolder, StringComparison.OrdinalIgnoreCase)) ||
                        (this.CommunityLocation.Equals(currentDirectory.Parent.FullName, StringComparison.OrdinalIgnoreCase) &&
                        (childDirectory.Name.Equals(Constants.AllToursFolder, StringComparison.OrdinalIgnoreCase) ||
                        childDirectory.Name.Equals(Constants.LatestFolder, StringComparison.OrdinalIgnoreCase))))
                {
                    continue;
                }

                // If any folders are there (other than pyramid), don't consider as a folder which contains only single WTML file.
                containOnlySingleWTMLFile = false;

                Folder child = new Folder(childDirectory.Name, folder);

                // Process the folders/files present inside the child folder. This will be recursive.
                ProcessFolderItems(childDirectory, child);
            }

            ProcessFiles(currentDirectory, folder, containOnlySingleWTMLFile);
        }

        /// <summary>
        /// Process all the files in the current folder and adds them to the appropriate folder object.
        /// </summary>
        /// <param name="currentDirectory">Current folder being processed</param>
        /// <param name="folder">Folder object representing current folder</param>
        /// <param name="containOnlySingleWTMLFile">Current directory contains only a single WTML file</param>
        private void ProcessFiles(DirectoryInfo currentDirectory, Folder folder, bool containOnlySingleWTMLFile)
        {
            int wtmlFileCount = 0;

            // Ignore system and hidden files.
            List<FileInfo> files = currentDirectory.GetFiles().Where(e => !e.Attributes.HasFlag(FileAttributes.Hidden) & !e.Attributes.HasFlag(FileAttributes.System)).ToList();
            foreach (FileInfo file in files)
            {
                // Ignore thumbnail files, description files and plate files.
                if ((Path.GetFileNameWithoutExtension(file.Name).Equals("Thumbnail", StringComparison.OrdinalIgnoreCase) && file.IsImageFile()) ||
                        file.Extension.Equals(Constants.PlateFileExtension, StringComparison.OrdinalIgnoreCase) ||
                        file.Name.Equals(Constants.DescriptionFileName, StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                // For local WTML files (WTML not served by SharingService), thumbnail file will have the same name as WTML file. Need
                // to find such image files (images and WTML files with same name) and ignore them also.
                if (file.IsImageFile())
                {
                    string searchPattern = string.Format(CultureInfo.InvariantCulture, Constants.WTMLFile, Path.GetFileNameWithoutExtension(file.Name));
                    FileInfo[] wtmlFile = currentDirectory.GetFiles(searchPattern);
                    if (wtmlFile.Length > 0)
                    {
                        continue;
                    }
                }

                // Get the file Id, i.e. relative path by removing the community folder location.
                string fileId = file.FullName.Replace(this.CommunityLocation, string.Empty).TrimStartSlashes();

                if (file.Extension.Equals(Constants.WTMLExtension, StringComparison.OrdinalIgnoreCase))
                {
                    // Processing WTML files (.wtml) here.

                    // Get number of WTML files in current folder. If there is only one WTML file along with its Pyramid, plate and
                    // thumbnail files in a folder, then the folder containing the WTML file will not be shown in WWT.
                    wtmlFileCount++;

                    // For WTML files, replace the URLs if they are having local URLs, leave them as it is if they are http or https.
                    XmlDocument wtmlFileDoc = new XmlDocument();
                    wtmlFileDoc.Load(file.FullName);
                    ReplaceLocalUrlWithRelative(wtmlFileDoc, currentDirectory.FullName);
                    string innerText = string.Empty;
                    bool latestFile = file.IsLatestFile(this.CommunityServiceLatestFileDays);

                    // In case if there is only one item inside a WTML file, then the root node/document element which
                    // displays a folder for the WTML will be ignored. In case if there are more then one items in a WTML file,
                    // then the folder will be displayed which will be grouping the items in WWT.
                    if (wtmlFileDoc.DocumentElement.ChildNodes.Count == 1)
                    {
                        if (latestFile)
                        {
                            // Adding the ModifiedDate attribute to the FirstChild element which will be added as children of Latest folder.
                            // This is needed for sorting the WTML collection in Latest folder.
                            XmlAttribute modifiedDateAttrib = wtmlFileDoc.CreateAttribute(Constants.ModifiedDateAttribute);
                            modifiedDateAttrib.Value = XmlConvert.ToString(file.GetModifiedDate(), XmlDateTimeSerializationMode.Utc);
                            wtmlFileDoc.DocumentElement.FirstChild.Attributes.Append(modifiedDateAttrib);
                        }

                        innerText = wtmlFileDoc.DocumentElement.InnerXml;
                    }
                    else
                    {
                        if (latestFile)
                        {
                            // Adding the ModifiedDate attribute to the Document element which will be added as children of Latest folder.
                            // This is needed for sorting the WTML collection in Latest folder.
                            XmlAttribute modifiedDateAttrib = wtmlFileDoc.CreateAttribute(Constants.ModifiedDateAttribute);
                            modifiedDateAttrib.Value = XmlConvert.ToString(file.GetModifiedDate(), XmlDateTimeSerializationMode.Utc);
                            wtmlFileDoc.DocumentElement.Attributes.Append(modifiedDateAttrib);
                        }

                        innerText = wtmlFileDoc.DocumentElement.OuterXml;
                    }

                    folder.InnerText += innerText;
                    if (latestFile)
                    {
                        // Add the WTML inner text to the "Latest" folder which is the second children of RootFolder only if the file is modified with
                        // the specified date in the configuration file.
                        folder.RootFolder.Children[1].InnerText += innerText;
                    }

                    if (wtmlFileCount > 1)
                    {
                        containOnlySingleWTMLFile = false;
                    }
                }
                else if (file.Extension.Equals(Constants.TourExtension, StringComparison.OrdinalIgnoreCase))
                {
                    // Processing Tour files (.wtt) here.

                    // If any files other than WTML files, don't consider as a folder which contains only WTML file.
                    containOnlySingleWTMLFile = false;

                    // For tour files, get the properties needed for constructing the Tour tag from WTT file itself.
                    XmlDocument tourFile = ParseTourFile(file);

                    if (tourFile != null)
                    {
                        // Creating the tour object by extracting the required properties from Tour XML file.
                        Tour tour = new Tour(
                                tourFile.GetAttributeValue("Tour", "Title"),
                                tourFile.GetAttributeValue("Tour", "ID"),
                                fileId,
                                tourFile.GetAttributeValue("Tour", "ThumbnailUrl"));

                        tour.Description = tourFile.GetAttributeValue("Tour", "Descirption");
                        tour.Author = tourFile.GetAttributeValue("Tour", "Author");
                        tour.OrganizationUrl = tourFile.GetAttributeValue("Tour", "OrganizationUrl");
                        tour.OrganizationName = tourFile.GetAttributeValue("Tour", "OrganizationName");
                        tour.AuthorImageUrl = tourFile.GetAttributeValue("Tour", "AuthorImageUrl");
                        tour.ModifiedDate = file.GetModifiedDate();

                        tour.Parent = folder;

                        // Add the tour to the "Latest" folder which is the second children of RootFolder only if the file is modified with
                        // the specified date in the configuration file.
                        if (file.IsLatestFile(this.CommunityServiceLatestFileDays))
                        {
                            Tour existingTour = folder.RootFolder.Children[1].Tours.FirstOrDefault(e => e.ID == tour.ID);

                            // Make sure the same Tour is not added already which could be there in some other folder.
                            // If the tour already exists, replace the current tour if it is the latest one.
                            if (existingTour == null)
                            {
                                folder.RootFolder.Children[1].Tours.Add(tour);
                            }
                            else if (tour.ModifiedDate > existingTour.ModifiedDate)
                            {
                                folder.RootFolder.Children[1].Tours.Remove(existingTour);
                                folder.RootFolder.Children[1].Tours.Add(tour);
                            }
                        }
                    }
                }
                else if (file.Extension.Equals(Constants.TextExtension, StringComparison.OrdinalIgnoreCase) &&
                        file.Name.StartsWith("Link", StringComparison.OrdinalIgnoreCase))
                {
                    // Processing external link files (Link*.txt) here.

                    // If any files other than WTML files, don't consider as a folder which contains only WTML file.
                    containOnlySingleWTMLFile = false;

                    // For link files (Link*.txt), read the links from the file content.
                    ParseLinkFile(folder, file);
                }
                else
                {
                    // Processing all other local files here.

                    // If any files other than WTML files, don't consider as a folder which contains only WTML file.
                    containOnlySingleWTMLFile = false;

                    // Add links for all other files.
                    Place place = new Place(file.Name, fileId, string.Empty);
                    place.IsLocal = true;
                    place.Parent = folder;
                    place.ModifiedDate = file.GetModifiedDate();

                    // Add the Place to the "Latest" folder which is the second children of RootFolder only if the file is modified with
                    // the specified date in the configuration file.
                    if (file.IsLatestFile(this.CommunityServiceLatestFileDays))
                    {
                        Place existingPlace = folder.RootFolder.Children[1].Links.FirstOrDefault(e => (e.Name == place.Name && e.Thumbnail == place.Thumbnail));

                        // Make sure the same Link is not added already which could be there in some other folder. Links will be 
                        // identified same based on their name and thumbnail.
                        // If the Link already exists, replace the current Link if it is the latest one.
                        if (existingPlace == null)
                        {
                            folder.RootFolder.Children[1].Links.Add(place);
                        }
                        else if (place.ModifiedDate > existingPlace.ModifiedDate)
                        {
                            folder.RootFolder.Children[1].Links.Remove(existingPlace);
                            folder.RootFolder.Children[1].Links.Add(place);
                        }
                    }
                }
            }

            if (containOnlySingleWTMLFile && currentDirectory.GetFiles().Length > 0)
            {
                // If only WTML files are there in the current directory, remove folder which contains the WTML file and
                // add the WTML file to its parent. If current folder is community folder (parent will be null), don't do this.
                if (folder.Parent != null)
                {
                    folder.Parent.InnerText += folder.InnerText;
                    folder.Parent.Children.Remove(folder);
                }
            }
        }

        /// <summary>
        /// Parses link file and adds Place objects to folder object for the links present inside the text file.
        /// </summary>
        /// <param name="folder">Folder to which the link belongs to</param>
        /// <param name="file">Link file object</param>
        private void ParseLinkFile(Folder folder, FileInfo file)
        {
            try
            {
                FileStream fileStream = null;

                try
                {
                    fileStream = file.OpenRead();
                    using (StreamReader streamReader = new StreamReader(fileStream))
                    {
                        string lineText = string.Empty;
                        while (null != (lineText = streamReader.ReadLine()))
                        {
                            // Link file is tab delimited. Its format is Name\tUrl\tThumbnail. Thumbnail is optional.
                            string[] urlParts = lineText.Split(new char[] { '\t' }, StringSplitOptions.RemoveEmptyEntries);

                            if (urlParts.Length == 2)
                            {
                                AddPlaceElementToFolder(
                                        urlParts[0],
                                        urlParts[1],
                                        string.Empty,
                                        file.IsLatestFile(this.CommunityServiceLatestFileDays),
                                        folder,
                                        file.GetModifiedDate());
                            }
                            else if (urlParts.Length == 3)
                            {
                                AddPlaceElementToFolder(
                                        urlParts[0],
                                        urlParts[1],
                                        urlParts[2],
                                        file.IsLatestFile(this.CommunityServiceLatestFileDays),
                                        folder,
                                        file.GetModifiedDate());
                            }
                        }
                    }
                }
                finally
                {
                    if (fileStream != null)
                    {
                        fileStream.Dispose();
                    }
                }
            }
            catch (XmlException ex)
            {
                // Consume any Xml Exception.
                ErrorHandler.LogException(ex);
            }
            catch (IOException ex)
            {
                // Consume any IO Exception.
                ErrorHandler.LogException(ex);
            }
        }

        /// <summary>
        /// Replace all local URL reference in the WTML file.
        /// </summary>
        /// <param name="wtmlFile">Dom object of the WTML file.</param>
        private void ReplaceLocalUrlWithRelative(XmlDocument wtmlFile, string folderName)
        {
            // Getting nodes which are having URL attribute.
            XmlNodeList nodesWithUrl = wtmlFile.SelectNodes(Constants.NodesWithUrlAttributeXPath);
            foreach (XmlNode node in nodesWithUrl)
            {
                // Getting URL attribute value of each node.
                string urlValue = node.Attributes[Constants.UrlAttribute].Value;
                string folderRelativePath = folderName.Replace(this.CommunityLocation, string.Empty).TrimStartSlashes();
                if (!string.IsNullOrEmpty(urlValue) && !urlValue.IsValidUrl())
                {
                    // Only for ImageSet nodes, we need to set the URL as Tile serving URL with format as http://<ServiceURL>>/Pyramid/{1}/{2}/L{1}X{2}Y{3}.PNG
                    // This is the format in which WWT expects the WMTL file to get the tile images.
                    if (node.Name.Equals(Constants.ImageSetNodeName, StringComparison.OrdinalIgnoreCase))
                    {
                        if (node.Attributes[Constants.UrlAttribute] != null)
                        {
                            // Set the relative folder name as tile image path.
                            node.Attributes[Constants.UrlAttribute].Value = folderRelativePath;
                        }

                        // TODO: Need to revisit based on how samples put DEM Url.
                        if (node.Attributes[Constants.DEMUrl] != null &&
                                node.Attributes[Constants.DEMUrl].Value.StartsWith(Constants.DefaultDEMUrl, StringComparison.OrdinalIgnoreCase))
                        {
                            // Set the relative folder name as DEM tile image path.
                            node.Attributes[Constants.DEMUrl].Value = folderRelativePath;
                        }

                        // Get the thumbnail node of the image set. If no thumbnail node exists, add a thumbnail node with default image.
                        XmlNode thumbnailNode = node.SelectSingleNode(Constants.ThumbnailUrl);
                        if (thumbnailNode == null)
                        {
                            // Get thumbnail relative file path, if any image file with name Thumbnail exists.
                            string thumbnailFile = new DirectoryInfo(folderName).GetThumbnailFilePath(this.communityLocation);
                            thumbnailNode = wtmlFile.CreateElement(Constants.ThumbnailUrl);
                            thumbnailNode.InnerText = thumbnailFile;
                            node.AppendChild(thumbnailNode);
                        }
                        else if (!string.IsNullOrWhiteSpace(thumbnailNode.InnerText) && !thumbnailNode.InnerText.IsValidUrl())
                        {
                            string thumbnailFile = thumbnailNode.InnerText;

                            // Remove quotes in the path, if any.
                            thumbnailFile = thumbnailFile.Trim(new char[] { '\'', '"' });

                            // Combining relative path of the folder containing thumbnail and the thumbnail file name.
                            thumbnailNode.InnerText = Path.Combine(
                                    folderRelativePath,
                                    Path.GetFileName(thumbnailFile));
                        }
                    }
                    else
                    {
                        node.Attributes[Constants.UrlAttribute].Value = urlValue.Replace(this.CommunityLocation, string.Empty).TrimStartSlashes();
                    }
                }
            }
        }
    }
}
