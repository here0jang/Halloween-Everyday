using UnityEngine;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;
using MameshibaGames.Kekos.CharacterEditorScene.Customization;

public class OutfitManager : MonoBehaviour
{
    public TMPro.TMP_Text LimitText;
    public TMPro.TMP_Text NicNameText;
    public TMPro.TMP_Text KeywordText;

    public CustomizationMediator CustomizationMediator;


    private async void Start()
    {
        // �ε�
        GameObject loading = Instantiate(Resources.Load<GameObject>("Loading UI"));

        int relayCount = PlayerPrefs.GetInt("RelayCount");

        // ��� ��� �������� ��� ȭ������ �̵�
        if(relayCount >=  LobbyManager.CurLobby.Players.Count)
        {
            SceneManager.LoadScene("06 RESULT");
        }
        relayCount++;
        PlayerPrefs.SetInt("RelayCount", relayCount);

        await Task.Delay(3000);

        // Ű����, �г���
        KeywordText.text = "<color=#6D60CC>" + LobbyManager.CurLobby.Players[(PlayerPrefs.GetInt("FriendIndex") + 1) % LobbyManager.CurLobby.Players.Count].Data["Keyword_" + relayCount].Value + "</color>�� �ٹ̼���!";
        NicNameText.text = LobbyManager.CurLobby.Players[(PlayerPrefs.GetInt("FriendIndex") + 1) % LobbyManager.CurLobby.Players.Count].Data["Name"].Value + "<color=#6D60CC>�� ����</color>";

        loading.SetActive(false);

        // Ÿ�̸� ����
        float timer = (float)System.DateTime.Now.TimeOfDay.TotalSeconds +LobbyManager.OUTFIT_COUNT; /* �ϵ���� �ð� �̿� */
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
