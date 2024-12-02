using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace SuperInnovaLib
{
    public class iSession
    {
        private static iSession currentSession;
        public static iSession Current 
        {
            get 
            {
                if (PlayerPrefs.HasKey(iAPIHeader.X_SESSION_TOKEN))
                {
                    currentSession = new iSession(PlayerPrefs.GetString(iAPIHeader.X_SESSION_TOKEN));
                }
                return currentSession;
            }
        }

        private string token;
        public string Token
        {
            get 
            {
                return token;
            }
        }


        private iSession(string token)
        {
            this.token = token;
        }


        public static void Save(string token)
        {
            currentSession = new iSession(token);
            PlayerPrefs.SetString(iAPIHeader.X_SESSION_TOKEN, token);
            PlayerPrefs.Save();
        }


        public static void Clear()
        {
            currentSession = null;
            PlayerPrefs.DeleteKey(iAPIHeader.X_SESSION_TOKEN);
        }


        public bool IsEqual(iSession otherSession)
        {
            return token == otherSession.Token;
        }


        public IEnumerator ValidateSession(Action<iWResponse<MValidateSession>, UnityWebRequest> callback, iAPIOption option = null)
        {
            Dictionary<string,  string> headers = new Dictionary<string, string>();
            headers[iAPIHeader.X_SESSION_TOKEN] = token;

            yield return iAPI.POST<MValidateSession>("/me/validate", null, headers, callback, option);
        }
    }


    [System.Serializable]
    public class MValidateSession 
    {
        public bool valid;
        public string reason;
    }
}
