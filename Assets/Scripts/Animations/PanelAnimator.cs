using DG.Tweening;
using TMPro;
using UnityEngine;

public static class PanelAnimator
{
    public static float moveYDuration = 0.4f;
    public static float rotateBackYDuration = 0.2f;
    public static void Show(GameObject go, string error = "")
    {
        if (go.activeSelf) return;

        if(error != "")
        {
            TextMeshProUGUI txtComp = go.GetComponentInChildren<TextMeshProUGUI>();
            txtComp.text = error;
        }

        go.SetActive(true);
        go.transform.DOKill();
        go.transform.localPosition = new Vector3(0, 1550, 0);
        go.transform.localRotation = Quaternion.identity;

        Sequence sequence = DOTween.Sequence();

        sequence
            .Append(go.transform.DOLocalMoveY(0f, moveYDuration))
            .Join(go.transform.DOLocalRotate(new Vector3(0, 0, 12f), moveYDuration)).SetDelay(0.3f)
            .Append(go.transform.DOLocalRotate(Vector3.zero, rotateBackYDuration).SetEase(Ease.OutBack));
        
    }

    public static void Hide(GameObject go)
    {
        go.transform.DOKill();

        Sequence sequence = DOTween.Sequence();

        sequence
            .Append(go.transform.DOLocalRotate(new Vector3(0, 0, 12f), moveYDuration))
            .Join(go.transform.DOLocalMoveY(1550f, moveYDuration).SetEase(Ease.InBack))

            .OnComplete(() =>
            {
                go.SetActive(false);
                go.transform.localPosition = Vector3.zero;
            });
    }

}
