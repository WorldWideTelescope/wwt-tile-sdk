//-----------------------------------------------------------------------
// <copyright file="WtmlCollection.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation 2011. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;

namespace Microsoft.Research.Wwt.Sdk.Core
{
    /// <summary>
    /// Represents the World Wide Telescope Markup Language Collection file.
    /// </summary>
    public class WtmlCollection
    {
        /// <summary>
        /// Initializes a new instance of the WtmlCollection class.
        /// </summary>
        /// <param name="name">
        /// Name of the collection.
        /// </param>
        /// <param name="thumbnailUrl">
        /// Full path of the thumbnail image.
        /// </param>
        /// <param name="textureTilePath">
        /// Full path of texture image tiles in pyramid formatted as specified by WWT.
        /// </param>
        /// <param name="numberOfLevels">
        /// Number of levels of zoom supported by the pyramid.
        /// </param>
        /// <param name="type">
        /// Projection type of imagery.
        /// </param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1054:UriParametersShouldNotBeStrings", MessageId = "1#", Justification = "Not required to use Uri type as this can be local path.")]
        public WtmlCollection(string name, string thumbnailUrl, string textureTilePath, int numberOfLevels, ProjectionTypes type)
            : this(name, thumbnailUrl, textureTilePath, numberOfLevels, type, new Boundary(-180, -90, 180, 90))
        {
        }

        /// <summary>
        /// Initializes a new instance of the WtmlCollection class.
        /// </summary>
        /// <param name="name">
        /// Name of the collection.
        /// </param>
        /// <param name="thumbnailUrl">
        /// Full path of the thumbnail image.
        /// </param>
        /// <param name="textureTilePath">
        /// Full path of texture image tiles in pyramid formatted as specified by WWT.
        /// </param>
        /// <param name="numberOfLevels">
        /// Number of levels of zoom supported by the pyramid.
        /// </param>
        /// <param name="type">
        /// Projection type of imagery.
        /// </param>
        /// <param name="inputBoundary">
        /// Input boundary.
        /// </param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1054:UriParametersShouldNotBeStrings", MessageId = "1#", Justification = "Not required to use Uri type as this can be local path.")]
        public WtmlCollection(string name, string thumbnailUrl, string textureTilePath, int numberOfLevels, ProjectionTypes type, Boundary inputBoundary)
        {
            this.Name = name;
            this.PlaceName = name;
            this.FolderName = name;
            this.TextureTilePath = textureTilePath;
            this.NumberOfLevels = numberOfLevels;
            this.ProjectionType = type;
            this.ThumbnailUrl = thumbnailUrl;

            // Assign default values.
            this.BaseDegreesPerTile = type == ProjectionTypes.Toast ? 90 : 360;
            this.BandPass = BandPasses.Visible;
            this.DataSetType = DataSetTypes.Earth;
            this.DemTilePath = string.Empty;
            this.FileType = ".png";
            this.QuadTreeMap = string.Empty;

            if (inputBoundary != null)
            {
                // Calculate average latitude and longitude.
                this.Latitude = (inputBoundary.Bottom + inputBoundary.Top) / 2;
                this.Longitude = (inputBoundary.Right + inputBoundary.Left) / 2;
            }
        }

        /// <summary>
        /// Gets or sets the name of the collection.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the parent folder name.
        /// </summary>
        public string FolderName { get; set; }

        /// <summary>
        /// Gets or sets the name of place which has to be pointed to.
        /// </summary>
        /// <remarks>
        /// By Default it is same as Name of the WTML imageset.
        /// </remarks>
        public string PlaceName { get; set; }

        /// <summary>
        /// Gets or sets the full path of texture image tiles in pyramid formatted as specified by WWT.
        /// </summary>
        public string TextureTilePath { get; set; }

        /// <summary>
        /// Gets or sets the full path of DEM tiles in pyramid formatted as specified by WWT.
        /// </summary>
        public string DemTilePath { get; set; }

