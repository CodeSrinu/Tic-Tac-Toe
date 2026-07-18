using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameResult : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI resultTextComp;
    [SerializeField] private Image resultIconComp;
    [SerializeField] private Sprite winIcon;
    [SerializeField] private Sprite loseIcon;
    [SerializeField] private Sprite tieIcon;
    [SerializeField] private Button rematchBtn;
    [SerializeField] private Button backToLobby;


    private void Start()
    {
        rematchBtn.onClick.AddListener(() =>
        {
            GameManager.Instance.RequestRematchServerRpc();
        });

        backToLobby.onClick.AddListener(() =>
        {
            PanelAnimator.Hide(gameObject);
            GameManager.Instance.DeclineRematchServerRpc();
        });

        GameManager.Instance.OnGameWin += GameManager_OnGameWin;

        GameManager.Instance.OnGameTied += GameManager_OnGameTied;
        GameManager.Instance.OnRematch += GameManager_OnRematch;

        gameObject.SetActive(false);
    }


    private void GameManager_OnRematch(object sender, System.EventArgs e)
    {
        PanelAnimator.Hide(gameObject);
    }

    private void GameManager_OnGameTied(object sender, System.EventArgs e)
    {
        resultTextComp.text = "It's a Tie";
        resultIconComp.sprite = tieIcon;
        Invoke("ShowTie", 0.8f);
    }

    private void GameManager_OnGameWin(object sender, GameManager.OnGameWinEventArgs e)
    {

        if (GameManager.Instance.GetLocalPlayerType == e.winPlayerType)
        {
            resultTextComp.text = "Victory is yours!";
            resultIconComp.sprite = winIcon;
            Invoke("ShowWin", 0.8f);
        }
        else
        {
            resultTextComp.text = "Your strategy has failed!";
            resultIconComp.sprite = loseIcon;
            Invoke("ShowLose", 0.8f);

        }

    }

    private void ShowWin()
    {
        PanelAnimator.Show(gameObject);
        AudioManager.Instance.PlayWinSound();
    }
    private void ShowTie()
    {
        PanelAnimator.Show(gameObject);
        AudioManager.Instance.PlayTieSound();
    }
    private void ShowLose()
    {
        PanelAnimator.Show(gameObject);
        AudioManager.Instance.PlayLoseSound();
    }

}

