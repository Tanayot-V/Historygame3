using System.Collections;
using System.Collections.Generic;
using InnovaFramework.iGUI;
using UnityEditor;
using UnityEngine;

// TODO
// Event System
// Clear Property, color, backgroundColor blabla

public partial class SimpleWindow : iWindow
{

    public enum ExampleEnum
    {
        ONE,
        TWO,
        THREE,
        FOUR
    }

    [System.Flags]
    public enum ExampleEnumFlags
    {
        FLAG_ONE   = 1,
        FLAG_TWO   = 2,
        FLAG_FOUR  = 4,
        FLAG_EIGHT = 8
    }


    private void OnTabChange(iObject sender)
    {
        iTab tab = (iTab) sender;
        Debug.Log("Tab Change to index : " + tab.index);
    }


    private void OnCheckboxChange(iObject sender)
    {
        iCheckBox cb = (iCheckBox) sender;
        Debug.Log("CheckBox: " + cb.isChecked);
    }


    private void OnSliderChange(iObject sender)
    {
        iSlider slider    = (iSlider)sender;
        progressBar.value = slider.precent;
        progressBar.text  = "Progress Bar: " + (int)(progressBar.value * 100) + "%";
    }


    private void OnDelayedStringChange(iObject sender)
    {
        var ipt = (iInputField)sender;
        Debug.Log("Delayed String: " + ipt.value);
    }


    private void OnDropdownChange(iObject sender)
    {
        iDropDown d = (iDropDown)sender;
        Debug.Log("Dropdown: " + d.selectedItem + ":" + d.selectedObject);
    }


    private void OnMaskfieldChange(iObject sender)
    {
        iMaskField m = (iMaskField)sender;
        Debug.Log("Mask: " + m.flags + ":" + string.Join(", ", m.selectedItem) + ":" + m.IsSelected("Mask 3"));
    }


    private void OnDragUpdate(iObject sender)
    {
        bool isAccept = true;
        if(DragAndDrop.objectReferences.Length > 1)
        {
            isAccept = false;
        }

        if(DragAndDrop.objectReferences[0].GetType() != typeof(GameObject))
        {
            isAccept = false;
        }

        DragAndDrop.visualMode = isAccept ? DragAndDropVisualMode.Link : DragAndDropVisualMode.Rejected;
    }


    private void OnDragPerform(iObject sender)
    {
        if(DragAndDrop.visualMode == DragAndDropVisualMode.Rejected)
        {
            return;
        }

        Debug.Log("Drop with : " + DragAndDrop.objectReferences[0].name);
    }


    private void OnColorChanged(iObject sender)
    {
        iColor color = (iColor) sender;
        progressBar.contentColor = color.value;
    }


    private void OnArrayContainerChanged(iArrayContainer<iInputField> sender)
    {
        // You can access element by "sender[0]"
        // You can access count by "sender.Count"
        Debug.Log("Array Count: " + sender.Items.Length);
    }


    private void OnRadioSelected(iRadioButton rdo, object key)
    {
        Debug.Log($"Select: {rdo.text}, Key: {key}");
    }


    private void OnEnumFlagChange(iObject sender)
    {
        var enumProp = (iEnumPopup<ExampleEnumFlags>)sender;
        Debug.Log("Enum: " + enumProp.value);
    }


    private void OnCurveChange(iObject sender)
    {
        iCurve curve = (iCurve)sender;
        Debug.Log("Curve Length:" + curve.value.length);
    }
}
