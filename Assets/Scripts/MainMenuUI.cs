using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField] private Button playOnlineBtn;
    [SerializeField] private Button createRoomBtn;
    [SerializeField] private Button joinRoomBtn;
    [SerializeField] private Button settingsBtn;

    private void Awake()
    {
        playOnlineBtn.onClick.AddListener(() =>
        {
            SceneManager.LoadScene("Lobbies");
        });

        createRoomBtn.onClick.AddListener(() =>
        {

        });
        
        joinRoomBtn.onClick.AddListener(() =>
        {

        }); 
        
        settingsBtn.onClick.AddListener(() =>
        {

        });
    }


}
