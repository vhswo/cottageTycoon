using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    [Header("�ε�����â")]
    [SerializeField] Image loadingBar;
    [SerializeField] Text loadingState;

    bool login;

    private void Start()
    {
        gameManger.Instance.Firebase.SuccessLogin += succesLogin;
        StartCoroutine(LoadSceneAsync((int)gameManger.Instance.NowScene));
        
    }

    IEnumerator LoadSceneAsync(int sceneNum)
    {
        AsyncOperation op = SceneManager.LoadSceneAsync(sceneNum);
        op.allowSceneActivation = false;
        loadingState.text = "�ε���...";

        float timer = 0f;

        while (op.progress < 0.9f)
        {
            yield return null;

            loadingState.text = "���� �ε��ϰ� �ֽ��ϴ�";

            loadingBar.fillAmount = Mathf.Lerp(0f,0.5f, op.progress);

        }

        login = sceneNum == (int)Scenes.InGame ? true : false;

        if (login)
        {
            gameManger.Instance.Firebase.CallbackFirebaseData();
            loadingState.text = "�α��� ������ �޾ƿ����ֽ��ϴ�.";
        }

        while (login)
        {
            yield return null;

            loadingBar.fillAmount = Mathf.Lerp(loadingBar.fillAmount, 0.9f, Time.unscaledDeltaTime);

            loadingState.text = gameManger.Instance.testText.text;
        }

        float loading = loadingBar.fillAmount;

        while (true)
        {
            loadingState.text = "�ε��� ���� �Ϸ�Ǿ����ϴ�";
            timer += Time.unscaledDeltaTime;
            loadingBar.fillAmount = Mathf.Lerp(loading, 1.0f, timer);
            gameManger.Instance.Firebase.SuccessLogin -= succesLogin;

            if(loadingBar.fillAmount >= 1f)
            {
                op.allowSceneActivation = true;
                yield break;
            }
        }
    }
    void succesLogin(bool succes)
    {
        loadingState.text = "�����α�";
        login = succes;
    }
}
