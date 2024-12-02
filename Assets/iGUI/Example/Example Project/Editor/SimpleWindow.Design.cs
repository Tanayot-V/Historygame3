using System.Collections;
using System.Collections.Generic;
using InnovaFramework.iGUI;
using UnityEditor;
using UnityEngine;

public partial class SimpleWindow : iWindow
{
    public static SimpleWindow window;

    private iTab         tabs;
    private iBox         panel;
    private iProgressBar progressBar;


    [MenuItem("iGUI/Example")]
    public static void OpenWindow()
    {
        window = GetWindow<SimpleWindow>();
        window.titleContent = new GUIContent("Simple Window");
        window.rect    = new Rect(0, 0, 1200, 420);
        window.maxSize = window.rect.size;
        window.minSize = window.maxSize;
    }


    private void OnGUI()
    {
        // Use this for fix size
        if(window != null)
        {
            window.minSize = window.maxSize;
        }

        base.Render();
    }


    protected override void OnInitializeUI()
    {
        RenderDefaultUI();

        RenderPageCommon();
        RenderPageInputField();
        RenderPageFoldout();
        RenderPageArray();
        RenderPageDragNDrop();
        RenderPageImage();
        RenderPageZoom();
        RenderPageLayout();
        RenderPageMenuBar();
        RenderPageDateTime();
        RenderPageTreeView();
    }


    private void RenderDefaultUI()
    {
        tabs        = new iTab();
        panel       = new iBox();
        progressBar = new iProgressBar();

        RenderTopMenuBar(); // Top Menubar

        tabs.SetHeaders
        (
            iGUIUtility.CreateGUIContent(" Common"      , "PreMatCube@2x"           ),
            iGUIUtility.CreateGUIContent(" Input Field" , "InputField Icon"         ),
            iGUIUtility.CreateGUIContent(" Foldout"     , "FolderOpened On Icon"    ),
            iGUIUtility.CreateGUIContent(" Array"       , "VerticalLayoutGroup Icon"),
            iGUIUtility.CreateGUIContent(" Drag & Drop" , "ViewToolMove"),
            iGUIUtility.CreateGUIContent(" Image"       , "RawImage Icon"),
            iGUIUtility.CreateGUIContent(" Zoom Scale"  , "ViewToolZoom On"),
            iGUIUtility.CreateGUIContent(" Layout"      , "FreeformLayoutGroup Icon"),
            iGUIUtility.CreateGUIContent(" Menu Bar"    , "_Popup@2x"),
            iGUIUtility.CreateGUIContent(" Date/Time"   , "mat-2-icon.calendar_month"),
            iGUIUtility.CreateGUIContent(" TreeView"    , "mat-2-icon.checklist")
        );
        tabs.index = 10;

        tabs.size.x  = window.rect.width.space();
        tabs.size.y  = 24;
        tabs.RelativePosition(iRelativePosition.LEFT_IN, window);
        tabs.RelativePosition(iRelativePosition.BOTTOM_OF, menuBar);
        tabs.OnChange = OnTabChange;

        panel.size.x = window.rect.width.space();
        panel.size.y = iGUIUtility.HeightBetween2Objects(tabs, window.rect.height);
        panel.RelativePosition(iRelativePosition.LEFT_IN, window);
        panel.RelativePosition(iRelativePosition.BOTTOM_OF, tabs);

        AddChild(tabs);
        AddChild(panel);
    }
}
