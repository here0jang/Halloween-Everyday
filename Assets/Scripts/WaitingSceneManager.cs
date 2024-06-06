using System;
using System.Collections;
using Unity.Netcode;
using Unity.Services.Authentication;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class WaitingSceneManager : MonoBehaviour
{
    [SerializeField] private TMPro.TMP_Text mTopicText;
    [SerializeField] private TMPro.TMP_Text mGameModeText;
    //[SerializeField] private TMPro.TMP_Text mNicNameText;
    [SerializeField] private TMPro.TMP_Text mPlayerCountText;
    

    [SerializeField] private Button mReadyStartButton;
    [SerializeField] private TMPro.TMP_Text mDescText;  
    [SerializeField] private Button mExitButton;
    [SerializeField] private GameObject mInviteCode;
    [SerializeField] private TMPro.TMP_Text mInviteCodeText;
    [SerializeField] private Button mCopyCodeButton;
    [SerializeField] private Button mChangeTopicGameModeButton;

    [SerializeField] private TopicKeywordData mTopicKeywordData;

    private const string NICNAME = "별명";

    private string mMyId = "";
    private string mHostId = "";

    private float mTimer = 0f;
    private bool mIsStarted = false;

    private void Awake()
    {
        int topicIndex = Int32.Parse(LobbyManager.CurLobby.Data[LobbyDataKey.LOBBY_DATA_KEY_TOPIC_INDEX].Value);
        //mKeywordInput.text = mTopicKeywordData.Items[topicIndex].Keywords[UnityEngine.Random.Range(0, mTopicKeywordData.Items[topicIndex].Keywords.Count)];

        //mRandomKeywordButton.onClick.RemoveAllListeners();
        //mRandomKeywordButton.onClick.AddListener(() => 
        //{
        //    mKeywordInput.text = mTopicKeywordData.Items[topicIndex].Keywords[UnityEngine.Random.Range(0, mTopicKeywordData.Items[topicIndex].Keywords.Count)];
        //});

        mChangeTopicGameModeButton.onClick.RemoveAllListeners();
        mChangeTopicGameModeButton.onClick.AddListener(onChangeTopicGameModeClicked);

        mCopyCodeButton.onClick.RemoveAllListeners();
        mCopyCodeButton.onClick.AddListener(onCopyCodeClicked);
    }

    private IEnumerator Start()
    {
        mMyId = AuthenticationService.Instance.PlayerId;
        //mNicNameText.text = NICNAME;
        mHostId = LobbyManager.CurLobby.HostId;

        #region Init UI
        mDescText.text = mMyId == mHostId ? "시작하기 버튼을 누르면 게임이 시작됩니다. " : "방장이 게임을 시작할 때까지 기다리세요.";
        mReadyStartButton.gameObject.SetActive(mMyId == mHostId);
        mInviteCodeText.text = mMyId == mHostId ? LobbyManager.CurLobby.LobbyCode : string.Empty;
        mInviteCode.SetActive(mMyId == mHostId);
        mChangeTopicGameModeButton.gameObject.SetActive(mMyId == mHostId);
        #endregion

        updateTopicGamemodePlayercount();

        yield return new WaitUntil(() => NetworkManager.Singleton.SceneManager != null);

        NetworkManager.Singleton.SceneManager.OnLoadComplete -= OnLoadComplete;
        NetworkManager.Singleton.SceneManager.OnLoadComplete += OnLoadComplete;
    }

    private void updateTopicGamemodePlayercount()
    {
        mPlayerCountText.text = $"  {LobbyManager.CurLobby.Players.Count} / {LobbyManager.MAX_PLAYER}";

        mTopicText.text = LobbyManager.CurLobby.Data[LobbyDataKey.LOBBY_DATA_KEY_TOPIC].Value;

        #region GameMode UI
        switch (LobbyManager.CurLobby.Data[LobbyDataKey.LOBBY_DATA_KEY_GAME_MODE].Value)
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

    private void Update()
    {
        mTimer -= Time.deltaTime;
        if (mTimer < 0f)
        {
            mTimer = LobbyManager.LOBBY_UPDATE_TIMER_MAX;

            string joinCode = LobbyManager.CurLobby.Data[LobbyDataKey.LOBBY_DATA_KEY_RELAY_CODE].Value;
            if (joinCode != "" && mMyId != mHostId && !mIsStarted)
            {
                mIsStarted = true;
                GameObject loading = Instantiate(Resources.Load<GameObject>("Loading UI"));
                LobbyManager.StartClientWithRelay(joinCode);
                LobbyManager.mCurSceneName = SceneName.LOGIN_SCENE;
            }

            updateTopicGamemodePlayercount();
        }
    }

    public async void OnReadyStartClickedAsync()
    {
        GameObject loading = Instantiate(Resources.Load<GameObject>("Loading UI"));

        string relayCode = await LobbyManager.StartHostWithRelay();
        bool IsStarted = await LobbyManager.StartGame(relayCode);
        if (IsStarted)
        {
            NetworkManager.Singleton.SceneManager.LoadScene(SceneName.LOADING_SCENE, LoadSceneMode.Single);
            LobbyManager.mCurSceneName = SceneName.LOADING_SCENE;
        }
        else
        {
            Destroy(loading);
            PopUpUIManager popUp = Instantiate(Resources.Load<PopUpUIManager>(PopUpUIManager.RESOURCE_NAME));
            popUp.InstantiatePopUp("시작 실패");
        }
    }

    private void onChangeTopicGameModeClicked()
    {

    }

    private void onCopyCodeClicked()
    {

    }

    public void OnExitClicked()
    {
        PopUpUIManager popUp = Instantiate(Resources.Load<PopUpUIManager>(PopUpUIManager.RESOURCE_NAME));
        popUp.InstantiatePopUp("정말 퇴장하시겠습니까?", async () => 
        {
            GameObject loading = Instantiate(Resources.Load<GameObject>("Loading UI"));

            bool exited = await LobbyManager.LeaveRoom();
            if (exited)
            {
                SceneManager.LoadScene(SceneName.MAIN_SCENE);
            }
            else
            {
                Destroy(loading);
                PopUpUIManager popUp = Instantiate(Resources.Load<PopUpUIManager>(PopUpUIManager.RESOURCE_NAME));
                popUp.InstantiatePopUp("퇴장 실패");
            }
        }); ;
    }

    private void OnLoadComplete(ulong clientId, string sceneName, LoadSceneMode loadSceneMode)
    {
        if (!NetworkManager.Singleton.IsServer)
            return;


        switch(sceneName)
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
                    break;
                }

            case SceneName.OUTFIT_SCENE + GameMode.TOGETHER:
                {
                    OutfitTogetherSceneManager.Instance.OutfitSceneInit(clientId);
                    break;
                }
            case SceneName.OUTFIT_SCENE + GameMode.REALTIME:
                {
                    break;
                }
            default:
                {
                    break;
                }
        }
    }
}
