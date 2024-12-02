using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;
using System.Text;
using System.Text.RegularExpressions;

namespace SuperInnovaLib
{
    public class iAPIOption
    {
        public int retryLimit = 3;
        public bool dontConvertToSprite = false;
        public object attachment = null;
    }


    public class iAPI
    {
        public static IEnumerator GET<T>(string apiPath, Dictionary<string, string> headers, Action<iWResponse<T>, UnityWebRequest> callback = null, iAPIOption option = null)
        {
            yield return ProcessRequest<T>(apiPath, headers, option == null ? new iAPIOption() : option, callback, (url) => UnityWebRequest.Get(url));
        }


        public static IEnumerator POST<T>(string apiPath, object jsonData, Dictionary<string, string> headers, Action<iWResponse<T>, UnityWebRequest> callback = null, iAPIOption option = null)
        {
            yield return ProcessRequest<T>(apiPath, headers, option == null ? new iAPIOption() : option, callback, (url) => {
                string json = JsonConvert.SerializeObject(jsonData);
                UnityWebRequest uwr = UnityWebRequest.PostWwwForm(url, json);
                uwr.uploadHandler   = (UploadHandler)   new UploadHandlerRaw(Encoding.UTF8.GetBytes(json));
                uwr.downloadHandler = (DownloadHandler) new DownloadHandlerBuffer();
                uwr.SetRequestHeader("Content-Type", "application/json");
                return uwr;
            });
        }


        public static IEnumerator POST<T>(string apiPath, WWWForm form, Dictionary<string, string> headers, Action<iWResponse<T>, UnityWebRequest> callback = null, iAPIOption option = null)
        {
            yield return ProcessRequest<T>(apiPath, headers, option == null ? new iAPIOption() : option, callback, (url) => {
                UnityWebRequest uwr = UnityWebRequest.Post(url, form);
                uwr.downloadHandler = (DownloadHandler) new DownloadHandlerBuffer();
                return uwr;
            });
        }


        public static IEnumerator DELETE<T>(string apiPath, Dictionary<string, string> headers, Action<iWResponse<T>, UnityWebRequest> callback = null, iAPIOption option = null)
        {
            yield return ProcessRequest<T>(apiPath, headers, option == null ? new iAPIOption() : option, callback, (url) => {
                UnityWebRequest uwr = UnityWebRequest.Delete(url);
                uwr.downloadHandler = (DownloadHandler) new DownloadHandlerBuffer();
                return uwr;
            });
        }


        public static IEnumerator PUT<T>(string apiPath, object jsonData, Dictionary<string, string> headers, Action<iWResponse<T>, UnityWebRequest> callback = null, iAPIOption option = null)
        {
            yield return ProcessRequest<T>(apiPath, headers, option == null ? new iAPIOption() : option, callback, (url) => {
                string json = JsonConvert.SerializeObject(jsonData);
                UnityWebRequest uwr = UnityWebRequest.Put(url, json);
                uwr.uploadHandler   = (UploadHandler)   new UploadHandlerRaw(Encoding.UTF8.GetBytes(json));
                uwr.downloadHandler = (DownloadHandler) new DownloadHandlerBuffer();
                uwr.SetRequestHeader("Content-Type", "application/json");
                return uwr;
            });
        }


        public static IEnumerator GetXFile(string id, Dictionary<string, string> header, Action<iWResponse<MFile>, UnityWebRequest> callback, iAPIOption option = default(iAPIOption))
        {
            yield return GET<MFile>("filestorage/file".URLQuery($"xfileID={id}"), header, callback, option);
        }


        public static IEnumerator ImageByID(string id, Dictionary<string, string> header, Action<Sprite, Texture2D, iWResponse<MFile>> callback, iAPIOption option = default(iAPIOption))
        {
            iWResponse<MFile> container = null;
            bool isError = false;

            yield return GetXFile(id, header, (result, req) => 
            {
                //Debug.Log(result.success);
                if(result.error != null)
                {
                    Debug.Log(result.error.message + " : " + result.error.internalErrorMessage);
                }
                container = result;
                if (!result.success)
                {
                    isError = true;
                    Debug.LogWarning("[Error.ImageByID]: Not success");
                    callback(null, null, result);
                    return;
                }

                if (result.error != null)
                {
                    isError = true;
                    Debug.LogWarning("[Error.ImageByID]: " + result.error.message);
                    callback(null, null, result);
                    return;
                }

            }, option);

            if(!isError)
            {
                yield return ImageByURL(container.data.source.url, (sp, tex) => 
                {
                    callback(sp, tex, container);
                }, option);
            }
        }


