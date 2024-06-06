using UnityEngine;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine.SceneManagement;

public class LoadingSceneManager : MonoBehaviour
{
    [SerializeField] private TMPro.TMP_Text mCountdownText;

    private async void Start()
    {
        float timer = (float)System.DateTime.Now.TimeOfDay.TotalSeconds + GameCount.LOADING_COUNT;
        while (timer > (float)System.DateTime.Now.TimeOfDay.TotalSeconds)
        {
            mCountdownText.text = $"{timer - (float)System.DateTime.Now.TimeOfDay.TotalSeconds:N0}";
            await Task.Yield();
        }

        //GameObject loading = Instantiate(Resources.Load<GameObject>("Loading UI"));

        if (NetworkManager.Singleton.IsHost)
        {
            NetworkManager.Singleton.SceneManager.LoadScene(SceneName.KEYWORD_SCENE, LoadSceneMode.Single);
        }
    }
}
