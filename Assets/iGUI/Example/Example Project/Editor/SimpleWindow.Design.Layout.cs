using System.Collections;
using System.Collections.Generic;
using InnovaFramework.iGUI;
using UnityEditor;
using UnityEngine;

public partial class SimpleWindow : iWindow
{
    private void RenderPageLayout()
    {
        iBox topIN_leftIN     = new iBox();
        iBox bottomIN_rightIN = new iBox();
        iBox rightOf          = new iBox();
        iBox leftOf           = new iBox();

        iBox split4_1         = new iBox();
        iBox split4_2         = new iBox();
        iBox split4_3         = new iBox();
        iBox split4_4         = new iBox();

        iBox percent_1_5      = new iBox();
        iBox percent_2_0      = new iBox();
        iBox percent_3_0      = new iBox();
        iBox percent_3_5      = new iBox();

        iBox fillSize         = new iBox();

        // Relative
        topIN_leftIN.text = "TOP_IN & LEFT_IN";
        topIN_leftIN.size.x = 200;
        topIN_leftIN.size.y = 32;
        topIN_leftIN.backgroundColor = Color.green;
        topIN_leftIN.RelativePosition(iRelativePosition.TOP_IN , panel);
        topIN_leftIN.RelativePosition(iRelativePosition.LEFT_IN, panel);

        rightOf.text = "TOP & RIGHT_OF";
        rightOf.size.x = 200;
        rightOf.size.y = 32;
        rightOf.backgroundColor = Color.green;
        rightOf.RelativePosition(iRelativePosition.TOP     , topIN_leftIN);
        rightOf.RelativePosition(iRelativePosition.RIGHT_OF, topIN_leftIN);

        bottomIN_rightIN.text = "BOTTOM_IN & RIGHT_IN";
        bottomIN_rightIN.size.x = 200;
        bottomIN_rightIN.size.y = 32;
        bottomIN_rightIN.backgroundColor = Color.green;
        bottomIN_rightIN.RelativePosition(iRelativePosition.BOTTOM_IN , panel);
        bottomIN_rightIN.RelativePosition(iRelativePosition.RIGHT_IN  , panel);

        leftOf.text = "TOP & LEFT_OF";
        leftOf.size.x = 200;
        leftOf.size.y = 32;
        leftOf.backgroundColor = Color.green;
        leftOf.RelativePosition(iRelativePosition.TOP     , bottomIN_rightIN);
        leftOf.RelativePosition(iRelativePosition.LEFT_OF , bottomIN_rightIN);

        // Split Size
        float widthSplited = iGUIUtility.SplitSize(panel.width.space(), 4);

        split4_1.text            = "Split 1";
        split4_1.size.y          = 32;
        split4_1.size.x          = widthSplited;
        split4_1.backgroundColor = Color.yellow;
        split4_1.RelativePosition(iRelativePosition.LEFT_IN, panel);
        split4_1.RelativePosition(iRelativePosition.BOTTOM_OF, topIN_leftIN);

        split4_2.text            = "Split 2";
        split4_2.size.y          = 32;
        split4_2.size.x          = widthSplited;
        split4_2.backgroundColor = Color.yellow;
        split4_2.RelativePosition(iRelativePosition.RIGHT_OF, split4_1);
        split4_2.RelativePosition(iRelativePosition.TOP     , split4_1);

        split4_3.text            = "Split 3";
        split4_3.size.y          = 32;
        split4_3.size.x          = widthSplited;
        split4_3.backgroundColor = Color.yellow;
        split4_3.RelativePosition(iRelativePosition.RIGHT_OF, split4_2);
        split4_3.RelativePosition(iRelativePosition.TOP     , split4_1);

        split4_4.text            = "Split 4";
        split4_4.size.y          = 32;
        split4_4.size.x          = widthSplited;
        split4_4.backgroundColor = Color.yellow;
        split4_4.RelativePosition(iRelativePosition.RIGHT_OF, split4_3);
        split4_4.RelativePosition(iRelativePosition.TOP     , split4_1);

        // Split Percent
        float[] sizes = iGUIUtility.SplitSizeByPercentage(panel.width.space(), 1.5f, 2.0f, 3.0f, 3.5f);

        percent_1_5.text   = "15%";
        percent_1_5.size.y = 32;
        percent_1_5.size.x = sizes[0];
        percent_1_5.backgroundColor = Color.yellow;
        percent_1_5.RelativePosition(iRelativePosition.LEFT_IN, panel);
        percent_1_5.RelativePosition(iRelativePosition.TOP_OF, bottomIN_rightIN);

        percent_2_0.text   = "20%";
        percent_2_0.size.y = 32;
        percent_2_0.size.x = sizes[1];
        percent_2_0.backgroundColor = Color.yellow;
        percent_2_0.RelativePosition(iRelativePosition.RIGHT_OF, percent_1_5);
        percent_2_0.RelativePosition(iRelativePosition.TOP, percent_1_5);

        percent_3_0.text   = "30%";
        percent_3_0.size.y = 32;
        percent_3_0.size.x = sizes[2];
        percent_3_0.backgroundColor = Color.yellow;
        percent_3_0.RelativePosition(iRelativePosition.RIGHT_OF, percent_2_0);
        percent_3_0.RelativePosition(iRelativePosition.TOP, percent_2_0);

        percent_3_5.text   = "35%";
        percent_3_5.size.y = 32;
        percent_3_5.size.x = sizes[3];
        percent_3_5.backgroundColor = Color.yellow;
        percent_3_5.RelativePosition(iRelativePosition.RIGHT_OF, percent_3_0);
        percent_3_5.RelativePosition(iRelativePosition.TOP, percent_3_0);

        // Fill Size

        fillSize.text = "Fill Width & Height";
        fillSize.size.x = iGUIUtility.WidthBetween2Objects(panel.position.x, panel.right);
        fillSize.size.y = iGUIUtility.HeightBetween2Objects(split4_1, percent_1_5);
        fillSize.backgroundColor = Color.cyan;
        fillSize.RelativePosition(iRelativePosition.LEFT_IN, panel);
        fillSize.RelativePosition(iRelativePosition.BOTTOM_OF, split4_1);

        tabs.AddChild(topIN_leftIN    , 7);
        tabs.AddChild(bottomIN_rightIN, 7);
        tabs.AddChild(rightOf         , 7);
        tabs.AddChild(leftOf          , 7);

        tabs.AddChild(split4_1        , 7);
        tabs.AddChild(split4_2        , 7);
        tabs.AddChild(split4_3        , 7);
        tabs.AddChild(split4_4        , 7);

        tabs.AddChild(percent_1_5     , 7);
        tabs.AddChild(percent_2_0     , 7);
        tabs.AddChild(percent_3_0     , 7);
        tabs.AddChild(percent_3_5     , 7);

        tabs.AddChild(fillSize        , 7);
    }

}
