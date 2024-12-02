using System;
using System.Collections;
using System.Collections.Generic;
using System.IO.Compression;
using SuperInnovaLib;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public enum apiEnv
{
    DEVELOPMENT,
    PRODUCTION,
}

public class MainApi : MonoBehaviour
{
    public string url = iAPIConfig.LOCALHOST;
    public int port = 9196,version = 1;
    public apiEnv env = apiEnv.DEVELOPMENT;

    public static MainApi instance;
    void Awake()
    {
        if(instance != null)
        {
            Destroy(this);
            return;
        }
        instance = this;
        DontDestroyOnLoad(this);

        iAPIConfig.Create(url, port, version, env.ToString().ToLower());
        iAPIConfig.CreateFromConfig();
    }

    public string sessionToken;
    [Space(10)]
    [Header("Test")]
    public TMP_Text displayNameText;
    public TMP_Text zoneText;

    void Start()
    {
        iSession.Clear();
        iSession.Save(sessionToken);
    }

    public void TestGetUserInfo()
    {
        GetUserInfo((r,data) => {
            if(r)
            {
                displayNameText.text = data.account.displayName;
                zoneText.text = data.account.zone;
            }
            else
            {
                Debug.Log("Get User Info Error"); 
            }
        });
    }

    public void GetUserInfo(Action<bool,MAccount> callback)
    {
        StartCoroutine(GetInfo((r,data) => {
            if(r)
            {
                callback?.Invoke(true,data);
            }
            else
            {
                callback?.Invoke(false,null);
            }
        }));
    }

    IEnumerator GetInfo(Action<bool,MAccount> callback)
    {
        Dictionary<string, string> headers = new Dictionary<string, string>();
        headers[iAPIHeader.X_SESSION_TOKEN] = iSession.Current.Token;

        yield return iAPI.GET<MAccount>("api/v1/account/info", headers, (result, req) => 
        {
            if (result == null) 
            { 
                Debug.Log("Result is null"); 
                callback?.Invoke(false,null);
                return; 
            }
            if (result.error != null) 
            { 
                Debug.Log(result.error.message); 
                callback?.Invoke(false,null);
                return; 
            }
            //Debug.Log(req.downloadHandler.text);            
            callback?.Invoke(true,result.data);
        });

        yield return null;
        yield break;
    }
}
