//-----------------------------------------------------------------------
// <copyright file="CommunityFolderCacheDependency.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation 2011. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security;
using System.Security.Permissions;
using System.Threading;
using System.Web.Caching;

namespace Microsoft.Research.Wwt.SharingService.Web
{
    /// <summary>
    /// Class representing the community folder cache dependency. If any changes to community folder happened,
    /// this class will make sure that the payload XML are deleted so that they can be regenerated again.
    /// </summary>
    internal class CommunityFolderCacheDependency : CacheDependency
    {
        /// <summary>
        /// Initializes a new instance of the CommunityFolderCacheDependency class.
        /// </summary>
        /// <param name="communityLocation">Community folder location to which the cache is dependent on</param>
        [PermissionSetAttribute(SecurityAction.LinkDemand, Name = "FullTrust")] 
        internal CommunityFolderCacheDependency(string communityLocation)
        {
            FilesToBeDeleted = new List<string>();
            this.CommunityLocation = communityLocation;
            this.CommunityFolderWatcher = new FileSystemWatcher(communityLocation);
            this.CommunityFolderWatcher.EnableRaisingEvents = true;
            this.CommunityFolderWatcher.IncludeSubdirectories = true;
            this.CommunityFolderWatcher.Changed += new FileSystemEventHandler(CommunityContentChanged);
            this.CommunityFolderWatcher.Deleted += new FileSystemEventHandler(CommunityContentChanged);
            this.CommunityFolderWatcher.Created += new FileSystemEventHandler(CommunityContentChanged);
            this.CommunityFolderWatcher.Renamed += new RenamedEventHandler(CommunityContentRenamed);

            // Whenever the service is started or a new cache dependency is started, make sure that the payload files are up to date.
            // This is needed to make sure that if files are copied to a community which is already having a cached payload file, then the
            // payload file needs to be deleted, in case the copy happened before the cache dependency has been created.
            CheckPayloadValidity();
        }

        /// <summary>
        /// Gets or sets FileSystemWatcher instance which will be looking at the changes to the community folder.
        /// </summary>
        internal FileSystemWatcher CommunityFolderWatcher { get; set; }

        /// <summary>
        /// Gets or sets the files which are pending to be deleted. In case of the payload files are not deleted because they are open, retry will 
        /// happen later through the entries in the list.
        /// </summary>
        internal List<string> FilesToBeDeleted { get; set; }

        /// <summary>
        /// Gets or sets the Community folder location.
        /// </summary>
        private string CommunityLocation { get; set; }

        /// <summary>
        /// Checks whether the community folder is updated, i.e. any new content is added or deleted from the community
        /// after payload xml was generated and cached. DateModified property of each folder in the community will be checked
        /// against the DateModifed time of the payload xml which is cached.
        /// </summary>
        /// <param name="payloadFileWriteTime">Cached payload file's write time</param>
        /// <param name="communityFolder">Full path of the community folder</param>
        /// <returns>True, if the folder is updated, false otherwise</returns>
        private static bool CommunityFolderUpdated(DateTime payloadFileWriteTime, string communityFolder)
        {
            bool communityFolderUpdated = false;

            // First check the community folder (root folder) is modified recently (after generation of payload xml).
            if (Directory.GetLastWriteTimeUtc(communityFolder) > payloadFileWriteTime)
            {
                communityFolderUpdated = true;
            }
            else
            {
                // Check all sub directories of the community folder (root folder), if they are modified recently 
                // (after generation of payload xml).
                var dirs = from childDirectory in Directory.GetDirectories(communityFolder, "*", SearchOption.AllDirectories)
                           let childDirectoryInfo = new DirectoryInfo(childDirectory)
                           where childDirectoryInfo.LastWriteTimeUtc > payloadFileWriteTime
                           select childDirectory;

                if (dirs.Count() > 0)
                {
                    communityFolderUpdated = true;
                }
                else
                {
                    // Check all the files in directories of the community folder (root folder), if they are modified recently 
                    // (after generation of payload xml). It is needed since "Latest" folder will get updated if files are modified.
                    var files = from file in Directory.GetFiles(communityFolder, "*", SearchOption.AllDirectories)
                                let fileInfo = new FileInfo(file)
                                where fileInfo.LastWriteTimeUtc > payloadFileWriteTime
                                select file;

                    if (files.Count() > 0)
                    {
                        communityFolderUpdated = true;
                    }
                }
            }

            return communityFolderUpdated;
        }

