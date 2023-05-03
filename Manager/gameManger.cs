using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class gameManger : MonoBehaviour
{
    static gameManger instance;
    public static gameManger Instance => instance;


    firebaseManager firebaseMgr;
    public firebaseManager Firebase => firebaseMgr;


    [SerializeField] SceneController sceneController;

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
        sceneController.ChangeScene(scene);
    }

}
