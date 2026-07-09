using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    [SerializeField] private GameObject crossArrowImg;
    [SerializeField] private GameObject circleArrowImg;
    [SerializeField] private GameObject crossYouText;
    [SerializeField] private GameObject circleYouText;
    [SerializeField] private GameObject confirmExitPanel;
    [SerializeField] private Button backBtn;




    private void Awake()
    {
        crossArrowImg.SetActive(false);
        circleArrowImg.SetActive(false);
        crossYouText.SetActive(false);
        circleYouText.SetActive(false);
    }

    private void Start()
    {
        GameManager.Instance.OnGameStarted += GameManager_OnGameStarted;
        GameManager.Instance.OnCurrentPlayerTypeChange += GameManager_OnCurrentPlayerTypeChange;

        backBtn.onClick.AddListener(() =>
        {
            ShowConfirmToExit();
        });
    }

    private void GameManager_OnCurrentPlayerTypeChange(object sender, GameManager.PlayerType playerType)
    {
        UpdateArrowImage(playerType);
    }

    private void GameManager_OnGameStarted(object sender, System.EventArgs e)
    {
        if (GameManager.Instance.GetLocalPlayerType == GameManager.PlayerType.Cross)
        {
            crossYouText?.SetActive(true);
            circleYouText?.SetActive(false);
        }
        else
        {
            circleYouText?.SetActive(true);
            crossYouText?.SetActive(false);
        }

        UpdateArrowImage(GameManager.Instance.GetCurrentCurrentPlayerType);
    }

    private void UpdateArrowImage(GameManager.PlayerType playerType)
    {
        switch (playerType)
        {
            default:
            case GameManager.PlayerType.Cross:
                crossArrowImg.SetActive(true);
                circleArrowImg.SetActive(false);
                break;
            case GameManager.PlayerType.Circle:
                circleArrowImg.SetActive(true);
                crossArrowImg.SetActive(false);
                break;

        }
    }

    private void ShowConfirmToExit()
    {
        confirmExitPanel.SetActive(true);
    }
}
