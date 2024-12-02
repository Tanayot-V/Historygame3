using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace InnovaFramework.iGUI
{
    public class iSlider : iObject 
    {
        public int intMin;
        public int intMax;
        public int intValue;

        public float floatMin;
        public float floatMax;
        public float floatValue;

        public Action<iObject> OnChanged;

        private iSliderType type = iSliderType.INT;


        public float value
        {
            get
            {
                return type == iSliderType.INT ? intValue : floatValue;
            }
        }


        public float precent
        {
            get
            {
                return value / (float)(type == iSliderType.INT ? intMax : floatMax);
            }
        }


        public iSlider(iSliderType type)
        {
            size       = new Vector2(200, EditorGUIUtility.singleLineHeight);
            this.type  = type;
            labelSpace = 50;
        }


        public override void Render()
        {
            if (!active) return;

            BeginProcessProperty();
            EditorGUI.BeginChangeCheck();

            if (type == iSliderType.INT)
            {
                intValue = EditorGUI.IntSlider(rect, new GUIContent(text, texture, tooltips), intValue, intMin, intMax);
            }
            else
            {
                floatValue = EditorGUI.Slider(rect, new GUIContent(text, texture, tooltips), floatValue, floatMin, floatMax);
            }

            if(EditorGUI.EndChangeCheck())
            {
                OnChanged?.Invoke(this);
            }
            EndProcessProperty();
        }


        public void Setup(double min, double max, double value)
        {
            switch(type)
            {
                case iSliderType.INT:
                    this.intMin   = (int)min;
                    this.intMax   = (int)max;
                    this.intValue = (int)value;
                    break;
                case iSliderType.FLOAT:
                    this.floatMin   = (float)min;
                    this.floatMax   = (float)max;
                    this.floatValue = (float)value;
                    break;
            }
        }
    }
}
