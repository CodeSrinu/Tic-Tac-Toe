using UnityEngine;

public class GameVisualManager : MonoBehaviour
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
        Instantiate(crossPrefab, GetGridWorldPos(args.x, args.y), Quaternion.identity);
    }

    private Vector2 GetGridWorldPos(int x, int y)
    {
        return new Vector2(-GRID_SIZE + x * GRID_SIZE, -GRID_SIZE + y * GRID_SIZE);
    }

}
