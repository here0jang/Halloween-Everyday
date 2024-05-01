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
        // 로딩
        GameObject loading = Instantiate(Resources.Load<GameObject>("Loading UI"));

        int relayCount = PlayerPrefs.GetInt("RelayCount");

        // 퀴즈가 모두 끝났으면 결과 화면으로 이동
        if(relayCount >=  LobbyManager.CurLobby.Players.Count)
        {
            SceneManager.LoadScene("06 RESULT");
        }
        relayCount++;
        PlayerPrefs.SetInt("RelayCount", relayCount);

        await Task.Delay(3000);

        // 키워드, 닉네임
        KeywordText.text = "<color=#6D60CC>" + LobbyManager.CurLobby.Players[(PlayerPrefs.GetInt("FriendIndex") + 1) % LobbyManager.CurLobby.Players.Count].Data["Keyword_" + relayCount].Value + "</color>로 꾸미세요!";
        NicNameText.text = LobbyManager.CurLobby.Players[(PlayerPrefs.GetInt("FriendIndex") + 1) % LobbyManager.CurLobby.Players.Count].Data["Name"].Value + "<color=#6D60CC>의 퀴즈</color>";

        loading.SetActive(false);

        // 타이머 시작
        float timer = (float)System.DateTime.Now.TimeOfDay.TotalSeconds +LobbyManager.OUTFIT_COUNT; /* 하드웨어 시간 이용 */
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
