//-----------------------------------------------------------------------
// <copyright file="RegionMask.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation 2011. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Globalization;
using Catfood.Shapefile;
using Microsoft.Research.Wwt.Sdk.Utilities;

namespace Microsoft.Research.Wwt.Sdk.Samples
{
    /// <summary>
    /// Class which checks whether a given longitude, latitude values falls within a specified region.
    /// </summary>
    public class RegionMask : IRegionMask
    {
        /// <summary>
        /// Shapes list.
        /// </summary>
        private Dictionary<string, List<PointD[]>> shapesList;

        /// <summary>
        ///  Horizontal and vertical points.
        /// </summary>
        private Dictionary<string, List<string>> cells;

        /// <summary>
        /// The shape file path.
        /// </summary>
        private string shapeFilePath;

        /// <summary>
        /// The region HV map file path.
        /// </summary>
        private string regionHVMapFilePath;

        /// <summary>
        /// Initializes a new instance of the RegionMask class.
        /// </summary>
        /// <param name="shapeFilePath">Share file path.</param>
        /// <param name="regionHVMapFilePath">Region map file path.</param>
        /// <param name="shapeKey">Shape key used.</param>
        public RegionMask(string shapeFilePath, string regionHVMapFilePath, string shapeKey)
        {
            if (string.IsNullOrWhiteSpace(shapeFilePath))
            {
                throw new ArgumentNullException("shapeFilePath");
            }

            if (string.IsNullOrWhiteSpace(regionHVMapFilePath))
            {
                throw new ArgumentNullException("regionHVMapFilePath");
            }

            this.shapeFilePath = shapeFilePath;
            this.regionHVMapFilePath = regionHVMapFilePath;

            this.shapesList = new Dictionary<string, List<PointD[]>>();
            this.cells = new Dictionary<string, List<string>>();
            this.Initialize(shapeKey);
        }

        /// <summary>
        /// Checks whether a given longitude, latitude values falls within a specified region represented by horizontal
        /// and vertical axis.
        /// </summary>
        /// <param name="longitude">Longitude in degrees (in [-180; 180] range).</param>
        /// <param name="latitude">Latitude in degrees (in [-90; 90] range).</param>
        /// <returns>Valid or not.</returns>
        public bool IsBound(double longitude, double latitude, int horizontalAxis, int verticalAxis)
        {
            return this.CheckPoints(longitude, latitude, horizontalAxis, verticalAxis);
        }

        /// <summary>
        /// Initializes the shape file.
        /// </summary>
        /// <param name="shapeKey">Shape key used.</param>
        private void Initialize(string shapeKey)
        {
            // construct shapefile with the path to the .shp file
            using (Shapefile shapefile = new Shapefile(this.shapeFilePath))
            {
                // enumerate all shapes
                foreach (Shape shape in shapefile)
                {
                    // cast shape based on the type
                    switch (shape.Type)
                    {
                        case ShapeType.Polygon:
                            // a polygon contains one or more parts - each part is a list of points which
                            // are clockwise for boundaries and anti-clockwise for holes 
                            // see http://www.esri.com/library/whitepapers/pdfs/shapefile.pdf
                            ShapePolygon shapePolygon = shape as ShapePolygon;
                            string shapeName = shape.GetMetadata(shapeKey);
                            if (!string.IsNullOrEmpty(shapeName))
                            {
                                shapeName = shapeName.Trim().ToLowerInvariant();
                                if (!this.shapesList.ContainsKey(shapeName))
                                {
                                    this.shapesList.Add(shapeName, shapePolygon.Parts);
                                }
                                else
                                {
                                    this.shapesList[shapeName].AddRange(shapePolygon.Parts);
                                }
                            }

                            break;
                        default:
                            // and so on for other types...
                            break;
                    }
                }
            }

            for (int h = 0; h < Constants.MaxHvalue; h++)
            {
                for (int v = 0; v < Constants.MaxVvalue; v++)
                {
                    this.cells.Add(string.Format(CultureInfo.CurrentCulture, "h{0:D2}v{1:D2}", h, v), new List<string>());
                }
            }

            // Initialize Region to HV Map.
            foreach (var region in System.IO.File.ReadAllLines(this.regionHVMapFilePath))
            {
                string[] regionCellMap = region.Split(':');
                string regionName = regionCellMap[0].ToLower(CultureInfo.CurrentCulture);

                foreach (var cell in regionCellMap[1].Split(','))
                {
                    this.cells[cell].Add(regionName);
                }
            }
        }

        /// <summary>
        /// Checks whether the given points are within a specified region.
        /// </summary>
        /// <param name="longitude">The longitude.</param>
        /// <param name="latitude">The latitude.</param>
        /// <returns>Boolean indicating whether the point lies within a polygon.</returns>
        private bool CheckPoints(double longitude, double latitude, int horizontalAxis, int verticalAxis)
        {
            bool isInPoly = false;
            string cellname = string.Format(CultureInfo.CurrentCulture, "h{0:D2}v{1:D2}", horizontalAxis, verticalAxis);

            foreach (string regions in this.cells[cellname])
            {
                if (this.shapesList.ContainsKey(regions))
                {
                    foreach (PointD[] part in this.shapesList[regions])
                    {
                        if (this.IsInPolygon(part, latitude, longitude))
                        {
                            isInPoly = true;
                            break;
                        }
                    }
                }

                if (isInPoly)
                {
                    break;
                }
            }

            return isInPoly;
        }

        /// <summary>
        /// Determines whether geo-loaction is in polygon specified by points.
        /// </summary>
        /// <param name="points">The points.</param>
        /// <param name="lat">The latitude.</param>
        /// <param name="lon">The longitude.</param>
        /// <returns>
        /// <c>true</c> if is in polygon specified by points; otherwise, <c>false</c>.
        /// </returns>
        private bool IsInPolygon(PointD[] points, double lat, double lon)
        {
            var j = points.Length - 1;
            var inPoly = false;
            for (int i = 0; i < points.Length; i++)
            {
                if ((points[i].X < lon && points[j].X >= lon) || (points[j].X < lon && points[i].X >= lon))
                {
                    if (points[i].Y + (lon - points[i].X) / (points[j].X - points[i].X) * (points[j].Y - points[i].Y) < lat)
                    {
                        inPoly = !inPoly;
                    }
                }

                j = i;
            }

            return inPoly;
        }
    }
}
