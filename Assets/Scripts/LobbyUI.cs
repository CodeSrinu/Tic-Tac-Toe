using System.Linq;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LobbyUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI  playerOneName;
    [SerializeField] private TextMeshProUGUI  playerTwoName;
    [SerializeField] private TextMeshProUGUI  roomCode;
    [SerializeField] private TextMeshProUGUI  lobbyTittle;
    [SerializeField] private Image playerTwoAvatarComponent;
    [SerializeField] private Sprite defaultAvatar;
    [SerializeField] private Sprite avatarAfterJoined;
    [SerializeField] private Button readyBtn;
    [SerializeField] private Button backBtn;
    [SerializeField] private GameObject confirmExitPanel;

    private bool isReady = false;

    private void Start()
    {
        readyBtn.onClick.AddListener(() =>
        {
            isReady = !isReady;
            if (isReady)
            {
                LobbyManager.Instance.SetPlayersReadyStatus("true");
                readyBtn.GetComponentInChildren<TextMeshProUGUI>().text = "Cancel";
                SetUpLobbyPanel();
            }
            else
            {
                LobbyManager.Instance.SetPlayersReadyStatus("false");
                readyBtn.GetComponentInChildren<TextMeshProUGUI>().text = "Ready";
            }
        });


        SetUpLobbyPanel();
        LobbyManager.Instance.onLobbyUpdated += LobbyManager_onLobbyUpdated;
        backBtn.onClick.AddListener(() =>
        {
            ShowConfirmExitPanel();
        });
    }

    private void OnDestroy()
    {
        LobbyManager.Instance.onLobbyUpdated -= LobbyManager_onLobbyUpdated;
    }

    private void LobbyManager_onLobbyUpdated(Unity.Services.Lobbies.Models.Lobby lobby)
    {
        SetUpLobbyPanel();
    }

    public void SetUpLobbyPanel()
    {
        lobbyTittle.text = LobbyManager.Instance.CurrentLobby.Name;
        roomCode.text = LobbyManager.Instance.GetRoomCode;

        playerOneName.text = LobbyManager.Instance.CurrentLobby.Players[0].Data["PlayerName"].Value;
        if(LobbyManager.Instance.CurrentLobby.Players.Count > 1)
        {
            playerTwoAvatarComponent.sprite = avatarAfterJoined;
            playerTwoName.text = LobbyManager.Instance.CurrentLobby.Players[1].Data["PlayerName"].Value;
        }
        else
        {
            playerTwoAvatarComponent.sprite = defaultAvatar;
            playerTwoName.text = "Waiting...";
        }

        bool isAllReady = LobbyManager.Instance.CurrentLobby.Players.
            All(player => player.Data.ContainsKey("IsReady") && player.Data["IsReady"].Value == "true");

        if (isAllReady && NetworkManager.Singleton.IsHost)
        {
            NetworkManager.Singleton.SceneManager.LoadScene("Game", UnityEngine.SceneManagement.LoadSceneMode.Single);
        }
    }

    private  void ShowConfirmExitPanel()
    {
        confirmExitPanel.SetActive(true);
    }
}
