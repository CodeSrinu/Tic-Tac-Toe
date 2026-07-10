using UnityEngine;

public class LoadingScreenManager : MonoBehaviour
{
    public static LoadingScreenManager Instance { get; private set; }

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
        Debug.Log("Loading Screen Showed");
        gameObject.SetActive(true);
    }


    public void Hide()
    {
        Debug.Log("Loading Screen got hide");
        gameObject.SetActive(false);
    }
}
