using System;
using System.Diagnostics;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using MathNet.Spatial.Euclidean;
using Newtonsoft.Json.Linq;

namespace AlloUI
{
    public class View
    {
        public string ViewId {get; private set;} = Guid.NewGuid().ToString("N");
        public Bounds Bounds = new Bounds(0, 0, 0, 1, 1, 1);
        public List<View> Subviews = new List<View>();
        public View Superview {get; private set;} = null;
        private App _app;
        public AlloEntity Entity {get; internal set;} = null;
        public bool IsGrabbable;
        public bool IsPointable;

        public View()
        {
        }

        public virtual void Layout() {}

        /// Override this to describe how your view is represented in the world
        public virtual EntitySpecification Specification()
        {
            EntitySpecification spec = new EntitySpecification();

            spec.components.ui = new Component.UI(ViewId);

            spec.components.transform.matrix = Bounds.Pose;

            if(Superview != null && Superview.IsAwake)
            {
                spec.components.relationships = new Component.Relationships(Superview.Entity.id);
            }
            if(IsGrabbable || IsPointable)
            {
                spec.components.collider = new Component.BoxCollider(Bounds.Size.Width, Bounds.Size.Height, Bounds.Size.Depth);
            }
            if(IsGrabbable)
            {
                spec.components.grabbable = new Component.Grabbable();
            }
            return spec;
        }

        public View FindView(string viewId)
        {
            if(viewId == this.ViewId)
            {
                return this;
            }

            foreach(View view in Subviews)
            {
                View found = view.FindView(viewId);
                if(found != null)
                {
                    return found;
                }
            }
            return null;
        }

        public T addSubview<T>(T subview) where T : View
        {
            Debug.Assert(subview.Superview == null);

            Subviews.Add(subview);
            subview.app = app;
            subview.Superview = this;
            if(IsAwake)
            {
                subview.Spawn();
            } // else, wait for awake()

            return subview; // for chaining
        }

        public void Spawn()
        {
            Debug.Assert(Superview != null && Superview.IsAwake);
            Layout();
            EntitySpecification spec = Specification();
            app.client.InteractRequest(
                Superview.Entity.id,
                "place", 
                new List<object>{"spawn_entity", spec}, 
                null
            );
        }

        /// Asks the backend to update the components representing this view for everybody on the server.
        /// Use this to update things you've specified in :specification() but now want to change.
        /// comps: A struct of the desired changes
        public void UpdateComponents(AlloComponents comps)
        {
            Debug.Assert(this.IsAwake);
            var body = new List<object>{
                "change_components",
                this.Entity.id,
                "add_or_change",
                comps,
                "remove",
                new List<string>()
            };
            app.client.InteractRequest(this.Entity.id, "place", body, null);
        }

        /// Mark one or more Components as needing to have their server-side value updated ASAP.
        public void MarkAsDirty(params string[] keys)
        {
            // everything is implicitly dirty and will be updated when View is Awakened
            if(!IsAwake)
            {
                return;
            }
            AlloComponents dirty = new AlloComponents();
            AlloComponents spec = this.Specification().components;
            foreach(string key in keys)
            {
                FieldInfo prop = typeof(AlloComponents).GetField(key);
                prop.SetValue(dirty, prop.GetValue(spec));
            }
            UpdateComponents(dirty);
        }

        public App app
        {
            get => _app;
            set
            {
                _app = value;
                foreach(View child in Subviews) 
                {
                    child.app = value;
                }
            }
        }

        
        /// Awake() is called when the entity that represents this view
        /// starts existing and is bound to this view.
        public void Awake()
        {
            foreach(View child in Subviews)
            {
                if(child.Entity == null)
                {
                    child.Spawn();
                }
            }
        }
        public bool IsAwake
        {
            get => this.Entity != null;
        }

        /// Callback called when a user grabs this view in order to move it.
        ///  The server will then update Entity.components.transform to match
        ///  where the user wants to move it continuously. There is no callback for
        ///  when the entity is moved.
        ///  @tparam Entity hand The hand entity that started the grab
        virtual public void OnGrabStarted(AlloEntity hand)
        {
        }

        /// Callback called when a user lets go of and no longer wants to move it.
        ///  @tparam Entity hand The hand entity that released the grab.
        virtual public void OnGrabEnded(AlloEntity hand)
        {
        }

        /// Callback for when a hand is interacting with a view. 
        ///  NOTE: You must set view.pointable=true, or the user's cursor will just
        ///  fall right through this view!
        /// 
        ///  This is a catch-all callback; there is also
        ///  OnPointerEntered, OnPointerMoved, OnPointerExited, OnTouchDown and OnTouchUp
        ///  if you want to react only to specific events.
        ///  @tparam table pointer A table with keys:
        ///   * `hand`: The hand entity that is doing the pointing
        ///   * `state`: "hovering", "outside" or "touching"
        ///   * `touching`: bool, whether the hand is currently doing a poke on this view
        ///   *`pointedFrom`: a vec3 in world coordinate space with the coordinates 
        ///                   of the finger tip of the hand pointing at this view.
        ///   * `pointedTo`: the point on this view that is being pointed at
        ///                  (again, in world coordinates).
        virtual public void OnPointerChanged(Pointer pointer)
        {
        }

        /// Callback for when a hand's pointer ray entered this view.
        ///   The `state` in pointer is now Hovering
        ///  @tparam Pointer pointer see OnPointerChanged.
        virtual public void OnPointerEntered(Pointer pointer)
        {
        }

