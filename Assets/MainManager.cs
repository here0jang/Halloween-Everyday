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
        mGameModeDescText.text = "������ ���";

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
        string[] data = googleData.Split('\n'); // ������ ����

        for (int i = 2; i < data.Length; i++)
        {
            string[] row = data[i].Split('\t'); // ���� ���� ����
            if (row[0] != "")
            {
                // ������ ����
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
        if(mTopicInputField.text != mTopic)
        {
            // TODO : �ڵ��ϼ�
            mTopicIndex = 0;
            mTopic = mTopicInputField.text;
        }

        // �ݹ� ���� �ּ� 4��
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

            // TODO : �÷��̾ �ذ��� �� �ִ� ������ ��� �ȳ� �޽��� �߰� (�����ڵ�� �з�)
            PopUpUIManager popUp = Instantiate(Resources.Load<PopUpUIManager>("PopUp UI"));
            popUp.InstantiatePopUp("�� ������ �����߽��ϴ�.\n��� �� �ٽ� �õ����ּ���.");
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
            mRoomStateDescText.text = "��� ����� ������ �� �ֽ��ϴ�.";
        }
        else
        {
            mRoomStateDescText.text = "�ڵ尡 �ִ� ģ���� ������ �� �ֽ��ϴ�. ";
        }
    }

    public void OnGameModeToggleOn()
    {
        if(mNormalModeToggle.isOn)
        {
            mGameMode = EGameMode.Normal;
            mGameModeDescText.text = "������ ���";
        }
        else if(mTogetherModeToggle.isOn)
        {
            // �ּ� 4��
            if(mMaxPlayers < LobbyManager.TOGETHER_MODE_MIN)
            {
                mMaxPlayers = LobbyManager.TOGETHER_MODE_MIN;
                mMaxPlayerText.text = mMaxPlayers.ToString();
            }

            mGameMode = EGameMode.Together;
            mGameModeDescText.text = "�Ӹ��� ���� �Ұ� ���� ���� ������?\nģ���� ������ �ٹ̴� ��� <color=red>(4�� �̻�)</color>";
        }
        else if(mChaosModeToggle.isOn)
        {
            mGameMode = EGameMode.Chaos;
            mGameModeDescText.text = "���� �ܾ ���� ���� �ƹ��� ����\n�ְ��� ���";
        }
        else if (mRealTimeModeToggle.isOn)
        {
            mGameMode = EGameMode.RealTime;
            mGameModeDescText.text = "�ǽð����� ģ���� �ٹ̴� ���� ���� \n�ٷ� ���ߴ� ���, RPC�� �ϱ�";
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
