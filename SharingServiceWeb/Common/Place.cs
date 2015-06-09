//-----------------------------------------------------------------------
// <copyright file="Place.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation 2011. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Globalization;
using System.Security;
using System.Xml;
using System.Xml.Serialization;

namespace Microsoft.Research.Wwt.SharingService.Web
{
    /// <summary>
    /// Class representing a place, i.e. links of a Community.
    /// </summary>
    [Serializable]
    public class Place
    {
        /// <summary>
        /// Parent folder object.
        /// </summary>
        private Folder parent;

        /// <summary>
        /// Initializes a new instance of the Place class.
        /// </summary>
        /// <param name="name">Name of the link</param>
        /// <param name="url">Url of the link</param>
        /// <param name="thumbnail">Thumbnail of the link</param>
        internal Place(string name, string url, string thumbnail)
        {
            Name = name;
            Url = url;
            Thumbnail = thumbnail;
        }

        /// <summary>
        /// Prevents a default instance of the Place class from being created. Needed for XmlSerializer.
        /// </summary>
        private Place()
        {
        }

        /// <summary>
        /// Gets or sets the parent folder of the link.
        /// </summary>
        [XmlIgnore]
        public Folder Parent
        { 
            get
            {
                return parent;
            }
            set 
            {
                if (value != null)
                {
                    parent = value;
                    parent.Links.Add(this);
                }
            } 
        }

        /// <summary>
        /// Gets or sets a value indicating whether the link is local (file in the communities folder) or external (files outside communities folder, may be Web).
        /// </summary>
        [XmlIgnore]
        public bool IsLocal { get; set; }

        /// <summary>
        /// Gets or sets the name attribute value for the link.
        /// </summary>
        [XmlAttribute]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the URL attribute value for the link.
        /// </summary>
        [XmlAttribute]
        public string Url { get; set; }

        /// <summary>
        /// Gets or sets the thumbnail attribute value for the link.
        /// </summary>
        [XmlAttribute]
        public string Thumbnail { get; set; }

        /// <summary>
        /// Gets or sets the Modified date of the Link file.
        /// </summary>
        [XmlAttribute]
        public DateTime ModifiedDate { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether URL rewritten happened or not.
        /// </summary>
        [XmlIgnore]
        internal bool UrlRewritten { get; set; }

        /// <summary>
        /// Rewrites the Thumbnail path and URL of the links for the community payload.
        /// </summary>
        /// <param name="serviceUrl">Community service URL</param>
        /// <param name="applicationPath">Application where the service is hosted.</param>
        /// <remarks>
        /// Service URL represents the URL of the Community service whereas application path represents the WebSite or Virtual directory 
        /// where the service and all its files are deployed.
        /// </remarks>
        internal void RewriteLocalUrls(string serviceUrl, string applicationPath)
        {
            UrlRewritten = true;

            if (string.IsNullOrWhiteSpace(Thumbnail))
            {
                // If no thumbnail is specified, add the default thumbnail URL. Excel file will be having different thumbnail icon and
                // all other files will be having a different thumbnail icon.
                if (Name.IsExcelFile())
                {
                    Thumbnail = applicationPath + Constants.DefaultExcelThumbnail;
                }
                else if (IsLocal)
                {
                    // For local files, use default file thumbnail icon.
                    Thumbnail = applicationPath + Constants.DefaultFileThumbnail;
                }
                else
                {
                    // For external links, use default link thumbnail icon.
                    Thumbnail = applicationPath + Constants.DefaultLinkThumbnail;
                }
            }
            else if (IsLocal && !Thumbnail.IsValidUrl())
            {
                // Only for local file links, if already a thumbnail is specified and it is not an valid URL, then rewrite the URL.
                // Links which are obtained from Link*.txt files will not be modified.
                Thumbnail = string.Format(CultureInfo.InvariantCulture, Constants.FileServicePath, serviceUrl, Thumbnail);
            }

            // Only for local links (Place tags added for the files inside a community), rewrite the URL.
            // If the URL is coming from parsed text file, do not rewrite.
            if (IsLocal && !string.IsNullOrWhiteSpace(Url) && !Url.IsValidUrl())
            {
                Url = string.Format(CultureInfo.InvariantCulture, Constants.FileServicePath, serviceUrl, Url);
            }

            // Any XML special characters mentioned in the URL to be decoded.
            Url = SecurityElement.Escape(Url);
            Name = SecurityElement.Escape(Name);

            // Need this query string to open the link in a maximized browser when WWT opening the links.
            // This query string should be at the end of the URL string, which is expected by WWT.
            if (!string.IsNullOrWhiteSpace(Url) && !Url.EndsWith("wwtfull", StringComparison.OrdinalIgnoreCase))
            {
                if (Url.Contains("?"))
                {
                    Url += "&amp;wwtfull";
                }
                else
                {
                    Url += "?&amp;wwtfull";
                }
            }
        }
    }
}
