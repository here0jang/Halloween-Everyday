//using MameshibaGames.Kekos.CharacterEditorScene.Customization;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using System.Threading.Tasks;

public class OutfitTogetherSceneManager : NetworkBehaviour
{
    [SerializeField] private TMPro.TMP_Text mTimerText;
    [SerializeField] private TMPro.TMP_Text mKeywordText;
    //[SerializeField] private TMPro.TMP_Text mDescText;
    [SerializeField] private GameObject mLoading;

    //[SerializeField] private CustomizationMediator mCustomizationMediator;

    private List<ulong> mConnectedClients = new List<ulong>();


    public static OutfitTogetherSceneManager Instance { get; private set; }
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



    public void OutfitSceneInit(ulong id)
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

        Debug.Log("모든 플레이어 도착 " + mConnectedClients.Count);
        StartOutFitClientRpc();
    }

    [ClientRpc]
    private void StartOutFitClientRpc()
    {
        startOutfitAsync();
    }

    private async void startOutfitAsync()
    {
        // 로딩
        mLoading.SetActive(false);


        // 첫번째 키워드 가져오기
        mKeywordText.text = "머리는 내가 할게";
        PlayerManager mPlayerManager = NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<PlayerManager>();
        //mPlayerManager.GetKeywordServerRpc();
        //await Task.Delay(3000);
        //mKeywordText.text = "<color=#6D60CC>" + mPlayerManager.mCurKeyword.Value + "</color>로 꾸미세요!";
        //mDescText.text = "머리는 내가 할게";


        // 타이머 시작
        float timer = (float)System.DateTime.Now.TimeOfDay.TotalSeconds + GameCount.OUTFIT_COUNT; /* 하드웨어 시간 이용 */
        while (timer > (float)System.DateTime.Now.TimeOfDay.TotalSeconds)
        {
            mTimerText.text = $"{timer - (float)System.DateTime.Now.TimeOfDay.TotalSeconds:N0}";
            await Task.Yield();
        }

        // 로딩
        mLoading.SetActive(true);


        // 머리 스타일 저장
        //string styleId = mCustomizationMediator.SaveSettings();
        //mPlayerManager.SetCurStyleServerRpc(styleId);

        // 로딩
        mLoading.SetActive(false);


        // 두번째 키워드 & 스타일 가져오기
        mKeywordText.text = "옷은 내가 입힐게";
        //mPlayerManager.GetKeywordServerRpc();
        //mPlayerManager.GetStyleServerRpc();
        //await Task.Delay(3000);
        //mKeywordText.text = "<color=#6D60CC>" + mPlayerManager.mCurKeyword.Value + "</color>로 꾸미세요!";
        //mCustomizationMediator.LoadSettings(mPlayerManager.mCurStyle.Value.ToString());
        //mDescText.text = "옷은 내가 입힐게";

        // 머리 버튼 막기




        // 타이머 시작
        timer = (float)System.DateTime.Now.TimeOfDay.TotalSeconds + GameCount.OUTFIT_COUNT; /* 하드웨어 시간 이용 */
        while (timer > (float)System.DateTime.Now.TimeOfDay.TotalSeconds)
        {
            mTimerText.text = $"{timer - (float)System.DateTime.Now.TimeOfDay.TotalSeconds:N0}";
            await Task.Yield();
        }

        // 두번째 스타일 저장

        // 퀴즈 이동
    }
}
