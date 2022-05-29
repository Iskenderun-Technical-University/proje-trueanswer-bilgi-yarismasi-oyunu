using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System.Text.RegularExpressions;

[System.Serializable()]
public class LeaderBoardUI
{
    [SerializeField] public GameObject placeText = null;
    [SerializeField] public GameObject userName = null;
    [SerializeField] public GameObject userScore = null;
    [SerializeField] public Image placeColor = null;
}

[System.Serializable()]
public class ProfilUI
{
    [SerializeField] public GameObject warningIcon = null;
    [SerializeField] public Text warningText = null;
}

public class MenuManager : MonoBehaviour
{
    #region Variables

    [Header("References")]
    [SerializeField] private GameEvents events = null;

    [Space]
    [Header("Canvases")]
    [SerializeField] private GameObject mainCanvas = null;
    [SerializeField] private GameObject shopCanvas = null;
    [SerializeField] private GameObject leaderBoardCanvas = null;
    [SerializeField] private GameObject settingsCanvas = null;
    [SerializeField] private GameObject profileCanvas = null;
    [SerializeField] private GameObject warningMsgCanvas = null;
    [SerializeField] private GameObject blockCanvas = null;

    [Space]
    [Header("Settings")]
    [SerializeField] private Button profButton = null;
    [SerializeField] private Text userName = null;
    [SerializeField] private Toggle soundEffects = null;
    [SerializeField] private Toggle gameMusic = null;
    [SerializeField] private Toggle vibrations = null;

    [Space]
    [Header("Profile")]
    [SerializeField] private TMP_InputField nameInput = null;
    [SerializeField] private TMP_InputField emailInput = null;
    [SerializeField] private TMP_InputField newPassInput = null;
    [SerializeField] private TMP_InputField confPassInput = null;
    [SerializeField] private ProfilUI[] profilUIs = null;

    [Space]
    [Header("LeaderBoard")]
    [SerializeField] private LeaderBoardUI[] leaderBoardUIs = null;
    [SerializeField] private GameObject lBoardUserContent = null;
    [SerializeField] private Color lBDefColor = Color.white;
    [SerializeField] private Color lBUserColor = Color.white;

    [Space]
    [Header("Shop")]
    [SerializeField] private Toggle premiumToggle = null;
    [SerializeField] private Toggle eJokerToggle = null;

    public static MenuManager Instance = null;

    #endregion

    #region OnStart

    private void Awake()
    {
        Debug.Log("MenuManager Awake");
        if (Instance == null) Instance = this;
    }

    private void Start()
    {
        for (int i = 0; i < events.leaderBoardDatas.Length; i++)
            events.leaderBoardDatas[i] = new LeaderBoardData();

        mainCanvas.SetActive(true);
        shopCanvas.SetActive(false);
        leaderBoardCanvas.SetActive(false);
        settingsCanvas.SetActive(false);
        blockCanvas.SetActive(false);

        SettingsControl();
    }

    private void SettingsControl()
    {
        int getSoundEffects = PlayerPrefs.GetInt(GameRecords.SaveSoundEffects);
        int getGameMusic = PlayerPrefs.GetInt(GameRecords.SaveGameMusic);
        int getVibrations = PlayerPrefs.GetInt(GameRecords.SaveVibration);

        soundEffects.isOn = (getSoundEffects == 0) ? false : true;
        gameMusic.isOn = (getGameMusic == 0) ? false : true;
        vibrations.isOn = (getVibrations == 0) ? false : true;

        if (getGameMusic == 1) AudioManager.Instance.PlayGameMusic();
        AudioManager.Instance.ChangeSFXVolume(soundEffects.isOn);

        profButton.interactable = events.playModeisONLINE;
        if (events.playModeisONLINE)
        {
            Debug.Log("Receiving datas..");
            FirebaseController.Instance.GetUserDatas();
            FirebaseController.Instance.GetLeaderBoardDatas();
        }
        else
        {
            events.userData = new UserData
            {
                premium = false,
                extraJoker = false,
                email = "offline",
                name = "offline",
                highScore = PlayerPrefs.GetInt(GameRecords.SaveOfflineHighScoreKey),
                userId = "offline",
                language = "Turkish"
            };
        }
    }

    #endregion

    #region Menu Navigations

    public void PlayButton()
    {
        SceneManager.LoadSceneAsync((SceneManager.GetActiveScene().buildIndex) + 1);
    }

    public void ShopButton()
    {
        UpdateShopUI();
        mainCanvas.SetActive(false);
        shopCanvas.SetActive(true);
        leaderBoardCanvas.SetActive(false);
        settingsCanvas.SetActive(false);
    }

