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
            SceneManager.LoadScene(buildIndex - 1);
        });

        noBtn.onClick.AddListener(() =>
        {
            gameObject.SetActive(false);
        });
    }
}
