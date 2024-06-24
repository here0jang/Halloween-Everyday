using UnityEngine;
using Unity.Netcode;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System.Collections;

public class KeywordSceneManager : NetworkBehaviour
{
    [SerializeField] private TMPro.TMP_Text mLimitText;
    [SerializeField] private TMPro.TMP_Text mTopicText;
    [SerializeField] private TMPro.TMP_InputField mKeywordInput;
    [SerializeField] private GameObject mRandomButton;
    [SerializeField] private GameObject mLoading;

    [SerializeField] private TopicKeywordData mTopicKeywordData;
    private int mTopicIndex = 0;
    private List<ulong> mConnectedClients = new List<ulong>();

    public static KeywordSceneManager Instance { get; private set; }
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }


    public void KeywordSceneInit(ulong id)
    {
        if (!NetworkManager.Singleton.IsHost)
        {
            return;
        }

        mConnectedClients.Add(id);


        if (mConnectedClients.Count < NetworkManager.Singleton.ConnectedClients.Count)
        {
            return;
        }

        StartKeywordClientRpc();
    }

    [ClientRpc]
    private void StartKeywordClientRpc()
    {
        #region Init UI
        mLoading.SetActive(false);

        mTopicText.text = "<color=#6D60CC>" + mTopicKeywordData.Items[MultiplayerManager.Instance.TopicIndex].Topic + "</color>에는 뭐가 있나요?";
        mRandomButton.SetActive(MultiplayerManager.Instance.TopicIndex != 0);
        OnRandomButtonClicked();
        #endregion

        StartCoroutine(timer());
    }

    IEnumerator timer()
    {
        float timer = GameCount.KEYWORD_COUNT;
        while (timer > 0)
        {
            mLimitText.text = timer.ToString();
            timer--;
            yield return new WaitForSecondsRealtime(1);
        }

        mLoading.SetActive(true);


        PlayerManager mPlayerManager = NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<PlayerManager>();
        MultiplayerManager.Instance.AddQuizServerRpc(mPlayerManager.mMyIndex.Value, mKeywordInput.text);


        if (NetworkManager.Singleton.IsHost)
        {
            LoadingSceneManager.Instance.LoadScene(SceneName.OUTFIT_SCENE + MultiplayerManager.Instance.GetGameMode(), true);
        }
    }

    public void OnRandomButtonClicked()
    {
        if (MultiplayerManager.Instance.TopicIndex != 0)
        {
            mKeywordInput.text = mTopicKeywordData.Items[MultiplayerManager.Instance.TopicIndex].Keywords[UnityEngine.Random.Range(0, mTopicKeywordData.Items[MultiplayerManager.Instance.TopicIndex].Keywords.Count)];
        }
    }
}