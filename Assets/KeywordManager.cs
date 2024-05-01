using UnityEngine;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;
using Unity.Services.Authentication;

public class KeywordManager : MonoBehaviour
{
    public TMPro.TMP_Text LimitText;
    public TMPro.TMP_Text TopicText;

    public TMPro.TMP_InputField KeywordInputField;

    private async void Start()
    {
        string myId = AuthenticationService.Instance.PlayerId;
        int i;
        for (i = 0; i < LobbyManager.CurLobby.Players.Count; i++)
        {
            if (LobbyManager.CurLobby.Players[i].Id == myId)
            {
                break;
            }
        }

        PlayerPrefs.SetInt("MyIndex", i);
        PlayerPrefs.SetInt("FriendIndex", i);
        PlayerPrefs.SetInt("RelayCount", 0);

        // ����
        TopicText.text = LobbyManager.CurLobby.Data["Topic"].Value;

        // Ÿ�̸�
        float timer = (float)System.DateTime.Now.TimeOfDay.TotalSeconds + LobbyManager.KEYWORD_COUNT; /* �ϵ���� �ð� �̿� */
        while (timer > (float)System.DateTime.Now.TimeOfDay.TotalSeconds)
        {
            LimitText.text = $"{timer - (float)System.DateTime.Now.TimeOfDay.TotalSeconds:N0}";
            await Task.Yield();
        }

        // �ε�
        GameObject loading = Instantiate(Resources.Load<GameObject>("Loading UI"));

        bool keywordSent = await LobbyManager.SetPlayerData("Keyword_1", KeywordInputField.text, "Style1_1", " ", "Style2_1", " ");
        if (keywordSent)
        {
            SceneManager.LoadScene("04 OUTFIT");
        }
        else
        {
            Destroy(loading);

            // TODO : �÷��̾ �ذ��� �� �ִ� ������ ��� �ȳ� �޽��� �߰� (�����ڵ�� �з�)
            PopUpUIManager popUp = Instantiate(Resources.Load<PopUpUIManager>("PopUp UI"));
            popUp.InstantiatePopUp("Ű���� ���� ����");
        }
    }
}
