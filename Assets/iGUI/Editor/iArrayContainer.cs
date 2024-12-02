using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;


namespace InnovaFramework.iGUI
{
    public class iArrayContainer<T> : iObject where T : iObject
    {
        public bool     showIndex;
        public float    indexWidth;
        public float    fixedHeight;
        public string   messageDelete;
        public iPadding padding;

        public Action<iArrayContainer<T>>                         OnChanged;
        public Action<iArrayContainer<T>>                         OnRemoved;
        public Func<iArrayContainerItem<T>, bool>                 OnRemoving;
        public Action<iArrayContainer<T>, iArrayContainerItem<T>> OnAdded;

        private iBox        boxBackground;
        private iFoldout    foldout;
        private iLabel      txtCount;
        private iLabel      labelEmpty;
        private iScrollView scvCanvas;
        private iButton     btnAdd;

        private Func<T>                      builderFunction;
        private Dictionary<T, int>           cachedItemToIndex;
        private List<iArrayContainerItem<T>> children;


        public int Count
        {
            get { return children.Count; }
        }


        public bool isOpened
        {
            get { return foldout.isOpened; }
            set
            {
                foldout.isOpened = value;
                OnFoldoutChange(foldout);
            }
        }


        public T this[int index]
        {
            get
            {
                return children[index].main;
            }
        }


        public int this[T item]
        {
            get
            {
                if (cachedItemToIndex.ContainsKey(item))
                {
                    return cachedItemToIndex[item];
                }

                return -1;
            }
        }


        public T[] Items
        {
            get
            {
                return children.Select(o => o.main).ToArray();
            }
        }


        public iArrayContainerItem<T>[] RawItems
        {
            get
            {
                return children.ToArray();
            }
        }


        public iArrayContainer()
        {
            boxBackground = new iBox();
            foldout       = new iFoldout();
            txtCount      = new iLabel();
            labelEmpty    = new iLabel();
            scvCanvas     = new iScrollView();
            btnAdd        = new iButton();

            children          = new List<iArrayContainerItem<T>>();
            cachedItemToIndex = new Dictionary<T, int>();

            showIndex            = true;
            indexWidth           = 0;
            fixedHeight          = 0;
            size                 = new Vector2(250, EditorGUIUtility.singleLineHeight);
            padding              = new iPadding(0, 0, 0, 0, 4);
            boxBackground.size.y = 24;
            messageDelete        = "Do you want to delete?";
        }


        public override void Start()
        {
            base.Start();

            foldout.style = EditorStyles.foldout;
            foldout.style.fontStyle = FontStyle.Bold;
            foldout.RelativePosition(iRelativePosition.LEFT, this);
            foldout.RelativePosition(iRelativePosition.TOP , this);

            btnAdd.size  = new Vector2(24, EditorGUIUtility.singleLineHeight);
            btnAdd.text  = "✚";
            btnAdd.RelativePosition(iRelativePosition.RIGHT, this);
            btnAdd.RelativePosition(iRelativePosition.CENTER_Y_OF, foldout);

            txtCount.SetText(children.Count.ToString());
            txtCount.RelativePosition(iRelativePosition.LEFT_OF    , btnAdd);
            txtCount.RelativePosition(iRelativePosition.CENTER_Y_OF, btnAdd);

            boxBackground.size.x = width;
            boxBackground.RelativePosition(iRelativePosition.BOTTOM_OF, this, iGUIUtility.spaceHalf);
            boxBackground.RelativePosition(iRelativePosition.LEFT     , this);

            scvCanvas.size         = boxBackground.size;
            scvCanvas.position     = boxBackground.size.space(1);
            scvCanvas.autoSizeMode = iScrollViewAutoSize.HORIZONTAL;
            scvCanvas.direction    = iScrollViewDirection.VERTICAL;
            scvCanvas.padding      = padding;
            scvCanvas.RelativePosition(iRelativePosition.CENTER_X_OF, boxBackground);
            scvCanvas.RelativePosition(iRelativePosition.CENTER_Y_OF, boxBackground);

            labelEmpty.SetText("List is Empty");
            labelEmpty.RelativePosition(iRelativePosition.LEFT_IN, boxBackground, iGUIUtility.spaceHalf);
            labelEmpty.RelativePosition(iRelativePosition.TOP_IN,  boxBackground, iGUIUtility.spaceHalf);

            foldout.OnChanged = OnFoldoutChange;
            btnAdd.OnClicked  = (e) => { Add(); };
        }


        public override void Render()
        {
            base.Render();

            foldout.text         = text;
            boxBackground.size.x = width;
            scvCanvas.size.x     = boxBackground.width;
            scvCanvas.padding    = padding;

            btnAdd.CopyProperty(this);
            foldout.CopyProperty(this);
            txtCount.CopyProperty(this);

            btnAdd.Render();
            foldout.Render();
            txtCount.Render();

            if (foldout.isOpened)
            {
                boxBackground.Render();
                scvCanvas.children.ForEach( o => 
                {
                    o.enabled = this.enabled;
                });
                if (scvCanvas.GetVisibleChild().Length == 0)
                {
                    labelEmpty.Render();
                }
                ReChildrenPosition();
                scvCanvas.Render();
            }
        }


