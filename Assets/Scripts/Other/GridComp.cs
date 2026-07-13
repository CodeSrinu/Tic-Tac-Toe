using UnityEngine;
using UnityEngine.EventSystems;

public class GridComp : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private int x;
    [SerializeField] private int y;
    public void OnPointerClick(PointerEventData eventData)
    {
        GameManager.Instance.CheckGridpositionRpc(x, y, GameManager.Instance.GetLocalPlayerType);
    }
}
