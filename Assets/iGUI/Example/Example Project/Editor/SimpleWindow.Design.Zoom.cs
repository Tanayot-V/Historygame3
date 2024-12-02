using System.Collections;
using System.Collections.Generic;
using InnovaFramework.iGUI;
using UnityEditor;
using UnityEngine;

public partial class SimpleWindow : iWindow
{
    private void RenderPageZoom()
    {
        iBox           background    = new iBox();
        iSlider        sliderZoom    = new iSlider(iSliderType.FLOAT);
        iButton        btn1          = new iButton();
        iCheckBox      checkBox      = new iCheckBox();
        iInputField    input         = new iInputField(iInputType.INT);
        iZoomContainer zoomContainer = new iZoomContainer();

        background.size.x = 300;
        background.size.y = panel.height.space();
        background.RelativePosition(iRelativePosition.LEFT_IN, panel);
        background.RelativePosition(iRelativePosition.TOP_IN, panel);

        btn1.size.x  = background.width.space();
        btn1.text    = "Button";
        btn1.RelativePosition(iRelativePosition.LEFT_IN, background);
        btn1.RelativePosition(iRelativePosition.TOP_IN, background);

        input.size.x      = background.width.space();
        input.text        = "Text";
        input.RelativePosition(iRelativePosition.LEFT_IN, background);
        input.RelativePosition(iRelativePosition.BOTTOM_OF, btn1);

        checkBox.size.x    = background.width.space();
        checkBox.text      = "Check Box";
        checkBox.RelativePosition(iRelativePosition.LEFT_IN, background);
        checkBox.RelativePosition(iRelativePosition.BOTTOM_OF, input);

        sliderZoom.Setup(0.0f, 1.0f, 1.0f);
        sliderZoom.text = "Zoom";
        sliderZoom.size.x = 200;
        sliderZoom.RelativePosition(iRelativePosition.RIGHT_OF, background);
        sliderZoom.RelativePosition(iRelativePosition.BOTTOM_IN, panel);

        zoomContainer.vanishingPoint = background.center;

        // Event
        sliderZoom.OnChanged = (sender) => 
        {
            zoomContainer.scale = sliderZoom.value;
        };

        background.useEventCallback = true;
        background.OnMouseClick += (sender, evt) => { Debug.Log("Click");};
        background.OnMouseDown  += (sender, evt) => { Debug.Log("Down");};
        background.OnMouseUp    += (sender, evt) => { Debug.Log("Up");};

        zoomContainer.AddChild(background);
        zoomContainer.AddChild(btn1);
        zoomContainer.AddChild(input);
        zoomContainer.AddChild(checkBox);

        tabs.AddChild(zoomContainer, 6);
        tabs.AddChild(sliderZoom   , 6);

    }

}
