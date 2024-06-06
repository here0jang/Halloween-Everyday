using System;
using System.Threading.Tasks;
using UnityEngine;
using Unity.Netcode;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

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
        startKeywordAsync();
    }

    private async void startKeywordAsync()
    {
        #region Init UI
        mLoading.SetActive(false);

        mTopicText.text = "주제 : " + LobbyManager.CurLobby.Data[LobbyDataKey.LOBBY_DATA_KEY_TOPIC].Value;
        mTopicIndex = Int32.Parse(LobbyManager.CurLobby.Data[LobbyDataKey.LOBBY_DATA_KEY_TOPIC_INDEX].Value);
        mRandomButton.SetActive(mTopicIndex != 0);
        OnRandomButtonClicked();
        #endregion

        #region Timer
        float timer = (float)System.DateTime.Now.TimeOfDay.TotalSeconds + GameCount.KEYWORD_COUNT;
        while (timer > (float)System.DateTime.Now.TimeOfDay.TotalSeconds)
        {
            mLimitText.text = $"{timer - (float)System.DateTime.Now.TimeOfDay.TotalSeconds:N0}";
            await Task.Yield();
        }

        mLoading.SetActive(true);
        #endregion

        PlayerManager mPlayerManager = NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<PlayerManager>();
        MultiplayerManager.Instance.AddDataServerRpc((ulong)mPlayerManager.mMyIndex.Value, mKeywordInput.text);


        if (NetworkManager.Singleton.IsHost)
        {
            NetworkManager.Singleton.SceneManager.LoadScene(SceneName.OUTFIT_SCENE + LobbyManager.CurLobby.Data[LobbyDataKey.LOBBY_DATA_KEY_GAME_MODE].Value, LoadSceneMode.Single);
        }
    }

    public void OnRandomButtonClicked()
    {
        if (mTopicIndex != 0)
        {
            mKeywordInput.text = mTopicKeywordData.Items[mTopicIndex].Keywords[UnityEngine.Random.Range(0, mTopicKeywordData.Items[mTopicIndex].Keywords.Count)];
        }
    }
}