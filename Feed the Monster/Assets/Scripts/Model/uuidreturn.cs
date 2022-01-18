using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[System.Serializable]
public class Params
{
    public string id;
    public string organization;
}
[System.Serializable]
public class UuidReturn
{
    public string uuid;
    public Params @params;
}

