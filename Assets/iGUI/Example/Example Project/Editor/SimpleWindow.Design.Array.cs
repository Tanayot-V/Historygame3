using System.Collections;
using System.Collections.Generic;
using InnovaFramework.iGUI;
using UnityEditor;
using UnityEngine;

public partial class SimpleWindow : iWindow
{
    private void RenderPageArray()
    {
        var container      = new iArrayContainer<iInputField>();
        var containerFixed = new iArrayContainer<iButton>();

        container.text     = "Array Container";
        container.size.x   = panel.width.space().Split(2);
        container.isOpened = true;
        container.RelativePosition(iRelativePosition.LEFT_IN, panel);
        container.RelativePosition(iRelativePosition.TOP_IN , panel);
        container.Builder( () => 
        {
            iInputField inputField = new iInputField(iInputType.OBJECT, typeof(GameObject));
            return inputField;
        });


        containerFixed.text        = "Fixed Height & Hide Index";
        containerFixed.size.x      = container.size.x;
        containerFixed.showIndex   = false;
        containerFixed.fixedHeight = 100;
        containerFixed.isOpened    = true;
        container.RelativePosition(iRelativePosition.LEFT_IN, panel);
        containerFixed.RelativePosition(iRelativePosition.RIGHT_OF, container);
        containerFixed.RelativePosition(iRelativePosition.TOP_IN  , panel);
        containerFixed.Builder( () => 
        {
            iButton btn = new iButton();
            btn.text = "{INDEX}";
            btn.OnClicked = (sender) => 
            {
                Debug.Log("Array click at: " + containerFixed[btn]);
            };

            return btn;
        });


        container.OnChanged = OnArrayContainerChanged;

        tabs.AddChild(container     , 3);
        tabs.AddChild(containerFixed, 3);

    }

}
