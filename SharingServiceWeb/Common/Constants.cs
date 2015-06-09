//-----------------------------------------------------------------------
// <copyright file="Constants.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation 2011. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Microsoft.Research.Wwt.SharingService.Web
{
    /// <summary>
    /// Class which defines the constants.
    /// </summary>
    public static class Constants
    {
        /// <summary>
        /// Output directory path.
        /// </summary>
        public const string TileServiceInputDirectory = "TileServiceInputDirectory";

        /// <summary>
        /// Community input directory path.
        /// </summary>
        public const string CommunityServiceInputDirectory = "CommunityServiceInputDirectory";

        /// <summary>
        /// Days to be considered for latest files in Community service.
        /// </summary>
        public const string CommunityServiceLatestFileDays = "CommunityServiceLatestFileDays";

        /// <summary>
        /// WTML files search pattern.
        /// </summary>
        public const string WTMLSearchPattern = "*.wtml";

        /// <summary>
        /// WTML extension.
        /// </summary>
        public const string WTMLExtension = ".wtml";

        /// <summary>
        /// WTML extension.
        /// </summary>
        public const string TourExtension = ".wtt";

        /// <summary>
        /// WTML extension.
        /// </summary>
        public const string TextExtension = ".txt";

        /// <summary>
        /// WTML extension.
        /// </summary>
        public const string WTMLFile = "{0}.wtml";

        /// <summary>
        /// Tile image path.
        /// </summary>
        public const string TileImagePath = @"Pyramid\{0}\{1}\L{0}X{1}Y{2}.{3}";

        /// <summary>
        /// Dem tile path
        /// </summary>
        public const string DemTilePath = @"Pyramid\{0}\{1}\DL{0}X{1}Y{2}.{3}";

        /// <summary>
        /// Thumbnail image.
        /// </summary>
        public const string ThumbNailImagePath = @"{0}.{1}";

        /// <summary>
        /// Elevation model path.
        /// </summary>
        public const string WTMLElevationModelAttribute = "ElevationModel";

        /// <summary>
        /// WTML name.
        /// </summary>
        public const string WTMLName = "Name";

        /// <summary>
        /// WTML tile levels.
        /// </summary>
        public const string WTMLTileLevel = "TileLevels";

        /// <summary>
        /// WTML Projection type.
        /// </summary>
        public const string WTMLProjection = "Projection";

        /// <summary>
        /// Image set path.
        /// </summary>
        public const string ImageSetPath = "//ImageSet";

        /// <summary>
        /// Place node path.
        /// </summary>
        public const string PlacePath = "//Place";

        /// <summary>
        /// Place Thumbnail attribute in WTML file.
        /// </summary>
        public const string PlaceThumbnailAttribute = "Thumbnail";

        /// <summary>
        /// ModifiedDate attribute in temporary payload file.
        /// </summary>
        public const string ModifiedDateAttribute = "ModifiedDate";

        /// <summary>
        /// XPath for items with ModifiedDate attribute.
        /// </summary>
        public const string ItemsWithModifiedDateAttributePath = "//*[@ModifiedDate]";

        /// <summary>
        /// XPath for the Latest folder in the payload XML.
        /// </summary>
        public const string LatestFolderPath = "Folder/Folder[@Name='Latest']";

        /// <summary>
        /// WTML Place name.
        /// </summary>
        public const string PlaceName = "Name";

        /// <summary>
        /// XPath for finding nodes with URL attribute.
        /// </summary>
        public const string NodesWithUrlAttributeXPath = "//*[@Url or @DemUrl]";

        /// <summary>
        /// ImageSet node name.
        /// </summary>
        public const string ImageSetNodeName = "ImageSet";

        /// <summary>
        /// Thumbnail path for through the service.
        /// </summary>
        public const string ThumbnailServicePath = @"/Service/TileService.svc/Resources/Thumbnail?Id={0}&name={1}";

        /// <summary>
        /// Tile sharing service path.
        /// </summary>
        public const string TileSharingServicePath = @"/Service/TileService.svc/Resources/Tile?Id={0}&level={1}&x={2}&y={3}";

        /// <summary>
        /// WTML service path. 
        /// </summary>
        public const string WTMLServicePath = @"/Service/TileService.svc/Resources/Wtml?id={0}&name={1}";

        /// <summary>
        /// URL attribute in WTML file.
        /// </summary>
        public const string UrlAttribute = "Url";

        /// <summary>
        /// WTML service URL.
        /// </summary>
        public const string WTMLServiceUrl = "/Tile?Id={0}";

        /// <summary>
        /// DEM service URL.
        /// </summary>
        public const string DEMServiceUrl = "/Dem?Id={0}";

        /// <summary>
        /// Thumbnail service URL.
        /// </summary>
        public const string ThumbnailServiceUrl = "/Thumbnail?Id={0}&name={1}";

        /// <summary>
        /// WTML tile path.
        /// </summary>
        public const string WtmlFilepath = @"&level={1}&x={2}&y={3}";

        /// <summary>
        /// WTML tile path.
        /// </summary>
        public const string DemFilepath = @"&level={0}&x={1}&y={2}";

        /// <summary>
        /// DEM extension.
        /// </summary>
        public const string DEMUrl = "DemUrl";

        /// <summary>
        /// Credits attribute
        /// </summary>
        public const string Credits = "Credits";

        /// <summary>
        /// Credit URL attribute
        /// </summary>
        public const string CreditURL = "CreditsUrl";
        
        /// <summary>
        /// Default DEM URL.
        /// </summary>
        public const string DefaultDEMUrl = "http://(web server address)";

        /// <summary>
        /// Thumbnail URL.
        /// </summary>
        public const string ThumbnailUrl = "ThumbnailUrl";

        /// <summary>
        /// Default thumbnail image
        /// </summary>
        public const string DefaultImage = @"Resources\thumbnail.jpeg";

        /// <summary>
        /// Default community thumbnail image
        /// </summary>
        public const string DefaultCommunityThumbnail = @"Resources\DefaultCommunityThumbnail.png";

        /// <summary>
        /// Default Tour thumbnail image
        /// </summary>
        public const string DefaultTourThumbnail = @"Resources\DefaultTourThumbnail.png";

        /// <summary>
        /// Default WTML thumbnail image
        /// </summary>
        public const string DefaultWtmlThumbnail = @"Resources\DefaultWtmlThumbnail.png";

        /// <summary>
        /// Default Link thumbnail image
        /// </summary>
        public const string DefaultLinkThumbnail = @"Resources\DefaultLinkThumbnail.png";

        /// <summary>
        /// Default File thumbnail image
        /// </summary>
        public const string DefaultFileThumbnail = @"Resources\DefaultFileThumbnail.png";

        /// <summary>
        /// Default Excel thumbnail image
        /// </summary>
        public const string DefaultExcelThumbnail = @"Resources\DefaultExcelThumbnail.png";

        /// <summary>
        /// DEM extension.
        /// </summary>
        public const string DemExtension = "dem";

        /// <summary>
        /// Pyramid folder name
        /// </summary>
        public const string PyramidFolder = "Pyramid";

        /// <summary>
        /// Plate file search pattern.
        /// </summary>
        public const string PlateFileSearchPattern = "*.plate";

        /// <summary>
        /// Multiple plates folder name
        /// </summary>
        public const string PlatesFolder = "Plates";

        /// <summary>
        /// Multiple DEM plates folder name
        /// </summary>
        public const string DEMPlatesFolder = "DEMPlates";

        /// <summary>
        /// All Tours folder name
        /// </summary>
        public const string AllToursFolder = "All Tours";

        /// <summary>
        /// Latest folder name
        /// </summary>
        public const string LatestFolder = "Latest";

        /// <summary>
        /// Plate file extension.
        /// </summary>
        public const string PlateFileExtension = ".plate";

        /// <summary>
        /// Description file name.
        /// </summary>
        public const string DescriptionFileName = "desc.txt";

        /// <summary>
        /// Top level pyramid plate.
        /// </summary>
        public const string TopLevelPlate = "L0X0Y0.plate";

        /// <summary>
        /// Base level pyramid plate format.
        /// </summary>
        public const string BaseLevelPlateFormat = "L{0}X{1}Y{2}.plate";

        /// <summary>
        /// Top level DEM plate.
        /// </summary>
        public const string DEMTopLevelPlate = "DL0X0Y0.plate";

        /// <summary>
        /// Base level DEM plate format.
        /// </summary>
        public const string DEMBaseLevelPlateFormat = "DL{0}X{1}Y{2}.plate";

        /// <summary>
        /// Thumbnail search pattern.
        /// </summary>
        public const string ThumbnailSearchPattern = "Thumbnail.*";

        /// <summary>
        /// Pay load file URL format.
        /// </summary>
        public const string PayloadServicePath = "{0}/payload?CommunityId={1}";

        /// <summary>
        /// File URL format.
        /// </summary>
        public const string FileServicePath = "{0}/file?id={1}";

        /// <summary>
        /// Sign up file name format.
        /// </summary>
        public const string SignUpFileNameFormat = "{0}_Signup.wtml";

        /// <summary>
        /// Signup path served through the service.
        /// </summary>
        public const string SignupServicePath = @"{0}/Signup?CommunityId={1}";

        /// <summary>
        /// Path format for WCF client of community service.
        /// </summary>
        public const string CommunityServiceWcfClientPathFormat = "{0}://{1}:{2}{3}/Service/CommunityService.svc";

        /// <summary>
        /// Path format for WCF client of Tile service.
        /// </summary>
        public const string TileServiceWcfClientPathFormat = "{0}://{1}:{2}{3}/Service/TileService.svc";

        /// <summary>
        /// Path for format for wtml path
        /// </summary>
        public const string TileServicePathFormat = "{0}://{1}:{2}{3}";

        /// <summary>
        /// Ascending order style
        /// </summary>
        public const string SortAscendingStyle = "sortAscending-header";

        /// <summary>
        /// Descending order style
        /// </summary>
        public const string SortDescendingStyle = "sortDescending-header";

        /// <summary>
        /// Column header style
        /// </summary>
        public const string ColumnDefaultStyle = "columnHeader";
    }
}
