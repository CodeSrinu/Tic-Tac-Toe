using DG.Tweening;
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
    [SerializeField] private Button exitBtn;
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


    [SerializeField] private GameObject confirmExitPanel;
    [SerializeField] private GameObject errorPanel;
    [SerializeField] private Button errorPanelOkBtn;

    [SerializeField] private GameObject[] doodles;

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
            PanelAnimator.Show(createRoomPanel);
        });

        createRoomPanelCancelBtn.onClick.AddListener(() =>
        {
            PanelAnimator.Hide(createRoomPanel);
        });
        
        joinRoomBtn.onClick.AddListener(() =>
        {
            if (string.IsNullOrEmpty(playerNameInputComp.text))
            {
                UpdateInputError(playerNameInputComp, nameErrorTxtComp);
                return;
            }
            PanelAnimator.Show(joinRoomPanel);
        });

        joinRoomPanelCancelBtn.onClick.AddListener(() => {
            PanelAnimator.Hide(joinRoomPanel);
        });

        quickMatch.onClick.AddListener(() =>
        {
            if (string.IsNullOrEmpty(playerNameInputComp.text))
            {
                UpdateInputError(playerNameInputComp, nameErrorTxtComp);
                return;
            }
            string name = playerNameInputComp.text.Trim();
            BoostrapManager.Instance.ShowLoading();
            _ = QuickJoinLobbyFlow();
        });

        settingsBtn.onClick.AddListener(() =>
        {
            BoostrapManager.Instance.ShowSettings();
        });


        createRoomConfirmBtn.onClick.AddListener(() =>
        {
            BoostrapManager.Instance.ShowLoading();
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

            BoostrapManager.Instance.ShowLoading();
            JoinByCodeFlow(code, playerName);
        });

        exitBtn.onClick.AddListener(() =>
        {
            PanelAnimator.Show(confirmExitPanel);
        });

        errorPanelOkBtn.onClick.AddListener(() =>
        {
            PanelAnimator.Hide(errorPanel);
        });

        nameErrorTxtComp.GetComponent<CanvasGroup>().alpha = 0f;
        roomCodeErrorTxtComp.GetComponent<CanvasGroup>().alpha = 0f;
    }

    private void Start()
    {
        LobbyManager.Instance.onLobbyCreated += LobbyManager_onLobbyCreated;
        LobbyManager.Instance.onLobbyFailed += LobbyManager_onLobbyFailed;
        LobbyManager.Instance.onLobbyJoined += LobbyManager_onLobbyJoined;

        SketchAnimator.DrawInSequence(doodles, 0.3f);
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
        BoostrapManager.Instance.HideLoading();
    }

    private void LobbyManager_onLobbyFailed()
    {
        BoostrapManager.Instance.HideLoading();
        PanelAnimator.Show(errorPanel,"Lobby creation is failed");
    }

    private void LobbyManager_onLobbyCreated()
    {
        
        SceneManager.LoadScene("Lobby");
        BoostrapManager.Instance.HideLoading();
    }

    public async void CreateRoomFlow()
    {
        string lobbyName = lobbyNameTxtComp.text;
        string hostName = playerNameInputComp.text.Trim();
        if(string.IsNullOrEmpty(lobbyName) || string.IsNullOrEmpty(hostName))
        {
            return;
        }

        await LobbyManager.Instance.CreateLobby(lobbyName, hostName, isPrivateToggle.isOn);
    }

    private async void JoinByCodeFlow(string code, string playerName)
    {
        bool isValidCode = await LobbyManager.Instance.JoinLobbyByCode(code, playerName);

        if (!isValidCode)
        {
            BoostrapManager.Instance.HideLoading();
            UpdateInputError(roomCodeInputComp, roomCodeErrorTxtComp);
        }
    }


    private void UpdateInputError(TMP_InputField txtComp, TextMeshProUGUI errorTxtComp)
    {
        errorTxtComp.GetComponent<CanvasGroup>().alpha = 0f;

        if (errorTxtComp == roomCodeErrorTxtComp)
        {
            FadeText(errorTxtComp, 1);
            Invoke("HideAllInputErrors", 10f);
            return;
        }

        string error = "";
        if (string.IsNullOrEmpty(txtComp.text))
        {
            error = "name Shouldn't be Empty";
        }
        else if (txtComp.text.Length <= 2)
        {
            error = "name should be more \nthan two characters";
        }

        errorTxtComp.text = error;
        FadeText(errorTxtComp, 1);

        Invoke("HideAllInputErrors", 10f);
    }

    private void HideAllInputErrors()
    {
        if(nameErrorTxtComp.GetComponent<CanvasGroup>().alpha == 1)
            FadeText(nameErrorTxtComp, 0);

        if (roomCodeErrorTxtComp.GetComponent<CanvasGroup>().alpha == 1)
            FadeText(roomCodeErrorTxtComp, 0);

    }

    private void FadeText(TextMeshProUGUI txt, float targetAlpha, float duration = 0.2f)
    {
        txt.GetComponent<CanvasGroup>().DOFade(targetAlpha, duration);
    }

    private async Task QuickJoinLobbyFlow()
    {
        bool isJoined = await LobbyManager.Instance.QuickJoinLobby(name);

        if (!isJoined)
        {
            BoostrapManager.Instance.HideLoading();
            string error = "No players are playing right now, create room and play with your friends";
            PanelAnimator.Show(errorPanel, error);
        }
    }
}
