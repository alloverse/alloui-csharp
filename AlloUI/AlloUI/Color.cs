using System;
using System.Diagnostics;
using System.IO;
using System.Collections;
using System.Collections.Generic;

namespace AlloUI
{
    public class Color
    {
        public double Red = 1.0;
        public double Green = 1.0;
        public double Blue = 1.0;
        public double Alpha = 1.0;

        public Color() {}
        public Color(double r, double g, double b, double a)
        {
            Red = r;
            Green = g;
            Blue = b;
            Alpha = a;
        }

        public List<double> AsList { 
            get {
                return new List<double>{Red, Green, Blue, Alpha};
            }
            set {
                Red = value[0];
                Green = value[1];
                Blue = value[2];
                Alpha = value[3];
            }
        }
    }
}