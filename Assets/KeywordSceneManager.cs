using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Netcode;

public class KeywordSceneManager : MonoBehaviour
{
    [SerializeField] private TMPro.TMP_Text mLimitText;
    [SerializeField] private TMPro.TMP_Text mTopicText;
    [SerializeField] private TMPro.TMP_InputField mKeywordInput;
    [SerializeField] private GameObject mRandomButton;
    [SerializeField] private GameObject mLoading;

    [SerializeField] private TopicKeywordData mTopicKeywordData;
    private int mTopicIndex = 0;


    public async void Start()
    {
        // �ε�
        mLoading.SetActive(false);

        // ����
        mTopicText.text = "���� : " + LobbyManager.CurLobby.Data["Topic"].Value;
        mTopicIndex = Int32.Parse(LobbyManager.CurLobby.Data["TopicIndex"].Value);
        mRandomButton.SetActive(mTopicIndex != 0);
        OnRandomButtonClicked();

        // Ÿ�̸�
        float timer = (float)System.DateTime.Now.TimeOfDay.TotalSeconds + LobbyManager.KEYWORD_COUNT; /* �ϵ���� �ð� �̿� */
        while (timer > (float)System.DateTime.Now.TimeOfDay.TotalSeconds)
        {
            mLimitText.text = $"{timer - (float)System.DateTime.Now.TimeOfDay.TotalSeconds:N0}";
            await Task.Yield();
        }

        // �ε� 
        mLoading.SetActive(true);

        // Ű���� ����
        PlayerManager mPlayerManager = NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<PlayerManager>();
        mPlayerManager.SendKeywordServerRpc(mKeywordInput.text);


        // �������� �̵�
        if (NetworkManager.Singleton.IsHost)
        {
            NetworkManager.Singleton.SceneManager.LoadScene("04 OUTFIT_" + LobbyManager.CurLobby.Data["GameMode"].Value, LoadSceneMode.Single);
        }
    }

    public void OnRandomButtonClicked()
    {
        if(mTopicIndex != 0)
        {
            mKeywordInput.text = mTopicKeywordData.Items[mTopicIndex].Keywords[UnityEngine.Random.Range(0, mTopicKeywordData.Items[mTopicIndex].Keywords.Count)];
        }
    }
}
