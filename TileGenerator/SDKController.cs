//-----------------------------------------------------------------------
// <copyright file="SDKController.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation 2011. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.Threading;
using Microsoft.Research.Wwt.Sdk.Core;
using Core = Microsoft.Research.Wwt.Sdk.Core;

namespace Microsoft.Research.Wwt.TileGenerator
{
    /// <summary>
    /// This class defines method for interaction with SDK Core API.
    /// </summary>
    public sealed class SDKController : IDisposable
    {
        private BackgroundWorker createImageWorker;
        private CancellationTokenSource cancellationToken = null;
        private Core.TileGenerator tileGenerator = null;
        private Core.PlateFileGenerator plateGenerator = null;

        /// <summary>
        /// Initializes a new instance of the SDKController class
        /// </summary>
        public SDKController()
        {
            this.createImageWorker = new BackgroundWorker();
            this.createImageWorker.WorkerSupportsCancellation = true;
            this.createImageWorker.WorkerReportsProgress = false;

            this.createImageWorker.DoWork += new DoWorkEventHandler(OnCreateImageWorkerDoWork);
            this.createImageWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(OnCreateImageWorkerRunWorkerCompleted);
        }

        #region Events

        /// <summary>
        /// Image pyramid generation cancelled event.
        /// </summary>
        public event EventHandler<CancelEventArgs> Canceled;

        /// <summary>
        /// Image pyramid generation completed event.
        /// </summary>
        public event EventHandler Completed;

        /// <summary>
        /// Image pyramid generation error event.
        /// </summary>
        public event EventHandler<ErrorEventArgs> Error;

        /// <summary>
        /// Notifies if a step is started or completed.
        /// </summary>
        public event EventHandler<NotifyEventArgs> Notify;

        #endregion

        /// <summary>
        /// Gets a value indicating whether the image pyramid generation is in progress or not.
        /// </summary>
        public bool IsGeneratingPyramids
        {
            get
            {
                return this.createImageWorker.IsBusy;
            }
        }

        /// <summary>
        /// Gets total number of image pyramids which are processed.
        /// </summary>
        public long TilesProcessed
        {
            get
            {
                return this.tileGenerator == null ? -1 : this.tileGenerator.TilesProcessed;
            }
        }

        /// <summary>
        /// Gets number of plate tiles which are processed.
        /// </summary>
        public long PlateTilesProcessed
        {
            get
            {
                return this.plateGenerator == null ? -1 : this.plateGenerator.TilesProcessed;
            }
        }

        /// <summary>
        /// Gets or sets thumbnail estimated time
        /// </summary>
        public long ThumbnailEstimatedTime
        {
            get;
            set;
        }

        /// <summary>
        /// This function is used to create image pyramids.
        /// </summary>
        /// <param name="inputDetails">
        /// Input image details.
        /// </param>
        public void CreatePyramids(ImagePyramidDetails inputDetails)
        {
            if (!this.createImageWorker.IsBusy)
            {
                this.tileGenerator = null;
                this.plateGenerator = null;
                this.cancellationToken = null;

                // Start the image pyramids generation operation asynchronously.
                this.createImageWorker.RunWorkerAsync(inputDetails);
            }
        }

        /// <summary>
        /// This function is used to cancel the existing image pyramid generation process.
        /// </summary>
        public void CancelCreatePyramid()
        {
            if (this.createImageWorker.IsBusy && this.createImageWorker.WorkerSupportsCancellation == true)
            {
                // Cancel the asynchronous operation.
                this.createImageWorker.CancelAsync();

                // Cancel parallel operations.
                if (this.cancellationToken != null)
                {
                    this.cancellationToken.Cancel(true);
                }
            }
        }

        #region IDisposable Members

        /// <summary>
        /// Dispose All member variables.
        /// </summary>
        public void Dispose()
        {
            if (this.createImageWorker != null)
            {
                this.createImageWorker.Dispose();
            }

            if (this.cancellationToken != null)
            {
                this.cancellationToken.Dispose();
            }
        }

        #endregion

