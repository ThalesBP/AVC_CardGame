using UnityEngine;

/// <summary>
/// Singleton works as a static class.
/// </summary>
public class Singleton<T> : MonoBehaviour where T : Singleton<T>
{
    public static T Instance;

    private bool m_IsPersistant = true;

    public bool IsPersistant
    {
        get { return m_IsPersistant; }
        set { m_IsPersistant = value; }
    }

    public virtual void Awake()
    {
        if (IsPersistant)
        {
            if (!Instance)
                Instance = this as T;
            else
                DestroyObject(gameObject);
//            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Instance = this as T;
        }
    }
}