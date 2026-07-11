using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ConfirmExitPanelUI : MonoBehaviour
{
    [SerializeField] private Button yesBtn;
    [SerializeField] private Button noBtn;

    private void Awake()
    {
        yesBtn.onClick.AddListener(() =>
        {
            int buildIndex = SceneManager.GetActiveScene().buildIndex;
            if(buildIndex == 4)
            {
                BoostrapManager.Instance.ShowLoading();
                GameManager.Instance.DeclineRematchServerRpc();
            }
            else if(buildIndex == 3)
            {
                LobbyManager.Instance.LeaveLobby();
            }
            else if(buildIndex == 1)
            {
                Application.Quit();
                return;
            }

            SceneManager.LoadScene(buildIndex - 1);

        });

        noBtn.onClick.AddListener(() =>
        {
            PanelAnimator.Hide(gameObject);
        });
    }
}