        /// <summary>
        /// This function is used to check if the user has clicked cancel. 
        ///     If the user has clicked cancel then stop the existing process and raise an OperationCanceledException.
        /// </summary>
        /// <param name="e">
        /// DoWork event argument.
        /// </param>
        private void CheckCancel(DoWorkEventArgs e)
        {
            if ((createImageWorker.CancellationPending == true) && this.cancellationToken != null)
            {
                e.Cancel = true;
                throw new OperationCanceledException(this.cancellationToken.Token);
            }
        }

        /// <summary>
        /// This event is raised when the create pyramid function is called.
        /// </summary>
        /// <param name="sender">
        /// create ImageWorker.
        /// </param>
        /// <param name="e">
        /// DoWork EventArgs.
        /// </param>
        private void OnCreateImageWorkerDoWork(object sender, DoWorkEventArgs e)
        {
            ImagePyramidDetails inputDetails = e.Argument as ImagePyramidDetails;
            if (inputDetails != null)
            {
                this.CheckCancel(e);

                NotifyMessage(PyramidGenerationSteps.LoadingImage, PyramidGenerationStatus.Started);

                this.CheckCancel(e);

                NotifyMessage(PyramidGenerationSteps.PyramidGeneration, PyramidGenerationStatus.Started);

                // Define ITileCreator instance for creating image tiles.
                // If the input projection type and output projection type is same then we need to tile the image.
                Core.ITileCreator tileCreator = null;

                // Calculates the estimated thumbnail generation time
                Stopwatch thumbnailGenerationStopwatch = new Stopwatch();
                thumbnailGenerationStopwatch.Start();
                if (inputDetails.InputProjection == InputProjections.EquiRectangular)
                {
                    // Read and initialize the image as an IGrid.
                    var imageGrid = new Core.ImageGrid(inputDetails.InputImagePath, true);
                    var equirectangularGridMap = new Core.EquirectangularGridMap(imageGrid, inputDetails.InputBoundary);
                    var imageColorMap = new Core.ImageColorMap(equirectangularGridMap);

                    tileCreator = Core.TileCreatorFactory.CreateImageTileCreator(
                        imageColorMap,
                        inputDetails.OutputProjection,
                        inputDetails.OutputDirectory);
                }
                else
                {
                    Core.IImageTileSerializer serializer = new Core.ImageTileSerializer(
                        Core.TileHelper.GetDefaultImageTilePathTemplate(inputDetails.OutputDirectory),
                        ImageFormat.Png);
                    tileCreator = new Core.TileChopper(inputDetails.InputImagePath, serializer, inputDetails.OutputProjection, inputDetails.InputBoundary);
                }

                thumbnailGenerationStopwatch.Stop();

                // Thumbnail estimated time is time required to load the image and then create the thumbnail with graphics
                this.ThumbnailEstimatedTime = thumbnailGenerationStopwatch.ElapsedMilliseconds * Constants.ThumbnailMultiplier;

                // Define plumbing for looping through all the tiles to be created for base image and pyramid.
                tileGenerator = new Core.TileGenerator(tileCreator);

                this.cancellationToken = new CancellationTokenSource();
                tileGenerator.ParallelOptions.CancellationToken = this.cancellationToken.Token;

                this.CheckCancel(e);

                // Define bounds of the image. Image is assumed to cover the entire world.
                // If not, change the coordinates accordingly.
                // For Mercator projection, longitude spans from -180 to +180 and latitude from 90 to -90.
                Core.Boundary gridBoundary = new Core.Boundary(
                    inputDetails.InputBoundary.Left,
                    inputDetails.InputBoundary.Top,
                    inputDetails.InputBoundary.Right,
                    inputDetails.InputBoundary.Bottom);
                if (inputDetails.OutputProjection == ProjectionTypes.Toast)
                {
                    // For Toast projection, longitude spans from 0 to +360 and latitude from 90 to -90.
                    gridBoundary.Left += 180;
                    gridBoundary.Right += 180;
                }

                // Start building base image and the pyramid.
                tileGenerator.Generate(inputDetails.Level, gridBoundary);

                if (inputDetails.IsGeneratePlate)
                {
                    NotifyMessage(PyramidGenerationSteps.PlateFileGeneration, PyramidGenerationStatus.Started);
                    this.CheckCancel(e);

                    // Generate Plate file.
                    Core.ImageTileSerializer pyramid = new Core.ImageTileSerializer(
                        Core.TileHelper.GetDefaultImageTilePathTemplate(inputDetails.OutputDirectory),
                        ImageFormat.Png);

                    plateGenerator = new Core.PlateFileGenerator(
                        System.IO.Path.Combine(inputDetails.OutputDirectory, inputDetails.OutputFilename + ".plate"),
                        inputDetails.Level,
                        ImageFormat.Png);

                    plateGenerator.CreateFromImageTile(pyramid);
                }

                this.CheckCancel(e);

                NotifyMessage(PyramidGenerationSteps.ThumbnailGeneration, PyramidGenerationStatus.Started);

                // Generate Thumbnail Images.
                string thumbnailFile = System.IO.Path.Combine(inputDetails.OutputDirectory, inputDetails.OutputFilename + ".jpeg");
                Core.TileHelper.GenerateThumbnail(inputDetails.InputImagePath, 96, 45, thumbnailFile, ImageFormat.Jpeg);

                this.CheckCancel(e);

                NotifyMessage(PyramidGenerationSteps.WTMLGeneration, PyramidGenerationStatus.Started);

                // Get the path of image tiles created and save it in WTML file.
                string pyramidPath = Core.WtmlCollection.GetWtmlTextureTilePath(Core.TileHelper.GetDefaultImageTilePathTemplate(inputDetails.OutputDirectory), ImageFormat.Png.ToString());

                Core.Boundary inputBoundary = new Core.Boundary(
                    inputDetails.InputBoundary.Left,
                    inputDetails.InputBoundary.Top,
                    inputDetails.InputBoundary.Right,
                    inputDetails.InputBoundary.Bottom);

                // Create and save WTML collection file.
                Core.WtmlCollection wtmlCollection = new Core.WtmlCollection(inputDetails.OutputFilename, thumbnailFile, pyramidPath, inputDetails.Level, inputDetails.OutputProjection, inputBoundary);
                wtmlCollection.Credit = inputDetails.Credits;
                wtmlCollection.CreditUrl = inputDetails.CreditsURL;
                string path = System.IO.Path.Combine(inputDetails.OutputDirectory, inputDetails.OutputFilename + ".wtml");
                wtmlCollection.Save(path);
            }
        }

