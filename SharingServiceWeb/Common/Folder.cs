//-----------------------------------------------------------------------
// <copyright file="Folder.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation 2011. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using System.Xml.XPath;

namespace Microsoft.Research.Wwt.SharingService.Web
{
    /// <summary>
    /// Class representing a community folder and its properties/contents. Contents may be other folders and files like,
    /// tours, collections (WTML) and links.
    /// </summary>
    [Serializable]
    public class Folder
    {
        /// <summary>
        /// Initializes a new instance of the Folder class.
        /// </summary>
        /// <param name="name">Folder name</param>
        /// <param name="parent">Parent folder</param>
        internal Folder(string name, Folder parent)
        {
            Name = name;
            Parent = parent;
            Children = new Collection<Folder>();
            Links = new Collection<Place>();
            Tours = new Collection<Tour>();

            // Only for community root folder, parent will be null.
            if (parent == null)
            {
                Group = "Explorer";
                Searchable = "True";
                FolderType = "Earth";
                RootFolder = this;
            }
            else
            {
                parent.Children.Add(this);
                RootFolder = Parent.RootFolder;
            }
        }

        /// <summary>
        /// Prevents a default instance of the Folder class from being created. Needed for XmlSerializer.
        /// </summary>
        private Folder()
        {
        }

        /// <summary>
        /// Gets or sets the Parent folder of the current folder.
        /// </summary>
        [XmlIgnore]
        public Folder Parent { get; set; }

        /// <summary>
        /// Gets or sets the Root folder of the current folder.
        /// </summary>
        [XmlIgnore]
        public Folder RootFolder { get; set; }

        /// <summary>
        /// Gets or sets all child directories of the current folder.
        /// </summary>
        [XmlElement("Folder")]
        public Collection<Folder> Children { get; set; }

        /// <summary>
        /// Gets or sets the links present in the current folder.
        /// </summary>
        [XmlElement("Place")]
        public Collection<Place> Links { get; set; }

        /// <summary>
        /// Gets or sets the tours present in the current folder.
        /// </summary>
        [XmlElement("Tour")]
        public Collection<Tour> Tours { get; set; }

        /// <summary>
        /// Gets or sets the name of the folder.
        /// </summary>
        [XmlAttribute]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the group of the folder.
        /// </summary>
        [XmlAttribute]
        public string Group { get; set; }

        /// <summary>
        /// Gets or sets the searchable property of the folder.
        /// </summary>
        [XmlAttribute]
        public string Searchable { get; set; }

        /// <summary>
        /// Gets or sets the Type property of the folder.
        /// </summary>
        [XmlAttribute("Type")]
        public string FolderType { get; set; }

        /// <summary>
        /// Gets or sets the Thumbnail path of the folder.
        /// </summary>
        [XmlAttribute]
        public string Thumbnail { get; set; }

        /// <summary>
        /// Gets or sets the inner text for the Folder. Useful when parsing an WTML where only the contents of file needs to 
        /// copied here.
        /// </summary>
        [XmlText]
        public string InnerText { get; set; }

        /// <summary>
        /// Rewrites the Thumbnail path and the URL of the Payload XML for the community.
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
            foreach (Folder folder in Children)
            {
                // Loop through all the folder and replace URLs for the objects in them also.
                folder.RewriteLocalUrls(serviceUrl, applicationPath, communityId);
            }

            // Rewrite URLs in all Tours
            foreach (Tour tour in Tours)
            {
                // URL rewritten might have happened already since Tours will be added to All Tours folder and may be in Latest folder also.
                // If it is already done, do not do it again.
                if (!tour.UrlRewritten)
                {
                    tour.RewriteLocalUrls(serviceUrl, applicationPath);
                }
            }

            // Rewrite URLs in all Links
            foreach (Place place in Links)
            {
                // URL rewritten might have happened already since Links may be added to Latest folder. If it is already done, do not do it again.
                if (!place.UrlRewritten)
                {
                    place.RewriteLocalUrls(serviceUrl, applicationPath);
                }
            }

            if (string.IsNullOrWhiteSpace(Thumbnail))
            {
                // Only for community (root folder), Group, Searchable and FolderType properties will be non empty.
                if (!string.IsNullOrWhiteSpace(Group) && !string.IsNullOrWhiteSpace(Searchable) && !string.IsNullOrWhiteSpace(FolderType))
                {
                    Thumbnail = applicationPath + Constants.DefaultCommunityThumbnail;
                }
            }
            else if (!Thumbnail.IsValidUrl())
            {
                Thumbnail = string.Format(CultureInfo.InvariantCulture, Constants.FileServicePath, serviceUrl, Thumbnail);
            }

