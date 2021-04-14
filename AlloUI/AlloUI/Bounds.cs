using System;
using System.Diagnostics;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using MathNet.Spatial.Euclidean;

namespace AlloUI
{
        
    public class Size
    {
        public double Width = 1.0;
        public double Height = 1.0;
        public double Depth = 1.0;
        public Size() {}
        public Size(double w, double h, double d)
        {
            Width = w;
            Height = h;
            Depth = d;
        }

        public Size(Size other)
        {
            Width = other.Width;
            Height = other.Height;
            Depth = other.Depth;
        }
    }

    public class Bounds
    {
        public Size Size = new Size();
        public CoordinateSystem Pose = new CoordinateSystem();
        public Bounds()
        {
        }
        public Bounds(double x, double y, double z, double w, double h, double d)
        {
            Size.Width = w;
            Size.Height = h;
            Size.Depth = d;
            Move(x, y, z);
        }

        public Bounds(Bounds other)
        {
            Size = new Size(other.Size);
            Pose = new CoordinateSystem(other.Pose);
        }

        public Bounds MoveToOrigin()
        {
            Pose = new CoordinateSystem(); // not sure if there's a better way to set it to identity...
            return this;
        }
        public Bounds Move(double x, double y, double z)
        {
            Pose.Multiply(CoordinateSystem.Translation(new Vector3D(x, y, z)), Pose);
            return this;
        }
    }
}
