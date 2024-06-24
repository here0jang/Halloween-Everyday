using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GuessNormalSceneManager : NetworkBehaviour
{
    [SerializeField] private TMPro.TMP_Text mLimitText;
    [SerializeField] private TMPro.TMP_InputField mInputField;
    [SerializeField] private GameObject mLoading;

    [SerializeField] private ItemDatabase[] mItemDatabases                          = new ItemDatabase[(int)EItems.last];
    [SerializeField] private SkinnedMeshRenderer[] mSkinMeshRenderers = new SkinnedMeshRenderer[(int)EItems.last];

    private List<ulong> mConnectedClients = new List<ulong>();

    public static GuessNormalSceneManager Instance { get; private set; }
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

    public void GuessSceneInit(ulong id)
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

        startGuessClientRpc();
    }

    [ClientRpc]
    private void startGuessClientRpc()
    {
        PlayerManager playerManager = NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<PlayerManager>();
        ulong index = (playerManager.mCurIndex.Value + 1) % (ulong)MultiplayerManager.Instance.mConnectedCount.Value;
        playerManager.UpdateCurIndexServerRpc(index);

        mSkinMeshRenderers[(int)EItems.hair].sharedMesh = MultiplayerManager.Instance.GetStyle(index).HairId == -1 ? null : mItemDatabases[(int)EItems.hair].mDatas[MultiplayerManager.Instance.GetStyle(index).HairId].mMesh;
        mSkinMeshRenderers[(int)EItems.body].sharedMesh = MultiplayerManager.Instance.GetStyle(index).BodyId == -1 ? null : mItemDatabases[(int)EItems.body].mDatas[MultiplayerManager.Instance.GetStyle(index).BodyId].mMesh;
        mSkinMeshRenderers[(int)EItems.eyebrow].sharedMesh = MultiplayerManager.Instance.GetStyle(index).EyebrowId == -1 ? null : mItemDatabases[(int)EItems.eyebrow].mDatas[MultiplayerManager.Instance.GetStyle(index).EyebrowId].mMesh;
        mSkinMeshRenderers[(int)EItems.mustache].sharedMesh = MultiplayerManager.Instance.GetStyle(index).MustacheId == -1 ? null : mItemDatabases[(int)EItems.mustache].mDatas[MultiplayerManager.Instance.GetStyle(index).MustacheId].mMesh;
        mSkinMeshRenderers[(int)EItems.glasses].sharedMesh = MultiplayerManager.Instance.GetStyle(index).GlassesId == -1 ? null : mItemDatabases[(int)EItems.glasses].mDatas[MultiplayerManager.Instance.GetStyle(index).GlassesId].mMesh;
        mSkinMeshRenderers[(int)EItems.hat].sharedMesh = MultiplayerManager.Instance.GetStyle(index).HatId == -1 ? null : mItemDatabases[(int)EItems.hat].mDatas[MultiplayerManager.Instance.GetStyle(index).HatId].mMesh;

        mSkinMeshRenderers[(int)EItems.outerwear].sharedMesh = MultiplayerManager.Instance.GetStyle(index).OuterId == -1 ? null : mItemDatabases[(int)EItems.outerwear].mDatas[MultiplayerManager.Instance.GetStyle(index).OuterId].mMesh;
        mSkinMeshRenderers[(int)EItems.pants].sharedMesh = MultiplayerManager.Instance.GetStyle(index).PantsId == -1 ? null : mItemDatabases[(int)EItems.pants].mDatas[MultiplayerManager.Instance.GetStyle(index).PantsId].mMesh;
        mSkinMeshRenderers[(int)EItems.shoe].sharedMesh = MultiplayerManager.Instance.GetStyle(index).ShoeId == -1 ? null : mItemDatabases[(int)EItems.shoe].mDatas[MultiplayerManager.Instance.GetStyle(index).ShoeId].mMesh;
        mSkinMeshRenderers[(int)EItems.glove].sharedMesh = MultiplayerManager.Instance.GetStyle(index).GloveId == -1 ? null : mItemDatabases[(int)EItems.glove].mDatas[MultiplayerManager.Instance.GetStyle(index).GloveId].mMesh;
        mSkinMeshRenderers[(int)EItems.backpack].sharedMesh = MultiplayerManager.Instance.GetStyle(index).BackpackId == -1 ? null : mItemDatabases[(int)EItems.backpack].mDatas[MultiplayerManager.Instance.GetStyle(index).BackpackId].mMesh;
        mSkinMeshRenderers[(int)EItems.fullBody].sharedMesh = MultiplayerManager.Instance.GetStyle(index).FullBodyId == -1 ? null : mItemDatabases[(int)EItems.fullBody].mDatas[MultiplayerManager.Instance.GetStyle(index).FullBodyId].mMesh;

        mLoading.SetActive(false);

        StartCoroutine(timer(index, playerManager));
    }

    IEnumerator timer(ulong index, PlayerManager playerManager)
    {
        int i =  GameCount.GUESS_COUNT;
        while (i > 0)
        {
            mLimitText.text = i.ToString();
            i--;
            yield return new WaitForSecondsRealtime(1);
        }

        // 정답 저장
        MultiplayerManager.Instance.AddAnswerServerRpc(index, playerManager.mMyIndex.Value, mInputField.text);

        #region Move
        if (NetworkManager.Singleton.IsHost)
        {
            NetworkManager.Singleton.SceneManager.LoadScene(SceneName.SHOW_SCENE + MultiplayerManager.Instance.GetGameMode(), LoadSceneMode.Single);


            //    int nextIndex = (mPlayerManager.mCurIndex.Value + 1) % MultiplayerManager.Instance.mConnectedCount.Value;
            //    if (nextIndex != mPlayerManager.mMyIndex.Value)
            //    {
            //        NetworkManager.Singleton.SceneManager.LoadScene(SceneName.OUTFIT_SCENE + LobbyManager.CurLobby.Data[LobbyDataKey.LOBBY_DATA_KEY_GAME_MODE].Value, LoadSceneMode.Single);
            //    }
            //    else
            //    {
            //        NetworkManager.Singleton.SceneManager.LoadScene(SceneName.SHOW_SCENE + LobbyManager.CurLobby.Data[LobbyDataKey.LOBBY_DATA_KEY_GAME_MODE].Value, LoadSceneMode.Single);
            //    }
        }
        #endregion
    }
}
