using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;


public class GameVisualManager : NetworkBehaviour
{
    [SerializeField] private Transform crossPrefab;
    [SerializeField] private Transform circlePrefab;
    [SerializeField] private Transform lineCompletePrefab;
    [SerializeField] private Transform confirmRematchPanel;
    [SerializeField] private Button rematchConfirmBtn;
    [SerializeField] private Button rematchRejectBtn;
    private List<GameObject> allSpawnedObjs = new List<GameObject>();

    const float GRID_SIZE = 3.1f;

    private void Start()
    {
        GameManager.Instance.OnClickedOnGridPos += HandleGridClick;
        GameManager.Instance.OnGameWin += GameManger_OnGameWin;
        GameManager.Instance.OnRematchRequested += GameManager_OnRematchRequested;
        GameManager.Instance.OnRematch += GameManager_OnRematch;

        rematchConfirmBtn.onClick.AddListener(() =>
        {
            GameManager.Instance.RematchRpc();
            confirmRematchPanel.gameObject.SetActive(false);
        });

        rematchRejectBtn.onClick.AddListener(() =>
        {
            GameManager.Instance.DeclineRematchServerRpc();
        });
    }

    private void GameManager_OnRematchRequested(object sender, System.EventArgs e)
    {
        confirmRematchPanel.gameObject.SetActive(true);
    }
    private void GameManager_OnRematch(object sender, System.EventArgs e)
    {
        if (!NetworkManager.Singleton.IsHost) return;

        foreach(GameObject obj in allSpawnedObjs)
        {
            Destroy(obj);
        }
        allSpawnedObjs.Clear();
    }
    private void GameManger_OnGameWin(object sender, GameManager.OnGameWinEventArgs e)
    {
        if (!NetworkManager.Singleton.IsHost) return;

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
        allSpawnedObjs.Add(lineCompletePrefabTransform.gameObject);
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
        allSpawnedObjs.Add(crossPrefabTransform.gameObject);
    }

    private Vector2 GetGridWorldPos(int x, int y)
    {
        return new Vector2(-GRID_SIZE + x * GRID_SIZE, -GRID_SIZE + y * GRID_SIZE);
    }

}
