using System;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public class GameManager : NetworkBehaviour
{
    public static GameManager Instance { get; private set; }
    public event EventHandler<OnClickedOnGridPosEventArgs> OnClickedOnGridPos;
    public event EventHandler OnGameStarted;
    public event EventHandler<PlayerType> OnCurrentPlayerTypeChange;
    public event EventHandler<OnGameWinEventArgs> OnGameWin;
    public event EventHandler OnRematchRequested;
    public event EventHandler OnRematch;
    public event EventHandler OnGameTied;
    public class OnClickedOnGridPosEventArgs: EventArgs{
        public int x;
        public int y;
        public PlayerType playerType;
    }
    public class OnGameWinEventArgs : EventArgs
    {
        public Vector2Int centerGridPos;
        public Oreintation oreintation;
        public PlayerType winPlayerType;
    }
    public enum PlayerType
    {
        None,
        Cross,
        Circle
    }
    public enum Oreintation
    {
        Horizontal,
        Vertical,
        DiagnolA,
        DiagonlB,
    }

    public PlayerType GetLocalPlayerType => _localPlayerType;
    public PlayerType GetCurrentCurrentPlayerType => _currentPlayablePlayerType.Value;

    private PlayerType _localPlayerType;
    private NetworkVariable<PlayerType> _currentPlayablePlayerType = new NetworkVariable<PlayerType>();

    private PlayerType[,] playerTypeArray = new PlayerType[3,3];

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

    }

    //private void Update()
    //{
    //    if (Input.GetKeyDown(KeyCode.Space))
    //    {
    //        Debug.Log("Space clicked");
    //        OnGameWin?.Invoke(this, new OnGameWinEventArgs
    //        {
    //            centerGridPos = new Vector2Int(1, 1),
    //            oreintation = 0,
    //            winPlayerType = playerTypeArray[0, 1],
    //        });
    //        Debug.Log($"OnGameWin subscriber count: {OnGameWin?.GetInvocationList().Length ?? 0}");

    //    }
    //}

    public override void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
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
            NetworkManager.Singleton.SceneManager.OnLoadEventCompleted += SceneManager_OnLoadEventCompleted;
        }

        _currentPlayablePlayerType.OnValueChanged += (PlayerType prevPlayerType, PlayerType newPlayerType) =>
        {
            OnCurrentPlayerTypeChange?.Invoke(this, _currentPlayablePlayerType.Value);
        };
    }

    private void SceneManager_OnLoadEventCompleted(string sceneName, UnityEngine.SceneManagement.LoadSceneMode loadSceneMode, System.Collections.Generic.List<ulong> clientsCompleted, System.Collections.Generic.List<ulong> clientsTimedOut)
    {
        NetworkManager.Singleton.SceneManager.OnLoadEventCompleted -= SceneManager_OnLoadEventCompleted;

        _currentPlayablePlayerType.Value = PlayerType.Cross;
        TriggerOnGameStartedRpc();
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void TriggerOnGameStartedRpc()
    {
        OnGameStarted?.Invoke(this, EventArgs.Empty);
        BoostrapManager.Instance.HideLoading();
    }

    [Rpc(SendTo.Server)]
    public void CheckGridpositionRpc(int x, int y, PlayerType playerType)
    {
        if(playerType != _currentPlayablePlayerType.Value)
        {
            return;
        }

        if (playerTypeArray[x, y] != PlayerType.None) return;//already occupied

        playerTypeArray[x, y] = playerType;

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
        TestWinnerRpc();
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void TestWinnerRpc()
    {
        for(int x = 0; x < 3; x++)
        {

            bool isHorizontalWin = TestHorizontalLineWinner(x);
            bool isVerticalWin = TestVeticalLineWinner(x);
            bool isDiagnolAWin = TestDiagnolA();
            bool isDiagnolBWin = TestDiagnolB();
            Oreintation orein = isVerticalWin ? Oreintation.Vertical
                : isHorizontalWin ? Oreintation.Horizontal
                : isDiagnolAWin ? Oreintation.DiagnolA
                : Oreintation.DiagonlB;
            Vector2Int center = isVerticalWin ? new Vector2Int(x, 1) : isHorizontalWin ? new Vector2Int(1, x) : new Vector2Int(1, 1);

            if (isHorizontalWin || isVerticalWin || isDiagnolAWin || isDiagnolBWin)
            {
                TriggerOnGameWinRpc(center, orein, playerTypeArray[center.x,center.y]);
                _currentPlayablePlayerType.Value = PlayerType.None;
                return;
            }

            bool hasTied = true;
            for(int i = 0; i < playerTypeArray.GetLength(0); i++)
            {
                for(int j = 0; j < playerTypeArray.GetLength(1); j++)
                {
                    if (playerTypeArray[i, j] == PlayerType.None)
                    {
                        hasTied = false;
                        break;
                    }
                }
            }

            if (hasTied)
            {
                TriggerOnGameTiedRpc();
            }
        }  
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void TriggerOnGameTiedRpc()
    {
        OnGameTied?.Invoke(this, EventArgs.Empty);
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void TriggerOnGameWinRpc(Vector2Int center, Oreintation orein, PlayerType winPlayerType)
    {
        OnGameWin?.Invoke(this, new OnGameWinEventArgs
        {
            centerGridPos = center,
            oreintation = orein,
            winPlayerType = winPlayerType,
        });
    }

    private bool TestVeticalLineWinner(int x)
    {
        return playerTypeArray[x, 0] != PlayerType.None &&
            playerTypeArray[x, 0] == playerTypeArray[x, 1] &&
            playerTypeArray[x, 1] == playerTypeArray[x, 2];
    }

    private bool TestHorizontalLineWinner(int y)
    {
        return playerTypeArray[0, y] != PlayerType.None &&
            playerTypeArray[0, y] == playerTypeArray[1, y] &&
            playerTypeArray[1,y] == playerTypeArray[2, y];
    }

    private bool TestDiagnolA()
    {
        return (playerTypeArray[0, 0] != PlayerType.None &&
             playerTypeArray[0, 0] == playerTypeArray[1, 1] &&
             playerTypeArray[1, 1] == playerTypeArray[2, 2]);
    }

    private bool TestDiagnolB()
    {

        return (playerTypeArray[0, 2] != PlayerType.None &&
            playerTypeArray[0, 2] == playerTypeArray[1, 1] &&
            playerTypeArray[1, 1] == playerTypeArray[2, 0]);

    }

    [Rpc(SendTo.Server)]
    public void RematchRpc()
    {
        for(int i = 0; i < playerTypeArray.GetLength(0); i++)
        {
            for(int j = 0; j < playerTypeArray.GetLength(1); j++)
            {
                playerTypeArray[i, j] = PlayerType.None;
            }
        }

        _currentPlayablePlayerType.Value = PlayerType.Cross;
        TriggerOnRematchRpc();
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void TriggerOnRematchRpc()
    {
        OnRematch?.Invoke(this, EventArgs.Empty);
    }

    [Rpc(SendTo.Server)]
    public void RequestRematchServerRpc(RpcParams rpcParams = default)
    {
        ulong requesterId = rpcParams.Receive.SenderClientId;
        NotifyRematchRequestClientRpc(requesterId);
    }

    [Rpc(SendTo.ClientsAndHost)]
    public void NotifyRematchRequestClientRpc(ulong requesterId)
    {
        if (NetworkManager.Singleton.LocalClientId == requesterId) return;

        OnRematchRequested?.Invoke(this, EventArgs.Empty);
    }

    [Rpc(SendTo.Server)]
    public void DeclineRematchServerRpc()
    {
        ResetPlayerReadyStatusClientRpc();
    }
    
    [Rpc(SendTo.ClientsAndHost)]
    public void ResetPlayerReadyStatusClientRpc()
    {
        ResetPlayerReadyStatus();
    }

    private async void ResetPlayerReadyStatus()
    {
         await LobbyManager.Instance.SetPlayersReadyStatus("false");

        if (IsServer)
        {
            NetworkManager.Singleton.SceneManager.LoadScene("Lobby", UnityEngine.SceneManagement.LoadSceneMode.Single);
        }
    }
}
