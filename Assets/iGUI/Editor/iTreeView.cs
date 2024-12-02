using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace InnovaFramework.iGUI
{
    public enum iCheckState
    {
        Checked   = 1,
        Multiple  = 0,
        Unchecked = -1
    }


    public class iTreeViewItem : iObject
    {
        public bool isOpen      { get { return container.isOpen; }}
        public bool hasChildren { get { return container.children.Count > 0; }}

        public iTreeView          treeView;
        public iTreeViewNode container;

        private iButton   btnArrow;
        private iButton   btnCheckBox;
        private iBox      box;
        private iImageBox icon;
        private iLabel    label;

        private GUIStyle styleCheckbox_Unchecked;
        private GUIStyle styleCheckbox_Checked;
        private GUIStyle styleCheckbox_Multiple;


        public iTreeViewItem() 
        {
            label       = new iLabel();

            icon       = new iImageBox();
            icon.style = new GUIStyle();

            box                 = new iBox();
            box.style           = new GUIStyle();
            box.backgroundColor = new Color(0.17f, 0.36f, 0.52f);
            box.OnMouseDown    += (sender, evt) => treeView.ClickItem(container);

            styleCheckbox_Checked   = new GUIStyle();
            styleCheckbox_Unchecked = new GUIStyle();
            styleCheckbox_Multiple  = new GUIStyle();

            styleCheckbox_Checked.LoadClickableTexture("mat-2-icon.check_box");
            styleCheckbox_Unchecked.LoadClickableTexture("mat-2-icon.check_box_outline_blank");
            styleCheckbox_Multiple.LoadClickableTexture("mat-2-icon.indeterminate_check_box");

            btnArrow           = new iButton();
            btnArrow.size      = new Vector2(iGUIUtility.singleLineHeight, iGUIUtility.singleLineHeight);
            btnArrow.OnClicked = (sender) =>
            {
                bool state = !container.isOpen;
                if (iEvent.IsKeyPressed(KeyCode.LeftAlt))
                {
                    container.SetCollapedStateAll(state);
                }
                else
                {
                    container.isOpen = state;
                }
            };

            btnCheckBox               = new iButton();
            btnCheckBox.size          = new Vector2(iGUIUtility.singleLineHeight, iGUIUtility.singleLineHeight);
            btnCheckBox.useEventCallback  = true;
            btnCheckBox.OnMouseClick += (sender, evt) => container.OnClickChecked();

            this.size.y = iGUIUtility.singleLineHeight + 2;
            this.size.x = 400;
        }


        public void Setup(iTreeViewNode container, iTreeView treeView)
        {
            this.treeView  = treeView;
            this.container = container;
            this.container.connectedItem = this;

            label.color = container.textColor;

            btnArrow.LoadClickableTexture(isOpen ? "mat-1-icon.keyboard_arrow_down" : "mat-1-icon.keyboard_arrow_right");
            UpdateCheckBox();
            this.size.y = iGUIUtility.singleLineHeight + 2;
            this.size.x = 400;
        }


        public void UpdateCheckBox()
        {
            switch(container.checkState)
            {
                case iCheckState.Checked  : btnCheckBox.SetClickableTexture(styleCheckbox_Checked);   break;
                case iCheckState.Unchecked: btnCheckBox.SetClickableTexture(styleCheckbox_Unchecked); break;
                case iCheckState.Multiple : btnCheckBox.SetClickableTexture(styleCheckbox_Multiple);  break;
            }
        }


        public void UpdateSelection(bool select)
        {
            box.style.normal.background = select ? EditorGUIUtility.whiteTexture : null;
        }


        public override void Render()
        {
            base.Render();

            box.Render();

            if (hasChildren)
            {
                btnArrow.Render();
            }
            if (treeView.useCheckBox)
            {
                btnCheckBox.Render();
            }

            icon.Render();
            label.Render();
            box.ProcessEvent(Event.current);
        }


        public float Update()
        {
            btnArrow.RelativePosition(iRelativePosition.LEFT_IN    , this, container.Indent * 16);
            btnArrow.RelativePosition(iRelativePosition.CENTER_Y_OF, this);

            btnCheckBox.RelativePosition(iRelativePosition.RIGHT_OF   , btnArrow, 2);
            btnCheckBox.RelativePosition(iRelativePosition.CENTER_Y_OF, this);

            box.size = this.size;
            box.RelativePosition(iRelativePosition.LEFT       , this);
            box.RelativePosition(iRelativePosition.CENTER_Y_OF, this);

            icon.size  = new Vector2(iGUIUtility.singleLineHeight, iGUIUtility.singleLineHeight);
            icon.style = new GUIStyle();
            icon.SetImage(container.icon);
            icon.RelativePosition(iRelativePosition.RIGHT_OF   , treeView.useCheckBox ? btnCheckBox : btnArrow, 2);
            icon.RelativePosition(iRelativePosition.CENTER_Y_OF, this);

            label.SetText(container.text);
            label.color = container.textColor;
            label.RelativePosition(iRelativePosition.RIGHT_OF   , icon, 4);
            label.RelativePosition(iRelativePosition.CENTER_Y_OF, this);

            return label.right;
        }
    }


    public class iTreeViewNode
    {
        public int                 Indent { get; private set;}

        public object              tag;
        public object              attachment;
        public iTreeView           treeView;
        public iTreeViewNode       parent;
        public List<iTreeViewNode> children = new List<iTreeViewNode>();
        public iTreeViewItem       connectedItem;

        private Color _textColor = Color.white;
        public  Color  textColor
        {
            get { return _textColor; }
            set
            {
                _textColor = value;
                connectedItem?.Update();
            }
        }

        private string _text;
        public  string  text
        {
            get { return _text; }
            set 
            {
                _text = value;
                connectedItem?.Update();
            }
        }

        private Texture2D _icon;
        public  Texture2D  icon
        {
            get { return _icon; }
            set 
            {
                _icon = value;
                connectedItem?.Update();
            }
        }

        private bool _isOpen = false;
        public  bool isOpen
        {
            get { return _isOpen; }
            set 
            {
                _isOpen = value;
                if (children.Count == 0) return;
                treeView.UpdateItem();
                treeView.OnToggleStateChanged?.Invoke(treeView, this);
            }
        }

        private iCheckState _checkState = iCheckState.Unchecked;
        public  iCheckState checkState
        {
            get { return _checkState; }
            set 
            {
                _checkState = value;
                if (parent != null)
                {
                    parent.UpdateCheckStateByChildren();
                }
                treeView.UpdateCheckState();
            }
        }


        public iTreeViewNode() { }
        public iTreeViewNode(Texture2D icon, string text, int indent, object tag, iTreeView treeView, iTreeViewNode parent = null, object attachment = null)
        {
            this.tag        = tag;
            this.icon       = icon;
            this.text       = text;
            this.Indent     = indent;
            this.parent     = parent;
            this.treeView   = treeView;
            this.attachment = attachment;
        }


        public iTreeViewNode AddChild(Texture2D icon, string text, object tag = null, object attachment = null)
        {
            iTreeViewNode item = treeView.ObjectPoolGetNode();
            item.tag        = tag;
            item.icon       = icon;
            item.text       = text;
            item.Indent     = Indent + 1;
            item.treeView   = treeView;
            item.parent     = this;
            item.attachment = attachment;
            children.Add(item);
            treeView.UpdateItem();
            return item;
        }


        public void RemoveThisNode()
        {
            treeView.ObjectPoolRemoveNode(this);

            if (parent == null)
            {
                treeView.RootNode = null;
            }
            else
            {
                parent.children.Remove(this);
                parent.UpdateCheckStateByChildren();
            }

            treeView.UpdateItem();
        }


        public void OnClickChecked()
        {
            if (children.Count == 0)
            {
                _checkState = _checkState == iCheckState.Unchecked ? iCheckState.Checked : iCheckState.Unchecked;
            }
            else
            {
                SetStateAll(_checkState == iCheckState.Checked ? iCheckState.Unchecked : iCheckState.Checked);
            }

            parent?.UpdateCheckStateByChildren();
            treeView.CheckBoxChanged(this);
        }


        public void SetStateAll(iCheckState state)
        {
            _checkState = state;
            children.ForEach( o => o.SetStateAll(state));
        }


        public void SetCollapedStateAll(bool state)
        {
            if (children.Count == 0) return;
            isOpen = state;
            children.ForEach( o => o.SetCollapedStateAll(state));   
        }


        public void UpdateCheckStateByChildren()
        {
            if (children.All( o => o._checkState == iCheckState.Checked))
            {
                _checkState = iCheckState.Checked;
            }
            else if (children.All( o => o._checkState == iCheckState.Unchecked))
            {
                _checkState = iCheckState.Unchecked;
            }
            else
            {
                _checkState = iCheckState.Multiple;
            }

            parent?.UpdateCheckStateByChildren();
        }
    }


    public class iTreeView : iObject
    {
        private iScrollView scrollViewContainer;
        private iBox background;
        private iBox footer;
        private List<iTreeViewNode> _selectedItems = new List<iTreeViewNode>();
        private Queue<iTreeViewNode> poolingObject = new Queue<iTreeViewNode>();
        private Queue<iTreeViewItem> poolingItems  = new Queue<iTreeViewItem>();
        private Dictionary<object, iTreeViewNode> cachedNode;

        // Properties
        public bool                              useCheckBox       = false;
        public bool                              multipleSelection = false;
        public Action<iObject>                   OnSelectItemChanged;
        public Action<iObject, iTreeViewNode>    OnCheckboxStateChanged;
        public Action<iObject, iTreeViewNode>    OnToggleStateChanged;

        public iTreeViewNode   RootNode      { get; internal set; }
        public iTreeViewNode[] SelectedItems { get { CleanSelected(); return _selectedItems.ToArray(); } }
        public iTreeViewNode[] CheckBoxes
        {
            get
            {
                List<iTreeViewNode> cb = new List<iTreeViewNode>();
                GetChecked(RootNode);
                void GetChecked(iTreeViewNode item)
                {
                    if (item == null) return;
                    if (item.checkState == iCheckState.Checked)
                    {
                        cb.Add(item);
                    }

                    for(int i = 0; i < item.children.Count; i++)
                    {
                        GetChecked(item.children[i]);
                    }
                }

                return cb.ToArray();
            }
        }

        public iTreeView()
        {
            cachedNode          = new Dictionary<object, iTreeViewNode>();
            background          = new iBox();
            footer              = new iBox();
            scrollViewContainer = new iScrollView();
        }


        public iTreeViewNode AddChildRoot(Texture2D icon, string text, object tag = null, object attachment = null)
        {
            RootNode = new iTreeViewNode(icon, text, 0, tag, this, null, attachment);
            UpdateItem();
            return RootNode;
        }


        public iTreeViewNode GetNode(object tag)
        {
            if (tag  == null) return null;
            if (!cachedNode.ContainsKey(tag)) return null;
            return cachedNode[tag];
        }


        public void Clear()
        {
            RootNode = null;
            UpdateItem();
        }


        public override void Start()
        {
            background.RelativeSize(() => this.size );
            background.RelativePosition(iRelativePosition.CENTER_X_OF, this);
            background.RelativePosition(iRelativePosition.CENTER_Y_OF, this);

            footer.text = "Deselect";
            footer.useEventCallback = true;
            footer.backgroundColor = iGUIUtility.COLOR_HEADER;
            footer.RelativeSize(() => new Vector2(this.width, 20));
            footer.RelativePosition(iRelativePosition.BOTTOM_IN  , background, 0);
            footer.RelativePosition(iRelativePosition.CENTER_X_OF, background);
            footer.OnMouseClick += (sender, evt) => 
            {
                _selectedItems.Clear();
                UpdateItem();
                OnSelectItemChanged?.Invoke(this);
            };

            scrollViewContainer.direction    = iScrollViewDirection.VERTICAL;
            scrollViewContainer.autoSizeMode = iScrollViewAutoSize.NONE;
            scrollViewContainer.RelativeSize ( () => scrollViewContainer.size = new Vector2(background.width.space(), iGUIUtility.HeightBetween2Objects(background.position.y, footer)) );
            scrollViewContainer.RelativePosition(iRelativePosition.TOP_OF, footer);
            scrollViewContainer.RelativePosition(iRelativePosition.CENTER_X_OF, background);
        }


        public override void Render()
        {
            base.Render();

            background.Render();
            footer.Render();
            scrollViewContainer.Render();
        }


        public void UpdateItem()
        {
            scrollViewContainer.children.ForEach( o => ObjectPoolRemoveItem(o as iTreeViewItem));
            scrollViewContainer.RemoveChildAll();
            cachedNode.Clear();

            if (RootNode == null) return;

            float maxSize = 0;

            DrawNodeItem(RootNode);
            UpdateCached(RootNode);
            void DrawNodeItem(iTreeViewNode node)
            {
                iTreeViewItem item = ObjectPoolGetItem();
                item.Setup(node, this);
                scrollViewContainer.AddChild(item);

                float size = item.Update();
                if (size > maxSize)
                {
                    maxSize = size;
                }

                if (node.isOpen)
                {
                    for(int i = 0; i < node.children.Count; i++)
                    {
                        DrawNodeItem(node.children[i]);
                    }
                }
            }

            void UpdateCached(iTreeViewNode node)
            {
                if (node.tag != null)
                {
                    cachedNode[node.tag] = node;
                }
                for(int i = 0; i < node.children.Count; i++)
                {
                    UpdateCached(node.children[i]);
                }
            }

            maxSize = Mathf.Max(maxSize + 4, scrollViewContainer.width);
            scrollViewContainer.children.ForEach( o => o.size.x = maxSize);
            UpdateSelection();

            GUI.changed = true;
        }


        internal void ClickItem(iTreeViewNode container)
        {
            CleanSelected();

            if (iEvent.IsKeyPressed(KeyCode.LeftControl) && multipleSelection)
            {
                if (_selectedItems.Contains(container))
                {
                    _selectedItems.Remove(container);
                }
                else
                {
                    _selectedItems.Add(container);
                }
            }
            else if (iEvent.IsKeyPressed(KeyCode.LeftShift) && multipleSelection)
            {
                if (_selectedItems.Count == 0)
                {
                    _selectedItems.Add(container);
                }
                else
                {
                    iTreeViewNode lastItem = _selectedItems[_selectedItems.Count - 1];

                    int lastIndex   = scrollViewContainer.children.FindIndex( o => ((iTreeViewItem)o).container == lastItem);
                    int selectIndex = scrollViewContainer.children.FindIndex( o => ((iTreeViewItem)o).container == container);

                    if (selectIndex > lastIndex)
                    {
                        selectIndex = selectIndex ^ lastIndex ^ (lastIndex = selectIndex); // swap 2 int
                    }

                    for(int i = selectIndex; i <= lastIndex; i++)
                    {
                        var c = ((iTreeViewItem)scrollViewContainer.children[i]).container;
                        if (!_selectedItems.Contains(c))
                        {
                            _selectedItems.Add(c);
                        }
                    }
                }
            }
            else
            {
                _selectedItems.Clear();
                _selectedItems.Add(container);
            }

            UpdateSelection();
            OnSelectItemChanged?.Invoke(this);
        }


        internal void CheckBoxChanged(iTreeViewNode container)
        {
            UpdateCheckState();
            OnCheckboxStateChanged?.Invoke(this, container);
        }


        public void UpdateSelection()
        {
            scrollViewContainer.children.ForEach( o =>
            {
                iTreeViewItem item = o as iTreeViewItem;
                item.UpdateSelection(_selectedItems.Contains(item.container));
            });
        }


        public void UpdateCheckState()
        {
            scrollViewContainer.children.ForEach( o => 
            {
                ((iTreeViewItem)o).UpdateCheckBox();
            });
        }


        public iTreeViewNode ObjectPoolGetNode()
        {
            if (poolingObject.Count == 0)
            {
                return new iTreeViewNode();
            }   
            return poolingObject.Dequeue();
        }


        public void ObjectPoolRemoveNode(iTreeViewNode node)
        {
            poolingObject.Enqueue(node);
        }


        private iTreeViewItem ObjectPoolGetItem()
        {
            if (poolingItems.Count == 0)
            {
                return new iTreeViewItem();
            }

            return poolingItems.Dequeue();
        }


        private void ObjectPoolRemoveItem(iTreeViewItem item)
        {
            poolingItems.Enqueue(item);
        }


        public override void UpdateRelative()
        {
            base.UpdateRelative();
            background.UpdateRelative();
            footer.UpdateRelative();
            scrollViewContainer.UpdateRelative();
        }


        private void CleanSelected()
        {
            for(int i = _selectedItems.Count - 1; i >= 0; i--)
            {
                if (_selectedItems[i] == null) _selectedItems.RemoveAt(i);
            }
        }
    }
}