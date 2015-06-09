//-----------------------------------------------------------------------
// <copyright file="ITileRepository.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation 2011. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Collections.ObjectModel;
using System.IO;
using System.Xml.XPath;

namespace Microsoft.Research.Wwt.SharingService.Web
{
    /// <summary>
    /// Interface is used to implement local and azure repositories.
    /// </summary>
    public interface ITileRepository
    {
        /// <summary>
        /// Gets or sets pyramid location
        /// </summary>
        /// <returns>Pyramid location</returns>
        string PyramidLocation { get; set; }

        /// <summary>
        /// Gets Pyramid data for the sharing service. 
        /// </summary>
        /// <returns>Details of all pyramids</returns>
        Collection<Pyramid> GetPyramidDetails();

        /// <summary>
        /// Gets tile image for the specified Tile id.
        /// </summary>
        /// <param name="id">Id for the tile.</param>
        /// <param name="level">Level of the image.</param>
        /// <param name="x">X axis image.</param>
        /// <param name="y">Y axis image.</param>
        /// <returns>Tile image for the specified image.</returns>
        Stream GetTileImage(string id, int level, int x, int y);

        /// <summary>
        /// Gets Dem for the specified Tile id.
        /// </summary>
        /// <param name="id">Id for the tile.</param>
        /// <param name="level">Level of the image.</param>
        /// <param name="x">X axis image.</param>
        /// <param name="y">Y axis image.</param>
        /// <returns>Dem for the specified image.</returns>
        Stream GetDem(string id, int level, int x, int y);

        /// <summary>
        /// Gets thumbnail image for the Pyramid.
        /// </summary>
        /// <param name="id">Id for the pyramid.</param>
        /// <param name="name">WTML Name of the Pyramid.</param>
        /// <returns>Image in the stream.</returns>
        Stream GetThumbnailImage(string id, string name);

        /// <summary>
        /// Get WTML file for the Pyramid.
        /// </summary>
        /// <param name="id">Id for the pyramid.</param>
        /// <param name="name">Name of the WTML file.</param>
        /// <returns>WTML file for the image.</returns>
        IXPathNavigable GetWtmlFile(string id, string name);
    }
}
