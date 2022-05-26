using TMPro;
using Google;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Firebase;
using Firebase.Analytics;
using Firebase.Auth;
using Firebase.Extensions;
using System.Text.RegularExpressions;

public enum InputType { Email, Pass, Name }

[System.Serializable()]
public class InputTexts
{
    [SerializeField] public InputType inputType;
    [SerializeField] public TMP_InputField inputTmp = null;
    [SerializeField] public GameObject warningImage = null;
    [SerializeField] public Text warningText = null;
}

public class SignInController : MonoBehaviour
{
    #region Variables

    [Header("References")]
    [SerializeField] GameEvents events = null;

    [Space]
    [Header("Panels")]
    [SerializeField] GameObject signInCanvas = null;
    [SerializeField] GameObject kayitCanvas = null;
    [SerializeField] GameObject startLoadingPanel = null;
    [SerializeField] GameObject signInLoadingPanel = null;

    [Space]
    [Header("Inputs")]
    [SerializeField] List<InputTexts> inputTextsSign = new List<InputTexts>();
    [SerializeField] List<InputTexts> inputTextsLogIn = new List<InputTexts>();

    [Space]
    [Header("Others")]
    [SerializeField] Text infoText = null;
    [SerializeField] string webClientId = "882134224419-vfthqbimn2vsvpnqfu2rvrns4of959vr.apps.googleusercontent.com";

    public static SignInController Instance = null;

    private GoogleSignInConfiguration configuration;

    #endregion

    #region OnStart

    private void Awake()
    {
        Debug.Log("SignInController Awake");
        if (Instance == null) Instance = this;        

        int firstTime = PlayerPrefs.GetInt(GameRecords.SaveFirtTime);
        if (firstTime == 0)
        {
            Debug.Log("Welcome");
            Data.GorulenSoruKayitlariOlustur();
            PlayerPrefs.SetInt(GameRecords.SaveGameMusic, 1);
            PlayerPrefs.SetInt(GameRecords.SaveSoundEffects, 1);
            PlayerPrefs.SetInt(GameRecords.SaveVibration, 0);
            PlayerPrefs.SetInt(GameRecords.SaveOfflineUser, 0);
            PlayerPrefs.SetInt(GameRecords.SaveFirtTime, 1);

            switch (Application.systemLanguage)
            {
                case SystemLanguage.Turkish:
                    PlayerPrefs.SetString(GameRecords.SaveDefaultLanguage, "tr");
                    break;
                case SystemLanguage.English:
                    PlayerPrefs.SetString(GameRecords.SaveDefaultLanguage, "en");
                    break;
                case SystemLanguage.German:
                    PlayerPrefs.SetString(GameRecords.SaveDefaultLanguage, "de");
                    break;
                case SystemLanguage.Spanish:
                    PlayerPrefs.SetString(GameRecords.SaveDefaultLanguage, "sp");
                    break;
                case SystemLanguage.French:
                    PlayerPrefs.SetString(GameRecords.SaveDefaultLanguage, "fr");
                    break;
                default:
                    PlayerPrefs.SetString(GameRecords.SaveDefaultLanguage, "en");
                    break;
            }
        }

        configuration = new GoogleSignInConfiguration
        { WebClientId = webClientId, RequestEmail = true, RequestIdToken = true };
    }

