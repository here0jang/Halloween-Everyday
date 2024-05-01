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


    // ������ ���
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


            // �÷��̾���� ��Ÿ�� ID�� Ȯ���ؼ� ��� �÷��̾ ��Ÿ�� ������ �Ϸ�Ǿ����� Ȯ��
            for (int i = 0; i < LobbyManager.CurLobby.Players.Count; i++)
            {
                if (LobbyManager.CurLobby.Players[i].Data["Style1_" + RelayCount].Value != " ")
                {
                    mOutfitFinishedCount++;
                }
            }


            // ��� �÷��̾ ��Ÿ�� ������ �Ϸ�Ǿ��ٸ� ���� ����
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
        TopicText.text = "���� : " + LobbyManager.CurLobby.Data["Topic"].Value;


        if(LobbyManager.CurLobby.Data["GameMode"].Value == "Normal")
        {

            for (int answerIndex = 0; answerIndex < LobbyManager.CurLobby.Players.Count; answerIndex++)
            {
                // �г���
                nicNameText.text = LobbyManager.CurLobby.Players[answerIndex].Data["Name"].Value + "�� ����!";

                // ĳ����
                CharacterSpawner.SpawnSavedCharacter(LobbyManager.CurLobby.Players[answerIndex].Data["StyleId1"].Value + LobbyManager.CurLobby.Players[answerIndex].Data["StyleId2"].Value);

                // ���� Ű����
                string answer = LobbyManager.CurLobby.Players[answerIndex].Data["Keyword"].Value;

                // ���� Ű���� 4��
                List<string> keywordList = new List<string>();

                // 4�� �̸�
                if (KeywordButtons.Count > LobbyManager.CurLobby.Players.Count)
                {
                    int i = 0;
                    for (i = 0; i < LobbyManager.CurLobby.Players.Count; i++)
                    {
                        keywordList.Add(LobbyManager.CurLobby.Players[i].Data["Keyword"].Value);
                    }


                    // TODO : ���߿� �ٸ� ������� ����
                    for (int j = i; j < KeywordButtons.Count; j++)
                    {
                        keywordList.Add("");
                    }
                }
                // 4�� �̻�
                else
                {
                    // ��ü Ű���� ����
                    List<string> tmpList = new List<string>();
                    for (int k = 0; k < LobbyManager.CurLobby.Players.Count; k++)
                    {
                        tmpList.Add(LobbyManager.CurLobby.Players[k].Data["Keyword"].Value);
                    }

                    // ���� ����
                    tmpList.Remove(answer);

                    // ������ ������ ���� Ű���� �������� �߰�
                    for (int l = 0; l < KeywordButtons.Count - 1; l++)
                    {
                        string keyword = tmpList[Random.Range(0, tmpList.Count)];
                        keywordList.Add(keyword);
                        tmpList.Remove(keyword);
                    }

                    // ���� �߰�
                    keywordList.Add(answer);
                }

                // Ű���� ����
                keywordList.Shuffle();

                // Ű���� ��ư�� �Ѹ���
                for (int index = 0; index < KeywordButtons.Count; index++)
                {
                    KeywordButtons[index].SetKeywordButton(keywordList[index], answer);
                }

                // Ÿ�̸� ����
                float timer = (float)System.DateTime.Now.TimeOfDay.TotalSeconds + LobbyManager.QUIZ_COUNT; /* �ϵ���� �ð� �̿� */
                while (timer > (float)System.DateTime.Now.TimeOfDay.TotalSeconds)
                {
                    LimitText.text = $"{timer - (float)System.DateTime.Now.TimeOfDay.TotalSeconds:N0}";
                    await Task.Yield();
                }

                // TODO : ���� ���� ��ȯ ����Ʈ �ֱ�
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
            // �г���
            nicNameText.text = LobbyManager.CurLobby.Players[friendIndex].Data["Name"].Value + "�� ����!";

            // ĳ����
            CharacterSpawner.SpawnSavedCharacter(LobbyManager.CurLobby.Players[friendIndex].Data["Style1_" + (relayCount - 1)].Value + LobbyManager.CurLobby.Players[friendIndex].Data["Style2_" + (relayCount - 1)].Value);

            // Ÿ�̸� ����
            float timer = (float)System.DateTime.Now.TimeOfDay.TotalSeconds + LobbyManager.QUIZ_COUNT; /* �ϵ���� �ð� �̿� */
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