        /// <summary>
        /// Event raised when an folder or file is getting renamed.
        /// </summary>
        /// <param name="sender">FileSystemWatcher instance</param>
        /// <param name="e">Renamed event argument</param>
        private void CommunityContentRenamed(object sender, RenamedEventArgs e)
        {
            HandleCommunityFolderEvent(e.FullPath);
        }

        /// <summary>
        /// Event raised when an folder or file is getting created or deleted or modified.
        /// </summary>
        /// <param name="sender">FileSystemWatcher instance</param>
        /// <param name="e">File system event args</param>
        private void CommunityContentChanged(object sender, FileSystemEventArgs e)
        {
            HandleCommunityFolderEvent(e.FullPath);
        }

        /// <summary>
        /// Handles this renamed or changed events of the folder or file. Makes sure that the payload XML of the community is deleted
        /// if any changes to the file or folder is made.
        /// </summary>
        /// <param name="eventContentPath">Content (file or folder) path to which the event is triggered for</param>
        private void HandleCommunityFolderEvent(string eventContentPath)
        {
            try
            {
                this.CommunityFolderWatcher.EnableRaisingEvents = false;

                if (!eventContentPath.EndsWith("_payload.xml", StringComparison.OrdinalIgnoreCase))
                {
                    string communityFolderPath = eventContentPath.Replace(this.CommunityLocation, string.Empty).TrimStartSlashes();

                    int directorySeparatorIndex = communityFolderPath.IndexOf(Path.DirectorySeparatorChar);
                    directorySeparatorIndex = directorySeparatorIndex == -1 ? communityFolderPath.Length : directorySeparatorIndex;

                    communityFolderPath = Path.Combine(this.CommunityLocation, communityFolderPath.Substring(0, directorySeparatorIndex));

                    DirectoryInfo communityFolder = new DirectoryInfo(communityFolderPath);
                    if (communityFolder.Exists)
                    {
                        FileInfo[] payloadFile = communityFolder.GetFiles("*_payload.xml");

                        foreach (FileInfo file in payloadFile)
                        {
                            // Add to the to be deleted list. In case if the file cannot be deleted at the moment, later those files can be deleted.
                            if (!FilesToBeDeleted.Contains(file.FullName))
                            {
                                FilesToBeDeleted.Add(file.FullName);
                            }

                            file.Delete();
                            FilesToBeDeleted.Remove(file.FullName);
                        }
                    }

                    // Delete if any payload files are pending to be deleted, because cache dependency could not delete last time.
                    for (int i = FilesToBeDeleted.Count - 1; i >= 0; i--)
                    {
                        string filePath = FilesToBeDeleted[i];
                        File.Delete(filePath);
                        FilesToBeDeleted.Remove(filePath);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.LogException(ex);
            }
            finally
            {
                this.CommunityFolderWatcher.EnableRaisingEvents = true;
            }
        }

        /// <summary>
        /// Checks whether the payload XML of all the communities are valid or not. If any file or folder of a community is changed
        /// after its payload file is created, then the payload file will be considered as invalid and deleted.
        /// </summary>
        private void CheckPayloadValidity()
        {
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
                            string payloadFilePath = string.Format(CultureInfo.CurrentCulture, "{0}\\{1}_payload.xml", dirInfo.FullName, dirInfo.Name);
                            if (CommunityFolderUpdated(File.GetLastWriteTimeUtc(payloadFilePath), dirInfo.FullName))
                            {
                                // Add to the to be deleted list. In case if the file cannot be deleted at the moment, later those files can be deleted.
                                if (!FilesToBeDeleted.Contains(payloadFilePath))
                                {
                                    FilesToBeDeleted.Add(payloadFilePath);
                                }

                                File.Delete(payloadFilePath);
                                FilesToBeDeleted.Remove(payloadFilePath);
                            }
                        }
                    }
                }
            }
            catch (SecurityException ex)
            {
                ErrorHandler.LogException(ex);
            }
            catch (UnauthorizedAccessException ex)
            {
                ErrorHandler.LogException(ex);
            }
            catch (ArgumentException ex)
            {
                ErrorHandler.LogException(ex);
            }
            catch (IOException ex)
            {
                ErrorHandler.LogException(ex);
            }
            catch (Exception ex)
            {
                ErrorHandler.LogException(ex);
            }
        }
    }
}