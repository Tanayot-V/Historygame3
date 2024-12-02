using Unity.VisualScripting;
using UnityEngine;

[System.Serializable]
public class MAccount : MBase
{
    public Account account;
}

[System.Serializable]
public class Account : MBase
{
    public string displayName;
    public string zone;
}
