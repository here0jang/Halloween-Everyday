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
                    mGameModeText.text = "�Ϲ� ���";
                    break;
                }
            case "Together":
                {
                    mGameModeText.text = "�ݹ� ���";
                    break;
                }
            case "Chaos":
                {
                    mGameModeText.text = "ī���� ���";
                    break;
                }
            case "RealTime":
                {
                    mGameModeText.text = "�ǽð� ���";
                    break;
                }
            default:
                {
                    mGameModeText.text = "�Ϲ� ���";
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
            InstructionText.text = mMyId == hostId ? "�����ϱ� ��ư�� ������ ������ ���۵˴ϴ�. " : "������ ������ ������ ������ ��ٸ�����.";
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
            popUp.InstantiatePopUp("���� ����");
        }
    }

    public void OnExitClicked()
    {
        PopUpUIManager popUp = Instantiate(Resources.Load<PopUpUIManager>("PopUp UI"));
        popUp.InstantiatePopUp("���� �����Ͻðڽ��ϱ�?", exitAsync);
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
            // TODO : �÷��̾ �ذ��� �� �ִ� ������ ��� �ȳ� �޽��� �߰� (�����ڵ�� �з�)
            PopUpUIManager popUp = Instantiate(Resources.Load<PopUpUIManager>("PopUp UI"));
            popUp.InstantiatePopUp("���� ����");
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
