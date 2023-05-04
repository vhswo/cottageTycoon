using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : Component
{
    static T instance;
    static public T Instance => instance;

    private void Awake()
    {
        if(instance == null)
        {
            instance = (T)FindObjectOfType(typeof(T));
        }
        else
        {
            if (instance != this) Destroy(this.gameObject);
        }
    }
}
