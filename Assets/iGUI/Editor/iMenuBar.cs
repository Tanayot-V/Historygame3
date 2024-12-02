using System;
using System.Collections;
using System.Collections.Generic;
using InnovaFramework.iGUI;
using UnityEditor;
using UnityEngine;
using System.Linq;
using System.IO;

public class iMenuBarItem
{
    public string tag  = "";
    public string path = "";
    public bool enabled;
    public bool active;
    public Action<string> callback;
}


public class iMenuBar : iObject
{
    private iBox        backgroundMenu;

    private Dictionary<string, List<iMenuBarItem>> menuContextItems = new Dictionary<string, List<iMenuBarItem>>();
    private Dictionary<string, iButton>            menuItems        = new Dictionary<string, iButton>();

    private int               channel             = -1;
    private bool              isInitialized       = false;
    private string            currentMenuName     = "";
    private iObject           lastMenuCreated     = null;
    private IiObjectContainer container           = null;
    private GenericMenu       menu                = null;

    public iMenuBar(iWindow window)
    {
        this._contentColor    = Color.white;
        this._backgroundColor = new Color(0.18f, 0.18f, 0.18f);
        
        backgroundMenu = new iBox();
        backgroundMenu.LoadWhiteTexture();
        backgroundMenu.backgroundColor = this.backgroundColor;

        this.window = window;
        this.window.OnUIInitialized += OnWindowUIInitialized;
        this.size.y = EditorGUIUtility.singleLineHeight + iGUIUtility.space;
    }


    public void Initialize(float width, int channel = -1, iObject objReference = null, IiObjectContainer container = null)
    {
        if (this.window == null) return;

        isInitialized  = true;
        this.channel   = channel;
        this.container = container;
        this.size.x    = width;

        if (objReference != null)
        {
            this.RelativePosition(iRelativePosition.TOP, objReference);
            this.RelativePosition(iRelativePosition.LEFT, objReference);
        }
        else
        {
            this.RelativePosition(iRelativePosition.TOP, window);
            this.RelativePosition(iRelativePosition.LEFT, window);
        }

        backgroundMenu.CopyTransform(this);
    }


    private void OnWindowUIInitialized()
    {
        if (container != null)
        {
            container.AddChild(this, channel);
            return;
        }

        window.AddChild(this, channel);
    }


    public void AddMenu(string menuName)
    {
        if (menuItems.ContainsKey(menuName)) return;

        iButton btn    = new iButton();
        btn.miniButton = false;
        btn.text       = menuName;
        btn.tag        = menuName;
        btn.size.x     = btn.style.CalcSize(new GUIContent(menuName)).x + iGUIUtility.spaceX2;
        btn.size.y     = backgroundMenu.height - 2;
        btn.LoadClickableTexture(iGUIUtility.GetSolidTextureColor(backgroundColor));
        btn.RelativePosition(iRelativePosition.TOP, backgroundMenu);
        btn.OnClicked = (sender) => Open(btn.tag);

        if (lastMenuCreated == null)
        {
            btn.RelativePosition(iRelativePosition.LEFT, backgroundMenu);
        }
        else
        {
            btn.RelativePosition(iRelativePosition.RIGHT_OF, lastMenuCreated, 0);
        }

        menuItems[menuName] = btn;

        lastMenuCreated = btn;
    }


    public iMenuBarItem AddContextMenu(string ownerMenu, string contextPath, Action<string> callback, bool enabled = true, string tag = "")
    {
        if (!menuContextItems.ContainsKey(ownerMenu))
        {
            menuContextItems[ownerMenu] = new List<iMenuBarItem>();
        }

        if (menuContextItems[ownerMenu].Exists( o => 
        {
            if (o == null) return false;
            return o.path == contextPath;
        })) return null;

        iMenuBarItem mbi = new iMenuBarItem();
        mbi.path     = contextPath;
        mbi.tag      = tag == "" ? contextPath : tag;
        mbi.enabled  = enabled;
        mbi.active   = true;
        mbi.callback = callback;

        menuContextItems[ownerMenu].Add(mbi);
        currentMenuName = ownerMenu;

        return mbi;
    }


    public void AddSeperator()
    {
        if (currentMenuName == "") return;

        if (!menuContextItems.ContainsKey(currentMenuName))
        {
            menuContextItems[currentMenuName] = new List<iMenuBarItem>();
        }

        menuContextItems[currentMenuName].Add(null);
    }


