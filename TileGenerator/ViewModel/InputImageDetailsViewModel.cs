//-----------------------------------------------------------------------
// <copyright file="InputImageDetailsViewModel.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation 2011. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Security;
using System.Threading.Tasks;

namespace Microsoft.Research.Wwt.TileGenerator
{
    /// <summary>
    /// View model for input image details.
    /// </summary>
    public class InputImageDetailsViewModel : PropertyChangeBase
    {
        #region Private Properties

        private string inputImagePath;
        private string topLeftLatitude;
        private string topLeftLongitude;
        private string bottomRightLatitude;
        private string bottomRightLongitude;
        private string errorMessage;
        private string imageDimension;

        private bool isErrorWindowVisible;
        private bool isInputDetailsBusy;
        private bool isMainWindowEnabled;

        private ObservableCollection<InputProjections> inputProjectionTypes;

        private InputProjections selectedInputProjectionType;

        private string invalidTopLeftLatitudeErrorMessage;
        private string invalidTopLeftLongitudeErrorMessage;
        private string invalidFolderPathErrorMessage;
        private string invalidBottomRightLatitudeErrorMessage;
        private string invalidBottomRightLongitudeErrorMessage;
        private string invalidBoundaryLatValErrorMessage;
        private string invalidBoundaryLongValErrorMessage;
        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the InputImageDetailsViewModel class
        /// </summary>
        public InputImageDetailsViewModel()
        {
            this.PopulateInputProjectionTypes();
            this.PopulateDefaultCoordinates();
            this.IsInputDetailsBusy = false;
            this.IsMainWindowEnabled = true;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets a value indicating whether the input details window is busy or not
        /// </summary>
        public bool IsInputDetailsBusy
        {
            get
            {
                return this.isInputDetailsBusy;
            }
            set
            {
                this.isInputDetailsBusy = value;
                OnPropertyChanged("IsInputDetailsBusy");
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the error window is visible
        /// </summary>
        public bool IsErrorWindowVisible
        {
            get
            {
                return this.isErrorWindowVisible;
            }
            set
            {
                this.isErrorWindowVisible = value;
                OnPropertyChanged("IsErrorWindowVisible");
            }
        }

        /// <summary>
        /// Gets or sets the input image path.
        /// </summary>
        public string InputImagePath
        {
            get
            {
                return this.inputImagePath;
            }
            set
            {
                if (value != null)
                {
                    Task.Factory.StartNew(() =>
                        {
                            this.IsInputDetailsBusy = true;
                            ValidateInputImagePath(value.Trim());
                            PopulateDefaultCoordinates();
                            this.inputImagePath = value.Trim();
                            OnPropertyChanged("InputImagePath");
                            this.IsInputDetailsBusy = false;
                        });
                }
            }
        }

        /// <summary>
        /// Gets or sets image name.
        /// </summary>
        public string ImageName
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets image dimension.
        /// </summary>
        public string ImageDimension
        {
            get
            {
                return this.imageDimension;
            }
            set
            {
                this.imageDimension = value;
                OnPropertyChanged("ImageDimension");
            }
        }

        /// <summary>
        /// Gets or sets top left latitude.
        /// </summary>
        public string TopLeftLatitude
        {
            get
            {
                return this.topLeftLatitude;
            }
            set
            {
                if (value != null)
                {
                    ValidateLatitude(value.Trim(), Position.TopLeft);
                    this.topLeftLatitude = value.Trim();
                    OnPropertyChanged("TopLeftLatitude");
                }
            }
        }

        /// <summary>
        /// Gets or sets top left longitude. 
        /// </summary>
        public string TopLeftLongitude
        {
            get
            {
                return this.topLeftLongitude;
            }
            set
            {
                if (value != null)
                {
                    ValidateLongitude(value.Trim(), Position.TopLeft);
                    this.topLeftLongitude = value.Trim();
                    OnPropertyChanged("TopLeftLongitude");
                }
            }
        }

        /// <summary>
        /// Gets or sets bottom right latitude.
        /// </summary>
        public string BottomRightLatitude
        {
            get
            {
                return this.bottomRightLatitude;
            }
            set
            {
                if (value != null)
                {
                    ValidateLatitude(value.Trim(), Position.BottomRight);
                    this.bottomRightLatitude = value;
                    OnPropertyChanged("BottomRightLatitude");
                }
            }
        }

        /// <summary>
        /// Gets or sets bottom right longitude.
        /// </summary>
        public string BottomRightLongitude
        {
            get
            {
                return this.bottomRightLongitude;
            }
            set
            {
                if (value != null)
                {
                    ValidateLongitude(value.Trim(), Position.BottomRight);
                    this.bottomRightLongitude = value;
                    OnPropertyChanged("BottomRightLongitude");
                }
            }
        }

        /// <summary>
        /// Gets or sets error message.
        /// </summary>
        public string ErrorMessage
        {
            get
            {
                return this.errorMessage;
            }
            set
            {
                this.errorMessage = value;
                OnPropertyChanged("ErrorMessage");
            }
        }

        /// <summary>
        /// Gets or sets the selected projection type
        /// </summary>
        public InputProjections SelectedInputProjectionType
        {
            get
            {
                return selectedInputProjectionType;
            }
            set
            {
                selectedInputProjectionType = value;
                OnPropertyChanged("SelectedInputProjectionType");
            }
        }

        /// <summary>
        /// Gets input projection types.
        /// </summary>
        public ObservableCollection<InputProjections> InputProjectionTypes
        {
            get
            {
                return this.inputProjectionTypes;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the main window is disabled/enabled.
        /// </summary>
        public bool IsMainWindowEnabled
        {
            get
            {
                return this.isMainWindowEnabled;
            }
            set
            {
                this.isMainWindowEnabled = value;
                OnPropertyChanged("IsMainWindowEnabled");
            }
        }

        #region Error Message

        /// <summary>
        /// Gets or sets error message for invalid folder path.
        /// </summary>
        public string InvalidFolderPathErrorMessage
        {
            get
            {
                return this.invalidFolderPathErrorMessage;
            }
            set
            {
                this.invalidFolderPathErrorMessage = value;
                OnPropertyChanged("InvalidFolderPathErrorMessage");
            }
        }

        /// <summary>
        /// Gets or sets error message for invalid top left latitude.
        /// </summary>
        public string InvalidTopLeftLatitudeErrorMessage
        {
            get
            {
                return this.invalidTopLeftLatitudeErrorMessage;
            }
            set
            {
                this.invalidTopLeftLatitudeErrorMessage = value;
                OnPropertyChanged("InvalidTopLeftLatitudeErrorMessage");
            }
        }

        /// <summary>
        /// Gets or sets error message for invalid longitude.
        /// </summary>
        public string InvalidTopLeftLongitudeErrorMessage
        {
            get
            {
                return this.invalidTopLeftLongitudeErrorMessage;
            }
            set
            {
                this.invalidTopLeftLongitudeErrorMessage = value;
                OnPropertyChanged("InvalidTopLeftLongitudeErrorMessage");
            }
        }

        /// <summary>
        /// Gets or sets error message for invalid bottom right latitude.
        /// </summary>
        public string InvalidBottomRightLatitudeErrorMessage
        {
            get
            {
                return this.invalidBottomRightLatitudeErrorMessage;
            }
            set
            {
                this.invalidBottomRightLatitudeErrorMessage = value;
                OnPropertyChanged("InvalidBottomRightLatitudeErrorMessage");
            }
        }

        /// <summary>
        /// Gets or sets error message for invalid bottom right  longitude.
        /// </summary>
        public string InvalidBottomRightLongitudeErrorMessage
        {
            get
            {
                return this.invalidBottomRightLongitudeErrorMessage;
            }
            set
            {
                this.invalidBottomRightLongitudeErrorMessage = value;
                OnPropertyChanged("InvalidBottomRightLongitudeErrorMessage");
            }
        }

        /// <summary>
        /// Gets or sets error message for invalid latitude boundary values.
        /// </summary>
        public string InvalidBoundaryLatValErrorMessage
        {
            get
            {
                return this.invalidBoundaryLatValErrorMessage;
            }
            set
            {
                this.invalidBoundaryLatValErrorMessage = value;
                OnPropertyChanged("InvalidBoundaryLatValErrorMessage");
            }
        }

        /// <summary>
        /// Gets or sets error message for invalid longitude boundary values.
        /// </summary>
        public string InvalidBoundaryLongValErrorMessage
        {
            get
            {
                return this.invalidBoundaryLongValErrorMessage;
            }
            set
            {
                this.invalidBoundaryLongValErrorMessage = value;
                OnPropertyChanged("InvalidBoundaryLongValErrorMessage");
            }
        }

        #endregion

        #endregion

        #region Public Methods

        /// <summary>
        /// Validates if latitude is greater than -90
        /// and less than +90
        /// </summary>
        /// <param name="latitude">Latitude (top left/bottom right)</param>
        /// <param name="position">Top left/Bottom Right</param>
        /// <returns>True if the latitude is between -90 and +90</returns>
        public bool ValidateLatitude(string latitude, Position position)
        {
            double latitudeVal;
            bool isValid = true;
            string invalidLaterrorMessage;
            if (string.IsNullOrEmpty(latitude))
            {
                isValid = false;
                invalidLaterrorMessage = Properties.Resources.MandatoryError;
            }
            else if (!Double.TryParse(latitude, out latitudeVal))
            {
                isValid = false;
                invalidLaterrorMessage = Properties.Resources.InvalidDataError;
            }
            else if (!(latitudeVal >= Constants.LatitudeMinValue && latitudeVal <= Constants.LatitudeMaxValue))
            {
                isValid = false;
                invalidLaterrorMessage = Properties.Resources.InvalidLatitudeError;
            }
            else
            {
                invalidLaterrorMessage = string.Empty;
            }

            // The error message is assigned to the proper label.
            switch (position)
            {
                case Position.TopLeft:
                    this.InvalidTopLeftLatitudeErrorMessage = invalidLaterrorMessage;
                    break;
                case Position.BottomRight:
                    this.InvalidBottomRightLatitudeErrorMessage = invalidLaterrorMessage;
                    break;
            }
            this.InvalidBoundaryLongValErrorMessage = string.Empty;
            this.InvalidBoundaryLatValErrorMessage = string.Empty;
            return isValid;
        }

        /// <summary>
        /// Validates if latitude is greater than -180
        /// and less than +180
        /// </summary>
        /// <param name="longitude">Longitude (top left/bottom right)</param>
        /// <param name="position">top left/bottom right</param>
        /// <returns>True if the longitude is between -180 and +180</returns>
        public bool ValidateLongitude(string longitude, Position position)
        {
            double longitudeVal;
            bool isValid = true;
            string invalidLongerrorMessage;
            if (string.IsNullOrEmpty(longitude))
            {
                isValid = false;
                invalidLongerrorMessage = Properties.Resources.MandatoryError;
            }
            else if (!Double.TryParse(longitude, out longitudeVal))
            {
                isValid = false;
                invalidLongerrorMessage = Properties.Resources.InvalidDataError;
            }
            else if (!(longitudeVal >= Constants.LongitudeMinValue && longitudeVal <= Constants.LongitudeMaxValue))
            {
                isValid = false;
                invalidLongerrorMessage = Properties.Resources.InvalidLongitudeError;
            }
            else
            {
                invalidLongerrorMessage = string.Empty;
            }

            // The error message is assigned to the proper label.
            switch (position)
            {
                case Position.TopLeft:
                    this.InvalidTopLeftLongitudeErrorMessage = invalidLongerrorMessage;
                    break;
                case Position.BottomRight:
                    this.InvalidBottomRightLongitudeErrorMessage = invalidLongerrorMessage;
                    break;
            }
            this.InvalidBoundaryLongValErrorMessage = string.Empty;
            this.InvalidBoundaryLatValErrorMessage = string.Empty;
            return isValid;
        }

        /// <summary>
        /// Validates boundary conditions for top left and bottom right latitude 
        /// </summary>
        /// <param name="topLeft">Top left (latitude)</param>
        /// <param name="bottomRight">Bottom Right (latitude)</param>
        /// <returns>True if the given values are valid and not same.</returns>
        public bool ValidateLatBoundaryValues(string topLeft, string bottomRight)
        {
            double topLeftVal, bottomRightVal;
            bool isValid = true;
            if (!string.IsNullOrEmpty(topLeft) && !string.IsNullOrEmpty(bottomRight) && Double.TryParse(topLeft, out topLeftVal)
                && Double.TryParse(bottomRight, out bottomRightVal) && topLeftVal == bottomRightVal)
            {
                this.InvalidBoundaryLatValErrorMessage = Properties.Resources.InvalidBoundaryLatValError;
                isValid = false;
            }
            else
            {
                this.InvalidBoundaryLatValErrorMessage = string.Empty;
            }
            return isValid;
        }

        /// <summary>
        /// Validates boundary conditions for top left and bottom right  
        /// longitude
        /// </summary>
        /// <param name="topLeft">Top left (longitude)</param>
        /// <param name="bottomRight">Bottom Right (longitude)</param>
        /// <returns>True if the given values are valid and not same.</returns>
        public bool ValidateLongBoundaryValues(string topLeft, string bottomRight)
        {
            double topLeftVal, bottomRightVal;
            bool isValid = true;
            if (!string.IsNullOrEmpty(topLeft) && !string.IsNullOrEmpty(bottomRight) && Double.TryParse(topLeft, out topLeftVal)
                && Double.TryParse(bottomRight, out bottomRightVal) && topLeftVal == bottomRightVal)
            {
                this.InvalidBoundaryLongValErrorMessage = Properties.Resources.InvalidBoundaryLongValError;
                isValid = false;
            }
            else
            {
                this.InvalidBoundaryLongValErrorMessage = string.Empty;
            }
            return isValid;
        }

        /// <summary>
        /// Validates the input image path.
        /// </summary>
        /// <param name="imagePath">Image path given by the user.</param>
        /// <returns>True if the image path is valid.</returns>
        public bool ValidateInputImagePath(string imagePath)
        {
            if (!string.IsNullOrEmpty(imagePath))
            {
                try
                {
                    string folderPath = Path.GetDirectoryName(imagePath);
                    DirectoryInfo directory = new DirectoryInfo(folderPath);
                    if (directory.Exists)
                    {
                        if (IsValidImage(imagePath) && SetImageDimension(imagePath))
                        {
                            this.InvalidFolderPathErrorMessage = string.Empty;
                            return true;
                        }
                    }
                    else
                    {
                        this.InvalidFolderPathErrorMessage = Properties.Resources.InvalidInputImagePath;
                    }
                }
                catch (PathTooLongException ex)
                {
                    this.ErrorMessage = Properties.Resources.PathTooLongErrorMessage;
                    this.IsErrorWindowVisible = true;
                    this.IsMainWindowEnabled = false;
                    Logger.LogException(ex);
                }
                catch (SecurityException ex)
                {
                    this.ErrorMessage = Properties.Resources.SecurityErrorMessage;
                    this.IsErrorWindowVisible = true;
                    this.IsMainWindowEnabled = false;
                    Logger.LogException(ex);
                }
                catch (ArgumentException ex)
                {
                    this.ErrorMessage = Properties.Resources.ArgumentErrorMessage;
                    this.IsErrorWindowVisible = true;
                    this.IsMainWindowEnabled = false;
                    Logger.LogException(ex);
                }
            }
            else
            {
                this.InvalidFolderPathErrorMessage = Properties.Resources.MandatoryError;
            }
            return false;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Populates valid image formats
        /// </summary>
        /// <returns>Collection of string which have valid image formats.</returns>
        private static Collection<string> PopulateImageFormats()
        {
            Collection<string> imageFomats = new Collection<string>();
            imageFomats.Add(ImageFormat.Jpeg.ToString().ToUpperInvariant());
            imageFomats.Add(Constants.JpgImageFormat.ToUpperInvariant());
            imageFomats.Add(Constants.TifImageFormat.ToUpperInvariant());
            imageFomats.Add(ImageFormat.Tiff.ToString().ToUpperInvariant());
            imageFomats.Add(ImageFormat.Png.ToString().ToUpperInvariant());
            return imageFomats;
        }

        /// <summary>
        /// Populates input projection types in alphabetical order.
        /// </summary>
        private void PopulateInputProjectionTypes()
        {
            this.inputProjectionTypes = new ObservableCollection<InputProjections>();
            this.inputProjectionTypes.Add(InputProjections.EquiRectangular);
            this.inputProjectionTypes.Add(InputProjections.Mercator);

            this.SelectedInputProjectionType = this.inputProjectionTypes[0];
        }

        /// <summary>
        /// Validates if the given path has valid image.
        /// </summary>
        /// <param name="filePath">Given file path.</param>
        /// <returns>Validates if the image is valid or not.</returns>
        private bool IsValidImage(string filePath)
        {
            try
            {
                string imageExtension = Path.GetExtension(filePath).Substring(1);
                Collection<string> imageFomats = PopulateImageFormats();
                if (imageFomats != null && imageFomats.Count > 0)
                {
                    if (imageFomats.Contains(imageExtension.ToUpperInvariant()))
                    {
                        this.InvalidFolderPathErrorMessage = string.Empty;
                        return true;
                    }
                    else
                    {
                        this.InvalidFolderPathErrorMessage = Properties.Resources.InvalidImageFormat;
                    }
                }
            }
            catch (ArgumentException ex)
            {
                this.InvalidFolderPathErrorMessage = Properties.Resources.ArgumentErrorMessage;
                Logger.LogException(ex);
            }
            return false;
        }

        /// <summary>
        /// Sets the image dimension
        /// </summary>
        /// <returns>True if the image is valid and the dimension was set</returns>
        private bool SetImageDimension(string imagePath)
        {
            try
            {
                using (Bitmap image = new Bitmap(imagePath))
                {
                    if (image != null)
                    {
                        this.ImageDimension = string.Format(CultureInfo.InvariantCulture, "{0}x{1}", image.Width.ToString(CultureInfo.InvariantCulture), image.Height.ToString(CultureInfo.InvariantCulture));
                        return true;
                    }
                }
            }
            catch (FileNotFoundException ex)
            {
                this.ErrorMessage = Properties.Resources.InputFileNotFoundError;
                this.IsErrorWindowVisible = true;
                this.IsMainWindowEnabled = false;
                Logger.LogException(ex);
            }
            catch (OutOfMemoryException ex)
            {
                this.ErrorMessage = Properties.Resources.OutOfMemoryMessage;
                this.IsErrorWindowVisible = true;
                this.IsMainWindowEnabled = false;
                Logger.LogException(ex);
            }
            catch (ArgumentException ex)
            {
                this.ErrorMessage = Properties.Resources.InvalidInputFile;
                this.IsErrorWindowVisible = true;
                this.IsMainWindowEnabled = false;
                Logger.LogException(ex);
            }
            return false;
        }

        /// <summary>
        /// Populates default coordinates.
        /// </summary>
        private void PopulateDefaultCoordinates()
        {
            this.BottomRightLatitude = Constants.BottomRightLatitude.ToString(CultureInfo.InvariantCulture);
            this.BottomRightLongitude = Constants.BottomRightLongitude.ToString(CultureInfo.InvariantCulture);
            this.TopLeftLatitude = Constants.TopLeftLatitude.ToString(CultureInfo.InvariantCulture);
            this.TopLeftLongitude = Constants.TopLeftLongitude.ToString(CultureInfo.InvariantCulture);
        }
        #endregion
    }
}
