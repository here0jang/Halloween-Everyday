using Unity.Collections;
using Unity.Netcode;

public class PlayerManager : NetworkBehaviour
{
    public NetworkVariable<ulong> mMyIndex = new(writePerm: NetworkVariableWritePermission.Server, readPerm: NetworkVariableReadPermission.Everyone);
    public NetworkVariable<ulong> mCurIndex = new(writePerm: NetworkVariableWritePermission.Server, readPerm: NetworkVariableReadPermission.Everyone);
    public NetworkVariable<FixedString32Bytes> mName = new(writePerm: NetworkVariableWritePermission.Server, readPerm: NetworkVariableReadPermission.Everyone);
    public override void OnNetworkSpawn()
    {
        mMyIndex.Value = OwnerClientId;
        mCurIndex.Value = OwnerClientId;

        DontDestroyOnLoad(this);
    }

    [ServerRpc(RequireOwnership = false)]
    public void UpdateCurIndexServerRpc(ulong index)
    {
        mCurIndex.Value = index;
    }
}
