using System.Collections;
using System.Collections.Generic;
using InnovaFramework.iGUI;
using UnityEditor;
using UnityEngine;

public partial class SimpleWindow : iWindow
{
    private void RenderPageDragNDrop()
    {
        var dragNdrop     = new iDragAndDrop();

        dragNdrop.size = new Vector2(panel.height.space(), panel.height.space());
        dragNdrop.RelativePosition(iRelativePosition.LEFT_IN, panel);
        dragNdrop.RelativePosition(iRelativePosition.TOP_IN, panel);
        dragNdrop.OnDragUpdate  = OnDragUpdate;
        dragNdrop.OnDrapPerform = OnDragPerform;
        dragNdrop.text = "Drag Prefab to Here";

        tabs.AddChild(dragNdrop   , 4);
    }

}
