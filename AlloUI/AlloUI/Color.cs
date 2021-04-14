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
        public static Color White { get { return new Color(1, 1, 1, 1); } }
        public static Color Black { get { return new Color(0, 0, 0, 1); } }
        public static Color FullRed { get { return new Color(1, 0, 0, 1); } }
        public static Color FullGreen { get { return new Color(0, 1, 0, 1); } }
        public static Color FullBlue { get { return new Color(0, 0, 1, 1); } }
        public static Color Purple { get { return new Color(0.5, 0, 0.5, 1); } }

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