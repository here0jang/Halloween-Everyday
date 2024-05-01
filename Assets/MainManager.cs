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
    [SerializeField] private string[] mTopics = { "����", "��ä", "����" };
    [SerializeField] private string mTopic;
    [SerializeField] private EGameMode mGameMode = EGameMode.Relay;
    [SerializeField] private int mMaxPlayers = INIT_PLAYER_COUNT;

    [Header("MAIN")] 
    [SerializeField] private CanvasGroup mMainCanvas;
    [Header("Create")]
    [SerializeField] private CanvasGroup mCreateCanvas;
    [Header("Create - Topic")]
    [SerializeField] private TMPro.TMP_InputField mTopicInputField;
    [SerializeField] private Button mRandomTopicButton;
    [Header("Create - GameMode")]
    [SerializeField] private Toggle mRelayModeToggle;
    [SerializeField] private Toggle mTogetherModeToggle;
    [Header("Create - MaxPlayer")]
    [SerializeField] private TMPro.TMP_Text mMaxPlayerText;
    [Header("Create - Room State")]
    [SerializeField] private Toggle mPublicRoomStateToggle;
    [SerializeField] private TMPro.TMP_Text mRoomStateDescText;





    private void Start()
    {
        hideCanvas(mCreateCanvas);
        showCanvas(mMainCanvas);
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
        mRelayModeToggle.isOn = true;

        mMaxPlayers = INIT_PLAYER_COUNT;
        mMaxPlayerText.text = mMaxPlayers.ToString();

        mPublicRoomStateToggle.isOn = true;


        hideCanvas(mMainCanvas);
        showCanvas(mCreateCanvas);
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
        hideCanvas(mCreateCanvas);
        showCanvas(mMainCanvas);
    }

    public async void OnCreateRoomClickedAsync()
    {
        // TODO : ��ư���� Ŭ���� ��ư�� �ش��ϴ� ����
        if(mTopicInputField.text != string.Empty)
        {
            mTopic = mTopicInputField.text;
        }

        // TODO : Addressable �� ����
        GameObject loading = Instantiate(Resources.Load<GameObject>("Loading UI"));

        bool IsCreated = await LobbyManager.CreateRoomAsync(LOBBY_NAME, mTopic, mGameMode, mMaxPlayers, !mPublicRoomStateToggle.isOn);
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
        mTopicInputField.text = mTopic;
    }

    public void OnAddPlayerClicked()
    {
        if (mMaxPlayers < MAX_PLAYERS)
        {
            mMaxPlayers += 1;
            mMaxPlayerText.text = mMaxPlayers.ToString();
        }
    }

    public void OnSubtractPlayerClicked()
    {
        if (mMaxPlayers > MIN_PLAYERS)
        {
            mMaxPlayers -= 1;
            mMaxPlayerText.text = mMaxPlayers.ToString();
        }
    }

    public void OnPublicToggleOn()
    {
        if(mPublicRoomStateToggle.isOn)
        {
            mRoomStateDescText.text = "��� ����� ������ �� �ֽ��ϴ�.";
        }
        else
        {
            mRoomStateDescText.text = "�ڵ尡 �ִ� ģ���� ������ �� �ֽ��ϴ�. ";
        }
    }

    public void OnGameModeToggleOn()
    {
        if(mRelayModeToggle.isOn)
        {
            mGameMode = EGameMode.Relay;
        }
        else if(mTogetherModeToggle.isOn)
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
