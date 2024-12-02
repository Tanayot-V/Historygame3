using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;


namespace InnovaFramework.iGUI
{
    public class iEnumPopup<T> : iObject where T : Enum
    {
        public T value;
        public Action<iObject> OnChanged;
        public bool isFlags;

        public iEnumPopup()
        {
            value      = default(T);
            size       = new Vector2(150, EditorGUIUtility.singleLineHeight);
            isFlags    = typeof(T).GetCustomAttributes<FlagsAttribute>().Any();
            labelSpace = 50;
        }


        public override void Start()
        {
            if (style == null)
            {
                style = new GUIStyle(EditorStyles.popup);
            }
        }


        public override void Render()
        {
            base.Render();

            if(!active) return;

            BeginProcessProperty();
            EditorGUI.BeginChangeCheck();

            if (isFlags)
            {
                value = (T)EditorGUI.EnumFlagsField(rect, new GUIContent(text, texture, tooltips), value, style);
            }
            else
            {
                value = (T)EditorGUI.EnumPopup(rect, new GUIContent(text, texture, tooltips), value, style) ;
            }

            if (EditorGUI.EndChangeCheck())
            {
                OnChanged?.Invoke(this);
            }
            EndProcessProperty();
        }

    }
}