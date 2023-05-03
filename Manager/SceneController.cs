using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public enum Scenes
{
    SignIn,
    InGame
}

public class SceneController : MonoBehaviour
{
    [Header("로딩관련창")]
    [SerializeField] GameObject loadingUI;
    [SerializeField] Image loadingBar;
    [SerializeField] Text loadingState;
    Scenes nowScene;

    public void ChangeScene(Scenes scene)
    {
        if (nowScene == scene) return;

        loadingUI.SetActive(true);
        nowScene = scene;
        StopAllCoroutines();
       // SceneManager.LoadScene((int)nowScene);
        StartCoroutine(LoadSceneAsync((int)nowScene));
    }

    IEnumerator LoadSceneAsync(int sceneNum)
    {
        AsyncOperation op = SceneManager.LoadSceneAsync(sceneNum);
        op.allowSceneActivation = false;
        loadingState.text = "로딩중...";

        float timer = 0f;

        while(!op.isDone)
        {
            yield return null;

            if(op.progress < 0.9f)
            {
                loadingBar.fillAmount = op.progress;
            }
            else
            {
                loadingState.text = "로딩이 거의 완료되었습니다";
                timer += Time.unscaledDeltaTime;
                loadingBar.fillAmount = Mathf.Lerp(0.9f, 1.0f, timer);
                if(loadingBar.fillAmount >= 1f)
                {
                    op.allowSceneActivation = true;
                    loadingUI.SetActive(false);
                    yield break;
                }

            }
        }
    }
}
