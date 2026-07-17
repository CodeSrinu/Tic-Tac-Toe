using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonAnimator : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    private Sequence idleSequence;
    Vector2 originalAnchoredPos;
    public bool CanMoveAndRotate = true;

    private void Start()
    {
        originalAnchoredPos = gameObject.GetComponent<RectTransform>().anchoredPosition;
        if (CanMoveAndRotate)
        {
            StartMovement();
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        idleSequence?.Pause();

        Sequence seq = DOTween.Sequence();
        float localZRotation = Random.Range(-5, 5);
        float scale = 0.95f;
        float duration = 0.06f;

        AudioManager.Instance.PlayBtnClickSound();
        seq
            .Append(transform.DORotate(new Vector3(0, 0, localZRotation), duration).SetEase(Ease.OutQuad))
            .Join(transform.DOScale(scale, duration));
    }

    public void OnPointerUp(PointerEventData eventData)
    {

        Sequence seq = DOTween.Sequence();
        float zRotation = 0f;
        float scale = 1f;
        float duration = 0.12f;

        seq
            .Append(transform.DOScale(scale, duration).SetEase(Ease.InOutSine))
            .Join(transform.DORotate(new Vector3(0, 0, zRotation), duration))
            .OnComplete(() =>
             {
                 idleSequence?.Play();
             });
    }

    private void StartMovement()
    {
        idleSequence?.Kill();
        idleSequence = DOTween.Sequence();

        float randomDuration = Random.Range(2f, 3f);
        float randomDelay = Random.Range(0, 1f);

        idleSequence = DOTween.Sequence();

        Vector2 targetPos = originalAnchoredPos + new Vector2(Random.Range(-4f, 4f), Random.Range(-4f, 4f));
        float targetRotation = Random.Range(-3f, 3f);

        idleSequence
            .Append(transform.DORotate(new Vector3(0, 0, targetRotation), randomDuration)
            .SetEase(Ease.InOutSine))
            .Join(gameObject.GetComponent<RectTransform>().DOAnchorPos(originalAnchoredPos + new Vector2(Random.Range(-5f,5f), Random.Range(-3f, 3f)), randomDuration)
            .SetEase(Ease.InOutSine));


        idleSequence
            .SetLoops(-1, LoopType.Yoyo)
            .SetDelay(randomDelay);
    }

    private void OnDestroy()
    {
        idleSequence?.Kill();
    }
}
