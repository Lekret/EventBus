using System;
using System.Collections.Generic;
using UnityEngine;

namespace LekretUtility.EventBus
{
    public class EventBus
    {
        private readonly Dictionary<Type, HashSet<object>> _listenersMap = new Dictionary<Type, HashSet<object>>();

        public void Pub<T>(Action<T> action) where T : class
        {
            if (TryGetListeners(typeof(T), out var listeners) && listeners.Count > 0)
            {
                NotifyListeners(listeners, action);
            }
        }

        public void Sub<T>(T listener) where T : class
        {
            if (!TryGetListeners(typeof(T), out var listeners))
            {
                listeners = new HashSet<object>();
                _listenersMap.Add(typeof(T), listeners);
            }
            listeners.Add(listener);
        }

        public void Unsub<T>(T listener) where T : class
        {
            if (TryGetListeners(typeof(T), out var listeners))
            {
                listeners.Remove(listener);
            }
        }

        public void UnsubAll<T>(T listener) where T : class
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
}