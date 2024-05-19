using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Netcode.Transports.UTP;
using Unity.Netcode;
using Unity.Networking.Transport.Relay;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.Services.Relay.Models;
using Unity.Services.Relay;
using UnityEngine;


public enum EGameMode
{
    Normal,
    Together,
    Chaos,
    RealTime,
}



public class LobbyManager : MonoBehaviour
{

    private static Lobby curLobby = null;
    public static Lobby CurLobby { get { return curLobby; }}

    private static Dictionary<string, string> localLobbyData = new Dictionary<string, string>();
    public static Dictionary<string, string> LocalLobbyData { get { return localLobbyData; } }
    public List<List<Dictionary<string, string>>> LocalPlayerDatas = new List<List<Dictionary<string, string>>>();

    // KEY : id, VALUE : keyword, styleId, answer...
    public static List<Dictionary<string, List<string>>> PlayData = new List<Dictionary<string, List<string>>>();
    //public static Dictionary<string, string> keywords = new Dictionary<string, string>();


    public const float LOBBY_UPDATE_TIMER_MAX = 1.1f;
    public const int KEYWORD_COUNT = 20;
    public const int OUTFIT_COUNT = 10;
    public const int QUIZ_COUNT = 100;
    public const int TOGETHER_MODE_MIN = 4;

    private float lobbyUpdateTimer = 0f;
    private float heartBeatTimer      = 0f;


    private async void Awake()
    {
        try
        {
            await UnityServices.InitializeAsync();
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }
    }
    private void Update()
    {

        handleLobbyHeartBeat();
        handleLobbyPollForUpdates();
    }

    private async void OnApplicationQuit()
    {
        if(curLobby != null)
        {
            await LeaveRoom();
            //LobbyService.Instance.DeleteLobbyAsync(curLobby.Id);
        }
    }


    private async void handleLobbyHeartBeat()
    {
        // ���� ȣ��Ʈ�� ��� �츰��
        if (curLobby != null && curLobby.HostId == AuthenticationService.Instance.PlayerId)
        {
            heartBeatTimer -= Time.deltaTime;
            if (heartBeatTimer < 0f)
            {
                float heartBeatTimerMax = 15;
                heartBeatTimer = heartBeatTimerMax;

                await LobbyService.Instance.SendHeartbeatPingAsync(curLobby.Id);
            }
        }
    }
    private async void handleLobbyPollForUpdates()
    {
        // ���� �κ� �������̸� �ֱ������� ������Ʈ
        if (curLobby != null)
        {
            lobbyUpdateTimer -= Time.deltaTime;
            if (lobbyUpdateTimer < 0f)
            {
                lobbyUpdateTimer = LOBBY_UPDATE_TIMER_MAX;

                // ������Ʈ ����
                try
                {
                    curLobby = await LobbyService.Instance.GetLobbyAsync(curLobby.Id);
                }
                catch (LobbyServiceException e)
                {
                    if (e.Reason == LobbyExceptionReason.EntityNotFound)
                    {
                        curLobby = null;
                    }

                    Debug.Log(e);
                }
            }
        }
    }


    //-----------------------------------------------------------------------
    //
    // LOGIN
    //
    public static async Task<Exception> Authenticate()
    {
        try
        {
            // TODO : �͸� �α��� (���� ���� �α������� ����)
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
            return null;
        }
        catch (AuthenticationException ex)
        {
            // Compare error code to AuthenticationErrorCodes
            // Notify the player with the proper error message
            Debug.LogException(ex);
            return ex;
        }
        catch (RequestFailedException ex)
        {
            // Compare error code to CommonErrorCodes
            // Notify the player with the proper error message
            Debug.LogException(ex);
            return ex;
        }
    }