        public override void UpdateRelative()
        {
            base.UpdateRelative();
            foldout.UpdateRelative();
            txtCount.UpdateRelative();

            float height = size.y;
            size.y       = EditorGUIUtility.singleLineHeight;
            boxBackground.UpdateRelative();
            labelEmpty.UpdateRelative();
            scvCanvas.UpdateRelative();
            size.y       = height;
        }


        public override void Dispose()
        {
            base.Dispose();
            foreach(var kv in children)
            {
                kv.main.Dispose();
            }
        }


        public void Builder(Func<T> builder)
        {
            this.builderFunction = builder;
        }


        public void ShowOnly(iObject[] filterItem)
        {
            scvCanvas.ShowOnly(filterItem);
            txtCount.SetText(scvCanvas.GetVisibleChild().Length.ToString());
            Update();
        }


        public iArrayContainerItem<T>[] Add(int count = 1, bool executeCallback = true)
        {
            if (builderFunction == null) return null;

            List<iArrayContainerItem<T>> items = new List<iArrayContainerItem<T>>();

            for(int i = 0; i < count; i++)
            {
                T item = builderFunction();

                iArrayContainerItem<T> arrayItem = new iArrayContainerItem<T>(item, this);
                children.Add(arrayItem);
                scvCanvas.AddChild(arrayItem);
                items.Add(arrayItem);

                Update();

                if (executeCallback)
                {
                    OnChanged?.Invoke(this);
                    OnAdded?.Invoke(this, arrayItem);
                }
            }

            return items.ToArray();
        }


        public void Update()
        {
            txtCount.SetText(scvCanvas.GetVisibleChild().Length.ToString());
            UpdateLabelIndex();
            ReChildrenPosition();
        }


        public void RemoveAt(int index, bool executeCallback = true)
        {
            bool isContinueDelete = true;
            if (index < 0 && index >= children.Count)
            {
                return;
            }

            if (executeCallback)
            {
                if (OnRemoving != null)
                {
                    isContinueDelete = OnRemoving(children[index]);
                }
            }

            if (!isContinueDelete)
            {
                return;
            }

            scvCanvas.RemoveChild(children[index]);
            children.RemoveAt(index);

            txtCount.SetText(children.Count.ToString());
            UpdateLabelIndex();
            ReChildrenPosition();

            if (executeCallback)
            {
                OnChanged?.Invoke(this);
                OnRemoved?.Invoke(this);
            }
        }


        public void RemoveAll(bool executeCallback = true)
        {
            scvCanvas.RemoveChildAll();
            children.Clear();
            txtCount.SetText("0");
            ReChildrenPosition();

            if (executeCallback)
            {
                OnChanged?.Invoke(this);
                OnRemoved?.Invoke(this);
            }
        }


        private void UpdateLabelIndex()
        {
            indexWidth = 0;

            iArrayContainerItem<T>[] visibleObject = scvCanvas.GetVisibleChild().Cast<iArrayContainerItem<T>>().ToArray();
            for(int i = 0; i < visibleObject.Length; i++)
            {
                float w = visibleObject[i].SetIndex(i);
                if (w > indexWidth)
                {
                    indexWidth = w;
                }
            }
        }


        public void ReChildrenPosition()
        {
            float minHeight = foldout.height + iGUIUtility.spaceHalf + 24f;
            float height    = iGUIUtility.spaceHalf;

            cachedItemToIndex.Clear();
            iArrayContainerItem<T>[] visibleObject = scvCanvas.GetVisibleChild().Cast<iArrayContainerItem<T>>().ToArray();
            for(int i = 0; i < visibleObject.Length; i++)
            {
                cachedItemToIndex[visibleObject[i].main] = i;
                visibleObject[i].ReChildrenPosition();
                height += visibleObject[i].height + (padding.space);
            }

            if (visibleObject.Length == 0)
            {
                height = 24f;
            }
            else
            {
                height += scvCanvas.padding.top + scvCanvas.padding.bottom;
            }

            if (foldout.height + iGUIUtility.spaceHalf + height > fixedHeight && fixedHeight > minHeight)
            {
                height = fixedHeight - foldout.height - iGUIUtility.spaceHalf;
            }

            boxBackground.size.y = height;
            scvCanvas.size       = boxBackground.size.space(1);
            scvCanvas.RePositionWithLastTwoRelativePosition();
            ForceUpdateHeight();
        }


        public void ForceUpdateHeight()
        {
            if (!foldout.isOpened)
            {
                size.y = EditorGUIUtility.singleLineHeight;
                window?.ReReletive();
                return;
            }

            float minY = position.y;
            float maxY = 0;

            maxY   = boxBackground.bottom;
            size.y = maxY - minY;

            window?.ReReletive();
        }


        private void OnFoldoutChange(iObject sender)
        {
            ReChildrenPosition();
        }
    }
}