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
        // 로딩
        mLoading.SetActive(false);

        // 주제
        mTopicText.text = "주제 : " + LobbyManager.CurLobby.Data["Topic"].Value;
        mTopicIndex = Int32.Parse(LobbyManager.CurLobby.Data["TopicIndex"].Value);
        mRandomButton.SetActive(mTopicIndex != 0);
        OnRandomButtonClicked();

        // 타이머
        float timer = (float)System.DateTime.Now.TimeOfDay.TotalSeconds + LobbyManager.KEYWORD_COUNT; /* 하드웨어 시간 이용 */
        while (timer > (float)System.DateTime.Now.TimeOfDay.TotalSeconds)
        {
            mLimitText.text = $"{timer - (float)System.DateTime.Now.TimeOfDay.TotalSeconds:N0}";
            await Task.Yield();
        }

        // 로딩 
        mLoading.SetActive(true);

        // 키워드 저장
        PlayerManager mPlayerManager = NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<PlayerManager>();
        mPlayerManager.SendKeywordServerRpc(mKeywordInput.text);


        // 옷입히기 이동
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
