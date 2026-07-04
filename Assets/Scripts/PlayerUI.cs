using UnityEngine;

public class PlayerUI : MonoBehaviour
{
    [SerializeField] private GameObject crossArrowImg;
    [SerializeField] private GameObject circleArrowImg;
    [SerializeField] private GameObject crossYouText;
    [SerializeField] private GameObject circleYouText;



    private void Awake()
    {
        crossArrowImg.SetActive(false);
        circleArrowImg.SetActive(false);
        crossYouText.SetActive(false);
        circleYouText.SetActive(false);
    }

    private void Start()
    {
        GameManager.Instance.OnGameStarted += PlayerUI_OnGameStarted;
        GameManager.Instance.OnCurrentPlayerTypeChange += PlayerUI_OnCurrentPlayerTypeChange;
    }

    private void PlayerUI_OnCurrentPlayerTypeChange(object sender, GameManager.PlayerType playerType)
    {
        UpdateArrowImage(playerType);
    }

    private void PlayerUI_OnGameStarted(object sender, System.EventArgs e)
    {
        if(GameManager.Instance.GetLocalPlayerType == GameManager.PlayerType.Cross)
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
}