        /// Callback for when a hand's pointer moved within this view.
        ///   The PointedFrom and PointedTo in pointer now likely have new values.
        ///  @tparam Pointer pointer see onPointerChanged.
        virtual public void OnPointerMoved(Pointer pointer)
        {
        }

        /// Callback for when the hand's pointer is no longer pointing within this view.
        ///   The `state` in pointer is now Outside
        ///  @tparam Pointer pointer see onPointerChanged.
        virtual public void OnPointerExited(Pointer pointer)
        {
        }

        /// Callback for when the hand's pointer is poking/touching this view
        ///   The `state` in pointer is now Touching
        ///  @tparam Pointer pointer see onPointerChanged.
        virtual public void OnTouchDown(Pointer pointer)
        {
        }

        /// Callback for when the hand's pointer stopped poking/touching this view.
        ///   This is a great time to invoke an action based on the touch.
        ///   For example, if you're implementing a button, this is where you'd 
        ///   trigger whatever you're trying to trigger.
        ///   NOTE: If pointer.state is now "outside", the user released
        ///   the trigger button outside of this view, and you should NOT
        ///   perform an action, but cancel it instead.
        ///  @tparam Pointer pointer see onPointerChanged.
        virtual public void OnTouchUp(Pointer pointer)
        {
        }

        /// Callback called when a file is dropped on the view
        /// @tparam string filename The name of the dropped file
        /// @tparam string asset_id The id of the asset dropped on you
        virtual public void OnFileDropped(string filename, string asset_id, AlloEntity senderAvatar)
        {
        }

        public virtual void OnInteraction(string type, List<object> body, AlloEntity sender)
        {
            if(body.Count < 1 || body[0].GetType() != typeof(string))
            {
                return;
            }
            string command = body[0] as string;

            if(command == "grabbing")
            {
                if((body[1] as bool?).GetValueOrDefault())
                {
                    OnGrabStarted(sender);
                } 
                else 
                {
                    OnGrabEnded(sender);
                }
            }
            else if(command == "point")
            {
                routePointing(body, sender);
            }
            else if(command == "point-exit")
            {
                routeEndPointing(body, sender);
            }
            else if(command == "poke")
            {
                routePoking(body, sender);
            }
            else if(command == "accept-file" && body[1].GetType() == typeof(string) && body[2].GetType() == typeof(string))
            {
                string filename = body[1] as string;
                string assetId = body[2] as string;
                OnFileDropped(filename, assetId, sender);
            }
        }
        
        Dictionary<string, Pointer> _pointers = new Dictionary<string, Pointer>();
        void routePointing(List<object> body, AlloEntity sender)
        {
            Pointer pointer;
            if(!_pointers.TryGetValue(sender.id, out pointer))
            {
                pointer = new Pointer{
                    Hand= sender,
                };
                _pointers[sender.id] = pointer;
            }

            var from = (body[1] as JArray).ToObject<List<double>>();
            var to = (body[2] as JArray).ToObject<List<double>>();
            pointer.PointedFrom = new Point3D(from[0], from[1], from[2]);
            pointer.PointedTo = new Point3D(from[0], from[1], from[2]);

            if(pointer.State == PointerState._Undetermined || pointer.State == PointerState.Outside)
            {
                pointer.State = PointerState.Hovering;
                OnPointerEntered(pointer);
            } 
            else
            {
                OnPointerMoved(pointer);
            }
            OnPointerChanged(pointer);
        }

        void routeEndPointing(List<object> body, AlloEntity sender)
        {
            Pointer pointer;
            if(!_pointers.TryGetValue(sender.id, out pointer)) { return; }
            
            var previousState = pointer.State;
            pointer.State = PointerState.Outside;
            pointer.PointedFrom = null;
            pointer.PointedTo = null;
            OnPointerExited(pointer);
            OnPointerChanged(pointer);
            if(previousState != PointerState.Touching)
            {
                _pointers.Remove(sender.id);
            }
        }

        void routePoking(List<object> body, AlloEntity sender)
        {
            Pointer pointer;
            if(!_pointers.TryGetValue(sender.id, out pointer)) { return; }

            if((body[1] as bool?).GetValueOrDefault())
            {
                pointer.State = PointerState.Touching;
                OnTouchDown(pointer);
            } 
            else 
            {
                if(pointer.PointedFrom != null)
                {
                    pointer.State = PointerState.Hovering;
                }
                else
                {
                    pointer.State = PointerState.Outside;
                    _pointers.Remove(sender.id);
                }
                OnTouchUp(pointer);
            }
            OnPointerChanged(pointer);
        }
    }

    /// Represents a pointing event from a user's avatar's hand onto a View.
    public class Pointer
    {
        /// Entity of the hand that is doing the pointing
        public AlloEntity Hand { internal set; get; }
        /// What is the current state of the pointing action?
        public PointerState State { internal set; get; }
        /// Where in world space is the tip of the finger doing the pointing?
        public Point3D? PointedFrom { internal set; get; }
        /// Where in world space is the ray cast from the finger hitting this view?
        public Point3D? PointedTo { internal set; get; }
    }

    public enum PointerState {
        /// Not visible outside of framework
        _Undetermined,

        /// This pointer is being tracked because it started inside this view, but it is currently outside it.
        Outside,
        /// This pointer is pointing somewhere inside this view's collider
        Hovering,
        /// This pointer is poking/touching this view, and some action should likely be taken 
        /// in response to the user's interaction.
        Touching,
    }
}

