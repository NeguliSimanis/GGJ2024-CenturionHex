using System;
using UnityEngine;
public enum datatype {
    Flag,
    String,
    Int,
    Double
}
[Serializable]
public class KeyTypeValueTuple
{
    public string key;
    public datatype type;
    public string value;
}
