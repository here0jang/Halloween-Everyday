using UnityEngine;
using System.Threading.Tasks;
using MameshibaGames.Kekos.RuntimeExampleScene.Spawner;
using System.Collections.Generic;
using MameshibaGames.Common.Helpers;
using Unity.Netcode;

public class QuizManager : NetworkBehaviour
{
    private GameObject mLoading;
    public TMPro.TMP_Text LimitText;
    public TMPro.TMP_Text TopicText;
    public TMPro.TMP_Text nicNameText;
    public List<KeywordButton> KeywordButtons = new List<KeywordButton>();
    public GameObject Result;

    public CharacterSpawner CharacterSpawner;

    private static List<ulong> mConnectedClients = new List<ulong>();

    public int RelayCount = 0;

    // 릴레이 모드
    public TMPro.TMP_InputField AnswerInputField;






    public static QuizManager Instance { get; private set; }
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






    public static void QuizSceneInit(ulong id)
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

        Debug.Log("모든 플레이어 도착 " + mConnectedClients.Count);
        PlayerManager mPlayerManager = NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<PlayerManager>();
        //mPlayerManager.StartQuizClientRpc();
    }



    public async void StartQuizAsync()
    {
        TopicText.text = "주제 : " + LobbyManager.CurLobby.Data["Topic"].Value;
        mLoading = Instantiate(Resources.Load<GameObject>("Loading UI"));
        await Task.Delay(5000);
        mLoading.SetActive(false);


        for (int answerIndex = 0; answerIndex < NetworkManager.Singleton.ConnectedClients.Count; answerIndex++)
        {
            // 캐릭터
            //CharacterSpawner.SpawnSavedCharacter(NetworkManager.Singleton.ConnectedClientsList[answerIndex].PlayerObject.GetComponent<PlayerManager>().mCurStyle.Value.ToString());

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


        //SceneManager.LoadScene("06 RESULT");

    }









    public void OnResultClicked()
    {
        Result.SetActive(false);
    }
}
