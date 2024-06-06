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

    // ������ ���
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

        Debug.Log("��� �÷��̾� ���� " + mConnectedClients.Count);
        PlayerManager mPlayerManager = NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<PlayerManager>();
        //mPlayerManager.StartQuizClientRpc();
    }



    public async void StartQuizAsync()
    {
        TopicText.text = "���� : " + LobbyManager.CurLobby.Data["Topic"].Value;
        mLoading = Instantiate(Resources.Load<GameObject>("Loading UI"));
        await Task.Delay(5000);
        mLoading.SetActive(false);


        for (int answerIndex = 0; answerIndex < NetworkManager.Singleton.ConnectedClients.Count; answerIndex++)
        {
            // ĳ����
            //CharacterSpawner.SpawnSavedCharacter(NetworkManager.Singleton.ConnectedClientsList[answerIndex].PlayerObject.GetComponent<PlayerManager>().mCurStyle.Value.ToString());

            // ���� Ű����
            //string answer = NetworkManager.Singleton.ConnectedClientsList[answerIndex].PlayerObject.GetComponent<PlayerManager>().mCurKeyword.Value.ToString();

            // ���� 4��
            List<string> keywordList = new List<string>();
            if (KeywordButtons.Count > NetworkManager.Singleton.ConnectedClients.Count)
            {
                int i = 0;
                for (i = 0; i < NetworkManager.Singleton.ConnectedClients.Count; i++)
                {
                    //keywordList.Add(NetworkManager.Singleton.ConnectedClientsList[i].PlayerObject.GetComponent<PlayerManager>().mCurKeyword.Value.ToString());
                }

                // TODO : ���߿� �ٸ� ������� ����
                for (int j = i; j < KeywordButtons.Count; j++)
                {
                    keywordList.Add("");
                }
            }
            else
            {
                // ��ü Ű���� ����
                List<string> tmpList = new List<string>();
                for (int k = 0; k < NetworkManager.Singleton.ConnectedClients.Count; k++)
                {
                    //tmpList.Add(NetworkManager.Singleton.ConnectedClientsList[k].PlayerObject.GetComponent<PlayerManager>().mCurKeyword.Value.ToString());
                }

                // ���� ����
                //tmpList.Remove(answer);

                // ������ ������ ���� Ű���� �������� �߰�
                for (int l = 0; l < KeywordButtons.Count - 1; l++)
                {
                    string keyword = tmpList[Random.Range(0, tmpList.Count)];
                    keywordList.Add(keyword);
                    tmpList.Remove(keyword);
                }

                // ���� �߰�
                //keywordList.Add(answer);
            }

            // Ű���� ����
            keywordList.Shuffle();

            // Ű���� ��ư�� �Ѹ���
            for (int index = 0; index < KeywordButtons.Count; index++)
            {
                //KeywordButtons[index].SetKeywordButton(keywordList[index], answer);
            }



            // Ÿ�̸� ����
            float timer = (float)System.DateTime.Now.TimeOfDay.TotalSeconds + GameCount.QUIZ_COUNT; /* �ϵ���� �ð� �̿� */
            while (timer > (float)System.DateTime.Now.TimeOfDay.TotalSeconds)
            {
                LimitText.text = $"{timer - (float)System.DateTime.Now.TimeOfDay.TotalSeconds:N0}";
                await Task.Yield();
            }

            // TODO : ���� ���� ��ȯ ����Ʈ �ֱ�
        }


        //SceneManager.LoadScene("06 RESULT");

    }









    public void OnResultClicked()
    {
        Result.SetActive(false);
    }
}
