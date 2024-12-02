using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

[CustomPropertyDrawer(typeof(ButtonGroupEffect))]
public class ButtonGroupEditor : PropertyDrawer
{
    public int lineCount = 0;
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        GUI.changed = true;
        Rect rect   = new Rect();
        lineCount   = 0;

        // Get All Property
        SerializedProperty effectType        = property.FindPropertyRelative("effectType");
        SerializedProperty effectShowAndHide = property.FindPropertyRelative("effectShowAndHideTarget");
        SerializedProperty effectSwapSprite  = property.FindPropertyRelative("effectSwapSprite");
        SerializedProperty effectColor       = property.FindPropertyRelative("effectColor");
        SerializedProperty effectAnimator    = property.FindPropertyRelative("effectAnimator");
        SerializedProperty effectVector2     = property.FindPropertyRelative("effectVector2");
        SerializedProperty effectVector3     = property.FindPropertyRelative("effectVector3");
        SerializedProperty transition        = property.FindPropertyRelative("transition");
        SerializedProperty transitionValue   = property.FindPropertyRelative("transitionValue");
        SerializedProperty targetEffect      = property.FindPropertyRelative("targetEffect");
        SerializedProperty targetRect        = property.FindPropertyRelative("targetRect");

        EditorGUI.BeginProperty(position, label, property);

        // Title 
        (string titleName, string texture, Color color) = GetTitleName(effectType.enumValueIndex);
        GUIStyle style  = new GUIStyle(EditorStyles.label);
        style.fontStyle = FontStyle.Bold;

