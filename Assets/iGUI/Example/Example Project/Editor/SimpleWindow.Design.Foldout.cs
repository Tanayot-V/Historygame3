using System.Collections;
using System.Collections.Generic;
using InnovaFramework.iGUI;
using UnityEditor;
using UnityEngine;

public partial class SimpleWindow : iWindow
{
    private void RenderPageFoldout()
    {
        iFoldout    foldOut     = new iFoldout();
        iFoldout    foldOut2    = new iFoldout();
        iVector2    position    = new iVector2();
        iVector3    rotation    = new iVector3();
        iInputField ipfName     = new iInputField(iInputType.DELAYED_STRING);
        iInputField ipfLastName = new iInputField(iInputType.DELAYED_STRING);

        // Fold 1
        foldOut.text = "Transform";
        foldOut.RelativePosition(iRelativePosition.TOP_IN , panel);
        foldOut.RelativePosition(iRelativePosition.LEFT_IN, panel);

        position.text = "Position";
        position.RelativePosition(iRelativePosition.LEFT_IN  , foldOut, iGUIUtility.spaceX2);
        position.RelativePosition(iRelativePosition.BOTTOM_OF, foldOut, iGUIUtility.spaceHalf);

        rotation.text = "Rotation";
        rotation.RelativePosition(iRelativePosition.LEFT_IN  , foldOut , iGUIUtility.spaceX2);
        rotation.RelativePosition(iRelativePosition.BOTTOM_OF, position, iGUIUtility.spaceHalf);

        // Fold 2
        foldOut2.text = "Property";
        foldOut2.RelativePosition(iRelativePosition.LEFT_IN  , panel);
        foldOut2.RelativePosition(iRelativePosition.BOTTOM_OF, foldOut);

        ipfName.text = "Name";
        ipfName.RelativePosition(iRelativePosition.LEFT_IN  , foldOut2, iGUIUtility.spaceX2);
        ipfName.RelativePosition(iRelativePosition.BOTTOM_OF, foldOut2, iGUIUtility.spaceHalf);

        ipfLastName.text = "Last";
        ipfLastName.RelativePosition(iRelativePosition.LEFT_IN  , foldOut2, iGUIUtility.spaceX2);
        ipfLastName.RelativePosition(iRelativePosition.BOTTOM_OF, ipfName , iGUIUtility.spaceHalf);


        ipfName.OnChanged     = OnDelayedStringChange;
        ipfLastName.OnChanged = OnDelayedStringChange;


        foldOut.AddChild(position);
        foldOut.AddChild(rotation);

        foldOut2.AddChild(ipfName);
        foldOut2.AddChild(ipfLastName);

        tabs.AddChild(foldOut , 2);
        tabs.AddChild(foldOut2, 2);

    }

}