        /// <summary>
        /// Gets or sets the coordinate system used for tiling.
        /// Should be empty if LX/LY system is used.
        /// </summary>
        public string QuadTreeMap { get; set; }

        /// <summary>
        /// Gets or sets the file type of image tiles.
        /// </summary>
        public string FileType { get; set; }

        /// <summary>
        /// Gets or sets the number of levels of zoom supported by the pyramid.
        /// </summary>
        public int NumberOfLevels { get; set; }

        /// <summary>
        /// Gets or sets the index of the first level of tiling.
        /// </summary>
        public int BaseTileLevel { get; set; }

        /// <summary>
        /// Gets or sets the number of degrees occupied by the top tile.
        /// </summary>
        public int BaseDegreesPerTile { get; set; }

        /// <summary>
        /// Gets or sets the X coordinate of the point where the image is centered at.
        /// </summary>
        public double CenterX { get; set; }

        /// <summary>
        /// Gets or sets the Y coordinate of the point where the image is centered at.
        /// </summary>
        public double CenterY { get; set; }

        /// <summary>
        /// Gets or sets the X coordinate of the point from which CenterX is offset by.
        /// </summary>
        public double OffsetX { get; set; }

        /// <summary>
        /// Gets or sets the Y coordinate of the point from which CenterY is offset by.
        /// </summary>
        public double OffsetY { get; set; }

