using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameOverUI : MonoBehaviour
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
        Debug.Log("In GameOverUI Start");
        rematchBtn.onClick.AddListener(() => 
        {
            BoostrapManager.Instance.ShowLoading();
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

    private void OnDestroy()
    {
        GameManager.Instance.OnGameWin -= GameManager_OnGameWin;
        GameManager.Instance.OnGameTied -= GameManager_OnGameTied;
        GameManager.Instance.OnRematch -= GameManager_OnRematch;
    }

    private void GameManager_OnRematch(object sender, System.EventArgs e)
    {
        PanelAnimator.Hide(gameObject);
    }

    private void GameManager_OnGameTied(object sender, System.EventArgs e)
    {
        resultTextComp.text = "It's a Tie";
        resultIconComp.sprite = tieIcon;
        PanelAnimator.Show(gameObject);
    }

    private void GameManager_OnGameWin(object sender, GameManager.OnGameWinEventArgs e)
    {
        Debug.Log("OnGameWin triggered inside gameOver UI");

        if (GameManager.Instance.GetLocalPlayerType == e.winPlayerType)
        {
            resultTextComp.text = "Victory is yours!";
            resultIconComp.sprite = winIcon;
        }
        else
        {
            resultTextComp.text = "Your strategy has failed!";
            resultIconComp.sprite = loseIcon;
        }
        PanelAnimator.Show(gameObject);

    }

}
