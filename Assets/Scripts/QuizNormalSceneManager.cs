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
        mTopicText.text = "���� : " + LobbyManager.CurLobby.Data["Topic"].Value;
        mLoading.SetActive(false);




        for (int answerIndex = 0; answerIndex < NetworkManager.Singleton.ConnectedClients.Count; answerIndex++)
        {
            // ĳ����
            //string styleId = NetworkManager.Singleton.ConnectedClientsList[answerIndex].PlayerObject.GetComponent<PlayerManager>().mCurStyle.Value.ToString();
            //string styleId = NetworkManager.Singleton.ConnectedClientsList[answerIndex].PlayerObject.GetComponent<PlayerManager>().mMyStyle.Value.ToString();
            //await Task.Delay(2000);
            //mCharacterSpawner.SpawnSavedCharacter(styleId);

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




        // ��� �̵�
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
