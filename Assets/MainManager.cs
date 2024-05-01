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
    [SerializeField] private string[] mTopics = { "과일", "야채", "동물" };
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
        hideCanvas(mCreateCanvas);
        showCanvas(mMainCanvas);
    }

    public async void OnCreateRoomClickedAsync()
    {
        // TODO : 버튼으로 클릭시 버튼에 해당하는 주제
        if(mTopicInputField.text != string.Empty)
        {
            mTopic = mTopicInputField.text;
        }

        // TODO : Addressable 로 변경
        GameObject loading = Instantiate(Resources.Load<GameObject>("Loading UI"));

        bool IsCreated = await LobbyManager.CreateRoomAsync(LOBBY_NAME, mTopic, mGameMode, mMaxPlayers, !mPublicRoomStateToggle.isOn);
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
            mRoomStateDescText.text = "모든 사람이 참가할 수 있습니다.";
        }
        else
        {
            mRoomStateDescText.text = "코드가 있는 친구만 참가할 수 있습니다. ";
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
