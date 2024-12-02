using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Reflection;
using System;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

[CustomPropertyDrawer(typeof(SuperEvent))]
public class MadEventDrawer: PropertyDrawer 
{
    private float spaceHeight = EditorGUIUtility.standardVerticalSpacing;
    private float lineHeight = EditorGUIUtility.singleLineHeight;
    private float prevHeight = 0;

    private string[] monoDefualtMethods = null;

    private GUIStyle btnCloseStyle = null;
    private GUIStyle btnAddStyle = null;
    private GUIStyle btnDelayStyle = null;
    private GUIStyle btnUpStyle = null;

    private List<int> deleteQueue = new List<int>();
    private List<int> moveQueue = new List<int>();

    private bool isFoldoutOpen = false;

    private Dictionary<SerializedProperty, Dictionary<string, SerializedProperty>> cachedProperty = new Dictionary<SerializedProperty, Dictionary<string, SerializedProperty>>();

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        float totalHeight = 0;
        var listEvent = property.FindPropertyRelative("handlerEvent");
        if(isFoldoutOpen)
        {
            totalHeight += ( listEvent.arraySize * ((lineHeight + spaceHeight) +  (spaceHeight * 3)) ) + lineHeight + spaceHeight * 3 + (lineHeight + spaceHeight) + spaceHeight;

            for(int i = 0; i < listEvent.arraySize; ++i)
            {
                var isDelay = listEvent.GetArrayElementAtIndex(i).FindPropertyRelative("isDelay");
                if(!isDelay.boolValue)
                {
                    totalHeight += (lineHeight + spaceHeight);
                }
            }
        }
        else
        {
            totalHeight = lineHeight + spaceHeight;
        }

