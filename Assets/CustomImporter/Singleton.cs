using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singleton<T> where T : Singleton<T>, new()
{
    private static T _mInstance;

    public static T Instance
    {
        get
        {
            if (_mInstance == null)
            {
                _mInstance = new T();
                _mInstance.Init();
            }
            return _mInstance;
        }
    }

    protected Singleton()
    {
    }

    protected virtual void Init()
    {
    }
}
