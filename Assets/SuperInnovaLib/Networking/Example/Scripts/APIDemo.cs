using System.Collections;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using SuperInnovaLib;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class APIDemo : MonoBehaviour
{
    // Start is called before the first frame update
    private string token = "GwGm-av63F-pr8mM";
    public Image image;


    void Start()
    {
        //iAPIConfig.Create("http://171.103.163.114", 2022, iAPIConfig.DEVELOPMENT);
        //iAPIConfig.CreateFromConfig();
        //iAPIConfig.SetDefault("doctor");

        //StartCoroutine(GetFileByURL("https://s3.ap-southeast-1.amazonaws.com/com.innova.doctor2.dev/62df98674444ed63d635af6a-qr.jpg"));
        //StartCoroutine(GetImageByID("62df98674444ed63d635af6b"));

        //StartCoroutine(GetResources());
        //StartCoroutine(PostLandEntityGenerate());
        //StartCoroutine(PostSignup());
        //StartCoroutine(GetAuthUnsettlement());
        
        //StartCoroutine(PutVerify());

        //iAPIDispatcher.Current().StartCoroutine(GetResources());
    }


    private IEnumerator GetImageByID(string id)
    {
        var header = new Dictionary<string, string>();
        header[iAPIHeader.X_SESSION_TOKEN] = "cL18frREh5vpNQZL";

        yield return iAPI.ImageByID(id, header, (sprite, texture, result) => 
        {
            image.sprite = sprite;
        });
    }


    private IEnumerator GetFileByURL(string url)
    {
        yield return iAPI.ImageByURL(url, (sp, tex) => 
        {
            image.sprite = sp;
        });
    }


    private IEnumerator UploadFile()
    {
        Dictionary<string, string> headers = new Dictionary<string, string>();
        headers[iAPIHeader.X_SESSION_TOKEN] = token;

        WWWForm form = new WWWForm();
        form.AddBinaryData("file", File.ReadAllBytes(@"C:\Users\USER\Desktop\FixSize.jpg"));
        form.AddField("fileName", "FixSize.jpg");

        yield return iAPI.POST<MFile>("api/v1/filestorage/file/upload", form, headers, (result, req) => 
        {
            if (result.error != null)
            {
                Debug.Log(result.error.message);
                return;
            }
            else
            {
                Debug.Log(result.data.source.url);
            }
        });
    }


    private IEnumerator PutVerify()
    {
        Dictionary<string, string> headers = new Dictionary<string, string>();
        headers[iAPIHeader.X_ADMIN_KEY] = "olJPOe6Cqok4_TMd";

        var jsonData = new {
            userID    = "62b33ff1b9cfd89daadccd89",
        };

        yield return iAPI.PUT<MNull>("api/v1/admin/verify", jsonData, headers, (result, req) => 
        {
            if (result == null) { Debug.Log("Result is null"); return; }
            if (result.error != null) { Debug.Log(result.error.message); return; }

            Debug.Log("Success");
        });
    }


    private IEnumerator PostSignup()
    {
        Dictionary<string, string> headers = new Dictionary<string, string>();
        headers[iAPIHeader.X_ADMIN_KEY] = "olJPOe6Cqok4_TMd";

        var jsonData = new {
            email    = "admin@innova.com",
            password = "CoolInnova2020",
            role     = "admin",
            username = "admin_temp"
        };

        yield return iAPI.POST<MNull>("api/v1/admin/signup", jsonData, headers, (result, req) => 
        {
            if (result == null) { Debug.Log("Result is null"); return; }
            if (result.error != null) { Debug.Log(result.error.message); return; }

            Debug.Log("Success");
        });
    }


    private IEnumerator GetAuthUnsettlement()
    {
        Dictionary<string, string> headers = new Dictionary<string, string>();
        headers[iAPIHeader.X_ADMIN_KEY] = "olJPOe6Cqok4_TMd";

        yield return iAPI.GET<MAuthUnsettle>("api/v1/admin/list/unsettlement", headers, (result, req) => 
        {
            if (result == null) { Debug.Log("Result is null"); return; }
            if (result.error != null) { Debug.Log(result.error.message); return; }

            foreach(var l in result.data.users)
            {
                Debug.Log(l.id);
            }

        });
        yield break;
    }


    // private IEnumerator PostLandEntityGenerate()
    // {
    //     Dictionary<string, string> headers = new Dictionary<string, string>();
    //     headers[iAPIHeader.X_SESSION_TOKEN] = "695sAp-0F6YaJN4V";

    //     var jsonObject = new {
    //         count = 10
    //     };

    //     yield return iAPI.POST<MLandEntityModel>("api/v1/land/entity/generate/id", jsonObject, headers, (result, req) => 
    //     {
    //         Debug.Log(req.downloadHandler.text);
    //     });
    // }


    // private IEnumerator GetResourcesByID()
    // {
    //     Dictionary<string, string> headers = new Dictionary<string, string>();
    //     headers[iAPIHeader.X_SESSION_TOKEN] = "695sAp-0F6YaJN4V";

    //     string resourcesID = "62a8b377251dd365bfea325a";

    //     yield return iAPI.GET<MResourcesEntity>("api/v1/resource/entity/{0}/get".Format(resourcesID), headers, (result, req) => 
    //     {
    //         Debug.Log(req.downloadHandler.text);

    //         if (result.error != null)
    //         {
    //             Debug.Log(result.error.message);
    //             return;
    //         }
    //     });
    // }


    // private IEnumerator GetResources()
    // {
    //     var headers = new Dictionary<string, string>();
    //     headers[iAPIHeader.X_SESSION_TOKEN] = "695sAp-0F6YaJN4V";

    //     yield return iAPI.GET<MResourcesEntities>("/api/v1/resource/entity/list".URLQuery("offset=0", "limit=100"), headers, (result, req) => 
    //     {
    //         Debug.Log(req.downloadHandler.text);

    //         if (result == null) 
    //         {
    //             Debug.Log("Can't connect to server");
    //             return;
    //         }

    //         if (result.error != null)
    //         {
    //             Debug.Log(result.error.message);
    //             return;
    //         }

    //         foreach(var kv in result.data.entities)
    //         {
    //             Debug.Log($"{kv.id} : {kv.descriptions[0].name} -> {kv.colorHex}");
    //         }

    //         iAPIDispatcher.DisposeAll();
    //     });
    // }
}
