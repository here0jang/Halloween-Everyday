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
        GameStateText.text = LobbyManager.CurLobby.IsPrivate ? "�����" : "����";
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

            InstructionText.text = AuthenticationService.Instance.PlayerId == LobbyManager.CurLobby.HostId ? "�����ϱ� ��ư�� ������ ������ ���۵˴ϴ�. " : "������ ������ ������ ������ ��ٸ�����.";
           
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
        if(exited)
        {
            SceneManager.LoadScene("01 Main");
        }
        else
        {
            Destroy(loading);
            // TODO : �÷��̾ �ذ��� �� �ִ� ������ ��� �ȳ� �޽��� �߰� (�����ڵ�� �з�)
            PopUpUIManager popUp = Instantiate(Resources.Load<PopUpUIManager>("PopUp UI"));
            popUp.InstantiatePopUp("���� ����");
        }
    }
}