        public static IEnumerator ThumbnailByID(string id, Dictionary<string, string> header, Action<Sprite, Texture2D, iWResponse<MFile>> callback, iAPIOption option = null)
        {
            var container = new iWResponse<MFile>();

            yield return GetXFile(id, header, (result, req) => 
            {
                container = result;

                if (!result.success)
                {
                    if (result.error != null)
                    {
                        Debug.LogWarning("[Error.ThumbnailByID]: " + result.error.message);
                        callback(null, null, result);
                    }
                    return;
                }

            }, option);

            if (container.data.thumbnailSource == null)
            {
                Debug.LogWarning("[Error.ThumbnailByID]: Thumbnail is null");
                yield break;
            }


            yield return ImageByURL(container.data.thumbnailSource.url, (sp, tex) => 
            {
                callback(sp, tex, container);
            }, option);
        }


        public static IEnumerator ImageByURL(string url, Action<Sprite, Texture2D> callback, iAPIOption option = null)
        {
            if (option == null) option = new iAPIOption();

            UnityWebRequest www = UnityWebRequestTexture.GetTexture(url);
            www.downloadHandler = new DownloadHandlerTexture();
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogWarning("[Error.ImageByURL]: " + www.error);
                callback(null, null);
                yield break;
            }
            else
            {
                Texture2D texture = ((DownloadHandlerTexture)www.downloadHandler).texture;
                Sprite sprite = null;
                if (!option.dontConvertToSprite)
                {
                    sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
                }

                callback(sprite, texture);
            }
        }


        private static IEnumerator ProcessRequest<T>(string apiPath, Dictionary<string, string> headers, iAPIOption option, Action<iWResponse<T>, UnityWebRequest> callback, Func<string, UnityWebRequest> requester)
        {
            // Clear slash
            while(apiPath.StartsWith("/"))
            {
                apiPath = apiPath.Remove(0, 1);
            }

            apiPath = Regex.Replace(apiPath, @"api/v\d+/(.*)", "$1");

            iAPIInfo config = iAPIConfig.Default;
            if (config == null)
            {
                Debug.LogWarning("[Error.Config]: No api config found.");
                yield break;
            }

            string url = string.Format("{0}/api/v{1}/{2}", config.endPoint, config.version, apiPath);
            if (apiPath.StartsWith("http"))
            {
                url = apiPath;
            }
            
            //Debug.Log("[Info.Endpoint]: " + url);

            if (headers == null)
            {
                headers = new Dictionary<string, string>();
            }

            if (!headers.ContainsKey(iAPIHeader.X_SESSION_TOKEN))
            {
                if (iSession.Current != null)
                {
                    headers[iAPIHeader.X_SESSION_TOKEN] = iSession.Current.Token;
                }
            }

            // Process with retry
            int retry = 0;
            int retryLimit = option.retryLimit;

            DateTime retryOn = DateTime.Now;

            UnityWebRequest req;
            while(true) 
            {
                req = requester(url);
                foreach(var kv in headers)
                {
                    req.SetRequestHeader(kv.Key, kv.Value);
                }

                req.certificateHandler = null;
                
                yield return req.SendWebRequest();
                if (req.result == UnityWebRequest.Result.ConnectionError)
                {
                    retry++;
                    if (retry <= retryLimit)
                    {
                        Debug.LogWarning($"[Error.Request]: {req.url} method: {req.method} due to error: {req.error}... retry {retry} / {retryLimit}");
                    }
                    else
                    {
                        Debug.LogWarning($"[Error.Request]: {req.url} method: {req.method} due to error: {req.error}... after retried for {retryLimit} times");
                        break;
                    }

                    var diff = DateTime.Now.Subtract(retryOn);
                    if (diff.TotalSeconds < 1)
                    {
                        yield return new WaitForSeconds(1f - (float)diff.TotalSeconds);
                    }
                    continue;
                }
                break;
            }

            //Debug.Log("response J-Son : " + req.result);
            //Debug.Log("response J-Son : " + req.downloadHandler.text);
            iWResponse<T> result = JsonConvert.DeserializeObject<iWResponse<T>>(req.downloadHandler.text);
            result.attachment = option.attachment;
            callback?.Invoke(result, req);
        }
    }
}