using UnityEngine;

/// <summary>
/// Generic based singleton for MonoBehaviours.<br>
/// </summary>
/// <typeparam name="T">is interface of singleton</typeparam>
public class Singleton<T> : MonoBehaviour where T : class
{
    /// <summary>
    /// Makes the object singleton not be destroyed automatically when loading a new scene.
    /// </summary>
    public bool DontDestroy = false;

    static T instance;
    public static T Instance
    {
        get { return instance; }
    }

    protected virtual void Awake()
    {
        if (instance != null)
            Destroy(gameObject);
        else
        {
            instance = this as T;
            if (DontDestroy)
                DontDestroyOnLoad(gameObject);
            Initialize();
        }
    }

    public static void Mock(T mock)
    {
        instance = mock;
    }

    protected virtual void Initialize()
    {
    }

    protected virtual void Shutdown()
    {
    }

    protected virtual void OnDestroy()
    {
        if ((instance as Singleton<T>) == this)
        {
            instance = null;
            Shutdown();
        }
    }

    protected virtual void OnApplicationQuit()
    {
        if ((instance as Singleton<T>) == this)
        {
            instance = null;
            Shutdown();
        }
    }
}