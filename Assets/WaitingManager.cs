using System.Collections;
using System.Collections.Generic;
using Unity.Services.Authentication;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class WaitingManager : MonoBehaviour
{
    public TMPro.TMP_Text TopicText;
    public TMPro.TMP_Text PlayerCountText;
    public TMPro.TMP_Text GameStateText;

    public GameObject InviteCode;
    public TMPro.TMP_Text InviteCodeText;

    public TMPro.TMP_Text InstructionText;

    public GameObject StartButton;

    private float updateTimer = 0f;
    private GameObject mLoading = null;

    private void Start()
    {
        TopicText.text = LobbyManager.CurLobby.Data["Topic"].Value;
        GameStateText.text = LobbyManager.CurLobby.IsPrivate ? "비공개" : "공개";
    }

    private void Update()
    {
        if(LobbyManager.CurLobby != null)
        {
            // Update Poll (Rate Limit)
            updateTimer -= Time.deltaTime;
            if (updateTimer < 0f)
            {
                updateTimer = LobbyManager.LOBBY_UPDATE_TIMER_MAX;
            }

            PlayerCountText.text = LobbyManager.CurLobby.Players.Count + "/" + LobbyManager.CurLobby.MaxPlayers;

            InviteCodeText.text = AuthenticationService.Instance.PlayerId == LobbyManager.CurLobby.HostId ? LobbyManager.CurLobby.LobbyCode : string.Empty;
            InviteCode.SetActive(AuthenticationService.Instance.PlayerId == LobbyManager.CurLobby.HostId);

            InstructionText.text = AuthenticationService.Instance.PlayerId == LobbyManager.CurLobby.HostId ? "시작하기 버튼을 누르면 게임이 시작됩니다. " : "방장이 게임을 시작할 때까지 기다리세요.";
           
            StartButton.SetActive(AuthenticationService.Instance.PlayerId == LobbyManager.CurLobby.HostId);


            if (LobbyManager.CurLobby.Data["IsStarted"].Value == "0" && mLoading == null)
            {
                mLoading = Instantiate(Resources.Load<GameObject>("Loading UI"));
                // Client Player Update
                SceneManager.LoadScene("03 KEYWORD");
            }
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
            SceneManager.LoadScene("03 KEYWORD");
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
            SceneManager.LoadScene("01 Main");
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
