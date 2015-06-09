//-----------------------------------------------------------------------
// <copyright file="DataGridTest.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation 2011. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.Research.Wwt.Sdk.Core.Test
{
    /// <summary>
    /// This is a test class for DataGridTest and is intended
    /// to contain all DataGridTest Unit Tests
    /// </summary>
    [TestClass()]
    public class DataGridTest
    {
        /// <summary>
        /// A test for GetValue
        /// </summary>
        [TestMethod()]
        public void GetValueTest()
        {
            double[][] inputData = new double[][] { new double[] { 0, 1, 2 }, new double[] { 3, 4, 5 }, new double[] { 5, 6, 7 } };
            bool isCircular = false;
            DataGrid target = new DataGrid(inputData, isCircular);
            Assert.AreEqual(4, target.GetValue(0.5, 0.5));
            Assert.AreEqual(3, target.GetValueAt(0, 1));
            Assert.AreEqual(1, target.GetXIndex(0.5));
            Assert.AreEqual(0, target.GetYIndex(0.4));
        }
    }
}
