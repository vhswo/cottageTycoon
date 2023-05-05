using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class InGameController : MonoBehaviour
{
    [SerializeField] InGameUI UI; // 인게임 UI로 교체 예정

    GameObject setting;

    private void Start()
    {
    }


    public void SetSetting()
    {
        setting.SetActive(true);
    }
}
