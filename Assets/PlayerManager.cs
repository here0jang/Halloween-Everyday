using Unity.Collections;
using Unity.Netcode;

public class PlayerManager : NetworkBehaviour
{
    public NetworkVariable<FixedString32Bytes> mMyKeyword= new(writePerm: NetworkVariableWritePermission.Server, readPerm: NetworkVariableReadPermission.Everyone);
    public NetworkVariable<FixedString32Bytes> mCurKeyword = new(writePerm: NetworkVariableWritePermission.Server, readPerm: NetworkVariableReadPermission.Everyone);

    public NetworkVariable<FixedString4096Bytes> mMyStyle = new(writePerm: NetworkVariableWritePermission.Server, readPerm: NetworkVariableReadPermission.Everyone);
    public NetworkVariable<FixedString4096Bytes> mCurStyle = new(writePerm: NetworkVariableWritePermission.Server, readPerm: NetworkVariableReadPermission.Everyone);

    public NetworkVariable<int> mCurIndex = new(writePerm: NetworkVariableWritePermission.Server, readPerm: NetworkVariableReadPermission.Everyone);
    
    public override void OnNetworkSpawn()
    {
        mMyKeyword.Value = "";
        mCurKeyword.Value = "";

        mMyStyle.Value = "";
        mCurStyle.Value = "";

        mCurIndex.Value = (int)OwnerClientId;

        //mIsReady.OnValueChanged += (FixedString32Bytes previousVal, FixedString32Bytes newVal) =>
        //{

        //};

        DontDestroyOnLoad(this);
    }

    // 클라이언트 -> 호스트
    [ServerRpc]
    public void SendKeywordServerRpc(string keyword)
    {
        mMyKeyword.Value = keyword;
    }

    [ServerRpc]
    public void GetKeywordServerRpc()
    {
        mCurIndex.Value = (mCurIndex.Value + 1) % NetworkManager.Singleton.ConnectedClientsList.Count;
        mCurKeyword.Value = NetworkManager.Singleton.ConnectedClientsList[mCurIndex.Value].PlayerObject.GetComponent<PlayerManager>().mMyKeyword.Value;
    }
   
    [ServerRpc]
    public void SetCurStyleServerRpc(string style)
    {
        mMyStyle.Value = style;
    }

    [ServerRpc]
    public void GetStyleServerRpc()
    {
        mCurStyle.Value = NetworkManager.Singleton.ConnectedClientsList[mCurIndex.Value].PlayerObject.GetComponent<PlayerManager>().mMyStyle.Value;
    }





    [ClientRpc]
    public void StartQuizClientRpc()
    {
        QuizManager.Instance.StartQuizAsync();
    }
}
