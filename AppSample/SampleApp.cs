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
            }.Move(0, 1.5, 0),
            Color= new Color(0.6, 0.4, 0, 1)
        };
        Label label = cube.addSubview(new Label{
            Bounds= new Bounds {
                Size= new Size(cube.Bounds.Size.Width, 0.1, 0.01)
            }.Move(0, cube.Bounds.Size.Height/2 - 0.12, cube.Bounds.Size.Depth/1.9),
            Text= "Hello World!"
        });

        cube.IsGrabbable = true;
        return cube;
    }
}
