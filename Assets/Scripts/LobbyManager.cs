using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Netcode.Transports.UTP;
using Unity.Netcode;
using Unity.Networking.Transport.Relay;
using Unity.Services.Authentication;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.Services.Relay.Models;
using Unity.Services.Relay;
using UnityEngine;
using System.Collections;

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



public struct GameMode
{
    public const string NORMAL = "Normal";
    public const string TOGETHER = "Together";
    public const string REALTIME = "RealTime";
    public const string QUIZ = "Quiz";
}

public struct LobbyDataKey
{
    public const string LOBBY_DATA_KEY_TOPIC = "Topic";
    public const string LOBBY_DATA_KEY_TOPIC_INDEX = "TopicIndex";
    public const string LOBBY_DATA_KEY_RELAY_CODE = "RelayCode";
    public const string LOBBY_DATA_KEY_GAME_MODE = "GameMode";
    public const string LOBBY_PLAYER_DATA_IS_READY = "IsReady";
}

public struct GameCount
{
    public const int CREATE_ROOM_COUNT = 10;
    public const int LOADING_COUNT = 5;
    public const int KEYWORD_COUNT = 15;
    public const int OUTFIT_COUNT = 120;
    public const int GUESS_COUNT = 20;
    public const int QUIZ_COUNT = 100;
    public const int SHOW_COUNT = 60;
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
    public static Lobby CurLobby { get; private set; }



    private void Update()
    {
        handleLobbyHeartBeat();
        handleLobbyPollForUpdates();
    }

    private async void OnApplicationQuit()
    {
        if(CurLobby != null)
        {
            await LeaveRoom();
        }
    }


    private async void handleLobbyHeartBeat()
    {
        // 내가 호스트면 계속 살린다
        if (CurLobby != null && CurLobby.HostId == AuthenticationService.Instance.PlayerId)
        {
            heartBeatTimer -= Time.deltaTime;
            if (heartBeatTimer < 0f)
            {
                float heartBeatTimerMax = 15;
                heartBeatTimer = heartBeatTimerMax;

                await LobbyService.Instance.SendHeartbeatPingAsync(CurLobby.Id);
            }
        }
    }
    private async void handleLobbyPollForUpdates()
    {
        // 현재 로비 입장중이면 주기적으로 업데이트
        if (CurLobby != null && mCurSceneName == SceneName.WAITING_SCENE)
        {
            lobbyUpdateTimer -= Time.deltaTime;
            if (lobbyUpdateTimer < 0f)
            {
                lobbyUpdateTimer = LobbyManager.LOBBY_UPDATE_TIMER_MAX;

                // 업데이트 폴링
                try
                {
                    CurLobby = await LobbyService.Instance.GetLobbyAsync(CurLobby.Id);
                }
                catch (LobbyServiceException e)
                {
                    if (e.Reason == LobbyExceptionReason.EntityNotFound)
                    {
                        CurLobby = null;
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
                // private : id, 코드로만 참여 가능
                {LobbyDataKey.LOBBY_DATA_KEY_TOPIC, new DataObject(visibility: DataObject.VisibilityOptions.Public, value: topic)},
                {LobbyDataKey.LOBBY_DATA_KEY_TOPIC_INDEX, new DataObject(visibility: DataObject.VisibilityOptions.Public, value: topicIndex.ToString())},
                {LobbyDataKey.LOBBY_DATA_KEY_RELAY_CODE, new DataObject(visibility: DataObject.VisibilityOptions.Public, value: "")},
                {LobbyDataKey.LOBBY_DATA_KEY_GAME_MODE, new DataObject(visibility: DataObject.VisibilityOptions.Public, value: GameMode.NORMAL)},
            };

            CurLobby = await LobbyService.Instance.CreateLobbyAsync(LOBBY_NAME, MAX_PLAYER, lobbyOptions);

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

            // 방이 없을 경우 예외 처리
            if(queryResponse.Results.Count == 0)
            {
                return false;
            }
            else
            {
                await Lobbies.Instance.JoinLobbyByIdAsync(queryResponse.Results[0].Id);
                CurLobby = queryResponse.Results[0];
                Debug.Log("Joined Room");
                return true;
            }
        }
        catch (LobbyServiceException e)
        {
            if (e.Reason == LobbyExceptionReason.LobbyNotFound)
            {
                // 현재 빈 방이 없습니다. 잠시 후 다시 시도해주세요
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
            CurLobby = await Lobbies.Instance.GetLobbyAsync(code);
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
            // 이제 새로운 플레이어 더 못들어오도록
            options.IsPrivate = true;
            options.Data = new Dictionary<string, DataObject>()
            {
                {LobbyDataKey.LOBBY_DATA_KEY_RELAY_CODE, new DataObject(visibility: DataObject.VisibilityOptions.Public, value: relayCode)},
            };
            CurLobby = await Lobbies.Instance.UpdateLobbyAsync(CurLobby.Id, options);

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

        NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(new RelayServerData(allocation, "wss"));
        NetworkManager.Singleton.GetComponent<UnityTransport>().UseWebSockets = true;
        return NetworkManager.Singleton.StartHost() ? joinCode : null;
    }

    public static async Task<bool> StartClientWithRelay(string joinCode)
    {
        try
        {
            var joinAllocation = await RelayService.Instance.JoinAllocationAsync(joinCode: joinCode);

            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(new RelayServerData(joinAllocation, "wss"));
            NetworkManager.Singleton.GetComponent<UnityTransport>().UseWebSockets = true;
            NetworkManager.Singleton.StartClient();
            return true;
            //return !string.IsNullOrEmpty(joinCode) && NetworkManager.Singleton.StartClient();
        }
        catch(RelayServiceException e)
        {
            Debug.Log(e);
            return false;
        }
    }

    #region Lobby Player Data
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
            var lobby = await LobbyService.Instance.UpdatePlayerAsync(CurLobby.Id, playerId, options);
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
            var lobby = await LobbyService.Instance.UpdatePlayerAsync(CurLobby.Id, playerId, options);
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
            var lobby = await LobbyService.Instance.UpdatePlayerAsync(CurLobby.Id, playerId, options);
            return true;
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
            return false;
        }
    }
    #endregion


    //-----------------------------------------------------------------------
    //
    // QUIT
    //

    public static async void migrateHost()
    {
        try
        {
            CurLobby = await Lobbies.Instance.UpdateLobbyAsync(CurLobby.Id, new UpdateLobbyOptions { HostId = CurLobby.Players[1].Id });
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
            if (CurLobby.Players.Count > 1)
            {
                // 내가 호스트면 양도
                if (AuthenticationService.Instance.PlayerId == CurLobby.HostId)
                {
                    migrateHost();
                }

                // 나가기
                await LobbyService.Instance.RemovePlayerAsync(CurLobby.Id, AuthenticationService.Instance.PlayerId);
            }
            else
            {
                // 딜리트
                await LobbyService.Instance.DeleteLobbyAsync(CurLobby.Id);
            }

            Debug.Log("Leaved Lobby");
            //LobbyEvents.OnLobbyUpdated -= OnLobbyUpdated;
            CurLobby = null;
            mCurSceneName = "";
            return true;
        }
        catch (LobbyServiceException e)
        {
            Debug.LogError(e);
            return false;
        }
    }
}
