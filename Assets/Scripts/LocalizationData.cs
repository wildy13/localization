using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocalizationData
{
    public List<LocalalizationItems> items;
}

[System.Serializable]
public class LocalalizationItems
{
    public string key;
    public string value;
}