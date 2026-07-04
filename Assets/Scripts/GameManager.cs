using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class GameManager : NetworkBehaviour
{
    public static GameManager Instance { get; private set; }
    public event EventHandler<OnClickedOnGridPosEventArgs> OnClickedOnGridPos;
    public event EventHandler OnGameStarted;
    public event EventHandler<PlayerType> OnCurrentPlayerTypeChange;
    public class OnClickedOnGridPosEventArgs{
        public int x;
        public int y;
        public PlayerType playerType;
    }
    public enum PlayerType
    {
        None,
        Cross,
        Circle
    }

    public PlayerType GetLocalPlayerType => _localPlayerType;
    public PlayerType GetCurrentCurrentPlayerType => _currentPlayablePlayerType.Value;

    private PlayerType _localPlayerType;
    private NetworkVariable<PlayerType> _currentPlayablePlayerType = new NetworkVariable<PlayerType>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public override void OnNetworkSpawn()
    {

        if(NetworkManager.Singleton.LocalClientId == 0)
        {
            _localPlayerType = PlayerType.Cross;
        }
        else
        {
            _localPlayerType = PlayerType.Circle;
        }

        if (IsServer)
        {
            NetworkManager.Singleton.OnClientConnectedCallback += NetworkManager_OnClientConnectedCallback;
        }

        _currentPlayablePlayerType.OnValueChanged += (PlayerType prevPlayerType, PlayerType newPlayerType) =>
        {
            OnCurrentPlayerTypeChange?.Invoke(this, _currentPlayablePlayerType.Value);
        };
    }

    private void NetworkManager_OnClientConnectedCallback(ulong obj)
    {
        if (NetworkManager.Singleton.ConnectedClientsList.Count >= 2)
        {
            _currentPlayablePlayerType.Value = PlayerType.Cross;
            TriggerOnGameStartedRpc();
        }
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void TriggerOnGameStartedRpc()
    {
        OnGameStarted?.Invoke(this, EventArgs.Empty);
    }

    [Rpc(SendTo.Server)]
    public void CheckGridpositionRpc(int x, int y, PlayerType playerType)
    {
        if(playerType != _currentPlayablePlayerType.Value)
        {
            return;
        }

        Debug.Log("CheckGridposition " + x + "," + y);
        OnClickedOnGridPos?.Invoke(this, new OnClickedOnGridPosEventArgs
        {
            x = x,
            y = y,
            playerType = playerType
        });

        switch (_currentPlayablePlayerType.Value)
        {
            default:
                case PlayerType.Cross:
                _currentPlayablePlayerType.Value = PlayerType.Circle;
                break;
                case PlayerType.Circle:
                _currentPlayablePlayerType.Value = PlayerType.Cross;
                break;
        }
    }

}
