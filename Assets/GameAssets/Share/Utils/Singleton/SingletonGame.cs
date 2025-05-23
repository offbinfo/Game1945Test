﻿#region

using UnityEngine;

#endregion

/// <summary>
///     Be aware this will not prevent a non singleton constructor
///     such as `T myT = new T();`
///     To prevent that, add `protected T () {}` to your singleton class.
///     As a note, this is made as MonoBehaviour because we need Coroutines.
/// </summary>
public class SingletonGame<T> : GameMonoBehaviour where T : MonoBehaviour
{
    private static T i;
    public static T instance
    {
        get
        {
            if (!i)
                i = FindObjectOfType<T>(true);
            return i;
        }
    }

    protected virtual void Awake()
    {
        if (i != null && i != this)
        {
            Destroy(gameObject);
            return;
        }

        i = this as T;
        DontDestroyOnLoad(gameObject);
    }

    /*private static T _instance;

    private static readonly object _lock = new object();

    protected static bool applicationIsQuitting;

    public static T instance
    {
        get
        {
            if (applicationIsQuitting)
            {
                Debug.LogWarning("[Singleton] Instance '" + typeof(T) +
                                 "' already destroyed on application quit." +
                                 " Won't create again - returning null.");
                return null;
            }

            lock (_lock)
            {
                if (_instance == null)
                {
                    _instance = (T) FindObjectOfType(typeof(T));
                    if (_instance != null)
                        DontDestroyOnLoad(_instance.gameObject);

                    if (FindObjectsOfType(typeof(T)).Length > 1)
                    {
                        Debug.LogError("[Singleton] Something went really wrong " +
                                       " - there should never be more than 1 singleton!" +
                                       " Reopening the scene might fix it.");
                        return _instance;
                    }

                    if (_instance == null && !applicationIsQuitting)
                    {
                        var singleton = new GameObject();
                        _instance = singleton.AddComponent<T>();
                        singleton.name = "(singleton) " + typeof(T);

                        DontDestroyOnLoad(singleton);

                        // Debug.Log("[Singleton] An instance of " + typeof(T) +
                        //     " is needed in the scene, so '" + singleton +
                        //     "' was created with DontDestroyOnLoad.");
                    }
                }

                return _instance;
            }
        }
    }*/

    /*protected virtual void Awake()
    {
        _instance = GetComponent<T>();
        if (_instance != null)
            DontDestroyOnLoad(gameObject);
    }

    /// <summary>
    ///     When Unity quits, it destroys objects in a random order.
    ///     In principle, a Singleton is only destroyed when application quits.
    ///     If any script calls Instance after it have been destroyed,
    ///     it will create a buggy ghost object that will stay on the Editor scene
    ///     even after stopping playing the Application. Really bad!
    ///     So, this was made to be sure we're not creating that buggy ghost object.
    /// </summary>
    public virtual void OnApplicationQuit()
    {
        applicationIsQuitting = true;
    }*/
}