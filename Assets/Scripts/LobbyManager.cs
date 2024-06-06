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

public enum EItems
{
    hair,
    body,
    eyebrow,
    mustache,
    glasses,
    hat,
    outerwear,
    pants,
    shoe,
    glove,
    backpack,
    fullBody,

    last,
}

public static class SceneName
{
    public const string LOGIN_SCENE = "00 LOGIN";
    public const string MAIN_SCENE = "01 MAIN";
    public const string WAITING_SCENE = "02 WAITING";
    public const string LOADING_SCENE = "03 LOADING";
    public const string KEYWORD_SCENE = "04 KEYWORD";
    public const string OUTFIT_SCENE = "05 OUTFIT_";
    public const string GUESS_SCENE = "06 GUESS_";
    public const string SHOW_SCENE = "07 SHOW_";
}

public static class GameMode
{
    public const string NORMAL = "Normal";
    public const string TOGETHER= "Together";
    public const string REALTIME = "RealTime";
    public const string QUIZ = "Quiz";
}

public static class LobbyDataKey
{
    public const string LOBBY_DATA_KEY_TOPIC = "Topic";
    public const string LOBBY_DATA_KEY_TOPIC_INDEX = "TopicIndex";
    public const string LOBBY_DATA_KEY_RELAY_CODE = "RelayCode";
    public const string LOBBY_DATA_KEY_GAME_MODE = "GameMode";
    public const string LOBBY_PLAYER_DATA_IS_READY = "IsReady";
}

public static class GameCount
{
    public const int LOADING_COUNT = 5;
    public const int KEYWORD_COUNT = 5;
    public const int OUTFIT_COUNT = 10;
    public const int QUIZ_COUNT = 100;
    public const int TOGETHER_MODE_MIN = 4;
}




public class LobbyManager : MonoBehaviour
{
    public const float LOBBY_UPDATE_TIMER_MAX = 1.1f;
    public const int MAX_PLAYER = 12;
    private const string LOBBY_NAME = "New Lobby";

    private float lobbyUpdateTimer = 0f;
    private float heartBeatTimer      = 0f;

    public static string mCurSceneName;

    private static Lobby curLobby = null;
    public static Lobby CurLobby { get { return curLobby; } }



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
        if (curLobby != null && mCurSceneName == SceneName.WAITING_SCENE)
        {
            lobbyUpdateTimer -= Time.deltaTime;
            if (lobbyUpdateTimer < 0f)
            {
                lobbyUpdateTimer = LobbyManager.LOBBY_UPDATE_TIMER_MAX;

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
    // CREATE & JOIN ROOM
    //
    public static async Task<bool> CreateRoomAsync(string topic, int topicIndex, bool IsPrivate)
    {
        try
        {
            CreateLobbyOptions lobbyOptions = new CreateLobbyOptions();
            lobbyOptions.IsPrivate = IsPrivate;
            lobbyOptions.Data = new Dictionary<string, DataObject>()
            {
                // private : id, �ڵ�θ� ���� ����
                {LobbyDataKey.LOBBY_DATA_KEY_TOPIC, new DataObject(visibility: DataObject.VisibilityOptions.Public, value: topic)},
                {LobbyDataKey.LOBBY_DATA_KEY_TOPIC_INDEX, new DataObject(visibility: DataObject.VisibilityOptions.Public, value: topicIndex.ToString())},
                {LobbyDataKey.LOBBY_DATA_KEY_RELAY_CODE, new DataObject(visibility: DataObject.VisibilityOptions.Public, value: "")},
                {LobbyDataKey.LOBBY_DATA_KEY_GAME_MODE, new DataObject(visibility: DataObject.VisibilityOptions.Public, value: GameMode.NORMAL)},
            };

            curLobby = await LobbyService.Instance.CreateLobbyAsync(LOBBY_NAME, MAX_PLAYER, lobbyOptions);

            return true;
        }
        catch (LobbyServiceException e)
        {
            Debug.LogError(e);
            return false;
        }
    }


    public static async Task<bool> JoinRandomRoom()
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
                Debug.Log("Joined Room");
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

    public static async Task<bool> JoinRoomByCode(string code)
    {
        try
        {
            await Lobbies.Instance.JoinLobbyByCodeAsync(code);
            curLobby = await Lobbies.Instance.GetLobbyAsync(code);
            return true;
        }
        catch(LobbyServiceException e)
        {
            Debug.LogError(e);
            return false;
        }
    }


    //-----------------------------------------------------------------------
    //
    // WAITING
    //
    public static  async Task<bool> StartGame(string relayCode)
    {
        try
        {
            UpdateLobbyOptions options = new UpdateLobbyOptions();
            // ���� ���ο� �÷��̾� �� ����������
            options.IsPrivate = true;
            options.Data = new Dictionary<string, DataObject>()
            {
                {LobbyDataKey.LOBBY_DATA_KEY_RELAY_CODE, new DataObject(visibility: DataObject.VisibilityOptions.Public, value: relayCode)},
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

    public static async Task<string> StartHostWithRelay(int maxConnections = MAX_PLAYER)
    {
        Allocation allocation = await RelayService.Instance.CreateAllocationAsync(maxConnections);
        var joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);

        NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(new RelayServerData(allocation, "dtls"));
        //NetworkManager.Singleton.OnClientConnectedCallback+= 
        return NetworkManager.Singleton.StartHost() ? joinCode : null;
    }

    public static async void StartClientWithRelay(string joinCode)
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

    public static async void migrateHost()
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
    public static async Task<bool> LeaveRoom()
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
            //LobbyEvents.OnLobbyUpdated -= OnLobbyUpdated;
            curLobby = null;
            mCurSceneName = "";
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
