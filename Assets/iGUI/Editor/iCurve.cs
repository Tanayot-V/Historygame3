using System;
using UnityEditor;
using UnityEngine;

namespace InnovaFramework.iGUI
{
    public class iCurve : iObject
    {
        public AnimationCurve  value;
        public Action<iObject> OnChanged;

        public iCurve()
        {
            size       = new Vector2(150, EditorGUIUtility.singleLineHeight);
            value      = new AnimationCurve();
            labelSpace = 50;
        }


        public override void Start()
        {
            base.Start();
        }


        public override void Render()
        {
            base.Render();

            if (!active) return;

            BeginProcessProperty();
            EditorGUI.BeginChangeCheck();

            value = EditorGUI.CurveField(rect, new GUIContent(text, texture, tooltips), value);
            
            if (EditorGUI.EndChangeCheck())
            {
                OnChanged?.Invoke(this);
            }
            EndProcessProperty();
        }
    }
}