using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace InnovaFramework.iGUI
{
    public class iZoomContainer : iObject
    {
        public List<iObject> children = new List<iObject>();

        private float _scale = 1;
        public float scale
        {
            set
            {
                _scale = value;
                if (_scale <= 0)
                {
                    _scale = 0.0001f;
                }
                GUI.changed = true;
            }
            get
            {
                return _scale;
            }
        }

        public Vector2 vanishingPoint = Vector2.zero;

        public iZoomContainer()
        {

        }


        public override void Render()
        {
            if(children.Count == 0) return;
            if(!active) return;

            GUI.enabled    = enabled;
            var matrix = iGUIUtility.BeginZoom(scale, vanishingPoint);
            for(int i = 0; i< children.Count; ++i)
            {
                var obj = children[i];
                if (!obj.active) continue;
                obj.Render();
            }
            iGUIUtility.ResetZoom(matrix);
            GUI.enabled = true;
        }


        public void ShowOnly(iObject[] filterItem)
        {
            HashSet<iObject> hsFilter = null;
            if (filterItem != null)
            {
                hsFilter = new HashSet<iObject>(filterItem);
            }

            for(int i = 0; i < children.Count; i++)
            {
                if (filterItem == null)
                {
                    children[i].active = true;
                }
                else
                {
                    children[i].active = hsFilter.Contains(children[i]);
                }
            }
            GUI.changed = true;
        }


        public iObject[] GetVisibleChild()
        {
            return children.FindAll( o => o.active).ToArray();
        }


        public void AddChild(iObject mObject)
        {
            if(children.Contains(mObject)) return;
            mObject.parent = this;
            mObject.window = window;
            children.Add(mObject);
        }


        public void RemoveChild(iObject mObject)
        {
            children.Remove(mObject);
        }


        public void RemoveChildAll()
        {
            children.Clear();
        }
    }
}