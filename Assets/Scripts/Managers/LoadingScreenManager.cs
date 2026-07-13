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
        gameObject.SetActive(true);
        gameObject.transform.GetComponent<CanvasGroup>().DOFade(1, 0.4f);
    }


    public void Hide()
    {
        gameObject.transform.GetComponent<CanvasGroup>().DOFade(0, 0.4f).OnComplete(() =>
        {
            gameObject.SetActive(false);
        });
    }

}
