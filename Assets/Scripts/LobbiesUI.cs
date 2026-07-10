using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LobbiesUI : MonoBehaviour
{
    [SerializeField] private TMP_InputField searchInputComp;

    [SerializeField] private Transform lobbiesContainer;
    [SerializeField] private GameObject lobbyItemPrefab;

    [SerializeField] private Sprite[] roomAvatars;


    [SerializeField] private GameObject joinRoomPanel;
    [SerializeField] private TMP_InputField roomCodeInputComp;
    [SerializeField] private TextMeshProUGUI roomCodeErrorTxtComp;
    [SerializeField] private Button joinRoomConfirmBtn;
    [SerializeField] private Button joinRoomPanelCancelBtn;
    [SerializeField] private List<Lobby> listedLobbies = new List<Lobby>();
    private string playerName;

    [SerializeField] private Button backBtn;
    [SerializeField] private Button settingsBtn;

    private void Start()
    {
        LobbyManager.Instance.onLobbyJoined += LobbyManager_onLobbyJoined;
        LobbyManager.Instance.onLobbiesRefreshed += LobbyManager_onLobbiesRefreshed;

        LobbyManager.Instance.StartPollingLobbiesList();

        joinRoomPanelCancelBtn.onClick.AddListener(() =>
        {
            joinRoomPanel.SetActive(false);
        });

        joinRoomConfirmBtn.onClick.AddListener(() =>
        {
            string code = roomCodeInputComp.text;
            if (string.IsNullOrEmpty(code))
            {
                UpdateRoomCodeError();
                return;
            }

            JoinByCodeFlow(code, playerName);
        });

        searchInputComp.onValueChanged.AddListener((value) =>
        {
            ShowAvailableLobbies(value);
        });

        backBtn.onClick.AddListener(() =>
        {
            SceneManager.LoadScene("MainMenu");
        });

        settingsBtn.onClick.AddListener(() =>
        {
            BoostrapManager.instance.ShowSettings();
        });

        playerName = PlayerPrefs.GetString("PlayerName", "Defalut");
    }
    private void OnDestroy()
    {
        LobbyManager.Instance.onLobbyJoined -= LobbyManager_onLobbyJoined;
        LobbyManager.Instance.onLobbiesRefreshed -= LobbyManager_onLobbiesRefreshed;
    }

    private void LobbyManager_onLobbiesRefreshed(List<Lobby> lobbies)
    {

        listedLobbies.Clear();
        listedLobbies = lobbies;
        ShowAvailableLobbies("");
    }


    private void LobbyManager_onLobbyJoined()
    {
        BoostrapManager.instance.ShowLoading();
        SceneManager.LoadScene("Lobby");
        BoostrapManager.instance.HideLoading();
    }

    private void ShowAvailableLobbies(string lobbyName)
    {
        foreach(Transform child in lobbiesContainer)
        {
            Destroy(child.gameObject);
        }

        listedLobbies = listedLobbies.Where(lobby => lobby.Name.Contains(lobbyName, System.StringComparison.OrdinalIgnoreCase)).ToList();

        foreach(Lobby lobby in listedLobbies)
        {
            GameObject lobbyItem = Instantiate(lobbyItemPrefab, lobbiesContainer);

            lobbyItem.transform.Find("Name").GetComponent<TextMeshProUGUI>().text = lobby.Name;
            lobbyItem.transform.Find("Players").GetComponent<TextMeshProUGUI>().text = $"{lobby.Players.Count}/{lobby.MaxPlayers}";

            Image icon = lobbyItem.transform.GetComponentInChildren<Image>();

            if(icon.sprite != null) 
                icon.sprite = roomAvatars[Random.Range(0,roomAvatars.Length)];

            lobbyItem.GetComponentInChildren<Button>().onClick.AddListener(() =>
            {
                
                if (lobby.IsPrivate)
                {
                    OpenRoomJoinPanel();
                }
                else
                {
                    LobbyManager.Instance.QuickJoinLobby(playerName);
                }

            });
        }
    }

    private void OpenRoomJoinPanel()
    {
        joinRoomPanel.SetActive(true);
    }

    private void HideError()
    {
        roomCodeErrorTxtComp.gameObject.SetActive(false);
    }

    private async void JoinByCodeFlow(string code, string playerName)
    {
        bool isValidCode = await LobbyManager.Instance.JoinLobbyByCode(code, playerName);

        if (!isValidCode)
        {
            UpdateRoomCodeError();
        }
    }

    private void UpdateRoomCodeError()
    {
        roomCodeErrorTxtComp.text = "Code is Invalid";
        roomCodeErrorTxtComp.gameObject.SetActive(true);
        Invoke("HideError", 5f);
    }

}
