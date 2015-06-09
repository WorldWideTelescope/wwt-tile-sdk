//-----------------------------------------------------------------------
// <copyright file="Community.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation 2011. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Globalization;
using System.IO;
using System.Runtime.Serialization;

namespace Microsoft.Research.Wwt.SharingService.Web
{
    /// <summary>
    /// Class representing the community to hold all the properties of a community.
    /// </summary>
    [DataContract(Namespace = "")]
    public class Community
    {
        /// <summary>
        /// Gets or sets the community id.
        /// </summary>
        /// <remarks>Id will be the name of the community folder which will also be the community name.</remarks>
        [DataMember]
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets Sign Up File.
        /// </summary>
        [DataMember]
        public string SignUpFile { get; set; }

        /// <summary>
        /// Gets or sets the description about the community.
        /// </summary>
        [DataMember]
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the thumbnail file for the community.
        /// </summary>
        [DataMember]
        public string Thumbnail { get; set; }

        /// <summary>
        /// Rewrites the Thumbnail path and the URL the community.
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

            SignUpFile = Path.Combine(string.Format(CultureInfo.InvariantCulture, Constants.SignupServicePath, serviceUrl, communityId));
        }
    }
}