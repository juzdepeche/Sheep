using System.Collections;
using System.Collections.Generic;

public delegate void ObserverCallback<T>(T value, string key);

public class ObserverData<T>
{
    public ObserverCallback<T> Callback;
    public string Key;

    public ObserverData(ObserverCallback<T> callback, string key)
    {
        Callback = callback;
        Key = key;
    }
}