    public void LeaderBoardButton()
    {
        UpdateLeaderBoardUI();
        mainCanvas.SetActive(false);
        shopCanvas.SetActive(false);
        leaderBoardCanvas.SetActive(true);
        settingsCanvas.SetActive(false);
    }

    public void SettingsButton()
    {
        UpdateSettingsUI();
        mainCanvas.SetActive(false);
        shopCanvas.SetActive(false);
        leaderBoardCanvas.SetActive(false);
        settingsCanvas.SetActive(true);
    }

    public void BackMainMenuButton()
    {
        mainCanvas.SetActive(true);
        shopCanvas.SetActive(false);
        leaderBoardCanvas.SetActive(false);
        settingsCanvas.SetActive(false);
        warningMsgCanvas.SetActive(false);
    }

    public void DisableBlockCanvas(bool display)
    {
        Debug.Log("Block canvas disable");
        blockCanvas.SetActive(display);
    }

    private void DisplayWarnMsgScreen(bool display)
    {
        warningMsgCanvas.SetActive(display);
    }

    public void CancelQuit()
    {
        DisplayWarnMsgScreen(false);
    }

    public void Quit()
    {
        Application.Quit();
    }

    #endregion

    #region Updates UI

    private void UpdateLeaderBoardUI()
    {
        leaderBoardUIs[6].placeText.GetComponent<TextMeshProUGUI>().text = "";
        leaderBoardUIs[6].userName.GetComponent<TextMeshProUGUI>().text = events.userData.name;
        leaderBoardUIs[6].userScore.GetComponent<TextMeshProUGUI>().text = events.userData.highScore.ToString();

        int count = 0;
        bool userInTop6 = false;
        foreach (LeaderBoardData lbd in events.leaderBoardDatas)
        {
            LeaderBoardUI lbUI = new LeaderBoardUI();
            lbUI = leaderBoardUIs[count];
            lbUI.placeText.GetComponent<TextMeshProUGUI>().text = (count + 1).ToString();
            lbUI.userName.GetComponent<TextMeshProUGUI>().text = lbd.name;
            lbUI.userScore.GetComponent<TextMeshProUGUI>().text = lbd.highScore.ToString();

            if (lbd.name == events.userData.name)
            {
                userInTop6 = true;
                lbUI.placeColor.color = lBUserColor;
            }
            else
            {
                lbUI.placeColor.color = lBDefColor;
            }
            count++;
        }
        lBoardUserContent.SetActive(!userInTop6);
    }

    private void UpdateShopUI()
    {
        premiumToggle.isOn = events.userData.premium;
        eJokerToggle.isOn = events.userData.extraJoker;
    }

    private void UpdateSettingsUI()
    {
        userName.text = events.userData.name;
    }

    // online only
    private void UpdateProfileUI()
    {
        nameInput.text = events.auth.CurrentUser.DisplayName;
        emailInput.text = events.auth.CurrentUser.Email;
        newPassInput.text = "";
        confPassInput.text = "";

        foreach (ProfilUI pf in profilUIs)
        {
            pf.warningIcon.SetActive(false);
            pf.warningText.text = "";
        }
    }

    #endregion

    #region Settings Navigations

    public void ProfileButton()
    {
        UpdateProfileUI();
        profileCanvas.SetActive(true);
    }

    public void OnSoundEffectsChanged(bool state)
    {
        int stateValue = (state) ? 1 : 0;
        PlayerPrefs.SetInt(GameRecords.SaveSoundEffects, stateValue);
        events.soundEffect = state;
        AudioManager.Instance.ChangeSFXVolume(state);
    }

    public void OnGameMusicChanged(bool state)
    {
        int stateValue = (state) ? 1 : 0;
        PlayerPrefs.SetInt(GameRecords.SaveGameMusic, stateValue);
        events.gameMusic = state;
        if (state) AudioManager.Instance.PlayGameMusic();
        else AudioManager.Instance.StopGameMusic();
    }

    public void OnVibrationsChanged(bool state)
    {
        int stateValue = (state) ? 1 : 0;
        PlayerPrefs.SetInt(GameRecords.SaveVibration, stateValue);
        events.vibration = state;
    }

    public void LogOutButton()
    {
        if (AudioManager.Instance != null) AudioManager.Instance.DestroyAudioMng();
        FirebaseController.Instance.LogOut();
    }

    #endregion

    #region Profile Navigations

