using System;
using System.Linq;
using System.Threading.Tasks;
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
    [SerializeField] private TextMeshProUGUI  hostReadyTxtComp;
    [SerializeField] private TextMeshProUGUI  clientReadyTxtComp;
    [SerializeField] private Image playerTwoAvatarComponent;
    [SerializeField] private Sprite defaultAvatar;
    [SerializeField] private Sprite avatarAfterJoined;
    [SerializeField] private Button readyBtn;
    [SerializeField] private Button backBtn;
    [SerializeField] private Button settingsBtn;
    [SerializeField] private GameObject confirmExitPanel;

    private bool isReady = false;
    private bool _hasReceivedFirstPoll = false;

    private void Start()
    {
        _hasReceivedFirstPoll = false;
        isReady = false;
        readyBtn.GetComponentInChildren<TextMeshProUGUI>().text = "Ready";
        hostReadyTxtComp.text = "Not Ready";
        clientReadyTxtComp.text = "Not Ready";



        readyBtn.onClick.AddListener(() =>
        {
            isReady = !isReady;

            if (NetworkManager.Singleton.IsHost)
            {
                hostReadyTxtComp.text = isReady ? "Ready" : "Not Ready";
            }
            else
            {
                clientReadyTxtComp.text = isReady ? "Ready" : "Not Ready";
            }

            if (isReady)
            {
                ResetPlayersReadyStatus("true");
                readyBtn.GetComponentInChildren<TextMeshProUGUI>().text = "Cancel";

                SetUpLobbyPanel();
            }
            else
            {
                UpdateReadyStatusUI();
                ResetPlayersReadyStatus("false");
                readyBtn.GetComponentInChildren<TextMeshProUGUI>().text = "Ready";
            }
        });


        SetUpLobbyPanel();
        LobbyManager.Instance.onLobbyUpdated += LobbyManager_onLobbyUpdated;
        backBtn.onClick.AddListener(() =>
        {
            ShowConfirmExitPanel();
        });

        settingsBtn.onClick.AddListener(() =>
        {
            BoostrapManager.instance.ShowSettings();
        });
    }
    private async void ResetPlayersReadyStatus(string isReady)
    {
        await LobbyManager.Instance.SetPlayersReadyStatus(isReady);
    }

    private void OnDestroy()
    {
        LobbyManager.Instance.onLobbyUpdated -= LobbyManager_onLobbyUpdated;
    }

    private void LobbyManager_onLobbyUpdated(Unity.Services.Lobbies.Models.Lobby lobby)
    {
        _hasReceivedFirstPoll = true;
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

        if (!_hasReceivedFirstPoll) return;

        bool isAllReady = LobbyManager.Instance.CurrentLobby.Players.
            All(player => player.Data.ContainsKey("IsReady") && player.Data["IsReady"].Value == "true");

        UpdateReadyStatusUI();

        if (isAllReady && NetworkManager.Singleton.IsHost)
        {
            NetworkManager.Singleton.SceneManager.LoadScene("Game", UnityEngine.SceneManagement.LoadSceneMode.Single);
        }
    }

    private void ShowConfirmExitPanel()
    {
        confirmExitPanel.SetActive(true);
    }

    private void UpdateReadyStatusUI()
    {
        bool ishostReady = 
            LobbyManager.Instance.CurrentLobby.Players.Count > 0 &&
            LobbyManager.Instance.CurrentLobby.Players[0].Data.ContainsKey("IsReady") &&
            LobbyManager.Instance.CurrentLobby.Players[0].Data["IsReady"].Value == "true";

        hostReadyTxtComp.text = ishostReady ? "Ready" : "Not Ready";

        bool isClientReady =
            LobbyManager.Instance.CurrentLobby.Players.Count > 1 &&
            LobbyManager.Instance.CurrentLobby.Players[1].Data.ContainsKey("IsReady") && 
            LobbyManager.Instance.CurrentLobby.Players[1].Data["IsReady"].Value == "true";

        clientReadyTxtComp.text = isClientReady ? "Ready" : "Not Ready";
    }
}
