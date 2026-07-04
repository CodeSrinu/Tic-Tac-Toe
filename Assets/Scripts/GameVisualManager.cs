using Unity.Netcode;
using UnityEngine;
using static UnityEngine.Rendering.GPUSort;

public class GameVisualManager : NetworkBehaviour
{
    [SerializeField] private Transform crossPrefab;
    [SerializeField] private Transform circlePrefab;

    const float GRID_SIZE = 3.1f;

    private void Start()
    {
        GameManager.Instance.OnClickedOnGridPos += HandleGridClick;
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
