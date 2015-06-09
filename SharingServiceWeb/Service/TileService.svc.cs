//-----------------------------------------------------------------------
// <copyright file="TileService.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation 2011. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using System.Xml;
using System.Xml.XPath;

namespace Microsoft.Research.Wwt.SharingService.Web
{
    /// <summary>
    /// Class which controls the service.
    /// </summary>
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class TileService : ITileService
    {
        private ITileRepository pyramidRepositoryInstance;

        /// <summary>
        /// Initializes a new instance of the TileService class.
        /// </summary>
        public TileService()
        {
            pyramidRepositoryInstance = TileRepositoryFactory.Create();
        }

        /// <summary>
        /// Gets details for all pyramids.
        /// </summary>
        /// <returns>Pyramid details for the service.</returns>
        public PyramidDetails GetPyramidDetails()
        {
            Collection<Pyramid> pyramids = null;
            try
            {
                pyramids = pyramidRepositoryInstance.GetPyramidDetails();
                pyramids.ToList().ForEach(pyramid =>
                {
                    pyramid.ThumbNailPath = string.Format(CultureInfo.InvariantCulture, Constants.ThumbnailServicePath, pyramid.Name, pyramid.WtmlDetails.Name);
                    pyramid.TilePyramidPath = string.Format(CultureInfo.InvariantCulture, Constants.TileSharingServicePath, pyramid.Name, 0, 0, 0);
                    pyramid.WtmlPath = string.Format(CultureInfo.InvariantCulture, Constants.WTMLServicePath, pyramid.Name, pyramid.WtmlDetails.Name);
                });
            }
            catch (FaultException)
            {
                throw;
            }

            return new PyramidDetails { Location = pyramidRepositoryInstance.PyramidLocation, Pyramids = pyramids };
        }

        /// <summary>
        /// Gets tile image.
        /// </summary>
        /// <param name="id">ID for specified Tile.</param>
        /// <param name="level">Level for the image.</param>
        /// <param name="x">X axis of the image.</param>
        /// <param name="y">Y axis of the image.</param>
        /// <returns>Stream for the tile image for the specified level, x and y axis.</returns>
        [WebGet(UriTemplate = "/Tile?Id={id}&level={level}&x={x}&y={y}", BodyStyle = WebMessageBodyStyle.Bare)]
        public Stream GetTileImage(string id, int level, int x, int y)
        {
            Stream stream = null;
            try
            {
                OutgoingWebResponseContext context = WebOperationContext.Current.OutgoingResponse;
                context.Headers.Add(System.Net.HttpResponseHeader.CacheControl, "public");
                context.ContentType = "image/jpeg";
                context.StatusCode = System.Net.HttpStatusCode.OK;

                stream = pyramidRepositoryInstance.GetTileImage(id, level, x, y);
            }
            catch (FaultException)
            {
                throw;
            }
            return stream;
        }

        /// <summary>
        /// Gets tile image.
        /// </summary>
        /// <param name="id">ID for specified Tile.</param>
        /// <param name="level">Level for the image.</param>
        /// <param name="x">X axis of the image.</param>
        /// <param name="y">Y axis of the image.</param>
        /// <returns>Stream for the image with Dem data.</returns>
        [WebGet(UriTemplate = "/Dem?Id={id}&level={level}&x={x}&y={y}", BodyStyle = WebMessageBodyStyle.Bare)]
        public Stream GetDem(string id, int level, int x, int y)
        {
            Stream stream = null;
            try
            {
                OutgoingWebResponseContext context = WebOperationContext.Current.OutgoingResponse;
                context.Headers.Add(System.Net.HttpResponseHeader.CacheControl, "public");
                context.ContentType = "application/octet-stream";
                context.StatusCode = System.Net.HttpStatusCode.OK;

                stream = pyramidRepositoryInstance.GetDem(id, level, x, y);
            }
            catch (FaultException)
            {
                throw;
            }
            return stream;
        }

        /// <summary>
        /// Gets the thumbnail image.
        /// </summary>
        /// <param name="id">ID for specified Pyramid.</param>
        /// <param name="name">WTML Name of the Pyramid.</param>
        /// <returns>Stream for the image.</returns>
        [WebGet(UriTemplate = "/Thumbnail?id={id}&name={name}", BodyStyle = WebMessageBodyStyle.Bare)]
        public Stream GetThumbnailImage(string id, string name)
        {
            Stream stream = null;
            try
            {
                OutgoingWebResponseContext context = WebOperationContext.Current.OutgoingResponse;
                context.Headers.Add(System.Net.HttpResponseHeader.CacheControl, "public");
                context.ContentType = "image/jpeg";
                context.StatusCode = System.Net.HttpStatusCode.OK;

                stream = pyramidRepositoryInstance.GetThumbnailImage(id, name);
            }
            catch (FaultException)
            {
                throw;
            }
            return stream;
        }

        /// <summary>
        /// Get WTML image.
        /// </summary>
        /// <param name="id">ID for specified Pyramid.</param>
        /// <param name="name">Name of the WTML file.</param>
        /// <returns>Stream with WTML file data.</returns>
        [WebGet(UriTemplate = "/Wtml?id={id}&name={name}", BodyStyle = WebMessageBodyStyle.Bare)]
        public Stream GetWtmlFile(string id, string name)
        {
            MemoryStream stream = null;
            try
            {
                OutgoingWebResponseContext context = WebOperationContext.Current.OutgoingResponse;
                context.Headers.Add(System.Net.HttpResponseHeader.CacheControl, "public");
                context.ContentType = "application/xml";
                context.Headers.Add("content-disposition", "attachment;filename=" + name + ".wtml");
                context.StatusCode = System.Net.HttpStatusCode.OK;
                XmlDocument xmlDoc = (XmlDocument)pyramidRepositoryInstance.GetWtmlFile(id, name);
                if (xmlDoc != null)
                {
                    var operationContext = System.ServiceModel.OperationContext.Current;
                    string appPath = operationContext.Channel.LocalAddress.ToString();

                    // Update Image set node attribute and child node values to consider sharing service path.
                    XmlNode imageSet = xmlDoc.SelectSingleNode(Constants.ImageSetPath);
                    if (imageSet != null)
                    {
                        // Update URL attribute.
                        if (imageSet.Attributes[Constants.UrlAttribute] != null && !imageSet.Attributes[Constants.UrlAttribute].Value.IsValidUrl())
                        {
                            imageSet.Attributes[Constants.UrlAttribute].Value = appPath + string.Format(CultureInfo.InvariantCulture, Constants.WTMLServiceUrl, id) + Constants.WtmlFilepath;
                        }

                        // Update DEM URL attribute.
                        if (imageSet.Attributes[Constants.DEMUrl] != null &&
                                (!imageSet.Attributes[Constants.DEMUrl].Value.IsValidUrl() ||
                                imageSet.Attributes[Constants.DEMUrl].Value.StartsWith(Constants.DefaultDEMUrl, StringComparison.OrdinalIgnoreCase)))
                        {
                            imageSet.Attributes[Constants.DEMUrl].Value = appPath + string.Format(CultureInfo.InvariantCulture, Constants.DEMServiceUrl, id) + Constants.DemFilepath;
                        }

                        // Update Thumbnail URL.
                        if (imageSet.ChildNodes != null && imageSet.ChildNodes.Count > 0)
                        {
                            foreach (XmlNode node in imageSet.ChildNodes)
                            {
                                if (node.Name.Equals(Constants.ThumbnailUrl, StringComparison.Ordinal) && 
                                        imageSet.Attributes[Constants.WTMLName] != null &&
                                        !node.InnerText.IsValidUrl())
                                {
                                    node.InnerText = appPath + string.Format(CultureInfo.InvariantCulture, Constants.ThumbnailServiceUrl, id, imageSet.Attributes[Constants.WTMLName].Value);
                                    break;
                                }
                            }
                        }
                    }

                    // Update Place node attribute values to consider sharing service path.
                    XmlNode place = xmlDoc.SelectSingleNode(Constants.PlacePath);
                    if (place != null &&
                            place.Attributes[Constants.PlaceName] != null &&
                            !string.IsNullOrWhiteSpace(place.Attributes[Constants.PlaceName].Value) &&
                            place.Attributes[Constants.PlaceName].Value.Equals(imageSet.Attributes[Constants.PlaceName].Value, StringComparison.OrdinalIgnoreCase))
                    {
                        if (place.Attributes[Constants.PlaceThumbnailAttribute] == null)
                        {
                            place.Attributes.Append(xmlDoc.CreateAttribute(Constants.PlaceThumbnailAttribute));
                        }

                        // Update Thumbnail URL.
                        if (place.Attributes[Constants.PlaceThumbnailAttribute] != null &&
                                !place.Attributes[Constants.PlaceThumbnailAttribute].Value.IsValidUrl())
                        {
                            place.Attributes[Constants.PlaceThumbnailAttribute].Value = appPath + string.Format(CultureInfo.InvariantCulture, Constants.ThumbnailServiceUrl, id, place.Attributes[Constants.PlaceName].Value);
                        }
                    }

                    stream = new MemoryStream();
                    xmlDoc.Save(stream);
                    stream.Seek(0, SeekOrigin.Begin);
                }
            }
            catch (XPathException)
            {
                throw;
            }
            catch (FaultException)
            {
                throw;
            }

            return stream;
        }
    }
}
