using System.Collections;
using System.Collections.Generic;
using InnovaFramework.iGUI;
using UnityEditor;
using UnityEngine;
using System.Linq;
using SuperInnovaLib;
using System.IO;
using Newtonsoft.Json;

public partial class iAPIConfigGenerator : iWindow
{
    private Dictionary<iObject, iAPIInfo> keys = new Dictionary<iObject, iAPIInfo>();
    private iObject currentSelected;


    private void OnInfoChange(iObject sender)
    {
        if (currentSelected == null) return;

        
        keys[currentSelected].host = ipfHost.stringValue;

        int port;
        int.TryParse(ipfPort.stringValue, out port);
        keys[currentSelected].port = port;
        keys[currentSelected].version = ipfVersion.intValue;
    }


    private void OnDefaultChange(iObject sender)
    {
        if (currentSelected == null) return;
        foreach(var kv in keys)
        {
            kv.Value.isDefault = false;
            kv.Key.text = kv.Value.key;
        }

        keys[currentSelected].isDefault = true;
        currentSelected.text = "*" + keys[currentSelected].key;
    }


    private void OnClickAddAPI(iObject sender)
    {
        string key = ipfKey.stringValue;
        if (key == "") return;
        PerformAddAPI(key);
    }


    private void OnClickGenerateConfig(iObject sender)
    {
        string path = EditorUtility.SaveFilePanel("Generate Config", "./", "iConfig.json", "json");
        if (string.IsNullOrEmpty(path))
        {
            return;
        }

        List<iAPIInfo> infos = new List<iAPIInfo>();
        foreach(var kv in keys)
        {
            infos.Add(kv.Value);
        }

        iConfig config = new iConfig();
        config.api = new iConfigAPI();
        config.api.infos = infos.ToArray();

        File.WriteAllText(path, JsonConvert.SerializeObject(config, Formatting.Indented));
        path = path.Replace(@"/", @"\");
        System.Diagnostics.Process.Start("explorer.exe", "/select,"+path);
    }


    private void OnClickOpen(iObject sender)
    {
        string file = EditorUtility.OpenFilePanelWithFilters("Select Config File", "Assets/../", new string[] { "Config File", "json"});
        if (string.IsNullOrEmpty(file))
        {
            return;
        }

        string json = File.ReadAllText(file);
        iConfig config = JsonConvert.DeserializeObject<iConfig>(json);

        if (config.api == null) return;

        ClearAllData();

        for(int i = 0; i < config.api.infos.Length; i++)
        {
            var cfg = config.api.infos[i];
            iButton obj = (iButton)PerformAddAPI(cfg.key);

            if (keys.ContainsKey(obj) && obj != null)
            {
                keys[obj].key       = cfg.key;
                keys[obj].host      = cfg.host;
                keys[obj].port      = cfg.port;
                keys[obj].version   = cfg.version;
                keys[obj].isDefault = cfg.isDefault;
            }

            if (i == 0)
            {
                obj.OnClicked?.Invoke(obj);
            }
        }

        GUI.changed = true;
    }


    private void ClearAllData()
    {
        scvAPIKeys.RemoveChildAll();
        keys.Clear();
        currentSelected = null;
    }


    private iObject PerformAddAPI(string key)
    {
        if (keys.Any( kv => kv.Value.key == key))
        {
            return null;
        }

        iButton btn = new iButton();
        btn.text = key;
        btn.OnClicked = (sender) => 
        {
            GUI.FocusControl(null);

            foreach(var kv in keys)
            {
                kv.Key.enabled = sender != kv.Key;
                kv.Key.backgroundColor = kv.Key.enabled ? new Color(0, 0, 0, 0) : Color.green;
            }

            ipfHost.stringValue   = keys[sender].host;
            ipfPort.stringValue   = keys[sender].port.ToString();
            ipfVersion.intValue   = keys[sender].version;
            cbIsDefault.isChecked = keys[sender].isDefault;

            currentSelected = sender;
            if (keys[sender].isDefault)
            {
                sender.text = "*" + keys[sender].key;
            }

            ipfHost.enabled     = true;
            ipfPort.enabled     = true;
            ipfVersion.enabled  = true;
            cbIsDefault.enabled = true;
        };

        scvAPIKeys.AddChild(btn);
        keys[btn] = new iAPIInfo() { key = key };

        if (keys.Count == 1)
        {
            keys[btn].isDefault = true;
            btn.OnClicked.Invoke(btn);
        }
        ipfKey.stringValue = "";

        return btn;
    }
}