using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;
using MameshibaGames.Kekos.RuntimeExampleScene.Spawner;
using MameshibaGames.Common.Helpers;

public class QuizNormalSceneManager : NetworkBehaviour
{
    [SerializeField] private GameObject mResult;
    [SerializeField] private GameObject mLoading;
    [SerializeField] private TMPro.TMP_Text mTopicText;
    public TMPro.TMP_Text LimitText;
    public TMPro.TMP_Text nicNameText;
    public List<KeywordButton> KeywordButtons = new List<KeywordButton>();


    [SerializeField] private CharacterSpawner mCharacterSpawner;

    private static List<ulong> mConnectedClients = new List<ulong>();



    public static QuizNormalSceneManager Instance { get; private set; }
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


    public void QuizSceneInit(ulong id)
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

        StartQuizClientRpc();
    }

    [ClientRpc]
    private void StartQuizClientRpc()
    {
        startQuizAsync();
    }

    private async void startQuizAsync()
    {
        mTopicText.text = "주제 : " + LobbyManager.CurLobby.Data["Topic"].Value;
        mLoading.SetActive(false);




        for (int answerIndex = 0; answerIndex < NetworkManager.Singleton.ConnectedClients.Count; answerIndex++)
        {
            // 캐릭터
            //string styleId = NetworkManager.Singleton.ConnectedClientsList[answerIndex].PlayerObject.GetComponent<PlayerManager>().mCurStyle.Value.ToString();
            //string styleId = NetworkManager.Singleton.ConnectedClientsList[answerIndex].PlayerObject.GetComponent<PlayerManager>().mMyStyle.Value.ToString();
            //await Task.Delay(2000);
            //mCharacterSpawner.SpawnSavedCharacter(styleId);

            // 현재 키워드
            //string answer = NetworkManager.Singleton.ConnectedClientsList[answerIndex].PlayerObject.GetComponent<PlayerManager>().mCurKeyword.Value.ToString();

            // 보기 4개
            List<string> keywordList = new List<string>();
            if (KeywordButtons.Count > NetworkManager.Singleton.ConnectedClients.Count)
            {
                int i = 0;
                for (i = 0; i < NetworkManager.Singleton.ConnectedClients.Count; i++)
                {
                    //keywordList.Add(NetworkManager.Singleton.ConnectedClientsList[i].PlayerObject.GetComponent<PlayerManager>().mCurKeyword.Value.ToString());
                }

                // TODO : 나중에 다른 방식으로 변경
                for (int j = i; j < KeywordButtons.Count; j++)
                {
                    keywordList.Add("");
                }
            }
            else
            {
                // 전체 키워드 복사
                List<string> tmpList = new List<string>();
                for (int k = 0; k < NetworkManager.Singleton.ConnectedClients.Count; k++)
                {
                    //tmpList.Add(NetworkManager.Singleton.ConnectedClientsList[k].PlayerObject.GetComponent<PlayerManager>().mCurKeyword.Value.ToString());
                }

                // 정답 제거
                //tmpList.Remove(answer);

                // 정답을 제외한 보기 키워드 랜덤으로 추가
                for (int l = 0; l < KeywordButtons.Count - 1; l++)
                {
                    string keyword = tmpList[Random.Range(0, tmpList.Count)];
                    keywordList.Add(keyword);
                    tmpList.Remove(keyword);
                }

                // 정답 추가
                //keywordList.Add(answer);
            }

            // 키워드 섞기
            keywordList.Shuffle();

            // 키워드 버튼에 뿌리기
            for (int index = 0; index < KeywordButtons.Count; index++)
            {
                //KeywordButtons[index].SetKeywordButton(keywordList[index], answer);
            }



            // 타이머 시작
            float timer = (float)System.DateTime.Now.TimeOfDay.TotalSeconds + GameCount.QUIZ_COUNT; /* 하드웨어 시간 이용 */
            while (timer > (float)System.DateTime.Now.TimeOfDay.TotalSeconds)
            {
                LimitText.text = $"{timer - (float)System.DateTime.Now.TimeOfDay.TotalSeconds:N0}";
                await Task.Yield();
            }

            // TODO : 다음 퀴즈 전환 이펙트 넣기
        }




        // 결과 이동
        mLoading.SetActive(true);
        if (NetworkManager.Singleton.IsHost)
        {
            NetworkManager.Singleton.SceneManager.LoadScene("06 RESULT", LoadSceneMode.Single);
        }
    }



    public void OnResultClicked()
    {
        mResult.SetActive(false);
    }
}
