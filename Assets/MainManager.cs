using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainManager : MonoBehaviour
{
    private const int INIT_PLAYER_COUNT = 4;
    private const int MAX_PLAYERS             = 10;
    private const int MIN_PLAYERS              = 2;
    private const string LOBBY_NAME = "my room";
    // TODO : 파일에서 가져오는 방식으로 변경
    [SerializeField]
    private string[] mTopics = { "과일", "야채", "동물" };


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
        if(TopicInputField.text != string.Empty)
        {
            mTopic = TopicInputField.text;
        }

        // TODO : Addressable 로 변경
        GameObject loading = Instantiate(Resources.Load<GameObject>("Loading UI"));

        bool IsCreated = await LobbyManager.CreateRoomAsync(LOBBY_NAME, mTopic, mGameMode, mMaxPlayers, !PublicRoomStateToggle.isOn);
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
            RoomStateDescText.text = "모든 사람이 참가할 수 있습니다.";
        }
        else
        {
            RoomStateDescText.text = "코드가 있는 친구만 참가할 수 있습니다. ";
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
