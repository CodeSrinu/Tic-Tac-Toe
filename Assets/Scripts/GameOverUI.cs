using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class GameOverUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI resultTextComp;
    [SerializeField] private Color winColor;
    [SerializeField] private Color loseColor;
    [SerializeField] private Color tieColor;
    [SerializeField] private Button rematchBtn;

    private void Start()
    {
        rematchBtn.onClick.AddListener(() => 
        {
            BoostrapManager.Instance.ShowLoading();
            GameManager.Instance.RequestRematchServerRpc();
        });

        Hide();

        GameManager.Instance.OnGameWin += GameManager_OnGameWin;
        GameManager.Instance.OnGameTied += GameManager_OnGameTied;
        GameManager.Instance.OnRematch += GameManager_OnRematch;
    }

    private void GameManager_OnRematch(object sender, System.EventArgs e)
    {
        Hide();
    }

    private void GameManager_OnGameTied(object sender, System.EventArgs e)
    {
        resultTextComp.text = "Tie";
        resultTextComp.color = tieColor;
        Show();
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
