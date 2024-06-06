using Unity.Netcode;

public class PlayerManager : NetworkBehaviour
{
    public NetworkVariable<int> mMyIndex = new(writePerm: NetworkVariableWritePermission.Server, readPerm: NetworkVariableReadPermission.Everyone);
    public NetworkVariable<int> mCurIndex = new(writePerm: NetworkVariableWritePermission.Server, readPerm: NetworkVariableReadPermission.Everyone);

    public override void OnNetworkSpawn()
    {
        mMyIndex.Value = (int)OwnerClientId;
        mCurIndex.Value = (int)OwnerClientId;

        DontDestroyOnLoad(this);
    }

    [ServerRpc(RequireOwnership = false)]
    public void UpdateCurIndexServerRpc(int index)
    {
        mCurIndex.Value = index;
    }





    [ClientRpc]
    public void StartQuizClientRpc()
    {
        QuizManager.Instance.StartQuizAsync();
    }
}
