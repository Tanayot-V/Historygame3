using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using InnovaFramework.iGUI;
using UnityEditor;
using UnityEngine;

public partial class SimpleWindow : iWindow
{
    iTreeView treeView;
    iButton   btnTreeViewShow;

    public void RenderPageTreeView()
    {
        treeView        = new iTreeView();
        btnTreeViewShow = new iButton();

        Texture2D csIcon          = iGUIUtility.LoadBuiltInIcon("cs Script Icon");
        Texture2D jsIcon          = iGUIUtility.LoadBuiltInIcon("Js Script Icon");
        Texture2D folderCloseIcon = iGUIUtility.LoadBuiltInIcon("Folder On Icon");
        Texture2D folderOpenIcon  = iGUIUtility.LoadBuiltInIcon("FolderOpened On Icon");

        btnTreeViewShow.text = "Show Select";
        btnTreeViewShow.RelativeSize(() => new Vector2(200, -1) ); // -1 for Ignore set Y
        btnTreeViewShow.RelativePosition(iRelativePosition.BOTTOM_IN, panel);
        btnTreeViewShow.RelativePosition(iRelativePosition.LEFT_IN  , panel);
        btnTreeViewShow.OnClicked = (sender) =>
        {
            if (treeView.SelectedItems.Length == 0)
            {
                EditorUtility.DisplayDialog("Selected", "No Selected Item", "OK");
                return;
            }

            StringBuilder builder = new StringBuilder();
            builder.AppendLine("Selected: " +  treeView.SelectedItems[0].text);
            builder.AppendLine("Checked: ");
            treeView.CheckBoxes.ToList().ForEach( o => builder.AppendLine(o.text));

            EditorUtility.DisplayDialog("Selected", builder.ToString(), "OK");
        };


        // TreeView with CheckBox
        treeView.useCheckBox = true;
        // for best practice -> Set size with in RelativeSize for Responsive
        treeView.RelativeSize( () => new Vector2( btnTreeViewShow.width, iGUIUtility.HeightBetween2Objects(panel.position.y, btnTreeViewShow))); 
        treeView.RelativePosition(iRelativePosition.TOP_IN , panel);
        treeView.RelativePosition(iRelativePosition.LEFT_IN, panel);
        treeView.OnToggleStateChanged = (sender, item) =>
        {
            item.icon = item.isOpen ? folderOpenIcon : folderCloseIcon;
        };
        treeView.OnSelectItemChanged = (sender) =>
        {
            if (treeView.SelectedItems.Length == 0)
            {
                Debug.Log("No Selection");
            }
            else
            {
                Debug.Log("Select: " + treeView.SelectedItems[0].text);
            }
        };

        // Method 1: Get returned node and add child
        iTreeViewNode root = treeView.AddChildRoot(folderCloseIcon, "Scripts");
        root.AddChild(csIcon         , "GameManager.cs"     , "GameManager.cs");
        root.AddChild(csIcon         , "PlayerController.cs", "PlayerController.cs");
        root.AddChild(folderCloseIcon, "Utility"            , "Utility");
        root.AddChild(folderCloseIcon, "Extension"          , "Extension");

        // Method 2: Get node by tag and add child
        iTreeViewNode node = treeView.GetNode("Utility");
        node.AddChild(jsIcon, "Helper.js");
        node.AddChild(jsIcon, "Web3.js");

        // Method 3: Manual !!
        treeView.RootNode.children[3].AddChild(csIcon, "Extension.cs");
        treeView.RootNode.children[3].AddChild(csIcon, "iGUI.cs");
        treeView.RootNode.children[3].AddChild(csIcon, "iGUIUtility.cs");

        tabs.AddChild(btnTreeViewShow, 10);
        tabs.AddChild(treeView, 10);

        DrawTreeViewMultipleSelection();
    }


    private void DrawTreeViewMultipleSelection()
    {
        iTreeView treeViewMultiple    = new iTreeView();
        iButton   btnTreeViewMultiple = new iButton();

        Texture2D prefabIcon      = iGUIUtility.LoadBuiltInIcon("Prefab Icon");
        Texture2D objectIcon      = iGUIUtility.LoadBuiltInIcon("GameObject On Icon");
        Texture2D sceneIcon       = iGUIUtility.LoadBuiltInIcon("SceneAsset On Icon");

        btnTreeViewMultiple.text = "Show Select";
        btnTreeViewMultiple.RelativeSize(() => new Vector2(200, -1) );
        btnTreeViewMultiple.RelativePosition(iRelativePosition.BOTTOM_IN, panel);
        btnTreeViewMultiple.RelativePosition(iRelativePosition.RIGHT_OF , btnTreeViewShow);
        btnTreeViewMultiple.OnClicked = (sender) =>
        {
            if (treeViewMultiple.SelectedItems.Length == 0)
            {
                EditorUtility.DisplayDialog("Selected", "No Selected Item", "OK");
                return;
            }

            StringBuilder builder = new StringBuilder();
            treeViewMultiple.SelectedItems.ToList().ForEach( o => 
            {
                builder.AppendLine(o.text);
            });

            EditorUtility.DisplayDialog("Selected", builder.ToString(), "OK");
        };

        // TreeView with Multiple Selection
        treeViewMultiple.multipleSelection = true;
        treeViewMultiple.RelativeSize( () => new Vector2(btnTreeViewMultiple.width ,iGUIUtility.HeightBetween2Objects(panel.position.y, btnTreeViewMultiple)) );
        treeViewMultiple.RelativePosition(iRelativePosition.TOP_IN , panel);
        treeViewMultiple.RelativePosition(iRelativePosition.RIGHT_OF, treeView);
        treeViewMultiple.OnSelectItemChanged = (sender) =>
        {
        };

        iTreeViewNode root = treeViewMultiple.AddChildRoot(sceneIcon, "Example Scene");
                                    root.AddChild(objectIcon, "Main Camera");
                                    root.AddChild(objectIcon, "Directional Light");
        iTreeViewNode playerNode =  root.AddChild(prefabIcon, "Player Prefab", "Player");
                                        playerNode.AddChild(prefabIcon, "Head");
                                        playerNode.AddChild(prefabIcon, "Leg");
                                        playerNode.AddChild(prefabIcon, "Hand")
                                                        .AddChild(prefabIcon, "Gun");
        iTreeViewNode enemyNode  =  root.AddChild(prefabIcon, "Enemy Prefab", "Enemy");
                                        enemyNode.AddChild(prefabIcon, "Sensor");
                                        enemyNode.AddChild(prefabIcon, "Head");
                                        enemyNode.AddChild(prefabIcon, "Leg");


        tabs.AddChild(btnTreeViewMultiple, 10);
        tabs.AddChild(treeViewMultiple, 10);
    }
}