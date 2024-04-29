using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public class LobbyManager : MonoBehaviour
{
    private static Lobby curLobby = null;
    public static Lobby CurLobby { get { return curLobby; }}


    public const float LOBBY_UPDATE_TIMER_MAX = 2f;

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

    private async void handleLobbyHeartBeat()
    {
        // 내가 호스트면 계속 살린다
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
        // 현재 로비 입장중이면 주기적으로 업데이트
        if (curLobby != null)
        {
            lobbyUpdateTimer -= Time.deltaTime;
            if (lobbyUpdateTimer < 0f)
            {
                lobbyUpdateTimer = LOBBY_UPDATE_TIMER_MAX;

                // 업데이트 폴링
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
            // TODO : 익명 로그인 (추후 구글 로그인으로 변경)
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
    public static async Task<bool> CreateRoomAsync(string lobbyName, string topic = "veggies", int maxMember = 2, bool IsPrivate = false)
    {
        try
        {
            CreateLobbyOptions lobbyOptions = new CreateLobbyOptions();
            lobbyOptions.IsPrivate = IsPrivate;
            lobbyOptions.Data = new Dictionary<string, DataObject>()
            {
                // private : id, 코드로만 참여 가능
                {"Topic", new DataObject(visibility: DataObject.VisibilityOptions.Public, value: topic)},
                {"IsStarted", new DataObject(visibility: DataObject.VisibilityOptions.Public, value: "1")},
            };

            curLobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, maxMember, lobbyOptions);
            Debug.Log("lobby created successfully, lobby topic : " + curLobby.Data["Topic"].Value);

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
                curLobby = queryResponse.Results[0];
                Debug.Log("Joined Room : " + curLobby.Data["Topic"].Value);
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

    //-----------------------------------------------------------------------
    //
    // WAITING
    //
    public static async Task<bool> StartGame()
    {
        try
        {
            UpdateLobbyOptions options = new UpdateLobbyOptions();
            options.Data = new Dictionary<string, DataObject>()
            {
                {"IsStarted", new DataObject(visibility: DataObject.VisibilityOptions.Public, value: "0")},
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

    public static async Task<bool> SetPlayerNicName(string name)
    {
        try
        {
            UpdatePlayerOptions options = new UpdatePlayerOptions();
            options.Data = new Dictionary<string, PlayerDataObject>()
            {
                {"Name", new PlayerDataObject(visibility: PlayerDataObject.VisibilityOptions.Public,value: name)},
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
    // KEYWORDSTYLING
    //
    public async Task<bool> UpdatePlayerKeywordData(string keyword, string name)
    {
        try
        {
            UpdatePlayerOptions options = new UpdatePlayerOptions();
            options.Data = new Dictionary<string, PlayerDataObject>()
            {
                {"Keyword", new PlayerDataObject(visibility: PlayerDataObject.VisibilityOptions.Public,value: keyword)},
                {"StyleId1", new PlayerDataObject(visibility: PlayerDataObject.VisibilityOptions.Public,value: "")},
                {"StyleId2", new PlayerDataObject(visibility: PlayerDataObject.VisibilityOptions.Public,value: "")},
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
    // STYLING
    //
    public async Task<bool> UpdatePlayerStyleData(string styleId1, string styleId2)
    {
        try
        {
            UpdatePlayerOptions options = new UpdatePlayerOptions();
            options.Data = new Dictionary<string, PlayerDataObject>()
            {
                {"StyleId1", new PlayerDataObject(visibility: PlayerDataObject.VisibilityOptions.Public,value: styleId1)},
                {"StyleId2", new PlayerDataObject(visibility: PlayerDataObject.VisibilityOptions.Public,value: styleId2)},
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

    public async Task<bool> StartQuiz()
    {
        try
        {
            UpdateLobbyOptions options = new UpdateLobbyOptions();
            options.Data = new Dictionary<string, DataObject>()
            {
                {"IsStarted", new DataObject(visibility: DataObject.VisibilityOptions.Public, value: "0")},
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

    //-----------------------------------------------------------------------
    //
    // QUIT
    //

    public static async void migrateHost()
    {
        try
        {
            curLobby = await Lobbies.Instance.UpdateLobbyAsync(curLobby.Id,
                new UpdateLobbyOptions { HostId = curLobby.Players[1].Id });
        }
        catch (LobbyServiceException e)
        {
            Debug.LogError(e);
        }
    }

    public static async Task<bool> LeaveRoom()
    {
        try
        {
            if (curLobby.Players.Count > 1)
            {
                // 내가 호스트면 양도
                if (AuthenticationService.Instance.PlayerId == curLobby.HostId)
                {
                    migrateHost();
                }

                // 나가기
                await LobbyService.Instance.RemovePlayerAsync(curLobby.Id, AuthenticationService.Instance.PlayerId);
            }
            else
            {
                // 딜리트
                await LobbyService.Instance.DeleteLobbyAsync(curLobby.Id);
            }

            Debug.Log("Leaved Lobby");
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

            // TODO  : 주제와 현재 멤버 수 가져와서 UI 보여주기

        }
        catch (LobbyServiceException e)
        {
            Debug.LogError(e);
        }
    }

    public void SearchRoom()
    {
        // 인기 주제
        // 자동완성
    }

    public void GoToMyRoom()
    {
        // 인기 주제
        // 내가 만든 퀴즈 아바타
    }
}
