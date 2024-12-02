using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace InnovaFramework.iGUI
{
    public class iDropDown : iObject 
    {
        public List<string>     options { get; private set; }
        public Action<iObject>  OnChanged;

        private iLabel label;
        private Dictionary<string, object> data = new Dictionary<string, object>();

        public int index = 0;
        public object selectedObject
        {
            get
            {
                if(index == -1 || index >= options.Count)
                {
                    return null;
                }

                return data[options[index]];
            }
        }
        public string selectedItem
        {
            get
            {
                if(index == -1 || index >= options.Count)
                {
                    return "";
                }

                return options[index];
            }
        }



        public iDropDown()
        {
            label    = new iLabel();
            options  = new List<string>();
            size     = new Vector2(150, EditorGUIUtility.singleLineHeight);
            fontSize = 12;
        }


        public override void Start()
        {
            if (style == null)
            {
                style = new GUIStyle(EditorStyles.popup);
            }

            label.RelativePosition(iRelativePosition.LEFT, this);
            label.RelativePosition(iRelativePosition.CENTER_Y_OF , this);
        }


        protected override void BeginProcessProperty()
        {
            base.BeginProcessProperty();
            style.fixedHeight = size.y;
        }


        public override void Render()
        {
            base.Render();

            if (!active) return;

            BeginProcessProperty();

            Rect totalRect = new Rect(rect);
            if (!string.IsNullOrEmpty(text))
            {
                label.CopyProperty(this);
                label.SetText(text);

                float offset = label.width + iGUIUtility.space;
                totalRect.width -= offset;
                totalRect.x += offset;
                label.Render();
            }

            EditorGUI.BeginChangeCheck();

            index = EditorGUI.Popup(totalRect, index, options.ToArray(), style);

            if (EditorGUI.EndChangeCheck())
            {
                OnChanged?.Invoke(this);
            }

            EndProcessProperty();
        }


        public override void UpdateRelative()
        {
            base.UpdateRelative();
            label.UpdateRelative();
        }


        public int GetIndexByOptionName(string option)
        {
            return options.IndexOf(option);
        }


        public bool AddOption(string option, object value = null)
        {
            if (options.Contains(option)) return false;
            options.Add(option);
            data[option] = value;
            return true;
        }


        public void SetOptions(string[] items)
        {
            options.Clear();
            data.Clear();

            for(int i = 0; i < items.Length; i++)
            {
                AddOption(items[i]);
            }
        }


        public void RemoveOption(string option)
        {
            options.Remove(option);
            if(data.ContainsKey(option))
            {
                data.Remove(option);
            }
        }


        public void RemoveOptionAt(int index)
        {
            if(index < 0 || index >= options.Count)
            {
                return;
            }

            if(data.ContainsKey(options[index]))
            {
                data.Remove(options[index]);
            }

            options.RemoveAt(index);
        }


        public void ClearOption()
        {
            options.Clear();
            data.Clear();
        }
    }
}
