using System;
using System.Diagnostics;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using MathNet.Spatial.Euclidean;

namespace AlloUI
{
    /// <summary>
    /// Simple stepper UI component with plus/minus buttons and a label displaying
    /// the current value in the middle.
    /// </summary>
    public class Stepper : View
    {
        /// <summary>
        /// Gets the plus button
        /// </summary>
        public Button Plus { get; private set; }

        /// <summary>
        /// Gets the minus button
        /// </summary>
        public Button Minus { get; private set; }

        /// <summary>
        /// Label displaying the current value in the middle.
        /// </summary>
        public Label Label { get; private set; }

        /// <summary>
        /// The default value the stepper is initialized with.
        /// </summary>
        public int DefaultValue { get; private set; }

        /// <summary>
        /// The current value of the stepper.
        /// </summary>
        public int CurrentValue { get; private set; }

        /// <summary>
        /// Size of the component
        /// </summary>
        public double Size { get; private set; }

        /// <summary>
        /// Constructs a stepper component given a size (height) of the
        /// buttons and a default (starting) value.
        /// </summary>
        /// <param name="size">Size (height) of the components.</param>
        /// <param name="defaultValue">Default value of the stepper.</param>
        public Stepper(double size, int defaultValue)
            : base()
        {
            //Initialize the components
            this.Plus = new Button();
            this.Minus = new Button();
            this.Label = new Label();

            //Append the components to the parent view.
            addSubview(this.Plus);
            addSubview(this.Minus);
            addSubview(this.Label);

            //Set the default and value and the size
            this.DefaultValue = defaultValue;
            this.CurrentValue = defaultValue;
            this.Size = size;

            //Subscribe to plus/minus events
            this.Plus.Action += PlusClick;
            this.Minus.Action += MinusClick;
        }

        private void MinusClick(object sender, Button.ActionArgs e)
        {
            //decrement current value and update UI
            this.CurrentValue--;
            UpdateLabel();
        }

        private void PlusClick(object sender, Button.ActionArgs e)
        {
            //increment current value and update UI
            this.CurrentValue++;
            UpdateLabel();
        }

        /// <summary>
        /// Updates the label text based on the current value.
        /// </summary>
        private void UpdateLabel()
        {
            this.Label.Text = this.CurrentValue.ToString();
        }

        public override void Layout()
        {
            double move = this.Size * 0.75;

            this.Plus.Bounds.Size = new Size(this.Size, this.Size, 0.10);
            this.Plus.Bounds.Move(move, 0, 0);
            this.Plus.Label.Text = "+";

            this.Minus.Bounds.Size = new Size(this.Size, this.Size, 0.10);
            this.Minus.Label.Text = "-";
            this.Minus.Bounds.Move(-move, 0, 0);

            this.Label.Bounds.Size = new Size(this.Size, this.Size, 0.10);

            UpdateLabel();
        }
    }
}
