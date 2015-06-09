//-----------------------------------------------------------------------
// <copyright file="TileGeneratorViewModel.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation 2011. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Security;
using System.Windows.Forms;
using Microsoft.Research.Wwt.Sdk.Core;

namespace Microsoft.Research.Wwt.TileGenerator
{
    /// <summary>
    /// View model for tile generator details for the wizard.
    /// </summary>
    public sealed class TileGeneratorViewModel : PropertyChangeBase, IDisposable
    {
        #region Private Properties

        private InputImageDetailsViewModel inputImageDetails;

        private string outputFileName;
        private string outputFolderPath;
        private string credits;
        private Uri creditsURL;
        private string progressMessage;
        private string updatedOutputFolderPath;

        private ObservableCollection<ProjectionTypes> outputProjectionTypes;
        private ObservableCollection<string> levels;

        private ProjectionTypes selectedOutputProjectionType;
        private PyramidGenerationSteps currentStep;
        private string selectedLevel;

        private bool isGeneratePlate;
        private bool isStartOverVisible;
        private bool isCloseVisible;
        private bool isCancelVisible;
        private bool isRestartVisible;
        private bool isBackToOutputVisible;
        private bool isCompletedPanelVisible;
        private bool isOutputDetailsScreenVisible;
        private bool isCreateImageScreenVisible;
        private bool isInputDetailsScreenVisible;
        private bool isOutputProjectionEnabled;

        private string errorMessage;
        private string invalidOutputPathErrorMessage;
        private string invalidOutputFileNameErrorMessage;
        private string pyramidGenerationErrorMessage;
        private string totalTimeRemaining;
        private string elapsedTime;

        private string progressBarTag;

        private SDKController sdkController;
        private Stopwatch generatePyramidsStopwatch;
        private Timer generatePyramidsTimer;
        private int guesstimatePlateFileGeneration;
        private long currentStepStartedAt;

        private long totalTiles;
        private long totalPlateTiles;

        #endregion

        #region Constructor

        /// <summary>
        ///  Initializes a new instance of the TileGeneratorViewModel class
        /// </summary>
        public TileGeneratorViewModel()
        {
            Initialize();
        }

        #endregion

        #region Events

        /// <summary>
        /// Application close event.
        /// </summary>
        public event EventHandler Close;

