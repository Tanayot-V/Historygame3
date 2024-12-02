using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace InnovaFramework.iGUI
{
    public class iRadioButton : iObject
    {
        public object                       key;
        public bool                         isChecked = false;
        public List<iRadioButton>           groups    = new List<iRadioButton>();
        public Action<iRadioButton, object> OnSelected;


        public iRadioButton(object key)
        {
            this.key = key;
            size     = new Vector2(64, EditorGUIUtility.singleLineHeight);
            groups.Add(this);
        }


        public override void Start()
        {
            base.Start();
            if (style == null)
            {
                style               = new GUIStyle(EditorStyles.radioButton);
                style.alignment     = TextAnchor.UpperLeft;
                style.contentOffset = new Vector2(5, -1.5f);
            }
        }


        public override void Render()
        {
            base.Render();

            if (!active) return;

            BeginProcessProperty();
            EditorGUI.BeginChangeCheck();

            isChecked = GUI.Toggle(rect, isChecked, new GUIContent(text, texture, tooltips), style);

            if(EditorGUI.EndChangeCheck())
            {
                if(isChecked)
                {
                    groups.ForEach( o => 
                    {
                        if (o != this)
                        {
                            o.isChecked = false;
                        }
                    });
                }

                if(OnSelected != null && isChecked)
                {
                    OnSelected(this, key);
                }
            }
            EndProcessProperty();
        }


        public void JoinGroup(iRadioButton other)
        {
            if (groups.Contains(other)) return;
            groups.Add(other);

            for(int i = 0; i < groups.Count; i++)
            {
                if (groups[i] == other)
                {
                    continue;
                }

                other.JoinGroup(groups[i]);
                groups[i].JoinGroup(other);
            }
        }
    }
}