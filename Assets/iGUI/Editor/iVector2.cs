using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace InnovaFramework.iGUI
{
    public class iVector2 : iObject 
    {
        public Vector2 value;
        public Action<iObject> OnChanged;

        public iVector2()
        {
            size = new Vector2(200, EditorGUIUtility.singleLineHeight * 2);
        }

        public override void Render()
        {
            base.Render();

            if (!active) return;
            BeginProcessProperty();
            EditorGUI.BeginChangeCheck();

            value = EditorGUI.Vector2Field(rect, new GUIContent(text, texture, tooltips), value);

            if (EditorGUI.EndChangeCheck())
            {
                OnChanged?.Invoke(this);
            }
            EndProcessProperty();
        }
    }
}