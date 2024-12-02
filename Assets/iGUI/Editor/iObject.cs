using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace InnovaFramework.iGUI
{
    public class iPadding
    {
        public float top;
        public float left;
        public float right;
        public float bottom;
        public float space;

        public iPadding(float top, float left, float right, float bottom, float space)
        {
            this.top = top;
            this.left = left;
            this.right = right;
            this.bottom = bottom;
            this.space = space;
        }
        public iPadding() { }
    }


    public enum iPreviewType
    {
        PREVIEW,
        INTERACTIVE_PREVIEW,
        INSPECTOR
    }


    public enum iRelativePosition
    {
        RIGHT_OF,
        LEFT_OF,
        TOP_OF,
        BOTTOM_OF,

        CENTER_Y_OF,
        CENTER_X_OF,

        RIGHT_IN,
        LEFT_IN,
        TOP_IN,
        BOTTOM_IN,

        LEFT,
        RIGHT,
        TOP,
        BOTTOM
    }


    public enum iSize
    {
        X,
        Y
    }


    public enum iRelativeSize
    {
        EQUAL,
        PERCENTAG,
        SPLIT,
        FILL,
        BETWEEN,
        MANUAL
    }


    public enum iInputType
    {
        INT,
        FLOAT,
        STRING,
        OBJECT,
        PASSWORD,
        DELAYED_INT,
        DELAYED_FLOAT,
        DELAYED_STRING
    }

    public enum iSliderType
    {
        INT,
        FLOAT
    }


    public delegate void iObjectEvent(iObject obj, Event evt);


    public class iObject
    {
        protected class RelativePositionData
        {
            public float             space;
            public iObject           obj;
            public iRelativePosition relative;
        }


        protected class RelativeSizeData
        {
            public Func<Vector2> manual;
        }

        // Properties
        public bool    active   = true;
        public bool    enabled  = true;
        public bool    isWindow = false; // Do not set to true
        public string  name            = "";
        public string  tag             = "";
        public string  description     = "";

        public string  text            = "";
        public string  tooltips        = "";
        public object  attechment      = null; // Use for Attech some object to this object
        public int     fontSize        = 12;
        public Texture texture         = null;

        protected Color _color = new Color(0, 0, 0, 0);
        public    Color  color 
        {
            get { return _color; }
            set { _color = value; OnColorChanged(); }
        }

        protected Color _contentColor = new Color(0, 0, 0, 0);
        public    Color  contentColor
        {
            get { return _contentColor; }
            set { _contentColor = value; OnContentColorChanged(); }
        }

        protected Color _backgroundColor = new Color(0, 0, 0, 0);
        public    Color  backgroundColor
        {
            get { return _backgroundColor; }
            set { _backgroundColor = value; OnBackgroundColorChanged(); }
        }


        // Transform
        public Vector2 position;
        public Vector2 size;

        // Setting 
        public iWindow window;
        public iObject parent;
        public GUIStyle style;
        public float labelSpace;
        public bool useEventCallback = false;

        // Get Only
        public float   bottom   { get { return position.y + size.y; } }
        public float   right    { get { return position.x + size.x; } }
        public float   width    { get { return size.x; } }
        public float   height   { get { return size.y; } }
        public float   widthEx  { get { return size.x + iGUIUtility.space; } }
        public float   heightEx { get { return size.y + iGUIUtility.space; } }
        public Rect    rect     { get { return new Rect(position.x, position.y, size.x, size.y );}}
        public Vector2 center   { get { return new Vector2( position.x + (size.x / 2f), position.y + (size.y / 2f) );}}

        // Callback
        private bool isMouseDown = false;
        private bool isMouseOver = false;
        public event iObjectEvent OnMouseDown;
        public event iObjectEvent OnMouseUp;
        public event iObjectEvent OnMouseClick;
        public event iObjectEvent OnMouseOver;
        public event iObjectEvent OnMouseExit;
        public event iObjectEvent OnMouseDrag;
        public event iObjectEvent OnMouseScrollWheel;

        protected RelativeSizeData           lastRelativeSize     = new RelativeSizeData();
        protected List<RelativePositionData> lastRelativePosition = new List<RelativePositionData>();

        private bool  isInit = false;
        private Color tempColor;
        private Color tempBGColor;
        private Color tempContentColor;
        private float tempLabelSpace;

        #region Method
        protected virtual void OnColorChanged() {}
        protected virtual void OnBackgroundColorChanged() {}
        protected virtual void OnContentColorChanged() {}
        public virtual void Dispose() {}
        public virtual void Start() {}
        public virtual void Render() 
        { 
            if (!isInit)
            {
                Start();
                isInit = true;
            }

            if(useEventCallback)
            {
                ProcessEvent(Event.current);
            }
        }


        public Vector2 SplitSize(int splitTo)
        {
            var vec = new Vector2();
            vec.x   = (size.x - (iGUIUtility.space * (splitTo + 1))) / splitTo;
            vec.y   = (size.y - (iGUIUtility.space * (splitTo + 1))) / splitTo;
            return vec;
        }


        protected virtual void OnRelativePositionChanged() {}


        protected virtual void BeginProcessProperty()
        {
            tempColor        = GUI.color;
            tempBGColor      = GUI.backgroundColor;
            tempContentColor = GUI.contentColor;
            tempLabelSpace   = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = string.IsNullOrEmpty(text) ? 0 : labelSpace;

            if (color.a > 0)           GUI.color           = color;
            if (contentColor.a > 0)    GUI.contentColor    = contentColor;
            if (backgroundColor.a > 0) GUI.backgroundColor = backgroundColor;
            if (style != null)         style.fontSize      = fontSize;

            GUI.enabled = enabled;
        }


        public void CopyProperty(iObject source)
        {
            enabled          = source.enabled;
            labelSpace       = source.labelSpace;
            _color           = source.color;
            _contentColor    = source.contentColor;
            _backgroundColor = source.backgroundColor;
        }


        protected virtual void EndProcessProperty()
        {
            GUI.color                   = tempColor;
            GUI.contentColor            = tempContentColor;
            GUI.backgroundColor         = tempBGColor;
            GUI.enabled                 = true;
            EditorGUIUtility.labelWidth = tempLabelSpace;
        }


        public void CopyTransform(iObject source)
        {
            this.position = source.position;
            this.size     = source.size;
        }


        public void ParseGUIContent(GUIContent content)
        {
            text = content.text;
            tooltips = content.tooltip;
            texture = content.image;
        }


        public void LoadBuiltInIcon(string name)
        {
            ParseGUIContent(new GUIContent(text, iGUIUtility.LoadBuiltInIcon(name), tooltips));
        }


        public void LoadClickableTexture(string textureName, float lighter = 0.2f, float darker = 0.3f)
        {
            LoadClickableTexture(iGUIUtility.LoadBuiltInIcon(textureName, true));
        }


        public void LoadClickableTexture(Texture2D texture, float lighter = 0.2f, float darker = 0.3f)
        {
            if (style == null) style = new GUIStyle();
            style.LoadClickableTexture(texture, lighter, darker);
        }


        public void SetClickableTexture(Texture2D normalTexture, Texture2D hoverTexture, Texture2D activeTexture)
        {
            if (style == null) style = new GUIStyle();
            style.SetClickableTexture(normalTexture, hoverTexture, activeTexture);
        }


        public void SetClickableTexture(GUIStyle source)
        {
            if (style == null) style = new GUIStyle();
            style.SetClickableTexture(source.normal.background, source.hover.background, source.active.background);
        }


        public void LoadWhiteTexture()
        {
            if (style == null) style = new GUIStyle();
            style.normal.background = EditorGUIUtility.whiteTexture;
        }


        public virtual void UpdateRelative()
        {
            ReSizeWithLastTwoRelativeSize();
            RePositionWithLastTwoRelativePosition();
        }


        public virtual void RePositionWithLastTwoRelativePosition()
        {
            for(int i = 0; i < lastRelativePosition.Count; i++)
            {
                RelativePosition(lastRelativePosition[i].relative, lastRelativePosition[i].obj, lastRelativePosition[i].space, false);
            }
        }


        public virtual void ReSizeWithLastTwoRelativeSize()
        {
            RelativeSize(lastRelativeSize.manual, false);
        }


        public void RelativeSize(Func<Vector2> action, bool enableCached = true)
        {
            if (enableCached)
            {
                lastRelativeSize.manual = action;
            }

            if (action != null)
            {
                Vector2 s = action();
                this.size.x = s.x < 0 ? this.size.x : s.x;
                this.size.y = s.y < 0 ? this.size.y : s.y;
            }

            GUI.changed = true;
        }


        public void RelativePosition(iRelativePosition relative, Rect rect, float space = 8, bool enableCached = true)
        {
            iObject obj  = new iObject();
            obj.size     = rect.size;
            obj.position = rect.position;
            RelativePosition(relative, obj, space, enableCached);
        }


        public void RelativePosition(iRelativePosition relative, iWindow mWindow, float space = 8, bool enableCached = true)
        {
            RelativePosition(relative, mWindow.targetObject, space, enableCached);
        }


        public void RelativePosition(iRelativePosition relative, iObject mObject, float space = 8, bool enableCached = true)
        {
            if(enableCached)
            {
                if(lastRelativePosition.Count >= 2) { lastRelativePosition.RemoveAt(0); }
                lastRelativePosition.Add(new RelativePositionData()
                {
                    obj      = mObject,
                    space    = space,
                    relative = relative
                });
            }

            switch (relative)
            {
                case iRelativePosition.TOP        : position = new Vector2(position.x                                                , mObject.position.y);                                     break;
                case iRelativePosition.TOP_IN     : position = new Vector2(position.x                                                , mObject.position.y + space);                             break;
                case iRelativePosition.TOP_OF     : position = new Vector2(position.x                                                , mObject.position.y - size.y - space);                    break;
                case iRelativePosition.BOTTOM_OF  : position = new Vector2(position.x                                                , mObject.position.y + mObject.size.y + space);            break;
                case iRelativePosition.BOTTOM     : position = new Vector2(position.x                                                , mObject.position.y + mObject.size.y - size.y);           break;
                case iRelativePosition.BOTTOM_IN  : position = new Vector2(position.x                                                , mObject.position.y + mObject.size.y - size.y - space);   break;
                case iRelativePosition.CENTER_Y_OF: position = new Vector2(position.x                                                , mObject.position.y + mObject.size.y / 2f - size.y / 2f); break;
                case iRelativePosition.LEFT       : position = new Vector2(mObject.position.x                                        , position.y);                                             break;
                case iRelativePosition.LEFT_IN    : position = new Vector2(mObject.position.x + space                                , position.y);                                             break;
                case iRelativePosition.LEFT_OF    : position = new Vector2(mObject.position.x - size.x - space                       , position.y);                                             break;
                case iRelativePosition.RIGHT_OF   : position = new Vector2(mObject.position.x + mObject.size.x + space               , position.y);                                             break;
                case iRelativePosition.RIGHT      : position = new Vector2(mObject.position.x + mObject.size.x - size.x              , position.y);                                             break;
                case iRelativePosition.RIGHT_IN   : position = new Vector2(mObject.position.x + mObject.size.x - size.x - space      , position.y);                                             break;
                case iRelativePosition.CENTER_X_OF: position = new Vector2(mObject.position.x + mObject.size.x * 0.5f - size.x * 0.5f, position.y);                                             break;
            }

            OnRelativePositionChanged();
            GUI.changed = true;
        }

        // Process Event
        public void ProcessEvent(Event evt)
        {
            if (rect.Contains(evt.mousePosition))
            {
                if (!isMouseOver)
                {
                    isMouseOver = true;
                    OnMouseOver?.Invoke(this, evt);
                }
            }
            else
            {
                if (isMouseOver)
                {
                    isMouseOver = false;
                    OnMouseExit?.Invoke(this, evt);
                }
            }

            switch(evt.type)
            {
                case EventType.MouseDown:
                {
                    if (rect.Contains(evt.mousePosition))
                    {
                        isMouseDown = true;
                        OnMouseDown?.Invoke(this, evt);
                    }
                    break;
                }
                case EventType.MouseUp:
                {
                    if (rect.Contains(evt.mousePosition))
                    {
                        OnMouseUp?.Invoke(this, evt);

                        if (isMouseDown)
                        {
                            OnMouseClick?.Invoke(this, evt);
                        }
                    }
                    isMouseDown = false;
                    break;
                }
                case EventType.MouseDrag:
                {
                    if (rect.Contains(evt.mousePosition))
                    {
                        OnMouseDrag?.Invoke(this, evt);
                    }
                    break;
                }
                case EventType.ScrollWheel:
                {
                    if (rect.Contains(evt.mousePosition))
                    {
                        OnMouseScrollWheel?.Invoke(this, evt);
                    }
                    break;
                }
            }
        }

        #endregion
    }
}