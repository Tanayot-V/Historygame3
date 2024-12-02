using System;
using System.Collections;
using System.Collections.Generic;
using InnovaFramework.iGUI;
using UnityEditor;
using UnityEngine;

public partial class SimpleWindow : iWindow
{
    private iLabel txtDateSingle;
    private iLabel txtDateStartEnd;
    private iLabel txtDatePopup;

    private iLabel dateSingle;
    private iLabel dateStartEnd;
    private iLabel datePopup;

    private iDatePicker datePickerSingle;
    private iDatePicker datePickerStartEnd;
    private iDatePicker datePickerPopup;
    private iButton btnDatePopup;

    private void RenderPageDateTime()
    {
        txtDateSingle      = new iLabel();
        txtDateStartEnd    = new iLabel();
        txtDatePopup       = new iLabel();

        dateSingle         = new iLabel();
        dateStartEnd       = new iLabel();
        datePopup          = new iLabel();

        datePickerSingle   = new iDatePicker();
        datePickerStartEnd = new iDatePicker();
        datePickerPopup    = new iDatePicker();
        btnDatePopup       = new iButton();

        Rect[] rectCols = iGUIUtility.SplitColumn(panel.rect, 3);

        // Single Date
        txtDateSingle.SetText("Single Date");
        txtDateSingle.RelativePosition(iRelativePosition.CENTER_X_OF, rectCols[0]);
        txtDateSingle.RelativePosition(iRelativePosition.TOP_IN, panel);

        dateSingle.SetText(DateTime.Now.ToString("dd/MM/yyyy"));
        dateSingle.RelativePosition(iRelativePosition.CENTER_X_OF, rectCols[0]);
        dateSingle.RelativePosition(iRelativePosition.BOTTOM_IN, panel);

        datePickerSingle.SetYearsRange(1990, 2030);
        datePickerSingle.RelativePosition(iRelativePosition.CENTER_X_OF, rectCols[0]);
        datePickerSingle.RelativePosition(iRelativePosition.CENTER_Y_OF, rectCols[0]);
        datePickerSingle.OnChanged = (sender) =>
        {
            dateSingle.SetText(datePickerSingle.Selected.ToString("dd/MM/yyyy"));
        };

        // Start-End Date
        txtDateStartEnd.SetText("Start-End Date");
        txtDateStartEnd.RelativePosition(iRelativePosition.CENTER_X_OF, rectCols[1]);
        txtDateStartEnd.RelativePosition(iRelativePosition.TOP_IN, panel);

        dateStartEnd.SetText(DateTime.Now.ToString("dd/MM/yyyy") + " - " + DateTime.Now.ToString("dd/MM/yyyy"));
        dateStartEnd.RelativePosition(iRelativePosition.CENTER_X_OF, rectCols[1]);
        dateStartEnd.RelativePosition(iRelativePosition.BOTTOM_IN, panel);

        datePickerStartEnd.useStartEndMode = true;
        datePickerStartEnd.SetTheme(null, new Color(0.55f, 0.32f, 0.55f), new Color(0.61f, 0.21f, 0.56f), null, null);
        datePickerStartEnd.RelativePosition(iRelativePosition.CENTER_Y_OF, rectCols[1]);
        datePickerStartEnd.RelativePosition(iRelativePosition.CENTER_X_OF, rectCols[1]);
        datePickerStartEnd.OnChanged = (sender) =>
        {
            dateStartEnd.SetText(datePickerStartEnd.Selected.ToString("dd/MM/yyyy") + " - " + datePickerStartEnd.Selected2.ToString("dd/MM/yyyy"));
        };

        // Popup Date
        txtDatePopup.SetText("Popup Date");
        txtDatePopup.RelativePosition(iRelativePosition.CENTER_X_OF, rectCols[2]);
        txtDatePopup.RelativePosition(iRelativePosition.TOP_IN, panel);

        datePopup.SetText(DateTime.Now.ToString("dd/MM/yyyy"));
        datePopup.RelativePosition(iRelativePosition.CENTER_X_OF, rectCols[2]);
        datePopup.RelativePosition(iRelativePosition.BOTTOM_IN, panel);

        btnDatePopup.size = new Vector2(64, 64);
        btnDatePopup.LoadBuiltInIcon("mat-2-icon.calendar_month");
        btnDatePopup.RelativePosition(iRelativePosition.CENTER_X_OF, rectCols[2]);
        btnDatePopup.RelativePosition(iRelativePosition.CENTER_Y_OF, rectCols[2]);
        btnDatePopup.OnClicked = (s) =>
        {
            datePickerPopup.Show((dt1, dt2) => 
            {
                if (dt1 == null) return;
                datePopup.SetText(dt1.Value.ToString("dd/MM/yyyy"));
            }, datePickerPopup.Selected);
        };

        datePickerPopup.size       = datePickerPopup.size.RatioY(300);
        datePickerPopup.fontSize   = 12;
        datePickerPopup.useAsPopup = true; // <--- Set this to make it popup style and use Show to open it.
        datePickerPopup.RelativePosition(iRelativePosition.RIGHT , btnDatePopup);
        datePickerPopup.RelativePosition(iRelativePosition.TOP_OF, btnDatePopup);

        // AddChild
        tabs.AddChild(txtDateSingle      , 9);
        tabs.AddChild(dateSingle         , 9);
        tabs.AddChild(datePickerSingle   , 9);

        tabs.AddChild(txtDateStartEnd    , 9);
        tabs.AddChild(dateStartEnd       , 9);
        tabs.AddChild(datePickerStartEnd , 9);

        tabs.AddChild(txtDatePopup       , 9);
        tabs.AddChild(datePopup          , 9);
        tabs.AddChild(btnDatePopup       , 9);
        tabs.AddChild(datePickerPopup    , 9); // AddChild with anything will be ok i.e, AddChild() with window is ok too.
    }
}