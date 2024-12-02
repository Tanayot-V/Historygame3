using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace InnovaFramework.iGUI
{
    public delegate void OniWindowInitialized();
    public class iWindow : EditorWindow 
    {
        public const int DEFAULT_BOTTOM_CHANNEL = -1;
        public const int DEFAULT_TOP_CHANNEL    = -2;

        public List<iObject>                  defaultsChannel = new List<iObject>();
        public Dictionary<int, List<iObject>> objectChannel   = new Dictionary<int, List<iObject>>();
        public List<iObject>                  lastsChannel    = new List<iObject>();

        public Rect rect;
        public int  channel = 0;
        public event OniWindowInitialized OnUIInitialized;

        protected bool isInitialized = false;

        private iObject _targetObject = new iObject();
        public iObject targetObject 
        {
            get
            {
                _targetObject.isWindow = true;
                _targetObject.position = rect.position;
                _targetObject.size     = rect.size;
                return _targetObject;
            }
        }


        public virtual void OnDisable()
        {
            defaultsChannel.ForEach( o => o.Dispose());
            foreach(var kv in objectChannel)
            {
                foreach(var i in kv.Value)
                {
                    i.Dispose();
                }
            }
            lastsChannel.ForEach(o => o.Dispose());
        }


        public void ReReletive()
        {
            foreach(var o in defaultsChannel)
            {
                o.UpdateRelative();
            }

            foreach(var kv in objectChannel)
            {
                foreach(var o in kv.Value)
                {
                    o.UpdateRelative();
                }
            }
        }


        public void AddChild(iObject obj, int channel = 0)
        {
            obj.window = this;

            if(channel == -1)
            {
                defaultsChannel.Add(obj);
                return;
            }

            if (channel == -2)
            {
                lastsChannel.Add(obj);
                return;
            }

            if(!objectChannel.ContainsKey(channel))
            {
                objectChannel[channel] = new List<iObject>();               
            }

            objectChannel[channel].Add(obj);
        }


        public void RemoveChild(iObject obj, int channel = 0) 
        {
            if(channel == -1)
            {
                defaultsChannel.Remove(obj);
                return;
            }

            if(channel == -2)
            {
                lastsChannel.Remove(obj);
                return;
            }

            if(!objectChannel.ContainsKey(channel))
            {
                return;
            }

            objectChannel[channel].Remove(obj);
        }


        public void Render()
        {
            if (!isInitialized)
            {
                isInitialized = true;
                OnInitializeUI();
                OnAfterInitializedUI();
                OnUIInitialized?.Invoke();
            }

            iEvent.ProcessEvent(Event.current);

            // Render Buttom Default Channel
            foreach(iObject obj in defaultsChannel)
            {
                obj.Render();
            }

            // Render Normal
            if(objectChannel.ContainsKey(channel))
            {
                for(int i = 0; i < objectChannel[channel].Count; i++)
                {
                    objectChannel[channel][i].Render();
                }
            }

            // Render Top Default Channel
            foreach(iObject obj in lastsChannel)
            {
                obj.Render();
            }

            if(GUI.changed)
            {
                Repaint();
            }
        }

        protected virtual void OnInitializeUI() { }
        protected virtual void OnAfterInitializedUI() { }
    }
}
