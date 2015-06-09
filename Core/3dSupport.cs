//-----------------------------------------------------------------------
// <copyright file="3dSupport.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation 2011. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.ObjectModel;
using System.Globalization;

namespace Microsoft.Research.Wwt.Sdk.Core
{
    /// <summary>
    /// Represents a vector in 2-D space.
    /// </summary>
    public struct Vector2d
    {
        /// <summary>
        /// Initializes a new instance of the Vector2d struct.
        /// </summary>
        /// <param name="x">
        /// X coordinate.
        /// </param>
        /// <param name="y">
        /// Y coordinate.
        /// </param>
        public Vector2d(double x, double y)
            : this()
        {
            this.X = x;
            this.Y = y;
        }

        /// <summary>
        /// Gets or sets the X coordinate.
        /// </summary>
        public double X { get; set; }

        /// <summary>
        /// Gets or sets the Y coordinate.
        /// </summary>
        public double Y { get; set; }

        /// <summary>
        /// Interpolates the two vectors.
        /// </summary>
        /// <param name="left">
        /// First vector.
        /// </param>
        /// <param name="right">
        /// Second vector.
        /// </param>
        /// <param name="interpolater">
        /// Interpolater factor.
        /// </param>
        /// <returns>
        /// Lerp-ed vector.
        /// </returns>
        public static Vector2d Lerp(Vector2d left, Vector2d right, double interpolater)
        {
            if (Math.Abs(left.X - right.X) > 180)
            {
                if (left.X > right.X)
                {
                    right.X += 360;
                }
                else
                {
                    left.X += 360;
                }
            }

            return new Vector2d(left.X * (1 - interpolater) + right.X * interpolater, left.Y * (1 - interpolater) + right.Y * interpolater);
        }

        /// <summary>
        /// Compares the two instances of Vector2d object.
        /// </summary>
        /// <param name="obj1">
        /// Object1 which we need to compare.
        /// </param>
        /// <param name="obj2">
        /// Object2 which we need to compare.
        /// </param>
        /// <returns>
        /// Returns a value that indicates whether objects are equal.
        /// </returns>
        public static bool operator ==(Vector2d obj1, Vector2d obj2)
        {
            return obj1.Equals(obj2);
        }

        /// <summary>
        /// Compares the two instances of PositionTexture object.
        /// </summary>
        /// <param name="obj1">
        /// Object1 which we need to compare.
        /// </param>
        /// <param name="obj2">
        /// Object2 which we need to compare.
        /// </param>
        /// <returns>
        /// Returns a value that indicates whether objects are Not equal.
        /// </returns>
        public static bool operator !=(Vector2d obj1, Vector2d obj2)
        {
            return !obj1.Equals(obj2);
        }

        /// <summary>
        /// Compares the current instance to a specified object.
        /// </summary>
        /// <param name="obj">
        /// Object with which to make the comparison.
        /// </param>
        /// <returns>
        /// Returns a value that indicates whether the current instance is equal to a specified object.
        /// </returns>
        public override bool Equals(object obj)
        {
            Vector2d equal = (Vector2d)obj;
            return this.X == equal.X && this.Y == equal.Y;
        }

        /// <summary>
        /// Computes hash code for the current instance.
        /// </summary>
        /// <returns>
        /// Hash code.
        /// </returns>
        public override int GetHashCode()
        {
            return this.X.GetHashCode() ^ this.Y.GetHashCode();
        }
    }

    /// <summary>
    /// Describes a custom vertex format structure that contains position and one set of texture coordinates.
    /// </summary>
    internal struct PositionTexture
    {
        /// <summary>
        /// Gets or sets the u component of the texture coordinate.
        /// </summary>
        internal double Tu;

        /// <summary>
        /// Gets or sets the v component of the texture coordinate.
        /// </summary>
        internal double Tv;

        /// <summary>
        /// Gets or sets the x component of the position.
        /// </summary>
        internal double X;

        /// <summary>
        /// Gets or sets the y component of the position.
        /// </summary>
        internal double Y;

        /// <summary>
        /// Gets or sets the z component of the position.
        /// </summary>
        internal double Z;

        /// <summary>
        /// Initializes a new instance of the PositionTexture struct.
        /// </summary>
        /// <param name="pos">
        /// A Microsoft.DirectX.Vector3d object that contains the vertex position.
        /// </param>
        /// <param name="u">
        /// U coordinate.
        /// </param>
        /// <param name="v">
        /// V coordinate.
        /// </param>
        public PositionTexture(Vector3d pos, double u, double v)
            : this(pos.X, pos.Y, pos.Z, u, v)
        {
        }

