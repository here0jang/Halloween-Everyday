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
            mCreateRoomButtonText.text = "�� ����� " + count + "��";
            count--;
            yield return new WaitForSecondsRealtime(1);
        }

        canCreate = true;
        mCreateRoomButtonText.text = "�� �����";
    }

    private async void onCreateRoomClickedAsync()
    {
        if (!canCreate)
        {
            PopUpUIManager popUp = Instantiate(Resources.Load<PopUpUIManager>(PopUpUIManager.RESOURCE_NAME));
            popUp.InstantiatePopUp("��� �� �ٽ� �õ����ּ���.");
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
            popUp.InstantiatePopUp("�� ������ �����߽��ϴ�.\n��� �� �ٽ� �õ����ּ���.");
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
            popUp.InstantiatePopUp("���� ����");
        }
    }

    private async void onJoinByCodeClicked()
    {
        if(mCodeInput.text == string.Empty)
        {
            PopUpUIManager popUp = Instantiate(Resources.Load<PopUpUIManager>(PopUpUIManager.RESOURCE_NAME));
            popUp.InstantiatePopUp("�ڵ带 �Է��ϼ���");
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
            popUp.InstantiatePopUp("���� ����");
        }
    }
}
