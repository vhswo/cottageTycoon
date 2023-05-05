using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using System;

/// <summary>
/// 안드로이드 앱을 제작할때 구글플레이 초기화 해주는 함수
/// </summary>
public static class FirebaseInitialize
{
    public static List<Action<DependencyStatus>> initailizeCallbacks = new();
    static DependencyStatus dependencyStatus;

    static bool initialized = false;
    static bool fetching = false;
    static bool activateFetch = false;

    public static void Initialize(Action<DependencyStatus> callback)
    {
        lock(initailizeCallbacks)
        {
            if(initialized)
            {
                callback(dependencyStatus);
                return;
            }

            initailizeCallbacks.Add(callback);
            FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
            {
                lock(initailizeCallbacks)
                {
                    dependencyStatus = task.Result;
                    initialized = true;
                    CallInitializedCallbacks();
                }
            });
        }
    }

    private static void CallInitializedCallbacks()
    {
        lock(initailizeCallbacks)
        {
            foreach(var callback in initailizeCallbacks)
            {
                callback(dependencyStatus);
            }

            initailizeCallbacks.Clear();
        }
    }
}
