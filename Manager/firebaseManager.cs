using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using Google;
using Firebase.Auth;
using Firebase.Database;
using Firebase;

public enum firebase
{
    LogIn,
    LogOut,

}

public class firebaseManager
{
    FirebaseAuth auth;
    FirebaseUser user;
    public string GetUserId => user.UserId;

    DatabaseReference dataRef;

    public firebaseManager()
    {
        PlayGamesPlatform.DebugLogEnabled = true;
        PlayGamesPlatform.Activate();
        dataRef = FirebaseDatabase.DefaultInstance.RootReference;
    }  

    public Action<bool,string> StateUI;
    public Action<bool> SuccessLogin;

    public void LogIn()
    {
        StateUI?.Invoke(false, "로그인 중...");

        PlayGamesPlatform.Instance.Authenticate((status) =>
        {
            if (status == SignInStatus.Success)
            {
                PlayGamesPlatform.Instance.RequestServerSideAccess(true, code =>
                {
                    auth = FirebaseAuth.DefaultInstance;
                    Credential credential = PlayGamesAuthProvider.GetCredential(code);

                    auth.SignInWithCredentialAsync(credential).ContinueWith(task =>
                    {
                        if (task.IsCanceled)
                        {
                            StateUI?.Invoke(false, "Auth cancelled");
                            return;
                        }
                        else if (task.IsFaulted)
                        {
                            int errorCode = GetFirebaseErrorCode(task.Exception);//AuthError
                            StateUI?.Invoke(false, $"errorCode : {errorCode} : {task.Exception}");
                            return;
                        }
                        else
                        {
                            FirebaseUser newUser = task.Result;
                            user = auth.CurrentUser;
                            StateUI?.Invoke(true, $"환영합니다 {user.UserId}님");
                        }
                    });
                });
            }
            else
            {
                StateUI?.Invoke(false, "로그인 정보가 없습니다");
            }
        });

    }

    public void LogOut()
    {
        if (user == auth.CurrentUser && auth.CurrentUser != null)
        {
            auth.SignOut();
            user = null;
            StateUI?.Invoke(true,"로그인을 해주세요");
        }
    }

    private int GetFirebaseErrorCode(AggregateException exception)
    {
        FirebaseException firebaseException = null;
        foreach(Exception e in exception.Flatten().InnerExceptions)
        {
            firebaseException = e as FirebaseException;
            if(firebaseException != null)
            {
                break;
            }
        }

        return firebaseException?.ErrorCode ?? 0;
    }

    public void CallbackFirebaseData()
    {
        string userId = user.UserId;

        dataRef.Child("users").Child(userId).Child("status").GetValueAsync().ContinueWith(task =>
        {
            gameManger.Instance.testText.text = "InLoadFuncSuceece but Failed";
            if (task.Exception != null)
            {
                gameManger.Instance.testText.text = "LoadException";
                return;
            }

            if (!task.IsCompleted)
            {
                gameManger.Instance.testText.text = "Load not Completed";
                return;
            }

            DataSnapshot snapshot = task.Result;

            string status = snapshot.GetRawJsonValue();

            PlayerStatus playerStatus = JsonUtility.FromJson<PlayerStatus>(status);

            if (playerStatus.id == string.Empty)
            {
                playerStatus = new(playerStatus.id);
            }

            gameManger.Instance.SetPlayer(playerStatus);
            SuccessLogin.Invoke(false);
        });
        
    }

}
