using System;
using UnityEngine;
using UnityEditor;

namespace InnovaFramework.iGUI
{
    public class iColor : iObject
    {
        public Color value;
        public Action<iObject> OnChanged;

        public iColor() 
        {
            size       = new Vector2(150, EditorGUIUtility.singleLineHeight);
            value      = Color.black;
            labelSpace = 50;
        }


        public override void Render()
        {
            base.Render();

            if (!active) return;

            BeginProcessProperty();
            EditorGUI.BeginChangeCheck();

            value = EditorGUI.ColorField(rect, new GUIContent(text, texture, tooltips), value);

            if (EditorGUI.EndChangeCheck())
            {
                OnChanged?.Invoke(this);
            }
            EndProcessProperty();
        }
    }
}