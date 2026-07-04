using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class GameOverUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI resultTextComp;
    [SerializeField] private Color winColor;
    [SerializeField] private Color loseColor;
    [SerializeField] private Button retryBtn;

    private void Start()
    {
        retryBtn.onClick.AddListener(() =>
        {
            NetworkManager.Singleton.SceneManager.LoadScene("Game", UnityEngine.SceneManagement.LoadSceneMode.Single);
        });

        Hide();
        GameManager.Instance.OnGameWin += GameManager_OnGameWin;
    }

    private void GameManager_OnGameWin(object sender, GameManager.OnGameWinEventArgs e)
    {
        if(GameManager.Instance.GetLocalPlayerType == e.winPlayerType)
        {
            resultTextComp.text = "You Win!";
            resultTextComp.color = winColor;
        }
        else
        {
            resultTextComp.text = "You Lose!";
            resultTextComp.color = loseColor;
        }
        Show();
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }

    private void Show()
    {
        gameObject.SetActive(true);
    }
}
