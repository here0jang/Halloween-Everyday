using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;
using MameshibaGames.Kekos.RuntimeExampleScene.Spawner;
using System.Collections.Generic;
using MameshibaGames.Common.Helpers;
using UnityEngine.SceneManagement;

public class QuizManager : MonoBehaviour
{


    private GameObject mLoading;
    private int mOutfitFinishedCount = 0;
    private float mUpdateTimer = 0f;

    public TMPro.TMP_Text LimitText;

    public TMPro.TMP_Text TopicText;
    public TMPro.TMP_Text nicNameText;

    public List<KeywordButton> KeywordButtons = new List<KeywordButton>();
    public GameObject Result;

    public CharacterSpawner CharacterSpawner;

    public int RelayCount = 0;


    // 릴레이 모드
    public TMPro.TMP_InputField AnswerInputField;


    void Start()
    {
        RelayCount = PlayerPrefs.GetInt("RelayCount");
        mLoading = Instantiate(Resources.Load<GameObject>("Loading UI"));
    }

    void Update()
    {
        if (LobbyManager.CurLobby != null && mLoading.activeSelf)
        {
            // Update Poll (Rate Limit)
            mUpdateTimer -= Time.deltaTime;
            if (mUpdateTimer < 0f)
            {
                mUpdateTimer = LobbyManager.LOBBY_UPDATE_TIMER_MAX;
            }


            // 플레이어들의 스타일 ID를 확인해서 모든 플레이어가 스타일 저장이 완료되었는지 확인
            for (int i = 0; i < LobbyManager.CurLobby.Players.Count; i++)
            {
                if (LobbyManager.CurLobby.Players[i].Data["Style1_" + RelayCount].Value != " ")
                {
                    mOutfitFinishedCount++;
                }
            }


            // 모든 플레이어가 스타일 저장이 완료되었다면 퀴즈 시작
            if (mOutfitFinishedCount == LobbyManager.CurLobby.Players.Count)
            {
                mLoading.SetActive(false);
                startQuizAsync();
            }
            else
            {
                mOutfitFinishedCount = 0;
            }
        }
    }

    private async void startQuizAsync()
    {
        TopicText.text = "주제 : " + LobbyManager.CurLobby.Data["Topic"].Value;


        if(LobbyManager.CurLobby.Data["GameMode"].Value == "Normal")
        {

            for (int answerIndex = 0; answerIndex < LobbyManager.CurLobby.Players.Count; answerIndex++)
            {
                // 닉네임
                nicNameText.text = LobbyManager.CurLobby.Players[answerIndex].Data["Name"].Value + "의 퀴즈!";

                // 캐릭터
                CharacterSpawner.SpawnSavedCharacter(LobbyManager.CurLobby.Players[answerIndex].Data["StyleId1"].Value + LobbyManager.CurLobby.Players[answerIndex].Data["StyleId2"].Value);

                // 현재 키워드
                string answer = LobbyManager.CurLobby.Players[answerIndex].Data["Keyword"].Value;

                // 보기 키워드 4개
                List<string> keywordList = new List<string>();

                // 4인 미만
                if (KeywordButtons.Count > LobbyManager.CurLobby.Players.Count)
                {
                    int i = 0;
                    for (i = 0; i < LobbyManager.CurLobby.Players.Count; i++)
                    {
                        keywordList.Add(LobbyManager.CurLobby.Players[i].Data["Keyword"].Value);
                    }


                    // TODO : 나중에 다른 방식으로 변경
                    for (int j = i; j < KeywordButtons.Count; j++)
                    {
                        keywordList.Add("");
                    }
                }
                // 4인 이상
                else
                {
                    // 전체 키워드 복사
                    List<string> tmpList = new List<string>();
                    for (int k = 0; k < LobbyManager.CurLobby.Players.Count; k++)
                    {
                        tmpList.Add(LobbyManager.CurLobby.Players[k].Data["Keyword"].Value);
                    }

                    // 정답 제거
                    tmpList.Remove(answer);

                    // 정답을 제외한 보기 키워드 랜덤으로 추가
                    for (int l = 0; l < KeywordButtons.Count - 1; l++)
                    {
                        string keyword = tmpList[Random.Range(0, tmpList.Count)];
                        keywordList.Add(keyword);
                        tmpList.Remove(keyword);
                    }

                    // 정답 추가
                    keywordList.Add(answer);
                }

                // 키워드 섞기
                keywordList.Shuffle();

                // 키워드 버튼에 뿌리기
                for (int index = 0; index < KeywordButtons.Count; index++)
                {
                    KeywordButtons[index].SetKeywordButton(keywordList[index], answer);
                }

                // 타이머 시작
                float timer = (float)System.DateTime.Now.TimeOfDay.TotalSeconds + LobbyManager.QUIZ_COUNT; /* 하드웨어 시간 이용 */
                while (timer > (float)System.DateTime.Now.TimeOfDay.TotalSeconds)
                {
                    LimitText.text = $"{timer - (float)System.DateTime.Now.TimeOfDay.TotalSeconds:N0}";
                    await Task.Yield();
                }

                // TODO : 다음 퀴즈 전환 이펙트 넣기
            }

            SceneManager.LoadScene("06 RESULT");
        }
        else
        {
            int relayCount = PlayerPrefs.GetInt("RelayCount");
            relayCount++;
            if (relayCount > LobbyManager.CurLobby.Players.Count)
            {
                SceneManager.LoadScene("06 RESULT");
            }



            int friendIndex = (PlayerPrefs.GetInt("FriendIndex") + 1) % LobbyManager.CurLobby.Players.Count;
            // 닉네임
            nicNameText.text = LobbyManager.CurLobby.Players[friendIndex].Data["Name"].Value + "의 퀴즈!";

            // 캐릭터
            CharacterSpawner.SpawnSavedCharacter(LobbyManager.CurLobby.Players[friendIndex].Data["Style1_" + (relayCount - 1)].Value + LobbyManager.CurLobby.Players[friendIndex].Data["Style2_" + (relayCount - 1)].Value);

            // 타이머 시작
            float timer = (float)System.DateTime.Now.TimeOfDay.TotalSeconds + LobbyManager.QUIZ_COUNT; /* 하드웨어 시간 이용 */
            while (timer > (float)System.DateTime.Now.TimeOfDay.TotalSeconds)
            {
                LimitText.text = $"{timer - (float)System.DateTime.Now.TimeOfDay.TotalSeconds:N0}";
                await Task.Yield();
            }

            bool IsSaved = await LobbyManager.SetPlayerData("Keyword_" + relayCount, AnswerInputField.text, "Style1_" + relayCount, " ", "Style2_" + relayCount, " ");
            if(IsSaved)
            {
                SceneManager.LoadScene("04 OUTFIT");
            }
            else
            {

            }
        }


    }

    public void OnResultClicked()
    {
        Result.SetActive(false);
    }
}