    public iButton GetMenu(string menuName)
    {
        if (!menuItems.ContainsKey(menuName)) return null;
        return menuItems[menuName];
    }


    public iMenuBarItem GetContextMenu(string ownerMenu, string tagOrPath)
    {
        if (!menuItems.ContainsKey(ownerMenu))
        {
            return null;
        }

        if (!menuContextItems.ContainsKey(ownerMenu))
        {
            return null;
        }
        
        iMenuBarItem item = menuContextItems[ownerMenu].Find( o => 
        {
            if (o == null) return false;
            return o.path == tagOrPath || o.tag == tagOrPath;
        });
        return item;
    }


    public int RemoveContextMenu(string ownerMenu, string tagOrPath, bool recursive = true)
    {
        int returnCount = 0;
        if (!menuItems.ContainsKey(ownerMenu))        return returnCount;
        if (!menuContextItems.ContainsKey(ownerMenu)) return returnCount;

        string path = tagOrPath;

        iMenuBarItem item = GetContextMenu(ownerMenu, path);
        if (item != null)
        {
            menuContextItems[ownerMenu].Remove(item);
            path = item.path;
            returnCount++;
        }

        if (recursive)
        {
            List<iMenuBarItem> items = menuContextItems[ownerMenu].FindAll( o => 
            { 
                if (o == null) return false;
                return o.path.StartsWith(path);
            });

            for(int i = 0; i < items.Count; i++)
            {
                menuContextItems[ownerMenu].Remove(items[i]);
                returnCount++;
            }
        }

        if (menuContextItems[ownerMenu].Count == 0)
        {
            menuContextItems.Remove(ownerMenu);
        }

        return returnCount;
    }


    public iMenuBarItem[] GetChildren(string ownerMenu, string contextPath)
    {
        List<iMenuBarItem> menus = new List<iMenuBarItem>();

        if (!menuItems.ContainsKey(ownerMenu))        return menus.ToArray();
        if (!menuContextItems.ContainsKey(ownerMenu)) return menus.ToArray();

        menus = menuContextItems[ownerMenu].FindAll( o => 
        { 
            if (o == null) return false;
            return o.path.StartsWith($"{contextPath}/");
        });
        return menus.ToArray();
    }


    public void Open(string menuName)
    {
        if (!menuItems.ContainsKey(menuName))        return;
        if (!menuContextItems.ContainsKey(menuName)) return;

        menu = new GenericMenu();

        List<iMenuBarItem> menus = menuContextItems[menuName];

        string lastPath  = "";
        for(int i = 0; i < menus.Count; i++)
        {
            iMenuBarItem item = menus[i];
            if (item == null)
            {
                try { menu.AddSeparator(Path.GetDirectoryName(lastPath) + "/"); } catch {} continue;
            }

            if (!item.active) continue;

            if (menus[i].enabled)
            {
                menu.AddItem(new GUIContent(item.path), false, () => item.callback?.Invoke(item.tag));
            }
            else
            {
                menu.AddDisabledItem(new GUIContent(item.path), false);
            }

            lastPath = item.path;
        }

        iButton menuRef = menuItems[menuName];

        Rect rect = new Rect();
        rect.x = menuRef.position.x;
        rect.y = menuRef.bottom;
        menu.DropDown(rect);
    }


    public override void Render()
    {
        if (!isInitialized) return;

        backgroundMenu.CopyTransform(this);
        backgroundMenu.Render();
        foreach(var kv in menuItems)
        {
            kv.Value.style.normal.textColor = contentColor;
            kv.Value.size.y = backgroundMenu.height - 2;
            kv.Value.UpdateRelative();
            kv.Value.Render();
        }
    }


    protected override void OnBackgroundColorChanged()
    {
        backgroundMenu.backgroundColor = this.backgroundColor;

        GUIStyle sharedStyle = new GUIStyle();
        sharedStyle.LoadClickableTexture(iGUIUtility.GetSolidTextureColor(this.backgroundColor));

        foreach(var kv in menuItems)
        {
            kv.Value.SetClickableTexture(sharedStyle);
        }

        GUI.changed = true;
    }


    public override void UpdateRelative()
    {
        base.UpdateRelative();
        backgroundMenu.UpdateRelative();
        foreach(var kv in menuItems)
        {
            kv.Value.UpdateRelative();
        }
    }


    public void ResetBackgroundColor()
    {
        backgroundColor = new Color(0.18f, 0.18f, 0.18f);
    }
}
