//-----------------------------------------------------------------------
// <copyright file="ImagePyramidDetails.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation 2011. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Globalization;
using Microsoft.Research.Wwt.Sdk.Core;
using Core = Microsoft.Research.Wwt.Sdk.Core;

namespace Microsoft.Research.Wwt.TileGenerator
{
    /// <summary>
    /// This class is used to store the details of the pyramid generation.
    /// </summary>
    public class ImagePyramidDetails
    {
        /// <summary>
        /// Initializes a new instance of the ImagePyramidDetails class.
        /// </summary>
        public ImagePyramidDetails(TileGeneratorViewModel viewModel)
        {
            if (viewModel != null)
            {
                this.InputImagePath = viewModel.InputImageDetails.InputImagePath;
                this.InputProjection = viewModel.InputImageDetails.SelectedInputProjectionType;
                this.OutputFilename = viewModel.OutputFileName;
                this.OutputProjection = viewModel.SelectedOutputProjectionType;
                this.OutputDirectory = viewModel.UpdatedOutputFolderPath;
                this.IsGeneratePlate = viewModel.IsGeneratePlate;
                this.Level = int.Parse(viewModel.SelectedLevel, CultureInfo.InvariantCulture);
                this.Credits = viewModel.Credits;
                this.CreditsURL = viewModel.CreditsURL == null ? string.Empty : viewModel.CreditsURL.ToString();

                this.InputBoundary = new Core.Boundary(
                    double.Parse(viewModel.InputImageDetails.TopLeftLongitude, CultureInfo.InvariantCulture),
                    double.Parse(viewModel.InputImageDetails.BottomRightLatitude, CultureInfo.InvariantCulture),
                    double.Parse(viewModel.InputImageDetails.BottomRightLongitude, CultureInfo.InvariantCulture),
                    double.Parse(viewModel.InputImageDetails.TopLeftLatitude, CultureInfo.InvariantCulture));
            }
        }

        /// <summary>
        /// Gets or sets input image path.
        /// </summary>
        public string InputImagePath { get; set; }

        /// <summary>
        /// Gets or sets output directory path.
        /// </summary>
        public string OutputDirectory { get; set; }

        /// <summary>
        /// Gets or sets the output file name.
        /// </summary>
        public string OutputFilename { get; set; }

        /// <summary>
        /// Gets or sets credits.
        /// </summary>
        public string Credits { get; set; }

        /// <summary>
        /// Gets or sets Credit URL.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1056:UriPropertiesShouldNotBeStrings", Justification = "Need to be a string.")]
        public string CreditsURL { get; set; }

        /// <summary>
        /// Gets or sets input projection
        /// </summary>
        public InputProjections InputProjection { get; set; }

        /// <summary>
        /// Gets or sets output projection.
        /// </summary>
        public ProjectionTypes OutputProjection { get; set; }

        /// <summary>
        /// Gets or sets Input Boundary.
        /// </summary>
        public Core.Boundary InputBoundary { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to generate the plate file or not.
        /// </summary>
        public bool IsGeneratePlate { get; set; }

        /// <summary>
        /// Gets or sets the number of levels which has to be generated.
        /// </summary>
        public int Level { get; set; }
    }
}
