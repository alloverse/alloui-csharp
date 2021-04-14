using System;
using System.Diagnostics;
using System.IO;
using System.Collections;
using System.Collections.Generic;

namespace AlloUI
{
    public class HorizontalTextAlignment
    {
        public string Value { get; set; }
        public static HorizontalTextAlignment Left { get { return new HorizontalTextAlignment{Value="left"}; } }
        public static HorizontalTextAlignment Center { get { return new HorizontalTextAlignment{Value="center"}; } }
        public static HorizontalTextAlignment Right { get { return new HorizontalTextAlignment{Value="right"}; } }
    }
    public class Label : View
    {
        private Color _color = Color.Black;
        public Color Color {
            get { return _color; }
            set { _color = value; MarkAsDirty("material"); } 
        }
        private string _text = "";
        public string Text {
            get { return _text; }
            set { _text = value; MarkAsDirty("text"); }
        }
        private double _lineHeight;
        public double LineHeight {
            get { return _lineHeight; }
            set { _lineHeight = value; MarkAsDirty("text"); }
        }
        private double _wrap;
        public double Wrap {
            get { return _wrap; }
            set { _wrap = value; MarkAsDirty("text"); }
        }
        private HorizontalTextAlignment _halign = HorizontalTextAlignment.Center;
        public HorizontalTextAlignment HorizontalTextAlignment {
            get { return _halign; }
            set { _halign = value; MarkAsDirty("text"); }
        }
        private double _fitToWidth;
        public double FitToWidth {
            get { return _fitToWidth; }
            set { _fitToWidth = value; MarkAsDirty("text"); }
        }

        public override EntitySpecification Specification()
        {
            var spec = base.Specification();

            spec.components.text = new Component.Text{
                text=_text,
                height= _lineHeight > 0 ? _lineHeight : Bounds.Size.Height,
                wrap=_wrap > 0 ? _wrap : Bounds.Size.Width,
                halign=_halign.Value,
                fitToWidth=_fitToWidth
            };

            return spec;
        }
    }
}