        /// <summary>
        /// Initializes a new instance of the PositionTexture struct.
        /// </summary>
        /// <param name="xvalue">
        /// Floating-point value that represents the x coordinate of the position.
        /// </param>
        /// <param name="yvalue">
        /// Floating-point value that represents the y coordinate of the position.
        /// </param>
        /// <param name="zvalue">
        /// Floating-point value that represents the z coordinate of the position.
        /// </param>
        /// <param name="u">
        /// U coordinate.
        /// </param>
        /// <param name="v">
        /// V coordinate.
        /// </param>
        public PositionTexture(double xvalue, double yvalue, double zvalue, double u, double v)
        {
            this.Tu = u;
            this.Tv = v;
            this.X = xvalue;
            this.Y = yvalue;
            this.Z = zvalue;
        }

        /// <summary>
        /// Gets the vertex position.
        /// </summary>
        public Vector3d Position
        {
            get
            {
                return new Vector3d(this.X, this.Y, this.Z);
            }
        }

        /// <summary>
        /// Compares the two instances of PositionTexture object.
        /// </summary>
        /// <param name="obj1">
        /// Object1 which we need to compare.
        /// </param>
        /// <param name="obj2">
        /// Object2 which we need to compare.
        /// </param>
        /// <returns>
        /// Returns a value that indicates whether objects are equal.
        /// </returns>
        public static bool operator ==(PositionTexture obj1, PositionTexture obj2)
        {
            return obj1.Equals(obj2);
        }

        /// <summary>
        /// Compares the two instances of PositionTexture object.
        /// </summary>
        /// <param name="obj1">
        /// Object1 which we need to compare.
        /// </param>
        /// <param name="obj2">
        /// Object2 which we need to compare.
        /// </param>
        /// <returns>
        /// Returns a value that indicates whether objects are Not equal.
        /// </returns>
        public static bool operator !=(PositionTexture obj1, PositionTexture obj2)
        {
            return !obj1.Equals(obj2);
        }

        /// <summary>
        /// Gets a string representation of the current instance.
        /// </summary>
        /// <returns>
        /// String representation of the current instance.
        /// </returns>
        public override string ToString()
        {
            return String.Format(CultureInfo.InvariantCulture, "{0}, {1}, {2}, {3}, {4}", this.X, this.Y, this.Z, this.Tu, this.Tv);
        }

        /// <summary>
        /// Compares the current instance to a specified object.
        /// </summary>
        /// <param name="obj">
        /// Object with which to make the comparison.
        /// </param>
        /// <returns>
        /// Returns a value that indicates whether the current instance is equal to a specified object.
        /// </returns>
        public override bool Equals(object obj)
        {
            PositionTexture equal = (PositionTexture)obj;
            return this.Tu == equal.Tu && this.Tv == equal.Tv && this.X == equal.X && this.Y == equal.Y && this.Z == equal.Z;
        }

        /// <summary>
        /// Computes hash code for the current instance.
        /// </summary>
        /// <returns>
        /// Hash code.
        /// </returns>
        public override int GetHashCode()
        {
            return this.Tu.GetHashCode() ^ this.Tv.GetHashCode() ^ this.X.GetHashCode() ^ this.Y.GetHashCode() ^ this.Z.GetHashCode();
        }
    }

    /// <summary>
    /// Describes and manipulates a vector in three-dimensional (3-D) space.
    /// </summary>
    [Serializable]
    internal struct Vector3d
    {
        /// <summary>
        /// Gets or sets the x component of a 3-D vector.
        /// </summary>
        internal double X;

        /// <summary>
        /// Gets or sets the y component of a 3-D vector.
        /// </summary>
        internal double Y;

        /// <summary>
        /// Gets or sets the z component of a 3-D vector.
        /// </summary>
        internal double Z;

        /// <summary>
        /// Initializes a new instance of the Vector3d struct.
        /// </summary>
        /// <param name="valueX">
        /// Initial Microsoft.DirectX.Vector3d.X value.
        /// </param>
        /// <param name="valueY">
        /// Initial Microsoft.DirectX.Vector3d.Y value.
        /// </param>
        /// <param name="valueZ">
        /// Initial Microsoft.DirectX.Vector3d.Z value.
        /// </param>
        public Vector3d(double valueX, double valueY, double valueZ)
        {
            this.X = valueX;
            this.Y = valueY;
            this.Z = valueZ;
        }

        /// <summary>
        /// Negates the vector.
        /// </summary>
        /// <param name="vector">
        /// Source Microsoft.DirectX.Vector3d structure.
        /// </param>
        /// <returns>
        /// The Microsoft.DirectX.Vector3d structure that is the result of the operation.
        /// </returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2225:OperatorOverloadsHaveNamedAlternates", Justification = "Cannot change the name.")]
        public static Vector3d operator -(Vector3d vector)
        {
            Vector3d result;
            result.X = -vector.X;
            result.Y = -vector.Y;
            result.Z = -vector.Z;
            return result;
        }

