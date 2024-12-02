using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InnovaFramework.iGUI;
using System.Linq;
using UnityEditor;

public partial class ProjectGenerator: iWindow
{
    public static ProjectGenerator window;

    private iButton     btnBrowse;
    private iButton     btnCreate;
    private iInputField inputName;
    private iInputField inputPath;

    [MenuItem("iGUI/Create Project", false, -1)]
    public static void OpenWindow()
    {
    //public static Rect rect = new Rect(0, 0, 400, 78);
        window = GetWindow<ProjectGenerator>();
        window.rect = new Rect(0, 0, 400, 80);
        window.titleContent = new GUIContent("Project Generator");
        window.maxSize = window.rect.size;
        window.minSize = window.maxSize;
    }


    private void OnEnable()
    {
        if(EditorApplication.isCompiling) return;
    }


    private void OnGUI()
    {
        if(window != null)
        {
            window.minSize = window.maxSize;
        }

        base.Render();
    }


    protected override void OnInitializeUI()
    {
        btnBrowse = new iButton();
        btnCreate = new iButton();
        inputName = new iInputField(iInputType.STRING);
        inputPath = new iInputField(iInputType.STRING);

        inputName.text   = "Name";
        inputName.size.x = rect.width.space();
        inputName.RelativePosition(iRelativePosition.TOP_IN , rect);
        inputName.RelativePosition(iRelativePosition.LEFT_IN, rect);

        btnBrowse.text   = "Browse";
        btnBrowse.RelativePosition(iRelativePosition.BOTTOM_OF, inputName);
        btnBrowse.RelativePosition(iRelativePosition.RIGHT_IN,  rect);

        inputPath.text = "Path";
        inputPath.stringValue = "Assets";
        inputPath.size.x = rect.width.space() - btnBrowse.widthEx;
        inputPath.RelativePosition(iRelativePosition.BOTTOM_OF, inputName);
        inputPath.RelativePosition(iRelativePosition.LEFT_IN,   rect);

        btnCreate.text = "Create Project";
        btnCreate.size.x = rect.width.space();
        btnCreate.RelativePosition(iRelativePosition.BOTTOM_OF, inputPath);
        btnCreate.RelativePosition(iRelativePosition.CENTER_X_OF, rect);

        btnBrowse.OnClicked = OnClickBrowse;
        btnCreate.OnClicked = OnClickCreate;

        AddChild(inputName);
        AddChild(inputPath);
        AddChild(btnBrowse);
        AddChild(btnCreate);
    }
}
