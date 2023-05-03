using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using Google;
using Firebase.Auth;
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
    public FirebaseUser GetUser => user;

    public firebaseManager()
    {
        PlayGamesPlatform.DebugLogEnabled = true;
        PlayGamesPlatform.Activate();
    }

    public Action<bool,string> StateUI;

    public void LogIn()
    {
        //StateUI?.Invoke(false, "로그인 중...");

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
                            StateUI?.Invoke(true, $"환영합니다 {newUser.UserId}님");
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

}
