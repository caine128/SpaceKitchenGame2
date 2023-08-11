using System;
using System.Reflection;
using UnityEngine;

public abstract class Singleton<T_Type>
    where T_Type : class
{
    private static readonly T_Type _instance;
    public static T_Type Instance { get => _instance; }
    //private static readonly object _lock = new object();

    static Singleton()
    {
        Type t = typeof(T_Type);
        ConstructorInfo[] ctorsPublic = t.GetConstructors(BindingFlags.Instance | BindingFlags.Public);
        if (ctorsPublic.Length > 0)
            throw new Exception(t.FullName + " has one or more public constructors so the property cannot be enforced.");

        ConstructorInfo ctorNonPublic = t.GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic, null, new Type[0], new ParameterModifier[0]);
        if(ctorNonPublic == null)
            throw new Exception(
                    t.FullName + " doesn't have a private/protected constructor so the property cannot be enforced.");

        try
        {
            _instance = (T_Type)ctorNonPublic.Invoke(new object[0]);
        }
        catch (Exception e)
        {
            throw new Exception(
                    "The Singleton couldnt be constructed, check if " + t.FullName + " has a default constructor", e);
        }

        //_instance = new T_Type();
    }

    /*
    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }

        else
        {
            lock (_lock)
            {
                if (_instance == null)
                {
                    _instance = this;
                    DontDestroyOnLoad(this.gameObject);
                }
            }
        }

        Config();
    }*/
}



