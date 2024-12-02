using System.Collections;
using System.Collections.Generic;
using InnovaFramework.iGUI;
using UnityEditor;
using UnityEngine;
using System.Linq;
using SuperInnovaLib;
using System.IO;
using Newtonsoft.Json;

public partial class iAPIConfigGenerator : iWindow
{
    public static iAPIConfigGenerator window;

    private static Rect windowRect = new Rect(0f, 0f, 500f, 300f);

    public iTab        tabTopic;
    public iInputField ipfHost;
    public iInputField ipfPort;
    public iInputField ipfKey;
    public iInputField ipfVersion;
    public iCheckBox   cbIsDefault; 
    public iButton     btnGenerate;
    public iButton     btnOpen;
    public iButton     btnAddAPI;
    public iScrollView scvAPIKeys;

    [MenuItem("Innova Engine/iAPI/Config Generator")]
    public static void OpenWindow()
    {
        window = GetWindow<iAPIConfigGenerator>();
        window.titleContent = new GUIContent("iAPI: Config Generator");
        window.maxSize = windowRect.size;
        window.minSize = window.maxSize;
    }

    private void OnEnable()
    {
        if(EditorApplication.isCompiling) return;
        InitializeUI();
    }

 
    private void OnGUI()
    {
        if(window != null)
        {
            window.minSize = window.minSize;
        }

        base.Render();
    }


    private void InitializeUI()
    {
        tabTopic         = new iTab();
        tabTopic.size    = new Vector2(windowRect.width.space(), 26);
        tabTopic.SetHeaders("API Endpoint", "Amazon S3");
        tabTopic.RelativePosition(iRelativePosition.TOP_IN, windowRect);
        tabTopic.RelativePosition(iRelativePosition.CENTER_X_OF, windowRect);
        AddChild(tabTopic);

        InitializeUIAPI();
    }


    private void InitializeUIAPI()
    {
        ipfHost     = new iInputField(iInputType.STRING);
        ipfPort     = new iInputField(iInputType.STRING);
        ipfKey      = new iInputField(iInputType.STRING);
        ipfVersion  = new iInputField(iInputType.INT);
        cbIsDefault = new iCheckBox();
        btnGenerate = new iButton();
        btnOpen     = new iButton();
        btnAddAPI   = new iButton();
        scvAPIKeys  = new iScrollView();
        iBox box    = new iBox();


        btnOpen.text = "Open";
        btnOpen.RelativePosition(iRelativePosition.BOTTOM_IN, windowRect, 2);
        btnOpen.RelativePosition(iRelativePosition.RIGHT_IN, windowRect);

        btnGenerate.text   = "Generate Config";
        btnGenerate.size.x = windowRect.width.space() - btnOpen.widthEx;
        btnGenerate.RelativePosition(iRelativePosition.BOTTOM_IN,   windowRect, 2);
        btnGenerate.RelativePosition(iRelativePosition.LEFT_IN, windowRect);

        btnAddAPI.text = "Create";
        btnAddAPI.RelativePosition(iRelativePosition.TOP_OF, btnGenerate);
        btnAddAPI.RelativePosition(iRelativePosition.RIGHT,  new Rect(250, 0, 0, 0));

        // Left Side
        ipfKey.text = "Key";
        ipfKey.labelSpace = 26;
        ipfKey.size.x = 242 - btnAddAPI.widthEx;
        ipfKey.RelativePosition(iRelativePosition.LEFT_OF, btnAddAPI);
        ipfKey.RelativePosition(iRelativePosition.TOP_OF,  btnGenerate);

        scvAPIKeys.size.x       = 242;
        scvAPIKeys.size.y       = iGUIUtility.HeightBetween2Objects(tabTopic, btnAddAPI);
        scvAPIKeys.direction    = iScrollViewDirection.VERTICAL;
        scvAPIKeys.autoSizeMode = iScrollViewAutoSize.HORIZONTAL;
        scvAPIKeys.padding      = new iPadding(4,4,4,4,4);
        scvAPIKeys.RelativePosition(iRelativePosition.LEFT_IN,   windowRect);
        scvAPIKeys.RelativePosition(iRelativePosition.BOTTOM_OF, tabTopic);

        box.size = scvAPIKeys.size;
        box.position = scvAPIKeys.position;

        // Right Side
        ipfHost.enabled = false;
        ipfHost.text    = "Host";
        ipfHost.size.x  = windowRect.width / 2f - iGUIUtility.spaceX2;
        ipfHost.RelativePosition(iRelativePosition.BOTTOM_OF, tabTopic);
        ipfHost.RelativePosition(iRelativePosition.RIGHT_IN, windowRect);

        ipfPort.enabled = false;
        ipfPort.text    = "Port";
        ipfPort.size.x  = ipfHost.width;
        ipfPort.RelativePosition(iRelativePosition.BOTTOM_OF, ipfHost);
        ipfPort.RelativePosition(iRelativePosition.RIGHT_IN, windowRect);

        ipfVersion.enabled  = false;
        ipfVersion.text     = "Version";
        ipfVersion.size.x   = ipfHost.width;
        ipfVersion.intValue = 1;
        ipfVersion.RelativePosition(iRelativePosition.BOTTOM_OF, ipfPort);
        ipfVersion.RelativePosition(iRelativePosition.RIGHT_IN, windowRect);

        cbIsDefault.enabled = false;
        cbIsDefault.text    = "Set to Default";
        cbIsDefault.size.x  = ipfHost.width;
        cbIsDefault.RelativePosition(iRelativePosition.BOTTOM_OF, ipfVersion);
        cbIsDefault.RelativePosition(iRelativePosition.RIGHT_IN, windowRect);

        // Event
        ipfHost.OnChanged     = OnInfoChange;
        ipfPort.OnChanged     = OnInfoChange;
        ipfVersion.OnChanged  = OnInfoChange;
        btnAddAPI.OnClicked   = OnClickAddAPI;
        btnGenerate.OnClicked = OnClickGenerateConfig;
        btnOpen.OnClicked     = OnClickOpen;
        cbIsDefault.OnChanged = OnDefaultChange;

        tabTopic.AddChild(box,         0);
        tabTopic.AddChild(scvAPIKeys,  0);
        tabTopic.AddChild(btnGenerate, 0);
        tabTopic.AddChild(btnOpen,     0);
        tabTopic.AddChild(btnAddAPI,   0);
        tabTopic.AddChild(ipfKey,      0);
        tabTopic.AddChild(ipfHost,     0);
        tabTopic.AddChild(ipfPort,     0);
        tabTopic.AddChild(ipfVersion,  0);
        tabTopic.AddChild(cbIsDefault, 0);
    }
}