using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InGameUI : MonoBehaviour
{
    [SerializeField] Image profile; //리소스 활용
    [SerializeField] Text gold;
    [SerializeField] Text level;
    [SerializeField] Text UserID;

    private void Start()
    {
        gameManger.Instance.player.SubScript += UpdateUI;
        UpdateUI(gameManger.Instance.player.status);
    }

    public void UpdateUI(PlayerStatus status)
    {
        gold.text = status.gold.ToString();
        level.text = status.level.ToString();
        UserID.text = status.id;
    }


    public void GetGold()
    {
        gameManger.Instance.player.SetGold(1);
    }

    public void ReturnLogIn()
    {
        gameManger.Instance.player.SubScript -= UpdateUI;
        gameManger.Instance.ChangeScene(Scenes.SignIn);
    }

}
