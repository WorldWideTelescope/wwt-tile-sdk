//-----------------------------------------------------------------------
// <copyright file="Tour.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation 2011. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
using System;
using System.Globalization;
using System.Linq;
using System.Security;
using System.Xml.Serialization;

namespace Microsoft.Research.Wwt.SharingService.Web
{
    /// <summary>
    /// Class representing a Tour of a Community.
    /// </summary>
    [Serializable]
    public class Tour
    {
        /// <summary>
        /// Parent folder object.
        /// </summary>
        private Folder parent;

        /// <summary>
        /// Initializes a new instance of the Tour class.
        /// </summary>
        /// <param name="title">Title of the Tour</param>
        /// <param name="id">Id of the Tour</param>
        /// <param name="tourUrl">TourUrl for the tour</param>
        /// <param name="thumbnail">Thumbnail of the tour</param>
        internal Tour(string title, string id, string tourUrl, string thumbnail)
        {
            Title = title;
            ID = id;
            TourUrl = tourUrl;
            ThumbnailUrl = thumbnail;
        }

        /// <summary>
        /// Prevents a default instance of the Tour class from being created. Needed for XmlSerializer.
        /// </summary>
        private Tour()
        {
        }

        /// <summary>
        /// Gets or sets the parent folder of the tour.
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
                    parent.Tours.Add(this);

                    // Make sure the same Tour is not added already which could be there in some other folder.
                    if (!parent.RootFolder.Children[0].Tours.Any(e => e.ID == this.ID))
                    {
                        // Add the tour to the "All Tours" folder which is the first children of RootFolder.
                        parent.RootFolder.Children[0].Tours.Add(this);
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets the Title attribute value for the tour.
        /// </summary>
        [XmlAttribute]
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets the URL attribute value for the tour.
        /// </summary>
        [XmlAttribute]
        public string ID { get; set; }

        /// <summary>
        /// Gets or sets the description attribute value for the tour.
        /// </summary>
        /// <remarks>
        /// Spelling is wrong in WWT, DO NOT change this.
        /// </remarks>
        [XmlAttribute("Descirption")]
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the Author attribute value for the tour.
        /// </summary>
        [XmlAttribute]
        public string Author { get; set; }

        /// <summary>
        /// Gets or sets the OrganizationUrl attribute value for the tour.
        /// </summary>
        [XmlAttribute]
        public string OrganizationUrl { get; set; }

        /// <summary>
        /// Gets or sets the OrganizationName attribute value for the tour.
        /// </summary>
        [XmlAttribute]
        public string OrganizationName { get; set; }

        /// <summary>
        /// Gets or sets the TourUrl attribute value for the tour.
        /// </summary>
        [XmlAttribute]
        public string TourUrl { get; set; }

        /// <summary>
        /// Gets or sets the AuthorImageUrl attribute value for the tour.
        /// </summary>
        [XmlAttribute]
        public string AuthorImageUrl { get; set; }

        /// <summary>
        /// Gets or sets the ThumbnailUrl attribute value for the tour.
        /// </summary>
        [XmlAttribute]
        public string ThumbnailUrl { get; set; }

        /// <summary>
        /// Gets or sets the Modified date of the tour file.
        /// </summary>
        [XmlAttribute]
        public DateTime ModifiedDate { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether URL rewritten happened or not.
        /// </summary>
        [XmlIgnore]
        internal bool UrlRewritten { get; set; }

        /// <summary>
        /// Rewrites the Thumbnail path and the Tour URL of the tour for the community payload.
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

            // Special characters in these properties needs to be escaped so that XML serialization will work fine.
            Title = SecurityElement.Escape(Title);
            Description = SecurityElement.Escape(Description);
            Author = SecurityElement.Escape(Author);
            OrganizationUrl = SecurityElement.Escape(OrganizationUrl);
            OrganizationName = SecurityElement.Escape(OrganizationName);
            TourUrl = SecurityElement.Escape(TourUrl);
            AuthorImageUrl = SecurityElement.Escape(AuthorImageUrl);
            ThumbnailUrl = SecurityElement.Escape(ThumbnailUrl);

            if (string.IsNullOrWhiteSpace(ThumbnailUrl))
            {
                ThumbnailUrl = applicationPath + Constants.DefaultTourThumbnail;
            }
            else if (!ThumbnailUrl.IsValidUrl())
            {
                ThumbnailUrl = string.Format(CultureInfo.InvariantCulture, Constants.FileServicePath, serviceUrl, ThumbnailUrl);
            }

            if (!string.IsNullOrWhiteSpace(TourUrl) && !TourUrl.IsValidUrl())
            {
                TourUrl = string.Format(CultureInfo.InvariantCulture, Constants.FileServicePath, serviceUrl, TourUrl);
            }
        }
    }
}
