using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SuperInnovaLib
{
    [System.Serializable]
    public class iConfigAPI
    {
        public iAPIInfo[] infos;
    }


    [System.Serializable]
    public class iConfig
    {
        public iConfigAPI api;
    }
}
