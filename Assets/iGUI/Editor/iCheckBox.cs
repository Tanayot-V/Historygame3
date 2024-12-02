using System;
using UnityEditor;
using UnityEngine;

namespace InnovaFramework.iGUI
{
    public class iCheckBox : iObject
    {
        public bool rightCheckBox = false;
        public bool isChecked     = false;
        public Action<iObject> OnChanged;


        public iCheckBox()
        {
            size = new Vector2(64, EditorGUIUtility.singleLineHeight);
            labelSpace = 50;
        }


        public override void Start()
        {
            base.Start();
            if (style == null)
            {
                style = new GUIStyle(EditorStyles.label);
                style.contentOffset = new Vector2(0, -1);
            }
        }


        public override void Render()
        {
            base.Render();

            if(!active) return;

            BeginProcessProperty();
            EditorGUI.BeginChangeCheck();

            if (rightCheckBox)
            {
                isChecked = EditorGUI.Toggle(rect, new GUIContent(text, texture, tooltips), isChecked, style);
            }
            else
            {
                isChecked = EditorGUI.ToggleLeft(rect, new GUIContent(text, texture, tooltips), isChecked, style);
            }

            if(EditorGUI.EndChangeCheck())
            {
                OnChanged?.Invoke(this);
            }
            EndProcessProperty();
        }
    }
}