    public void BackToSettings()
    {
        UpdateSettingsUI();
        profileCanvas.SetActive(false);
    }

    public void ProfileButtonSave()
    {
        Debug.Log("ProfileButtonSave");
        if (events.auth.CurrentUser.DisplayName != nameInput.text)
        {
            Debug.Log("ProfileButtonSave name");
            if (isNameAlphaNumeric(nameInput.text))
            {
                FirebaseController.Instance.UpdateUserProfile(nameInput.text.Trim(), "");
            }
            else
            {
                profilUIs[0].warningIcon.SetActive(true);
                profilUIs[0].warningText.text = "Uygun değil. 4 ile 20 karakter.";
            }
        }

        if (events.auth.CurrentUser.Email != emailInput.text)
        {
            Debug.Log("ProfileButtonSave email");
            if (isEmailRegex(emailInput.text))
            {
                FirebaseController.Instance.UpdateUserEmail(emailInput.text.Trim());
            }
            else
            {
                profilUIs[1].warningIcon.SetActive(true);
                profilUIs[1].warningText.text = "Uygun değil";
            }
        }

        if (newPassInput.text != "" && newPassInput.text != null)
        {
            if (confPassInput.text != "" && confPassInput.text != null)
            {
                if (newPassInput.text == confPassInput.text)
                {
                    if (isPasswordRegex(newPassInput.text))
                    {
                        FirebaseController.Instance.UpdateUserPassword(newPassInput.text.Trim());
                    }
                    else
                    {
                        profilUIs[2].warningIcon.SetActive(true);
                        profilUIs[2].warningText.text = "Uygun değil";
                    }
                }
                else
                {
                    profilUIs[2].warningIcon.SetActive(true);
                    profilUIs[2].warningText.text = "Şifreler eşleşmiyor";
                    profilUIs[3].warningIcon.SetActive(true);
                    profilUIs[3].warningText.text = "Şifreler eşleşmiyor";
                }
            }
            else
            {
                profilUIs[3].warningIcon.SetActive(true);
                profilUIs[3].warningText.text = "Şifreyi tekrar girin";
            }
        }
    }

    #endregion

    #region Input Controls

    private bool isNameAlphaNumeric(string strToCheck)
    {
        string patternStrict = @"^([\w\-\s]+).{4,20}$";
        Regex rg = new Regex(patternStrict);
        return rg.IsMatch(strToCheck);
    }

    private bool isEmailRegex(string emailAddress)
    {
        string patternStrict2 = @"^(([^<>()[\]\\.,;:\s@\""]+"
        + @"(\.[^<>()[\]\\.,;:\s@\""]+)*)|(\"".+\""))@"
        + @"((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}"
        + @"\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+"
        + @"[a-zA-Z]{2,}))$";
        Regex rg = new Regex(patternStrict2);
        return rg.IsMatch(emailAddress);
    }

    private bool isPasswordRegex(string password)
    {
        string patternStrict = @"^.{4,18}$";
        Regex rg = new Regex(patternStrict);
        return rg.IsMatch(password);
    }

    #endregion

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (profileCanvas.activeSelf)
            {
                BackToSettings();
            }
            else if (mainCanvas.activeSelf)
                DisplayWarnMsgScreen(true);
            else
                BackMainMenuButton();
        }
    }

    public void EditProfielOnValueChanged()
    {
        /*for (int i = 0; i < 4; i++)
        {
            profilUIs[i].warningIcon.SetActive(false);
            profilUIs[i].warningText.text = "";
        }

        Debug.Log("EditProfielOnValueChanged");
        string name = events.auth.CurrentUser.DisplayName;
        string email = events.auth.CurrentUser.Email;

        bool valuesChange = false;
        if (name != nameInput.text)
        {
            if (!isNameAlphaNumeric(nameInput.text))
            {
                profilUIs[0].warningIcon.SetActive(true);
                profilUIs[0].warningText.text = "Uygun değil. Sadece harf ve rakam, 4 ile 20 karakter.";
            }
            else
            {
                valuesChange = true;
            }
        }
        if (email != emailInput.text)
        {
            if (!isEmailRegex(emailInput.text))
            {
                profilUIs[1].warningIcon.SetActive(true);
                profilUIs[1].warningText.text = "Uygun değil.";
            }
            else
            {
                valuesChange = true;
            }
        }
        if (newPassInput.text != "" && newPassInput.text != null && newPassInput.text == confPassInput.text)
        {
            Debug.Log("Pass is changed" + newPassInput.text);
            valuesChange = true;
        }
        editProfileButton.interactable = valuesChange;*/
    }
}
