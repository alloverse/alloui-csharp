using System;
using System.Diagnostics;
using System.IO;
using System.Collections;
using System.Collections.Generic;

namespace AlloUI
{
    public class Surface : View
    {
        private Color _color = null;
        public Color Color {
            get {
                return _color;
            }
            set {
                _color = value;
                MarkAsDirty("material");
            } 
        }

        public override EntitySpecification Specification()
        {
            var spec = base.Specification();

            var geom = new Component.InlineGeometry();
            Size s = Bounds.Size;
            double w2 = s.Width / 2.0;
            double h2 = s.Height / 2.0;
            //              #bl                   #br                  #tl                   #tr
            geom.vertices= new List<List<double>> { new List<double> {-w2, -h2, 0.0}, new List<double> {w2, -h2, 0.0}, new List<double> {-w2, h2, 0.0},  new List<double> {w2, h2, 0.0}};
            geom.uvs=      new List<List<double>> { new List<double> {0.0, 0.0},      new List<double> {1.0, 0.0},     new List<double> {0.0, 1.0},      new List<double> {1.0, 1.0}};
            geom.triangles= new List<List<int>> {new List<int> {0, 1, 3}, new List<int> {0, 3, 2}, new List<int> {1, 0, 2}, new List<int> {1, 2, 3}};
            
            spec.components.geometry = geom;

            if(Color != null) {
                spec.components.material = new Component.Material();
                spec.components.material.color = Color.AsList;
            }

            return spec;
        }
    }
}