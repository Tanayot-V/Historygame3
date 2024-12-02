using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace InnovaFramework.iGUI
{
    public class iFoldout : iObject
    {
        public bool isOpened = false;
        public Action<iObject> OnChanged;

        private List<iObject> children = new List<iObject>();

        public iFoldout()
        {
            size = new Vector2(64, 16);
        }


        public override void Start()
        {
            if (style == null)
            {
                style = EditorStyles.foldout;
            }
        }


        public override void Render()
        {
            base.Render();

            if(!active) return;

            BeginProcessProperty();
            EditorGUI.BeginChangeCheck();

            Rect r = new Rect(position, new Vector2(size.x, EditorGUIUtility.singleLineHeight));
            isOpened = EditorGUI.Foldout(r, isOpened, new GUIContent(text, texture, tooltips), style);

            if(EditorGUI.EndChangeCheck())
            {
                ForceUpdateHeight();
                OnChanged?.Invoke(this);
            }
            EndProcessProperty();

            if (isOpened)
            {
                foreach(var o in children)
                {
                    o.Render();
                }
            }
        }


        public void AddChild(iObject child)
        {
            if (!children.Contains(child))
            {
                child.parent = this;
                child.window = window;
                children.Add(child);
            }

            ForceUpdateHeight();
        }


        public void RemoveChild(iObject child)
        {
            if (children.Contains(child))
            {
                child.parent = null;
                children.Remove(child);
            }

            ForceUpdateHeight();
        }


        public void ForceUpdateHeight()
        {
            if (!isOpened)
            {
                size.y = 16;
                if (window != null)
                {
                    window.ReReletive();
                }
                return;
            }

            float minY = position.y;
            float maxY = position.y + size.y;

            foreach(var o in children)
            {
                if (o.position.y < minY)
                {
                    minY = o.position.y;
                }

                if (o.position.y + o.height > maxY)
                {
                    maxY = o.position.y + o.height;
                }
            }

            size.y = maxY - minY;

            if (window != null)
            {
                window.ReReletive();
            }
        }


        public override void UpdateRelative()
        {
            base.UpdateRelative();

            float tempHeight = size.y;
            size.y = 16;
            foreach(var o in children)
            {
                o.UpdateRelative();
            }
            size.y = tempHeight;
        }


        public override void Dispose()
        {
            base.Dispose();
            foreach(var kv in children)
            {
                kv.Dispose();
            }
        }
    }
}
