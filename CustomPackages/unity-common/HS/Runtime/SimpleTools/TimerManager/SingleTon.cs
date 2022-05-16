using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    public static T Instance
    {
        get;
        private set;
    }

    /// <summary>
    /// Checks if the instance exists and creates a new one when there is none.
    /// </summary>
    protected virtual void Awake()
    {
        if (Instance)
        {
            Destroy(this);
            return;
        }
        Instance = GetComponent<T>();
        
        AfterAwake();
    }

    protected virtual void AfterAwake()
    {

    }
}