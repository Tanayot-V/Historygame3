using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MFileSource
{
    public string bucketName;
    public string key;
    public string url;
}


[System.Serializable]
public class MFileData : MBase
{
    public string fileName;
    public string meta;
    public MFileSource source;
    public MFileSource thumbnailSource;
    public string userID;
}

[System.Serializable]
public class MFile
{
    public string id;
    public string fileName;
    public string meta;
    public MFileSource source;
    public MFileSource thumbnailSource;
    public string userID;
}

[System.Serializable]
public class MFiles
{
    public MFileData[] entities;
}