//---------------------------------------------------------------------------
// <copyright file="MercatorDemTileCreator.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation 2011. All rights reserved.
// </copyright>
//---------------------------------------------------------------------------

using System;

namespace Microsoft.Research.Wwt.Sdk.Core
{
    /// <summary>
    /// Creates depth elevation model tiles for Mercator projection.
    /// </summary>
    public class MercatorDemTileCreator : ITileCreator
    {
        /// <summary>
        /// Mutex used.
        /// </summary>
        private static object token = new object();

        /// <summary>
        /// Mapping of vertices where elevation is sampled.
        /// </summary>
        private static int[][] vertexMapping;

        /// <summary>
        /// Elevation map used.
        /// </summary>
        private IElevationMap elevationMap;

        /// <summary>
        /// Tile serializer used.
        /// </summary>
        private IDemTileSerializer tileSerializer;

        /// <summary>
        /// Initializes a new instance of the MercatorDemTileCreator class.
        /// </summary>
        /// <param name="map">
        /// Color map.
        /// </param>
        /// <param name="serializer">
        /// Tile serializer.
        /// </param>
        public MercatorDemTileCreator(IElevationMap map, IDemTileSerializer serializer)
        {
            if (map == null)
            {
                throw new ArgumentNullException("map");
            }

            if (serializer == null)
            {
                throw new ArgumentNullException("serializer");
            }

            this.elevationMap = map;
            this.tileSerializer = serializer;
            this.ProjectionType = ProjectionTypes.Mercator;
        }

        /// <summary>
        /// Gets the ProjectionType.
        /// </summary>
        public ProjectionTypes ProjectionType { get; private set; }

        /// <summary>
        /// Creates the tile specified by level.
        /// </summary>
        /// <param name="level">
        /// Zoom level.
        /// </param>
        /// <param name="tileX">
        /// X tile coordinate.
        /// </param>
        /// <param name="tileY">
        /// Y tile coordinate.
        /// </param>
        public void Create(int level, int tileX, int tileY)
        {
            double latMin = checked(Helper.AbsoluteMetersToLatitudeAtZoom(tileY * 256, level));
            double latCenter = checked(Helper.AbsoluteMetersToLatitudeAtZoom(((tileY * 2) + 1) * 256, level + 1));
            double latMax = checked(Helper.AbsoluteMetersToLatitudeAtZoom((tileY + 1) * 256, level));

            double tileDegrees = 360.0 / (1 << level);
            double lngMin = (((double)tileX * tileDegrees) - 180.0);
            double lngMax = checked(((((double)(tileX + 1)) * tileDegrees) - 180.0));
            tileDegrees = lngMax - lngMin;

            int subDivisions = 32;
            double textureStep = 1.0 / subDivisions;
            short[] data = new short[33 * 33];
            int index = 0;

            double lat, lng;
            int x1, y1;

            double latDegrees = latMax - latCenter;
            for (y1 = 0; y1 < subDivisions / 2; y1++)
            {
                if (y1 != subDivisions / 2)
                {
                    lat = latMax - (2 * textureStep * latDegrees * (double)y1);
                }
                else
                {
                    lat = latCenter;
                }

                for (x1 = 0; x1 <= subDivisions; x1++)
                {
                    if (x1 != subDivisions)
                    {
                        lng = lngMin + (textureStep * tileDegrees * (double)x1);
                    }
                    else
                    {
                        lng = lngMax;
                    }

                    data[index] = this.elevationMap.GetElevation(lng, lat);
                    index++;
                }
            }

            latDegrees = latMin - latCenter;
            for (y1 = subDivisions / 2; y1 <= subDivisions; y1++)
            {
                if (y1 != subDivisions)
                {
                    lat = latCenter + (2 * textureStep * latDegrees * (double)(y1 - (subDivisions / 2)));
                }
                else
                {
                    lat = latMin;
                }

                for (x1 = 0; x1 <= subDivisions; x1++)
                {
                    if (x1 != subDivisions)
                    {
                        lng = lngMin + (textureStep * tileDegrees * (double)x1);
                    }
                    else
                    {
                        lng = lngMax;
                    }

                    data[index] = this.elevationMap.GetElevation(lng, lat);
                    index++;
                }
            }

            this.tileSerializer.Serialize(data, level, tileX, tileY);
        }

        /// <summary>
        /// Aggregates lower level DEM tiles to construct upper level tiles.
        /// </summary>
        /// <param name="level">
        /// Zoom level.
        /// </param>
        /// <param name="tileX">
        /// X coordinate.
        /// </param>
        /// <param name="tileY">
        /// Y coordinate.
        /// </param>
        public void CreateParent(int level, int tileX, int tileY)
        {
            int level1 = checked(level + 1);
            int x1 = checked(2 * tileX);
            int y1 = checked(2 * tileY);

            short[][] h = new short[4][];
            h[2] = this.tileSerializer.Deserialize(level1, x1, y1);
            h[3] = this.tileSerializer.Deserialize(level1, x1 + 1, y1);
            h[0] = this.tileSerializer.Deserialize(level1, x1, y1 + 1);
            h[1] = this.tileSerializer.Deserialize(level1, x1 + 1, y1 + 1);

            int[][] mapping = MercatorDemTileCreator.GetMapping();
            short[] hp = new short[mapping.Length];
            for (int k = 0; k < mapping.Length; k++)
            {
                int[] m = mapping[k];
                if (m != null)
                {
                    int region = m[0];
                    int position = m[1];
                    if (h[region] != null)
                    {
                        hp[k] = h[region][position];
                    }
                    else
                    {
                        hp[k] = 0;
                    }
                }
                else
                {
                    hp[k] = 0;
                }
            }

            this.tileSerializer.Serialize(hp, level, tileX, tileY);
        }

        /// <summary>
        /// Computes mapping for the vertices where elevation is sampled.
        /// </summary>
        /// <returns>
        /// Mapping generated.
        /// </returns>
        private static int[][] GetMapping()
        {
            if (MercatorDemTileCreator.vertexMapping == null)
            {
                lock (token)
                {
                    MercatorDemTileCreator.vertexMapping = new int[33 * 33][];
                    int index = 0;
                    for (int y1 = 0; y1 < 16; y1++)
                    {
                        int region = 0;
                        for (int x1 = 0; x1 <= 16; x1++)
                        {
                            MercatorDemTileCreator.vertexMapping[index] = new int[] { region, 2 * (33 * y1 + x1) };
                            index++;
                        }

                        region = 1;
                        for (int x1 = 1; x1 <= 16; x1++)
                        {
                            MercatorDemTileCreator.vertexMapping[index] = new int[] { region, 2 * (33 * y1 + x1) };
                            index++;
                        }
                    }

                    for (int y1 = 0; y1 <= 16; y1++)
                    {
                        int region = 2;
                        for (int x1 = 0; x1 <= 16; x1++)
                        {
                            MercatorDemTileCreator.vertexMapping[index] = new int[] { region, 2 * (33 * y1 + x1) };
                            index++;
                        }

                        region = 3;
                        for (int x1 = 1; x1 <= 16; x1++)
                        {
                            MercatorDemTileCreator.vertexMapping[index] = new int[] { region, 2 * (33 * y1 + x1) };
                            index++;
                        }
                    }
                }
            }

            return MercatorDemTileCreator.vertexMapping;
        }
    }
}
