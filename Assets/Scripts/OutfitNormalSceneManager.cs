using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Netcode;
using UnityEngine.UI;
using System;
using System.Collections;

public class OutfitNormalSceneManager : NetworkBehaviour
{
    [SerializeField] private TMPro.TMP_Text mLimitText;
    [SerializeField] private TMPro.TMP_Text mKeywordText;

    [SerializeField] private GameObject mLoading;

    private List<ulong> mConnectedClients = new List<ulong>();


    public Button itemButton;

    [SerializeField] private Button mHeadMainButton;
    [SerializeField] private Button mBodyMainButton;

    [SerializeField] private GameObject mHeadSide;
    [SerializeField] private GameObject mBodySide;

    [SerializeField] private ItemDatabase[] mItemDatabases                           = new ItemDatabase[(int)EItems.last];
    [SerializeField] private SkinnedMeshRenderer[] mSkinMeshRenderers  = new SkinnedMeshRenderer[(int)EItems.last];
    [SerializeField] private GameObject[] mItemScrolls                                     = new GameObject[(int)EItems.last];
    [SerializeField] private Transform[] mContentTransforms                          = new Transform[(int)EItems.last];
    [SerializeField] private Button[] mItemButtons                                             = new Button[(int)EItems.last];
    [SerializeField] private Button[] mRemoveItemButtons                              = new Button[(int)EItems.last];

    public int[] mItemIndexs                                                                                    = new int[(int)EItems.last];
    private bool[] mIsButtonInited                                                                          = new bool[(int)EItems.last];

