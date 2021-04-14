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
            int bl = geom.AddVertexUV(-w2, -h2, 0.0,   0.0, 0.0);
            int br = geom.AddVertexUV( w2, -h2, 0.0,   1.0, 0.0);
            int tl = geom.AddVertexUV(-w2,  h2, 0.0,   0.0, 1.0);
            int tr = geom.AddVertexUV( w2,  h2, 0.0,   1.0, 1.0);
            geom.AddTriangle(bl, br, tr);
            geom.AddTriangle(bl, tr, tl);
            geom.AddTriangle(br, bl, tl);
            geom.AddTriangle(br, tl, tr);

            spec.components.geometry = geom;

            if(Color != null) {
                spec.components.material = new Component.Material();
                spec.components.material.color = Color.AsList;
            }

            return spec;
        }
    }
}