        /// <summary>
        /// Gets or sets the rotation angle of the view camera in degrees.
        /// </summary>
        public double Rotation { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the pyramid has elevation model or not.
        /// </summary>
        public bool IsElevationModel { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the pyramid is sparse or not.
        /// </summary>
        public bool IsSparse { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the pyramid has generic imagery or not.
        /// For foreground images, this should be False.
        /// </summary>
        public bool IsGeneric { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the pyramid is bottoms up or not.
        /// </summary>
        public bool IsBottomsUp { get; set; }

        /// <summary>
        /// Gets or sets the primary wavelength.
        /// </summary>
        public BandPasses BandPass { get; set; }

        /// <summary>
        /// Gets or sets the projection type of imagery.
        /// </summary>
        public ProjectionTypes ProjectionType { get; set; }

        /// <summary>
        /// Gets or sets the type of dataset this pyramid corresponds to.
        /// </summary>
        public DataSetTypes DataSetType { get; set; }

        /// <summary>
        /// Gets or sets the thumbnail URL.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1056:UriPropertiesShouldNotBeStrings", Justification = "Not required to use Uri type as this can be local path.")]
        public string ThumbnailUrl { get; set; }

        /// <summary>
        /// Gets or sets the Credit.
        /// </summary>
        public string Credit { get; set; }

        /// <summary>
        /// Gets or sets the Credit URL.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1056:UriPropertiesShouldNotBeStrings", Justification = "Not required to use Uri type as this can be local path.")]
        public string CreditUrl { get; set; }

        /// <summary>
        /// Gets or sets the Latitude.
        /// </summary>
        /// <remarks>
        /// By Default the value is set to 0 which is center of world.
        /// </remarks>
        public double Latitude { get; set; }

        /// <summary>
        /// Gets or sets the Longitude.
        /// </summary>
        /// <remarks>
        /// By Default the value is set to 0 which is center of world.
        /// </remarks>
        public double Longitude { get; set; }

        /// <summary>
        /// Gets or sets the Zoom level.
        /// </summary>
        /// <remarks>
        /// By Default the value is set to 1.
        /// </remarks>
        public double ZoomLevel { get; set; }

        /// <summary>
        /// Builds the path used to retrieve texture tiles in WTML file.
        /// </summary>
        /// <param name="tilePath">
        /// Path of image pyramid.
        /// </param>
        /// <param name="extension">
        /// Texture tile file extension.
        /// </param>
        /// <returns>
        /// WTML texture tile path.
        /// </returns>
        public static string GetWtmlTextureTilePath(string tilePath, string extension)
        {
            string wtmlTilePath = string.Empty;
            if (!string.IsNullOrEmpty(tilePath))
            {
                extension = string.IsNullOrEmpty(extension) ? Constants.DefaultImageExtension : extension;
                wtmlTilePath = tilePath.Replace("{3}", extension);
                wtmlTilePath = wtmlTilePath.Replace("{2}", "{3}");
                wtmlTilePath = wtmlTilePath.Replace("{1}", "{2}");
                wtmlTilePath = wtmlTilePath.Replace("{0}", "{1}");
            }

            return wtmlTilePath;
        }

        /// <summary>
        /// Serializes the Wtml document in the path specified.
        /// </summary>
        /// <param name="path">
        /// File path where the document should be saved.
        /// </param>
        public void Save(string path)
        {
            // Create the directory if it doesn't exist.
            path = Path.GetFullPath(path);
            var dirInfo = new DirectoryInfo(Path.GetDirectoryName(path));
            if (!dirInfo.Exists)
            {
                dirInfo.Create();
            }

            List<object> placeAttibutes = new List<object>
            {
                new XAttribute("Name", this.PlaceName),
                new XAttribute("DataSetType", this.DataSetType),
                new XAttribute("Lat", this.Latitude),
                new XAttribute("Lng", this.Longitude),
                new XAttribute("ZoomLevel", this.ZoomLevel),
                new XAttribute("Thumbnail", this.ThumbnailUrl)
            };

            XElement placeElement = new XElement(
                "Place",
                placeAttibutes.ToArray(),
                new XElement("BackgroundImageSet", GetImageSetElement()));

            XDocument wtmlDoc = new XDocument(
                new XElement(
                    "Folder",
                    new XAttribute("Name", this.FolderName),
                    placeElement));

            wtmlDoc.Save(path);
        }

        /// <summary>
        /// This function is used to retrieve the imageset element from pyramid details.
        /// </summary>
        /// <returns>
        /// ImageSet element for pyramid.
        /// </returns>
        private XElement GetImageSetElement()
        {
            List<object> wtmlAttributes = new List<object>
            {
                new XAttribute("Generic", this.IsGeneric.ToString()),
                new XAttribute("DataSetType", this.DataSetType),
                new XAttribute("BandPass", this.BandPass),
                new XAttribute("Name", this.Name),
                new XAttribute("BaseTileLevel", this.BaseTileLevel),
                new XAttribute("TileLevels", this.NumberOfLevels),
                new XAttribute("BaseDegreesPerTile", this.BaseDegreesPerTile),
                new XAttribute("FileType", this.FileType),
                new XAttribute("BottomsUp", this.IsBottomsUp.ToString()),
                new XAttribute("Projection", this.ProjectionType),
                new XAttribute("QuadTreeMap", this.QuadTreeMap),
                new XAttribute("CenterX", this.CenterX),
                new XAttribute("CenterY", this.CenterY),
                new XAttribute("OffsetX", this.OffsetX),
                new XAttribute("OffsetY", this.OffsetY),
                new XAttribute("Rotation", this.Rotation),
                new XAttribute("Sparse", this.IsSparse.ToString()),
                new XAttribute("Url", this.TextureTilePath),
                new XAttribute("ElevationModel", this.IsElevationModel.ToString()),
            };

            if (!string.IsNullOrEmpty(this.DemTilePath))
            {
                wtmlAttributes.Add(new XAttribute("DemUrl", this.DemTilePath));
            }

            XElement imageSetElement = new XElement("ImageSet", wtmlAttributes.ToArray());
            XElement thumbnailUrlElement = new XElement("ThumbnailUrl", this.ThumbnailUrl);

            imageSetElement.AddFirst(thumbnailUrlElement);

            // Add credit element.
            if (!string.IsNullOrWhiteSpace(this.Credit))
            {
                imageSetElement.Add(new XElement("Credits", this.Credit));
            }

            // Add credit URL element.
            if (!string.IsNullOrWhiteSpace(this.CreditUrl))
            {
                imageSetElement.Add(new XElement("CreditsUrl", this.CreditUrl));
            }

            return imageSetElement;
        }
    }
}
