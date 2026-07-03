using UnityEngine;
using UnityEngine.EventSystems;

public class GridComp : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private int x;
    [SerializeField] private int y;
    public void OnPointerClick(PointerEventData eventData)
    {
        GameManager.Instance.CheckGridposition(x, y);
    }
}
