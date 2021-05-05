using System;
using System.Diagnostics;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using MathNet.Spatial.Euclidean;

namespace AlloUI
{
    public class Slider : View
    {
        private double _minValue = 0.0;
        public double MinValue {
            get { return _minValue; }
            set { _minValue = value; Layout(); }
        }
        private double _maxValue = 1.0;
        public double MaxValue {
            get { return _maxValue; }
            set { _maxValue = value; Layout(); }
        }
        private double _currentValue;
        public double CurrentValue {
            get { return _currentValue; }
            set { _currentValue = value; Layout(); }
        }

        public Cube Track { get; private set; } = new Cube();
        public Cube Knob { get; private set; } = new Cube();

        public Slider()
        {
            IsPointable = true;
            
            addSubview(Track);
            //Track.Color = new Color(0, 0, 0, 1);
            
            addSubview(Knob);
            Knob.Color = new Color(0.5, 0, 0.4, 1);
        }

        public override void Layout()
        {
            Track.Bounds.Size = new Size(Bounds.Size);
            Track.Bounds.Size.Height /= 2.0;
            Track.Bounds.Size.Depth /= 2.0;
            //Track.Bounds.MoveToOrigin().Move(0, 0, -Track.Bounds.Size.Depth);

            Knob.Bounds.Size = new Size(Bounds.Size);
            Knob.Bounds.Size.Width = Knob.Bounds.Size.Height;
            double fraction = (_currentValue - _minValue) / (_maxValue - _minValue);
            //Debug.WriteLine($"{_currentValue} - {_minValue} / ({_maxValue} - {_minValue}) = {fraction}");
            double x = fraction * Bounds.Size.Width - Bounds.Size.Width/2;
            Knob.Bounds.MoveToOrigin().Move(x, 0, 0);

            Track.MarkAsDirty("transform");
            Knob.MarkAsDirty("transform");
        }

        public class ActionArgs : EventArgs
        {
            public AlloEntity Sender { get; internal set; }
            public double Value { get; internal set; }
        }
        public event EventHandler<ActionArgs> Action;
        public void Activate(AlloEntity sender, double value)
        {
            Action?.Invoke(this, new ActionArgs { Sender = sender, Value = value });
        }

        override public void OnTouchDown(Pointer pointer)
        {
            Point3D localPoint = ConvertPointFromView(pointer.PointedTo ?? new Point3D(0,0,0), null);
            double fraction = (localPoint.X + Bounds.Size.Width/2) / Bounds.Size.Width;

            Debug.WriteLine($"{localPoint}, {fraction}");

            if(fraction < 0 || fraction > 1) return;
            
            double newValue = _minValue + (fraction * (_maxValue - _minValue));
            
            CurrentValue = newValue;
            Activate(pointer.Hand, newValue);
        }

        override public void OnPointerMoved(Pointer pointer)
        {
            if(pointer.State == PointerState.Touching)
            {
                OnTouchDown(pointer);
            }
        }


    }
}
