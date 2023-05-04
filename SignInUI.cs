using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum SignState
{
    SignOut,
    SignIn,
}

public class SignInUI : MonoBehaviour
{
    [SerializeField] GameObject BackGroundUI;
    [SerializeField] Text LogInState;

    [Header("LogIn")]
    [SerializeField] GameObject SignInObject;

    [Header("LogOut")]
    [SerializeField] GameObject SignOutObject;

    private void Start()
    {
        gameManger.Instance.Firebase.StateUI += UpdateStateText;
        SignIn();
    }

    public void SignIn()
    {
        if (LogInState.text == "로그인 중...") return;

        gameManger.Instance.UseFirebase(firebase.LogIn);
    }

    public void SignOut()
    {
        gameManger.Instance.UseFirebase(firebase.LogOut);
    }

    public void UpdateStateText(bool success, string text)
    {
        if(success)
        {
            SignInObject.SetActive(!SignInObject.activeSelf);
            SignOutObject.SetActive(!SignOutObject.activeSelf);
        }

        LogInState.text = text;
    }
    public void GameStart()
    {
        gameManger.Instance.Firebase.StateUI -= UpdateStateText;
        gameManger.Instance.ChangeScene(Scenes.InGame);
    }

}
