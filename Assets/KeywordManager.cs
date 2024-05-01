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

        // 주제
        TopicText.text = LobbyManager.CurLobby.Data["Topic"].Value;

        // 타이머
        float timer = (float)System.DateTime.Now.TimeOfDay.TotalSeconds + LobbyManager.KEYWORD_COUNT; /* 하드웨어 시간 이용 */
        while (timer > (float)System.DateTime.Now.TimeOfDay.TotalSeconds)
        {
            LimitText.text = $"{timer - (float)System.DateTime.Now.TimeOfDay.TotalSeconds:N0}";
            await Task.Yield();
        }

        // 로딩
        GameObject loading = Instantiate(Resources.Load<GameObject>("Loading UI"));

        bool keywordSent = await LobbyManager.SetPlayerData("Keyword_1", KeywordInputField.text, "Style1_1", " ", "Style2_1", " ");
        if (keywordSent)
        {
            SceneManager.LoadScene("04 OUTFIT");
        }
        else
        {
            Destroy(loading);

            // TODO : 플레이어가 해결할 수 있는 원인일 경우 안내 메시지 추가 (에러코드로 분류)
            PopUpUIManager popUp = Instantiate(Resources.Load<PopUpUIManager>("PopUp UI"));
            popUp.InstantiatePopUp("키워드 전송 실패");
        }
    }
}
