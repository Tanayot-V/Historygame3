using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InnovaFramework.iGUI;
using System.Linq;
using UnityEditor;
using System.IO;
using System.Text;

public partial class ProjectGenerator : iWindow
{
    private void OnClickBrowse(iObject sender)
    {
        string path = EditorUtility.OpenFolderPanel("Select Folder", "Assets", "");

        int index = path.IndexOf("Assets/");

        if (string.IsNullOrEmpty(path) || index == -1)
        {
            inputPath.stringValue = "Assets";
            return;
        }

        path = path.Remove(0, index);
        inputPath.stringValue = path;
    }


    private void OnClickCreate(iObject sender)
    {
        string path = (string) inputPath.value;
        string name = (string) inputName.value;

        if (!Validate(path, name)) return;

        path = Path.Combine(path, name);
        Directory.CreateDirectory(path);

        path = Path.Combine(path, "Editor");
        Directory.CreateDirectory(path);

        string[] splitedSpace = name.Split(' ');
        for(int i = 0; i < splitedSpace.Length; i++)
        {
            if(splitedSpace[i].Length > 0)
            {
                splitedSpace[i] = char.ToUpper(splitedSpace[i][0]) + splitedSpace[i].Substring(1);
            }
        }

        string splitName = string.Join("", splitedSpace);

        string template       = GetTemplate(splitName);
        string templateDesign = GetTemplateDesign(name, splitName);

        File.WriteAllText(Path.Combine(path, name + ".cs"), template);
        File.WriteAllText(Path.Combine(path, name + ".Design.cs"), templateDesign);
        AssetDatabase.Refresh();

        UnityEngine.Object obj = AssetDatabase.LoadAssetAtPath(Path.Combine(path, name + ".cs"), typeof(UnityEngine.Object));
        Selection.activeObject = obj;
        EditorGUIUtility.PingObject(obj);

        window.Close();
    }


    private string GetTemplateDesign(string original, string name)
    {
        StringBuilder builder = new StringBuilder();
        builder.AppendLine("using System.Collections;");
        builder.AppendLine("using System.Collections.Generic;");
        builder.AppendLine("using System.Linq;");
        builder.AppendLine("using UnityEditor;");
        builder.AppendLine("using UnityEngine;");
        builder.AppendLine("using InnovaFramework.iGUI;");
        builder.AppendLine();

        builder.AppendLine($"public partial class {name}: iWindow");
        builder.AppendLine("{");
        builder.AppendLine($"    public static {name} window;");
        builder.AppendLine();
        builder.AppendLine();
        builder.AppendLine("    [MenuItem(\"Window/" + original + "\", false, -1)]");
        builder.AppendLine("    public static void OpenWindow()");
        builder.AppendLine("    {");
        builder.AppendLine($"        window = GetWindow<{name}>();");
        builder.AppendLine("        window.rect = new Rect(0, 0, 400, 400);");
        builder.AppendLine("        window.titleContent = new GUIContent(\"" + original + "\");");
        builder.AppendLine("        window.maxSize = window.rect.size;");
        builder.AppendLine("        window.minSize = window.maxSize;");
        builder.AppendLine("    }");
        builder.AppendLine();
        builder.AppendLine();
        builder.AppendLine("    private void OnGUI()");
        builder.AppendLine("    {");
        builder.AppendLine("        // Remove this if you don't want to close while unity compiling");
        builder.AppendLine("        if(EditorApplication.isCompiling)");
        builder.AppendLine("        {");
        builder.AppendLine("            if(window != null)");
        builder.AppendLine("            {");
        builder.AppendLine("                window.Close();");
        builder.AppendLine("                return;");
        builder.AppendLine("            }");
        builder.AppendLine("        }");
        builder.AppendLine();
        builder.AppendLine("        // Use this for fix size");
        builder.AppendLine("        if(window != null)");
        builder.AppendLine("        {");
        builder.AppendLine("            window.minSize = window.maxSize;");
        builder.AppendLine("        }");
        builder.AppendLine();
        builder.AppendLine("        base.Render();");
        builder.AppendLine("    }");
        builder.AppendLine();
        builder.AppendLine("    // Initialize function");
        builder.AppendLine("    protected override void OnInitializeUI()");
        builder.AppendLine("    {");
        builder.AppendLine();
        builder.AppendLine("    }");
        builder.AppendLine();
        builder.AppendLine("    // After UI Initialized");
        builder.AppendLine("    protected override void OnAfterInitializedUI()");
        builder.AppendLine("    {");
        builder.AppendLine("        OnBegin();");
        builder.AppendLine("    }");
        builder.AppendLine("}");

        return builder.ToString();
    }


    private string GetTemplate(string name)
    {
        StringBuilder builder = new StringBuilder();
        builder.AppendLine("using System.Collections;");
        builder.AppendLine("using System.Collections.Generic;");
        builder.AppendLine("using InnovaFramework.iGUI;");
        builder.AppendLine("using UnityEditor;");
        builder.AppendLine("using UnityEngine;");
        builder.AppendLine();
        builder.AppendLine($"public partial class {name}: iWindow");
        builder.AppendLine("{");
        builder.AppendLine("    // This function will call after UI initialized");
        builder.AppendLine("    private void OnBegin()");
        builder.AppendLine("    {");
        builder.AppendLine();
        builder.AppendLine("    }");
        builder.AppendLine("}");

        return builder.ToString();
    }


    private bool Validate(string path, string name)
    {
        if (string.IsNullOrEmpty(name) || string.IsNullOrWhiteSpace(name))
        {
            Debug.LogError("[Error.Name]: Name is not correct.");
            return false;
        }

        if (!path.StartsWith("Assets"))
        {
            Debug.LogError("[Error.Path]: Path is not correct.");
            return false;
        }

        if (!Directory.Exists(path))
        {
            Debug.LogError("[Error.DirectoryNotExist]: Directory is not exist.");
            return false;
        }

        if (string.IsNullOrEmpty(name) || string.IsNullOrWhiteSpace(name))
        {
            Debug.LogError("[Error.Name]: Name is not correct.");
            return false;
        }

        if (Directory.Exists(Path.Combine(path, name)))
        {
            Debug.LogError("[Error.DirectoryExist]: Project is existed.");
            return false;
        }

        return true;
    }
}
