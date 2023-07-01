using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singleton<T> where T : Singleton<T>, new()
{
    private static T m_instance;

    public static T Instance
    {
        get
        {
            if (m_instance == null)
            {
                m_instance = new T();
                m_instance.Init();
            }
            return m_instance;
        }
    }

    protected Singleton()
    {
    }

    protected virtual void Init()
    {
    }
}
