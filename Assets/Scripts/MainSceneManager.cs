using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainSceneManager : MonoBehaviour
{
    [SerializeField] private TopicKeywordData mTopicKeywordData;

    [SerializeField] private TMPro.TMP_InputField mTopicInput;
    [SerializeField] private Button mRandomTopicButton;
    [SerializeField] private Toggle mPublicRoomStateToggle;
    [SerializeField] private Button mCreateRoomButton;
    [SerializeField] private TMPro.TMP_Text mCreateRoomButtonText;


    [SerializeField] private Button mJoinRandombutton;
    [SerializeField] private Button mJoinByCodebutton;
    [SerializeField] private TMPro.TMP_InputField mCodeInput;

    public bool canCreate = false;


    private void Awake()
    {
        mCreateRoomButton.onClick.RemoveAllListeners();
        mCreateRoomButton.onClick.AddListener(onCreateRoomClickedAsync);    

        mJoinRandombutton.onClick.RemoveAllListeners();
        mJoinRandombutton.onClick.AddListener(onJoinRandomClickedAsync);

       // mJoinByCodebutton.onClick.RemoveAllListeners();
        //mJoinByCodebutton.onClick.AddListener(onJoinByCodeClicked);

        StartCoroutine(startCount());
    }

    IEnumerator startCount()
    {
        int count = GameCount.CREATE_ROOM_COUNT;
        while (count > 0) 
        {
            mCreateRoomButtonText.text = "방 만들기 " + count + "초";
            count--;
            yield return new WaitForSecondsRealtime(1);
        }

        canCreate = true;
        mCreateRoomButtonText.text = "방 만들기";
    }

    private async void onCreateRoomClickedAsync()
    {
        if (!canCreate)
        {
            PopUpUIManager popUp = Instantiate(Resources.Load<PopUpUIManager>(PopUpUIManager.RESOURCE_NAME));
            popUp.InstantiatePopUp("잠시 후 다시 시도해주세요.");
            return;    
        }

        GameObject loading = Instantiate(Resources.Load<GameObject>("Loading UI"));

        int topicIndex = Random.Range(1, mTopicKeywordData.Items.Count);

        bool IsLobbyCreated = await LobbyManager.CreateRoomAsync(mTopicKeywordData.Items[topicIndex].Topic,  topicIndex, false);
        if (IsLobbyCreated)
        {
            SceneManager.LoadScene(SceneName.WAITING_SCENE);
            LobbyManager.mCurSceneName = SceneName.WAITING_SCENE;
        }
        else
        {
            Destroy(loading);
            PopUpUIManager popUp = Instantiate(Resources.Load<PopUpUIManager>(PopUpUIManager.RESOURCE_NAME));
            popUp.InstantiatePopUp("방 생성에 실패했습니다.\n잠시 후 다시 시도해주세요.");
        }
    }

    private async void onJoinRandomClickedAsync()
    {
        GameObject loading = Instantiate(Resources.Load<GameObject>("Loading UI"));

        bool IsJoined = await LobbyManager.JoinRandomRoom();
        if (IsJoined)
        {
            SceneManager.LoadScene(SceneName.WAITING_SCENE);
            LobbyManager.mCurSceneName = SceneName.WAITING_SCENE;
        }
        else
        {
            Destroy(loading);
            PopUpUIManager popUp = Instantiate(Resources.Load<PopUpUIManager>(PopUpUIManager.RESOURCE_NAME));
            popUp.InstantiatePopUp("입장 실패");
        }
    }

    private async void onJoinByCodeClicked()
    {
        if(mCodeInput.text == string.Empty)
        {
            PopUpUIManager popUp = Instantiate(Resources.Load<PopUpUIManager>(PopUpUIManager.RESOURCE_NAME));
            popUp.InstantiatePopUp("코드를 입력하세요");
            return;
        }

        GameObject loading = Instantiate(Resources.Load<GameObject>("Loading UI"));

        bool IsJoined = await LobbyManager.JoinRoomByCode(mCodeInput.text);
        if (IsJoined)
        {
            SceneManager.LoadScene(SceneName.WAITING_SCENE);
            LobbyManager.mCurSceneName = SceneName.WAITING_SCENE;
        }
        else
        {
            Destroy(loading);
            PopUpUIManager popUp = Instantiate(Resources.Load<PopUpUIManager>(PopUpUIManager.RESOURCE_NAME));
            popUp.InstantiatePopUp("입장 실패");
        }
    }
}
