using UnityEngine;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;
using MameshibaGames.Kekos.CharacterEditorScene.Customization;

public class OutfitManager : MonoBehaviour
{
    public const int OUTFIT_COUNT = 3;

    public TMPro.TMP_Text LimitText;
    public TMPro.TMP_Text TopicText;
    public TMPro.TMP_Text KeywordText;

    public CustomizationMediator CustomizationMediator;


    private async void Start()
    {
        // ����, Ű���� �ֱ�
        if(LobbyManager.CurLobby != null)
        {
            TopicText.text = LobbyManager.CurLobby.Data["Topic"].Value;
        }

        KeywordText.text = PlayerPrefs.GetString("Keyword");


        // Ÿ�̸� ����
        float timer = (float)System.DateTime.Now.TimeOfDay.TotalSeconds +OUTFIT_COUNT; /* �ϵ���� �ð� �̿� */
        while (timer > (float)System.DateTime.Now.TimeOfDay.TotalSeconds)
        {
            LimitText.text = $"{timer - (float)System.DateTime.Now.TimeOfDay.TotalSeconds:N0}";
            await Task.Yield();
        }


        // ������ ��Ÿ�� �����ϰ� ����� �̵�
        GameObject loading = Instantiate(Resources.Load<GameObject>("Loading UI"));

        string styleId = CustomizationMediator.SaveSettings(); /* 2048�ڱ����� ���� �����ؼ� ������ ���� */     
        bool updated = await LobbyManager.UpdatePlayerStyleData(styleId.Substring(0, styleId.Length / 2), styleId.Substring(styleId.Length / 2));
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
