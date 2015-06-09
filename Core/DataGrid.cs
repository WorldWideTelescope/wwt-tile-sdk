//-----------------------------------------------------------------------
// <copyright file="DataGrid.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation 2011. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;

namespace Microsoft.Research.Wwt.Sdk.Core
{
    /// <summary>
    /// Represents the data grid for data set in two dimensional array format. 
    /// The first dimension represents the grid height and second represents grid width.
    /// </summary>
    public class DataGrid : IGrid
    {
        /// <summary>
        /// Values contained in the data set.
        /// </summary>
        private double[][] gridData;

        /// <summary>
        /// Width of the grid.
        /// </summary>
        private int gridWidth;

        /// <summary>
        /// Height of the grid.
        /// </summary>
        private int gridHeight;

        /// <summary>
        /// nu variable for interpolation
        /// </summary>
        private int nu;

        /// <summary>
        /// nv variable for interpolation
        /// </summary>
        private int nv;

        /// <summary>
        /// du variable for interpolation
        /// </summary>
        private double du;

        /// <summary>
        /// dv variable for interpolation
        /// </summary>
        private double dv;

        /// <summary>
        /// If the grid is circular with right edge and left edge of the grid
        /// representing the same boundary
        /// </summary>
        private bool circular;

        /// <summary>
        /// Initializes a new instance of the DataGrid class.
        /// </summary>
        /// <param name="inputData">
        /// Input data in 2 Dimensional array format.
        /// The first dimension represents the grid height and second represents grid width.
        /// </param>
        /// <param name="isCircular">
        /// If the grid is circular with right edge and left edge of the grid
        /// representing the same boundary
        /// </param>
        public DataGrid(double[][] inputData, bool isCircular)
        {
            if (inputData == null)
            {
                throw new ArgumentNullException("inputData");
            }

            this.gridData = inputData;
            this.gridHeight = inputData.GetLength(0);
            this.gridWidth = inputData[0].Length;
            nu = this.gridWidth - 1;
            nv = this.gridHeight - 1;
            du = 1.0 / this.gridWidth;
            dv = 1.0 / this.gridHeight;
            circular = isCircular;
        }

        /// <summary>
        /// Gets the Height of the grid.
        /// </summary>
        public int Height
        {
            get
            {
                return this.gridHeight;
            }
        }

        /// <summary>
        /// Gets the Width of the grid.
        /// </summary>
        public int Width
        {
            get
            {
                return this.gridWidth;
            }
        }

        /// <summary>
        /// Gets the value for the specified u and v co-ordinates in the grid.
        /// </summary>
        /// <param name="u">
        /// U co-ordinate.
        /// </param>
        /// <param name="v">
        /// V co-ordinate.
        /// </param>
        /// <returns>
        /// Value for the specified u and v co-ordinates in the grid.
        /// </returns>
        public double GetValue(double u, double v)
        {
            int idash = (int)Math.Floor((u / du) - 0.5);
            int jdash = (int)Math.Floor((v / dv) - 0.5);

            if (u < 0.0 || u > 1.0 || v < 0.0 || v > 1.0 || idash < -1 || idash > nu || jdash < -1 || jdash > nv)
            {
                return double.NaN;
            }

            double u1 = du * (0.5 + idash);
            double v1 = dv * (0.5 + jdash);

            int i1 = (idash >= 0) ? idash : (circular) ? nu : 0;
            int i2 = (idash < nu) ? (idash + 1) : (circular) ? 0 : nu;
            int j1 = (jdash >= 0) ? jdash : 0;
            int j2 = (jdash < nv) ? (jdash + 1) : nv;

            return BilinearInterpolation(u1, v1, du, dv, gridData[j1][i1], gridData[j1][i2], gridData[j2][i1], gridData[j2][i2], u, v);
        }

        /// <summary>
        /// Gets the value contained at the specified i and j index.
        /// </summary>
        /// <param name="i">
        /// Index along X-axis.
        /// </param>
        /// <param name="j">
        /// Index along Y-axis.
        /// </param>
        /// <returns>
        /// Value contained at the specified i and j index.
        /// </returns>
        public double GetValueAt(int i, int j)
        {
            if (j < 0 || j >= this.gridData.GetLength(0) || i < 0 || i >= this.gridData[0].Length)
            {
                return double.NaN;
            }

            return this.gridData[j][i];
        }

        /// <summary>
        /// Gets the X index value for the specified u co-ordinate in the grid
        /// </summary>
        /// <param name="u">
        /// U co-ordinate
        /// </param>
        /// <returns>
        /// Index along X-axis
        /// </returns>
        public int GetXIndex(double u)
        {
            int idash = (int)Math.Floor((u / du) - 0.5);
            if (u < 0.0 || u > 1.0 || idash < -1 || idash > nu)
            {
                // Error condition
                return -1;
            }

            return idash;
        }

        /// <summary>
        /// Gets the Y index value for the specified v co-ordinate in the grid
        /// </summary>
        /// <param name="v">
        /// V co-ordinate
        /// </param>
        /// <returns>
        /// Index along Y axis
        /// </returns>
        public int GetYIndex(double v)
        {
            int jdash = (int)Math.Floor((v / dv) - 0.5);
            if (v < 0.0 || v > 1.0 || jdash < -1 || jdash > nv)
            {
                // Error condition
                return -1;
            }

            return jdash;
        }

        /// <summary>
        /// This function is used to get the value based on bilinear interpolation.
        /// </summary>
        /// <param name="u1">
        /// Updated u1 value.
        /// </param>
        /// <param name="v1">
        /// Updated v1 value.
        /// </param>
        /// <param name="deltaU">
        /// du Delta value.
        /// </param>
        /// <param name="deltaV">
        /// dv Delta value.
        /// </param>
        /// <param name="f11">
        /// Pixel value at (1,1).
        /// </param>
        /// <param name="f21">
        /// Pixel value at (2,1).
        /// </param>
        /// <param name="f12">
        /// Pixel value at (1,2).
        /// </param>
        /// <param name="f22">
        /// Pixel value at (2,2).
        /// </param>
        /// <param name="u">
        /// Pixel value at u.
        /// </param>
        /// <param name="v">
        /// Pixel value at v.
        /// </param>
        /// <returns>
        /// Interpolate value of (u,v) pixel.
        /// </returns>
        private static int BilinearInterpolation(double u1, double v1, double deltaU, double deltaV, double f11, double f21, double f12, double f22, double u, double v)
        {
            double us = (u - u1) / deltaU;
            double vs = (v - v1) / deltaV;
            double f1 = us * (f21 - f11) + f11;
            double f2 = us * (f22 - f12) + f12;
            return (int)(vs * (f2 - f1) + f1);
        }
    }
}