        /// <summary>
        /// Start over the generation with default values for view model
        /// </summary>
        public event EventHandler StartOverClickedEvent;
        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets input image details.
        /// </summary>
        public InputImageDetailsViewModel InputImageDetails
        {
            get
            {
                return this.inputImageDetails;
            }
            set
            {
                this.inputImageDetails = value;
                OnPropertyChanged("InputImageDetails");
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the start over button is visible or not.
        /// </summary>
        public bool IsStartOverVisible
        {
            get
            {
                return this.isStartOverVisible;
            }
            set
            {
                this.isStartOverVisible = value;
                OnPropertyChanged("IsStartOverVisible");
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the close button is visible or not.
        /// </summary>
        public bool IsCloseVisible
        {
            get
            {
                return this.isCloseVisible;
            }
            set
            {
                this.isCloseVisible = value;
                OnPropertyChanged("IsCloseVisible");
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the cancel button is visible or not.
        /// </summary>
        public bool IsCancelVisible
        {
            get
            {
                return this.isCancelVisible;
            }
            set
            {
                this.isCancelVisible = value;
                OnPropertyChanged("IsCancelVisible");
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the restart tiling button is visible or not.
        /// </summary>
        public bool IsRestartVisible
        {
            get
            {
                return this.isRestartVisible;
            }
            set
            {
                this.isRestartVisible = value;
                OnPropertyChanged("IsRestartVisible");
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the back to output button is visible or not.
        /// </summary>
        public bool IsBackToOutputVisible
        {
            get
            {
                return this.isBackToOutputVisible;
            }
            set
            {
                this.isBackToOutputVisible = value;
                OnPropertyChanged("IsBackToOutputVisible");
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the completed panel should be visible or not.
        /// </summary>
        public bool IsCompletedPanelVisible
        {
            get
            {
                return this.isCompletedPanelVisible;
            }
            set
            {
                this.isCompletedPanelVisible = value;
                OnPropertyChanged("IsCompletedPanelVisible");
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the plate file has to be generated or not.
        /// </summary>
        public bool IsGeneratePlate
        {
            get
            {
                return this.isGeneratePlate;
            }
            set
            {
                this.isGeneratePlate = value;
                OnPropertyChanged("IsGeneratePlate");
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the output details screen visible or not.
        /// </summary>
        public bool IsOutputDetailsScreenVisible
        {
            get
            {
                return this.isOutputDetailsScreenVisible;
            }
            set
            {
                this.isOutputDetailsScreenVisible = value;
                OnPropertyChanged("IsOutputDetailsScreenVisible");
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the input details screen visible or not.
        /// </summary>
        public bool IsInputDetailsScreenVisible
        {
            get
            {
                return this.isInputDetailsScreenVisible;
            }
            set
            {
                this.isInputDetailsScreenVisible = value;
                OnPropertyChanged("IsInputDetailsScreenVisible");
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the create image screen is visible or not.
        /// </summary>
        public bool IsCreateImageScreenVisible
        {
            get
            {
                return this.isCreateImageScreenVisible;
            }
            set
            {
                this.isCreateImageScreenVisible = value;
                OnPropertyChanged("IsCreateImageScreenVisible");
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the output projection type is enabled or not.
        /// </summary>
        public bool IsOutputProjectionEnabled
        {
            get
            {
                return this.isOutputProjectionEnabled;
            }
            set
            {
                this.isOutputProjectionEnabled = value;
                OnPropertyChanged("IsOutputProjectionEnabled");
            }
        }

        /// <summary>
        /// Gets or sets output file name.
        /// </summary>
        public string OutputFileName
        {
            get
            {
                return this.outputFileName;
            }
            set
            {
                if (value != null)
                {
                    IsValidFileName(value.Trim());
                    this.outputFileName = value.Trim();
                    OnPropertyChanged("OutputFileName");
                }
            }
        }

        /// <summary>
        /// Gets or sets output folder path.
        /// </summary>
        public string OutputFolderPath
        {
            get
            {
                return this.outputFolderPath;
            }
            set
            {
                if (value != null)
                {
                    IsValidFolder(value.Trim());
                    this.outputFolderPath = value.Trim();
                    OnPropertyChanged("OutputFolderPath");
                }
            }
        }

        /// <summary>
        /// Gets or sets credits.
        /// </summary>
        public string Credits
        {
            get
            {
                return this.credits;
            }
            set
            {
                if (!string.IsNullOrWhiteSpace(value))
                {
                    this.credits = value.Trim();
                    OnPropertyChanged("Credits");
                }
            }
        }

        /// <summary>
        /// Gets or sets message which has to be shown to user.
        /// </summary>
        public string PyramidGenerationMessage
        {
            get
            {
                return this.pyramidGenerationErrorMessage;
            }
            set
            {
                this.pyramidGenerationErrorMessage = value;
                OnPropertyChanged("PyramidGenerationMessage");
            }
        }

        /// <summary>
        /// Gets or sets progress message.
        /// </summary>
        public string ProgressMessage
        {
            get
            {
                return this.progressMessage;
            }
            set
            {
                this.progressMessage = value;
                OnPropertyChanged("ProgressMessage");
            }
        }

        /// <summary>
        /// Gets or sets the Total Time Remaining in seconds.
        /// </summary>
        public string TotalTimeRemaining
        {
            get
            {
                return this.totalTimeRemaining;
            }
            set
            {
                this.totalTimeRemaining = value;
                OnPropertyChanged("TotalTimeRemaining");
            }
        }

        /// <summary>
        /// Gets or sets Elapsed Time.
        /// </summary>
        public string ElapsedTime
        {
            get
            {
                return this.elapsedTime;
            }
            set
            {
                this.elapsedTime = value;
                OnPropertyChanged("ElapsedTime");
            }
        }

        /// <summary>
        /// Gets or sets the progress bar tag
        /// </summary>
        public string ProgressBarTag
        {
            get
            {
                return this.progressBarTag;
            }
            set
            {
                this.progressBarTag = value;
                OnPropertyChanged("ProgressBarTag");
            }
        }

        /// <summary>
        /// Gets or sets Credit URL.
        /// </summary>
        public Uri CreditsURL
        {
            get
            {
                return this.creditsURL;
            }
            set
            {
                if (value != null)
                {
                    this.creditsURL = value;
                    OnPropertyChanged("CreditsURL");
                }
            }
        }

        /// <summary>
        /// Gets or sets selected output projection type
        /// </summary>
        public ProjectionTypes SelectedOutputProjectionType
        {
            get
            {
                return this.selectedOutputProjectionType;
            }
            set
            {
                this.selectedOutputProjectionType = value;
                OnPropertyChanged("SelectedOutputProjectionType");
            }
        }

        /// <summary>
        /// Gets the output projection types.
        /// </summary>
        public ObservableCollection<ProjectionTypes> OutputProjectionTypes
        {
            get
            {
                return this.outputProjectionTypes;
            }
        }

        /// <summary>
        /// Gets or sets selected level
        /// </summary>
        public string SelectedLevel
        {
            get
            {
                return this.selectedLevel;
            }
            set
            {
                this.selectedLevel = value;
                OnPropertyChanged("SelectedLevel");
            }
        }

        /// <summary>
        /// Gets or sets levels for the projection.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly", Justification = "Observable collection for binding in XAML")]
        public ObservableCollection<string> Levels
        {
            get
            {
                return this.levels;
            }
            set
            {
                this.levels = value;
                OnPropertyChanged("Levels");
            }
        }

        /// <summary>
        /// Gets or sets updated output folder path
        /// </summary>
        public string UpdatedOutputFolderPath
        {
            get
            {
                return this.updatedOutputFolderPath;
            }
            set
            {
                this.updatedOutputFolderPath = value;
                OnPropertyChanged("UpdatedOutputFolderPath");
            }
        }

        #region Error Message

        /// <summary>
        /// Gets or sets error message to be shown on exception
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
        /// Gets or sets error message if output folder path is invalid.
        /// </summary>
        public string InvalidOutputPathErrorMessage
        {
            get
            {
                return this.invalidOutputPathErrorMessage;
            }
            set
            {
                this.invalidOutputPathErrorMessage = value;
                OnPropertyChanged("InvalidOutputPathErrorMessage");
            }
        }

        /// <summary>
        /// Gets or sets error message if output file name is invalid.
        /// </summary>
        public string InvalidOutputFileNameErrorMessage
        {
            get
            {
                return this.invalidOutputFileNameErrorMessage;
            }
            set
            {
                this.invalidOutputFileNameErrorMessage = value;
                OnPropertyChanged("InvalidOutputFileNameErrorMessage");
            }
        }

        #endregion

        #endregion

        #region ICommand

        /// <summary>
        /// Gets the create image command
        /// </summary>
        public RelayCommand CreateImageCommand
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the back to input details command.
        /// </summary>
        public RelayCommand BackToInputDetailsCommand
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the back to output details command.
        /// </summary>
        public RelayCommand BackToOutputDetailsCommand
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the start over command.
        /// </summary>
        public RelayCommand StartOverCommand
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the close command.
        /// </summary>
        public RelayCommand CloseCommand
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the open file command.
        /// </summary>
        public RelayCommand OpenFileCommand
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the open output folder command.
        /// </summary>
        public RelayCommand OpenOutputFolderCommand
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the input details command
        /// </summary>
        public RelayCommand InputDetailsCommand
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the cancel generate image command
        /// </summary>
        public RelayCommand CancelGenerateImageCommand
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the reset generate image command
        /// </summary>
        public RelayCommand RestartGenerateImageCommand
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the error close command
        /// </summary>
        public RelayCommand ErrorCloseCommand
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the open input image command
        /// </summary>
        public RelayCommand OpenInputImageCommand
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the open output image command
        /// </summary>
        public RelayCommand OpenOutputImageCommand
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the input projection type change command.
        /// </summary>
        public RelayCommand InputProjectionChangeCommand
        {
            get;
            private set;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Dispose all instance variables.
        /// </summary>
        public void Dispose()
        {
            if (this.generatePyramidsTimer != null)
            {
                this.generatePyramidsTimer.Dispose();
            }

            if (this.sdkController != null)
            {
                this.sdkController.Dispose();
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// This function is used to open the path in a new process.
        /// </summary>
        /// <param name="path">
        /// Path which has to be opened in a different process.
        /// </param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "We need to ignore all exceptions.")]
        private static void OpenPathInNewProcess(string path)
        {
            try
            {
                // Create process
                using (System.Diagnostics.Process process = new System.Diagnostics.Process())
                {
                    // strCommand is path and file name of command to run
                    process.StartInfo.FileName = path;

                    // Start the process
                    process.Start();
                }
            }
            catch
            {
                // Ignore exception.
            }
        }

        #region Command Methods

        /// <summary>
        /// Resets generation of image
        /// </summary>
        private void RestartGenerateImage()
        {
            // Restart generation of pyramids
            // Create pyramids asynchronously.
            this.CreatePyramidsAsync();
        }

        /// <summary>
        /// Starts over the tile generator for the user to create another image.
        /// </summary>
        private void StartOverTileGenerator()
        {
            this.StartOverClickedEvent.OnFire(this, new EventArgs());
        }

        /// <summary>
        /// Closes the tile generator application.
        /// </summary>
        private void CloseApplication()
        {
            // Close application
            if (!this.sdkController.IsGeneratingPyramids)
            {
                this.Close.OnFire(this, new EventArgs());
            }
            else
            {
                this.CloseImageGeneration();
            }
        }

        /// <summary>
        /// Opens WTML file which is created.
        /// </summary>
        private void OpenFile()
        {
            // Open the output file.
            string wtmlFile = Path.Combine(this.updatedOutputFolderPath, this.outputFileName + ".wtml");
            if (File.Exists(wtmlFile))
            {
                OpenPathInNewProcess(wtmlFile);
            }
            else
            {
                this.inputImageDetails.ErrorMessage = Properties.Resources.FileNotFoundError;
                this.inputImageDetails.IsErrorWindowVisible = true;
                this.inputImageDetails.IsMainWindowEnabled = false;
            }
        }

        /// <summary>
        /// Cancels creation of images and plate files.
        /// </summary>
        private void CancelImageGeneration()
        {
            // Cancel image pyramid generation.
            this.sdkController.CancelCreatePyramid();
        }

        /// <summary>
        /// Cancels and close the application if the pyramid generation is in progress.
        /// </summary>
        private void CloseImageGeneration()
        {
            // Cancel image pyramid generation and closes the application.
            this.sdkController.CancelCreatePyramid();
            this.Close.OnFire(this, new EventArgs());
        }

        /// <summary>
        /// Creates image pyramid asynchronously.
        /// </summary>
        private void CreateImage()
        {
            bool isValid = true;
            isValid = isValid && IsValidFileName(this.outputFileName);
            isValid = IsValidFolder(this.outputFolderPath) && isValid;

            if (isValid)
            {
                // Create pyramids asynchronously.
                CreatePyramidsAsync();
            }
        }

        /// <summary>
        /// Takes user back to the input details screen.
        /// </summary>
        private void BackToInputDetails()
        {
            this.IsOutputDetailsScreenVisible = false;
            this.IsInputDetailsScreenVisible = true;
        }

        /// <summary>
        /// Takes user back to the output details screen.
        /// </summary>
        private void BackToOutputDetails()
        {
            this.IsCreateImageScreenVisible = false;
            this.IsOutputDetailsScreenVisible = true;
        }

        /// <summary>
        /// Opens output folder which is created.
        /// </summary>
        private void OpenOutputFolder()
        {
            if (Directory.Exists(this.updatedOutputFolderPath))
            {
                OpenPathInNewProcess(this.updatedOutputFolderPath);
            }
            else
            {
                this.inputImageDetails.ErrorMessage = Properties.Resources.FolderNotFoundError;
                this.inputImageDetails.IsErrorWindowVisible = true;
                this.inputImageDetails.IsMainWindowEnabled = false;
            }
        }

        /// <summary>
        /// Validates the input details and uses the details to generate output data.
        /// </summary>
        private void ValidateInputDetails()
        {
            bool isValid = true;
            isValid = isValid && inputImageDetails.ValidateLatitude(inputImageDetails.TopLeftLatitude, Position.TopLeft);
            isValid = inputImageDetails.ValidateLatitude(inputImageDetails.BottomRightLatitude, Position.BottomRight) && isValid;
            isValid = inputImageDetails.ValidateLongitude(inputImageDetails.TopLeftLongitude, Position.TopLeft) && isValid;
            isValid = inputImageDetails.ValidateLongitude(inputImageDetails.BottomRightLongitude, Position.BottomRight) && isValid;
            isValid = inputImageDetails.ValidateLatBoundaryValues(inputImageDetails.TopLeftLatitude, inputImageDetails.BottomRightLatitude) && isValid;
            isValid = inputImageDetails.ValidateLongBoundaryValues(inputImageDetails.TopLeftLongitude, inputImageDetails.BottomRightLongitude) && isValid;
            isValid = inputImageDetails.ValidateInputImagePath(inputImageDetails.InputImagePath) && isValid;

            if (isValid)
            {
                this.IsInputDetailsScreenVisible = false;
                this.IsOutputDetailsScreenVisible = true;
            }
        }

        /// <summary>
        /// Closes the error pop up.
        /// </summary>
        private void ErrorPopupClose()
        {
            this.inputImageDetails.IsErrorWindowVisible = false;
            this.inputImageDetails.IsMainWindowEnabled = true;
        }

        /// <summary>
        /// Opens input image open dialog
        /// </summary>
        private void OpenInputImageFileDialog()
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Title = Properties.Resources.OpenFileDialogTitle;
                openFileDialog.Filter = Properties.Resources.OpenFileFilter;
                DialogResult result = openFileDialog.ShowDialog();
                if (result == DialogResult.OK)
                {
                    if (!openFileDialog.FileName.Equals(this.InputImageDetails.InputImagePath, StringComparison.OrdinalIgnoreCase))
                    {
                        this.InputImageDetails.InputImagePath = openFileDialog.FileName;
                    }
                }
            }
        }

        /// <summary>
        /// Opens output image file dialog
        /// </summary>
        private void OpenOutputImageFileDialog()
        {
            using (FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog())
            {
                folderBrowserDialog.Description = Properties.Resources.OpenFolderDialogDesc;
                folderBrowserDialog.SelectedPath = this.OutputFolderPath;
                DialogResult result = folderBrowserDialog.ShowDialog();
                if (result == DialogResult.OK)
                {
                    this.OutputFolderPath = folderBrowserDialog.SelectedPath;
                }
            }
        }

        /// <summary>
        /// Sets selected output projection type based on the input projection selected.
        /// </summary>
        /// <param name="inputProjection">Input projection type.</param>
        private void SetSelectedOutputProjectionTypes(InputProjections inputProjection)
        {
            switch (inputProjection)
            {
                case InputProjections.Mercator:
                    this.SelectedOutputProjectionType = ProjectionTypes.Mercator;
                    this.IsOutputProjectionEnabled = false;
                    break;
                case InputProjections.EquiRectangular:
                    this.SelectedOutputProjectionType = ProjectionTypes.Toast;
                    this.IsOutputProjectionEnabled = true;
                    break;
                default:
                    break;
            }
        }

        #endregion

        /// <summary>
        /// Gets the updated output folder path with the file name.
        /// </summary>
        /// <param name="outputDirectory">Output file name</param>
        /// <returns>Updated output directory</returns>
        private string GetUpdatedOutputFolderPath(string outputDirectory)
        {
            string outputPath = string.Empty;
            if (!string.IsNullOrEmpty(outputDirectory))
            {
                string outputFolderName = string.Format(CultureInfo.InvariantCulture, Constants.OutputFolderStructure, this.outputFileName, this.selectedOutputProjectionType.ToString(), DateTime.Now.ToString(Constants.OutputFolderDateFormat, CultureInfo.InvariantCulture));
                if (Path.IsPathRooted(outputDirectory))
                {
                    outputPath = Path.Combine(outputDirectory, outputFolderName);
                }
                else
                {
                    outputPath = Path.Combine(TileHelper.DefaultOutputDirectory, outputFolderName);
                }
            }
            return outputPath;
        }

        /// <summary>
        /// This function is used to create pyramids asynchronously.
        /// </summary>
        private void CreatePyramidsAsync()
        {
            this.ProgressBarTag = Properties.Resources.ProgressBarProcessingTag;
            this.TotalTimeRemaining = Properties.Resources.CalculatingLabel;
            this.ElapsedTime = Properties.Resources.CalculatingLabel;
            this.ProgressMessage = string.Empty;

            this.UpdatedOutputFolderPath = GetUpdatedOutputFolderPath(this.outputFolderPath);

            // Image pyramid details.
            ImagePyramidDetails inputDetails = new ImagePyramidDetails(this);

            // Get total tiles which will be generated for pyramid and plate files.
            totalTiles = GetTotalTiles();
            totalPlateTiles = GetTotalTiles(int.Parse(this.SelectedLevel, CultureInfo.InvariantCulture));

            // Set UI
            this.IsOutputDetailsScreenVisible = false;
            this.IsCreateImageScreenVisible = true;

            // Set Buttons visibility
            this.IsCancelVisible = true;
            this.IsCloseVisible = true;
            this.IsStartOverVisible = false;
            this.IsRestartVisible = false;
            this.IsBackToOutputVisible = false;

            // Set completed pane visibility.
            this.IsCompletedPanelVisible = false;

            // Start timers and stopWatches.
            this.generatePyramidsStopwatch.Reset();
            this.generatePyramidsStopwatch.Start();
            this.generatePyramidsTimer.Start();

            // Create Image pyramid.
            this.sdkController.CreatePyramids(inputDetails);
        }

        /// <summary>
        /// Populates levels from 0 to max level
        /// </summary>
        /// <param name="maxLevel">Maximum level</param>
        private void PopulateLevels(int maxLevel)
        {
            this.Levels = new ObservableCollection<string>();
            for (int level = maxLevel; level >= 0; level--)
            {
                this.Levels.Add(level.ToString(CultureInfo.InvariantCulture));
            }

            this.SelectedLevel = maxLevel.ToString(CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Populates image name and output file name.
        /// </summary>
        private void PopulateOutputFileName()
        {
            if (string.IsNullOrEmpty(inputImageDetails.ImageName))
            {
                inputImageDetails.ImageName = Path.GetFileNameWithoutExtension(inputImageDetails.InputImagePath);
                this.OutputFileName = inputImageDetails.ImageName;
            }
        }

        /// <summary>
        /// Set levels and max level
        /// </summary>
        private void SetLevels()
        {
            if (this.inputImageDetails.ImageDimension != null && !string.IsNullOrEmpty(this.InputImageDetails.TopLeftLatitude) && !string.IsNullOrEmpty(this.InputImageDetails.BottomRightLatitude)
                && !string.IsNullOrEmpty(this.InputImageDetails.BottomRightLongitude) && !string.IsNullOrEmpty(this.InputImageDetails.TopLeftLongitude))
            {
                // Max level
                int imageWidth = int.Parse(this.inputImageDetails.ImageDimension.Split('x')[0], CultureInfo.InvariantCulture);
                int imageHeight = int.Parse(this.inputImageDetails.ImageDimension.Split('x')[1], CultureInfo.InvariantCulture);

                Boundary inputBoundary = new Boundary(
                  double.Parse(this.InputImageDetails.TopLeftLongitude, CultureInfo.InvariantCulture),
                  double.Parse(this.InputImageDetails.BottomRightLatitude, CultureInfo.InvariantCulture),
                  double.Parse(this.InputImageDetails.BottomRightLongitude, CultureInfo.InvariantCulture),
                 double.Parse(this.InputImageDetails.TopLeftLatitude, CultureInfo.InvariantCulture));

                int maxLevel = TileHelper.CalculateMaximumLevel(imageHeight, imageWidth, inputBoundary);
                PopulateLevels(maxLevel);
            }
        }

        /// <summary>
        /// Populates all the default values for tile generator on the 
        /// reset of input image.
        /// </summary>
        private void PopulateDefaultValues()
        {
            inputImageDetails.ImageName = string.Empty;
            PopulateOutputFileName();
            SetSelectedOutputProjectionTypes(this.inputImageDetails.SelectedInputProjectionType);
            SetLevels();
            this.IsGeneratePlate = false;
            this.Credits = string.Empty;
            this.CreditsURL = null;
        }

        /// <summary>
        /// Sets default values for the view model properties.
        /// </summary>
        private void Initialize()
        {
            PopulateProjectionTypes();
            this.OutputFolderPath = TileHelper.DefaultOutputDirectory;
            AttachCommandHandlers();

            this.IsInputDetailsScreenVisible = true;
            this.inputImageDetails = new InputImageDetailsViewModel();
            this.inputImageDetails.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(OnInputImagePathPropertyChanged);

            this.sdkController = new SDKController();
            this.sdkController.Canceled += new EventHandler<System.ComponentModel.CancelEventArgs>(OnCanceled);
            this.sdkController.Completed += new EventHandler(OnCompleted);
            this.sdkController.Error += new EventHandler<ErrorEventArgs>(OnError);
            this.sdkController.Notify += new EventHandler<NotifyEventArgs>(OnNotify);

            this.generatePyramidsStopwatch = new Stopwatch();

            this.generatePyramidsTimer = new Timer();
            this.generatePyramidsTimer.Interval = Constants.TimerInterval;
            this.generatePyramidsTimer.Tick += new EventHandler(OnTick);
        }

        /// <summary>
        /// Attaches command handlers.
        /// </summary>
        private void AttachCommandHandlers()
        {
            this.CreateImageCommand = new RelayCommand(() => CreateImage());
            this.BackToInputDetailsCommand = new RelayCommand(() => BackToInputDetails());
            this.BackToOutputDetailsCommand = new RelayCommand(() => BackToOutputDetails());
            this.StartOverCommand = new RelayCommand(() => StartOverTileGenerator());
            this.CloseCommand = new RelayCommand(() => CloseApplication());
            this.OpenFileCommand = new RelayCommand(() => OpenFile());
            this.OpenOutputFolderCommand = new RelayCommand(() => OpenOutputFolder());
            this.InputDetailsCommand = new RelayCommand(() => ValidateInputDetails());
            this.CancelGenerateImageCommand = new RelayCommand(() => CancelImageGeneration());
            this.RestartGenerateImageCommand = new RelayCommand(() => RestartGenerateImage());
            this.ErrorCloseCommand = new RelayCommand(() => ErrorPopupClose());
            this.OpenInputImageCommand = new RelayCommand(() => OpenInputImageFileDialog());
            this.OpenOutputImageCommand = new RelayCommand(() => OpenOutputImageFileDialog());
            this.InputProjectionChangeCommand = new RelayCommand(() => SetSelectedOutputProjectionTypes(this.inputImageDetails.SelectedInputProjectionType));
        }

        /// <summary>
        /// Populates projection types for output projection types.
        /// </summary>
        private void PopulateProjectionTypes()
        {
            this.outputProjectionTypes = new ObservableCollection<ProjectionTypes>();
            this.outputProjectionTypes.Add(ProjectionTypes.Toast);
            this.outputProjectionTypes.Add(ProjectionTypes.Mercator);
        }

        /// <summary>
        /// Checks if the output folder is valid or not.
        /// </summary>
        /// <param name="outputPath">Output folder path</param>
        /// <returns>True if folder is valid</returns>
        private bool IsValidFolder(string outputPath)
        {
            if (!string.IsNullOrEmpty(outputPath))
            {
                try
                {
                    // Directory info doesn't throw security exception in network path.
                    DirectoryInfo directory = new DirectoryInfo(outputPath);
                    if (directory != null)
                    {
                        this.InvalidOutputPathErrorMessage = string.Empty;
                        return true;
                    }
                }
                catch (ArgumentException ex)
                {
                    this.InvalidOutputPathErrorMessage = Properties.Resources.OutputPathInvalidCharacterError;
                    Logger.LogException(ex);
                }
                catch (PathTooLongException ex)
                {
                    this.InvalidOutputPathErrorMessage = Properties.Resources.PathTooLongErrorMessage;
                    Logger.LogException(ex);
                }
                catch (SecurityException ex)
                {
                    this.InvalidOutputPathErrorMessage = Properties.Resources.SecurityErrorMessage;
                    Logger.LogException(ex);
                }
                catch (IOException ex)
                {
                    this.InvalidOutputPathErrorMessage = Properties.Resources.SecurityErrorMessage;
                    Logger.LogException(ex);
                }
                catch (UnauthorizedAccessException ex)
                {
                    this.InvalidOutputPathErrorMessage = Properties.Resources.SecurityErrorMessage;
                    Logger.LogException(ex);
                }
                catch (NotSupportedException ex)
                {
                    this.InvalidOutputPathErrorMessage = Properties.Resources.GenericErrorMessage;
                    Logger.LogException(ex);
                }
            }
            else
            {
                this.InvalidOutputPathErrorMessage = Properties.Resources.MandatoryError;
            }
            return false;
        }

        /// <summary>
        /// Checks if the file name is empty or having invalid character.
        /// </summary>
        /// <param name="fileName">Output file name</param>
        /// <returns>True if the file name is not empty or not having invalid character.</returns>
        private bool IsValidFileName(string fileName)
        {
            bool isValid = true;
            if (string.IsNullOrEmpty(fileName))
            {
                this.InvalidOutputFileNameErrorMessage = Properties.Resources.MandatoryError;
                isValid = false;
            }
            else
            {
                foreach (char invalidCharacter in Path.GetInvalidFileNameChars())
                {
                    if (fileName.Contains(invalidCharacter.ToString()))
                    {
                        this.InvalidOutputFileNameErrorMessage = Properties.Resources.OutputFilenameInvalidCharacterError;
                        isValid = false;
                        break;
                    }
                }

                if (isValid)
                {
                    this.InvalidOutputFileNameErrorMessage = string.Empty;
                }
            }
            return isValid;
        }

        /// <summary>
        /// This is an event handler for error event.
        /// </summary>
        /// <param name="sender">
        /// Instance of SDKController.
        /// </param>
        /// <param name="e">
        /// Error event arguments.
        /// </param>
        private void OnError(object sender, ErrorEventArgs e)
        {
            // Set Buttons visibility
            this.IsStartOverVisible = true;
            this.IsCloseVisible = true;
            this.IsBackToOutputVisible = true;
            this.IsRestartVisible = true;
            this.IsCancelVisible = false;

            // Set completed pane visibility.
            this.IsCompletedPanelVisible = false;

            this.ProgressBarTag = Properties.Resources.ProgressBarErrorTag;
            this.TotalTimeRemaining = Properties.Resources.TileCompletedTime;

            // Stop timers and stopWatches.
            this.generatePyramidsStopwatch.Stop();
            this.generatePyramidsTimer.Stop();

            this.ElapsedTime = string.Format(CultureInfo.CurrentUICulture, "{0:D} Sec", (this.generatePyramidsStopwatch.ElapsedMilliseconds / 1000));

            bool isOutofMemoryException = false;
            if (e.Error is OutOfMemoryException)
            {
                isOutofMemoryException = true;
            }
            else if (e.Error is AggregateException)
            {
                AggregateException exception = (AggregateException)e.Error;
                foreach (Exception innerException in exception.InnerExceptions)
                {
                    if (innerException is OutOfMemoryException)
                    {
                        isOutofMemoryException = true;
                        break;
                    }
                }
            }

            this.PyramidGenerationMessage = isOutofMemoryException ?
                Properties.Resources.PyramidGenerationOutOfMemoryMessage :
                Properties.Resources.PyramidGenerationErrorMessage;
        }

        /// <summary>
        /// This is an event handler for completed event.
        /// </summary>
        /// <param name="sender">
        /// Instance of SDKController.
        /// </param>
        /// <param name="e">
        /// Event arguments.
        /// </param>
        private void OnCompleted(object sender, EventArgs e)
        {
            // Set Buttons visibility
            this.IsStartOverVisible = true;
            this.IsCloseVisible = true;
            this.IsCancelVisible = false;
            this.IsRestartVisible = false;
            this.IsBackToOutputVisible = false;

            // Set completed pane visibility.
            this.IsCompletedPanelVisible = true;

            this.ProgressBarTag = Properties.Resources.ProgressBarSuccessTag;
            this.TotalTimeRemaining = Properties.Resources.TileCompletedTime;

            // Stop timers and stopWatches.
            this.generatePyramidsStopwatch.Stop();
            this.generatePyramidsTimer.Stop();

            this.ElapsedTime = string.Format(CultureInfo.CurrentUICulture, "{0:D} Sec", (this.generatePyramidsStopwatch.ElapsedMilliseconds / 1000));

            // Set success message
            this.PyramidGenerationMessage = string.Format(
                CultureInfo.CurrentCulture,
                Properties.Resources.PyramidGenerationCompletedMessage,
                (this.generatePyramidsStopwatch.ElapsedMilliseconds / 1000));
        }

        /// <summary>
        /// This is an event handler for cancelled event.
        /// </summary>
        /// <param name="sender">
        /// Instance of SDKController.
        /// </param>
        /// <param name="e">
        /// Cancel Event arguments.
        /// </param>
        private void OnCanceled(object sender, System.ComponentModel.CancelEventArgs e)
        {
            // Set Buttons visibility
            this.IsStartOverVisible = true;
            this.IsCloseVisible = true;
            this.IsBackToOutputVisible = true;
            this.IsRestartVisible = true;
            this.IsCancelVisible = false;

            // Set completed pane visibility.
            this.IsCompletedPanelVisible = false;

            this.ProgressBarTag = Properties.Resources.ProgressBarErrorTag;
            this.TotalTimeRemaining = Properties.Resources.TileCompletedTime;

            // Stop timers and stopWatches.
            this.generatePyramidsStopwatch.Stop();
            this.generatePyramidsTimer.Stop();

            this.ElapsedTime = string.Format(CultureInfo.CurrentUICulture, "{0:D} Sec", (this.generatePyramidsStopwatch.ElapsedMilliseconds / 1000));

            // Set user cancelled message
            this.PyramidGenerationMessage = Properties.Resources.UserCancelledErrorMessage;
        }

        /// <summary>
        /// This is an event handler for Notify step event.
        /// </summary>
        /// <param name="sender">
        /// Instance of SDKController.
        /// </param>
        /// <param name="e">
        /// Notify step event arguments.
        /// </param>
        private void OnNotify(object sender, NotifyEventArgs e)
        {
            string step = string.Empty;
            switch (e.Step)
            {
                case PyramidGenerationSteps.LoadingImage:
                    step = Properties.Resources.StepLoadingImage;
                    break;
                case PyramidGenerationSteps.PyramidGeneration:
                    step = Properties.Resources.StepPyramidGeneration;
                    break;
                case PyramidGenerationSteps.PlateFileGeneration:
                    step = Properties.Resources.StepPlateFileGeneration;
                    break;
                case PyramidGenerationSteps.ThumbnailGeneration:
                    step = Properties.Resources.StepThumbnailGeneration;
                    break;
                case PyramidGenerationSteps.WTMLGeneration:
                    step = Properties.Resources.StepWTMLFileCreate;
                    break;
            }

            this.currentStep = e.Step;

            string status = string.Empty;
            switch (e.Status)
            {
                case PyramidGenerationStatus.Started:
                    this.currentStepStartedAt = this.generatePyramidsStopwatch.ElapsedMilliseconds;
                    status = Properties.Resources.StepStarted;
                    break;
                case PyramidGenerationStatus.Completed:
                    status = Properties.Resources.StepCompleted;
                    break;
            }

            string lineBreak = Constants.LineBreak;
            if (string.IsNullOrEmpty(this.progressMessage))
            {
                lineBreak = string.Empty;
            }
            this.ProgressMessage = string.Format(CultureInfo.CurrentUICulture, "{0}{1}: {2} has {3}...", this.progressMessage + lineBreak, DateTime.Now.TimeOfDay.ToString(@"hh\:mm\:ss", CultureInfo.CurrentCulture), step, status);
        }

        /// <summary>
        /// This is an event handler for on tick event.
        /// </summary>
        /// <param name="sender">
        /// Instance of generatePyramidsTimer.
        /// </param>
        /// <param name="e">
        /// Event arguments.
        /// </param>
        private void OnTick(object sender, EventArgs e)
        {
            // Update elapsed time.
            this.ElapsedTime = string.Format(CultureInfo.CurrentUICulture, "{0:D} Sec", (this.generatePyramidsStopwatch.ElapsedMilliseconds / 1000));

            // Update time remaining.
            long processedTiles = this.sdkController.TilesProcessed;
            long processedPlateTiles = this.sdkController.PlateTilesProcessed;

            long timeTaken = this.generatePyramidsStopwatch.ElapsedMilliseconds - this.currentStepStartedAt;

            if (processedTiles <= 0)
            {
                // Tiles progress is not started or completed. So show calculating in the time remaining text block.
                this.TotalTimeRemaining = Properties.Resources.CalculatingLabel;
            }
            else
            {
                long timeRemaining = GetEstimatedTimeRemaining(processedTiles, processedPlateTiles, timeTaken);

                this.TotalTimeRemaining = string.Format(CultureInfo.CurrentUICulture, "{0:D} Sec", timeRemaining / 1000);
            }
        }

        /// <summary>
        /// Event is fired on the input image property change.
        /// </summary>
        /// <param name="sender">Input image view model</param>
        /// <param name="e">Routed event</param>
        private void OnInputImagePathPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == Constants.InputImagePath)
            {
                PopulateDefaultValues();
            }
            else if (e.PropertyName == Constants.TopLeftLatitudeProp || e.PropertyName == Constants.TopLeftLongitudeProp || e.PropertyName == Constants.BottomRightLatitudeProp ||
                e.PropertyName == Constants.BottomRightLongitudeProp)
            {
                SetLevels();
            }
        }

        /// <summary>
        /// This function is used to get the total number of tiles till a given level for plate files.
        /// </summary>
        /// <param name="maxLevel">
        /// Number of levels.
        /// </param>
        /// <returns>
        /// Total number of tiles.
        /// </returns>
        private long GetTotalTiles(int maxLevel)
        {
            long tilesCount;
            if (maxLevel <= 0)
            {
                return 1;
            }
            else
            {
                tilesCount = (long)(Math.Pow(2, maxLevel) * Math.Pow(2, maxLevel));
            }

            return tilesCount + GetTotalTiles(maxLevel - 1);
        }

        /// <summary>
        /// This function is used to get the total number of tiles till a given level for pyramids.
        /// </summary>
        /// <returns>
        /// Total number of tiles.
        /// </returns>
        private long GetTotalTiles()
        {
            int maxLevel = int.Parse(this.SelectedLevel, CultureInfo.InvariantCulture);
            long tilesCount = 0;

            Boundary inputBoundary = new Boundary(
                   double.Parse(this.InputImageDetails.TopLeftLongitude, CultureInfo.InvariantCulture),
                   double.Parse(this.InputImageDetails.BottomRightLatitude, CultureInfo.InvariantCulture),
                   double.Parse(this.InputImageDetails.BottomRightLongitude, CultureInfo.InvariantCulture),
                  double.Parse(this.InputImageDetails.TopLeftLatitude, CultureInfo.InvariantCulture));

            // Calculate base pyramid tiles which are going to be created.
            if (this.SelectedOutputProjectionType == ProjectionTypes.Toast)
            {
                // For Toast projection, longitude spans from 0 to +360 and latitude from 90 to -90.
                inputBoundary.Left += 180;
                inputBoundary.Right += 180;

                IDictionary<int, List<Tile>> toastProjectionCoordinates = ToastHelper.ComputeTileCoordinates(inputBoundary, maxLevel);
                for (int level = 0; level <= maxLevel; level++)
                {
                    tilesCount += toastProjectionCoordinates[level].Count;
                }
            }
            else
            {
                for (int level = 0; level <= maxLevel; level++)
                {
                    int tileXMin = Helper.GetMercatorXTileFromLongitude(inputBoundary.Left, level);
                    int tileXMax = Helper.GetMercatorXTileFromLongitude(inputBoundary.Right, level);
                    int tileYMin = Helper.GetMercatorYTileFromLatitude(inputBoundary.Bottom, level);
                    int tileYMax = Helper.GetMercatorYTileFromLatitude(inputBoundary.Top, level);

                    // If both x-minimum (y-minimum) and x-maximum(y-maximum) are equal,
                    // the loop should run at least for ONE time, Hence incrementing the  x-maximum(y-maximum) by 1
                    tileXMax = (tileXMin == tileXMax) ? tileXMax + 1 : tileXMax;
                    tileYMax = (tileYMin == tileYMax) ? tileYMax + 1 : tileYMax;

                    tilesCount += (tileXMax - tileXMin) * (tileYMax - tileYMin);
                }
            }
            return tilesCount;
        }

        /// <summary>
        /// Gets estimated time remaining for the pyramid generation.
        /// </summary>
        /// <param name="processedTiles">Processed tiles</param>
        /// <param name="processedPlateTiles">Processed plate files</param>
        /// <param name="timeTaken">Time taken to process the current step.</param>
        /// <returns>Estimated remaining time</returns>
        private long GetEstimatedTimeRemaining(long processedTiles, long processedPlateTiles, long timeTaken)
        {
            // Time remaining = (Average Time taken for each tile) * (Total unprocessed tiles)
            long timeRemaining = (timeTaken / processedTiles) * (totalTiles - processedTiles);

            // If we need to generate plate files then we need to consider plate file generation also for time remaining.
            if (this.IsGeneratePlate)
            {
                if (processedPlateTiles <= 0)
                {
                    // If for first time we are calculating the time remaining then we need to get the guesstimate for Plate file generation
                    //// As of now we are calculating the time taken for plate file as 2 time the estimated value for pyramid generation.

                    if (((double)processedTiles / (double)totalTiles) * 100 <= Constants.ProcessedTilesPercentage)
                    {
                        this.guesstimatePlateFileGeneration = (int)(timeRemaining * Constants.TileTimeMultiplier);
                    }

                    timeRemaining += guesstimatePlateFileGeneration;
                }
                else
                {
                    timeRemaining = (timeTaken / processedPlateTiles) * (totalPlateTiles - processedPlateTiles);
                }
            }

            // If the thumbnail generation has not started the time remaining is the calculated time remaining for plate/pyramid 
            // and thumbnail generation time and if the thumbnail generation has started then the time remaining is the thumbnail 
            // estimated time reduced by the elapsed time.
            switch (this.currentStep)
            {
                case PyramidGenerationSteps.LoadingImage:
                case PyramidGenerationSteps.PyramidGeneration:
                case PyramidGenerationSteps.PlateFileGeneration:
                    timeRemaining = timeRemaining + sdkController.ThumbnailEstimatedTime;
                    break;
                case PyramidGenerationSteps.ThumbnailGeneration:
                    timeRemaining = sdkController.ThumbnailEstimatedTime - (this.generatePyramidsStopwatch.ElapsedMilliseconds - currentStepStartedAt);
                    break;
                case PyramidGenerationSteps.WTMLGeneration:
                    timeRemaining = 1;
                    break;
                default:
                    break;
            }

            if (timeRemaining <= 0)
            {
                timeRemaining = 1;
            }
            return timeRemaining;
        }

        #endregion
    }
}
