using System.Threading.Tasks;
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

    private bool isRoomPrivate = false;

    private void Awake()
    {
        playerNameInputComp.onValueChanged.AddListener((value) =>
        {
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
            //will write after lobby pipline completed
        });

        isPrivateToggle.onValueChanged.AddListener((value) =>
        {
            isRoomPrivate = value;
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

            LobbyManager.Instance.JoinLobbyByCode(code, playerName);
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

        await LobbyManager.Instance.CreateLobby(lobbyName, hostName, isRoomPrivate);
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
