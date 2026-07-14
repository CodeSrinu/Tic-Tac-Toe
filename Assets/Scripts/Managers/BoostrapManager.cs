using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BoostrapManager : MonoBehaviour
{
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private LoadingScreenManager loadingScreenManager;
    [SerializeField] private GameObject splashScreen;
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
        loadingScreenManager.gameObject.SetActive(false);

    }

    public void ShowSettings() => PanelAnimator.Show(settingsPanel);
    public void ShowLoading() => loadingScreenManager.Show();
    public void HideLoading() => loadingScreenManager.Hide();

    public void LoadMainMenuAndFadeSplashScreen() 
    {
        SceneManager.LoadScene("MainMenu");
        splashScreen.transform.GetComponent<CanvasGroup>().DOFade(0, 0.5f).OnComplete(()=> { 
            splashScreen.SetActive(false);
        });
    }



}
