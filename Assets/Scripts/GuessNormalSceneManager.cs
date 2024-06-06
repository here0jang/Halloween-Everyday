using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GuessNormalSceneManager : NetworkBehaviour
{
    [SerializeField] private TMPro.TMP_Text mLimitText;
    [SerializeField] private TMPro.TMP_InputField mInputField;

    [SerializeField] private ItemDatabase[] mItemDatabases = new ItemDatabase[(int)EItems.last];
    [SerializeField] private SkinnedMeshRenderer mSkinMeshRenderers;


    private async void Start()
    {
        PlayerManager mPlayerManager = NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<PlayerManager>();
        int index = (mPlayerManager.mCurIndex.Value + 1) % MultiplayerManager.Instance.mConnectedCount.Value;
        mPlayerManager.UpdateCurIndexServerRpc(index);
        mSkinMeshRenderers.sharedMesh = mItemDatabases[0].mDatas[MultiplayerManager.Instance.GetStyle(index).HairId].mMesh;


        #region Timer
        float timer = (float)System.DateTime.Now.TimeOfDay.TotalSeconds + GameCount.GUESS_COUNT; /* 하드웨어 시간 이용 */
        while (timer > (float)System.DateTime.Now.TimeOfDay.TotalSeconds)
        {
            mLimitText.text = $"{timer - (float)System.DateTime.Now.TimeOfDay.TotalSeconds:N0}";
            await Task.Yield();
        }
        #endregion


        // 정답 저장
        MultiplayerManager.Instance.AddAnswerServerRpc((ulong)index, (ulong)mPlayerManager.mMyIndex.Value, mInputField.text);



        #region Move
        if (NetworkManager.Singleton.IsHost)
        {
            int nextIndex = (mPlayerManager.mCurIndex.Value + 1) % MultiplayerManager.Instance.mConnectedCount.Value;
            if (nextIndex != mPlayerManager.mMyIndex.Value)
            {
                NetworkManager.Singleton.SceneManager.LoadScene(SceneName.OUTFIT_SCENE + LobbyManager.CurLobby.Data[LobbyDataKey.LOBBY_DATA_KEY_GAME_MODE].Value, LoadSceneMode.Single);
            }
            else
            {
                NetworkManager.Singleton.SceneManager.LoadScene(SceneName.SHOW_SCENE + LobbyManager.CurLobby.Data[LobbyDataKey.LOBBY_DATA_KEY_GAME_MODE].Value, LoadSceneMode.Single);
            }
        }
        #endregion
    }
}