        return totalHeight;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) 
    {
        #region Initialize
        if (monoDefualtMethods == null)
        {
            Type mono = typeof(MonoBehaviour);
            monoDefualtMethods = mono.GetMethods(BindingFlags.Public | BindingFlags.Instance).Select(m => m.Name).ToArray();
        }

        if (btnCloseStyle == null)
        {
            btnCloseStyle = new GUIStyle();
            btnCloseStyle.normal.background = EditorGUIUtility.FindTexture("CollabDeleted Icon");
        }

        if (btnAddStyle == null)
        {
            btnAddStyle = new GUIStyle();
            btnAddStyle.normal.background = EditorGUIUtility.FindTexture("CollabCreate Icon");
        }

        if (btnUpStyle == null)
        {
            btnUpStyle = new GUIStyle();
            btnUpStyle.normal.background = EditorGUIUtility.FindTexture("CollabMoved Icon");
        }

        if (btnDelayStyle == null)
        {
            btnDelayStyle = new GUIStyle();
            btnDelayStyle.normal.background = EditorGUIUtility.FindTexture("UnityEditor.AnimationWindow@2x");
        }
        #endregion

        EditorGUI.BeginProperty(position, label, property);    

        int indent = EditorGUI.indentLevel;
        EditorGUI.indentLevel = 0;

        prevHeight = lineHeight + spaceHeight;
        deleteQueue.Clear();
        moveQueue.Clear();


        var listEvent = property.FindPropertyRelative("handlerEvent");
        DrawHeader(property, listEvent, position, label);

        if (isFoldoutOpen)
        {
            for (int i = 0, size = listEvent.arraySize; i < size; ++i)
            {
                SerializedProperty currentEvent = listEvent.GetArrayElementAtIndex(i);
                SerializedProperty isDelay = currentEvent.FindPropertyRelative("isDelay");
                SerializedProperty isManualAssign = currentEvent.FindPropertyRelative("isManualAssigned");
                if (!isDelay.boolValue)
                {
                    if(!isManualAssign.boolValue)
                    {
                        DrawEvent(currentEvent, position, i);
                    }
                    else
                    {
                        DrawEventManual(currentEvent, position, i);
                    }
                }
                else
                {
                    DrawDelay(currentEvent, position, i);
                }

                prevHeight += spaceHeight;
            }

            while (deleteQueue.Count > 0)
            {
                listEvent.DeleteArrayElementAtIndex(deleteQueue[0]);
                deleteQueue.RemoveAt(0);
            }

            while (moveQueue.Count > 0)
            {
                listEvent.MoveArrayElement(moveQueue[0], moveQueue[0] - 1);
                moveQueue.RemoveAt(0);
            }

            DrawButton(position, listEvent);
        }
        EditorGUI.indentLevel = indent;
        EditorGUI.EndProperty();
    }

    private void DrawHeader(SerializedProperty property, SerializedProperty listEvent, Rect position, GUIContent label)
    {
        SerializedProperty folding = property.FindPropertyRelative("showing_attr");
        isFoldoutOpen = folding.boolValue;

        int methodCount = 0;
        Rect backgroundRect = new Rect();
        backgroundRect = new Rect()
        {
            x = position.x,
            y = position.y,
            width = position.width,
            // List Size                                                   Button                               Header 
            height = (listEvent.arraySize * (lineHeight + spaceHeight)) + (listEvent.arraySize * spaceHeight * 3) + lineHeight + spaceHeight * 3 + lineHeight + spaceHeight + spaceHeight
        };

        for(int i = 0; i < listEvent.arraySize; ++i)
        {
            var isDelay = listEvent.GetArrayElementAtIndex(i).FindPropertyRelative("isDelay");
            if(!isDelay.boolValue)
            {
                methodCount++;
                backgroundRect.height += (lineHeight + spaceHeight);
            }
        }


        if(isFoldoutOpen)
        {
            GUI.Box(backgroundRect, "", EditorStyles.helpBox);
        }

        backgroundRect = new Rect()
        {
            x = position.x,
            y = position.y,
            width = position.width,
            height = lineHeight + spaceHeight
        };
        var c = GUI.color;
        GUI.color = new Color(0.7f, 0.7f, 0.7f);
        GUI.Box(backgroundRect, "", EditorStyles.helpBox);
        GUI.color = c;

        Rect rectFoldout = new Rect()
        {
            x = position.x + 16,
            y = position.y - 3,
            width = 24,
            height = 24
        };

        folding.boolValue = EditorGUI.Foldout(rectFoldout, folding.boolValue, new GUIContent(label.text + string.Format(" ({0} Functions)", methodCount)), true);
    }

    private void DrawEventManual(SerializedProperty currentEvent, Rect position, int i)
    {
        // Register Cached
        if(!cachedProperty.ContainsKey(currentEvent)) cachedProperty[currentEvent] = new Dictionary<string, SerializedProperty>();
        if(!cachedProperty[currentEvent].ContainsKey("methodName"))         cachedProperty[currentEvent]["methodName"] = currentEvent.FindPropertyRelative("methodName");
        if(!cachedProperty[currentEvent].ContainsKey("fullMethodName"))     cachedProperty[currentEvent]["fullMethodName"] = currentEvent.FindPropertyRelative("fullMethodName");

        SerializedProperty methodName = cachedProperty[currentEvent]["methodName"];
        SerializedProperty fullMethodName = cachedProperty[currentEvent]["fullMethodName"];


        Rect rect = new Rect() 
        {
            x = position.x + 4f,
            y = position.y + prevHeight + spaceHeight + spaceHeight,
            width = 125f,
            height = lineHeight
        };

        EditorGUI.BeginChangeCheck();
        GUI.Label(rect, new GUIContent("Manual Assigned"));

        rect = new Rect()
        {
            x = position.x + 130f + 4f,
            y = rect.y,
            height = rect.height
        };

        float biasWidth = (position.width + position.x) - 16 - lineHeight - lineHeight;
        rect.width = biasWidth - rect.x;

        GUI.enabled = false;
        EditorGUI.Popup(rect, 0, new string[] { methodName.stringValue });
        GUI.enabled = true;

        var rectParam = new Rect()
        {
            x = position.x + 130f + 4f,
            y = rect.y + lineHeight + spaceHeight,
            width = rect.width,
            height = rect.height
        };


        GUIStyle style = new GUIStyle();
        style.normal.textColor = new Color(0.6f, 0.6f, 0.6f);
        GUI.Label(rectParam, "Non-Assigned Parameter", style);

        rect = new Rect()
        {
            x = rect.x + rect.width + 4f,
            y = rect.y,
            width = lineHeight,
            height = rect.height
        };

        prevHeight += lineHeight + spaceHeight;
        prevHeight += lineHeight + spaceHeight * 3;
    }

    private void DrawEvent(SerializedProperty currentEvent, Rect position, int i)
    {
        // Register Cached
        if(!cachedProperty.ContainsKey(currentEvent)) cachedProperty[currentEvent] = new Dictionary<string, SerializedProperty>();
        if(!cachedProperty[currentEvent].ContainsKey("handlerObject"))      cachedProperty[currentEvent]["handlerObject"] = currentEvent.FindPropertyRelative("handlerObject");
        if(!cachedProperty[currentEvent].ContainsKey("currentIndex"))       cachedProperty[currentEvent]["currentIndex"] = currentEvent.FindPropertyRelative("currentIndex");
        if(!cachedProperty[currentEvent].ContainsKey("methodName"))         cachedProperty[currentEvent]["methodName"] = currentEvent.FindPropertyRelative("methodName");
        if(!cachedProperty[currentEvent].ContainsKey("fullMethodName"))     cachedProperty[currentEvent]["fullMethodName"] = currentEvent.FindPropertyRelative("fullMethodName");
        if(!cachedProperty[currentEvent].ContainsKey("paramType"))          cachedProperty[currentEvent]["paramType"] = currentEvent.FindPropertyRelative("paramType");
        if(!cachedProperty[currentEvent].ContainsKey("paramInt"))           cachedProperty[currentEvent]["paramInt"] = currentEvent.FindPropertyRelative("paramInt");
        if(!cachedProperty[currentEvent].ContainsKey("paramFloat"))         cachedProperty[currentEvent]["paramFloat"] = currentEvent.FindPropertyRelative("paramFloat");
        if(!cachedProperty[currentEvent].ContainsKey("paramString"))        cachedProperty[currentEvent]["paramString"] = currentEvent.FindPropertyRelative("paramString");
        if(!cachedProperty[currentEvent].ContainsKey("paramGameObject"))    cachedProperty[currentEvent]["paramGameObject"] = currentEvent.FindPropertyRelative("paramGameObject");
        if(!cachedProperty[currentEvent].ContainsKey("paramBoolean"))       cachedProperty[currentEvent]["paramBoolean"] = currentEvent.FindPropertyRelative("paramBoolean");
        if(!cachedProperty[currentEvent].ContainsKey("paramVector3"))       cachedProperty[currentEvent]["paramVector3"] = currentEvent.FindPropertyRelative("paramVector3");
        if(!cachedProperty[currentEvent].ContainsKey("paramVector2"))       cachedProperty[currentEvent]["paramVector2"] = currentEvent.FindPropertyRelative("paramVector2");
        if(!cachedProperty[currentEvent].ContainsKey("paramObject"))        cachedProperty[currentEvent]["paramObject"] = currentEvent.FindPropertyRelative("paramObject");
        if(!cachedProperty[currentEvent].ContainsKey("paramTransform"))     cachedProperty[currentEvent]["paramTransform"] = currentEvent.FindPropertyRelative("paramTransform");
        if(!cachedProperty[currentEvent].ContainsKey("paramColor"))         cachedProperty[currentEvent]["paramColor"] = currentEvent.FindPropertyRelative("paramColor");
        if(!cachedProperty[currentEvent].ContainsKey("customType"))         cachedProperty[currentEvent]["customType"] = currentEvent.FindPropertyRelative("customType");
        if(!cachedProperty[currentEvent].ContainsKey("paramObject"))        cachedProperty[currentEvent]["paramObject"] = currentEvent.FindPropertyRelative("paramObject");
        if(!cachedProperty[currentEvent].ContainsKey("isCustomParam"))      cachedProperty[currentEvent]["isCustomParam"] = currentEvent.FindPropertyRelative("isCustomParam");



        SerializedProperty handlerObject = cachedProperty[currentEvent]["handlerObject"];
        SerializedProperty currentIndex = cachedProperty[currentEvent]["currentIndex"];
        SerializedProperty methodName = cachedProperty[currentEvent]["methodName"];
        SerializedProperty fullMethodName = cachedProperty[currentEvent]["fullMethodName"];

        bool isCustomProperty = false;
        List<string> listMethods = new List<string>();
        List<string> fullMethods = new List<string>();

        listMethods.Add("No Function");
        listMethods.Add(null);

        fullMethods.Add("No Function");
        fullMethods.Add(null);

        Rect rect = new Rect() 
        {
            x = position.x + 4f,
            y = position.y + prevHeight + spaceHeight + spaceHeight,
            width = 125f,
            height = lineHeight
        };

        EditorGUI.BeginChangeCheck();
        EditorGUI.ObjectField(rect, handlerObject, new GUIContent(""));
        if (EditorGUI.EndChangeCheck())
        {
            currentIndex.intValue = 0;
            cachedProperty.Clear();
        }

        if (handlerObject.objectReferenceValue != null)
        {
            GameObject target = handlerObject.objectReferenceValue as GameObject;
            Component[] allComponents = target.GetComponents(typeof(Component));

            foreach (Component c in allComponents)
            {
                MethodInfo[] mInfo = c.GetType().GetMethods(BindingFlags.Public | BindingFlags.Instance).Where(m => !m.Name.StartsWith("get_") && !m.Name.StartsWith("set_") && !monoDefualtMethods.Contains(m.Name)).ToArray();
                for (int a = 0; a < mInfo.Length; a++)
                {
                    ParameterInfo[] param = mInfo[a].GetParameters();
                    if(param.Length > 1)
                    {
                        continue;
                    }

                    StringBuilder sb = new StringBuilder();
                    sb.Append(" (");
                    if(param.Length == 1)
                    {
                        sb.Append(param[0].ParameterType.Name);
                    }
                    sb.Append(")");

                    StringBuilder sbFull = new StringBuilder();
                    sbFull.Append(" (");
                    if(param.Length == 1)
                    {
                        sbFull.Append(param[0].ParameterType);
                    }
                    sbFull.Append(")");

                    var _type = c.GetType();
                    listMethods.Add(_type.Name + "/" + mInfo[a].Name + sb.ToString());
                    fullMethods.Add(_type.FullName + "/" + mInfo[a].Name + sbFull.ToString());
                }
            }
        }

        // Add Extra Function
        AddExtra(null);
        AddExtra("MadEvent.Extra/SetActive (" + typeof(bool).GetTypeInfo() + ")");
        AddExtra("MadEvent.Extra/SetPosition (" + typeof(Vector3).GetTypeInfo() + ")");
        AddExtra("MadEvent.Extra/SetPosition (" + typeof(Transform).GetTypeInfo() + ")");
        AddExtra("MadEvent.Extra/SetRotation (" + typeof(Transform).GetTypeInfo() + ")");
        AddExtra("MadEvent.Extra/SetPositionAndRotation (" + typeof(Transform).GetTypeInfo() + ")");
        AddExtra("MadEvent.Extra/Translate (" + typeof(Vector3).GetTypeInfo() + ")");
        AddExtra("MadEvent.Extra/Destroy ()");
        AddExtra("MadEvent.Extra/Debug (" + typeof(string) + ")");

        void AddExtra(string method)
        {
            listMethods.Add(method);
            fullMethods.Add(method);
        }

        rect = new Rect()
        {
            x = position.x + 130f + 4f,
            y = rect.y,
            height = rect.height
        };

        float biasWidth = (position.width + position.x) - 16 - lineHeight - lineHeight;
        rect.width = biasWidth - rect.x;

        GUI.enabled = handlerObject.objectReferenceValue != null;

        int lastIndex = fullMethods.FindIndex( o => 
        {
            if(o != null)
            {
                return o.Replace(" (", "(") == fullMethodName.stringValue;
            }
            return false;
        });

        if(lastIndex != -1)
        {
            currentIndex.intValue = lastIndex;
        }

        EditorGUI.BeginChangeCheck();
        currentIndex.intValue = EditorGUI.Popup(rect, currentIndex.intValue, listMethods.ToArray());
        if(EditorGUI.EndChangeCheck())
        {
            cachedProperty[currentEvent]["paramObject"].objectReferenceValue = null;
        }
        GUI.enabled = true;

        if (currentIndex.intValue >= 0 && currentIndex.intValue < listMethods.Count)
        {
            if(listMethods[currentIndex.intValue] == null)
            {
                currentIndex.intValue = 0;
            }

            methodName.stringValue = listMethods[currentIndex.intValue].Replace(" (", "(");
            fullMethodName.stringValue = fullMethods[currentIndex.intValue].Replace(" (", "(");
        }

        var rectParam = new Rect()
        {
            x = position.x + 130f + 4f,
            y = rect.y + lineHeight + spaceHeight,
            width = rect.width,
            height = rect.height
        };


#region Parameter Area
        if(methodName.stringValue.Contains('/'))
        {
            if(!methodName.stringValue.StartsWith("MadEvent.Extra"))
            {
                if(!methodName.stringValue.Contains("("))
                {
                    if(listMethods == null)
                    {
                        DrawEvent(currentEvent, position, i);
                        return;
                    }

                    listMethods.RemoveAll(o => o == null);
                    currentIndex.intValue = listMethods.FindIndex(o => o.StartsWith(methodName.stringValue)) + 1;
                    methodName.stringValue = listMethods[currentIndex.intValue].Replace(" (", "(");
                    DrawEvent(currentEvent, position, i);
                    return;
                }

                string[] methodSplit = fullMethodName.stringValue.Split('/');
                string className = methodSplit[0];
                string[] actualMethodName = methodSplit[1].Replace(")", "").Split('(');

                Type selectedType = GetTypeSelf(className);
                MethodInfo info = null;
                
                if (actualMethodName[1] == "")
                {
                    info = selectedType.GetMethod(actualMethodName[0], new Type[0]);
                }
                else
                {
                    info = selectedType.GetMethod(actualMethodName[0], new Type[] { GetTypeSelf(actualMethodName[1]) });
                }
                isCustomProperty = info.GetCustomAttributes(typeof(SECustomProperty), false).Count() > 0;
            }
            else
            {
                isCustomProperty = true;
            }
            // Reset
            currentEvent.FindPropertyRelative("paramType").stringValue = "";
            currentEvent.FindPropertyRelative("customType").stringValue = "";
            currentEvent.FindPropertyRelative("isCustomParam").boolValue = false;


            if (isCustomProperty && cachedProperty.ContainsKey(currentEvent))
            {
                cachedProperty[currentEvent]["isCustomParam"].boolValue = true;

                Type paramType = null;

                if (methodName.stringValue.EndsWith("Int32)"))              paramType = typeof(int);
                else if (methodName.stringValue.EndsWith("String)"))        paramType = typeof(string);
                else if (methodName.stringValue.EndsWith("Single)"))        paramType = typeof(float);
                else if (methodName.stringValue.EndsWith("Boolean)"))       paramType = typeof(bool);
                else if (methodName.stringValue.EndsWith("GameObject)"))    paramType = typeof(GameObject);
                else if (methodName.stringValue.EndsWith("Transform)"))     paramType = typeof(Transform);
                else if (methodName.stringValue.EndsWith("Vector3)"))       paramType = typeof(Vector3);
                else if (methodName.stringValue.EndsWith("Vector2)"))       paramType = typeof(Vector2);
                else if (methodName.stringValue.EndsWith("Color)"))         paramType = typeof(Color);


                if (paramType == null)
                {
                    var match = Regex.Match(fullMethodName.stringValue, @".*\((.*?)\)");
                    if (match.Groups[1].Value == "")
                    {
                        DrawNoProp();
                    }
                    else
                    {
                        Type type = GetTypeSelf(match.Groups[1].Value);
                        if (type == null)
                        {
                            DrawNoProp();
                        }
                        else
                        {
                            if(type.IsEnum)
                            {
                                string[] enumName = Enum.GetNames(type);
                                cachedProperty[currentEvent]["paramInt"].intValue = EditorGUI.Popup(rectParam, cachedProperty[currentEvent]["paramInt"].intValue, enumName);
                                cachedProperty[currentEvent]["paramString"].stringValue = enumName[cachedProperty[currentEvent]["paramInt"].intValue];
                            }
                            else
                            {
                                currentEvent.FindPropertyRelative("paramObject").objectReferenceValue = EditorGUI.ObjectField(rectParam,currentEvent.FindPropertyRelative("paramObject").objectReferenceValue , type, true);
                            }
                            cachedProperty[currentEvent]["customType"].stringValue = match.Groups[1].Value;
                        }
                    }

                    void DrawNoProp()
                    {
                        GUIStyle style = new GUIStyle();
                        style.normal.textColor = new Color(0.6f, 0.6f, 0.6f);
                        GUI.Label(rectParam, "No Parameter", style);
                    }
                }
                else if (paramType == typeof(int))
                {
                    var paramInt = cachedProperty[currentEvent]["paramInt"];
                    paramInt.intValue = EditorGUI.IntField(rectParam, paramInt.intValue);
                }
                else if (paramType == typeof(float))
                {
                    var paramFloat = cachedProperty[currentEvent]["paramFloat"];
                    paramFloat.floatValue = EditorGUI.FloatField(rectParam, paramFloat.floatValue);
                }
                else if (paramType == typeof(bool))
                {
                    var paramBool = cachedProperty[currentEvent]["paramBoolean"];
                    paramBool.boolValue = EditorGUI.Toggle(rectParam, paramBool.boolValue);
                }
                else if (paramType == typeof(string))
                {
                    var paramString = cachedProperty[currentEvent]["paramString"];
                    paramString.stringValue = EditorGUI.TextField(rectParam, paramString.stringValue);
                }
                else if (paramType == typeof(GameObject))
                {
                    var paramObject = cachedProperty[currentEvent]["paramGameObject"];
                    paramObject.objectReferenceValue = EditorGUI.ObjectField(rectParam, paramObject.objectReferenceValue, paramType, true);
                }
                else if (paramType == typeof(Transform))
                {
                    var paramTransform = cachedProperty[currentEvent]["paramTransform"];
                    paramTransform.objectReferenceValue = EditorGUI.ObjectField(rectParam, paramTransform.objectReferenceValue, paramType, true);
                }
                else if (paramType == typeof(Vector3))
                {
                    var paramVector3 = cachedProperty[currentEvent]["paramVector3"];
                    paramVector3.vector3Value = EditorGUI.Vector3Field(rectParam, "", paramVector3.vector3Value);
                }
                else if (paramType == typeof(Vector2))
                {
                    var paramVector2 = cachedProperty[currentEvent]["paramVector2"];
                    paramVector2.vector2Value = EditorGUI.Vector2Field(rectParam, "", paramVector2.vector2Value);
                }
                else if (paramType == typeof(Color))
                {
                    var paramColor = cachedProperty[currentEvent]["paramColor"];
                    paramColor.colorValue = EditorGUI.ColorField(rectParam, "", paramColor.colorValue);
                }

                if(paramType != null)
                {
                    cachedProperty[currentEvent]["paramType"].stringValue = paramType.GetTypeInfo().ToString();
                }
            }
            else
            {
                GUIStyle style = new GUIStyle();
                style.normal.textColor = new Color(0.6f, 0.6f, 0.6f);
                GUI.Label(rectParam, "Non-Assigned Parameter", style);
            }
        }
        else
        {
            GUIStyle style = new GUIStyle();
            style.normal.textColor = new Color(0.6f, 0.6f, 0.6f);
            GUI.Label(rectParam, "No Function selected", style);
        }
#endregion


        rect = new Rect()
        {
            x = rect.x + rect.width + 4f,
            y = rect.y,
            width = lineHeight,
            height = rect.height
        };

        if (GUI.Button(rect, "", btnCloseStyle))
        {
            deleteQueue.Add(i);
        }

        if(i != 0)
        {
            rect = new Rect()
            {
                x = rect.x + rect.width + 4f,
                y = rect.y,
                width = lineHeight,
                height = rect.height
            };

            Rect guiRect = rect;
            float xValue = ((guiRect.x + guiRect.width / 2.0f));
            float yValue = ((guiRect.y + guiRect.height / 2.0f));
            Vector2 pivot = new Vector2(xValue, yValue);

            GUIUtility.RotateAroundPivot(-90, pivot);
            if (GUI.Button(rect, "", btnUpStyle))
            {
                moveQueue.Add(i);
            }
            GUI.matrix = Matrix4x4.identity;
        }

        prevHeight += lineHeight + spaceHeight;
        prevHeight += lineHeight + spaceHeight * 3;
    }

    private void DrawDelay(SerializedProperty currentEvent, Rect position, int i)
    {
        SerializedProperty delay = currentEvent.FindPropertyRelative("delay");
        SerializedProperty manualAssigned = currentEvent.FindPropertyRelative("isManualAssigned");

        Rect rect = new Rect() 
        {
            x = position.x + 1f,
            y = position.y + prevHeight,
            width = position.width - 2f,
            height = lineHeight + spaceHeight * 3
        };

        GUIStyle bg = new GUIStyle();
        //bg.normal.background = EditorGUIUtility.whiteTexture;

        Color _c = GUI.color;
        //GUI.color = new Color(0.77f, 0.77f, 0.77f);
        GUI.Box(rect, "", bg);
        //GUI.color = _c;

        rect = new Rect() 
        {
            x = position.x + 32,
            y = position.y + prevHeight + spaceHeight + 1,
            width = 125f,
            height = lineHeight
        };

        GUI.Box(new Rect() { x = position.x + 8, y = rect.y, width = lineHeight, height = lineHeight}, "", btnDelayStyle);
        EditorGUI.LabelField(rect, "Delay (second)");

        float biasWidth = (position.width + position.x) - 16 - lineHeight - lineHeight;
        rect = new Rect()
        {
            x = position.x + 130f + 4f,
            y = rect.y + 1f,
            height = rect.height,
            width = biasWidth - ((position.x + 130f) + 4)
        };


        Color c = GUI.backgroundColor;
        GUI.backgroundColor = new Color(0.9f, 0.9f, 0.9f, 0.9f);
        if(manualAssigned.boolValue)
        {
            GUI.enabled = false;
        }
        delay.floatValue = EditorGUI.FloatField(rect, delay.floatValue);
        GUI.enabled = true;
        GUI.backgroundColor = c;

        rect = new Rect()
        {
            x = rect.x + rect.width + 4f,
            y = rect.y,
            width = lineHeight,
            height = rect.height
        };

        if(!manualAssigned.boolValue)
        {
            if (GUI.Button(rect, "", btnCloseStyle))
            {
                deleteQueue.Add(i);
            }

            if (i != 0)
            {
                rect = new Rect()
                {
                    x = rect.x + rect.width + 4f,
                    y = rect.y,
                    width = lineHeight,
                    height = rect.height
                };

                Rect guiRect = rect;
                float xValue = ((guiRect.x + guiRect.width / 2.0f));
                float yValue = ((guiRect.y + guiRect.height / 2.0f));
                Vector2 pivot = new Vector2(xValue, yValue);

                GUIUtility.RotateAroundPivot(-90, pivot);
                if (GUI.Button(rect, "", btnUpStyle))
                {
                    moveQueue.Add(i);
                }
                GUI.matrix = Matrix4x4.identity;
            }
        }

        prevHeight += lineHeight;
        prevHeight += spaceHeight * 3;
    }

    private void DrawButton(Rect position, SerializedProperty listEvent)
    {
        Rect rectDelay = new Rect();
        rectDelay.width = lineHeight;
        rectDelay.x = ((position.width + position.x) - 8f) - rectDelay.width;
        rectDelay.y = position.y + prevHeight + spaceHeight * 2f;
        rectDelay.height = lineHeight;

        Rect rectButton = new Rect();
        rectButton.width = lineHeight;
        rectButton.x = ((position.width + position.x) - 8f) - rectButton.width - rectDelay.width - 4f;
        rectButton.y = position.y + prevHeight + spaceHeight * 2f;
        rectButton.height = lineHeight;

        Rect rectLine = new Rect();
        rectLine.width = position.width - 16;
        rectLine.x = position.x + 8;
        rectLine.y = position.y + prevHeight;
        rectLine.height = 1f;

        GUIStyle line = new GUIStyle();
        line.normal.background = EditorGUIUtility.whiteTexture;
        line.fixedHeight = 1f;
        
        Color c = GUI.color;
        Color backupColor = GUI.color;
        c.a = 0.2f;
        GUI.color = c;
        GUI.Box(rectLine, "", line);
        GUI.color = backupColor;

        if(GUI.Button(rectButton, "", btnAddStyle))
        {
            var lastIndex = listEvent.arraySize;
            listEvent.InsertArrayElementAtIndex(lastIndex);

            SerializedProperty lastObject = listEvent.GetArrayElementAtIndex(lastIndex);
            SerializedProperty obj = lastObject.FindPropertyRelative("handlerObject");
            SerializedProperty isDelay = lastObject.FindPropertyRelative("isDelay");
            SerializedProperty isManualAssigned = lastObject.FindPropertyRelative("isManualAssigned");
            SerializedProperty currentIndex = lastObject.FindPropertyRelative("currentIndex");

            obj.objectReferenceValue = null;
            isDelay.boolValue = false;
            isManualAssigned.boolValue = false;
            currentIndex.intValue = 0;
        }

        if(GUI.Button(rectDelay, "", btnDelayStyle))
        {
            var lastIndex = listEvent.arraySize;
            listEvent.InsertArrayElementAtIndex(lastIndex);

            SerializedProperty lastObject = listEvent.GetArrayElementAtIndex(lastIndex);
            SerializedProperty obj = lastObject.FindPropertyRelative("handlerObject");
            SerializedProperty isDelay = lastObject.FindPropertyRelative("isDelay");
            SerializedProperty delay = lastObject.FindPropertyRelative("delay");
            obj.objectReferenceValue = null;
            isDelay.boolValue = true;
            delay.floatValue = 0.0f;
        }
    }

    public Type GetTypeSelf( string TypeName )
    {
        var type = Type.GetType( TypeName);
        if( type != null ) return type;

        type = Type.GetType(TypeName + ", Assembly-CSharp");
        if( type != null ) return type;

        type = Type.GetType("UnityEngine." + TypeName + ", UnityEngine");
        if( type != null ) return type;

        type = Type.GetType("UnityEngine.UI." + TypeName + ", UnityEngine.UI");
        if( type != null ) return type;

        type = Type.GetType("MadFramework." + TypeName + ", Assembly-CSharp");
        if( type != null ) return type;

        return Type.GetType(TypeName + ", UnityEngine");
    }
}