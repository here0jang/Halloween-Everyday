using UnityEngine;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;
using MameshibaGames.Kekos.CharacterEditorScene.Customization;

public class OutfitManager : MonoBehaviour
{
    public const int OUTFIT_COUNT = 10;

    public TMPro.TMP_Text LimitText;
    public TMPro.TMP_Text NicNameText;
    public TMPro.TMP_Text TopicText;
    public TMPro.TMP_Text KeywordText;

    public CustomizationMediator CustomizationMediator;


    private async void Start()
    {
        GameObject loading = Instantiate(Resources.Load<GameObject>("Loading UI"));
        await Task.Delay(3000);
        loading.SetActive(false);

        int relayCount = PlayerPrefs.GetInt("RelayCount");
        relayCount++;
        if(relayCount > LobbyManager.CurLobby.Players.Count)
        {
            SceneManager.LoadScene("06 RESULT");
        }

        PlayerPrefs.SetInt("RelayCount", relayCount);


        // ����
        TopicText.text = LobbyManager.CurLobby.Data["Topic"].Value;


        // Ű����, �г���
        KeywordText.text = LobbyManager.CurLobby.Players[(PlayerPrefs.GetInt("FriendIndex") + 1) % LobbyManager.CurLobby.Players.Count].Data["Keyword_" + relayCount].Value;
        NicNameText.text = LobbyManager.CurLobby.Players[(PlayerPrefs.GetInt("FriendIndex") + 1) % LobbyManager.CurLobby.Players.Count].Data["Name"].Value + "�� ����!";

        // Ÿ�̸� ����
        float timer = (float)System.DateTime.Now.TimeOfDay.TotalSeconds +OUTFIT_COUNT; /* �ϵ���� �ð� �̿� */
        while (timer > (float)System.DateTime.Now.TimeOfDay.TotalSeconds)
        {
            LimitText.text = $"{timer - (float)System.DateTime.Now.TimeOfDay.TotalSeconds:N0}";
            await Task.Yield();
        }

        // ������ ��Ÿ�� �����ϰ� ����� �̵�
        loading.SetActive(true);
        string styleId = CustomizationMediator.SaveSettings(); /* 2048�ڱ����� ���� �����ؼ� ������ ���� */     
        bool updated = await LobbyManager.SetPlayerData("Style1_" + relayCount, styleId.Substring(0, styleId.Length / 2), "Style2_" + relayCount, styleId.Substring(styleId.Length / 2));             
        if(updated)
        {
            SceneManager.LoadScene("05 QUIZ");
        }
        else
        {
            Destroy(loading);
            // TODO : �÷��̾ �ذ��� �� �ִ� ������ ��� �ȳ� �޽��� �߰� (�����ڵ�� �з�)
            PopUpUIManager popUp = Instantiate(Resources.Load<PopUpUIManager>("PopUp UI"));
            popUp.InstantiatePopUp("��Ÿ�� ���� ����");
        }
    }
}
