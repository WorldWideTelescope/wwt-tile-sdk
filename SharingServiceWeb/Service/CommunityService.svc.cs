//-----------------------------------------------------------------------
// <copyright file="CommunityService.svc.cs" company="Microsoft Corporation">
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

namespace Microsoft.Research.Wwt.SharingService.Web
{
    /// <summary>
    /// Class which controls the community service by handling all the requests and sending responses.
    /// </summary>
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class CommunityService : ICommunityService
    {
        /// <summary>
        /// ContentType for .docx files.
        /// </summary>
        private const string ContentTypeDocx = "application/vnd.openxmlformats-officedocument.wordprocessingml.document";

        /// <summary>
        /// ContentType for .xlsx files.
        /// </summary>
        private const string ContentTypeXlsx = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

        /// <summary>
        /// ContentType for .pptx files.
        /// </summary>
        private const string ContentTypePptx = "application/vnd.openxmlformats-officedocument.presentationml.presentation";

        /// <summary>
        /// Community repository instance.
        /// </summary>
        private ICommunityRepository communityRepositoryInstance;

        /// <summary>
        /// Initializes a new instance of the CommunityService class.
        /// </summary>
        public CommunityService()
        {
            communityRepositoryInstance = CommunityRepositoryFactory.Create();
        }

        /// <summary>
        /// Gets details for all communities.
        /// </summary>
        /// <returns>Community details</returns>
        public CommunityDetails GetAllCommunites()
        {
            Collection<Community> communities = null;
            try
            {
                communities = communityRepositoryInstance.GetAllCommunites();

                var operationContext = System.ServiceModel.OperationContext.Current;
                
                // This method will be called from the CommunityServiceSample page hosted which will not have the CommunityResources URL.
                // Need to create the CommunityResources URI to get the service URL and application path to get the files with appropriate URL.
                Uri communityResourcesUri = new Uri(string.Format(CultureInfo.InvariantCulture, "{0}/CommunityResources", operationContext.Channel.LocalAddress.ToString()));
                string serviceUrl = communityResourcesUri.AbsoluteUri;
                string applicationPath = GetApplicationPath(communityResourcesUri);

                communities.ToList().ForEach(community =>
                {
                    community.RewriteLocalUrls(serviceUrl, applicationPath, community.Id);
                });
            }
            catch (FaultException)
            {
                throw;
            }

            return new CommunityDetails { Location = communityRepositoryInstance.CommunityLocation, Communities = communities };
        }

        /// <summary>
        /// Gets the signup WTML file for the given community.
        /// </summary>
        /// <returns>Signup file stream.</returns>
        [WebGet(UriTemplate = "/Signup?CommunityId={communityId}", BodyStyle = WebMessageBodyStyle.Bare)]
        public Stream GetSignUpFile(string communityId)
        {
            Stream stream = null;
            try
            {
                OutgoingWebResponseContext context = WebOperationContext.Current.OutgoingResponse;
                context.Headers.Add(System.Net.HttpResponseHeader.CacheControl, "public");
                context.ContentType = "application/xml";
                context.StatusCode = System.Net.HttpStatusCode.OK;
                string signUpFileName = string.Format(CultureInfo.InvariantCulture, Constants.SignUpFileNameFormat, communityId);
                context.Headers.Add("content-disposition", "attachment;filename=" + signUpFileName);
                SignUp signup = communityRepositoryInstance.GetSignUpDetail(communityId);
                if (signup != null)
                {
                    var operationContext = System.ServiceModel.OperationContext.Current;
                    string serviceUrl = operationContext.Channel.LocalAddress.ToString();
                    string applicationPath = GetApplicationPath(operationContext.Channel.LocalAddress.Uri);
                    signup.RewriteLocalUrls(serviceUrl, applicationPath, communityId);
                    stream = signup.GetXmlStream();
                }
            }
            catch (FaultException)
            {
                throw;
            }

            return stream;
        }

        /// <summary>
        /// Gets the Payload WTML file for the given community.
        /// </summary>
        /// <returns>Payload file stream.</returns>
        [WebGet(UriTemplate = "/Payload?CommunityId={communityId}", BodyStyle = WebMessageBodyStyle.Bare)]
        public Stream GetPayloadFile(string communityId)
        {
            Stream stream = null;
            try
            {
                OutgoingWebResponseContext context = WebOperationContext.Current.OutgoingResponse;
                context.Headers.Add(System.Net.HttpResponseHeader.CacheControl, "public");
                context.ContentType = "application/xml";
                context.StatusCode = System.Net.HttpStatusCode.OK;
                var operationContext = System.ServiceModel.OperationContext.Current;
                Folder rootFolder = communityRepositoryInstance.GetPayloadDetails(communityId);

                if (rootFolder != null)
                {
                    string serviceUrl = operationContext.Channel.LocalAddress.ToString();
                    string applicationPath = GetApplicationPath(operationContext.Channel.LocalAddress.Uri);
                    rootFolder.RewriteLocalUrls(serviceUrl, applicationPath, communityId);

                    stream = rootFolder.GetXmlStream();
                }
            }
            catch (FaultException)
            {
                throw;
            }

            return stream;
        }

        /// <summary>
        /// Gets the file identified by the given id from the communities folder.
        /// </summary>
        /// <returns>File stream.</returns>
        [WebGet(UriTemplate = "/File?id={id}", BodyStyle = WebMessageBodyStyle.Bare)]
        public Stream GetFile(string id)
        {
            Stream stream = null;
            try
            {
                string fileName = Path.GetFileName(id);
                OutgoingWebResponseContext context = WebOperationContext.Current.OutgoingResponse;
                context.Headers.Add(System.Net.HttpResponseHeader.CacheControl, "public");
                context.Headers.Add("content-disposition", "filename=" + fileName);
                context.StatusCode = System.Net.HttpStatusCode.OK;

                // Only for Excel and Word documents, ContentType will be set. For others, ContentType will be default.
                string contentType = GetFileContentType(fileName);
                if (!string.IsNullOrWhiteSpace(contentType))
                {
                    context.ContentType = contentType;
                }

                stream = communityRepositoryInstance.GetFile(id);
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
        /// <returns>Stream for the tile image for the specified level, x and y axis.</returns>
        [WebGet(UriTemplate = "/Tile?Id={id}&level={level}&x={x}&y={y}", BodyStyle = WebMessageBodyStyle.Bare)]
        public Stream GetTile(string id, int level, int x, int y)
        {
            Stream stream = null;
            try
            {
                OutgoingWebResponseContext context = WebOperationContext.Current.OutgoingResponse;
                context.Headers.Add(System.Net.HttpResponseHeader.CacheControl, "public");
                context.ContentType = "image/jpeg";
                context.StatusCode = System.Net.HttpStatusCode.OK;

                stream = communityRepositoryInstance.GetTile(id, level, x, y);
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

                stream = communityRepositoryInstance.GetDem(id, level, x, y);
            }
            catch (FaultException)
            {
                throw;
            }
            return stream;
        }

        /// <summary>
        /// Gets the application path where the service is hosted. Community service sample might have deployed in 
        /// root directory or under any other virtual directory.
        /// </summary>
        /// <param name="appPathUri">WCF app path Uri</param>
        /// <returns>Application path of the service.</returns>
        private static string GetApplicationPath(Uri appPathUri)
        {
            string resourcesPath = string.Empty;

            if (appPathUri != null)
            {
                // Remove "CommunityResources" from URL.
                resourcesPath = appPathUri.AbsoluteUri.Remove(appPathUri.AbsoluteUri.LastIndexOf(appPathUri.Segments[appPathUri.Segments.Length - 1], StringComparison.OrdinalIgnoreCase));
                
                // Remove "CommunityService.svc" from URL.
                resourcesPath = resourcesPath.Remove(appPathUri.AbsoluteUri.LastIndexOf(appPathUri.Segments[appPathUri.Segments.Length - 2], StringComparison.OrdinalIgnoreCase));
                
                // Remove "Service" from the URL.
                resourcesPath = resourcesPath.Remove(appPathUri.AbsoluteUri.LastIndexOf(appPathUri.Segments[appPathUri.Segments.Length - 3], StringComparison.OrdinalIgnoreCase));
            }

            return resourcesPath;
        }

        /// <summary>
        /// Gets the content type for the file based on its extension. Only supported files content type will be returned.
        /// For other files, content type will be empty.
        /// </summary>
        /// <param name="fileName">Name of the file</param>
        /// <returns>ContentType of the file</returns>
        private static string GetFileContentType(string fileName)
        {
            string contentType = string.Empty;

            switch (Path.GetExtension(fileName).ToUpper(CultureInfo.InvariantCulture))
            {
                case ".DOCX":
                    // Word doc content type for open XML format.
                    contentType = CommunityService.ContentTypeDocx;
                    break;

                case ".XLSX":
                    // Excel doc content type for open XML format.
                    contentType = CommunityService.ContentTypeXlsx;
                    break;

                case ".PPTX":
                    // PowerPoint doc content type.
                    contentType = CommunityService.ContentTypePptx;
                    break;
            }

            return contentType;
        }
    }
}