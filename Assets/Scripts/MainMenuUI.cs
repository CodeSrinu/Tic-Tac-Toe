using TMPro;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{
    [Header("Main Menu Btns")]
    [SerializeField] private Button playOnlineBtn;
    [SerializeField] private Button createRoomBtn;
    [SerializeField] private Button joinRoomBtn;
    [SerializeField] private Button quickMatch;
    [SerializeField] private Button settingsBtn;
    [SerializeField] private TMP_InputField playerNameInputComp;
    [SerializeField] private TextMeshProUGUI nameErrorTxtComp;


    [Header("Create Room Panel")]
    [SerializeField] private GameObject createRoomPanel;
    [SerializeField] private TMP_InputField lobbyNameTxtComp;
    [SerializeField] private Toggle isPrivateToggle;
    [SerializeField] private Button createRoomConfirmBtn;
    [SerializeField] private Button createRoomPanelCancelBtn;

    [Header("Join Room Panel")]
    [SerializeField] private GameObject joinRoomPanel;
    [SerializeField] private TMP_InputField roomCodeInputComp;
    [SerializeField] private TextMeshProUGUI roomCodeErrorTxtComp;
    [SerializeField] private Button joinRoomConfirmBtn;
    [SerializeField] private Button joinRoomPanelCancelBtn;


    private void Awake()
    {
        playerNameInputComp.text = PlayerPrefs.GetString("PlayerName", "");
        playerNameInputComp.onValueChanged.AddListener((value) =>
        {
            PlayerPrefs.SetString("PlayerName", value.Trim());
            PlayerPrefs.Save();
            UpdateInputError(playerNameInputComp, nameErrorTxtComp);
        });

        playOnlineBtn.onClick.AddListener(() =>
        {
            if (string.IsNullOrEmpty(playerNameInputComp.text))
            {
                UpdateInputError(playerNameInputComp, nameErrorTxtComp);
                return;
            }
            SceneManager.LoadScene("Lobbies");
        });

        createRoomBtn.onClick.AddListener(() =>
        {
            if (string.IsNullOrEmpty(playerNameInputComp.text))
            {
                UpdateInputError(playerNameInputComp, nameErrorTxtComp);
                return;
            }
            createRoomPanel.SetActive(true);
        });

        createRoomPanelCancelBtn.onClick.AddListener(() =>
        {
            createRoomPanel.SetActive(false);
        });
        
        joinRoomBtn.onClick.AddListener(() =>
        {
            if (string.IsNullOrEmpty(playerNameInputComp.text))
            {
                UpdateInputError(playerNameInputComp, nameErrorTxtComp);
                return;
            }
            joinRoomPanel.SetActive(true);
        });

        joinRoomPanelCancelBtn.onClick.AddListener(() => {
            joinRoomPanel.SetActive(false);
        });

        quickMatch.onClick.AddListener(() =>
        {
            if (string.IsNullOrEmpty(playerNameInputComp.text))
            {
                UpdateInputError(playerNameInputComp, nameErrorTxtComp);
                return;
            }
            string name = playerNameInputComp.text.Trim();
            LobbyManager.Instance.QuickJoinLobby(name);
        });

        settingsBtn.onClick.AddListener(() =>
        {
            BoostrapManager.instance.ShowSettings();
        });


        createRoomConfirmBtn.onClick.AddListener(() =>
        {
            CreateRoomFlow();
        });

        joinRoomConfirmBtn.onClick.AddListener(() =>
        {
            string playerName = playerNameInputComp.text;
            string code = roomCodeInputComp.text;
            if (string.IsNullOrEmpty(playerName))
            {
                UpdateInputError(playerNameInputComp, nameErrorTxtComp);
                return;
            }
            else if (string.IsNullOrEmpty(code))
            {
                UpdateInputError(roomCodeInputComp, roomCodeErrorTxtComp);
                return;
            }

            JoinByCodeFlow(code, playerName);
        });
    }

    private void Start()
    {
        LobbyManager.Instance.onLobbyCreated += LobbyManager_onLobbyCreated;
        LobbyManager.Instance.onLobbyFailed += LobbyManager_onLobbyFailed;
        LobbyManager.Instance.onLobbyJoined += LobbyManager_onLobbyJoined;
    }


    private void OnDestroy()
    {
        LobbyManager.Instance.onLobbyCreated -= LobbyManager_onLobbyCreated;
        LobbyManager.Instance.onLobbyFailed -= LobbyManager_onLobbyFailed;
        LobbyManager.Instance.onLobbyJoined -= LobbyManager_onLobbyJoined;

    }
    private void LobbyManager_onLobbyJoined()
    {
        SceneManager.LoadScene("Lobby");
    }

    private void LobbyManager_onLobbyFailed()
    {
        Debug.Log("Lobby creation is failed");
    }

    private void LobbyManager_onLobbyCreated()
    {
        SceneManager.LoadScene("Lobby");
    }

    public async void CreateRoomFlow()
    {
        string lobbyName = lobbyNameTxtComp.text;
        string hostName = playerNameInputComp.text.Trim();
        if(string.IsNullOrEmpty(lobbyName) || string.IsNullOrEmpty(hostName))
        {
            Debug.Log("Enter all of the details to continue");
            return;
        }

        await LobbyManager.Instance.CreateLobby(lobbyName, hostName, isPrivateToggle.isOn);
    }

    private async void JoinByCodeFlow(string code, string playerName)
    {
        bool isValidCode = await LobbyManager.Instance.JoinLobbyByCode(code, playerName);

        if (!isValidCode)
        {
            UpdateInputError(roomCodeInputComp, roomCodeErrorTxtComp);
        }
    }


    private void UpdateInputError(TMP_InputField txtComp, TextMeshProUGUI errorTxtComp)
    {
        string error = "";
        if (string.IsNullOrEmpty(txtComp.text))
        {
            error = "Name Shouldn't be Empty";
        }
        else if (txtComp.text.Length <= 2)
        {
            error = "Name should be more than 2 characters";
        }

        errorTxtComp.text = error;
        errorTxtComp.gameObject.SetActive(true);

        Invoke("HideAllInputErrors", 10f);
    }

    private void HideAllInputErrors()
    {
        nameErrorTxtComp.gameObject.SetActive(false);
        roomCodeErrorTxtComp.gameObject.SetActive(false);
    }
}
