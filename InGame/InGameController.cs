using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class InGameController : MonoBehaviour
{
    Text score;
    Image profile;
    GameObject setting;

    public void ReturnLogIn()
    {
        gameManger.Instance.ChangeScene(Scenes.SignIn);
    }

    public void SetSetting()
    {
        setting.SetActive(true);
    }
}
