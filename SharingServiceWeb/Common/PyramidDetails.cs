//-----------------------------------------------------------------------
// <copyright file="PyramidDetails.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation 2011. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Collections.ObjectModel;
using System.Runtime.Serialization;

namespace Microsoft.Research.Wwt.SharingService.Web
{
    /// <summary>
    /// Pyramid details for the service. 
    /// </summary>
    [DataContract(Namespace = "")]
    public class PyramidDetails
    {
        /// <summary>
        /// Gets Pyramid details collection
        /// </summary>
        [DataMember]
        public Collection<Pyramid> Pyramids { get; internal set; }

        /// <summary>
        /// Gets or sets the Name.
        /// </summary>
        [DataMember]
        public string Location { get; set; }
    }
}