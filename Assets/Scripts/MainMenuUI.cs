using System.Threading.Tasks;
using Unity.Services.Lobbies.Models;
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
            CreateRoomFlow();
        });
        
        joinRoomBtn.onClick.AddListener(() =>
        {

        }); 
        
        settingsBtn.onClick.AddListener(() =>
        {

        });
    }


    public async void CreateRoomFlow()
    {
        LobbyManager.Instance.CreateLobby();

    }

}
