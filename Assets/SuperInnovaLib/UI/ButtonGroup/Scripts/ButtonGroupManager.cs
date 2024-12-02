using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonGroupManager : MonoBehaviour
{
    private static ButtonGroupManager _Instance;
    public static ButtonGroupManager Instance
    {
        get 
        {
            if (_Instance == null)
            {
                GameObject manager = new GameObject("[Script]: Button Group Manager");
                manager.transform.SetAsFirstSibling();
                _Instance = manager.AddComponent<ButtonGroupManager>();
            }

            return _Instance;
        }
    }


    private Dictionary<string, ButtonGroup>       cachedButton = new Dictionary<string, ButtonGroup>();
    private Dictionary<string, List<ButtonGroup>> cachedGroup  = new Dictionary<string, List<ButtonGroup>>();

    private ButtonGroup targetBuffer = null;


    public void SetupGroup(string group, ButtonGroup target)
    {
        if (string.IsNullOrEmpty(group))
        {
            return;
        }

        if (!cachedGroup.ContainsKey(group))
        {
            cachedGroup[group] = new List<ButtonGroup>();
        }

        if (!cachedGroup[group].Contains(target))
        {
            cachedGroup[group].Add(target);
        }

        if (targetBuffer != null)
        {
            Select(targetBuffer);
        }

        if (string.IsNullOrEmpty(target.key) || string.IsNullOrWhiteSpace(target.key)) return;
        cachedButton[group + "." + target.key] = target;
    }


    public void Select(ButtonGroup target)
    {
        if (!cachedGroup.ContainsKey(target.groupName))
        {
            targetBuffer = target;
            return;
        }

        if (!cachedGroup[target.groupName].Contains(target))
        {
            targetBuffer = target;
            return;
        }

        string group = target.groupName;

        if(!DeselectAll(group))
        {
            return;
        }

        target.Select();
        targetBuffer = null;
    }


    public bool DeselectAll(string group)
    {
        if (!cachedGroup.ContainsKey(group))
        {
            return false;
        }

        for(int i = 0; i < cachedGroup[group].Count; i++)
        {
            cachedGroup[group][i].Deselect();
        }

        return true;
    }


    public Button GetButton(string group, string key)
    {
        string refKey = $"{group}.{key}";

        if (cachedButton.ContainsKey(refKey))
        {
            if (cachedButton[refKey] == null)
            {
                cachedButton.Remove(refKey);
                return null;
            }

            return cachedButton[refKey].GetComponent<Button>();
        }

        return null;
    }
}
