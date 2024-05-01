using Unity.Services.Authentication;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WaitingManager : MonoBehaviour
{
    public TMPro.TMP_Text TopicText;
    public TMPro.TMP_Text GameModeText;
    public TMPro.TMP_Text PlayerCountText;
    public TMPro.TMP_InputField NicNameInputField;

    public GameObject InviteCode;
    public TMPro.TMP_Text InviteCodeText;

    public TMPro.TMP_Text InstructionText;

    public GameObject StartButton;

    private float mUpdateTimer = 0f;
    private int mPlayerMax = 0;
    private string mMyId;

    private GameObject mLoading = null;

    private void Start()
    {
        if (LobbyManager.CurLobby != null)
        {
            TopicText.text = LobbyManager.CurLobby.Data["Topic"].Value;

            switch (LobbyManager.CurLobby.Data["GameMode"].Value)
            {
                case "Relay":
                    {
                        GameModeText.text = "릴레이 모드";
                        break;
                    }
                case "Together":
                    {
                        GameModeText.text = "머내옷누 모드";
                        break;
                    }
                default:
                    {
                        GameModeText.text = "일반 모드";
                        break;
                    }
            }

            mPlayerMax = LobbyManager.CurLobby.MaxPlayers;
            mMyId = AuthenticationService.Instance.PlayerId;
        }
    }

    private void Update()
    {
        if(LobbyManager.CurLobby != null)
        {
            // Update Poll (Rate Limit)
            mUpdateTimer -= Time.deltaTime;
            if (mUpdateTimer < 0f)
            {
                mUpdateTimer = LobbyManager.LOBBY_UPDATE_TIMER_MAX;
            }

            PlayerCountText.text = LobbyManager.CurLobby.Players.Count + "/" + mPlayerMax;

            string hostId = LobbyManager.CurLobby.HostId;

            InviteCodeText.text = mMyId == hostId ? LobbyManager.CurLobby.LobbyCode : string.Empty;
            InviteCode.SetActive(mMyId == hostId);

            InstructionText.text = mMyId == hostId ? "시작하기 버튼을 누르면 게임이 시작됩니다. " : "방장이 게임을 시작할 때까지 기다리세요.";
           
            StartButton.SetActive(mMyId == hostId);


            if (LobbyManager.CurLobby.Data["IsStarted"].Value == "0" && mLoading == null)
            {
                mLoading = Instantiate(Resources.Load<GameObject>("Loading UI"));

                // Client Player Update
                setNicNameAsync();
            }
        }
    }

    private async void setNicNameAsync()
    {
        bool hasNicNameSet = await LobbyManager.SetPlayerData("Name", NicNameInputField.text);
        if (hasNicNameSet)
        {
            SceneManager.LoadScene("03 KEYWORD");
        }
        else
        {
            PopUpUIManager popUp = Instantiate(Resources.Load<PopUpUIManager>("PopUp UI"));
            popUp.InstantiatePopUp("닉네임 실패");
        }
    }

    public async void OnStartClickedAsync()
    {
        mLoading = Instantiate(Resources.Load<GameObject>("Loading UI"));

        // IsStarted : 1 -> 0
        bool IsStarted = await LobbyManager.StartGame();
        if (IsStarted) 
        {
            // Host Player Update
            setNicNameAsync();
        }
        else
        {
            Destroy(mLoading);
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
        if(exited)
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
}
