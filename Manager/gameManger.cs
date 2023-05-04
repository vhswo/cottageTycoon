using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

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


    firebaseManager firebaseMgr;
    public firebaseManager Firebase => firebaseMgr;

    Scenes nowScene = Scenes.SignIn;
    public Scenes NowScene => nowScene;

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

}
