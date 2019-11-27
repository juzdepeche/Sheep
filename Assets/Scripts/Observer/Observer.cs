using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Observer : ScriptableObject
{
    public string Key;
    public bool HasExpectedValue = false;
    public delegate void ObserverCallback(object value, string key);
    public object Value;
    public ObserverCallback Callback;

    public Observer(ObserverCallback callback, string key, object value)
    {
        Callback = callback;
        Key = key;

        //if(value)
        // I assume that value is set
        Value = value;
        HasExpectedValue = true;
    }
}
