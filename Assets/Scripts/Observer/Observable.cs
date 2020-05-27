using System;
using System.Collections;
using System.Collections.Generic;

public abstract class Observable<T>
{
    static readonly Observable<T> instance;
    readonly List<ObserverData<T>> observers = new List<ObserverData<T>>();
    private Dictionary<string, T> properties = new Dictionary<string, T>();

    public T GetValue(string key) {
        if (!properties.ContainsKey(key)) return default(T);
		return properties[key];
	}

    public void SetValue(string key, T value, bool callCallbacks = true) {
        if (properties.ContainsKey(key))
        {
            properties[key] = value;
        }
        else
        {
            properties.Add(key, value);
        }

        if (callCallbacks) 
        {
		    CallObserversFrom(key, value);
        }
    }

    public void AddObserver(ObserverCallback<T> callback, string key) {
		AddObserverTo(callback, key);
	}

    public void AddObserver(ObserverCallback<T> callback, string key, T value, bool callCallbacks = false) {
		AddObserverTo(callback, key);
        SetValue(key, value, false);
	}

    private void AddObserverTo(ObserverCallback<T> callback, string key) {
		observers.Add(new ObserverData<T>(callback, key));
    }

    public void RemoveObservers(ObserverCallback<T> callback, string key) {
    	RemoveObserversFrom(callback, key);
    }

    public void RemoveObservers(ObserverCallback<T> callback, string key, T value) {
    	RemoveObserversFrom(callback, key);
        SetValue(key, value);
    }

    private void RemoveObserversFrom(ObserverCallback<T> callback, string key) {
        foreach (ObserverData<T> observer in observers) 
        {
            if (callback != observer.Callback && key != observer.Key) observers.Remove(observer);
        }
    }

    private void CallObserversFrom(string key, T value) {
		foreach (ObserverData<T> observer in observers) 
        {
            if (observer.Key == key) observer.Callback(value, key);
		}
	}
}
