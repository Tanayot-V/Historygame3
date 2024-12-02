using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using InnovaFramework.iGUI;

public partial class BuiltInUnityIcon: iWindow
{
    public static BuiltInUnityIcon window;

    private iInputField inputSearch;
    private iBox        boxBackground;
    private iScrollView scvContainer;
    private iLabel      labelFound;
    private iButton     btnRefresh;

    private iInputField matInputSearch;
    private iBox        matBoxBackground;
    private iScrollView matScvContainer;
    private iLabel      matLabelFound;
    private iButton     matBtnRefresh;

    private iTab tabTypes;


    [MenuItem("iGUI/BuiltIn Unity Icon", false, 1)]
    public static void OpenWindow()
    {
        window = GetWindow<BuiltInUnityIcon>();
        window.rect = new Rect(0, 0, 600, 1000);
        window.titleContent = new GUIContent("BuiltIn Unity Icon");
        window.maxSize = window.rect.size;
        window.minSize = window.maxSize;
    }

    private void OnEnable()
    {
        if(EditorApplication.isCompiling) return;
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

    // Initialize function
    protected override void OnAfterInitializedUI()
    {
        OnBegin();
    }

    protected override void OnInitializeUI()
    {
        tabTypes = new iTab();
        tabTypes.size.x = window.rect.width.space();
        tabTypes.SetHeaders("Built-In", "Material-UI Icon");
        tabTypes.RelativePosition(iRelativePosition.TOP_IN, window);
        tabTypes.RelativePosition(iRelativePosition.LEFT_IN, window);
        tabTypes.OnChange = OnTabChange;

        AddChild(tabTypes);

        DrawPageBuiltIn();
        DrawPageMaterialUI();
    }


    private void DrawPageBuiltIn()
    {
        inputSearch = new iInputField(iInputType.STRING);
        boxBackground = new iBox();
        scvContainer = new iScrollView();
        labelFound = new iLabel();
        btnRefresh = new iButton();

        inputSearch.text = "Search";
        inputSearch.size.x = rect.width.space();
        inputSearch.RelativePosition(iRelativePosition.LEFT_IN, rect);
        inputSearch.RelativePosition(iRelativePosition.BOTTOM_OF , tabTypes);

        labelFound.size.x = 150;
        labelFound.size.y = 16;
        labelFound.text = "Icons: 0";
        labelFound.RelativePosition(iRelativePosition.BOTTOM_OF, inputSearch);
        labelFound.RelativePosition(iRelativePosition.LEFT_IN, rect);

        btnRefresh.text = "Refresh";
        btnRefresh.RelativePosition(iRelativePosition.RIGHT_IN, rect);
        btnRefresh.RelativePosition(iRelativePosition.BOTTOM_OF, inputSearch);

        boxBackground.size.y = iGUIUtility.HeightBetween2Objects(labelFound, rect.height);
        boxBackground.size.x = rect.width.space();
        boxBackground.RelativePosition(iRelativePosition.BOTTOM_OF, labelFound);
        boxBackground.RelativePosition(iRelativePosition.LEFT_IN, rect);

        scvContainer.direction = iScrollViewDirection.VERTICAL;
        scvContainer.autoSizeMode = iScrollViewAutoSize.HORIZONTAL;
        scvContainer.padding = new iPadding(0, 0, 0, 0, 4);
        scvContainer.size = boxBackground.size.space();
        scvContainer.RelativePosition(iRelativePosition.CENTER_X_OF, boxBackground);
        scvContainer.RelativePosition(iRelativePosition.CENTER_Y_OF, boxBackground);

        inputSearch.OnChanged = OnSearchChange;
        btnRefresh.OnClicked = OnRefreshIcon;

        tabTypes.AddChild(inputSearch  , 0);
        tabTypes.AddChild(btnRefresh   , 0);
        tabTypes.AddChild(labelFound   , 0);
        tabTypes.AddChild(boxBackground, 0);
        tabTypes.AddChild(scvContainer , 0);
    }


    private void DrawPageMaterialUI()
    {
        matInputSearch = new iInputField(iInputType.STRING);
        matBoxBackground = new iBox();
        matScvContainer = new iScrollView();
        matLabelFound = new iLabel();
        matBtnRefresh = new iButton();

        matInputSearch.text = "Search";
        matInputSearch.size.x = rect.width.space();
        matInputSearch.RelativePosition(iRelativePosition.LEFT_IN, rect);
        matInputSearch.RelativePosition(iRelativePosition.BOTTOM_OF , tabTypes);

        matLabelFound.size.x = 150;
        matLabelFound.size.y = 16;
        matLabelFound.text = "Icons: 0";
        matLabelFound.RelativePosition(iRelativePosition.BOTTOM_OF, matInputSearch);
        matLabelFound.RelativePosition(iRelativePosition.LEFT_IN, rect);

        matBtnRefresh.text = "Refresh";
        matBtnRefresh.RelativePosition(iRelativePosition.RIGHT_IN, rect);
        matBtnRefresh.RelativePosition(iRelativePosition.BOTTOM_OF, matInputSearch);

        matBoxBackground.size.y = iGUIUtility.HeightBetween2Objects(matLabelFound, rect.height);
        matBoxBackground.size.x = rect.width.space();
        matBoxBackground.RelativePosition(iRelativePosition.BOTTOM_OF, matLabelFound);
        matBoxBackground.RelativePosition(iRelativePosition.LEFT_IN, rect);

        matScvContainer.direction = iScrollViewDirection.VERTICAL;
        matScvContainer.autoSizeMode = iScrollViewAutoSize.HORIZONTAL;
        matScvContainer.padding = new iPadding(0, 0, 0, 0, 4);
        matScvContainer.size = boxBackground.size.space();
        matScvContainer.RelativePosition(iRelativePosition.CENTER_X_OF, matBoxBackground);
        matScvContainer.RelativePosition(iRelativePosition.CENTER_Y_OF, matBoxBackground);

        matInputSearch.OnChanged = OnSearchChangeMaterialUI;
        matBtnRefresh.OnClicked = OnRefreshIconMaterialUI;

        tabTypes.AddChild(matInputSearch  , 1);
        tabTypes.AddChild(matBtnRefresh   , 1);
        tabTypes.AddChild(matLabelFound   , 1);
        tabTypes.AddChild(matBoxBackground, 1);
        tabTypes.AddChild(matScvContainer , 1);
    }
}
