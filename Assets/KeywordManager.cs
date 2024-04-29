using UnityEngine;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;

public class KeywordManager : MonoBehaviour
{
    public const int KEYWORD_COUNT = 3;

    public TMPro.TMP_Text LimitText;
    public TMPro.TMP_Text TopicText;

    public TMPro.TMP_InputField KeywordInputField;

    private async void Start()
    {
        if(LobbyManager.CurLobby != null)
        {
            TopicText.text = LobbyManager.CurLobby.Data["Topic"].Value;

            float timer = (float)System.DateTime.Now.TimeOfDay.TotalSeconds + KEYWORD_COUNT; /* �ϵ���� �ð� �̿� */
            while (timer > (float)System.DateTime.Now.TimeOfDay.TotalSeconds)
            {
                LimitText.text = $"{timer - (float)System.DateTime.Now.TimeOfDay.TotalSeconds:N0}";
                await Task.Yield();
            }


            GameObject loading = Instantiate(Resources.Load<GameObject>("Loading UI"));

            bool keywordSent = await LobbyManager.SetPlayerKeywordData(KeywordInputField.text);
            if(keywordSent)
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
}