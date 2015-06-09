//-----------------------------------------------------------------------
// <copyright file="TileGeneratorViewModelTest.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation 2011. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using Microsoft.Research.Wwt.Sdk.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.Research.Wwt.TileGenerator.Test
{
    /// <summary>
    /// This is a test class for TileGeneratorViewModelTest and is intended
    /// to contain all TileGeneratorViewModelTest Unit Tests
    /// </summary>
    [TestClass()]
    public class TileGeneratorViewModelTest
    {
        /// <summary>
        /// A test for TotalTimeRemaining
        /// </summary>
        [TestMethod()]
        public void TotalTimeRemainingTest()
        {
            using (TileGeneratorViewModel_Accessor target = new TileGeneratorViewModel_Accessor())
            {
                string expected = "12:30:00";
                string actual;
                target.TotalTimeRemaining = expected;
                actual = target.TotalTimeRemaining;
                Assert.AreEqual(expected, actual);
            }
        }

        /// <summary>
        /// A test for SelectedOutputProjectionType
        /// </summary>
        [TestMethod()]
        public void SelectedOutputProjectionTypeTest()
        {
            using (TileGeneratorViewModel_Accessor target = new TileGeneratorViewModel_Accessor())
            {
                ProjectionTypes expected = ProjectionTypes.Mercator;
                ProjectionTypes actual;
                target.SelectedOutputProjectionType = expected;
                actual = target.SelectedOutputProjectionType;
                Assert.AreEqual(expected, actual);
            }
        }

        /// <summary>
        /// A test for SelectedLevel
        /// </summary>
        [TestMethod()]
        public void SelectedLevelTest()
        {
            using (TileGeneratorViewModel_Accessor target = new TileGeneratorViewModel_Accessor())
            {
                string expected = "6";
                string actual;
                target.SelectedLevel = expected;
                actual = target.SelectedLevel;
                Assert.AreEqual(expected, actual);
            }
        }

        /// <summary>
        /// A test for PyramidGenerationMessage
        /// </summary>
        [TestMethod()]
        public void PyramidGenerationMessageTest()
        {
            using (TileGeneratorViewModel_Accessor target = new TileGeneratorViewModel_Accessor())
            {
                string expected = "Generated successfully";
                string actual;
                target.PyramidGenerationMessage = expected;
                actual = target.PyramidGenerationMessage;
                Assert.AreEqual(expected, actual);
            }
        }

        /// <summary>
        /// A test for ProgressMessage
        /// </summary>
        [TestMethod()]
        public void ProgressMessageTest()
        {
            using (TileGeneratorViewModel_Accessor target = new TileGeneratorViewModel_Accessor())
            {
                string expected = "Progressing";
                string actual;
                target.ProgressMessage = expected;
                actual = target.ProgressMessage;
                Assert.AreEqual(expected, actual);
            }
        }

        /// <summary>
        /// A test for ProgressBarTag
        /// </summary>
        [TestMethod()]
        public void ProgressBarTagTest()
        {
            using (TileGeneratorViewModel_Accessor target = new TileGeneratorViewModel_Accessor())
            {
                string expected = "Error";
                string actual;
                target.ProgressBarTag = expected;
                actual = target.ProgressBarTag;
                Assert.AreEqual(expected, actual);
            }
        }

        /// <summary>
        /// A test for OutputProjectionTypes
        /// </summary>
        [TestMethod()]
        public void OutputProjectionTypesTest()
        {
            using (TileGeneratorViewModel target = new TileGeneratorViewModel())
            {
                ProjectionTypes[] expected = (ProjectionTypes[])Enum.GetValues(typeof(ProjectionTypes));
                foreach (ProjectionTypes projectionType in expected)
                {
                    target.OutputProjectionTypes.Add(projectionType);
                }
                ObservableCollection<ProjectionTypes> actual;
                actual = target.OutputProjectionTypes;
                foreach (ProjectionTypes projectionType in expected)
                {
                    Assert.IsTrue(actual.Contains(projectionType));
                }
            }
        }

        /// <summary>
        /// A test for OutputFolderPath
        /// </summary>
        [TestMethod()]
        public void OutputFolderPathTest()
        {
            using (TileGeneratorViewModel target = new TileGeneratorViewModel())
            {
                string expected = Environment.CurrentDirectory + @"\Image\L0X0Y0.Png";
                string actual;
                target.OutputFolderPath = expected;
                actual = target.OutputFolderPath;
                Assert.AreEqual(expected, actual);
            }
        }

        /// <summary>
        /// A test for OutputFileName
        /// </summary>
        [TestMethod()]
        public void OutputFileNameTest()
        {
            using (TileGeneratorViewModel target = new TileGeneratorViewModel())
            {
                string expected = "Image";
                string actual;
                target.OutputFileName = expected;
                actual = target.OutputFileName;
                Assert.AreEqual(expected, actual);
            }
        }

        /// <summary>
        /// A test for Levels
        /// </summary>
        [TestMethod()]
        public void LevelsLayerTest()
        {
            using (TileGeneratorViewModel target = new TileGeneratorViewModel())
            {
                ObservableCollection<string> expected = new ObservableCollection<string>();
                expected.Add("0");
                expected.Add("1");
                ObservableCollection<string> actual;
                target.Levels = expected;
                actual = target.Levels;
                Assert.AreEqual(expected.Count, actual.Count);
                foreach (string level in expected)
                {
                    Assert.IsTrue(actual.Contains(level));
                }
            }
        }

        /// <summary>
        /// A test for IsStartOverEnabled
        /// </summary>
        [TestMethod()]
        public void IsStartOverEnabledTest()
        {
            using (TileGeneratorViewModel target = new TileGeneratorViewModel())
            {
                bool expected = true;
                bool actual;
                target.IsStartOverVisible = expected;
                actual = target.IsStartOverVisible;
                Assert.AreEqual(expected, actual);
            }
        }

        /// <summary>
        /// A test for IsResetEnabled
        /// </summary>
        [TestMethod()]
        public void IsResetEnabledTest()
        {
            using (TileGeneratorViewModel target = new TileGeneratorViewModel())
            {
                bool expected = true;
                bool actual;
                target.IsRestartVisible = expected;
                actual = target.IsRestartVisible;
                Assert.AreEqual(expected, actual);
            }
        }

        /// <summary>
        /// A test for IsOutputProjectionEnabled
        /// </summary>
        [TestMethod()]
        public void IsOutputProjectionEnabledTest()
        {
            using (TileGeneratorViewModel target = new TileGeneratorViewModel())
            {
                bool expected = true;
                bool actual;
                target.IsOutputProjectionEnabled = expected;
                actual = target.IsOutputProjectionEnabled;
                Assert.AreEqual(expected, actual);
            }
        }

        /// <summary>
        /// A test for IsOutputDetailsScreenVisible
        /// </summary>
        [TestMethod()]
        public void IsOutputDetailsScreenVisibleTest()
        {
            using (TileGeneratorViewModel target = new TileGeneratorViewModel())
            {
                bool expected = true;
                bool actual;
                target.IsOutputDetailsScreenVisible = expected;
                actual = target.IsOutputDetailsScreenVisible;
                Assert.AreEqual(expected, actual);
            }
        }

        /// <summary>
        /// A test for IsInputDetailsScreenVisible
        /// </summary>
        [TestMethod()]
        public void IsInputDetailsScreenVisibleTest()
        {
            using (TileGeneratorViewModel target = new TileGeneratorViewModel())
            {
                bool expected = false;
                bool actual;
                target.IsInputDetailsScreenVisible = expected;
                actual = target.IsInputDetailsScreenVisible;
                Assert.AreEqual(expected, actual);
            }
        }

        /// <summary>
        /// A test for IsGeneratePlate
        /// </summary>
        [TestMethod()]
        public void IsGeneratePlateTest()
        {
            using (TileGeneratorViewModel target = new TileGeneratorViewModel())
            {
                bool expected = true;
                bool actual;
                target.IsGeneratePlate = expected;
                actual = target.IsGeneratePlate;
                Assert.AreEqual(expected, actual);
            }
        }

        /// <summary>
        /// A test for IsCreateImageScreenVisible
        /// </summary>
        [TestMethod()]
        public void IsCreateImageScreenVisibleTest()
        {
            using (TileGeneratorViewModel target = new TileGeneratorViewModel())
            {
                bool expected = true;
                bool actual;
                target.IsCreateImageScreenVisible = expected;
                actual = target.IsCreateImageScreenVisible;
                Assert.AreEqual(expected, actual);
            }
        }

        /// <summary>
        /// A test for IsCompletedPanelVisible
        /// </summary>
        [TestMethod()]
        public void IsCompletedPanelVisibleTest()
        {
            using (TileGeneratorViewModel target = new TileGeneratorViewModel())
            {
                bool expected = false;
                bool actual;
                target.IsCompletedPanelVisible = expected;
                actual = target.IsCompletedPanelVisible;
                Assert.AreEqual(expected, actual);
            }
        }

        /// <summary>
        /// A test for IsCloseEnabled
        /// </summary>
        [TestMethod()]
        public void IsCloseEnabledTest()
        {
            using (TileGeneratorViewModel target = new TileGeneratorViewModel())
            {
                bool expected = true;
                bool actual;
                target.IsCloseVisible = expected;
                actual = target.IsCloseVisible;
                Assert.AreEqual(expected, actual);
            }
        }

        /// <summary>
        /// A test for IsCancelEnabled
        /// </summary>
        [TestMethod()]
        public void IsCancelEnabledLayerDetailsViewModel()
        {
            using (TileGeneratorViewModel target = new TileGeneratorViewModel())
            {
                bool expected = true;
                bool actual;
                target.IsCancelVisible = expected;
                actual = target.IsCancelVisible;
                Assert.AreEqual(expected, actual);
            }
        }

        /// <summary>
        /// A test for CreditsURL
        /// </summary>
        [TestMethod()]
        public void CreditsURLLayerDetailsViewModel()
        {
            using (TileGeneratorViewModel target = new TileGeneratorViewModel())
            {
                Uri expected = new Uri("https://google.com");
                Uri actual;
                target.CreditsURL = expected;
                actual = target.CreditsURL;
                Assert.AreEqual(expected, actual);
            }
        }

        /// <summary>
        /// A test for Credits
        /// </summary>
        [TestMethod()]
        public void CreditsTest()
        {
            using (TileGeneratorViewModel target = new TileGeneratorViewModel())
            {
                string expected = "2 credits";
                string actual;
                target.Credits = expected;
                actual = target.Credits;
                Assert.AreEqual(expected, actual);
            }
        }

        /// <summary>
        /// A test for ErrorMessage
        /// </summary>
        [TestMethod()]
        public void ErrorMessageTest()
        {
            using (TileGeneratorViewModel target = new TileGeneratorViewModel())
            {
                string expected = "error message";
                string actual;
                target.ErrorMessage = expected;
                actual = target.ErrorMessage;
                Assert.AreEqual(expected, actual);
            }
        }

        /// <summary>
        /// A test for InputImageDetails
        /// </summary>
        [TestMethod()]
        public void InputImageDetailsTest()
        {
            using (TileGeneratorViewModel target = new TileGeneratorViewModel())
            {
                InputImageDetailsViewModel expected = new InputImageDetailsViewModel();
                InputImageDetailsViewModel actual;
                target.InputImageDetails = expected;
                actual = target.InputImageDetails;
                Assert.AreEqual(expected, actual);
            }
        }

        /// <summary>
        /// A test for InvalidOutputPathErrorMessage
        /// </summary>
        [TestMethod()]
        public void InvalidOutputPathErrorMessageTest()
        {
            using (TileGeneratorViewModel target = new TileGeneratorViewModel())
            {
                string expected = "error message";
                string actual;
                target.InvalidOutputPathErrorMessage = expected;
                actual = target.InvalidOutputPathErrorMessage;
                Assert.AreEqual(expected, actual);
            }
        }

        /// <summary>
        /// A test for InvalidOutputFileNameErrorMessage
        /// </summary>
        [TestMethod()]
        public void InvalidOutputFileNameErrorMessageTest()
        {
            using (TileGeneratorViewModel target = new TileGeneratorViewModel())
            {
                string expected = "error message";
                string actual;
                target.InvalidOutputFileNameErrorMessage = expected;
                actual = target.InvalidOutputFileNameErrorMessage;
                Assert.AreEqual(expected, actual);
            }
        }

        /// <summary>
        /// A test for ValidateInputDetails
        /// </summary>
        [TestMethod()]
        public void ValidateInputDetailsTest()
        {
            using (TileGeneratorViewModel_Accessor target = new TileGeneratorViewModel_Accessor())
            {
                target.inputImageDetails.TopLeftLatitude = "90";
                target.inputImageDetails.BottomRightLatitude = "89";
                target.inputImageDetails.TopLeftLongitude = "120";
                target.inputImageDetails.BottomRightLongitude = "111";
                target.inputImageDetails.InputImagePath = Environment.CurrentDirectory + @"\Image\L0X0Y0.Png";
                target.inputImageDetails.SelectedInputProjectionType = InputProjections.Mercator;

                target.ValidateInputDetails();
                Assert.AreEqual(target.inputImageDetails.ImageName, target.OutputFileName);
                Assert.AreEqual(true, target.IsInputDetailsScreenVisible);
                Assert.AreEqual(false, target.IsOutputDetailsScreenVisible);
                Assert.AreEqual(false, target.IsOutputDetailsScreenVisible);
                Assert.AreEqual(false, target.IsOutputProjectionEnabled);
            }
        }

        /// <summary>
        /// A test for SetSelectedOutputProjectionTypes
        /// </summary>
        [TestMethod()]
        public void SetSelectedOutputProjectionTypesTest()
        {
            using (TileGeneratorViewModel_Accessor target = new TileGeneratorViewModel_Accessor())
            {
                InputProjections inputProjection = InputProjections.EquiRectangular;
                bool expected = true;
                target.SetSelectedOutputProjectionTypes(inputProjection);
                Assert.AreEqual(expected, target.IsOutputProjectionEnabled);
                Assert.AreNotEqual(inputProjection, target.SelectedOutputProjectionType);
            }
        }

        /// <summary>
        /// A test for PopulateProjectionTypes
        /// </summary>
        [TestMethod()]
        public void PopulateProjectionTypesTest()
        {
            using (TileGeneratorViewModel_Accessor target = new TileGeneratorViewModel_Accessor())
            {
                Collection<ProjectionTypes> expected = new Collection<ProjectionTypes>();
                expected.Add(ProjectionTypes.Toast);
                expected.Add(ProjectionTypes.Mercator);
                target.PopulateProjectionTypes();
                int index = 0;
                foreach (ProjectionTypes projectionType in expected)
                {
                    Assert.AreEqual(projectionType, target.outputProjectionTypes[index]);
                    index++;
                }
            }
        }

        /// <summary>
        /// A test for PopulateLevels
        /// </summary>
        [TestMethod()]
        public void PopulateLevelsTest()
        {
            using (TileGeneratorViewModel_Accessor target = new TileGeneratorViewModel_Accessor())
            {
                int maxLevel = 2;
                Collection<string> expected = new Collection<string>();
                for (int level = maxLevel; level >= 0; level--)
                {
                    expected.Add(level.ToString(CultureInfo.InvariantCulture));
                }
                target.PopulateLevels(maxLevel);
                int index = 0;
                foreach (string level in expected)
                {
                    Assert.AreEqual(level, target.Levels[index]);
                    index++;
                }
            }
        }

        /// <summary>
        /// A test for IsValidFolder
        /// </summary>
        [TestMethod()]
        public void IsValidFolderTest()
        {
            using (TileGeneratorViewModel_Accessor target = new TileGeneratorViewModel_Accessor())
            {
                string outputPath = Environment.CurrentDirectory + @"\Image\L0X0Y0.Png";
                bool expected = true;
                bool actual;
                actual = target.IsValidFolder(outputPath);
                Assert.AreEqual(expected, actual);
            }
        }

        /// <summary>
        /// A test for IsValidFolder with empty data
        /// </summary>
        [TestMethod()]
        public void IsValidFolderEmptyTest()
        {
            using (TileGeneratorViewModel_Accessor target = new TileGeneratorViewModel_Accessor())
            {
                string outputPath = string.Empty;
                bool expected = false;
                bool actual;
                actual = target.IsValidFolder(outputPath);
                Assert.AreEqual(expected, actual);
            }
        }

        /// <summary>
        /// A test for IsValidFileName
        /// </summary>
        [TestMethod()]
        public void IsValidFileNameTest()
        {
            using (TileGeneratorViewModel_Accessor target = new TileGeneratorViewModel_Accessor())
            {
                string fileName = "L0X0Y0.Png";
                bool expected = true;
                bool actual;
                actual = target.IsValidFileName(fileName);
                Assert.AreEqual(expected, actual);
                Assert.AreEqual(string.Empty, target.InvalidOutputFileNameErrorMessage);
            }
        }

        /// <summary>
        /// A test for IsValidFileName empty data
        /// </summary>
        [TestMethod()]
        public void IsValidFileNameEmptyTest()
        {
            using (TileGeneratorViewModel_Accessor target = new TileGeneratorViewModel_Accessor())
            {
                string fileName = string.Empty;
                bool expected = false;
                bool actual;
                actual = target.IsValidFileName(fileName);
                Assert.AreEqual(expected, actual);
                Assert.AreEqual(Properties.Resources.MandatoryError, target.InvalidOutputFileNameErrorMessage);
            }
        }

        /// <summary>
        /// A test for IsValidFileName invalid data
        /// </summary>
        [TestMethod()]
        public void IsValidFileNameInvalidTest()
        {
            using (TileGeneratorViewModel_Accessor target = new TileGeneratorViewModel_Accessor())
            {
                string fileName = Environment.CurrentDirectory + @"\Image\<>fff.Png";
                bool expected = false;
                bool actual;
                actual = target.IsValidFileName(fileName);
                Assert.AreEqual(expected, actual);
                Assert.AreEqual(Properties.Resources.OutputFilenameInvalidCharacterError, target.InvalidOutputFileNameErrorMessage);
            }
        }

        /// <summary>
        /// A test for GetTotalTiles
        /// </summary>
        [TestMethod()]
        public void GetTotalTilesTest()
        {
            using (TileGeneratorViewModel_Accessor target = new TileGeneratorViewModel_Accessor())
            {
                int level = 0;
                long expected = 1;
                long actual;
                actual = target.GetTotalTiles(level);
                Assert.AreEqual(expected, actual);
            }
        }

        /// <summary>
        /// A test for GetTotalTiles
        /// </summary>
        [TestMethod()]
        public void GetTotalLevelOneTest()
        {
            using (TileGeneratorViewModel_Accessor target = new TileGeneratorViewModel_Accessor())
            {
                int level = 1;
                long expected = 5;
                long actual;
                actual = target.GetTotalTiles(level);
                Assert.AreEqual(expected, actual);
            }
        }

        /// <summary>
        /// A test for CreateImage
        /// </summary>
        [TestMethod()]
        public void CreateImageLayerDetailsViewModel()
        {
            using (TileGeneratorViewModel_Accessor target = new TileGeneratorViewModel_Accessor())
            {
                target.outputFileName = "L0X0Y0";
                target.outputFolderPath = Environment.CurrentDirectory + @"\output";
                target.generatePyramidsStopwatch = new Stopwatch();
                target.SelectedOutputProjectionType = ProjectionTypes.Toast;
                target.IsGeneratePlate = true;
                target.SelectedLevel = "0";
                target.InputImageDetails.InputImagePath = Environment.CurrentDirectory + @"\Image\L0X0Y0.Png";
                target.InputImageDetails.TopLeftLatitude = "90";
                target.InputImageDetails.TopLeftLongitude = "-180";
                target.InputImageDetails.BottomRightLatitude = "-90";
                target.InputImageDetails.BottomRightLongitude = "180";

                target.CreateImage();
                Assert.AreEqual(false, target.IsOutputDetailsScreenVisible);
                Assert.AreEqual(true, target.IsCreateImageScreenVisible);
                Assert.AreEqual(false, target.IsStartOverVisible);
                Assert.AreEqual(true, target.IsCloseVisible);
                Assert.AreEqual(false, target.IsRestartVisible);
                Assert.AreEqual(true, target.IsCancelVisible);
            }
        }

        /// <summary>
        /// A test for CreateImage
        /// </summary>
        [TestMethod()]
        public void CreateImageLayerDetailsViewModelMercator()
        {
            using (TileGeneratorViewModel_Accessor target = new TileGeneratorViewModel_Accessor())
            {
                target.outputFileName = "L0X0Y0";
                target.outputFolderPath = Environment.CurrentDirectory + @"\output";
                target.generatePyramidsStopwatch = new Stopwatch();
                target.SelectedOutputProjectionType = ProjectionTypes.Mercator;
                target.IsGeneratePlate = false;
                target.SelectedLevel = "0";
                target.InputImageDetails.InputImagePath = Environment.CurrentDirectory + @"\Image\L0X0Y0.Png";
                target.InputImageDetails.TopLeftLatitude = "90";
                target.InputImageDetails.TopLeftLongitude = "-180";
                target.InputImageDetails.BottomRightLatitude = "-90";
                target.InputImageDetails.BottomRightLongitude = "180";

                target.CreateImage();
                Assert.AreEqual(false, target.IsOutputDetailsScreenVisible);
                Assert.AreEqual(true, target.IsCreateImageScreenVisible);
                Assert.AreEqual(false, target.IsStartOverVisible);
                Assert.AreEqual(true, target.IsCloseVisible);
                Assert.AreEqual(false, target.IsRestartVisible);
                Assert.AreEqual(true, target.IsCancelVisible);
            }
        }

        /// <summary>
        /// A test for ErrorPopupClose
        /// </summary>
        [TestMethod()]
        public void ErrorPopupCloseTest()
        {
            using (TileGeneratorViewModel_Accessor target = new TileGeneratorViewModel_Accessor())
            {
                target.ErrorPopupClose();
                bool expected = false;
                Assert.AreEqual(expected, target.inputImageDetails.IsErrorWindowVisible);
            }
        }

        /// <summary>
        /// A test for Initialize
        /// </summary>
        [TestMethod()]
        public void InitializeTest()
        {
            using (TileGeneratorViewModel_Accessor target = new TileGeneratorViewModel_Accessor())
            {
                Collection<ProjectionTypes> expected = new Collection<ProjectionTypes>();
                expected.Add(ProjectionTypes.Toast);
                expected.Add(ProjectionTypes.Mercator);
                target.Initialize();
                int index = 0;
                foreach (ProjectionTypes projectionType in expected)
                {
                    Assert.AreEqual(projectionType, target.outputProjectionTypes[index]);
                    index++;
                }
                Assert.AreEqual(true, target.IsInputDetailsScreenVisible);
            }
        }

        /// <summary>
        /// A test for BackToInputDetails
        /// </summary>
        [TestMethod()]
        public void BackToInputDetailsTest()
        {
            using (TileGeneratorViewModel_Accessor target = new TileGeneratorViewModel_Accessor())
            {
                target.BackToInputDetails();
                Assert.AreEqual(false, target.IsOutputDetailsScreenVisible);
                Assert.AreEqual(true, target.IsInputDetailsScreenVisible);
            }
        }

        /// <summary>
        /// A test for OnCanceled
        /// </summary>
        [TestMethod()]
        public void OnCanceledTest()
        {
            using (TileGeneratorViewModel_Accessor target = new TileGeneratorViewModel_Accessor())
            {
                target.generatePyramidsStopwatch = new Stopwatch();
                target.generatePyramidsStopwatch.Start();

                object sender = null;
                CancelEventArgs e = new CancelEventArgs(true);
                target.OnCanceled(sender, e);
                Assert.AreEqual(true, target.IsStartOverVisible);
                Assert.AreEqual(true, target.IsCloseVisible);
                Assert.AreEqual(true, target.IsBackToOutputVisible);
                Assert.AreEqual(true, target.IsRestartVisible);
                Assert.AreEqual(false, target.IsCancelVisible);
                Assert.AreEqual(false, target.generatePyramidsStopwatch.IsRunning);

                Assert.AreEqual(Properties.Resources.UserCancelledErrorMessage, target.PyramidGenerationMessage);
            }
        }

        /// <summary>
        /// A test for OnCanceled
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2201:DoNotRaiseReservedExceptionTypes", Justification = "Negative test case."), TestMethod()]
        public void OnErrorTest()
        {
            using (TileGeneratorViewModel_Accessor target = new TileGeneratorViewModel_Accessor())
            {
                target.generatePyramidsStopwatch = new Stopwatch();
                target.generatePyramidsStopwatch.Start();

                object sender = null;
                ErrorEventArgs e = new ErrorEventArgs();
                e.Error = new Exception();
                target.OnError(sender, e);
                Assert.AreEqual(true, target.IsStartOverVisible);
                Assert.AreEqual(true, target.IsCloseVisible);
                Assert.AreEqual(true, target.IsBackToOutputVisible);
                Assert.AreEqual(true, target.IsRestartVisible);
                Assert.AreEqual(false, target.IsCancelVisible);

                Assert.AreEqual(false, target.generatePyramidsStopwatch.IsRunning);
                Assert.AreEqual(Properties.Resources.PyramidGenerationErrorMessage, target.PyramidGenerationMessage);
            }
        }

        /// <summary>
        /// A test to get estimated time remaining
        /// </summary>
        [TestMethod()]
        public void GetEstimatedTimeRemainingTest()
        {
            using (TileGeneratorViewModel_Accessor target = new TileGeneratorViewModel_Accessor())
            {
                long processedTiles = 5;
                long processedPlateTile = -1;
                long timeTaken = 2;
                target.IsGeneratePlate = true;
                target.totalTiles = 7;
                long expected = 1;
                long estimatedTime = target.GetEstimatedTimeRemaining(processedTiles, processedPlateTile, timeTaken);
                Assert.AreEqual(expected, estimatedTime);
            }
        }

        /// <summary>
        /// A test for on tick 
        /// </summary>
        [TestMethod()]
        public void OnTickTest()
        {
            using (TileGeneratorViewModel_Accessor target = new TileGeneratorViewModel_Accessor())
            {
                object sender = null;
                EventArgs e = new EventArgs();
                target.OnTick(sender, e);
                Assert.AreEqual(Properties.Resources.CalculatingLabel, target.TotalTimeRemaining);
            }
        }
    }
}
