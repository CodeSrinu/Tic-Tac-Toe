using UnityEngine;

public class BoostrapManager : MonoBehaviour
{
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private LoadingScreenManager loadingScreenManager;
    public static BoostrapManager instance;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;

        DontDestroyOnLoad(gameObject);
        settingsPanel.SetActive(false);
        loadingScreenManager.Hide();
    }

    public void OpenSetings() => settingsPanel.SetActive(true);
    public void ShowLoading() => loadingScreenManager.Show();
    public void HideLoading() => loadingScreenManager.Hide();


}
