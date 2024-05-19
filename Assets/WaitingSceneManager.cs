using System.Collections;
using Unity.Netcode;
using Unity.Services.Authentication;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WaitingSceneManager : MonoBehaviour
{
    [SerializeField] private TMPro.TMP_Text mTopicText;
    [SerializeField] private TMPro.TMP_Text mGameModeText;

    [SerializeField] private TMPro.TMP_Text mPlayerCountText;

    //[SerializeField] private TMPro.TMP_InputField mNicNameInput;
    [SerializeField] private GameObject InviteCode;
    [SerializeField] private TMPro.TMP_Text InviteCodeText;

    [SerializeField] private TMPro.TMP_Text InstructionText;

    [SerializeField] private GameObject StartButton;

    public LobbyManager LobbyManager;

    private int mPlayerMax = 0;
    private string mMyId = "";

    private float mTimer = 0f;
    private bool mIsStarted = false;


    private IEnumerator Start()
    {
        mTopicText.text = LobbyManager.CurLobby.Data["Topic"].Value;
        mPlayerMax = LobbyManager.CurLobby.MaxPlayers;
        mMyId = AuthenticationService.Instance.PlayerId;

        switch (LobbyManager.CurLobby.Data["GameMode"].Value)
        {
            case "Normal":
                {
                    mGameModeText.text = "일반 모드";
                    break;
                }
            case "Together":
                {
                    mGameModeText.text = "반반 모드";
                    break;
                }
            case "Chaos":
                {
                    mGameModeText.text = "카오스 모드";
                    break;
                }
            case "RealTime":
                {
                    mGameModeText.text = "실시간 모드";
                    break;
                }
            default:
                {
                    mGameModeText.text = "일반 모드";
                    break;
                }
        }

        yield return new WaitUntil(() => NetworkManager.Singleton.SceneManager != null);

        NetworkManager.Singleton.SceneManager.OnLoadComplete -= OnLoadComplete;
        NetworkManager.Singleton.SceneManager.OnLoadComplete += OnLoadComplete;
    }

    private void Update()
    {
        mTimer -= Time.deltaTime;
        if (mTimer < 0f)
        {
            mTimer = LobbyManager.LOBBY_UPDATE_TIMER_MAX;

            mPlayerCountText.text = $"{LobbyManager.CurLobby.Players.Count}/{mPlayerMax}";

            string hostId = LobbyManager.CurLobby.HostId;
            InviteCodeText.text = mMyId == hostId ? LobbyManager.CurLobby.LobbyCode : string.Empty;
            InviteCode.SetActive(mMyId == hostId);
            InstructionText.text = mMyId == hostId ? "시작하기 버튼을 누르면 게임이 시작됩니다. " : "방장이 게임을 시작할 때까지 기다리세요.";
            StartButton.SetActive(mMyId == hostId);


            string joinCode = LobbyManager.CurLobby.Data["RelayCode"].Value;
            if (joinCode != "" && mMyId != hostId && !mIsStarted)
            {
                mIsStarted = true;
                GameObject loading = Instantiate(Resources.Load<GameObject>("Loading UI"));
                LobbyManager.StartClientWithRelay(joinCode);
            }
        }
    }
    public async void OnStartClickedAsync()
    {
        GameObject loading = Instantiate(Resources.Load<GameObject>("Loading UI"));

        string relayCode = await LobbyManager.StartHostWithRelay();
        bool IsStarted = await LobbyManager.StartGame(relayCode);
        if (IsStarted)
        {
            NetworkManager.Singleton.SceneManager.LoadScene("03 KEYWORD", LoadSceneMode.Single);
        }
        else
        {
            Destroy(loading);
            PopUpUIManager popUp = Instantiate(Resources.Load<PopUpUIManager>("PopUp UI"));
            popUp.InstantiatePopUp("시작 실패");
        }
    }

    public void OnExitClicked()
    {
        PopUpUIManager popUp = Instantiate(Resources.Load<PopUpUIManager>("PopUp UI"));
        popUp.InstantiatePopUp("정말 퇴장하시겠습니까?", exitAsync);
    }

    private async void exitAsync()
    {
        GameObject loading = Instantiate(Resources.Load<GameObject>("Loading UI"));

        bool exited = await LobbyManager.LeaveRoom();
        if (exited)
        {
            SceneManager.LoadScene("01 MAIN");
        }
        else
        {
            Destroy(loading);
            // TODO : 플레이어가 해결할 수 있는 원인일 경우 안내 메시지 추가 (에러코드로 분류)
            PopUpUIManager popUp = Instantiate(Resources.Load<PopUpUIManager>("PopUp UI"));
            popUp.InstantiatePopUp("퇴장 실패");
        }
    }

    private void OnLoadComplete(ulong clientId, string sceneName, LoadSceneMode loadSceneMode)
    {
        if (!NetworkManager.Singleton.IsServer)
            return;

        if (sceneName == "04 OUTFIT_" + EGameMode.Normal.ToString())
        {
            OutfitManager.Instance.OutfitSceneInit(clientId);
        }
        else if (sceneName == "04 OUTFIT_" + EGameMode.Together.ToString())
        {
            OutfitTogetherSceneManager.Instance.OutfitSceneInit(clientId);
        }
        else if (sceneName == "05 QUIZ")
        {
            QuizManager.QuizSceneInit(clientId);
        }
    }
}
