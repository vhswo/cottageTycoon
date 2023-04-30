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

public class firebaseController : MonoBehaviour
{
    [SerializeField] GameObject BackGroundUI;
    [SerializeField] Text LogInState;
    [Header("LogIn")]
    [SerializeField] GameObject LogInUI;
    //[SerializeField] Button LogInBtn;
    [Header("LogOut")]
    [SerializeField] GameObject LogOutUI;
    //[SerializeField] Button LogOutBtn;
    //[SerializeField] Button StartBtn;

    FirebaseAuth auth;
    FirebaseUser user;
    void Start()
    {
        PlayGamesPlatform.DebugLogEnabled = true;
        PlayGamesPlatform.Activate();
        LogIn();

    }

    public void LogIn()
    {
        LogInState.text = "�α��� ��...";
        LogInUI.SetActive(false);
        PlayGamesPlatform.Instance.Authenticate((status) =>
        {
            if (status == SignInStatus.Success)
            {
                PlayGamesPlatform.Instance.RequestServerSideAccess(false, code =>
                {
                    auth = FirebaseAuth.DefaultInstance;
                    Credential credential = PlayGamesAuthProvider.GetCredential(code);

                    auth.SignInWithCredentialAsync(credential).ContinueWith(task =>
                    {
                        if (task.IsCanceled)
                        {
                            LogInState.text = "Auth cancelled";
                            return;
                        }
                        else if (task.IsFaulted)
                        {
                            int errorCode = GetFirebaseErrorCode(task.Exception);//AuthError
                               LogInState.text = errorCode.ToString();
                            //LogInState.text += $"id : {Social.localUser.userName}";
                            LogInState.text += " Faulted : " + task.Exception;
                            return;
                        }
                        else
                        {
                            FirebaseUser newUser = task.Result;
                            user = auth.CurrentUser;
                            LogOutUI.SetActive(true);
                            LogInState.text = "�ݰ����ϴ� " + newUser + "��";
                        }
                    });
                });
            }
            else
            {
                LogInState.text += "PGP Authernticate falled";
            }
        });

    }

    public void LogOut()
    {
        if (user == auth.CurrentUser && auth.CurrentUser != null)
        {
            auth.SignOut();
            LogInState.text = "�α���";

            LogOutUI.SetActive(false);
            LogInUI.SetActive(true);
        }
    }

    public void GameStart()
    {
        BackGroundUI.SetActive(false);
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
