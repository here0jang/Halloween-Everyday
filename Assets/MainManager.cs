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
        // TODO : Addressable �� ����
        GameObject loading = Instantiate(Resources.Load<GameObject>("Loading UI"));

        bool IsJoined = await LobbyManager.JoinRandomRoom();
        if(IsJoined)
        {
            SceneManager.LoadScene("02 WAITING");
        }
        else
        {
            Destroy(loading);

            // TODO : �÷��̾ �ذ��� �� �ִ� ������ ��� �ȳ� �޽��� �߰� (�����ڵ�� �з�)
            PopUpUIManager popUp = Instantiate(Resources.Load<PopUpUIManager>("PopUp UI"));
            popUp.InstantiatePopUp("���� ����");
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
        // TODO : ��ư���� Ŭ���� ��ư�� �ش��ϴ� ����
        mTopic = TopicInputField.text;

        // TODO : Addressable �� ����
        GameObject loading = Instantiate(Resources.Load<GameObject>("Loading UI"));

        bool IsCreated = await LobbyManager.CreateRoomAsync(LOBBY_NAME, mTopic, mMaxPlayers, PrivateToggle.isOn);
        if(IsCreated)
        {
            SceneManager.LoadScene("02 WAITING");
        }
        else
        {
            Destroy(loading);

            // TODO : �÷��̾ �ذ��� �� �ִ� ������ ��� �ȳ� �޽��� �߰� (�����ڵ�� �з�)
            PopUpUIManager popUp = Instantiate(Resources.Load<PopUpUIManager>("PopUp UI"));
            popUp.InstantiatePopUp("�� ���� ����");
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
