using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class InGameController : MonoBehaviour
{
    [SerializeField] InGameUI UI; // �ΰ��� UI�� ��ü ����

    GameObject setting;

    private void Start()
    {
    }


    public void SetSetting()
    {
        setting.SetActive(true);
    }
}
