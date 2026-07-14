using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LobbyManager : MonoBehaviour
{
    private Lobby hostLobby;
    private Lobby currentLobby;

    public event Action onLobbyCreated;
    public event Action onLobbyJoined;
    public event Action onLobbyLeft;
    public event Action<Lobby> onLobbyUpdated;
    public event Action onLobbyFailed;
    public event Action<List<Lobby>> onLobbiesRefreshed;

    public static LobbyManager Instance { get; private set; }
    public string GetRoomCode => currentLobby.LobbyCode;
    public Lobby CurrentLobby => currentLobby;
    private void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        } 

        Instance = this;
    }

    private async void Start()
    {
        DontDestroyOnLoad(gameObject);
        await UnityServices.InitializeAsync();
        await AuthenticationService.Instance.SignInAnonymouslyAsync();
        BoostrapManager.Instance.LoadMainMenuAndFadeSplashScreen();
    }


    public async Task<string> CreateRelay()
    {
        try
        {
            Allocation allocation = await RelayService.Instance.CreateAllocationAsync(2);
            string joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);

            UnityTransport transport = NetworkManager.Singleton.gameObject.GetComponent<UnityTransport>();

            RelayServerData serverData = AllocationUtils.ToRelayServerData(allocation, "dtls");
            transport.SetRelayServerData(serverData);

            return joinCode;
        }
        catch(RelayServiceException rse)
        {
            Debug.LogError(rse.Message);
            return "";        
        }
    }

    public async Task CreateLobby(string lobbyName, string hostname, bool isPrivate)
    {
        try
        {
            int maxPlayers = 2;

            string joinCode = await CreateRelay();

            if (joinCode == "")
            {
                onLobbyFailed?.Invoke();
                return;
            }

            CreateLobbyOptions options = new CreateLobbyOptions
            {
                Data = new Dictionary<string, DataObject>
                {
                    {"RelayJoinCode",new DataObject(DataObject.VisibilityOptions.Member, joinCode) },
                },
                Player = new Player
                {
                    Data = new Dictionary<string, PlayerDataObject>
                    {
                        {"PlayerName", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, hostname) },
                    },
                },
                IsPrivate = isPrivate,
            };

            Lobby lobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, maxPlayers, options);
            hostLobby = lobby;
            currentLobby = lobby;

            StartHeartBeat();
            StartPollingLobbyData();
            NetworkManager.Singleton.StartHost();
            onLobbyCreated?.Invoke();
        }
        catch(Exception e)
        {
            onLobbyFailed?.Invoke();
            Debug.LogError(e.Message);
        }
    }

    public async Task<List<Lobby>> ListLobbies()
    {
        try
        {
            QueryLobbiesOptions options = new QueryLobbiesOptions
            {
                Count = 15,
                Filters = new List<QueryFilter>
                {
                    new QueryFilter(QueryFilter.FieldOptions.AvailableSlots, "1", QueryFilter.OpOptions.EQ),
                },
                Order = new List<QueryOrder>
                {
                    new QueryOrder(true, QueryOrder.FieldOptions.Created)
                }
                
            };

            QueryResponse queryResponse = await LobbyService.Instance.QueryLobbiesAsync();

            return queryResponse.Results;
        }
        catch(Exception e)
        {
            Debug.LogError(e.Message);
            return null;
        }
    }

    public async Task<bool> JoinLobbyByCode(string roomCode, string clientName)
    {
        try
        {
            JoinLobbyByCodeOptions joinLobbyByCodeOptions = new JoinLobbyByCodeOptions
            {
                Player = new Player
                {
                    Data = new Dictionary<string, PlayerDataObject>
                    {
                        {"PlayerName", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, clientName) },
                    },
                },
            };
            Lobby lobby = await LobbyService.Instance.JoinLobbyByCodeAsync(roomCode, joinLobbyByCodeOptions);
            currentLobby = lobby;

            string relayJoinCode = currentLobby.Data["RelayJoinCode"].Value;


            JoinAllocation joinAllocation = await RelayService.Instance.JoinAllocationAsync(relayJoinCode);
            UnityTransport transport = NetworkManager.Singleton.gameObject.GetComponent<UnityTransport>();

            RelayServerData relayServerData = AllocationUtils.ToRelayServerData(joinAllocation, "dtls");
            transport.SetRelayServerData(relayServerData);

            NetworkManager.Singleton.StartClient();
            StartPollingLobbyData();
            onLobbyJoined?.Invoke();
            return true;
        }
        catch(LobbyServiceException e)
        {
            onLobbyFailed?.Invoke();
            Debug.Log(e);
            return false;
        }
    }

    public async Task<bool> QuickJoinLobby(string clientName)
    {
        try
        {
            QuickJoinLobbyOptions quick = new QuickJoinLobbyOptions
            {
                Player = new Player
                {
                    Data = new Dictionary<string, PlayerDataObject>
                    {
                        {"PlayerName", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, clientName) },
                    },
                },
            };

            Lobby lobby = await LobbyService.Instance.QuickJoinLobbyAsync(quick);
            currentLobby = lobby;
            string relayJoinCode = currentLobby.Data["RelayJoinCode"].Value;

            JoinAllocation joinAllocation = await RelayService.Instance.JoinAllocationAsync(relayJoinCode);

            UnityTransport transport = NetworkManager.Singleton.gameObject.GetComponent<UnityTransport>();
            RelayServerData relayServerData = AllocationUtils.ToRelayServerData(joinAllocation, "dtls");
            transport.SetRelayServerData(relayServerData);

            NetworkManager.Singleton.StartClient();
            StartPollingLobbyData();
            onLobbyJoined?.Invoke();
            return true;
        }
        catch (LobbyServiceException e)
        {
            onLobbyFailed?.Invoke();
            Debug.Log(e);
            return false;
        }
    }
    public async void JoinLobyById(string lobbyId, string clientName)
    {
        try
        {
            JoinLobbyByIdOptions joinLobbyByIdOptions = new JoinLobbyByIdOptions
            {
                Player = new Player
                {
                    Data = new Dictionary<string, PlayerDataObject>
                    {
                        {"PlayerName", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, clientName) },
                    },
                },
            };

            Lobby lobby = await LobbyService.Instance.JoinLobbyByIdAsync(lobbyId, joinLobbyByIdOptions);
            currentLobby = lobby;
            string relayJoinCode = currentLobby.Data["RelayJoinCode"].Value;

            JoinAllocation joinAllocation = await RelayService.Instance.JoinAllocationAsync(relayJoinCode);

            UnityTransport transport = NetworkManager.Singleton.gameObject.GetComponent<UnityTransport>();
            RelayServerData relayServerData = AllocationUtils.ToRelayServerData(joinAllocation, "dtls");
            transport.SetRelayServerData(relayServerData);

            NetworkManager.Singleton.StartClient();
            StartPollingLobbyData();
            onLobbyJoined?.Invoke();
        }
        catch (LobbyServiceException e)
        {
            onLobbyFailed?.Invoke();
            Debug.Log(e);
        }
    }
    
    public async void LeaveLobby()
    {
        try
        {
            await LobbyService.Instance.RemovePlayerAsync(currentLobby.Id, AuthenticationService.Instance.PlayerId);
            onLobbyLeft?.Invoke();
        }
        catch(LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

    private async void StartHeartBeat()
    {
        try
        {
            while(hostLobby != null)
            {
                await LobbyService.Instance.SendHeartbeatPingAsync(hostLobby.Id);
                await Task.Delay(15000);
            }
        }
        catch(LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }
    

    private async void StartPollingLobbyData()
    {
        try
        {
            while (currentLobby != null)
            {
                Lobby lobby = await LobbyService.Instance.GetLobbyAsync(currentLobby.Id);
                currentLobby = lobby;
                onLobbyUpdated?.Invoke(lobby);
                await Task.Delay(3000);
            }
        }
        catch(LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

    public async void StartPollingLobbiesList()
    {
        while(currentLobby == null)
        {
            List<Lobby> lobbies = await ListLobbies();
            if(lobbies != null)
                onLobbiesRefreshed?.Invoke(lobbies);
            await Task.Delay(3000);
        }
    }

    public async Task SetPlayersReadyStatus(string isReady)
    {
        UpdatePlayerOptions updatePlayerOptions = new UpdatePlayerOptions()
        {
            Data = new Dictionary<string, PlayerDataObject>()
            {
                {"IsReady", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, isReady) },
            }
        };
        await LobbyService.Instance.UpdatePlayerAsync(currentLobby.Id, AuthenticationService.Instance.PlayerId, updatePlayerOptions);
    }


}
