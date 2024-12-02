using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace InnovaFramework.iGUI
{
    public enum iScrollViewDirection
    {
        NONE,
        HORIZONTAL,
        VERTICAL
    };

    public enum iScrollViewAutoSize
    {
        NONE,
        HORIZONTAL,
        VERTICAL
    }

    public class iScrollView : iObject
    {
        public iPadding             padding                  = new iPadding();
        public bool                 isScrollbarRightVisible  = false;
        public bool                 isScrollbarBottomVisible = false;
        public iScrollViewDirection direction                = iScrollViewDirection.VERTICAL;
        public iScrollViewAutoSize  autoSizeMode             = iScrollViewAutoSize.NONE;
        public List<iObject>        children                 = new List<iObject>();

        private Rect    containSize    = new Rect();
        private Vector2 scrollPosition = new Vector2();

        public iScrollView()
        {
            size = new Vector2(100, 100);
        }


        public override void Render()
        {
            if(children.Count == 0) return;
            if(!active) return;

            UpdatePosition();

            GUI.enabled    = enabled;
            scrollPosition = GUI.BeginScrollView(rect, scrollPosition, containSize);
            for(int i = 0; i< children.Count; ++i)
            {
                var obj = children[i];
                if (!obj.active) continue;
                obj.Render();
            }
            GUI.EndScrollView();
            GUI.enabled = true;
        }


        public void UpdatePosition()
        {
            float count           = children.Count;
            float currentPosition = padding.top;

            if(direction == iScrollViewDirection.HORIZONTAL)
            {
                currentPosition = padding.left;
            }

            float maxVertical   = 0;
            float maxHorizontal = 0;

            // Sorting Object
            bool isFirst = false;
            for(int i = 0; i < count; ++i)
            {
                iObject obj = children[i];
                if(!obj.active) continue;

                currentPosition += ( !isFirst ? 0 : padding.space);
                isFirst = true;

                switch(autoSizeMode)
                {
                    case iScrollViewAutoSize.HORIZONTAL:
                    {
                        obj.size.x    = size.x - padding.left - padding.right;
                        if(direction == iScrollViewDirection.VERTICAL && isScrollbarRightVisible)
                        {
                            obj.size.x -= 16;
                        }

                        break;
                    }
                    case iScrollViewAutoSize.VERTICAL:
                    {
                        obj.size.y    = size.y - padding.top - padding.bottom;
                        if(direction == iScrollViewDirection.HORIZONTAL && isScrollbarBottomVisible)
                        {
                            obj.size.y -= 16;
                        }
                        break;
                    }
                }

                switch(direction)
                {
                    case iScrollViewDirection.VERTICAL:
                    {
                        obj.position.x   = padding.left;
                        obj.position.y   = currentPosition;
                        currentPosition += obj.size.y;

                        break;
                    }
                    case iScrollViewDirection.HORIZONTAL:
                    {
                        obj.position.x   = currentPosition;
                        obj.position.y   = padding.top;
                        currentPosition += obj.size.x;
                        break;
                    }
                }

                if((obj.size.x) > maxHorizontal)
                {
                    maxHorizontal = obj.size.x;
                }

                if(obj.size.y > maxVertical)
                {
                    maxVertical = obj.size.y;
                }
            }

            containSize = new Rect();
            containSize.x = 0;
            containSize.y = 0;
            containSize.width  = direction == iScrollViewDirection.VERTICAL ? maxHorizontal : currentPosition;
            containSize.height = direction == iScrollViewDirection.HORIZONTAL ? maxVertical : currentPosition;

            if (direction == iScrollViewDirection.NONE && children.Count > 0)
            {
                containSize.width  = children.OrderByDescending(o => o.right).ToList()[0].right;
                containSize.height = children.OrderByDescending(o => o.bottom).ToList()[0].bottom;
            }

            isScrollbarRightVisible  = (containSize.height > rect.height);
            isScrollbarBottomVisible = (containSize.width > rect.width);
        }


        public void ShowOnly(params iObject[] filterItem)
        {
            HashSet<iObject> hsFilter = null;
            if (filterItem != null)
            {
                hsFilter = new HashSet<iObject>(filterItem);
            }

            for(int i = 0; i < children.Count; i++)
            {
                if (filterItem == null)
                {
                    children[i].active = true;
                }
                else
                {
                    children[i].active = hsFilter.Contains(children[i]);
                }
            }
            GUI.changed = true;
        }


        public iObject[] GetVisibleChild()
        {
            return children.FindAll( o => o.active).ToArray();
        }


        public void AddChild(iObject mObject)
        {
            if(children.Contains(mObject)) return;
            mObject.parent = this;
            mObject.window = window;
            children.Add(mObject);
            UpdatePosition();
        }


        public void RemoveChild(iObject mObject)
        {
            children.Remove(mObject);
            UpdatePosition();
        }


        public void RemoveChildAll()
        {
            children.Clear();
            UpdatePosition();
        }


        public override void UpdateRelative()
        {
            base.UpdateRelative();
            if (direction == iScrollViewDirection.NONE)
            {
                foreach(var o in children)
                {
                    o.UpdateRelative();
                }
            }
        }


        public override void Dispose()
        {
            base.Dispose();
            foreach(var kv in children)
            {
                kv.Dispose();
            }
        }
    }
}