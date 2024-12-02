using System.Collections;
using System.Collections.Generic;
using InnovaFramework.iGUI;
using UnityEditor;
using UnityEngine;

public enum BuiltInUnityIconItemMode
{
    BUILT_IN,
    MATERIAL_UI
}

public class BuiltInUnityIconItem : iObject
{
    private iBox background;
    private iBox icon;
    private iLabel txtName;
    private iButton btnCopy;
    private iButton btnCopy_x1;
    private BuiltInUnityIconItemMode mode;

    public string iconName;
    public Texture2D iconTexture;

    public BuiltInUnityIconItem(string name, Texture2D iconTexture,  BuiltInUnityIconItemMode mode = BuiltInUnityIconItemMode.BUILT_IN)
    {
        icon       = new iBox();
        background = new iBox();
        txtName    = new iLabel();
        btnCopy    = new iButton();
        btnCopy_x1 = new iButton();

        this.name = name;
        this.mode = mode;
        this.iconName = name;
        this.iconTexture = iconTexture;

        size = new Vector2(32, 32);
    }


    public override void Start()
    {
        base.Start();

        background.size = size;
        background.position = position;
        background.backgroundColor = new Color(0.05f, 0.05f, 0.05f);

        icon.style = new GUIStyle();
        icon.style.normal.background = iconTexture;
        icon.size = icon.style.CalcSize(new GUIContent("", iconTexture));

        if (icon.size.y > size.y.space(1))
        {
            icon.size = icon.size.RatioY(size.y.space(1));
        }
        else if (icon.size.y < 5)
        {
            icon.size = icon.size.RatioY(10);
        }

        icon.RelativePosition(iRelativePosition.LEFT_IN    , background);
        icon.RelativePosition(iRelativePosition.CENTER_Y_OF, background);

        btnCopy.size.x = 50;
        btnCopy.size.y = size.y.space(1);
        btnCopy.text = mode == BuiltInUnityIconItemMode.BUILT_IN ? "Copy" : "48x48";
        btnCopy.RelativePosition(iRelativePosition.RIGHT_IN, background, 4);
        btnCopy.RelativePosition(iRelativePosition.CENTER_Y_OF,background);
        btnCopy.OnClicked = OnClickCopy;

        btnCopy_x1.size.x = 50;
        btnCopy_x1.size.y = size.y.space(1);
        btnCopy_x1.text = mode == BuiltInUnityIconItemMode.BUILT_IN ? "Copy" : "24x24";
        btnCopy_x1.RelativePosition(iRelativePosition.LEFT_OF, btnCopy, 4);
        btnCopy_x1.RelativePosition(iRelativePosition.CENTER_Y_OF,background);
        btnCopy_x1.OnClicked = OnClickCopy_x1;

        txtName.size.x = iGUIUtility.WidthBetween2Objects(icon, btnCopy);
        txtName.size.y = 18;
        txtName.fontSize = 16;
        txtName.SetText(iconName, false);
        txtName.RelativePosition(iRelativePosition.LEFT_IN    , background, 150);
        txtName.RelativePosition(iRelativePosition.CENTER_Y_OF, btnCopy);
    }


    public override void Render()
    {
        base.Render();
        background.size = size;
        background.position = position;
        btnCopy.UpdateRelative();
        btnCopy_x1.UpdateRelative();


        background.Render();
        icon.Render();
        btnCopy.Render();
        if (mode == BuiltInUnityIconItemMode.MATERIAL_UI)
        {
            btnCopy_x1.Render();
        }
        txtName.Render();
    }


    public override void UpdateRelative()
    {
        base.UpdateRelative();
        background.UpdateRelative();
        icon.UpdateRelative();
        btnCopy.UpdateRelative();
        btnCopy_x1.UpdateRelative();
        txtName.UpdateRelative();
    }


    private void OnClickCopy(iObject sender)
    {
        string copyMsg = iconName;
        if (mode == BuiltInUnityIconItemMode.MATERIAL_UI)
        {
            copyMsg = "mat-2-icon." + iconName;
        }

        EditorGUIUtility.systemCopyBuffer = copyMsg;
        Debug.Log("Copied: " + copyMsg);
    }

    private void OnClickCopy_x1(iObject sender)
    {
        string copyMsg = "mat-1-icon." + iconName;
        EditorGUIUtility.systemCopyBuffer = copyMsg;
        Debug.Log("Copied: " + copyMsg);
    }
}
