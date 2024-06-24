using System.Collections;
using UnityEngine;

public class BootManager : MonoBehaviour
{
    IEnumerator Start()
    {
        yield return new WaitUntil(() => LoadingSceneManager.Instance != null);

        LoadingSceneManager.Instance.LoadScene(SceneName.WEB_LOGIN_SCENE, false);
    }
}
