using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

using UnityEngine.UI;

public enum Scenes
{
    SignIn,
    InGame,
    Loding,
}

public class gameManger : MonoBehaviour
{
    static gameManger instance;
    public static gameManger Instance => instance;

    public Text testText;

    firebaseManager firebaseMgr;
    public firebaseManager Firebase => firebaseMgr;

    Scenes nowScene = Scenes.SignIn;
    public Scenes NowScene => nowScene;

    public Player player { get; private set; }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);

            firebaseMgr = new();
        }
        else
        {
            if (instance != this)
            {
                Destroy(gameObject);
                return;
            }
        }
    }

    public void UseFirebase(firebase use)
    {

        switch (use)
        {
            case firebase.LogIn:
                firebaseMgr.LogIn();
                break;
            case firebase.LogOut:
                firebaseMgr.LogOut();
                break;
            default:
                break;
        }
    }

    public void ChangeScene(Scenes scene)
    {
        if (nowScene == scene) return;

        nowScene = scene;

        SceneManager.LoadScene((int)Scenes.Loding);
    }

    public void SetPlayer(PlayerStatus status)
    {
        player = new(status);
    }
}
