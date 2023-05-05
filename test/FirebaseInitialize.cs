using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using System;

/// <summary>
/// �ȵ���̵� ���� �����Ҷ� �����÷��� �ʱ�ȭ ���ִ� �Լ�
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