    //-----------------------------------------------------------------------
    //
    // CREATE & JOIN ROOM
    //
    public async Task<bool> CreateRoomAsync(string lobbyName, string topic = "veggies", int topicIndex = 0, EGameMode gameMode = EGameMode.Normal, int maxMember = 2, bool IsPrivate = false)
    {
        try
        {
            CreateLobbyOptions lobbyOptions = new CreateLobbyOptions();
            lobbyOptions.IsPrivate = IsPrivate;
            lobbyOptions.Data = new Dictionary<string, DataObject>()
            {
                // private : id, �ڵ�θ� ���� ����
                {"Topic", new DataObject(visibility: DataObject.VisibilityOptions.Public, value: topic)},
                {"TopicIndex", new DataObject(visibility: DataObject.VisibilityOptions.Public, value: topicIndex.ToString())},
                {"RelayCode", new DataObject(visibility: DataObject.VisibilityOptions.Public, value: "")},
                {"GameMode", new DataObject(visibility: DataObject.VisibilityOptions.Public, value: gameMode.ToString())},
            };

            curLobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, maxMember, lobbyOptions);
            Debug.Log("lobby created successfully, lobby topic : " + curLobby.Data["Topic"].Value + " Room State : " + curLobby.IsPrivate + " GameMode : " + curLobby.Data["GameMode"].Value);
            
            localLobbyData.Add("Topic", topic);
            localLobbyData.Add("GameMode", gameMode.ToString());

            return true;
        }
        catch (LobbyServiceException e)
        {
            Debug.LogError(e);
            return false;
        }
    }


    public async Task<bool> JoinRandomRoom()
    {
        try
        {
            QueryResponse queryResponse = await Lobbies.Instance.QueryLobbiesAsync();

            // ���� ���� ��� ���� ó��
            if(queryResponse.Results.Count == 0)
            {
                return false;
            }
            else
            {
                await Lobbies.Instance.JoinLobbyByIdAsync(queryResponse.Results[0].Id);
                curLobby = queryResponse.Results[0];

                Debug.Log("Joined Room : " + curLobby.Data["Topic"].Value);

                return true;
            }
        }
        catch (LobbyServiceException e)
        {
            if (e.Reason == LobbyExceptionReason.LobbyNotFound)
            {
                // ���� �� ���� �����ϴ�. ��� �� �ٽ� �õ����ּ���
            }

            Debug.LogError(e);
            return false;
        }
    }

    //-----------------------------------------------------------------------
    //
    // WAITING
    //
    public async Task<bool> StartGame(string relayCode)
    {
        try
        {
            UpdateLobbyOptions options = new UpdateLobbyOptions();
            // ���� ���ο� �÷��̾� �� ����������
            options.IsPrivate = true;
            options.Data = new Dictionary<string, DataObject>()
            {
                {"RelayCode", new DataObject(visibility: DataObject.VisibilityOptions.Public, value: relayCode)},
            };
            curLobby = await Lobbies.Instance.UpdateLobbyAsync(CurLobby.Id, options);

            return true;
        }
        catch (LobbyServiceException e)
        {
            Debug.LogError(e);
            return false;
        }
    }

    public async Task<string> StartHostWithRelay(int maxConnections = 5)
    {
        Allocation allocation = await RelayService.Instance.CreateAllocationAsync(maxConnections);
        var joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);

        NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(new RelayServerData(allocation, "dtls"));

        return NetworkManager.Singleton.StartHost() ? joinCode : null;
    }

    public async void StartClientWithRelay(string joinCode)
    {
        try
        {
            var joinAllocation = await RelayService.Instance.JoinAllocationAsync(joinCode: joinCode);

            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(new RelayServerData(joinAllocation, "dtls"));

            NetworkManager.Singleton.StartClient();

            //return !string.IsNullOrEmpty(joinCode) && NetworkManager.Singleton.StartClient();
        }
        catch(RelayServiceException e)
        {
            Debug.Log(e);
        }



    }


    // ������ ����??? 

    //-----------------------------------------------------------------------
    //
    // PLAYER DATA
    //
    public static async Task<bool> SetPlayerData(string _key, string _value)
    {
        try
        {
            UpdatePlayerOptions options = new UpdatePlayerOptions();
            options.Data = new Dictionary<string, PlayerDataObject>()
            {
                {_key, new PlayerDataObject(visibility: PlayerDataObject.VisibilityOptions.Public,value: _value)},
            };

            string playerId = AuthenticationService.Instance.PlayerId;
            var lobby = await LobbyService.Instance.UpdatePlayerAsync(curLobby.Id, playerId, options);
            return true;
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
            return false;
        }
    }
    public static async Task<bool> SetPlayerData(string _key1,  string _value1, string _key2, string _value2)
    {
        try
        {
            UpdatePlayerOptions options = new UpdatePlayerOptions();
            options.Data = new Dictionary<string, PlayerDataObject>()
            {
                {_key1, new PlayerDataObject(visibility: PlayerDataObject.VisibilityOptions.Public,value: _value1)},
                {_key2, new PlayerDataObject(visibility: PlayerDataObject.VisibilityOptions.Public,value: _value2)},
            };

            string playerId = AuthenticationService.Instance.PlayerId;
            var lobby = await LobbyService.Instance.UpdatePlayerAsync(curLobby.Id, playerId, options);
            return true;
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
            return false;
        }
    }
    public static async Task<bool> SetPlayerData(string _key1, string _value1, string _key2, string _value2, string _key3, string _value3)
    {
        try
        {
            UpdatePlayerOptions options = new UpdatePlayerOptions();
            options.Data = new Dictionary<string, PlayerDataObject>()
            {
                {_key1, new PlayerDataObject(visibility: PlayerDataObject.VisibilityOptions.Public,value: _value1)},
                {_key2, new PlayerDataObject(visibility: PlayerDataObject.VisibilityOptions.Public,value: _value2)},
                {_key3, new PlayerDataObject(visibility: PlayerDataObject.VisibilityOptions.Public,value: _value3)},
            };

            string playerId = AuthenticationService.Instance.PlayerId;
            var lobby = await LobbyService.Instance.UpdatePlayerAsync(curLobby.Id, playerId, options);
            return true;
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
            return false;
        }
    }

    //-----------------------------------------------------------------------
    //
    // QUIT
    //

    public async void migrateHost()
    {
        try
        {
            curLobby = await Lobbies.Instance.UpdateLobbyAsync(curLobby.Id, new UpdateLobbyOptions { HostId = curLobby.Players[1].Id });
        }
        catch (LobbyServiceException e)
        {
            Debug.LogError(e);
        }
    }
    public async Task<bool> LeaveRoom()
    {
        //LobbyEvents.OnLobbyUpdated -= OnLobbyUpdated;

        try
        {
            if (curLobby.Players.Count > 1)
            {
                // ���� ȣ��Ʈ�� �絵
                if (AuthenticationService.Instance.PlayerId == curLobby.HostId)
                {
                    migrateHost();
                }

                // ������
                await LobbyService.Instance.RemovePlayerAsync(curLobby.Id, AuthenticationService.Instance.PlayerId);
            }
            else
            {
                // ����Ʈ
                await LobbyService.Instance.DeleteLobbyAsync(curLobby.Id);
            }

            Debug.Log("Leaved Lobby");
            localLobbyData.Clear();
            //LobbyEvents.OnLobbyUpdated -= OnLobbyUpdated;
            curLobby = null;

            return true;
        }
        catch (LobbyServiceException e)
        {
            Debug.LogError(e);
            return false;
        }
    }









    private async void listLobbies()
    {
        try
        {
            QueryLobbiesOptions queryLobbiesOptions = new QueryLobbiesOptions
            {
                Count = 10,
                Filters = new List<QueryFilter> { new QueryFilter(QueryFilter.FieldOptions.AvailableSlots, "0", QueryFilter.OpOptions.GT) },
                Order = new List<QueryOrder> { new QueryOrder(false, QueryOrder.FieldOptions.Created) }
            };

            QueryResponse queryResponse = await Lobbies.Instance.QueryLobbiesAsync(queryLobbiesOptions);

            foreach (Lobby lobby in queryResponse.Results)
            {
                Debug.Log("Name : " + lobby.Name);
            }

            // TODO  : ������ ���� ��� �� �����ͼ� UI �����ֱ�

        }
        catch (LobbyServiceException e)
        {
            Debug.LogError(e);
        }
    }

    public void SearchRoom()
    {
        // �α� ����
        // �ڵ��ϼ�
    }

    public void GoToMyRoom()
    {
        // �α� ����
        // ���� ���� ���� �ƹ�Ÿ
    }
}
