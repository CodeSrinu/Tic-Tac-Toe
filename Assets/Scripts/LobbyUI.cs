using TMPro;
using UnityEngine;
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
    [SerializeField] private Button startBtn;

    private void Start()
    {
        startBtn.onClick.AddListener(() =>
        {
            
        });

        SetUpLobbyPanel();
        LobbyManager.Instance.onLobbyUpdated += LobbyManager_onLobbyUpdated;
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
        lobbyTittle.text = playerOneName.text + " Lobby";
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
    }

}