        /// <summary>
        /// Subtracts two 3-D vectors.
        /// </summary>
        /// <param name="left">
        /// The Microsoft.DirectX.Vector3d structure to the left of the subtraction operator.
        /// </param>
        /// <param name="right">
        /// The Microsoft.DirectX.Vector3d structure to the right of the subtraction operator.
        /// </param>
        /// <returns>
        /// Resulting Microsoft.DirectX.Vector3d structure.
        /// </returns>
        public static Vector3d operator -(Vector3d left, Vector3d right)
        {
            return new Vector3d(left.X - right.X, left.Y - right.Y, left.Z - left.Z);
        }

        /// <summary>
        /// Compares the current instance of a class to another instance to determine whether they are different.
        /// </summary>
        /// <param name="left">
        /// The Microsoft.DirectX.Vector3d structure to the left of the inequality operator.
        /// </param>
        /// <param name="right">
        /// The Microsoft.DirectX.Vector3d structure to the right of the inequality operator.
        /// </param>
        /// <returns>
        /// Value that is true if the objects are different, or false if they are the same.
        /// </returns>
        public static bool operator !=(Vector3d left, Vector3d right)
        {
            return (left.X != right.X || left.Y != right.Y || left.Z != right.Z);
        }

        /// <summary>
        /// Compares the current instance of a class to another instance to determine whether they are the same.
        /// </summary>
        /// <param name="left">
        /// The Microsoft.DirectX.Vector3d structure to the left of the equality operator.
        /// </param>
        /// <param name="right">
        /// The Microsoft.DirectX.Vector3d structure to the right of the equality operator.
        /// </param>
        /// <returns>
        /// Value that is true if the objects are the same, or false if they are different.
        /// </returns>
        public static bool operator ==(Vector3d left, Vector3d right)
        {
            return (left.X == right.X || left.Y == right.Y || left.Z == right.Z);
        }

        /// <summary>
        /// Computes midpoint of two given vectors.
        /// </summary>
        /// <param name="first">
        /// First Microsoft.DirectX.Vector3d structure.
        /// </param>
        /// <param name="second">
        /// Second Microsoft.DirectX.Vector3d structure.
        /// </param>
        /// <returns>
        /// Midpoint of the two vectors.
        /// </returns>
        public static Vector3d MidPoint(Vector3d first, Vector3d second)
        {
            Vector3d result = new Vector3d((first.X + second.X) / 2, (first.Y + second.Y) / 2, (first.Z + second.Z) / 2);
            result.Normalize();
            return result;
        }

        /// <summary>
        /// Performs a linear interpolation between two 3-D vectors.
        /// </summary>
        /// <param name="left">
        /// First vector.
        /// </param>
        /// <param name="right">
        /// Second vector.
        /// </param>
        /// <param name="interpolater">
        /// Parameter that linearly interpolates between the vectors.
        /// </param>
        /// <returns>
        /// A Microsoft.DirectX.Vector3d structure that is the result of the linear interpolation.
        /// </returns>
        public static Vector3d Lerp(Vector3d left, Vector3d right, double interpolater)
        {
            return new Vector3d(
                left.X * (1.0 - interpolater) + right.X * interpolater,
                left.Y * (1.0 - interpolater) + right.Y * interpolater,
                left.Z * (1.0 - interpolater) + right.Z * interpolater);
        }

        /// <summary>
        /// Compares the current instance to a specified object.
        /// </summary>
        /// <param name="obj">
        /// Object with which to make the comparison.
        /// </param>
        /// <returns>
        /// Returns a value that indicates whether the current instance is equal to a specified object.
        /// </returns>
        public override bool Equals(object obj)
        {
            Vector3d comp = (Vector3d)obj;
            return this.X == comp.X && this.Y == comp.Y && this.Z == comp.Z;
        }

        /// <summary>
        /// Computes hash code for the current instance.
        /// </summary>
        /// <returns>
        /// Hash code.
        /// </returns>
        public override int GetHashCode()
        {
            return this.X.GetHashCode() ^ this.Y.GetHashCode() ^ this.Z.GetHashCode();
        }

        /// <summary>
        /// Returns the length of a 3-D vector.
        /// </summary>
        /// <returns>
        /// Length of the vector.
        /// </returns>
        public double Length()
        {
            return System.Math.Sqrt(this.X * this.X + this.Y * this.Y + this.Z * this.Z);
        }

        /// <summary>
        /// Normalizes the given vector.
        /// </summary>
        public void Normalize()
        {
            // Vector3.Length property is under length section
            double length = this.Length();
            if (length != 0)
            {
                this.X /= length;
                this.Y /= length;
                this.Z /= length;
            }
        }

        /// <summary>
        /// Obtains a string representation of the current instance.
        /// </summary>
        /// <returns>
        /// String that represents the object.
        /// </returns>
        public override string ToString()
        {
            return String.Format(CultureInfo.InvariantCulture, "{0}, {1}, {2}", this.X, this.Y, this.Z);
        }

