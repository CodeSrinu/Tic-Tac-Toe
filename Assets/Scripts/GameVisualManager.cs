using Unity.Netcode;
using UnityEngine;
using static UnityEngine.Rendering.GPUSort;

public class GameVisualManager : NetworkBehaviour
{
    [SerializeField] private Transform crossPrefab;
    [SerializeField] private Transform circlePrefab;
    [SerializeField] private Transform lineCompletePrefab;

    const float GRID_SIZE = 3.1f;

    private void Start()
    {
        GameManager.Instance.OnClickedOnGridPos += HandleGridClick;
        GameManager.Instance.OnGameWin += GameManger_OnGameWin;
    }

    private void GameManger_OnGameWin(object sender, GameManager.OnGameWinEventArgs e)
    {
        Vector3 center = GetGridWorldPos(e.centerGridPos.x, e.centerGridPos.y);
        float rotatoinZ;
        switch (e.oreintation)
        {
            default:
                case GameManager.Oreintation.Vertical:
                    rotatoinZ = 0f;
                    break;
                case GameManager.Oreintation.Horizontal:
                    rotatoinZ = 90f;
                    break;
                case GameManager.Oreintation.DiagnolA:
                    rotatoinZ = -45f;
                    break;
                case GameManager.Oreintation.DiagonlB:
                    rotatoinZ = 45f;
                    break;

        }
        Transform lineCompletePrefabTransform = Instantiate(lineCompletePrefab, center, Quaternion.Euler(0, 0, rotatoinZ));
        lineCompletePrefabTransform.GetComponent<NetworkObject>().Spawn(true);
    }

    private void HandleGridClick(object sender, GameManager.OnClickedOnGridPosEventArgs args)
    {
        Debug.Log("HandleGridClick");
        SpawnPrefabRpc(args.x, args.y, args.playerType);
    }

    [Rpc(SendTo.Server)]
    public void SpawnPrefabRpc(int x, int y, GameManager.PlayerType playerType)
    {
        Debug.Log("SpawnPrefabRpc");

        Transform prefab;

        switch (playerType)
        {
            default:
            case GameManager.PlayerType.Cross:
                prefab = crossPrefab;
                break;
            case GameManager.PlayerType.Circle:
                prefab = circlePrefab;
                break;
        }


        Transform crossPrefabTransform = Instantiate(prefab, GetGridWorldPos(x, y), Quaternion.identity);
        crossPrefabTransform.GetComponent<NetworkObject>().Spawn(true);

    }

    private Vector2 GetGridWorldPos(int x, int y)
    {
        return new Vector2(-GRID_SIZE + x * GRID_SIZE, -GRID_SIZE + y * GRID_SIZE);
    }

}
