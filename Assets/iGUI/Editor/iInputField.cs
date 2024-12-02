using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace InnovaFramework.iGUI
{
    public class iInputField : iObject
    {
        public int                intValue;
        public float              floatValue;
        public string             stringValue;
        public UnityEngine.Object objectValue;

        public Type            typeObject;
        private iInputType     inputType;
        public Action<iObject> OnChanged;

        public object value
        {
            get
            {
                switch(inputType)
                {
                    case iInputType.DELAYED_INT:
                    case iInputType.INT:      return intValue;
                    case iInputType.DELAYED_FLOAT:
                    case iInputType.FLOAT:    return floatValue;
                    case iInputType.DELAYED_STRING:
                    case iInputType.STRING:
                    case iInputType.PASSWORD: return stringValue;
                    case iInputType.OBJECT:   return objectValue;
                }

                return null;
            }
        }

        public iInputField(iInputType inputType, Type objectType = null)
        {
            this.inputType = inputType;

            size        = new Vector2(400, EditorGUIUtility.singleLineHeight);
            text        = "";
            intValue    = 0;
            floatValue  = 0;
            stringValue = "";
            objectValue = null;
            labelSpace  = 50;

            if (objectType != null)
            {
                typeObject = objectType;
            }
        }

        public override void Start()
        {
            if (style == null)
            {
                style = new GUIStyle(EditorStyles.textField);
                style.alignment = TextAnchor.UpperLeft;
            }
        }

        public override void Render()
        {
            base.Render();

            if(!active) return;

            BeginProcessProperty();
            EditorGUI.BeginChangeCheck();

            switch(inputType)
            {
                case iInputType.INT:
                    {
                        intValue = EditorGUI.IntField(rect, new GUIContent(text, texture, tooltips), intValue, style);
                        break;
                    }
                case iInputType.FLOAT:
                    {
                        floatValue = EditorGUI.FloatField(rect, new GUIContent(text, texture, tooltips), floatValue, style);
                        break;
                    }
                case iInputType.STRING:
                    {
                        stringValue = EditorGUI.TextField(rect, new GUIContent(text, texture, tooltips), stringValue, style);
                        break;
                    }
                case iInputType.DELAYED_INT:
                    {
                        intValue = EditorGUI.DelayedIntField(rect, new GUIContent(text, texture, tooltips), intValue, style);
                        break;
                    }
                case iInputType.DELAYED_FLOAT:
                    {
                        floatValue = EditorGUI.DelayedFloatField(rect, new GUIContent(text, texture, tooltips), floatValue, style);
                        break;
                    }
                case iInputType.DELAYED_STRING:
                    {
                        stringValue = EditorGUI.DelayedTextField(rect, new GUIContent(text, texture, tooltips), stringValue, style);
                        break;
                    }
                case iInputType.PASSWORD:
                    {
                        stringValue = EditorGUI.PasswordField(rect, new GUIContent(text, texture, tooltips), stringValue, style);
                        break;
                    }
                case iInputType.OBJECT:
                    {
                        objectValue = EditorGUI.ObjectField(rect, new GUIContent(text, texture, tooltips), objectValue, typeObject, true);
                        break;
                    }
            }

            if(EditorGUI.EndChangeCheck())
            {
                OnChanged?.Invoke(this);
            }
            EndProcessProperty();
        }
    }
}

