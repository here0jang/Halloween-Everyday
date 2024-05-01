using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainManager : MonoBehaviour
{
    private const int INIT_PLAYER_COUNT = 4;
    private const int MAX_PLAYERS             = 10;
    private const int MIN_PLAYERS              = 2;
    private const string LOBBY_NAME = "my room";
    // TODO : ���Ͽ��� �������� ������� ����
    [SerializeField]
    private string[] mTopics = { "����", "��ä", "����" };


    [Header("MAIN")]
    public CanvasGroup MainCanvas;
    [Header("Create")]
    public CanvasGroup CreateCanvas;

    public TMPro.TMP_InputField TopicInputField;
    public Button RandomTopicButton;

    public Toggle RelayModeToggle;
    public Toggle TogetherModeToggle;

    public TMPro.TMP_Text MaxPlayerText;

    public Toggle PublicRoomStateToggle;
    public TMPro.TMP_Text RoomStateDescText;


    [SerializeField]
    private string mTopic;
    [SerializeField]
    private EGameMode mGameMode = EGameMode.Relay;
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
        OnRandomTopicClicked();

        mGameMode = EGameMode.Relay;
        RelayModeToggle.isOn = true;

        mMaxPlayers = INIT_PLAYER_COUNT;
        MaxPlayerText.text = mMaxPlayers.ToString();

        PublicRoomStateToggle.isOn = true;


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
        if(TopicInputField.text != string.Empty)
        {
            mTopic = TopicInputField.text;
        }

        // TODO : Addressable �� ����
        GameObject loading = Instantiate(Resources.Load<GameObject>("Loading UI"));

        bool IsCreated = await LobbyManager.CreateRoomAsync(LOBBY_NAME, mTopic, mGameMode, mMaxPlayers, !PublicRoomStateToggle.isOn);
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

    public void OnRandomTopicClicked()
    {
        mTopic = mTopics[Random.Range(0, mTopics.Length)];
        TopicInputField.text = mTopic;
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

    public void OnPublicToggleOn()
    {
        if(PublicRoomStateToggle.isOn)
        {
            RoomStateDescText.text = "��� ����� ������ �� �ֽ��ϴ�.";
        }
        else
        {
            RoomStateDescText.text = "�ڵ尡 �ִ� ģ���� ������ �� �ֽ��ϴ�. ";
        }
    }

    public void OnGameModeToggleOn()
    {
        if(RelayModeToggle.isOn)
        {
            mGameMode = EGameMode.Relay;
        }
        else if(TogetherModeToggle.isOn)
        {
            mGameMode = EGameMode.Together;
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
