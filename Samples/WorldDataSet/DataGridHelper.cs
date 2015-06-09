//-----------------------------------------------------------------------
// <copyright file="DataGridHelper.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation 2011. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using Microsoft.Research.Wwt.Sdk.Core;

namespace Microsoft.Research.Wwt.Sdk.Samples
{
    public static class DataGridHelper
    {
        /// <summary>
        /// Load data grid by reading image pixel values.
        /// </summary>
        /// <param name="path">
        /// XYZ file path.
        /// </param>
        public static DataGridDetails LoadFromFile(string path)
        {
            DataGridDetails inputImageDetails = LoadDataDetails(path);

            double deltaX = (inputImageDetails.Boundary.Right - inputImageDetails.Boundary.Left) / (inputImageDetails.Width - 1);
            double deltaY = (inputImageDetails.Boundary.Top - inputImageDetails.Boundary.Bottom) / (inputImageDetails.Height - 1);

            double[][] gridData = new double[inputImageDetails.Height][];
            for (int index = 0; index < inputImageDetails.Height; index++)
            {
                gridData[index] = new double[inputImageDetails.Width];
            }

            FileStream fileStream = null;
            try
            {
                fileStream = File.OpenRead(path);
                using (StreamReader stream = new StreamReader(fileStream))
                {
                    fileStream = null;
                    string line = stream.ReadLine();
                    if (!string.IsNullOrEmpty(line))
                    {
                        do
                        {
                            string[] parts = line.Split('\t');
                            double longValue = double.Parse(parts[0], CultureInfo.InvariantCulture);
                            double latValue = double.Parse(parts[1], CultureInfo.InvariantCulture);
                            double depth = double.Parse(parts[2], CultureInfo.InvariantCulture);

                            int i = (int)((longValue - inputImageDetails.Boundary.Left) / deltaX);
                            int j = (int)((latValue - inputImageDetails.Boundary.Bottom) / deltaY);
                            gridData[j][i] = depth;

                            line = stream.ReadLine();
                        }
                        while (!string.IsNullOrEmpty(line));
                    }
                }
            }
            finally
            {
                if (fileStream != null)
                {
                    fileStream.Dispose();
                }
            }

            SanitizeData(inputImageDetails.Width, inputImageDetails.Height, gridData);
            inputImageDetails.Data = gridData;

            return inputImageDetails;
        }

        /// <summary>
        /// Sanitizes the data by filling up empty rows and columns by neighboring rows and columns.
        /// </summary>
        private static void SanitizeData(int gridWidth, int gridHeight, double[][] pixelData)
        {
            List<int> allZeroRows = new List<int>();
            for (int j = 0; j < gridHeight; j++)
            {
                int i = 0;
                bool allZero = true;
                while (i < gridWidth)
                {
                    if (pixelData[j][i] != 0)
                    {
                        allZero = false;
                        break;
                    }

                    i++;
                }

                if (allZero)
                {
                    allZeroRows.Add(j);
                }
            }

            List<int> allZeroCols = new List<int>();
            for (int i = 0; i < gridWidth; i++)
            {
                int j = 0;
                bool allZero = true;
                while (j < gridHeight)
                {
                    if (pixelData[j][i] != 0)
                    {
                        allZero = false;
                        break;
                    }

                    j++;
                }

                if (allZero)
                {
                    allZeroCols.Add(i);
                }
            }

            foreach (int i in allZeroCols)
            {
                for (int j = 0; j < gridHeight; j++)
                {
                    pixelData[j][i] = pixelData[j][i - 1];
                }
            }

            for (int i = 0; i < gridWidth; i++)
            {
                foreach (int j in allZeroRows)
                {
                    pixelData[j][i] = pixelData[j - 1][i];
                }
            }
        }

        /// <summary>
        /// Analyzes the data to compute Width, Height, DeltaX, DeltaY, Maximum and Minimum values.
        /// </summary>
        /// <param name="path">
        /// Path of XYZ file.
        /// </param>
        private static DataGridDetails LoadDataDetails(string path)
        {
            HashSet<double> longitudes = new HashSet<double>();
            HashSet<double> latitudes = new HashSet<double>();
            HashSet<double> values = new HashSet<double>();
            FileStream fileStream = null;
            try
            {
                fileStream = File.OpenRead(path);
                using (StreamReader stream = new StreamReader(fileStream))
                {
                    fileStream = null;
                    string line = stream.ReadLine();
                    if (!string.IsNullOrEmpty(line))
                    {
                        do
                        {
                            string[] parts = line.Split('\t');
                            double longValue = 0;
                            double latValue = 0;
                            double value = 0;
                            if (!double.TryParse(parts[0], out longValue))
                            {
                                longValue = 0;
                            }

                            if (!double.TryParse(parts[1], out latValue))
                            {
                                latValue = 0;
                            }

                            if (!double.TryParse(parts[2], out value))
                            {
                                value = 0;
                            }

                            longitudes.Add(longValue);
                            latitudes.Add(latValue);
                            values.Add(value);

                            line = stream.ReadLine();
                        }
                        while (!string.IsNullOrEmpty(line));
                    }
                }
            }
            finally
            {
                if (fileStream != null)
                {
                    fileStream.Dispose();
                }
            }

            // Added 1 to use the variable as an array index so that the right size is allotted
            int gridWidth = longitudes.Count() + 1;
            int gridHeight = latitudes.Count() + 1;

            var boundary = new Boundary(longitudes.Min(), latitudes.Min(), longitudes.Max(), latitudes.Max());
            double minimumThreshold = values.Min();
            double maximumThreshold = values.Max();

            return new DataGridDetails(null, gridWidth, gridHeight, boundary, minimumThreshold, maximumThreshold);
        }
    }
}
