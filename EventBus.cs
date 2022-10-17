using System;
using System.Collections.Generic;
using UnityEngine;

public class EventBus
{
    private readonly Dictionary<Type, HashSet<object>> _listenersMap = new Dictionary<Type, HashSet<object>>();

    public bool Notify<T>(Action<T> action) where T : class
    {
        if (TryGetListeners(typeof(T), out var listeners) && listeners.Count > 0)
        {
            NotifyListeners(listeners, action);
            return true;
        }
        return false;
    }

    public void Register<T>(T listener) where T : class
    {
        var type = typeof(T);
        if (!TryGetListeners(type, out var listeners))
        {
            listeners = new HashSet<object>();
            _listenersMap.Add(type, listeners);
        }
        listeners.Add(listener);
    }

    public void Unregister<T>(T listener) where T : class
    {
        if (TryGetListeners(typeof(T), out var listeners))
        {
            listeners.Remove(listener);
        }
    }

    public void UnregisterAll<T>(T listener) where T : class
    {
        foreach (var listeners in _listenersMap.Values)
        {
            listeners.Remove(listener);
        }
    }

    public void Clear()
    {
        _listenersMap.Clear();
    }

    private bool TryGetListeners(Type type, out HashSet<object> listeners)
    {
        return _listenersMap.TryGetValue(type, out listeners);
    }

    private static void NotifyListeners<T>(HashSet<object> listeners, Action<T> action)
    {
        foreach (T listener in listeners)
        {
            try
            {
                action.Invoke(listener);
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
        }
    }
}