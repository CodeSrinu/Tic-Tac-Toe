using DG.Tweening;
using UnityEngine;

public class LoadingScreenManager : MonoBehaviour
{
    public static LoadingScreenManager Instance { get; private set; }

    public GameObject leftPage;
    public GameObject rightPage;

    private void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(transform.parent);
    }


    public void Show()
    {
        gameObject.transform.GetComponent<CanvasGroup>().alpha = 0f;
        gameObject.SetActive(true);

        gameObject.transform.GetComponent<CanvasGroup>().DOFade(1, 0.4f).OnComplete(() =>
        {
            //AudioManager.Instance.PlayLoadingSound();
        });
    }


    public void Hide()
    {
        //AudioManager.Instance.StopLoadingSound();
        gameObject.transform.GetComponent<CanvasGroup>().DOFade(0, 0.4f).OnComplete(() =>
        {
            gameObject.SetActive(false);
        });
    }

}