        /// <summary>
        /// This function is used to notify the step status
        /// </summary>
        /// <param name="step">
        /// Step details.
        /// </param>
        /// <param name="status">
        /// Status of the step.
        /// </param>
        private void NotifyMessage(PyramidGenerationSteps step, PyramidGenerationStatus status)
        {
            NotifyEventArgs eventArgs = new NotifyEventArgs() { Step = step, Status = status };
            this.Notify.OnFire(this, eventArgs);
        }

        /// <summary>
        /// This event is raised when the create pyramid process is completed.
        /// </summary>
        /// <param name="sender">
        /// Background Worker.
        /// </param>
        /// <param name="e">
        /// Background Worker Completed EventArgs.
        /// </param>
        private void OnCreateImageWorkerRunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled)
            {
                // Cancelled by user.
                this.Canceled.OnFire(this, new CancelEventArgs(true));
            }
            else if (e.Error != null)
            {
                // An error has error while generating pyramids..
                if (e.Error is OperationCanceledException)
                {
                    this.Canceled.OnFire(this, new CancelEventArgs(true));
                }
                else
                {
                    Logger.LogException(e.Error);
                    this.Error.OnFire(this, new ErrorEventArgs() { Error = e.Error });
                }
            }
            else
            {
                // Pyramids generation completed successfully.
                this.Completed.OnFire(this, new EventArgs());
            }

            this.tileGenerator = null;
        }
    }
}
