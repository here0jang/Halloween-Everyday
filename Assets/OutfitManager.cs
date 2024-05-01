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


        // 주제
        TopicText.text = LobbyManager.CurLobby.Data["Topic"].Value;


        // 키워드, 닉네임
        KeywordText.text = LobbyManager.CurLobby.Players[(PlayerPrefs.GetInt("FriendIndex") + 1) % LobbyManager.CurLobby.Players.Count].Data["Keyword_" + relayCount].Value;
        NicNameText.text = LobbyManager.CurLobby.Players[(PlayerPrefs.GetInt("FriendIndex") + 1) % LobbyManager.CurLobby.Players.Count].Data["Name"].Value + "의 퀴즈!";

        // 타이머 시작
        float timer = (float)System.DateTime.Now.TimeOfDay.TotalSeconds +OUTFIT_COUNT; /* 하드웨어 시간 이용 */
        while (timer > (float)System.DateTime.Now.TimeOfDay.TotalSeconds)
        {
            LimitText.text = $"{timer - (float)System.DateTime.Now.TimeOfDay.TotalSeconds:N0}";
            await Task.Yield();
        }

        // 끝나면 스타일 저장하고 퀴즈로 이동
        loading.SetActive(true);
        string styleId = CustomizationMediator.SaveSettings(); /* 2048자까지만 저장 가능해서 반으로 나눔 */     
        bool updated = await LobbyManager.SetPlayerData("Style1_" + relayCount, styleId.Substring(0, styleId.Length / 2), "Style2_" + relayCount, styleId.Substring(styleId.Length / 2));             
        if(updated)
        {
            SceneManager.LoadScene("05 QUIZ");
        }
        else
        {
            Destroy(loading);
            // TODO : 플레이어가 해결할 수 있는 원인일 경우 안내 메시지 추가 (에러코드로 분류)
            PopUpUIManager popUp = Instantiate(Resources.Load<PopUpUIManager>("PopUp UI"));
            popUp.InstantiatePopUp("스타일 저장 실패");
        }
    }
}
