//-----------------------------------------------------------------------
// <copyright file="CommunityDetails.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation 2011. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Collections.ObjectModel;
using System.Runtime.Serialization;

namespace Microsoft.Research.Wwt.SharingService.Web
{
    /// <summary>
    /// Class representing the community details for the service.
    /// </summary>
    [DataContract(Namespace = "")]
    public class CommunityDetails
    {
        /// <summary>
        /// Gets community details collection
        /// </summary>
        [DataMember]
        public Collection<Community> Communities { get; internal set; }

        /// <summary>
        /// Gets or sets the Location from where communities are picked from.
        /// </summary>
        [DataMember]
        public string Location { get; set; }
    }
}