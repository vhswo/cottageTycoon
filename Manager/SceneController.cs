using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    [Header("로딩관련창")]
    [SerializeField] Image loadingBar;
    [SerializeField] Text loadingState;

    private void Start()
    {
        Debug.Log("시작");
        StartCoroutine(LoadSceneAsync((int)gameManger.Instance.NowScene));
    }


    IEnumerator LoadSceneAsync(int sceneNum)
    {
        AsyncOperation op = SceneManager.LoadSceneAsync(sceneNum);
        op.allowSceneActivation = false;
        loadingState.text = "로딩중...";

        float timer = 0f;
        loadingBar.fillAmount = 0f;

        while (!op.isDone)
        {
            yield return null;

            if (op.progress < 0.9f)
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
                    yield break;
                }

            }
        }
    }
}
