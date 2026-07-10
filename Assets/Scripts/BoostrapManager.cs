using UnityEngine;

public class BoostrapManager : MonoBehaviour
{
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private LoadingScreenManager loadingScreenManager;
    public static BoostrapManager Instance;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        DontDestroyOnLoad(gameObject);
        settingsPanel.SetActive(false);
        loadingScreenManager.Hide();
    }

    public void ShowSettings() => settingsPanel.SetActive(true);
    public void ShowLoading() => loadingScreenManager.Show();
    public void HideLoading() => loadingScreenManager.Hide();


}
