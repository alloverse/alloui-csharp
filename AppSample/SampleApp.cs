using System;
using System.Diagnostics;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using AlloUI;

class SampleApp
{
    App app;
    static void Main(string[] args)
    {
         SampleApp sample = new SampleApp();
         sample.Run(args[0]);
    }

    void Run(string url)
    {
        app = new App(new AlloClient(), "csharp-appsample");
        app.mainView = MakeMainUI();
        app.Connect(url);
        app.Run(20);
    }

    View MakeMainUI()
    {
        Cube cube = new Cube{
            Bounds= new Bounds{
                Size= new Size(1.0, 1.0, 0.10)
            }.Move(0, 1.5, -2),
            Color= new Color(0.6, 0.4, 0, 1)
        };
        Label label = cube.addSubview(new Label{
            Bounds= new Bounds {
                Size= new Size(cube.Bounds.Size.Width, 0.1, 0.01)
            }.Move(0, cube.Bounds.Size.Height/2 - 0.12, cube.Bounds.Size.Depth/2.0),
            Text= "Hello World!"
        });

        Button button = cube.addSubview(new Button{
            Bounds= new Bounds {
                Size= new Size(0.6, 0.13, 0.1)
            }.Move(0, 0, cube.Bounds.Size.Depth/2.0 + 0.05),
        });
        button.Label.Text = "Do the thing";
        button.Action += delegate(object sender, Button.ActionArgs args) {
            button.Cube.Color = Color.Random();
        };

        Slider slider = cube.addSubview(new Slider{
            Bounds= new Bounds {
                Size= new Size(0.8, 0.13, 0.1)
            }.Move(0, -0.3, cube.Bounds.Size.Depth/2.0 + 0.05),
        });
        slider.Action += delegate(object sender, Slider.ActionArgs args) {
            label.Text = $"{args.Value}";
        };

        cube.IsGrabbable = true;
        return cube;
    }
}
