//-----------------------------------------------------------------------
// <copyright file="InputImageDetailsViewModelTest.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation 2011. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.ObjectModel;
using System.Drawing.Imaging;
using System.Globalization;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.Research.Wwt.TileGenerator.Test
{
    /// <summary>
    /// This is a test class for InputImageDetailsViewModel and is intended
    /// to contain all InputImageDetailsViewModel Unit Tests
    /// </summary>
    [TestClass()]
    public class InputImageDetailsViewModelTest
    {
        /// <summary>
        /// A test for TopLeftLatitude
        /// </summary>
        [TestMethod()]
        public void TopLeftLatitudeTest()
        {
            InputImageDetailsViewModel_Accessor target = new InputImageDetailsViewModel_Accessor();
            string expected = "90";
            string actual;
            target.topLeftLatitude = expected;
            actual = target.TopLeftLatitude;
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// A test for TopLeftLatitude
        /// </summary>
        [TestMethod()]
        public void TopLeftLatitudeNegative()
        {
            InputImageDetailsViewModel_Accessor target = new InputImageDetailsViewModel_Accessor();
            string expected = "120";
            string actual;
            target.topLeftLatitude = expected;
            actual = target.TopLeftLatitude;
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// A test for SelectedInputProjectionType
        /// </summary>
        [TestMethod()]
        public void SelectedInputProjectionTypeTest()
        {
            InputImageDetailsViewModel_Accessor target = new InputImageDetailsViewModel_Accessor();
            InputProjections expected = InputProjections.EquiRectangular;
            InputProjections actual;
            target.selectedInputProjectionType = expected;
            actual = target.SelectedInputProjectionType;
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// A test for TopLeftLongitude
        /// </summary>
        [TestMethod()]
        public void TopLeftLongitudeTest()
        {
            InputImageDetailsViewModel_Accessor target = new InputImageDetailsViewModel_Accessor();
            string expected = "91";
            string actual;
            target.topLeftLongitude = expected;
            actual = target.TopLeftLongitude;
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// A test for TopLeftLongitude
        /// </summary>
        [TestMethod()]
        public void TopLeftLongitudeNegativeTest()
        {
            InputImageDetailsViewModel_Accessor target = new InputImageDetailsViewModel_Accessor();
            string expected = "-191";
            string actual;
            target.topLeftLongitude = expected;
            actual = target.TopLeftLongitude;
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// A test for BottomRightLatitude
        /// </summary>
        [TestMethod()]
        public void BottomRightLatitudeTest()
        {
            InputImageDetailsViewModel_Accessor target = new InputImageDetailsViewModel_Accessor();
            string expected = "90";
            string actual;
            target.bottomRightLatitude = expected;
            actual = target.BottomRightLatitude;
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// A test for BottomRightLatitude
        /// </summary>
        [TestMethod()]
        public void BottomRightLatitudeNegativeTest()
        {
            InputImageDetailsViewModel_Accessor target = new InputImageDetailsViewModel_Accessor();
            string expected = "-91";
            string actual;
            target.bottomRightLatitude = expected;
            actual = target.BottomRightLatitude;
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// A test for IsErrorWindowVisible
        /// </summary>
        [TestMethod()]
        public void IsErrorWindowVisibleLayerDetailsViewModel()
        {
            InputImageDetailsViewModel_Accessor target = new InputImageDetailsViewModel_Accessor();
            bool expected = false;
            bool actual;
            target.isErrorWindowVisible = expected;
            actual = target.IsErrorWindowVisible;
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// A test for InvalidTopLeftLongitudeErrorMessage
        /// </summary>
        [TestMethod()]
        public void InvalidTopLeftLongitudeErrorMessageTest()
        {
            InputImageDetailsViewModel_Accessor target = new InputImageDetailsViewModel_Accessor();
            string expected = "Please provide proper data";
            string actual;
            target.invalidTopLeftLongitudeErrorMessage = expected;
            actual = target.InvalidTopLeftLongitudeErrorMessage;
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// A test for InvalidTopLeftLatitudeErrorMessage
        /// </summary>
        [TestMethod()]
        public void InvalidTopLeftLatitudeErrorMessageTest()
        {
            InputImageDetailsViewModel_Accessor target = new InputImageDetailsViewModel_Accessor();
            string expected = "Please provide proper data";
            string actual;
            target.invalidTopLeftLatitudeErrorMessage = expected;
            actual = target.InvalidTopLeftLatitudeErrorMessage;
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// A test for InvalidFolderPathErrorMessage
        /// </summary>
        [TestMethod()]
        public void InvalidFolderPathErrorMessageTest()
        {
            InputImageDetailsViewModel_Accessor target = new InputImageDetailsViewModel_Accessor();
            string expected = "Please provide proper data";
            string actual;
            target.invalidFolderPathErrorMessage = expected;
            actual = target.InvalidFolderPathErrorMessage;
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// A test for InvalidBoundaryLongValErrorMessage
        /// </summary>
        [TestMethod()]
        public void InvalidBoundaryLongValErrorMessageTest()
        {
            InputImageDetailsViewModel_Accessor target = new InputImageDetailsViewModel_Accessor();
            string expected = "Please provide proper data";
            string actual;
            target.invalidBoundaryLongValErrorMessage = expected;
            actual = target.InvalidBoundaryLongValErrorMessage;
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// A test for InvalidBoundaryLatValErrorMessage
        /// </summary>
        [TestMethod()]
        public void InvalidBoundaryLatValErrorMessageTest()
        {
            InputImageDetailsViewModel_Accessor target = new InputImageDetailsViewModel_Accessor();
            string expected = "Please provide proper data";
            string actual;
            target.invalidBoundaryLatValErrorMessage = expected;
            actual = target.InvalidBoundaryLatValErrorMessage;
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// A test for InvalidBottomRightLongitudeErrorMessage
        /// </summary>
        [TestMethod()]
        public void InvalidBottomRightLongitudeErrorMessageTest()
        {
            InputImageDetailsViewModel_Accessor target = new InputImageDetailsViewModel_Accessor();
            string expected = "Please provide proper data";
            string actual;
            target.invalidBottomRightLongitudeErrorMessage = expected;
            actual = target.InvalidBottomRightLongitudeErrorMessage;
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// A test for InvalidBottomRightLatitudeErrorMessage
        /// </summary>
        [TestMethod()]
        public void InvalidBottomRightLatitudeErrorMessageTest()
        {
            InputImageDetailsViewModel_Accessor target = new InputImageDetailsViewModel_Accessor();
            string expected = "Please provide proper data";
            string actual;
            target.invalidBottomRightLatitudeErrorMessage = expected;
            actual = target.InvalidBottomRightLatitudeErrorMessage;
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// A test for InputProjectionTypes
        /// </summary>
        [TestMethod()]
        public void InputProjectionTypesTest()
        {
            InputImageDetailsViewModel_Accessor target = new InputImageDetailsViewModel_Accessor();
            target.inputProjectionTypes = new ObservableCollection<InputProjections>();
            target.inputProjectionTypes.Add(InputProjections.EquiRectangular);
            target.inputProjectionTypes.Add(InputProjections.Mercator);
            ObservableCollection<InputProjections> actual;
            actual = target.InputProjectionTypes;
            Assert.AreEqual(target.inputProjectionTypes.Count, actual.Count);
            int index = 0;
            foreach (InputProjections projectionTypes in target.inputProjectionTypes)
            {
                Assert.AreEqual(projectionTypes, actual[index]);
                index++;
            }
        }

        /// <summary>
        /// A test for ImageName
        /// </summary>
        [TestMethod()]
        public void ImageNameTest()
        {
            InputImageDetailsViewModel_Accessor target = new InputImageDetailsViewModel_Accessor();
            string expected = "Image1";
            string actual;
            target.ImageName = expected;
            actual = target.ImageName;
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// A test for ImageDimension
        /// </summary>
        [TestMethod()]
        public void ImageDimensionTest()
        {
            InputImageDetailsViewModel target = new InputImageDetailsViewModel();
            string expected = "100x100";
            string actual;
            target.ImageDimension = expected;
            actual = target.ImageDimension;
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// A test for ErrorMessage
        /// </summary>
        [TestMethod()]
        public void ErrorMessageLayerDetailsViewModel()
        {
            InputImageDetailsViewModel_Accessor target = new InputImageDetailsViewModel_Accessor();
            string expected = "Please provide proper data";
            string actual;
            target.errorMessage = expected;
            actual = target.ErrorMessage;
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// A test for BottomRightLongitude
        /// </summary>
        [TestMethod()]
        public void BottomRightLongitudeLayerDetailsViewModel()
        {
            InputImageDetailsViewModel_Accessor target = new InputImageDetailsViewModel_Accessor();
            string expected = "90";
            string actual;
            target.bottomRightLongitude = expected;
            actual = target.BottomRightLongitude;
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// A test for ValidateLatitude for positive data
        /// </summary>
        [TestMethod()]
        public void ValidateLatitudePositiveTest()
        {
            InputImageDetailsViewModel_Accessor target = new InputImageDetailsViewModel_Accessor();
            string latitude = "90";
            Position position = Position.TopLeft;
            bool expected = true;
            bool actual;
            actual = target.ValidateLatitude(latitude, position);
            Assert.AreEqual(expected, actual);
            Assert.AreEqual(string.Empty, target.InvalidTopLeftLatitudeErrorMessage);
        }

        /// <summary>
        /// A test for ValidateLatitude for empty data
        /// </summary>
        [TestMethod()]
        public void ValidateLatitudeEmptyTest()
        {
            InputImageDetailsViewModel_Accessor target = new InputImageDetailsViewModel_Accessor();
            string latitude = string.Empty;
            Position position = Position.BottomRight;
            bool expected = false;
            bool actual;
            actual = target.ValidateLatitude(latitude, position);
            Assert.AreEqual(expected, actual);
            Assert.AreEqual(Properties.Resources.MandatoryError, target.InvalidBottomRightLatitudeErrorMessage);
        }

        /// <summary>
        /// A test for ValidateLatitude for invalid data
        /// </summary>
        [TestMethod()]
        public void ValidateLatitudeInvalidDataTest()
        {
            InputImageDetailsViewModel_Accessor target = new InputImageDetailsViewModel_Accessor();
            string latitude = "91";
            Position position = Position.BottomRight;
            bool expected = false;
            bool actual;
            actual = target.ValidateLatitude(latitude, position);
            Assert.AreEqual(expected, actual);
            Assert.AreEqual(Properties.Resources.InvalidLatitudeError, target.InvalidBottomRightLatitudeErrorMessage);
        }

        /// <summary>
        /// A test for ValidateLatBoundaryValues
        /// </summary>
        [TestMethod()]
        public void ValidateLatBoundaryValuesPositiveTest()
        {
            InputImageDetailsViewModel_Accessor target = new InputImageDetailsViewModel_Accessor();
            string topLeft = "90";
            string bottomRight = "89";
            bool expected = true;
            bool actual;
            actual = target.ValidateLatBoundaryValues(topLeft, bottomRight);
            Assert.AreEqual(expected, actual);
            Assert.AreEqual(string.Empty, target.InvalidBoundaryLatValErrorMessage);
        }

        /// <summary>
        /// A test for ValidateLatBoundaryValues for invalid data
        /// </summary>
        [TestMethod()]
        public void ValidateLatBoundaryValuesInvalidTest()
        {
            InputImageDetailsViewModel_Accessor target = new InputImageDetailsViewModel_Accessor();
            string topLeft = "90";
            string bottomRight = "90";
            bool expected = false;
            bool actual;
            actual = target.ValidateLatBoundaryValues(topLeft, bottomRight);
            Assert.AreEqual(expected, actual);
            Assert.AreEqual(Properties.Resources.InvalidBoundaryLatValError, target.InvalidBoundaryLatValErrorMessage);
        }

        /// <summary>
        /// A test for ValidateLongBoundaryValues
        /// </summary>
        [TestMethod()]
        public void ValidateLongBoundaryValuesPositiveTest()
        {
            InputImageDetailsViewModel_Accessor target = new InputImageDetailsViewModel_Accessor();
            string topLeft = "90";
            string bottomRight = "89";
            bool expected = true;
            bool actual;
            actual = target.ValidateLongBoundaryValues(topLeft, bottomRight);
            Assert.AreEqual(expected, actual);
            Assert.AreEqual(string.Empty, target.InvalidBoundaryLongValErrorMessage);
        }

        /// <summary>
        /// A test for ValidateLongBoundaryValues for invalid data
        /// </summary>
        [TestMethod()]
        public void ValidateLongBoundaryValuesInvalidTest()
        {
            InputImageDetailsViewModel_Accessor target = new InputImageDetailsViewModel_Accessor();
            string topLeft = "90";
            string bottomRight = "90";
            bool expected = false;
            bool actual;
            actual = target.ValidateLongBoundaryValues(topLeft, bottomRight);
            Assert.AreEqual(expected, actual);
            Assert.AreEqual(Properties.Resources.InvalidBoundaryLongValError, target.InvalidBoundaryLongValErrorMessage);
        }

        /// <summary>
        /// A test for ValidateLongitude
        /// </summary>
        [TestMethod()]
        public void ValidateLongitudePositiveTest()
        {
            InputImageDetailsViewModel_Accessor target = new InputImageDetailsViewModel_Accessor();
            string longitude = "91";
            Position position = Position.TopLeft;
            bool expected = true;
            bool actual;
            actual = target.ValidateLongitude(longitude, position);
            Assert.AreEqual(expected, actual);
            Assert.AreEqual(string.Empty, target.InvalidTopLeftLongitudeErrorMessage);
        }

        /// <summary>
        /// A test for ValidateLongitude empty data
        /// </summary>
        [TestMethod()]
        public void ValidateLongitudeEmptyTest()
        {
            InputImageDetailsViewModel_Accessor target = new InputImageDetailsViewModel_Accessor();
            string longitude = string.Empty;
            Position position = Position.BottomRight;
            bool expected = false;
            bool actual;
            actual = target.ValidateLongitude(longitude, position);
            Assert.AreEqual(expected, actual);
            Assert.AreEqual(Properties.Resources.MandatoryError, target.InvalidBottomRightLongitudeErrorMessage);
        }

        /// <summary>
        /// A test for ValidateLongitude invalid data
        /// </summary>
        [TestMethod()]
        public void ValidateLongitudeInvalidTest()
        {
            InputImageDetailsViewModel_Accessor target = new InputImageDetailsViewModel_Accessor();
            string longitude = "190";
            Position position = Position.BottomRight;
            bool expected = false;
            bool actual;
            actual = target.ValidateLongitude(longitude, position);
            Assert.AreEqual(expected, actual);
            Assert.AreEqual(Properties.Resources.InvalidLongitudeError, target.InvalidBottomRightLongitudeErrorMessage);
        }

        /// <summary>
        /// A test for ValidateInputImagePath
        /// </summary>
        [TestMethod()]
        public void ValidateInputImagePathTest()
        {
            InputImageDetailsViewModel_Accessor target = new InputImageDetailsViewModel_Accessor();
            string imagePath = Environment.CurrentDirectory + @"\Image\L0X0Y0.Png";
            bool expected = true;
            bool actual;
            actual = target.ValidateInputImagePath(imagePath);
            Assert.AreEqual(expected, actual);
            Assert.AreEqual(target.InvalidFolderPathErrorMessage, string.Empty);
        }

        /// <summary>
        /// A test for ValidateInputImagePath for invalid data
        /// </summary>
        [TestMethod()]
        public void ValidateInputImagePathInvalidTest()
        {
            InputImageDetailsViewModel_Accessor target = new InputImageDetailsViewModel_Accessor();
            string imagePath = Environment.CurrentDirectory + @"\Test.Png";
            bool expected = false;
            bool actual;
            actual = target.ValidateInputImagePath(imagePath);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// A test for ValidateInputImagePath for empty data
        /// </summary>
        [TestMethod()]
        public void ValidateInputImagePathEmptyTest()
        {
            InputImageDetailsViewModel_Accessor target = new InputImageDetailsViewModel_Accessor();
            string imagePath = string.Empty;
            bool expected = false;
            bool actual;
            actual = target.ValidateInputImagePath(imagePath);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// A test for IsValidImage
        /// </summary>
        [TestMethod()]
        public void IsValidImageTest()
        {
            InputImageDetailsViewModel_Accessor target = new InputImageDetailsViewModel_Accessor();
            string filePath = Environment.CurrentDirectory + @"\Image\L0X0Y0.Png";
            bool expected = true;
            bool actual;
            actual = target.IsValidImage(filePath);
            Assert.AreEqual(expected, actual);
            Assert.AreEqual(string.Empty, target.InvalidFolderPathErrorMessage);
        }

        /// <summary>
        /// A test for IsValidImage for invalid data
        /// </summary>
        [TestMethod()]
        public void IsValidImageInvalidTest()
        {
            InputImageDetailsViewModel_Accessor target = new InputImageDetailsViewModel_Accessor();
            string filePath = Environment.CurrentDirectory + @"\L0X0Y0.txt";
            bool expected = false;
            bool actual;
            actual = target.IsValidImage(filePath);
            Assert.AreEqual(expected, actual);
            Assert.AreEqual(Properties.Resources.InvalidImageFormat, target.InvalidFolderPathErrorMessage);
        }

        /// <summary>
        /// A test for PopulateDefaultCoordinates
        /// </summary>
        [TestMethod()]
        public void PopulateDefaultCoordinatesTest()
        {
            InputImageDetailsViewModel_Accessor target = new InputImageDetailsViewModel_Accessor();
            target.PopulateDefaultCoordinates();
            Assert.AreEqual(Constants.BottomRightLatitude.ToString(CultureInfo.InvariantCulture), target.BottomRightLatitude);
            Assert.AreEqual(Constants.BottomRightLongitude.ToString(CultureInfo.InvariantCulture), target.BottomRightLongitude);
            Assert.AreEqual(Constants.TopLeftLatitude.ToString(CultureInfo.InvariantCulture), target.TopLeftLatitude);
            Assert.AreEqual(Constants.TopLeftLongitude.ToString(CultureInfo.InvariantCulture), target.TopLeftLongitude);
        }

        /// <summary>
        /// A test for PopulateImageFormats
        /// </summary>
        [TestMethod()]
        public void PopulateImageFormatsTest()
        {
            Collection<string> expected = new Collection<string>();
            expected.Add(ImageFormat.Jpeg.ToString().ToUpperInvariant());
            expected.Add(Constants.JpgImageFormat.ToUpperInvariant());
            expected.Add(Constants.TifImageFormat.ToUpperInvariant());
            expected.Add(ImageFormat.Tiff.ToString().ToUpperInvariant());
            expected.Add(ImageFormat.Png.ToString().ToUpperInvariant());
            Collection<string> actual;
            actual = InputImageDetailsViewModel_Accessor.PopulateImageFormats();
            Assert.AreEqual(expected.Count, actual.Count);
            int index = 0;
            foreach (string image in expected)
            {
                Assert.AreEqual(image, actual[index]);
                index++;
            }
        }

        /// <summary>
        /// A test for PopulateInputProjectionTypes
        /// </summary>
        [TestMethod()]
        public void PopulateInputProjectionTypesTest()
        {
            InputImageDetailsViewModel_Accessor target = new InputImageDetailsViewModel_Accessor();
            target.PopulateInputProjectionTypes();
            InputProjections[] expected = new InputProjections[] { InputProjections.EquiRectangular, InputProjections.Mercator };

            foreach (InputProjections projectionType in expected)
            {
                Assert.IsTrue(target.inputProjectionTypes.Contains(projectionType));
            }
        }

        /// <summary>
        /// A test for SetImageDimension
        /// </summary>
        [TestMethod()]
        public void SetImageDimensionTest()
        {
            InputImageDetailsViewModel_Accessor target = new InputImageDetailsViewModel_Accessor();
            string imagePath = Environment.CurrentDirectory + @"\Image\L0X0Y0.Png";
            bool expected = true;
            bool actual;
            actual = target.SetImageDimension(imagePath);
            Assert.AreEqual(expected, actual);
            Assert.AreEqual("256x256", target.imageDimension);
        }
    }
}
