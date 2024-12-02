[System.Serializable]
public class AuthUnsettleUser
{
    public string id;
    public string createdOn;
    public string updatedOn;
    public string username;
    public string hashedPassword;
    public string email;
    public string deactivedOn;
    public string role;

    public bool isActive;
    public bool verified;
}


[System.Serializable]
public class MAuthUnsettle
{
    public AuthUnsettleUser[] users;
}

[System.Serializable]
public class MUnsettlement
{
    public string data;
}

[System.Serializable]
public class MAdminSignup
{
    public string email;
    public string password;
    public string role;
    public string userName;
}