            if (!string.IsNullOrWhiteSpace(InnerText))
            {
                // For the WTML file contents replace the relative URLs.
                ReplaceRelativeUrlInInnerText(serviceUrl, applicationPath);
            }
        }

        /// <summary>
        /// Serializes the Payload XML content and returns the content as a memory stream.
        /// </summary>
        /// <returns>Stream having the Payload xml string.</returns>
        internal Stream GetXmlStream()
        {
            MemoryStream stream = null;

            try
            {
                StringBuilder sb = new StringBuilder();
                XmlSerializer x = new XmlSerializer(typeof(Folder));
                using (XmlWriter xw = XmlWriter.Create(sb))
                {
                    XmlSerializerNamespaces emptyNamespace = new XmlSerializerNamespaces();
                    emptyNamespace.Add(string.Empty, string.Empty);
                    x.Serialize(xw, this, emptyNamespace);
                }

                // Since the contents of WTML are added as InnerText, < and > will be encoded which needs to be decoded again.
                string xmlContent = sb.Replace("&lt;", "<").Replace("&gt;", ">").Replace("&amp;", "&").ToString();

                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(xmlContent);

                // Sort the items (Tours, Links/Places and WTML collections) of the Latest folder.
                SortLatestFolderContents(xmlDoc);

                // Remove the custom attribute (ModifiedDate) which is added for sorting the "Latest" folder.
                RemoveCustomAttributes(xmlDoc);

                stream = new MemoryStream();
                xmlDoc.Save(stream);
                stream.Seek(0, SeekOrigin.Begin);
            }
            catch (XmlException ex)
            {
                // Return null stream in case of exception.
                ErrorHandler.LogException(ex);
            }

            return stream;
        }

        /// <summary>
        /// Removes the custom attributes added for generating the payload XML which are not part of
        /// Tours, Links/Places and WTML collections.
        /// </summary>
        /// <param name="xmlDoc">Xml Document object</param>
        private static void RemoveCustomAttributes(XmlDocument xmlDoc)
        {
            if (xmlDoc != null)
            {
                XmlNodeList nodes = xmlDoc.SelectNodes(Constants.ItemsWithModifiedDateAttributePath);
                foreach (XmlNode node in nodes)
                {
                    node.Attributes.RemoveNamedItem(Constants.ModifiedDateAttribute);
                }
            }
        }

        /// <summary>
        /// Sorts the items (Place/Links, Tours and WTML collections) of the "Latest" folder and updates the payload XML.
        /// </summary>
        /// <param name="xmlDoc">Xml Document object</param>
        private static void SortLatestFolderContents(XmlDocument xmlDoc)
        {
            if (xmlDoc != null)
            {
                try
                {
                    XDocument document = XDocument.Parse(xmlDoc.OuterXml);

                    IEnumerable latestFolderEnumerable = (IEnumerable)document.XPathEvaluate(Constants.LatestFolderPath);
                    XElement latestFolder = latestFolderEnumerable.Cast<XElement>().FirstOrDefault();

                    if (latestFolder != null)
                    {
                        var sortedLatestFolder = latestFolder.Elements().OrderByDescending(s => (DateTime)s.Attribute(Constants.ModifiedDateAttribute));

                        if (sortedLatestFolder != null)
                        {
                            xmlDoc.DocumentElement.ChildNodes[1].InnerXml = string.Concat(sortedLatestFolder.Select(x => x.ToString()).ToArray());
                        }
                    }
                }
                catch (XmlException ex)
                {
                    // Ignore the exception since it is used only for sorting.
                    ErrorHandler.LogException(ex);
                }
                catch (ArgumentNullException ex)
                {
                    // Ignore the exception since it is used only for sorting.
                    ErrorHandler.LogException(ex);
                }
            }
        }

