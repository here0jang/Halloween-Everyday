using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainManager : MonoBehaviour
{
    public CanvasGroup MainCanvas;

    public CanvasGroup CreateCanvas;
    public TMPro.TMP_InputField TopicInputField;
    public TMPro.TMP_Text MaxPlayerText;
    public Toggle PrivateToggle;

    private const int INIT_PLAYER_COUNT = 4;
    private const int MAX_PLAYERS             = 10;
    private const int MIN_PLAYERS              = 2;

    private const string LOBBY_NAME = "my room";
    private const string TOPIC               = "veggies";

    [SerializeField]
    private string mTopic = TOPIC;
    [SerializeField]
    private int mMaxPlayers = INIT_PLAYER_COUNT;


    private void Start()
    {
        hideCanvas(CreateCanvas);
        showCanvas(MainCanvas);
    }

    // ---------------------------------------------------------------------------------------------
    //
    // MAIN
    //
    public void OnGoToCreateClicked()
    {
        // Init Create
        TopicInputField.text = string.Empty;
        PrivateToggle.isOn = false;
        mMaxPlayers = INIT_PLAYER_COUNT;
        MaxPlayerText.text = mMaxPlayers.ToString();

        hideCanvas(MainCanvas);
        showCanvas(CreateCanvas);
    }

    public async void OnJoinRandomClickedAsync()
    {
        // TODO : Addressable 로 변경
        GameObject loading = Instantiate(Resources.Load<GameObject>("Loading UI"));

        bool IsJoined = await LobbyManager.JoinRandomRoom();
        if(IsJoined)
        {
            SceneManager.LoadScene("02 WAITING");
        }
        else
        {
            Destroy(loading);

            // TODO : 플레이어가 해결할 수 있는 원인일 경우 안내 메시지 추가 (에러코드로 분류)
            PopUpUIManager popUp = Instantiate(Resources.Load<PopUpUIManager>("PopUp UI"));
            popUp.InstantiatePopUp("입장 실패");
        }
    }

    // ---------------------------------------------------------------------------------------------
    //
    // CREATE
    //
    public void OnExitCreateClicked()
    {
        hideCanvas(CreateCanvas);
        showCanvas(MainCanvas);
    }

    public async void OnCreateRoomClickedAsync()
    {
        // TODO : 버튼으로 클릭시 버튼에 해당하는 주제
        mTopic = TopicInputField.text;

        // TODO : Addressable 로 변경
        GameObject loading = Instantiate(Resources.Load<GameObject>("Loading UI"));

        bool IsCreated = await LobbyManager.CreateRoomAsync(LOBBY_NAME, mTopic, mMaxPlayers, PrivateToggle.isOn);
        if(IsCreated)
        {
            SceneManager.LoadScene("02 WAITING");
        }
        else
        {
            Destroy(loading);

            // TODO : 플레이어가 해결할 수 있는 원인일 경우 안내 메시지 추가 (에러코드로 분류)
            PopUpUIManager popUp = Instantiate(Resources.Load<PopUpUIManager>("PopUp UI"));
            popUp.InstantiatePopUp("방 생성 실패");
        }
    }

    public void OnAddPlayerClicked()
    {
        if (mMaxPlayers < MAX_PLAYERS)
        {
            mMaxPlayers += 1;
            MaxPlayerText.text = mMaxPlayers.ToString();
        }
    }

    public void OnSubtractPlayerClicked()
    {
        if (mMaxPlayers > MIN_PLAYERS)
        {
            mMaxPlayers -= 1;
            MaxPlayerText.text = mMaxPlayers.ToString();
        }
    }




    private void showCanvas(CanvasGroup canvasGroup)
    {
        canvasGroup.alpha = 1.0f;
        canvasGroup.blocksRaycasts = true;
    }

    private void hideCanvas(CanvasGroup canvasGroup)
    {
        canvasGroup.alpha = 0f;
        canvasGroup.blocksRaycasts = false;
    }
}
