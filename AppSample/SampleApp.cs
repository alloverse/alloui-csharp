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
        Surface root = new Surface{
            Bounds= new Bounds{
                Size= new Size(1.0, 1.0, 0.05)
            }.Move(0, 1.5, 0),
            Color= new Color(1, 0, 0, 1)
        };
        root.Color = new Color(0, 1, 0, 1);

        root.IsGrabbable = true;
        return root;
    }
}
