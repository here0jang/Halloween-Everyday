using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainManager : MonoBehaviour
{
    public LobbyManager LobbyManager;

    private const int MAX_PLAYERS             = 10;
    private const int INIT_PLAYER_COUNT = 4;
    private const int MIN_PLAYERS              = 2;
    private const string LOBBY_NAME = "my room";

    [SerializeField] private string mTopic;
    [SerializeField] private TopicKeywordData mTopicKeywordData;
    const string TOPIC_KEYWORD_URL = "https://docs.google.com/spreadsheets/d/1yFwrfthu-ryOOeo74lIS7zOib64a80CdK6LjmAJvp0U/export?format=tsv";
    [SerializeField] private int mTopicIndex = 0;
    [SerializeField] private EGameMode mGameMode = EGameMode.Normal;
    [SerializeField] private int mMaxPlayers = INIT_PLAYER_COUNT;


    [Header("MAIN")] 
    [SerializeField] private CanvasGroup mMainCanvas;
    [Header("Create")]
    [SerializeField] private CanvasGroup mCreateCanvas;
    [Header("Create - Topic")]
    [SerializeField] private TMPro.TMP_InputField mTopicInputField;
    [SerializeField] private Button mRandomTopicButton;
    [Header("Create - GameMode")]
    [SerializeField] private Toggle mNormalModeToggle;
    [SerializeField] private Toggle mTogetherModeToggle;
    [SerializeField] private Toggle mChaosModeToggle;
    [SerializeField] private Toggle mRealTimeModeToggle;
    [SerializeField] private TMPro.TMP_Text mGameModeDescText;
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

        if (mTopicKeywordData.Items.Count == 0)
        {
            StartCoroutine(GetGoogleSpreadSheet());
        }
        else
        {
            OnRandomTopicClicked();
        }

        mGameMode = EGameMode.Normal;
        mNormalModeToggle.isOn = true;
        mGameModeDescText.text = "객관식 모드";

        mMaxPlayers = INIT_PLAYER_COUNT;
        mMaxPlayerText.text = mMaxPlayers.ToString();

        mPublicRoomStateToggle.isOn = true;

        hideCanvas(mMainCanvas);
        showCanvas(mCreateCanvas);
    }

    IEnumerator GetGoogleSpreadSheet()
    {
        Item item = new Item();
        List<string> keywords = new List<string>();

        GameObject loading = Instantiate(Resources.Load<GameObject>("Loading UI"));

        UnityWebRequest www = UnityWebRequest.Get(TOPIC_KEYWORD_URL);
        yield return www.SendWebRequest();
        string googleData = www.downloadHandler.text;
        string[] data = googleData.Split('\n'); // 행으로 나눔

        for (int i = 2; i < data.Length; i++)
        {
            string[] row = data[i].Split('\t'); // 행을 열로 나눔
            if (row[0] != "")
            {
                // 이전꺼 저장
                item.Keywords = keywords;
                mTopicKeywordData.Items.Add(item);

                keywords = new List<string>();

                item = new Item();
                item.Topic = row[0];
            }

            keywords.Add(row[1]);
        }

        OnRandomTopicClicked();
        Destroy(loading);
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
        if(mTopicInputField.text != mTopic)
        {
            // TODO : 자동완성
            mTopicIndex = 0;
            mTopic = mTopicInputField.text;
        }

        // 반반 모드는 최소 4명
        if(mGameMode == EGameMode.Together && mMaxPlayers < LobbyManager.TOGETHER_MODE_MIN)
        {
            mMaxPlayers = LobbyManager.TOGETHER_MODE_MIN;
        }

        GameObject loading = Instantiate(Resources.Load<GameObject>("Loading UI"));


        bool IsCreated = await LobbyManager.CreateRoomAsync(LOBBY_NAME, mTopic, mTopicIndex, mGameMode, mMaxPlayers, !mPublicRoomStateToggle.isOn);
        if(IsCreated)
        {
            SceneManager.LoadScene("02 WAITING");
        }
        else
        {
            Destroy(loading);

            // TODO : 플레이어가 해결할 수 있는 원인일 경우 안내 메시지 추가 (에러코드로 분류)
            PopUpUIManager popUp = Instantiate(Resources.Load<PopUpUIManager>("PopUp UI"));
            popUp.InstantiatePopUp("방 생성에 실패했습니다.\n잠시 후 다시 시도해주세요.");
        }
    }

    public void OnRandomTopicClicked()
    {
        mTopicIndex = Random.Range(1, mTopicKeywordData.Items.Count);
        mTopic = mTopicKeywordData.Items[mTopicIndex].Topic;
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
        if(mGameMode == EGameMode.Together)
        {
            if (mMaxPlayers > LobbyManager.TOGETHER_MODE_MIN)
            {
                mMaxPlayers -= 1;
                mMaxPlayerText.text = mMaxPlayers.ToString();
            }
        }
        else
        {
            if (mMaxPlayers > MIN_PLAYERS)
            {
                mMaxPlayers -= 1;
                mMaxPlayerText.text = mMaxPlayers.ToString();
            }
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
        if(mNormalModeToggle.isOn)
        {
            mGameMode = EGameMode.Normal;
            mGameModeDescText.text = "객관식 모드";
        }
        else if(mTogetherModeToggle.isOn)
        {
            // 최소 4명
            if(mMaxPlayers < LobbyManager.TOGETHER_MODE_MIN)
            {
                mMaxPlayers = LobbyManager.TOGETHER_MODE_MIN;
                mMaxPlayerText.text = mMaxPlayers.ToString();
            }

            mGameMode = EGameMode.Together;
            mGameModeDescText.text = "머리는 내가 할게 옷은 누가 입힐래?\n친구와 나눠서 꾸미는 모드 <color=red>(4명 이상)</color>";
        }
        else if(mChaosModeToggle.isOn)
        {
            mGameMode = EGameMode.Chaos;
            mGameModeDescText.text = "다음 단어가 뭐가 될지 아무도 몰라\n주관식 모드";
        }
        else if (mRealTimeModeToggle.isOn)
        {
            mGameMode = EGameMode.RealTime;
            mGameModeDescText.text = "실시간으로 친구가 꾸미는 것을 보고 \n바로 맞추는 모드, RPC로 하기";
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
