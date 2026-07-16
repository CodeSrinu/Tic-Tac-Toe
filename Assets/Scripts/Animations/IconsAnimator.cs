using DG.Tweening;
using UnityEngine;

public class IconsAnimator : MonoBehaviour
{
    [SerializeField] private bool canRotate = false;
    [SerializeField] private bool canMove = false;

    [SerializeField] private float rotationDuration = 20f;
    [SerializeField] private float zRotation = 180f;
    Vector3 originalPos;
    private void Start()
    {
        originalPos = transform.localPosition;
        if (canRotate)
        {
            RotateContinuos();
        }
        if(canMove)
        {
            SlightMove();
        }
    }

    public void RotateContinuos()
    {
        transform.DORotate(new Vector3(0,0,zRotation), rotationDuration).SetEase(Ease.InOutBack).SetLoops(-1, LoopType.Yoyo);
    }
    public void SlightMove()
    {
        Sequence seq = DOTween.Sequence();

        seq
            .Append(transform.DOLocalMoveX(originalPos.x + 6f, 1f).SetEase(Ease.InBack)).SetDelay(Random.Range(0.5f, 2f));

        seq.SetLoops(-1, LoopType.Yoyo);
    }
}
