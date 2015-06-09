//-----------------------------------------------------------------------
// <copyright file="SignUp.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation 2011. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
using System;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using System.Globalization;
using System.IO;

namespace Microsoft.Research.Wwt.SharingService.Web
{
    /// <summary>
    /// Class representing a Signup details of a Community.
    /// </summary>
    [Serializable]
    [XmlRoot("Folder")]
    public class SignUp
    {
        /// <summary>
        /// Initializes a new instance of the SignUp class.
        /// </summary>
        /// <param name="name">Name of the SignUp</param>
        /// <param name="thumbnail">Thumbnail of the SignUp</param>
        internal SignUp(string name, string thumbnail)
        {
            Name = name;
            Thumbnail = thumbnail;

            // Set default values for sign up xml.
            Group = "Community";
            Searchable = "True";
            CommunityType = "Earth";
        }

        /// <summary>
        /// Prevents a default instance of the SignUp class from being created. Needed for XmlSerializer.
        /// </summary>
        private SignUp()
        {
        }

        /// <summary>
        /// Gets or sets the name attribute value for the SignUp.
        /// </summary>
        [XmlAttribute]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the Group attribute value for the SignUp.
        /// </summary>
        [XmlAttribute]
        public string Group { get; set; }

        /// <summary>
        /// Gets or sets the Searchable attribute value for the SignUp.
        /// </summary>
        [XmlAttribute]
        public string Searchable { get; set; }

        /// <summary>
        /// Gets or sets the Type attribute value for the SignUp.
        /// </summary>
        [XmlAttribute("Type")]
        public string CommunityType { get; set; }

        /// <summary>
        /// Gets or sets the URL attribute value for the SignUp.
        /// </summary>
        [XmlAttribute]
        public string Url { get; set; }

        /// <summary>
        /// Gets or sets the thumbnail attribute value for the SignUp.
        /// </summary>
        [XmlAttribute]
        public string Thumbnail { get; set; }

        /// <summary>
        /// Rewrites the Thumbnail path and the URL of the Signup XML for the community signup.
        /// </summary>
        /// <param name="serviceUrl">Community service URL</param>
        /// <param name="applicationPath">Application where the service is hosted.</param>
        /// <param name="communityId">Community Id, which is the relative path of the community.</param>
        /// <remarks>
        /// Service URL represents the URL of the Community service whereas application path represents the WebSite or Virtual directory 
        /// where the service and all its files are deployed.
        /// </remarks>
        internal void RewriteLocalUrls(string serviceUrl, string applicationPath, string communityId)
        {
            if (string.IsNullOrWhiteSpace(Thumbnail))
            {
                Thumbnail = applicationPath + Constants.DefaultCommunityThumbnail;
            }
            else
            {
                Thumbnail = string.Format(CultureInfo.InvariantCulture, Constants.FileServicePath, serviceUrl, Thumbnail);
            }

            Url = string.Format(CultureInfo.InvariantCulture, Constants.PayloadServicePath, serviceUrl, communityId);
        }

        /// <summary>
        /// Serializes the signup XML content and returns the content as a memory stream.
        /// </summary>
        /// <returns>Stream having the Signup xml string.</returns>
        internal Stream GetXmlStream()
        {
            MemoryStream stream = null;

            try
            {
                StringBuilder sb = new StringBuilder();
                XmlSerializer x = new XmlSerializer(typeof(SignUp));
                using (XmlWriter xw = XmlWriter.Create(sb))
                {
                    XmlSerializerNamespaces emptyNamespace = new XmlSerializerNamespaces();
                    emptyNamespace.Add(string.Empty, string.Empty);
                    x.Serialize(xw, this, emptyNamespace);
                }

                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(sb.ToString());
                stream = new MemoryStream();
                xmlDoc.Save(stream);
                stream.Seek(0, SeekOrigin.Begin);
            }
            catch (XmlException)
            {
                // Return null stream in case of exception.
            }

            return stream;
        }
    }
}