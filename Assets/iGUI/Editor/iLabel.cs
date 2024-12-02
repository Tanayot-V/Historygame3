using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace InnovaFramework.iGUI
{
    public class iLabel : iObject 
    {
        private bool cachedResize = false;
        public void Resize()
        {
            if(style == null)
            {
                style = new GUIStyle(EditorStyles.label);
            }

            if(style != null)
            {
                style.fontSize = fontSize;
                size = style.CalcSize(new GUIContent(text));
                GUI.changed = true;
            }
            cachedResize = false;
            UpdateRelative();
        }

        public void SetText(string text, bool autoSize = true)
        {
            this.text = text;
            if(autoSize)
            {
                try
                {
                    Resize();
                }
                catch
                {
                    cachedResize = true;
                }
            }
        }


        public override void Start()
        {
            if(style == null)
            {
                style = new GUIStyle(EditorStyles.label);
            }
        }


        public override void Render()
        {
            base.Render();

            if(!active) return;

            BeginProcessProperty();

            if(cachedResize)
            {
                Resize();
                UpdateRelative();
            }

            GUI.Label ( new Rect(position, size), new GUIContent(text, texture, tooltips), style);

            EndProcessProperty();
        }
    }
}
