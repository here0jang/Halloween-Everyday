using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public struct SceneName
{
    public const string APP_LOGIN_SCENE = "00 App";
    public const string WEB_LOGIN_SCENE = "00 Web";
    public const string MAIN_SCENE = "01 MAIN";
    public const string WAITING_SCENE = "02 WAITING";
    public const string LOADING_SCENE = "03 LOADING";
    public const string KEYWORD_SCENE = "04 KEYWORD";
    public const string OUTFIT_SCENE = "05 OUTFIT_";
    public const string GUESS_SCENE = "06 GUESS_";
    public const string SHOW_SCENE = "07 SHOW_";
}

public class LoadingSceneManager : MonoBehaviour
{
    public static LoadingSceneManager Instance;
    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void LoadScene(string sceneName, bool IsNetworkSessionActive  = true)
    {
        if(IsNetworkSessionActive)
        {
            if(NetworkManager.Singleton.IsHost)
            {
                NetworkManager.Singleton.SceneManager.LoadScene(sceneName, UnityEngine.SceneManagement.LoadSceneMode.Single);
            }
        }
        else
        {
            SceneManager.LoadScene(sceneName);
        }
    }

    public void OnLoadComplete(ulong clientId, string sceneName, LoadSceneMode loadSceneMode)
    {
        if (!NetworkManager.Singleton.IsServer)
            return;


        switch (sceneName)
        {
            case SceneName.KEYWORD_SCENE:
                {
                    KeywordSceneManager.Instance.KeywordSceneInit(clientId);
                    break;
                }
            case SceneName.OUTFIT_SCENE + GameMode.NORMAL:
                {
                    OutfitNormalSceneManager.Instance.OutfitSceneInit(clientId);
                    break;
                }
            case SceneName.GUESS_SCENE + GameMode.NORMAL:
                {
                    GuessNormalSceneManager.Instance.GuessSceneInit(clientId);
                    break;
                }
            case SceneName.SHOW_SCENE + GameMode.NORMAL:
                {
                    ShowNormalSceneManager.Instance.ShowSceneInit(clientId);
                    break;
                }
            default:
                {
                    break;
                }
        }
    }
}