        Color c          = GUI.contentColor;
        GUI.contentColor = color;
        {
            EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), new GUIContent(titleName, EditorGUIUtility.IconContent(texture).image), style);
        }
        GUI.contentColor = c;

        int indent            = EditorGUI.indentLevel;
        EditorGUI.indentLevel = 1;

        // Draw Type Enum
        rect = GetRect(position, ++lineCount);
        EditorGUI.PropertyField(rect, effectType, new GUIContent("Type"));

        switch(effectType.enumValueIndex)
        {
            case 0: // Draw Show And Hide Property
            {
                rect = GetRect(position, ++lineCount);
                EditorGUI.PropertyField(rect, effectShowAndHide, new GUIContent("Target"));
                break;
            }
            case 1: // Draw Swap Sprite Property
            {
                rect = GetRect(position, ++lineCount);
                EditorGUI.PropertyField(rect, effectSwapSprite, new GUIContent("Sprite"));

                rect = GetRect(position, ++lineCount);
                EditorGUI.PropertyField(rect, targetEffect, new GUIContent("Target"));
                break;
            }
            case 2: // Draw Color Property
            {
                rect = GetRect(position, ++lineCount);
                EditorGUI.PropertyField(rect, effectColor, new GUIContent("Color"));

                rect = GetRect(position, ++lineCount);
                EditorGUI.PropertyField(rect, targetEffect, new GUIContent("Target"));
                break;
            }
            case 3: // Draw Animation Property
            {
                rect = GetRect(position, ++lineCount);
                EditorGUI.PropertyField(rect, effectAnimator, new GUIContent("Animator"));

                rect        = GetRect(position, ++lineCount);
                rect.x     += EditorGUIUtility.labelWidth;
                rect.width -= EditorGUIUtility.labelWidth;

                float width = rect.width;
                rect.width  = width * 0.7f - 2;
                if(GUI.Button(rect, new GUIContent("Generate Animator", EditorGUIUtility.IconContent("CreateAddNew").image), EditorStyles.miniButton))
                {
                    GenerateAnimator(property);
                }

                rect.x     += rect.width + 4;
                rect.width  = width * 0.3f - 2;
                if(GUI.Button(rect, new GUIContent("Load", EditorGUIUtility.IconContent("Download-Available").image), EditorStyles.miniButton))
                {
                    LoadAnimator(property);
                }
                break;
            }
            case 4: // Draw Scale Property
            {
                rect = GetRect(position, ++lineCount);
                EditorGUI.PropertyField(rect, effectVector2, new GUIContent("Multiplier"));

                rect = GetRect(position, ++lineCount);

                float width = rect.width;
                width      -= EditorGUIUtility.labelWidth - 4;
                rect.width  = width * 0.7f;
                rect.width += EditorGUIUtility.labelWidth;
                EditorGUI.PropertyField(rect, transition   , new GUIContent("Transition"));

                rect.x     += rect.width - 4;
                rect.width  = width * 0.3f;

                if (transition.enumValueIndex == 0)
                {
                    GUI.enabled = false;
                }
                EditorGUI.PropertyField(rect, transitionValue, new GUIContent());
                GUI.enabled = true;

                rect = GetRect(position, ++lineCount);
                EditorGUI.PropertyField(rect, targetRect , new GUIContent("Target"));
                break;
            }
            case 5: // Draw Translate Property
            {
                rect = GetRect(position, ++lineCount);
                EditorGUI.PropertyField(rect, effectVector2, new GUIContent("Translate"));

                rect = GetRect(position, ++lineCount);

                float width = rect.width;
                width      -= EditorGUIUtility.labelWidth - 4;
                rect.width  = width * 0.7f;
                rect.width += EditorGUIUtility.labelWidth;
                EditorGUI.PropertyField(rect, transition   , new GUIContent("Transition"));

                rect.x     += rect.width - 4;
                rect.width  = width * 0.3f;
                if (transition.enumValueIndex == 0)
                {
                    GUI.enabled = false;
                }
                EditorGUI.PropertyField(rect, transitionValue, new GUIContent());
                GUI.enabled = true;

                rect = GetRect(position, ++lineCount);
                EditorGUI.PropertyField(rect, targetRect , new GUIContent("Target"));
                break;
            }
            case 6: // Draw Rotation Property
            {
                rect = GetRect(position, ++lineCount);
                EditorGUI.PropertyField(rect, effectVector3, new GUIContent("Rotate"));

                rect = GetRect(position, ++lineCount);

                float width = rect.width;
                width      -= EditorGUIUtility.labelWidth - 4;
                rect.width  = width * 0.7f;
                rect.width += EditorGUIUtility.labelWidth;
                EditorGUI.PropertyField(rect, transition   , new GUIContent("Transition"));

                rect.x     += rect.width - 4;
                rect.width  = width * 0.3f;
                if (transition.enumValueIndex == 0)
                {
                    GUI.enabled = false;
                }
                EditorGUI.PropertyField(rect, transitionValue, new GUIContent());
                GUI.enabled = true;

                rect = GetRect(position, ++lineCount);
                EditorGUI.PropertyField(rect, targetRect , new GUIContent("Target"));
                break;
            }
        }

        EditorGUI.indentLevel = indent;
        EditorGUI.EndProperty();
    }


    public Rect GetRect(Rect refRect, int lineCount)
    {
        var rect = new Rect(refRect.x, refRect.y + (EditorGUIUtility.singleLineHeight * lineCount) + (EditorGUIUtility.standardVerticalSpacing * (lineCount - 1)), refRect.width, EditorGUIUtility.singleLineHeight);
        return rect;
    }


    public (string, string, Color) GetTitleName(int index)
    {
        switch(index)
        {
            case 0: return (" Show and Hide", "scenevis_visible_hover@2x" , new Color(0.0f, 1.0f, 1.0f));
            case 1: return (" Sprite"       , "RawImage Icon"             , new Color(0.3f, 1.0f, 0.3f));
            case 2: return (" Color"        , "eyeDropper.Large@2x"       , new Color(1.0f, 0.5f, 1.0f));
            case 3: return (" Animator"     , "AnimationClip On Icon"     , new Color(1.0f, 1.0f, 0.0f));
            case 4: return (" Scale"        , "ScaleTool On"              , new Color(1.0f, 0.6f, 0.3f));
            case 5: return (" Translate"    , "MoveTool on"               , new Color(1.0f, 0.6f, 0.3f));
            case 6: return (" Rotation"     , "RotateTool On"             , new Color(1.0f, 0.6f, 0.3f));
        }

        return ("", "", Color.white);
    }


    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        int lineCount = 0;

        SerializedProperty effectType = property.FindPropertyRelative("effectType");
        switch(effectType.enumValueIndex)
        {
            case 0: lineCount = 2; break;
            case 1: lineCount = 3; break;
            case 2: lineCount = 3; break;
            case 3: lineCount = 3; break;
            case 4: lineCount = 4; break;
            case 5: lineCount = 4; break;
            case 6: lineCount = 4; break;
        }
        return EditorGUIUtility.singleLineHeight * (lineCount + 1) + EditorGUIUtility.standardVerticalSpacing * (lineCount);
    }


    private void LoadAnimator(SerializedProperty property)
    {
        ButtonGroup target = (ButtonGroup)property.serializedObject.targetObject;
        if (target == null) return;

        string path = EditorUtility.OpenFilePanel("Select Animator", "", "controller");
        if (string.IsNullOrEmpty(path)) return;

        if (path.StartsWith(Application.dataPath))
        {
            path = "Assets" + path.Substring(Application.dataPath.Length);
        }

        AnimatorController controller = AssetDatabase.LoadAssetAtPath<AnimatorController>(path);
        if (controller == null)
        {
            return;
        }

        if (!controller.parameters.Any( o => o.name == "Idle" && o.type == AnimatorControllerParameterType.Trigger))
        {
            Debug.LogWarning("Animator doesn't have \'Idle\' as Trigger Parameter"); return;
        }

        if (!controller.parameters.Any( o => o.name == "Select" && o.type == AnimatorControllerParameterType.Trigger))
        {
            Debug.LogWarning("Animator doesn't have \'Select\' as Trigger Parameter"); return;
        }

        Animator animator = target.gameObject.GetComponent<Animator>();
        if (animator == null)
        {
            animator = target.gameObject.AddComponent<Animator>();
        }

        if (animator.runtimeAnimatorController == null)
        {
            animator.runtimeAnimatorController = controller;
        }

        property.FindPropertyRelative("effectAnimator").objectReferenceValue = animator;
        property.serializedObject.ApplyModifiedProperties();
    }


    private void GenerateAnimator(SerializedProperty property)
    {
        ButtonGroup target = (ButtonGroup)property.serializedObject.targetObject;
        if (target == null) return;

        string path = EditorUtility.SaveFilePanel("Select Directory", "", $"{target.gameObject.name}_ButtonGroup.controller", "controller");
        if (string.IsNullOrEmpty(path)) return;

        if (path.StartsWith(Application.dataPath))
        {
            path = "Assets" + path.Substring(Application.dataPath.Length);
        }

        string dirPath  = Path.GetDirectoryName(path);
        string filePath = Path.GetFileNameWithoutExtension(path);

        AnimatorController controller = AnimatorController.CreateAnimatorControllerAtPath(path);

        AnimationClip idleClip     = new AnimationClip();
        AnimationClip selectedClip = new AnimationClip();
        idleClip.name     = "Idle";
        selectedClip.name = "Selected";

        AssetDatabase.AddObjectToAsset(idleClip    , path);
        AssetDatabase.AddObjectToAsset(selectedClip, path);
        AssetDatabase.SaveAssets();

        Animator animator = target.gameObject.GetComponent<Animator>();
        if (animator == null)
        {
            animator = target.gameObject.AddComponent<Animator>();
        }

        if (animator.runtimeAnimatorController == null)
        {
            animator.runtimeAnimatorController = controller;
        }

        controller.AddParameter("Idle"  , AnimatorControllerParameterType.Trigger);
        controller.AddParameter("Select", AnimatorControllerParameterType.Trigger);

        var rootStateMachine = controller.layers[0].stateMachine;
        var idleState        = rootStateMachine.AddState("Idle");
        var selectState      = rootStateMachine.AddState("Select");

        var idleTransition   = rootStateMachine.AddAnyStateTransition(idleState);
        var selectTransition = rootStateMachine.AddAnyStateTransition(selectState);

        idleState.motion   = idleClip;
        selectState.motion = selectedClip;

        idleTransition.AddCondition(AnimatorConditionMode.If  , 0, "Idle");
        selectTransition.AddCondition(AnimatorConditionMode.If, 0, "Select");

        idleTransition.exitTime         = 0;
        idleTransition.hasFixedDuration = false;
        idleTransition.duration         = 0;

        selectTransition.exitTime         = 0;
        selectTransition.hasFixedDuration = false;
        selectTransition.duration         = 0;

        property.FindPropertyRelative("effectAnimator").objectReferenceValue = animator;
        property.serializedObject.ApplyModifiedProperties();
        EditorUtility.SetDirty(controller);
    }
}
