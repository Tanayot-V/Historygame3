using System.Collections;
using System.Collections.Generic;
using InnovaFramework.iGUI;
using UnityEditor;
using UnityEngine;

public partial class SimpleWindow : iWindow
{
    private void RenderPageCommon()
    {
        var bgScrollView  = new iBox();
        var scrollView    = new iScrollView();
        var checkBox      = new iCheckBox();
        var maskField     = new iMaskField();
        var dropDown      = new iDropDown();
        var enumProp      = new iEnumPopup<ExampleEnum>();
        var enumPropFlags = new iEnumPopup<ExampleEnumFlags>();
        var animCurve     = new iCurve();
        var rdoChoice1    = new iRadioButton(1);
        var rdoChoice2    = new iRadioButton(2);
        var rdoChoice3    = new iRadioButton(3);
        var rdoChoice4    = new iRadioButton(4);

        bgScrollView.size.x = 128;
        bgScrollView.size.y = panel.height.space();
        bgScrollView.RelativePosition(iRelativePosition.LEFT_IN, panel);
        bgScrollView.RelativePosition(iRelativePosition.TOP_IN, panel);

        scrollView.size         = bgScrollView.size.space();
        scrollView.autoSizeMode = iScrollViewAutoSize.HORIZONTAL;
        scrollView.direction    = iScrollViewDirection.VERTICAL;
        scrollView.padding      = new iPadding(0, 0, 0, 0, 4);
        scrollView.RelativePosition(iRelativePosition.CENTER_X_OF, bgScrollView);
        scrollView.RelativePosition(iRelativePosition.CENTER_Y_OF, bgScrollView);

        for(int i = 0; i < 10; ++i)
        {
            var btn        = new iButton();
            btn.text       = "Index : " + i;
            btn.size.y     = 18;
            btn.miniButton = true;
            btn.OnClicked  = (e) => Debug.Log("Click at : " + e.text);
            scrollView.AddChild(btn);
        }

        checkBox.text = "Hello World";
        checkBox.size.x = 100;
        checkBox.RelativePosition(iRelativePosition.TOP_IN  , panel);
        checkBox.RelativePosition(iRelativePosition.RIGHT_OF, bgScrollView);

        dropDown.text = "Dropdown";
        dropDown.SetOptions(new string[] { "Value 1", "Value 2", "Value 3", "Value 4"});
        dropDown.RelativePosition(iRelativePosition.BOTTOM_OF, checkBox);
        dropDown.RelativePosition(iRelativePosition.LEFT     , checkBox);

        maskField.text = "Mask";
        maskField.SetOptions(new string[] { "Mask 1", "Mask 2", "Mask 3", "Mask 4"});
        maskField.RelativePosition(iRelativePosition.BOTTOM_OF, dropDown);
        maskField.RelativePosition(iRelativePosition.LEFT     , checkBox);

        enumProp.text       = "Enum Popup";
        enumProp.labelSpace = 80;
        enumProp.RelativePosition(iRelativePosition.BOTTOM_OF, maskField);
        enumProp.RelativePosition(iRelativePosition.LEFT     , checkBox);

        enumPropFlags.text       = "Enum Flags";
        enumPropFlags.labelSpace = 80;
        enumPropFlags.RelativePosition(iRelativePosition.BOTTOM_OF, enumProp);
        enumPropFlags.RelativePosition(iRelativePosition.LEFT     , checkBox);

        animCurve.text       = "Curve";
        animCurve.labelSpace = 80;
        animCurve.RelativePosition(iRelativePosition.BOTTOM_OF, enumPropFlags);
        animCurve.RelativePosition(iRelativePosition.LEFT     , checkBox);

        rdoChoice1.text = "Choice 1";
        rdoChoice1.isChecked = true;
        rdoChoice1.RelativePosition(iRelativePosition.TOP_IN , panel);
        rdoChoice1.RelativePosition(iRelativePosition.RIGHT_OF, dropDown);

        rdoChoice2.text = "Choice 2";
        rdoChoice2.RelativePosition(iRelativePosition.BOTTOM_OF, rdoChoice1);
        rdoChoice2.RelativePosition(iRelativePosition.LEFT  , rdoChoice1);
        rdoChoice2.JoinGroup(rdoChoice1);

        rdoChoice3.text = "Choice 3";
        rdoChoice3.RelativePosition(iRelativePosition.BOTTOM_OF, rdoChoice2);
        rdoChoice3.RelativePosition(iRelativePosition.LEFT  , rdoChoice1);
        rdoChoice3.JoinGroup(rdoChoice1);

        rdoChoice4.text = "Choice 4";
        rdoChoice4.RelativePosition(iRelativePosition.BOTTOM_OF, rdoChoice3);
        rdoChoice4.RelativePosition(iRelativePosition.LEFT  , rdoChoice1);
        rdoChoice4.JoinGroup(rdoChoice1);

        rdoChoice1.OnSelected   = OnRadioSelected;
        rdoChoice2.OnSelected   = OnRadioSelected;
        rdoChoice3.OnSelected   = OnRadioSelected;
        rdoChoice4.OnSelected   = OnRadioSelected;

        checkBox.OnChanged      = OnCheckboxChange;
        dropDown.OnChanged      = OnDropdownChange;
        maskField.OnChanged     = OnMaskfieldChange;
        animCurve.OnChanged     = OnCurveChange;
        enumPropFlags.OnChanged = OnEnumFlagChange;


        tabs.AddChild(bgScrollView , 0);
        tabs.AddChild(scrollView   , 0);
        tabs.AddChild(checkBox     , 0);
        tabs.AddChild(dropDown     , 0);
        tabs.AddChild(maskField    , 0);
        tabs.AddChild(enumProp     , 0);
        tabs.AddChild(enumPropFlags, 0);
        tabs.AddChild(animCurve    , 0);
        tabs.AddChild(rdoChoice1   , 0);
        tabs.AddChild(rdoChoice2   , 0);
        tabs.AddChild(rdoChoice3   , 0);
        tabs.AddChild(rdoChoice4   , 0);
    }

}
