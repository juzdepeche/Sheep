using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class IObserverValue<T> : MonoBehaviour
{
    public string Key;
    public T Value;
    public abstract void CallCallback(T value, string key);
}
