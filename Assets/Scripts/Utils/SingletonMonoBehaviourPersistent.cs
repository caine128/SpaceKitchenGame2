using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingletonMonoBehaviourPersistent<T_Type> : MonoBehaviour
    where T_Type:MonoBehaviour
{
    private static T_Type _instance;
    public static T_Type Instance { get { return _instance; } }
    private static readonly object _lock = new object();

    protected virtual void Awake()
    {
        if (_instance != null && _instance != this as T_Type)
        {
            Destroy(this.gameObject);
        }
        else
        {
            lock (_lock)
            {
                if (_instance == null)
                {
                    _instance = this as T_Type;
                    DontDestroyOnLoad(this.gameObject);
                }
            }
        }
    }
}
