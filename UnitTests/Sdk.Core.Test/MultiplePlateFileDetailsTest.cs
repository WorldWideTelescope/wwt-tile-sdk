//-----------------------------------------------------------------------
// <copyright file="MultiplePlateFileDetailsTest.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation 2011. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.Research.Wwt.Sdk.Core.Test
{
    /// <summary>
    /// This is a test class for MultiplePlateFileDetailsTest and is intended
    /// to contain all MultiplePlateFileDetailsTest Unit Tests
    /// </summary>
    [TestClass()]
    public class MultiplePlateFileDetailsTest
    {
        /// <summary>
        /// A test for MultiplePlateFileDetails Constructor
        /// </summary>
        [TestMethod()]
        public void MultiplePlateFileDetailsConstructorTestLevel5()
        {
            int maxLevel = 5;
            MultiplePlateFileDetails target = new MultiplePlateFileDetails(maxLevel);

            Assert.AreEqual(4, target.LevelsPerPlate);
            Assert.AreEqual(3, target.MaxOverlappedLevel);
            Assert.AreEqual(2, target.MinOverlappedLevel);
            Assert.AreEqual(2, target.TotalOverlappedLevels);
        }

        /// <summary>
        /// A test for MultiplePlateFileDetails Constructor
        /// </summary>
        [TestMethod()]
        public void MultiplePlateFileDetailsConstructorTestLevel18()
        {
            int maxLevel = 18;
            MultiplePlateFileDetails target = new MultiplePlateFileDetails(maxLevel);

            Assert.AreEqual(10, target.LevelsPerPlate);
            Assert.AreEqual(9, target.MaxOverlappedLevel);
            Assert.AreEqual(9, target.MinOverlappedLevel);
            Assert.AreEqual(1, target.TotalOverlappedLevels);
        }
    }
}
