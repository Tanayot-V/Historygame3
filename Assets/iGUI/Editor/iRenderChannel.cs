using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InnovaFramework.iGUI
{
    public class iRenderChannel : iObject, IiObjectContainer
    {
        private int _channel = 0;
        public int channel
        {
            get 
            {
                return _channel;
            }
            set
            {
                _channel    = value;
                GUI.changed = true;
                OnChange?.Invoke(this);
            }
        }

        public Action<iObject> OnChange = null;

        private Dictionary<iObject, List<int>> cachedObj2Index = new Dictionary<iObject, List<int>>();
        private Dictionary<int, List<iObject>> channelObjects  = new Dictionary<int, List<iObject>>();

        public override void Render()
        {
            base.Render();

            if (!active) return;

            if (channelObjects.ContainsKey(channel))
            {
                foreach(var obj in channelObjects[channel])
                {
                    obj.Render();
                }
            }
        }


        public override void UpdateRelative()
        {
            base.UpdateRelative();
            foreach(var kv in cachedObj2Index)
            {
                kv.Key.UpdateRelative();
            }
        }


        public override void Dispose()
        {
            base.Dispose();
            foreach(var kv in cachedObj2Index)
            {
                kv.Key.Dispose();
            }
        }


        public void AddChild(iObject obj, int index)
        {
            if (!channelObjects.ContainsKey(index))
            {
                channelObjects[index] = new List<iObject>();
            }

            if (channelObjects[index].Contains(obj))
            {
                return;
            }

            if (!cachedObj2Index.ContainsKey(obj))
            {
                cachedObj2Index[obj] = new List<int>();
            }

            channelObjects[index].Add(obj);
            cachedObj2Index[obj].Add(index);
            obj.window = window;
            GUI.changed = true;
        }


        public void RemoveChild(iObject obj, int targetIndex = -99)
        {
            if (!cachedObj2Index.ContainsKey(obj))
            {
                return;
            }

            List<int> index = cachedObj2Index[obj];
            if (targetIndex == -99)
            {
                for(int i = 0; i < index.Count; i++)
                {
                    channelObjects[index[i]].Remove(obj);
                }
                cachedObj2Index.Remove(obj);
                obj.window = null;
            }
            else
            {
                if (index.Contains(targetIndex))
                {
                    cachedObj2Index[obj].Remove(targetIndex);
                    channelObjects[targetIndex].Remove(obj);

                    if (cachedObj2Index[obj].Count == 0)
                    {
                        cachedObj2Index.Remove(obj);
                        obj.window = null;
                    }
                }
            }
        }
    }
}