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
        // 주제, 키워드 넣기
        if(LobbyManager.CurLobby != null)
        {
            TopicText.text = LobbyManager.CurLobby.Data["Topic"].Value;
        }

        KeywordText.text = PlayerPrefs.GetString("Keyword");


        // 타이머 시작
        float timer = (float)System.DateTime.Now.TimeOfDay.TotalSeconds +OUTFIT_COUNT; /* 하드웨어 시간 이용 */
        while (timer > (float)System.DateTime.Now.TimeOfDay.TotalSeconds)
        {
            LimitText.text = $"{timer - (float)System.DateTime.Now.TimeOfDay.TotalSeconds:N0}";
            await Task.Yield();
        }


        // 끝나면 스타일 저장하고 퀴즈로 이동
        GameObject loading = Instantiate(Resources.Load<GameObject>("Loading UI"));

        string styleId = CustomizationMediator.SaveSettings(); /* 2048자까지만 저장 가능해서 반으로 나눔 */     
        bool updated = await LobbyManager.UpdatePlayerStyleData(styleId.Substring(0, styleId.Length / 2), styleId.Substring(styleId.Length / 2));
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
