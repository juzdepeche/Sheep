using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Observer;

public abstract class Observable : MonoBehaviour
{
    static readonly Observable instance;
    readonly List<Observer> observers = new List<Observer>();
    private Dictionary<string, object> properties = new Dictionary<string, object>();

    public object GetValue(string key) {
		return properties[key];
	}

    public void SetValue(string key, object value, bool allowEqualValues = false) {
        //add allowEqualValues condition
        if (properties.ContainsKey(key))
        {
            properties[key] = value;
        }
        else
        {
            properties.Add(key, value);
        }

		CallObserversFrom(key, value);
    }

    public Action AddObserver(ObserverCallback callback, string key, object value) {
		return AddObserverTo(callback, key, value);
	}

    public void RemoveObservers(ObserverCallback callback, string key, object value) {
    	RemoveObserversFrom(callback, key, value);
    }
    
    Action AddObserverTo(ObserverCallback callback, string key, object value) {
		if (callback != null) {
			observers.Add(new Observer(callback, key, value));
		}

        return () => { RemoveObserversFrom(callback, key, value); };
    }

    void CallObserversFrom(string key, object value) {
		for (var i = observers.Count - 1; i >= 0; i--) {
			var observer = observers[i];
            if (observer.Key != null && observer.Key != key)
                continue;
            observer.Callback(value, key);
		}
	}

    void RemoveObserversFrom(ObserverCallback callback, string key, object value) {
        for (var i = observers.Count - 1; i >= 0; i--)
        {
            var observer = observers[i];
            if (callback != observer.Callback)
            {
                continue;
            }
            else if (key != null && key != observer.Key)
            {
                continue;
            }
            else if (value != null && value != observer.Value)
            {
                continue;
            }
            observers.RemoveAt(i);
        }
    }
}
