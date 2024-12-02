using System.Collections;
using System.Collections.Generic;
using InnovaFramework.iGUI;
using UnityEditor;
using UnityEngine;

public partial class SimpleWindow : iWindow
{
    private iMenuBar     menuBar;
    private iMenuBar     menuBarInsideTab;
    private iArrayContainer<iBox> menuBarSubMenus;
    private iButton menuBarRemoveAll;

    private void RenderTopMenuBar()
    {
        menuBar = new iMenuBar(this);
        // menuBar No need to AddChild
        menuBar.Initialize(window.rect.width, iWindow.DEFAULT_TOP_CHANNEL);
        menuBar.AddMenu("Example");
        menuBar.AddMenu("About");

        // Context Menu - Example
        menuBar.AddContextMenu("Example", "Open Website/Google",      (p) => Application.OpenURL("https://www.google.com"));
        menuBar.AddContextMenu("Example", "Open Website/Bitcoin",     (p) => Application.OpenURL("https://coinmarketcap.com/currencies/bitcoin"));
        menuBar.AddContextMenu("Example", "Open Website/Binance USD", (p) => Application.OpenURL("https://coinmarketcap.com/currencies/binance-usd"));
        menuBar.AddContextMenu("Example", "Show Explorer",            (p) => System.Diagnostics.Process.Start("explorer.exe", "/select," + "Assets"));
        menuBar.AddSeperator();
        menuBar.AddContextMenu("Example", "Complex Menu", null, false);

        // Context Menu - About
        menuBar.AddContextMenu("About", "Version", (path) => 
        {
            EditorUtility.DisplayDialog("Innova GUI", $"Current Version: {iGUIUtility.Version}", "OK");
        });
    }


    private void RenderPageMenuBar()
    {
        menuBarInsideTab     = new iMenuBar(this);
        menuBarRemoveAll     = new iButton();
        menuBarSubMenus      = new iArrayContainer<iBox>();

        // Menubar with referene object
        menuBarInsideTab.Initialize(panel.width, 8, panel, tabs); // panel is reference position and tabs is container with 8th channel
        menuBarInsideTab.AddMenu("Option");
        menuBarInsideTab.AddMenu("Set Color");
        menuBarInsideTab.AddContextMenu("Option", "Clear All Item",  (p) => menuBarRemoveAll.OnClicked?.Invoke(null));
        menuBarInsideTab.AddContextMenu("Set Color", "Default",      (p) => menuBarInsideTab.ResetBackgroundColor());
        menuBarInsideTab.AddContextMenu("Set Color", "Red",          (p) => menuBarInsideTab.backgroundColor = new Color(0.9f, 0.5f, 0.5f));
        menuBarInsideTab.AddContextMenu("Set Color", "Green",        (p) => menuBarInsideTab.backgroundColor = new Color(0.5f, 0.9f, 0.5f));
        menuBarInsideTab.AddContextMenu("Set Color", "Blue",         (p) => menuBarInsideTab.backgroundColor = new Color(0.5f, 0.5f, 0.9f));
        menuBarInsideTab.AddContextMenu("Set Color", "Text - Black", (p) => menuBarInsideTab.contentColor    = Color.black);
        menuBarInsideTab.AddContextMenu("Set Color", "Text - White", (p) => menuBarInsideTab.contentColor    = Color.white);

        menuBarRemoveAll.text = "Clear All: Main Menu Bar";
        menuBarRemoveAll.size.x = 250;
        menuBarRemoveAll.RelativePosition(iRelativePosition.BOTTOM_IN, panel);
        menuBarRemoveAll.RelativePosition(iRelativePosition.LEFT_IN  , panel);
        menuBarRemoveAll.OnClicked = (sender) =>
        {
            menuBarSubMenus.RemoveAll();
            menuBar.GetContextMenu("Example", "Complex Menu").active = true;
            menuBar.RemoveContextMenu("Example", "Complex Menu/"); // '/' end of path for remove only children;
        };

        menuBarSubMenus.text        = "Complex Menu";
        menuBarSubMenus.showIndex   = false;
        menuBarSubMenus.fixedHeight = iGUIUtility.HeightBetween2Objects(menuBarInsideTab, menuBarRemoveAll);
        menuBarSubMenus.RelativePosition(iRelativePosition.BOTTOM_OF, menuBarInsideTab);
        menuBarSubMenus.RelativePosition(iRelativePosition.LEFT_IN, panel);
        menuBarSubMenus.Builder( () => 
        {
            iBox item   = new iBox();
            item.size.y = 24;
            item.text   = "Random ID: " + Random.Range(1000, 9999);
            return item;
        });

        menuBarSubMenus.OnAdded = (sender, itemContainer) => 
        {
            menuBar.GetContextMenu("Example", "Complex Menu").active = false; // Hide 

            iMenuBarItem addedMenu = menuBar.AddContextMenu("Example", $"Complex Menu/{itemContainer.main.text}", (p) => Debug.Log("Clicked: " + p));
            itemContainer.main.attechment = addedMenu; // attach menubar to item list
        };

        menuBarSubMenus.OnRemoving = (itemContainer) => 
        {
            iMenuBarItem mbi = (iMenuBarItem)itemContainer.main.attechment;

            menuBar.RemoveContextMenu("Example", mbi.path);

            if (menuBar.GetChildren("Example", "Complex Menu").Length == 0)
            {
                menuBar.GetContextMenu("Example", "Complex Menu").active = true;
            }

            return true; // False will cancel delete
        };

        tabs.AddChild(menuBarSubMenus , 8);
        tabs.AddChild(menuBarRemoveAll, 8);
    }
}
