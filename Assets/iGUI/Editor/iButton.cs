using System;
using UnityEngine;
using UnityEditor;

namespace InnovaFramework.iGUI
{
    public class iButton : iObject
    {
        public Action<iObject> OnClicked;

        private bool _miniButton;
        public bool miniButton 
        {
            set
            {
                _miniButton = value;
                style = value ? new GUIStyle(EditorStyles.miniButton) : new GUIStyle(GUI.skin.button);
            }
            get
            {
                return _miniButton;
            }
        }

        public iButton()
        {
            size = new Vector2(64, EditorGUIUtility.singleLineHeight);
            fontSize = 12;
        }


        public override void Start()
        {
            if (style == null)
            {
                style = new GUIStyle(GUI.skin.button);
            }
        }


        public override void Render()
        {
            base.Render();

            if(!active) return;

            BeginProcessProperty();
            if(GUI.Button(rect, new GUIContent(text, texture, tooltips), style))
            {
                OnClicked?.Invoke(this);
            }
            EndProcessProperty();
        }
    }
}