        /// <summary>
        /// Converts the vector to spherical coordinates.
        /// </summary>
        /// <returns>
        /// Vector object converted to spherical coordinates.
        /// </returns>
        public Vector2d ToSpherical()
        {
            double ascention;
            double declination;

            double radius = Math.Sqrt(this.X * this.X + this.Y * this.Y + this.Z * this.Z);
            double product = Math.Sqrt(this.X * this.X + this.Z * this.Z);
            declination = Math.Asin(this.Y / radius);
            if (product == 0)
            {
                ascention = 0;
            }
            else if (0 <= this.X)
            {
                ascention = Math.Asin(this.Z / product);
            }
            else
            {
                ascention = Math.PI - Math.Asin(this.Z / product);
            }

            return new Vector2d((((ascention + Math.PI) % (2.0 * Math.PI))), ((declination + (Math.PI / 2.0))));
        }

        /// <summary>
        /// Converts the given vector to RA-Dec coordinates.
        /// </summary>
        /// <returns>
        /// Converted Vector2d object.
        /// </returns>
        public Vector2d ToRaDec()
        {
            Vector2d point = this.ToSpherical();
            point.X = point.X / Math.PI * 180;
            point.Y = (point.Y / Math.PI * 180) - 90;
            return point;
        }
    }

    /// <summary>
    /// Implements the Triangle functionality.
    /// </summary>
    internal class Triangle
    {
        /// <summary>
        /// Initializes a new instance of the Triangle class.
        /// </summary>
        public Triangle()
        {
            this.A = -1;
            this.B = -1;
            this.C = -1;
        }

        /// <summary>
        /// Initializes a new instance of the Triangle class.
        /// </summary>
        /// <param name="a">
        /// First vertex.
        /// </param>
        /// <param name="b">
        /// Second vertex.
        /// </param>
        /// <param name="c">
        /// Third vertex.
        /// </param>
        public Triangle(int a, int b, int c)
        {
            this.A = a;
            this.B = b;
            this.C = c;
        }

        /// <summary>
        /// Gets or sets the first vertex.
        /// </summary>
        internal int A { get; set; }

        /// <summary>
        /// Gets or sets the second vertex.
        /// </summary>
        internal int B { get; set; }

        /// <summary>
        /// Gets or sets the third vertex.
        /// </summary>
        internal int C { get; set; }

        /// <summary>
        /// Subdivides the given list of triangles with the given list of vertices.
        /// </summary>
        /// <param name="triList">
        /// List of triangles.
        /// </param>
        /// <param name="vertexList">
        /// List of vertices.
        /// </param>
        public void SubDivide(Collection<Triangle> triList, Collection<PositionTexture> vertexList)
        {
            if (triList == null)
            {
                throw new ArgumentNullException("triList");
            }

            if (vertexList == null)
            {
                throw new ArgumentNullException("vertexList");
            }

            Vector3d first = Vector3d.Lerp(vertexList[this.B].Position, vertexList[this.C].Position, .5f);
            Vector3d second = Vector3d.Lerp(vertexList[this.C].Position, vertexList[this.A].Position, .5f);
            Vector3d third = Vector3d.Lerp(vertexList[this.A].Position, vertexList[this.B].Position, .5f);

            Vector2d firstUV = Vector2d.Lerp(new Vector2d(vertexList[this.B].Tu, vertexList[this.B].Tv), new Vector2d(vertexList[this.C].Tu, vertexList[this.C].Tv), .5);
            Vector2d secondUV = Vector2d.Lerp(new Vector2d(vertexList[this.C].Tu, vertexList[this.C].Tv), new Vector2d(vertexList[this.A].Tu, vertexList[this.A].Tv), .5);
            Vector2d thirdUV = Vector2d.Lerp(new Vector2d(vertexList[this.A].Tu, vertexList[this.A].Tv), new Vector2d(vertexList[this.B].Tu, vertexList[this.B].Tv), .5);

            first.Normalize();
            second.Normalize();
            third.Normalize();

            int firstIndex = vertexList.Count;
            int secondIndex = vertexList.Count + 1;
            int thirdIndex = vertexList.Count + 2;

            vertexList.Add(new PositionTexture(first, firstUV.X, firstUV.Y));
            vertexList.Add(new PositionTexture(second, secondUV.X, secondUV.Y));
            vertexList.Add(new PositionTexture(third, thirdUV.X, thirdUV.Y));

            triList.Add(new Triangle(this.A, thirdIndex, secondIndex));
            triList.Add(new Triangle(this.B, firstIndex, thirdIndex));
            triList.Add(new Triangle(this.C, secondIndex, firstIndex));
            triList.Add(new Triangle(firstIndex, secondIndex, thirdIndex));
        }
    }
}