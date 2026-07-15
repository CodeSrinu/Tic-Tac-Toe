using DG.Tweening;
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


    [SerializeField] private TMP_InputField roomCodeInputComp;
    [SerializeField] private TextMeshProUGUI roomCodeErrorTxtComp;
    [SerializeField] private TextMeshProUGUI noLobbiesTxtComp;
    [SerializeField] private List<Lobby> listedLobbies = new List<Lobby>();
    private string playerName;

    [SerializeField] private Button backBtn;
    [SerializeField] private Button settingsBtn;

    [SerializeField] private GameObject errorPanel;

    private void Start()
    {
        LobbyManager.Instance.onLobbyJoined += LobbyManager_onLobbyJoined;
        LobbyManager.Instance.onLobbiesRefreshed += LobbyManager_onLobbiesRefreshed;
        LobbyManager.Instance.onLobbyFailed += LobbyManager_onLobbyFailed;

        LobbyManager.Instance.StartPollingLobbiesList();


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
            BoostrapManager.Instance.ShowSettings();
        });

        playerName = PlayerPrefs.GetString("PlayerName", "Defalut");
    }
    private void OnDestroy()
    {
        LobbyManager.Instance.onLobbyJoined -= LobbyManager_onLobbyJoined;
        LobbyManager.Instance.onLobbiesRefreshed -= LobbyManager_onLobbiesRefreshed;
    }

    private void LobbyManager_onLobbyFailed()
    {
        string error = "Lobby Joining Failed, Try Again";
        PanelAnimator.Show(errorPanel, error);
    }

    private void LobbyManager_onLobbiesRefreshed(List<Lobby> lobbies)
    {

        listedLobbies.Clear();
        listedLobbies = lobbies;
        ShowAvailableLobbies("");
    }


    private void LobbyManager_onLobbyJoined()
    {
        SceneManager.LoadScene("Lobby");
    }

    private void ShowAvailableLobbies(string lobbyName)
    {
        foreach(Transform child in lobbiesContainer)
        {
            Destroy(child.gameObject);
        }

        listedLobbies = listedLobbies.Where(lobby => lobby.Name.Contains(lobbyName, System.StringComparison.OrdinalIgnoreCase)).ToList();

        if(listedLobbies.Count <= 0)
        {
            noLobbiesTxtComp.gameObject.GetComponent<CanvasGroup>().DOFade(1, 0.5f);
        }
        else
        {
            noLobbiesTxtComp.gameObject.GetComponent<CanvasGroup>().DOFade(0, 0.5f);
        }


        foreach (Lobby lobby in listedLobbies)
            {
                GameObject lobbyItem = Instantiate(lobbyItemPrefab, lobbiesContainer);
                lobbyItem.GetComponent<CanvasGroup>().alpha = 0;

                lobbyItem.transform.Find("Name").GetComponent<TextMeshProUGUI>().text = lobby.Name;
                lobbyItem.transform.Find("Players").GetComponent<TextMeshProUGUI>().text = $"{lobby.Players.Count}/{lobby.MaxPlayers}";

                Image icon = lobbyItem.transform.GetComponentInChildren<Image>();

                if (icon.sprite != null)
                    icon.sprite = roomAvatars[Random.Range(0, roomAvatars.Length)];

                lobbyItem.GetComponentInChildren<Button>().onClick.AddListener(() =>
                {
                    BoostrapManager.Instance.ShowLoading();
                    LobbyManager.Instance.JoinLobyById(lobby.Id, playerName);
                });

                lobbyItem.GetComponent<CanvasGroup>().DOFade(1, 0.2f);
            }
    }

}
