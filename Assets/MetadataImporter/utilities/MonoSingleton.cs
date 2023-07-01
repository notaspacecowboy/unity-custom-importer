using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonoSingleton<T> : MonoBehaviour where T : MonoSingleton<T>
{
    private static T m_instance;

    public static T Instance
    {
        get
        {
            if (m_instance == null)
            {
                //find the object in the scene
                var instances = FindObjectsOfType<T>();
                if (instances.Length == 0)
                {
                    Debug.LogError($"instance of {typeof(T).Name} does not exist!");
                    return null;
                }
                else if (instances.Length > 1)
                {
                    Debug.LogError($"instance of {typeof(T).Name} is more than one!");
                    return null;
                }

                m_instance = instances[0];
            }
            return m_instance;
        }
    }

    protected virtual void Init()
    {
    }
}
                        