    PlayerManager mPlayerManager;
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
            mRemoveItemButtons[tmp].onClick.AddListener(() =>
            {
                mSkinMeshRenderers[tmp].sharedMesh = null;
                mItemIndexs[tmp] = -1;
            });
        }



        for (int i = 0; i < (int)EItems.outerwear; i++)
        {
            int tmp  = i;

            mItemButtons[tmp].onClick.RemoveAllListeners();
            mItemButtons[tmp].onClick.AddListener(() => { onHeadSideManuClicked(tmp); });
        }

        for (int i = (int)EItems.outerwear; i < (int)EItems.last; i++)
        {
            int tmp = i;
            //initItemButtons(tmp, mItemDatabases[tmp], mContentTransforms[tmp], changeItem);
            mItemButtons[tmp].onClick.RemoveAllListeners();
            mItemButtons[tmp].onClick.AddListener(() => { onBodySideManuClicked(tmp); });
        }

        onHeadSideManuClicked(0);
        onBodySideManuClicked(-1);
        mHeadSide.SetActive(true);
        mBodySide.SetActive(false);
    }

    private void changeItem(int category, int index)
    {
        switch(category)
        {
            case (int)EItems.hair:
                {
                    if (mSkinMeshRenderers[(int)EItems.hat].sharedMesh != null)
                    {
                        mSkinMeshRenderers[(int)EItems.hat].sharedMesh = null;
                    }
                    break;
                }
            case (int)EItems.hat:
                {
                    if (mSkinMeshRenderers[(int)EItems.hair].sharedMesh != null)
                    {
                        mSkinMeshRenderers[(int)EItems.hair].sharedMesh = null;
                    }
                    break;
                }
            case (int)EItems.outerwear:
                {
                    if (mSkinMeshRenderers[(int)EItems.fullBody].sharedMesh != null)
                    {
                        mSkinMeshRenderers[(int)EItems.fullBody].sharedMesh = null;
                    }
                    break;
                }
            case (int)EItems.fullBody:
                {
                    if (mSkinMeshRenderers[(int)EItems.outerwear].sharedMesh != null)
                    {
                        mSkinMeshRenderers[(int)EItems.outerwear].sharedMesh = null;
                    }
                    break;
                }
            default:
                {
                    break;
                }
        }

        mItemIndexs[category] = index;
        mSkinMeshRenderers[category].sharedMesh = mItemDatabases[category].mDatas[index].mMesh;
    }

    private void onHeadSideManuClicked(int index)
    {
        mItemScrolls[0].SetActive(index == 0);
        mItemScrolls[1].SetActive(index == 1);
        mItemScrolls[2].SetActive(index == 2);
        mItemScrolls[3].SetActive(index == 3);
        mItemScrolls[4].SetActive(index == 4);
        mItemScrolls[5].SetActive(index == 5);

        if (index != -1 && mIsButtonInited[index] == false)
        {
            mIsButtonInited[index] = true;
            initItemButtons(index, mItemDatabases[index], mContentTransforms[index], changeItem);
        }
    }

    private void onBodySideManuClicked(int index)
    {
        mItemScrolls[6].SetActive(index == 6);
        mItemScrolls[7].SetActive(index == 7);
        mItemScrolls[8].SetActive(index == 8);
        mItemScrolls[9].SetActive(index == 9);
        mItemScrolls[10].SetActive(index == 10);
        mItemScrolls[11].SetActive(index == 11);

        if (index != -1 && mIsButtonInited[index] == false)
        {
            mIsButtonInited[index] = true;
            initItemButtons(index, mItemDatabases[index], mContentTransforms[index], changeItem);
        }
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
        startOutfit();
    }
            
    private void startOutfit()
    {
        // 변수 초기화
        mItemIndexs[(int)EItems.hair] = 0;
        mItemIndexs[(int)EItems.body] = 0;
        mItemIndexs[(int)EItems.eyebrow] = 0;
        mItemIndexs[(int)EItems.mustache] = -1;
        mItemIndexs[(int)EItems.glasses] = -1;
        mItemIndexs[(int)EItems.hat] = -1;

        mItemIndexs[(int)EItems.outerwear] = 0;
        mItemIndexs[(int)EItems.pants] = 0;
        mItemIndexs[(int)EItems.shoe] = 0;
        mItemIndexs[(int)EItems.glove] = 0;
        mItemIndexs[(int)EItems.backpack] = -1;
        mItemIndexs[(int)EItems.fullBody] = -1;

        // 키워드, 닉네임
        mPlayerManager = NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<PlayerManager>();
        ulong index = (mPlayerManager.mCurIndex.Value + 1) % (ulong)MultiplayerManager.Instance.mConnectedCount.Value;
        mPlayerManager.UpdateCurIndexServerRpc(index);
        //string keyword = MultiplayerManager.Instance.GetAnswer(index); 
        string keyword = MultiplayerManager.Instance.GetQuiz(index); 
        mKeywordText.text = "<color=#6D60CC>" + keyword + "</color>로 꾸미세요!";
        mLoading.SetActive(false);

        StartCoroutine(timer());
    }

    IEnumerator timer()
    {
        #region Timer
        int i = GameCount.OUTFIT_COUNT;
        while (i > 0) 
        {
            mLimitText.text = i.ToString();
            i--;
            yield return new WaitForSecondsRealtime(1);
        }
        #endregion


        // 스타일 저장
        mLoading.SetActive(true);
        MultiplayerManager.Instance.AddStyleServerRpc((ulong)mPlayerManager.mCurIndex.Value, (ulong)mPlayerManager.mMyIndex.Value,
            mItemIndexs[(int)EItems.hair],
            mItemIndexs[(int)EItems.body],
            mItemIndexs[(int)EItems.eyebrow],
            mItemIndexs[(int)EItems.mustache],
            mItemIndexs[(int)EItems.glasses],
            mItemIndexs[(int)EItems.hat],
            mItemIndexs[(int)EItems.outerwear],
            mItemIndexs[(int)EItems.pants],
            mItemIndexs[(int)EItems.shoe],
            mItemIndexs[(int)EItems.glove],
            mItemIndexs[(int)EItems.backpack],
            mItemIndexs[(int)EItems.fullBody]);


        // 이동
        if (NetworkManager.Singleton.IsHost)
        {
            NetworkManager.Singleton.SceneManager.LoadScene(SceneName.GUESS_SCENE + MultiplayerManager.Instance.GetGameMode(), LoadSceneMode.Single);

            //int nextIndex = (mPlayerManager.mCurIndex.Value + 1) % MultiplayerManager.Instance.mConnectedCount.Value;
            //if (nextIndex != mPlayerManager.mMyIndex.Value)
            //{
            //    NetworkManager.Singleton.SceneManager.LoadScene(SceneName.GUESS_SCENE + LobbyManager.CurLobby.Data[LobbyDataKey.LOBBY_DATA_KEY_GAME_MODE].Value, LoadSceneMode.Single);
            //}
            //else
            //{
            //    NetworkManager.Singleton.SceneManager.LoadScene(SceneName.SHOW_SCENE + LobbyManager.CurLobby.Data[LobbyDataKey.LOBBY_DATA_KEY_GAME_MODE].Value, LoadSceneMode.Single);
            //}
        }
    }
}
