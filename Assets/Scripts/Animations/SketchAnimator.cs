using DG.Tweening;
using UnityEngine;

public static class SketchAnimator
{
    public static void DrawInSequence(GameObject[] objs, float stagerDelay)
    {
        for(int i = 0; i < objs.Length; i++)
        {
            DrawDoodle(objs[i], stagerDelay * i);
        }
    }

    public static void DrawDoodle(GameObject doodle, float delay)
    {
        doodle.SetActive(true);
        Vector3 originalScale = doodle.transform.localScale;
        doodle.transform.localScale = Vector3.zero;

        Sequence seq = DOTween.Sequence();

        seq
            .Append(doodle.transform.DOScale(originalScale, 0.5f).SetDelay(delay));

        SlightMove(doodle);


    }

    public static void RotateContinuos(GameObject obj)
    {
        obj.transform.DORotate(new Vector3(0, 0, 180f), 15).SetEase(Ease.InOutBack).SetLoops(-1, LoopType.Yoyo);
    }
    public static void SlightMove(GameObject obj)
    {
        Sequence seq = DOTween.Sequence();

        seq
            .Append(obj.transform.DOLocalMove(obj.transform.localPosition +new Vector3(Random.Range(-4, 4), Random.Range(-4, 4)), 1f).SetEase(Ease.InOutBack)).SetDelay(Random.Range(0.5f, 2f));

        seq.SetLoops(-1, LoopType.Yoyo);
    }
}