        /// <summary>
        /// Replace all relative URL reference in the WTML file.
        /// </summary>
        /// <param name="serviceUrl">Service URL</param>
        /// <param name="applicationPath">Application path</param>
        private void ReplaceRelativeUrlInInnerText(string serviceUrl, string applicationPath)
        {
            try
            {
                XmlDocument wtmlFile = new XmlDocument();

                // There could be changes that multiple WTML files present in a folder and they will not form a proper XML because of
                // missing root node. Add a dummy root node and start replacing the URL.
                string wtmlFileContent = string.Format(CultureInfo.InvariantCulture, "<Dummy>{0}</Dummy>", InnerText);
                wtmlFile.LoadXml(wtmlFileContent);

                // Get all the nodes which are having URL attribute.
                XmlNodeList nodesWithUrl = wtmlFile.SelectNodes(Constants.NodesWithUrlAttributeXPath);
                foreach (XmlNode node in nodesWithUrl)
                {
                    string urlValue = node.Attributes[Constants.UrlAttribute].Value;
                    if (!urlValue.IsValidUrl())
                    {
                        if (node.Attributes[Constants.UrlAttribute].Value != null)
                        {
                            // If the URL is not valid (doesn't start with http or https, rewrite the URL with the service URL.
                            node.Attributes[Constants.UrlAttribute].Value = string.Format(
                                    CultureInfo.InvariantCulture,
                                    "{0}{1}{2}",
                                    serviceUrl,
                                    string.Format(CultureInfo.InvariantCulture, Constants.WTMLServiceUrl, node.Attributes[Constants.UrlAttribute].Value),
                                    Constants.WtmlFilepath);
                        }

                        if (node.Attributes[Constants.DEMUrl] != null && !node.Attributes[Constants.DEMUrl].Value.IsValidUrl())
                        {
                            // If the URL is not valid (doesn't start with http or https, rewrite the URL with the service URL.
                            node.Attributes[Constants.DEMUrl].Value = string.Format(
                                    CultureInfo.InvariantCulture,
                                    "{0}{1}{2}",
                                    serviceUrl,
                                    string.Format(CultureInfo.InvariantCulture, Constants.DEMServiceUrl, node.Attributes[Constants.DEMUrl].Value),
                                    Constants.DemFilepath);
                        }

                        // Get the thumbnail node and do the same thing if thumbnail node exists.
                        XmlNode thumbnailNode = node.SelectSingleNode(Constants.ThumbnailUrl);
                        if (thumbnailNode != null)
                        {
                            if (string.IsNullOrWhiteSpace(thumbnailNode.InnerText))
                            {
                                // Add default thumbnail node only in case if there is a thumbnail element and it is empty.
                                thumbnailNode.InnerText = applicationPath + Constants.DefaultWtmlThumbnail;
                            }
                            else if (!thumbnailNode.InnerText.IsValidUrl())
                            {
                                thumbnailNode.InnerText = string.Format(CultureInfo.InvariantCulture, Constants.FileServicePath, serviceUrl, thumbnailNode.InnerText);
                            }

                            // Thumbnail update for Place node if the names for place node and image set node match
                            if (node.Attributes[Constants.PlaceName] != null)
                            {
                                XmlNode parentNode = node.ParentNode;
                                if (parentNode != null)
                                {
                                    XmlNode placeNode = parentNode.ParentNode;

                                    // Place node present and has the same name as that of the image set node.
                                    if (placeNode != null && 
                                            placeNode.Attributes != null && 
                                            placeNode.Attributes[Constants.PlaceName] != null &&
                                            !string.IsNullOrWhiteSpace(placeNode.Attributes[Constants.PlaceName].Value) &&
                                            placeNode.Attributes[Constants.PlaceName].Value.Equals(node.Attributes[Constants.PlaceName].Value, StringComparison.OrdinalIgnoreCase))
                                    {
                                        if (placeNode.Attributes[Constants.PlaceThumbnailAttribute] == null)
                                        {
                                            placeNode.Attributes.Append(wtmlFile.CreateAttribute(Constants.PlaceThumbnailAttribute));
                                        }

                                        if (!placeNode.Attributes[Constants.PlaceThumbnailAttribute].Value.IsValidUrl())
                                        {
                                            // Set the Thumbnail value same as that of the image set thumbnail value
                                            placeNode.Attributes[Constants.PlaceThumbnailAttribute].Value = thumbnailNode.InnerText;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                // Set the updated InnerText back which is having replaced URLs.
                InnerText = wtmlFile.DocumentElement.InnerXml;
            }
            catch (XmlException ex)
            {
                // Any issue with XML, ignore inner text.
                ErrorHandler.LogException(ex);
            }
        }
    }
}
