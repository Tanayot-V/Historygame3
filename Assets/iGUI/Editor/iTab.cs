using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace InnovaFramework.iGUI
{
    public class iTab : iObject, IiObjectContainer
    {
        public int index = 0;

        public Action<iTab> OnChange = null;

        private List<GUIContent> contentHeaders = new List<GUIContent>();
        private Dictionary<iObject, List<int>> cachedObj2Index = new Dictionary<iObject, List<int>>();
        private Dictionary<int, List<iObject>> tabs = new Dictionary<int, List<iObject>>();


        public iTab()
        {
            size = new Vector2(24, EditorGUIUtility.singleLineHeight * 2);
        }


        public override void Render()
        {
            if (!active) return;

            BeginProcessProperty();
            EditorGUI.BeginChangeCheck();

            index = GUI.Toolbar(rect, index, contentHeaders.ToArray());

            if (EditorGUI.EndChangeCheck())
            {
                OnChange?.Invoke(this);
            }

            if (tabs.ContainsKey(index))
            {
                foreach (var obj in tabs[index])
                {
                    obj.Render();
                }
            }
            EndProcessProperty();
        }


        public GUIContent[] GetHeaders()
        {
            return contentHeaders.ToArray();
        }


        public List<GUIContent> GetRawHeaders()
        {
            return contentHeaders;
        }


        public void SetHeaders(params string[] texts)
        {
            contentHeaders.Clear();

            for (int i = 0; i < texts.Length; i++)
            {
                var content = new GUIContent();
                content.text = texts[i];
                contentHeaders.Add(content);
            }
        }


        public void SetHeaders(params Texture[] textures)
        {
            for (int i = 0; i < textures.Length; i++)
            {
                var content = new GUIContent();
                content.image = textures[i];
                contentHeaders.Add(content);
            }
        }


        public void SetHeaders(params GUIContent[] contents)
        {
            contentHeaders.Clear();
            contentHeaders.AddRange(contents);
        }


        public override void UpdateRelative()
        {
            base.UpdateRelative();

            foreach (var kv in cachedObj2Index)
            {
                kv.Key.UpdateRelative();
            }
        }


        public override void Dispose()
        {
            base.Dispose();
            foreach(var kv in cachedObj2Index)
            {
                kv.Key.Dispose();
            }
        }


        public void AddChild(iObject obj, int index)
        {
            if (!tabs.ContainsKey(index))
            {
                tabs[index] = new List<iObject>();
            }

            if (tabs[index].Contains(obj))
            {
                return;
            }

            if (!cachedObj2Index.ContainsKey(obj))
            {
                cachedObj2Index[obj] = new List<int>();
            }

            tabs[index].Add(obj);
            cachedObj2Index[obj].Add(index);
            obj.window = window;
            GUI.changed = true;
        }


        public void RemoveChild(iObject obj, int targetIndex = -99)
        {
            if (!cachedObj2Index.ContainsKey(obj))
            {
                return;
            }


            List<int> index = cachedObj2Index[obj];
            if (targetIndex == -99)
            {
                for (int i = 0; i < index.Count; i++)
                {
                    tabs[index[i]].Remove(obj);
                }
                cachedObj2Index.Remove(obj);
                obj.window = null;
            }
            else
            {
                if (index.Contains(targetIndex))
                {
                    cachedObj2Index[obj].Remove(targetIndex);
                    tabs[targetIndex].Remove(obj);

                    if (cachedObj2Index[obj].Count == 0)
                    {
                        cachedObj2Index.Remove(obj);
                        obj.window = null;
                    }
                }
            }
            GUI.changed = true;
        }
    }

}