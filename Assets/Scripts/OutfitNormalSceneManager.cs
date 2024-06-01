using MameshibaGames.Kekos.CharacterEditorScene.Customization;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Netcode;
using System;


public class OutfitNormalSceneManager : NetworkBehaviour
{
    [SerializeField] private TMPro.TMP_Text mLimitText;
    //[SerializeField] private TMPro.TMP_Text mNicNameText;
    [SerializeField] private TMPro.TMP_Text mKeywordText;
    [SerializeField] private Animator mKeywordAnimator;
    [SerializeField] private GameObject mLoading;

    [SerializeField] private CustomizationMediator mCustomizationMediator;

    private List<ulong> mConnectedClients = new List<ulong>();

    [SerializeField] private ItemDatabase mItemDatabase;
    [SerializeField] private SkinnedMeshRenderer skinnedMesh;
    public NetworkVariable<int> HairIndex;



    public static OutfitNormalSceneManager Instance { get; private set; }

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

    public override void OnNetworkSpawn()
    {
        HairIndex.OnValueChanged += (int prevValue, int newValue) => 
        {
            skinnedMesh.sharedMesh = mItemDatabase.mDatas[0].meshes[newValue];
        };
    }



    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            int index = UnityEngine.Random.Range(0, 3);
            changeHairServerRpc(index);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void changeHairServerRpc(int index)
    {
        HairIndex.Value = index;
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

        StartOutFitClientRpc();
    }

    [ClientRpc]
    private void StartOutFitClientRpc()
    {
        startOutfitAsync();
    }
            
    private async void startOutfitAsync()
    {
        // 키워드, 닉네임
        PlayerManager mPlayerManager = NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<PlayerManager>();
        //mPlayerManager.GetKeywordServerRpc(mPlayerManager.mCurIndex.Value);
        string keyword = MultiplayerManager.Instance.GetKeyword(mPlayerManager.mCurIndex.Value);

        await Task.Delay(2000);
        mLoading.SetActive(false);
        mKeywordAnimator.SetBool("Entered", true);
        mKeywordText.text = "<color=#6D60CC>" + keyword + "</color>로 꾸미세요!";
        //NicNameText.text = mPlayerManager.mCurNicName.Value + "<color=#6D60CC>의 퀴즈</color>";


        // 타이머
        float timer = (float)System.DateTime.Now.TimeOfDay.TotalSeconds + LobbyManager.OUTFIT_COUNT; /* 하드웨어 시간 이용 */
        while (timer > (float)System.DateTime.Now.TimeOfDay.TotalSeconds)
        {
            mLimitText.text = $"{timer - (float)System.DateTime.Now.TimeOfDay.TotalSeconds:N0}";
            await Task.Yield();
        }


        // 스타일 저장
        mLoading.SetActive(true);
        string styleId = mCustomizationMediator.SaveSettings();
        mPlayerManager.SetCurStyleServerRpc(styleId);


        // 퀴즈 이동
        if (NetworkManager.Singleton.IsHost)
        {
            NetworkManager.Singleton.SceneManager.LoadScene("05 QUIZ", LoadSceneMode.Single);
        }
    }
}
