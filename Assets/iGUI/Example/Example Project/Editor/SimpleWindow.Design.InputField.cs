using System.Collections;
using System.Collections.Generic;
using InnovaFramework.iGUI;
using UnityEditor;
using UnityEngine;

public partial class SimpleWindow : iWindow
{
    private void RenderPageInputField()
    {
        iColor       color       = new iColor();
        iSlider      slider      = new iSlider(iSliderType.INT);
        iInputField  ipInt       = new iInputField(iInputType.INT);
        iInputField  ipFloat     = new iInputField(iInputType.FLOAT);
        iInputField  ipString    = new iInputField(iInputType.STRING);
        iInputField  ipPassword  = new iInputField(iInputType.PASSWORD);
        iInputField  ipObject    = new iInputField(iInputType.OBJECT, typeof(GameObject));

        float size = panel.width.space();

        ipInt.text       = "Input Int";
        ipInt.size.x     = size;
        ipInt.labelSpace = 100;
        ipInt.RelativePosition(iRelativePosition.LEFT_IN, panel);
        ipInt.RelativePosition(iRelativePosition.TOP_IN, panel);

        ipFloat.text       = "Input Float";
        ipFloat.size.x     = size;
        ipFloat.labelSpace = 100;
        ipFloat.RelativePosition(iRelativePosition.BOTTOM_OF, ipInt);
        ipFloat.RelativePosition(iRelativePosition.LEFT, ipInt);

        ipString.text       = "Input String";
        ipString.size.x     = size;
        ipString.labelSpace = 100;
        ipString.RelativePosition(iRelativePosition.BOTTOM_OF, ipFloat);
        ipString.RelativePosition(iRelativePosition.LEFT, ipInt);

        ipObject.text       = "Input Object";
        ipObject.size.x     = size;
        ipObject.labelSpace = 100;
        ipObject.RelativePosition(iRelativePosition.BOTTOM_OF, ipString);
        ipObject.RelativePosition(iRelativePosition.LEFT, ipInt);

        ipPassword.text       = "Input Password";
        ipPassword.size.x     = size;
        ipPassword.labelSpace = 100;
        ipPassword.RelativePosition(iRelativePosition.BOTTOM_OF, ipObject);
        ipPassword.RelativePosition(iRelativePosition.LEFT, ipInt);

        slider.text       = "Slider";
        slider.labelSpace = 100;
        slider.size.x     = ipInt.width;
        slider.Setup(0, 100, 50);
        slider.RelativePosition(iRelativePosition.BOTTOM_OF, ipPassword);
        slider.RelativePosition(iRelativePosition.LEFT, ipInt);

        color.text       = "Color";
        color.size.x     = ipInt.width;
        color.value      = Color.green;
        color.labelSpace = 100;
        color.RelativePosition(iRelativePosition.BOTTOM_OF, slider);
        color.RelativePosition(iRelativePosition.LEFT, ipInt);

        progressBar.text           = "Progress Bar: 50%";
        progressBar.value          = 0.5f;
        progressBar.size.x         = ipInt.width;
        progressBar.RelativePosition(iRelativePosition.BOTTOM_OF, color);
        progressBar.RelativePosition(iRelativePosition.LEFT, ipInt);

        color.OnChanged  = OnColorChanged;
        slider.OnChanged = OnSliderChange;

        tabs.AddChild(ipInt,       1);
        tabs.AddChild(ipFloat,     1);
        tabs.AddChild(ipString,    1);
        tabs.AddChild(ipObject,    1);
        tabs.AddChild(ipPassword,  1);
        tabs.AddChild(slider,      1);
        tabs.AddChild(color,       1);
        tabs.AddChild(progressBar, 1);

    }

}
