using System;
using System.Diagnostics;
using System.IO;
using System.Collections;
using System.Collections.Generic;

namespace AlloUI
{
    public class App
    {
        public AlloClient client {get;}
        public View mainView = new View();
        public List<View> rootViews {get; private set; } = new List<View>();
        
        public bool running {get; private set;} = true;
        public string appName {get; private set;}
        
        public App(AlloClient client, string appName)
        {
            this.client = client;
            this.appName = appName;
            client.onInteraction = this.routeInteraction;
            client.onDisconnected = this.clientWasDisconnected;
            client.onAdded = this.checkForAddedViewEntity;
        }

        public void Connect(string url)
        {
            this.rootViews.Add(this.mainView);

            AlloIdentity identity = new AlloIdentity();
            identity.display_name = this.appName;

            this.mainView.Layout();
            EntitySpecification mainViewSpec = this.mainView.Specification();

            this.client.Connect(url, identity, mainViewSpec);
            Debug.WriteLine("Connected");

            foreach(View view in this.rootViews)
            {
                view.app = this;
                if(view != this.mainView)
                {
                    this.client.SpawnEntity(view.Specification());
                }
            }
        }

        public void Run(int hz)
        {
            Console.CancelKeyPress += new ConsoleCancelEventHandler(delegate(object sender, ConsoleCancelEventArgs args)
            {
                Debug.WriteLine("Interrupted!");
                running = false;
                args.Cancel = true;
            });
            double timeout = 1/(double)hz;
            try {
                while(running)
                {
                    client.Poll(timeout);
                }
            } finally {
                Debug.WriteLine("Exiting...");
                client.Disconnect(0);
            }
        }

        public void AddRootView(View view)
        {
            this.rootViews.Add(view);
            if(this.client.connected)
            {
                view.app = this;
                view.Layout();
                this.client.SpawnEntity(view.Specification());
            }
        }

        public View FindView(string viewId)
        {
            foreach(View view in rootViews)
            {
                View found = view.FindView(viewId);
                if(found != null)
                {
                    return found;
                }
            }
            return null;
        }

        void routeInteraction(string type, AlloEntity sender, AlloEntity receiver, List<object> body)
        {
            if(receiver == null) return;
            if(receiver.components.ui == null) return;
            string vid = receiver.components.ui.view_id;
            View view = this.FindView(vid);
            if(view != null)
            {
                view.OnInteraction(type, body, sender);
            } 
            else
            {
                Debug.WriteLine($"Warning: Got interaction {body[0].ToString()} for nonexistent vid {vid} receiver {receiver.id} sender {sender.id}");
            }
        }

        void clientWasDisconnected()
        {
            this.running = false;
        }

        void checkForAddedViewEntity(AlloEntity entity)
        {
            if(entity.components.ui == null) return;

            View matchingView = FindView(entity.components.ui.view_id);
            if(matchingView != null)
            {
                matchingView.Entity = entity;
                matchingView.Awake();
            }
        }
    }
}