    private void Start()
    {
        startLoadingPanel.SetActive(true);
        signInLoadingPanel.SetActive(false);
        signInCanvas.SetActive(true);
        kayitCanvas.SetActive(false);

        inputTextsSign[0].inputTmp.text = PlayerPrefs.GetString(GameRecords.SaveLastUserMail);
        inputTextsSign[1].inputTmp.text = "123456";

        bool offlineUser = (PlayerPrefs.GetInt(GameRecords.SaveOfflineUser) == 0) ? false : true;

        if (!offlineUser)
        {
            Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
            {
                FirebaseAnalytics.SetAnalyticsCollectionEnabled(true);
                var dependencyStatus = task.Result;
                if (dependencyStatus == Firebase.DependencyStatus.Available)
                {
                    events.app = Firebase.FirebaseApp.DefaultInstance;

                    Debug.Log(System.String.Format("Firebase is ready to use"));
                }
                else
                {
                    UnityEngine.Debug.LogError(System.String.Format(
                      "Could not resolve all Firebase dependencies: {0}", dependencyStatus));
                }
                events.auth = Firebase.Auth.FirebaseAuth.DefaultInstance;
                events.auth.StateChanged += AuthStateChanged;
                AuthStateChanged(this, null);
            });
        }
        else
        {
            Debug.Log("offline user");
            events.playModeisONLINE = false;
            SceneManager.LoadScene((SceneManager.GetActiveScene().buildIndex) + 1);
        }
    }

    #endregion

    #region Email Sign Methods

    public void SignInWithEMail()
    {
        signInLoadingPanel.SetActive(true);
        if (CheckInputs(inputTextsSign))
        {
            string eMail = "";
            string pass = "";
            foreach (InputTexts inputText in inputTextsSign)
            {
                if (inputText.inputType == InputType.Email) eMail = inputText.inputTmp.text;
                if (inputText.inputType == InputType.Pass) pass = inputText.inputTmp.text;
            }
            events.auth.SignInWithEmailAndPasswordAsync(eMail, pass).ContinueWithOnMainThread(task =>
                {
                    if (task.IsCanceled)
                    {
                        Debug.LogError("SignInWithEmailAndPasswordAsync was canceled.");
                        signInLoadingPanel.SetActive(false);
                        return;
                    }
                    if (task.IsFaulted)
                    {
                        inputTextsSign[inputTextsSign.Count - 1].warningText.text = GetErrorText(task.Exception);
                        Debug.LogError("SignInWithEmailAndPasswordAsync encountered an error: " + task.Exception);
                        signInLoadingPanel.SetActive(false);
                        return;
                    }
                    Firebase.Auth.FirebaseUser newUser = task.Result;
                    Debug.LogFormat("User signed in successfully: {0} ({1})",
                        newUser.DisplayName, newUser.UserId);
                });
        }
        else
        {
            signInLoadingPanel.SetActive(false);
        }
    }

    public void LogInWithEMail()
    {
        signInLoadingPanel.SetActive(true);
        if (CheckInputs(inputTextsLogIn))
        {
            string eMail = "";
            string name = "";
            string pass = "";
            foreach (InputTexts inputText in inputTextsLogIn)
            {
                if (inputText.inputType == InputType.Email) eMail = inputText.inputTmp.text;
                if (inputText.inputType == InputType.Name) name = inputText.inputTmp.text;
                if (inputText.inputType == InputType.Pass) pass = inputText.inputTmp.text;
            }
            events.auth.CreateUserWithEmailAndPasswordAsync(eMail, pass).ContinueWithOnMainThread(task =>
                            {
                                if (task.IsCanceled)
                                {
                                    Debug.LogError("CreateUserWithEmailAndPasswordAsync was canceled.");
                                    signInLoadingPanel.SetActive(false);
                                    return;
                                }
                                if (task.IsFaulted)
                                {
                                    inputTextsLogIn[inputTextsLogIn.Count - 1].warningText.text = GetErrorText(task.Exception);
                                    Debug.LogError("CreateUserWithEmailAndPasswordAsync encountered an error: " + task.Exception);
                                    signInLoadingPanel.SetActive(false);
                                    return;
                                }
                                events.userData.name = name;
                                signInLoadingPanel.SetActive(false);
                                Firebase.Auth.FirebaseUser newUser = task.Result;
                                Debug.LogFormat("Firebase user created successfully: {0} ({1})",
                                    newUser.DisplayName, newUser.UserId);
                            });
        }
        else
        {
            signInLoadingPanel.SetActive(false);
        }
    }

    #endregion

    #region Google Sign Methods

    public void SignInWithGoogle()
    {
        GoogleSignIn.Configuration = configuration;
        GoogleSignIn.Configuration.UseGameSignIn = false;
        GoogleSignIn.Configuration.RequestIdToken = true;
        AddToInformation("Calling SignIn");

        GoogleSignIn.DefaultInstance.SignIn().ContinueWith(OnAuthenticationFinished);
    }

    internal void OnAuthenticationFinished(Task<GoogleSignInUser> task)
    {
        if (task.IsFaulted)
        {
            using (IEnumerator<Exception> enumerator = task.Exception.InnerExceptions.GetEnumerator())
            {
                if (enumerator.MoveNext())
                {
                    GoogleSignIn.SignInException error = (GoogleSignIn.SignInException)enumerator.Current;
                    AddToInformation("Got Error: " + error.Status + " " + error.Message);
                }
                else
                {
                    AddToInformation("Got Unexpected Exception?!?" + task.Exception);
                }
            }
        }
        else if (task.IsCanceled)
        {
            AddToInformation("Canceled");
        }
        else
        {
            AddToInformation("Welcome: " + task.Result.DisplayName + "!");
            AddToInformation("Email = " + task.Result.Email);
            AddToInformation("Google ID Token = " + task.Result.IdToken);
            AddToInformation("Email = " + task.Result.Email);
            SignInWithGoogleOnFirebase(task.Result.IdToken);
        }
    }

    private void SignInWithGoogleOnFirebase(string idToken)
    {
        Credential credential = GoogleAuthProvider.GetCredential(idToken, null);

        events.auth.SignInWithCredentialAsync(credential).ContinueWith(task =>
        {
            AggregateException ex = task.Exception;
            if (ex != null)
            {
                if (ex.InnerExceptions[0] is FirebaseException inner && (inner.ErrorCode != 0))
                    AddToInformation("\nError code = " + inner.ErrorCode + " Message = " + inner.Message);
            }
            else
            {
                AddToInformation("Sign In Successful.");
            }
        });
    }

    private void OnSignOut()
    {
        AddToInformation("Calling SignOut");
        GoogleSignIn.DefaultInstance.SignOut();
    }

    public void OnDisconnect()
    {
        AddToInformation("Calling Disconnect");
        GoogleSignIn.DefaultInstance.Disconnect();
    }

    private void AddToInformation(string str) { infoText.text += "\n" + str; }

    #endregion

    #region Controls

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

    private bool CheckInputs(List<InputTexts> inputs)
    {
        foreach (InputTexts inputText in inputs)
        {
            inputText.warningImage.SetActive(false);
            inputText.warningText.text = "";
        }
        bool state = true;
        string firstPas = "";
        foreach (InputTexts inputText in inputs)
        {
            if (inputText.inputTmp.text == null || inputText.inputTmp.text == "")
            {
                inputText.warningImage.SetActive(true);
                inputText.warningText.text = "Boş alan";
                state = false;
                //break;
            }
            else if (inputText.inputType == InputType.Email)
            {
                if (!isEmailRegex(inputText.inputTmp.text))
                {
                    inputText.warningImage.SetActive(true);
                    inputText.warningText.text = "Geçersiz email adresi";
                    state = false;
                    //break;
                }
            }
            else if (inputText.inputType == InputType.Name)
            {
                if (!isNameAlphaNumeric(inputText.inputTmp.text))
                {
                    inputText.warningImage.SetActive(true);
                    inputText.warningText.text = "Geçersiz kullanıcı adı, 4 ile 20 karakterden oluşmalı.";
                    state = false;
                    //break;
                }
            }
            else if (inputText.inputType == InputType.Pass)
            {
                if (!isPasswordRegex(inputText.inputTmp.text))
                {
                    inputText.warningImage.SetActive(true);
                    inputText.warningText.text = "Geçersiz şifre, 4 ile 20 karakterden oluşmalı.";
                    state = false;
                    //break;
                }
                else if (firstPas == "")
                {
                    firstPas = inputText.inputTmp.text;
                }
                else
                {
                    if (inputText.inputTmp.text != firstPas)
                    {
                        inputText.warningImage.SetActive(true);
                        inputText.warningText.text = "Şifreler uyuşmuyor";
                        state = false;
                    }
                }
            }
        }
        return state;
    }

    private string GetErrorText(System.AggregateException exception)
    {
        int errorCode = (-1);
        string errorText = "";
        Firebase.FirebaseException fbEx = null;
        foreach (System.Exception e in exception.Flatten().InnerExceptions)
        {
            fbEx = e as Firebase.FirebaseException;
            if (fbEx != null)
            {
                errorCode = fbEx.ErrorCode;
                break;
            }
        }
        Debug.Log("Ecode: " + errorCode);
        switch (errorCode)
        {
            case -1:
                errorText = "Bilinmeyen bir hata oluştu";
                break;
            case 8:
                errorText = "Email adresi zaten kullanılıyor";
                break;
            case 11:
                errorText = "Geçersiz Email";
                break;
            case 12:
                errorText = "Şifre yanlış";
                break;
            case 14:
                errorText = "Kullanıcı bulunamadı";
                break;
            case 19:
                errorText = "Network Request Failed";
                break;
            case 23:
                errorText = "Zayıf şifre";
                break;
            default:
                errorText = "Bilinmeyen bir hata oluştu";
                break;
        }
        return errorText;
    }

    #endregion

    public void AuthStateChanged(object sender, System.EventArgs eventArgs)
    {
        // SignInController destroy haldeyken firebasecontroller tarafindan cagriliyor

        FirebaseAuth auth = events.auth;
        FirebaseUser user = events.user;

        if (auth.CurrentUser != user)
        {
            bool signedIn = user != auth.CurrentUser && auth.CurrentUser != null;
            if (!signedIn && user != null)
            {
                Debug.Log("Signed out " + events.user.UserId);
            }
            events.user = events.auth.CurrentUser;
            if (signedIn)
            {
                Debug.Log("Signed in " + events.user.UserId);
                events.playModeisONLINE = true;
                SceneManager.LoadScene((SceneManager.GetActiveScene().buildIndex) + 1);
            }
        }
        if (events.auth.CurrentUser == null)
        {
            Debug.Log("Current user is null");
            if (SignInController.Instance != null)
            {
                Debug.Log("signcont is not null");
                startLoadingPanel.SetActive(false);
                signInLoadingPanel.SetActive(false);
            }
            else
            {
                Debug.Log("signcont is null");
            }
        }
    }

    public void OpenLogInPanel()
    {
        kayitCanvas.SetActive(true);
    }

    public void SignInWithOffLine()
    {
        events.playModeisONLINE = false;
        PlayerPrefs.SetInt(GameRecords.SaveOfflineUser, 1);
        SceneManager.LoadScene((SceneManager.GetActiveScene().buildIndex) + 1);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (kayitCanvas.activeSelf) kayitCanvas.SetActive(false);
        }
    }

    private void OnDisable()
    {
        if (events.auth != null)
            events.auth.StateChanged -= AuthStateChanged;
    }
}
