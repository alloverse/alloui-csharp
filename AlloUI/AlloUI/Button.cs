using System;
using System.Diagnostics;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using MathNet.Spatial.Euclidean;

namespace AlloUI
{
    public class Button : View
    {
        public Label Label = new Label();
        public Cube Cube = new Cube();

        private bool _selected;
        private bool _highlighted;
        public bool IsSelected { 
            get { return _selected; }
            private set { 
                if(value == _selected) {return;} 
                _selected = value; 
                _updateLooks(); 
            }
        }
        public bool IsHighlighted { 
            get { return _highlighted; }
            private set {
                if(value == _highlighted) {return;}
                _highlighted = value;
                _updateLooks();
            }
        }

        public Button() : base()
        {
            IsPointable = true;
            addSubview(Label);
            addSubview(Cube);
            Cube.Color = Color.Purple;
            Label.Color = Color.Black;
        }

        public class ActionArgs : EventArgs
        {
            public AlloEntity Sender { get; internal set; }
        }
        public event EventHandler<ActionArgs> Action;
        public void Activate(AlloEntity sender)
        {
            Action?.Invoke(this, new ActionArgs { Sender = sender });
        }

        public override void Layout()
        {
            Cube.Bounds.Size = new Size(Bounds.Size);
            Debug.WriteLine($"{IsSelected}, {IsHighlighted}");
            if(IsSelected && IsHighlighted)
            {
                Cube.Bounds.Size.Depth = 0.01;
                Cube.Bounds.MoveToOrigin().Move(0, 0, -Bounds.Size.Depth/2);
            }
            Label.Bounds.Size = new Size(Bounds.Size);
            Label.Bounds.Size.Width *= 0.9;
            Label.Bounds.Size.Height *= 0.7;

            Label.Bounds.Pose = new CoordinateSystem(Cube.Bounds.Pose);
            Label.Bounds.Move(0, 0, Cube.Bounds.Size.Depth/2);
            Cube.MarkAsDirty("transform", "geometry");
            Label.MarkAsDirty("transform");
        }

        private void _updateLooks()
        {
            Layout();
        }

        public override void OnPointerEntered(Pointer pointer)
        {
            IsHighlighted = true;
        }
        public override void OnPointerExited(Pointer pointer)
        {
            IsHighlighted = false;
        }

        public override void OnTouchDown(Pointer pointer)
        {
            IsSelected = true;
        }

        public override void OnTouchUp(Pointer pointer)
        {
            IsSelected = false;
            if(IsHighlighted)
            {
                Activate(pointer.Hand);
            }
        }
    }
}
