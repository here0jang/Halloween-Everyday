using System.Collections;
using System.Collections.Generic;
using System.Text;
using Unity.Netcode;
using Unity.Services.Relay;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class WaitingSceneManager : NetworkBehaviour
{
    [Header("[INVITE CODE UI]")]
    [SerializeField] private TMPro.TMP_Text mInviteCodeText;
    [SerializeField] private Button mCodeButton;

    [Header("[TOPIC GAMEMODE UI]")]
    [SerializeField] private TMPro.TMP_Text mGameModeText;
    [SerializeField] private TMPro.TMP_Text mTopicText;
    [SerializeField] private Button mChangeButton;

    [Header("[PLAYER COUNT UI]")]
    [SerializeField] private Image[] mNameImages = new Image[LobbyManager.MAX_PLAYER];
    [SerializeField] private TMPro.TMP_Text mPlayerCountText;

    [Header("[TIMER UI]")]
    [SerializeField] private GameObject mTimer;
    [SerializeField] private TMPro.TMP_Text mTimerText;

    [Header("[NEW PLAYER ENTER UI]")]
    [SerializeField] private GameObject mEnter;
    [SerializeField] private TMPro.TMP_Text mEnterText;

    [Header("")]
    [SerializeField] private Button mStartButton;
    [SerializeField] private TMPro.TMP_Text mDescText;
    [SerializeField] private Button mExitButton;

    [Header("")]
    [SerializeField] private TopicKeywordData mTopicKeywordData;
    private StringBuilder mStringBuilder = new StringBuilder(16);


    public static WaitingSceneManager Instance { get; private set; }
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        mChangeButton.onClick.RemoveAllListeners();
        mChangeButton.onClick.AddListener(onChangeTopicGameModeClicked);

        mCodeButton.onClick.RemoveAllListeners();
        mCodeButton.onClick.AddListener(onCopyCodeClicked);

        mStartButton.onClick.RemoveAllListeners();
        mStartButton.onClick.AddListener(startGameClientRpc);

        mExitButton.onClick.RemoveAllListeners();
        mExitButton.onClick.AddListener(() => 
        {
            PopUpUIManager popUp = Instantiate(Resources.Load<PopUpUIManager>(PopUpUIManager.RESOURCE_NAME));
            popUp.InstantiatePopUp("정말 퇴장하시겠습니까?", () => { NetworkManager.Singleton.Shutdown(); }, null);
        });

        NetworkManager.Singleton.SceneManager.OnLoadComplete -= LoadingSceneManager.Instance.OnLoadComplete;
        NetworkManager.Singleton.SceneManager.OnLoadComplete += LoadingSceneManager.Instance.OnLoadComplete;

        NetworkManager.Singleton.OnClientStopped -= onClientStopped;
        NetworkManager.Singleton.OnClientStopped += onClientStopped;
    }

    private void Start()
    {
        PlayerManager playerManager = NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<PlayerManager>();
        MultiplayerManager.Instance.AddNameServerRpc(playerManager.mMyIndex.Value, PlayerPrefs.GetString("Name"));

        if (IsHost)
        {
            MultiplayerManager.Instance.TopicIndex = (int)UnityEngine.Random.Range(1, 4);
            MultiplayerManager.Instance.mTopic.Value = mTopicKeywordData.Items[MultiplayerManager.Instance.TopicIndex].Topic;
            MultiplayerManager.Instance.mGameMode.Value = GameMode.NORMAL;
            MultiplayerManager.Instance.mRelayCode.Value = PlayerPrefs.GetString("Code");



            MultiplayerManager.Instance.mConnectedCount.Value += 1;

            NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
            NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;

            NetworkManager.Singleton.OnClientDisconnectCallback -= onClientDisconnected;
            NetworkManager.Singleton.OnClientDisconnectCallback += onClientDisconnected;
        }

        #region Init UI
        updateTopicGameModeUI();

        mDescText.text = NetworkManager.Singleton.IsHost ? "시작하기 버튼을 누르면 게임이 시작됩니다. " : "방장이 게임을 시작할 때까지 기다리세요.";
        mStartButton.gameObject.SetActive(NetworkManager.Singleton.IsHost);
        mInviteCodeText.text = NetworkManager.Singleton.IsHost ? "초대코드   " +  MultiplayerManager.Instance.mRelayCode.Value.ToString() : string.Empty;
        mChangeButton.gameObject.SetActive(NetworkManager.Singleton.IsHost);
        #endregion
    }

    private void onClientDisconnected(ulong obj)
    {
        MultiplayerManager.Instance.RemoveDisconnectedPlayerName(obj);
        MultiplayerManager.Instance.mConnectedCount.Value -= 1;
    }
    private void OnClientConnected(ulong obj)
    {
        MultiplayerManager.Instance.mConnectedCount.Value += 1;
    }

    public void UpdatePlayerCount(int newVal)
    {
        mStringBuilder.Clear();
        mStringBuilder.Append(newVal);
        mStringBuilder.Append("/");
        mStringBuilder.Append(LobbyManager.MAX_PLAYER);
        mPlayerCountText.text = mStringBuilder.ToString();
    }
    public void ShowEnter(string name)
    {
        StartCoroutine(enter(name));
    }

    IEnumerator enter(string name)
    {
        mEnterText.text = name + "님이 입장했습니다.";
        mEnter.gameObject.SetActive(true);
        yield return new WaitForSecondsRealtime(1.5f);
        mEnter.gameObject.SetActive(false);
    }

    public void UpdatePlayerList()
    {
        Color enableColor = new Color(0, 0, 0, 1);
        Color disableColor = new Color(0, 0, 0, 0.2f);

        List<string> list = MultiplayerManager.Instance.GetNames();

        int i;
        for (i = 0; i < list.Count; i++)
        {
            mNameImages[i].GetComponent<Image>().color = enableColor;
            mNameImages[i].GetComponentInChildren<TMPro.TMP_Text>().text = list[i];
        }

        for(i = list.Count; i < mNameImages.Length; i++)
        {
            mNameImages[i].GetComponent<Image>().color = disableColor;
            mNameImages[i].GetComponentInChildren<TMPro.TMP_Text>().text = "별명";
        }
    }

    private void updateTopicGameModeUI()
    {
        mTopicText.text = mTopicKeywordData.Items[MultiplayerManager.Instance.TopicIndex].Topic;
        
        #region GameMode UI
        switch (MultiplayerManager.Instance.GetGameMode())
        {
            case GameMode.NORMAL:
                {
                    mGameModeText.text = "일반 모드";
                    break;
                }
            case GameMode.TOGETHER:
                {
                    mGameModeText.text = "반반 모드";
                    break;
                }
            case GameMode.REALTIME:
                {
                    mGameModeText.text = "실시간 모드";
                    break;
                }
            case GameMode.QUIZ:
                {
                    mGameModeText.text = "퀴즈 모드";
                    break;
                }
            default:
                {
                    Debug.Assert(false, "wrong Game Mode!");
                    mGameModeText.text = "일반 모드";
                    break;
                }
        }
        #endregion
    }

    private void onChangeTopicGameModeClicked()
    {

    }

    private void onCopyCodeClicked()
    {

    }

    [ClientRpc]
    private void startGameClientRpc()
    {
        StartCoroutine(timer());
    }

    IEnumerator timer()
    {
        mTimer.SetActive(true);

        int i = 3;
        while(i > 0)
        {
            mTimerText.text = i.ToString();
            i--;
            yield return new WaitForSecondsRealtime(1);
        }

        if(IsHost)
        {
            LoadingSceneManager.Instance.LoadScene(SceneName.KEYWORD_SCENE, true);
        }
    }

    private void onClientStopped(bool obj)
    {
        PopUpUIManager popUp = Instantiate(Resources.Load<PopUpUIManager>(PopUpUIManager.RESOURCE_NAME));
        popUp.InstantiatePopUp("방이 닫혔습니다.\n메인 화면으로 이동하세요", () => { LoadingSceneManager.Instance.LoadScene(SceneName.WEB_LOGIN_SCENE, false); });
    }
}
