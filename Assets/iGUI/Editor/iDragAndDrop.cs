using UnityEngine;
using UnityEditor;
using System;


namespace InnovaFramework.iGUI
{

    public class iDragAndDrop : iObject
    {
        private iBox background;

        public bool            showBackground;
        public Action<iObject> OnDragUpdate;
        public Action<iObject> OnDrapPerform;


        public iDragAndDrop()
        {
            size           = new Vector2(100, 100);
            showBackground = true;
        }


        public override void Start()
        {
            if (style == null)
            {
                style = new GUIStyle(EditorStyles.helpBox);
            }

            background = new iBox();
        }


        public override void Render()
        {
            base.Render();

            if(!active) return;

            BeginProcessProperty();

            if (showBackground)
            {
                background.text = this.text;
                background.CopyProperty(this);
                background.CopyTransform(this);
                background.Render();
            }
            ProcessDND();

            EndProcessProperty();
        }


        private void ProcessDND()
        {
            if(!GUI.enabled) return;

            Event evt = Event.current;
            if (new Rect(position, size).Contains(Event.current.mousePosition))
            {
                if (evt.type == EventType.DragUpdated)
                {
                    if (OnDragUpdate != null)
                    {
                        OnDragUpdate(this);
                        Event.current.Use();
                    }
                }
                else if (evt.type == EventType.DragPerform)
                {
                    if (OnDrapPerform != null)
                    {
                        OnDrapPerform(this);
                        Event.current.Use();
                    }
                }
            }
        }
    }
}