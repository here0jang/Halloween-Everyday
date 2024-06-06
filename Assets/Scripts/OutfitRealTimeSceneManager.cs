using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Netcode;
using UnityEngine.UI;
using System;

public class OutfitRealTimeSceneManager : NetworkBehaviour
{
    public int mHairIndex;
    public NetworkVariable<int> HairIndex;
    public NetworkVariable<int> BodyIndex;
    public NetworkVariable<int> EyebrowIndex;
    public NetworkVariable<int> MustacheIndex;
    public NetworkVariable<int> GlassesIndex;
    public NetworkVariable<int> HatIndex;

    public NetworkVariable<int> OuterwearIndex;
    public NetworkVariable<int> PantsIndex;
    public NetworkVariable<int> ShoeIndex;
    public NetworkVariable<int> GloveIndex;
    public NetworkVariable<int> BackpackIndex;
    public NetworkVariable<int> FullbodyIndex;



    [SerializeField] private TMPro.TMP_Text mLimitText;
    [SerializeField] private TMPro.TMP_Text mKeywordText;

    [SerializeField] private GameObject mLoading;

    private List<ulong> mConnectedClients = new List<ulong>();


    public Button itemButton;

    [SerializeField] private Button mHeadMainButton;
    [SerializeField] private Button mBodyMainButton;

    [SerializeField] private GameObject mHeadSide;
    [SerializeField] private GameObject mBodySide;

    [SerializeField] private ItemDatabase[] mItemDatabases = new ItemDatabase[(int)EItems.last];
    [SerializeField] private SkinnedMeshRenderer[] mSkinMeshRenderers = new SkinnedMeshRenderer[(int)EItems.last];
    [SerializeField] private GameObject[] mItemScrolls = new GameObject[(int)EItems.last];
    [SerializeField] private Transform[] mContentTransforms = new Transform[(int)EItems.last];
    [SerializeField] private Button[] mItemButtons = new Button[(int)EItems.last];
    [SerializeField] private Button[] mRemoveItemButtons = new Button[(int)EItems.last];




    public static OutfitRealTimeSceneManager Instance { get; private set; }

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

        mHeadMainButton.onClick.RemoveAllListeners();
        mHeadMainButton.onClick.AddListener(() =>
        {
            mHeadSide.SetActive(true);
            mBodySide.SetActive(false);
            onBodySideManuClicked(-1);
            onHeadSideManuClicked(0);
        });

        mBodyMainButton.onClick.RemoveAllListeners();
        mBodyMainButton.onClick.AddListener(() =>
        {
            mHeadSide.SetActive(false);
            mBodySide.SetActive(true);
            onHeadSideManuClicked(-1);
            onBodySideManuClicked(6);
        });

        for (int i = 0; i < (int)EItems.last; i++)
        {
            int tmp = i;
            mRemoveItemButtons[tmp].onClick.RemoveAllListeners();
            mRemoveItemButtons[tmp].onClick.AddListener(() => { onRemoveItemClicked(tmp); });
        }

        for (int i = 0; i < (int)EItems.outerwear; i++)
        {
            int tmp = i;
            initItemButtons(tmp, mItemDatabases[tmp], mContentTransforms[tmp], changeItemServerRpc);
            mItemButtons[tmp].onClick.RemoveAllListeners();
            mItemButtons[tmp].onClick.AddListener(() => { onHeadSideManuClicked(tmp); });
        }

        for (int i = (int)EItems.outerwear; i < (int)EItems.last; i++)
        {
            int tmp = i;
            initItemButtons(tmp, mItemDatabases[tmp], mContentTransforms[tmp], changeItemServerRpc);
            mItemButtons[tmp].onClick.RemoveAllListeners();
            mItemButtons[tmp].onClick.AddListener(() => { onBodySideManuClicked(tmp); });
        }



