//-----------------------------------------------------------------------
// <copyright file="Extensions.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation 2011. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Xml;

namespace Microsoft.Research.Wwt.SharingService.Web
{
    /// <summary>
    /// Class having implementation for extension methods.
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// Image file extensions.
        /// </summary>
        private static string[] thumbnailImageFormats = new string[]
        { 
            ImageFormat.Bmp.ToString().ToUpperInvariant(),
            ImageFormat.Gif.ToString().ToUpperInvariant(),
            ImageFormat.Jpeg.ToString().ToUpperInvariant(),
            ImageFormat.Png.ToString().ToUpperInvariant(),
            ImageFormat.Tiff.ToString().ToUpperInvariant()
        };

        /// <summary>
        /// Checks whether the current string ends with any of the string in the
        /// given collection.
        /// </summary>
        /// <param name="value">String value to be compared.</param>
        /// <param name="endsWithCollection">Items with which the string ends.</param>
        /// <returns>
        /// True, if current string ends with any of the string in the given collection.
        /// False, if current string not ends with any of the string in collection.
        /// </returns>
        public static bool EndsWithAny(this string value, IEnumerable<string> endsWithCollection) 
        {
            bool endsWith = false;

            if (value != null)
            {
                endsWith = endsWithCollection.Any(value.EndsWith);
            }

            return endsWith;
        }

        /// <summary>
        /// Gets thumbnail relative file path, if any image file with name Thumbnail exists in the given directory.
        /// </summary>
        /// <param name="value">Current directory</param>
        /// <param name="communityFolderName">Community folder name which will be removed from the thumbnail path to get relative path.</param>
        /// <returns>Thumbnail file name.</returns>
        public static string GetThumbnailFilePath(this DirectoryInfo value, string communityFolderName)
        {
            string thumbnailFile = string.Empty;

            if (value != null)
            {
                FileInfo fileInfo = value.GetFiles(Constants.ThumbnailSearchPattern).Where(f => f.Name.ToUpperInvariant().EndsWithAny(thumbnailImageFormats)).FirstOrDefault();
                if (null != fileInfo)
                {
                    thumbnailFile = fileInfo.FullName.Replace(communityFolderName, string.Empty).TrimStartSlashes();
                }
            }

            return thumbnailFile;
        }
        
        /// <summary>
        /// Trims the slashes from the given string in the starting location of the string.
        /// </summary>
        /// <param name="value">Value in which slash to be removed.</param>
        /// <returns>Slash trimmed string.</returns>
        public static string TrimStartSlashes(this string value)
        {
            if (value != null)
            {
                value = value.TrimStart(new char[] { '\\', '/' });
            }

            return value;
        }

        /// <summary>
        /// Gets the value for the given attribute in the given element of the current xml document.
        /// </summary>
        /// <param name="value">Xml Document object</param>
        /// <param name="elementXPath">XPath of the element.</param>
        /// <returns>Value of the element</returns>
        public static string GetElementValue(this XmlDocument value, string elementXPath)
        {
            string elementValue = string.Empty;

            if (value != null)
            {
                XmlNode firstElement = value.SelectSingleNode(elementXPath);
                if (firstElement != null && firstElement.FirstChild.NodeType == XmlNodeType.Text)
                {
                    elementValue = firstElement.InnerText;
                }
            }

            return elementValue;
        }

        /// <summary>
        /// Gets the value for the given attribute in the given element of the current xml document.
        /// </summary>
        /// <param name="value">Xml Document object</param>
        /// <param name="elementXPath">XPath of the element.</param>
        /// <param name="attributeName">Attribute whose value to be returned.</param>
        /// <returns>Value of the attribute</returns>
        public static string GetAttributeValue(this XmlDocument value, string elementXPath, string attributeName)
        {
            string attributeValue = string.Empty;

            if (value != null)
            {
                XmlNode firstElement = value.SelectSingleNode(elementXPath);
                if (firstElement != null && firstElement.Attributes[attributeName] != null)
                {
                    attributeValue = firstElement.Attributes[attributeName].Value;
                }
            }

            return attributeValue;
        }

        /// <summary>
        /// Checks whether the file represented by FileInfo object is an image file or not.
        /// </summary>
        /// <param name="value">FileInfo object.</param>
        /// <returns>True, if the file is image. False, otherwise</returns>
        public static bool IsImageFile(this FileSystemInfo value)
        {
            bool imageFile = false;
            if (value != null)
            {
                string extension = value.Extension.Remove(0, 1).ToUpperInvariant();
                if (extension.EndsWithAny(thumbnailImageFormats))
                {
                    imageFile = true;
                }
            }

            return imageFile;
        }

        /// <summary>
        /// Checks whether the file represented by FileInfo object is latest file to be added in the Latest folder of the community.
        /// </summary>
        /// <param name="value">FileInfo object.</param>
        /// <param name="daysToConsider">Days to consider the file as latest.</param>
        /// <returns>True, if the file is latest. False, otherwise</returns>
        public static bool IsLatestFile(this FileSystemInfo value, int daysToConsider)
        {
            bool latestFile = false;
            if (value != null)
            {
                DateTime latestDateTime = DateTime.UtcNow.AddDays(-(daysToConsider));
                if (value.LastWriteTimeUtc > latestDateTime || value.CreationTimeUtc > latestDateTime)
                {
                    // If file last write time or creation time is greater than the date to consider for latest files, then return true.
                    latestFile = true;
                }
            }

            return latestFile;
        }

        /// <summary>
        /// Gets the modified date time for the file.
        /// For the files which are copied from other locations, creation date time to be considered as modified time.
        /// If the files are copied from other locations, creation date time will be greater than modified date time.
        /// </summary>
        /// <param name="value">FileInfo object</param>
        /// <returns>Modified date time for the file</returns>
        public static DateTime GetModifiedDate(this FileSystemInfo value)
        {
            DateTime modifiedDate = DateTime.MinValue;

            if (value != null)
            {
                if (value.LastWriteTimeUtc > value.CreationTimeUtc)
                {
                    modifiedDate = value.LastWriteTimeUtc;
                }
                else
                {
                    modifiedDate = value.CreationTimeUtc;
                }
            }

            return modifiedDate;
        }

        /// <summary>
        /// Checks whether the file represented by file name is an Excel file or not.
        /// </summary>
        /// <param name="value">file name string.</param>
        /// <returns>True, if the file is Excel. False, otherwise</returns>
        public static bool IsExcelFile(this string value)
        {
            bool excelFile = false;
            if (!string.IsNullOrWhiteSpace(value) && !string.IsNullOrWhiteSpace(Path.GetExtension(value)))
            {
                string extension = Path.GetExtension(value).Remove(0, 1).ToUpperInvariant();
                if (extension.EndsWithAny(new string[] { "XLS", "XLSX" }))
                {
                    excelFile = true;
                }
            }

            return excelFile;
        }

        /// <summary>
        /// Checks whether the string is a valid URL string. URL can start with http or https.
        /// </summary>
        /// <param name="value">String to be checked</param>
        /// <returns>True, if string is valid URL. False, otherwise.</returns>
        public static bool IsValidUrl(this string value)
        {
            bool validUrl = false;

            if (!string.IsNullOrWhiteSpace(value))
            {
                if (value.StartsWith("http://", StringComparison.OrdinalIgnoreCase) ||
                        value.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
                {
                    validUrl = true;
                }
            }

            return validUrl;
        }
    }
}