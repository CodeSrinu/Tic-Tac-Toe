using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    [SerializeField] private GameObject crossArrowImg;
    [SerializeField] private GameObject circleArrowImg;
    [SerializeField] private GameObject penIcon;
    [SerializeField] private TextMeshProUGUI playerNameTxtComp;
    [SerializeField] private TextMeshProUGUI opponentNameTxtComp;
    [SerializeField] private GameObject confirmExitPanel;
    [SerializeField] private Button backBtn;
    [SerializeField] private Button settingsBtn;




    private void Awake()
    {
        crossArrowImg.SetActive(false);
        circleArrowImg.SetActive(false);
    }

    private void Start()
    {
        GameManager.Instance.OnGameStarted += GameManager_OnGameStarted;
        GameManager.Instance.OnCurrentPlayerTypeChange += GameManager_OnCurrentPlayerTypeChange;

        backBtn.onClick.AddListener(() =>
        {
            ShowConfirmToExit();
        });

        settingsBtn.onClick.AddListener(() =>
        {
            BoostrapManager.Instance.ShowSettings();
        });
    }

    private void GameManager_OnCurrentPlayerTypeChange(object sender, GameManager.PlayerType playerType)
    {
        UpdatePenAnimation(playerType);
    }

    private void GameManager_OnGameStarted(object sender, System.EventArgs e)
    {
        playerNameTxtComp.text = LobbyManager.Instance.CurrentLobby.Players[0].Data["PlayerName"].Value;
        opponentNameTxtComp.text = LobbyManager.Instance.CurrentLobby.Players[1].Data["PlayerName"].Value;

        penIcon.transform.position = new Vector3(playerNameTxtComp.transform.position.x, penIcon.transform.position.y);

        UpdatePenAnimation(GameManager.Instance.GetCurrentCurrentPlayerType);
    }

    private void UpdatePenAnimation(GameManager.PlayerType playerType)
    {
        float x = 0f;
        switch (playerType)
        {
            default:
            case GameManager.PlayerType.Cross:
                x = playerNameTxtComp.transform.position.x;
                break;
            case GameManager.PlayerType.Circle:
                x = opponentNameTxtComp.transform.position.x;
                break;
        }
        PlayPenAnimation(x);
    }

    private void PlayPenAnimation(float x)
    {
        penIcon.transform.DOMoveX(x, 1.5f).SetEase(Ease.InOutQuart);
    }

    private void ShowConfirmToExit()
    {
        PanelAnimator.Show(confirmExitPanel);
    }
}