        onHeadSideManuClicked(0);
        onBodySideManuClicked(-1);
        mHeadSide.SetActive(true);
        mBodySide.SetActive(false);
    }

    private void onRemoveItemClicked(int index)
    {
        mSkinMeshRenderers[index].sharedMesh = null;
    }

    private void onHeadSideManuClicked(int index)
    {
        mItemScrolls[0].SetActive(index == 0);
        mItemScrolls[1].SetActive(index == 1);
        mItemScrolls[2].SetActive(index == 2);
        mItemScrolls[3].SetActive(index == 3);
        mItemScrolls[4].SetActive(index == 4);
        mItemScrolls[5].SetActive(index == 5);
    }

    private void onBodySideManuClicked(int index)
    {
        mItemScrolls[6].SetActive(index == 6);
        mItemScrolls[7].SetActive(index == 7);
        mItemScrolls[8].SetActive(index == 8);
        mItemScrolls[9].SetActive(index == 9);
        mItemScrolls[10].SetActive(index == 10);
        mItemScrolls[11].SetActive(index == 11);
    }


    private void initItemButtons(int category, ItemDatabase database, Transform contentTransform, Action<int, int> action)
    {
        for (int i = 0; i < database.mDatas.Count; i++)
        {
            int tmp = i;
            Button button = Instantiate(itemButton, contentTransform.transform);
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() =>
            {
                action(category, tmp);
            });
            button.image.sprite = database.mDatas[i].mSprite;
        }
    }

    public override void OnNetworkSpawn()
    {
        HairIndex.OnValueChanged += (int prevValue, int newValue) =>
        {
            if (mSkinMeshRenderers[(int)EItems.hat].sharedMesh != null)
            {
                mSkinMeshRenderers[(int)EItems.hat].sharedMesh = null;
            }
            mSkinMeshRenderers[(int)EItems.hair].sharedMesh = mItemDatabases[(int)EItems.hair].mDatas[newValue].mMesh;
        };

        BodyIndex.OnValueChanged += (int prevValue, int newValue) =>
        {
            mSkinMeshRenderers[(int)EItems.body].sharedMesh = mItemDatabases[(int)EItems.body].mDatas[newValue].mMesh;
        };

        EyebrowIndex.OnValueChanged += (int prevValue, int newValue) =>
        {
            mSkinMeshRenderers[(int)EItems.eyebrow].sharedMesh = mItemDatabases[(int)EItems.eyebrow].mDatas[newValue].mMesh;
        };

        MustacheIndex.OnValueChanged += (int prevValue, int newValue) =>
        {
            mSkinMeshRenderers[(int)EItems.mustache].sharedMesh = mItemDatabases[(int)EItems.mustache].mDatas[newValue].mMesh;
        };

        GlassesIndex.OnValueChanged += (int prevValue, int newValue) =>
        {
            mSkinMeshRenderers[(int)EItems.glasses].sharedMesh = mItemDatabases[(int)EItems.glasses].mDatas[newValue].mMesh;
        };

        HatIndex.OnValueChanged += (int prevValue, int newValue) =>
        {
            if (mSkinMeshRenderers[(int)EItems.hair].sharedMesh != null)
            {
                mSkinMeshRenderers[(int)EItems.hair].sharedMesh = null;
            }
            mSkinMeshRenderers[(int)EItems.hat].sharedMesh = mItemDatabases[(int)EItems.hat].mDatas[newValue].mMesh;
        };

        OuterwearIndex.OnValueChanged += (int prevValue, int newValue) =>
        {
            if (mSkinMeshRenderers[(int)EItems.fullBody].sharedMesh != null)
            {
                mSkinMeshRenderers[(int)EItems.fullBody].sharedMesh = null;
            }
            mSkinMeshRenderers[(int)EItems.outerwear].sharedMesh = mItemDatabases[(int)EItems.outerwear].mDatas[newValue].mMesh;
        };

        PantsIndex.OnValueChanged += (int prevValue, int newValue) =>
        {
            mSkinMeshRenderers[(int)EItems.pants].sharedMesh = mItemDatabases[(int)EItems.pants].mDatas[newValue].mMesh;
        };

        ShoeIndex.OnValueChanged += (int prevValue, int newValue) =>
        {
            mSkinMeshRenderers[(int)EItems.shoe].sharedMesh = mItemDatabases[(int)EItems.shoe].mDatas[newValue].mMesh;
        };

        GloveIndex.OnValueChanged += (int prevValue, int newValue) =>
        {
            mSkinMeshRenderers[(int)EItems.glove].sharedMesh = mItemDatabases[(int)EItems.glove].mDatas[newValue].mMesh;
        };

        BackpackIndex.OnValueChanged += (int prevValue, int newValue) =>
        {
            mSkinMeshRenderers[(int)EItems.backpack].sharedMesh = mItemDatabases[(int)EItems.backpack].mDatas[newValue].mMesh;
        };

        FullbodyIndex.OnValueChanged += (int prevValue, int newValue) =>
        {
            if (mSkinMeshRenderers[(int)EItems.outerwear].sharedMesh != null)
            {
                mSkinMeshRenderers[(int)EItems.outerwear].sharedMesh = null;
            }
            mSkinMeshRenderers[(int)EItems.fullBody].sharedMesh = mItemDatabases[(int)EItems.fullBody].mDatas[newValue].mMesh;
        };
    }

    [ServerRpc(RequireOwnership = false)]
    private void changeItemServerRpc(int category, int index)
    {
        switch (category)
        {
            case (int)EItems.hair:
                {
                    HairIndex.Value = index;
                    break;
                }
            case (int)EItems.body:
                {
                    BodyIndex.Value = index;
                    break;
                }
            case (int)EItems.eyebrow:
                {
                    EyebrowIndex.Value = index;
                    break;
                }
            case (int)EItems.mustache:
                {
                    MustacheIndex.Value = index;
                    break;
                }
            case (int)EItems.glasses:
                {
                    GlassesIndex.Value = index;
                    break;
                }
            case (int)EItems.hat:
                {
                    HatIndex.Value = index;
                    break;
                }
            case (int)EItems.outerwear:
                {
                    OuterwearIndex.Value = index;
                    break;
                }
            case (int)EItems.pants:
                {
                    PantsIndex.Value = index;
                    break;
                }
            case (int)EItems.shoe:
                {
                    ShoeIndex.Value = index;
                    break;
                }
            case (int)EItems.glove:
                {
                    GloveIndex.Value = index;
                    break;
                }
            case (int)EItems.backpack:
                {
                    BackpackIndex.Value = index;
                    break;
                }
            case (int)EItems.fullBody:
                {
                    FullbodyIndex.Value = index;
                    break;
                }
            default:
                {
                    break;
                }
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
        mPlayerManager.mCurIndex.Value = (mPlayerManager.mCurIndex.Value + 1) % MultiplayerManager.Instance.mConnectedCount.Value;
        string keyword = MultiplayerManager.Instance.GetAnswer(mPlayerManager.mCurIndex.Value);
        await Task.Delay(2000);
        mLoading.SetActive(false);
        mKeywordText.text = "<color=#6D60CC>" + keyword + "</color>로 꾸미세요!";


        #region Timer
        // 타이머
        float timer = (float)System.DateTime.Now.TimeOfDay.TotalSeconds + GameCount.OUTFIT_COUNT; /* 하드웨어 시간 이용 */
        while (timer > (float)System.DateTime.Now.TimeOfDay.TotalSeconds)
        {
            mLimitText.text = $"{timer - (float)System.DateTime.Now.TimeOfDay.TotalSeconds:N0}";
            await Task.Yield();
        }
        #endregion


        // 스타일 저장
        mLoading.SetActive(true);
        //MultiplayerManager.Instance.AddStyleServerRpc((ulong)mPlayerManager.mCurIndex.Value, (ulong)mPlayerManager.mMyIndex.Value, mHairIndex);


        // 퀴즈 이동
        if (NetworkManager.Singleton.IsHost)
        {
            NetworkManager.Singleton.SceneManager.LoadScene(SceneName.GUESS_SCENE, LoadSceneMode.Single);
        }
    }
}
