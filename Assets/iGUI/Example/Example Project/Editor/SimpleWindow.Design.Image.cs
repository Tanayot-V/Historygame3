using System.Collections;
using System.Collections.Generic;
using InnovaFramework.iGUI;
using UnityEditor;
using UnityEngine;

public partial class SimpleWindow : iWindow
{
    private void RenderPageImage()
    {
        iLabel    labelShowBackground  = new iLabel();
        iLabel    labelHideBackground  = new iLabel();
        iLabel    labelStretchToFill   = new iLabel();
        iLabel    labelScaleAndCrop    = new iLabel();
        iImageBox imgShowBackground    = new iImageBox();
        iImageBox imgHideBackground    = new iImageBox();
        iImageBox imgStretchToFill     = new iImageBox();
        iImageBox imgScaleAndCrop      = new iImageBox();

        float width  = panel.width.space().Split(4);
        float height = 100;
        string imagePath = "iGUIExample/UnityLogo";

        imgShowBackground.size.x = width;
        imgShowBackground.size.y = height;
        imgShowBackground.showBackground = true;
        imgShowBackground.RelativePosition(iRelativePosition.LEFT_IN    , panel);
        imgShowBackground.RelativePosition(iRelativePosition.CENTER_Y_OF, panel);
        imgShowBackground.SetImageFromResources(imagePath);

        imgHideBackground.size.x = width;
        imgHideBackground.size.y = height;
        imgHideBackground.RelativePosition(iRelativePosition.RIGHT_OF, imgShowBackground);
        imgHideBackground.RelativePosition(iRelativePosition.TOP     , imgShowBackground);
        imgHideBackground.SetImageFromResources(imagePath);

        imgStretchToFill.size.x = width;
        imgStretchToFill.size.y = height;
        imgStretchToFill.RelativePosition(iRelativePosition.RIGHT_OF, imgHideBackground);
        imgStretchToFill.RelativePosition(iRelativePosition.TOP     , imgHideBackground);
        imgStretchToFill.SetImageFromResources(imagePath, ScaleMode.StretchToFill);

        imgScaleAndCrop.size.x = width;
        imgScaleAndCrop.size.y = height;
        imgScaleAndCrop.RelativePosition(iRelativePosition.RIGHT_OF, imgStretchToFill);
        imgScaleAndCrop.RelativePosition(iRelativePosition.TOP     , imgStretchToFill);
        imgScaleAndCrop.SetImageFromResources(imagePath, ScaleMode.ScaleAndCrop);

        labelShowBackground.SetText("Show Background");
        labelShowBackground.RelativePosition(iRelativePosition.BOTTOM_OF  , imgShowBackground);
        labelShowBackground.RelativePosition(iRelativePosition.CENTER_X_OF, imgShowBackground);

        labelHideBackground.SetText("Hide Background");
        labelHideBackground.RelativePosition(iRelativePosition.BOTTOM_OF  , imgHideBackground);
        labelHideBackground.RelativePosition(iRelativePosition.CENTER_X_OF, imgHideBackground);

        labelStretchToFill.SetText("Stretch To Fill");
        labelStretchToFill.RelativePosition(iRelativePosition.BOTTOM_OF  , imgStretchToFill);
        labelStretchToFill.RelativePosition(iRelativePosition.CENTER_X_OF, imgStretchToFill);

        labelScaleAndCrop.SetText("Scale And Crop");
        labelScaleAndCrop.RelativePosition(iRelativePosition.BOTTOM_OF  , imgScaleAndCrop);
        labelScaleAndCrop.RelativePosition(iRelativePosition.CENTER_X_OF, imgScaleAndCrop);

        tabs.AddChild(imgShowBackground  , 5);
        tabs.AddChild(imgHideBackground  , 5);
        tabs.AddChild(imgStretchToFill   , 5);
        tabs.AddChild(imgScaleAndCrop    , 5);
        tabs.AddChild(labelShowBackground, 5);
        tabs.AddChild(labelHideBackground, 5);
        tabs.AddChild(labelStretchToFill , 5);
        tabs.AddChild(labelScaleAndCrop  , 5);

    }

}
