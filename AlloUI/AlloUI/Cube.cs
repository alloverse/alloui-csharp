using System;
using System.Diagnostics;
using System.IO;
using System.Collections;
using System.Collections.Generic;

namespace AlloUI
{
    public class Cube : View
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
            double d2 = s.Depth / 2.0;
            int fbl = geom.AddVertexUV(-w2, -h2, d2, 0.0, 0.0);
            int fbr = geom.AddVertexUV(w2, -h2, d2, 1.0, 0.0);
            int ftl = geom.AddVertexUV(-w2, h2, d2, 0.0, 1.0);
            int ftr = geom.AddVertexUV(w2, h2, d2, 1.0, 1.0);
            int rbl = geom.AddVertexUV(-w2, -h2, -d2, 0.0, 0.0);
            int rbr = geom.AddVertexUV(w2, -h2, -d2, 1.0, 0.0);
            int rtl = geom.AddVertexUV(-w2, h2, -d2, 0.0, 1.0);
            int rtr = geom.AddVertexUV(w2, h2, -d2, 1.0, 1.0);
            geom.AddTriangle(fbl, fbr, ftl); geom.AddTriangle(fbr, ftr, ftl); // front
            geom.AddTriangle(ftl, ftr, rtl); geom.AddTriangle(ftr, rtr, rtl); // top
            geom.AddTriangle(fbr, rtr, ftr); geom.AddTriangle(rbr, rtr, fbr); // right
            geom.AddTriangle(rbr, fbr, fbl); geom.AddTriangle(rbl, rbr, fbl); // bottom
            geom.AddTriangle(rbl, fbl, ftl); geom.AddTriangle(rbl, ftl, rtl); // left
            geom.AddTriangle(rbl, rtl, rbr); geom.AddTriangle(rbr, rtl, rtr); // read
     
            spec.components.geometry = geom;

            if(Color != null) {
                spec.components.material = new Component.Material();
                spec.components.material.color = Color.AsList;
            }

            return spec;
        }
    }
}