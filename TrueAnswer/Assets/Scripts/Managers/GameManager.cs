using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
using System;

public class GameManager : MonoBehaviour
{

    #region Variables

    [Header("References")]
    [SerializeField] GameEvents events = null;

    [Space][Header("UI Design")]
    [SerializeField] GameObject adPannel = null;
    [SerializeField] GameObject mainContent = null;
    [SerializeField] GameObject hContent = null;
    [SerializeField] GameObject qContent = null;
    [SerializeField] GameObject jContent = null;
    [SerializeField] List<GameObject> answerList = new List<GameObject>();

    [Space][Header("HUD Content")]
    [SerializeField] Text curScoreText = null;
    [SerializeField] Text curLevelText = null;
    [SerializeField] Text timerText = null;
    [SerializeField] RectTransform timerBar = null;
    [SerializeField] Text totalHealth = null;

    [Space][Header("QUESTION Content")]
    [SerializeField] Text questionText = null;
    [SerializeField] Color answerNormalColor = Color.white;
    [SerializeField] Color answerSelectedColor = Color.white;
    [SerializeField] Color answerWrongColor = Color.white;
    [SerializeField] Color answerCorrectColor = Color.white;

    [Space][Header("Jokers")]
    [SerializeField] List<GameObject> JokerList = new List<GameObject>();

    [Space][Header("Result Screen")]
    [SerializeField] Text highLevelText = null;
    [SerializeField] Text resultLevelText = null;
    [SerializeField] RectTransform resultScreenRT = null;
    [SerializeField] CanvasGroup mainCanvasGroup = null;
    [SerializeField] GameObject warningMsgScreen = null;

    [Space][Header("Ad Panel")]
    [SerializeField] GameObject mainCanvasBlockImage = null;
    [SerializeField] GameObject reklamPnl = null;
    [SerializeField] GameObject rewardedAdBtn = null;
    [SerializeField] RectTransform reklamBar = null;

    private Data _C_data = new Data();

    private IEnumerator IE_StartTimer = null;
    private IEnumerator IE_ReklamPnlGeriSayim = null;

    private List<int> elenenCevaplar = new List<int>();
    private List<int> questionsList = new List<int>();

    private DateTime bitisSaati;
    private DateTime baslangicSaati;

    private float timePast = 0;
    private int curQuestionInd = -1;
    private int currentHealth = 1;
    private int currentLevel = 1;
    private int corAnswerInd = -1;
    private int selAnswerInd = -1;
    private int soruSuresi = 0;
    private int adHealth = 1;
    private int delayTime = 1;
    private int saniye = 0;

    //private bool bannerIsLoaded = false;
    //private bool IntAdIsReady = false;
    private bool RewAdIsReady = false;

    private bool timeIsRunning = false;
    private bool isGamePaused = false;
    private bool timeIsOver = false;
    private bool ciftCevap = false;
    private bool jokerStopTimer = false;
    private bool IsFinished
    {
        get
        {
            if (currentLevel >= events.maxLevel) return true;
            else return false;
        }
    }

    #endregion

    #region Default Unity Methods

    private void OnEnable()
    {
        events.RewardedAdLoaded += RewardedAdLoaded;
        events.RewardedAdFailedToLoad += RewardedAdFailedToLoad;
        events.RewardedAdOpening += RewardedAdOpening;
        events.RewardedAdFailedToShow += RewardedAdFailedToShow;
        events.UserEarnedReward += UserEarnedReward;
        events.RewardedAdClosed += RewardedAdClosed;
    }

    private void OnDisable()
    {
        events.RewardedAdLoaded -= RewardedAdLoaded;
        events.RewardedAdFailedToLoad -= RewardedAdFailedToLoad;
        events.RewardedAdOpening -= RewardedAdOpening;
        events.RewardedAdFailedToShow -= RewardedAdFailedToShow;
        events.UserEarnedReward -= UserEarnedReward;
        events.RewardedAdClosed -= RewardedAdClosed;
    }

    private void Awake()
    {
        Debug.Log("GameManager Awake");
    }

    private void Start()
    {
        mainCanvasGroup.blocksRaycasts = false;
        var seed = UnityEngine.Random.Range(int.MinValue, int.MaxValue);
        UnityEngine.Random.InitState(seed);
        SettingsControl();
        UpdateHudUI();
        try
        {
            //YeniSoruGetir(true);
        }
        catch
        {
            Debug.LogWarning("Ups! Something went wrong while trying to display new Question data." +
                            " Issue occured in GameManager.Display() method.");
        }
    }

    private void SettingsControl()
    {
        bool extraJkr = events.userData.extraJoker;
        extraJkr = true; // ************* TEST ************** TEST *************** TEST ************** TEST **********
        bool premium = events.userData.premium;
        premium = true; // ************* TEST ************** TEST *************** TEST ************** TEST **********

        currentLevel = 1;
        currentHealth = 3 + ((premium) ? 0 : 0) + ((extraJkr) ? 0 : 0);
        adHealth = ((premium) ? 0 : 6);
        JokerList[4].SetActive(extraJkr);
        JokerList[5].SetActive(extraJkr);


        if(!premium)
        {
            adPannel.SetActive(true);
            mainContent.GetComponent<RectTransform>().offsetMax = new Vector2(0, -200);
            hContent.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 200);
            qContent.GetComponent<RectTransform>().offsetMax = new Vector2(0, -200);
            qContent.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 500);
            jContent.GetComponent<RectTransform>().offsetMax = new Vector2(0, -700);
            jContent.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 200);
            //AdsManager.Instance.SendRequestes();
        }
    }

    #endregion

    #region Get New Question Methods

    Soru GetRandomQuestion()
    {
        var random = 0;
        if (questionsList.Count == 0) ResetQuestionList();
        random = UnityEngine.Random.Range(0, questionsList.Count);
        curQuestionInd = questionsList[random];
        return _C_data.Sorular[curQuestionInd];        
    }

    private void ResetQuestionList()
    {
        for (int i = 0; i < _C_data.Sorular.Length; i++)
        {
            PlayerPrefs.SetInt(currentLevel.ToString() + i.ToString(), 0);
            questionsList.Add(i);
        }
    }

    private void YeniSoruGetir(bool loadNewData)
    {
        if(loadNewData)
        {
            if (!events.userData.premium && currentLevel % 5 == 0)
            {
                Debug.Log("---gecisli   1234");
                //Ads AdsManager.Instance.ShowGec();
            }

            var path = Path.Combine(GameRecords.FileDir, GameRecords.FileName + currentLevel);
            _C_data = Data.Fetch(path);

            questionsList.Clear();
            for (int i = 0; i < _C_data.Sorular.Length; i++)
            {
                int durum = PlayerPrefs.GetInt(currentLevel.ToString() + i.ToString());
                if (durum == 0) questionsList.Add(i);                
            }
            if (questionsList.Count == 0)   ResetQuestionList();            
        }

        IEnumerator Beklet()
        {
            yield return new WaitForSeconds(delayTime);
            Display();
        }
        StartCoroutine(Beklet());
    }
    
    private void Display()
    {
        var question = GetRandomQuestion();
        jokerStopTimer = false;
        ciftCevap = false;
        elenenCevaplar.Clear();
        int dogruCevapIndTut = question.GetDogruCevapIndex();
        string dogruCevapStr = question.Cevaplar[dogruCevapIndTut].Cevap_Text;
        List<string> cevaplarTut = new List<string>();

        for (int i = 0; i < question.Cevaplar.Length; i++)
        {
            cevaplarTut.Add(question.Cevaplar[i].Cevap_Text);
        }
        for (int i = 0; i < 4; i++)
        {
            var random = UnityEngine.Random.Range(0, cevaplarTut.Count);
            string cevapStr = cevaplarTut[random];
            answerList[i].GetComponentInChildren<Text>().text = cevapStr;
            if (cevapStr == dogruCevapStr)
            {
                corAnswerInd = i;
                elenenCevaplar.Add(i);
            }
            cevaplarTut.RemoveAt(random);
            answerList[i].SetActive(true);
            answerList[i].GetComponent<Image>().raycastTarget = true;
            answerList[i].GetComponent<Image>().color = answerNormalColor;
        }
        
        soruSuresi =(currentLevel <= 1) ? 8 : (currentLevel <= 2) ? 12 :
                    (currentLevel <= 3) ? 15 : (currentLevel <= 5) ? 20 :
                    (currentLevel <= 8) ? 25 : (currentLevel <= 13) ? 30 :
                    (currentLevel <= 21) ? 35 : (currentLevel <= 34) ? 40 :
                    (currentLevel <= 55) ? 50 : (currentLevel <= 89) ? 60 : 120;

        baslangicSaati = DateTime.Now;
        bitisSaati = DateTime.Now.AddSeconds(soruSuresi);
        questionText.text = question.Soru_Text;
        PlayerPrefs.SetInt(currentLevel.ToString() + curQuestionInd.ToString(), 1);
        mainCanvasGroup.blocksRaycasts = true;
        ReStartTimer(bitisSaati);
    }

    #endregion

    #region Game Actions
   
    public void Accept(int index)
    {        
        mainCanvasGroup.blocksRaycasts = false;
        StopTimer();

        answerList[index].GetComponent<Image>().color = answerSelectedColor;
        answerList[index].GetComponent<Image>().raycastTarget = false;
        selAnswerInd = index;
        questionsList.Remove(curQuestionInd);

        IEnumerator Beklet()
        {
            yield return new WaitForSeconds(1);
            SonucuGetir();
        }
        StartCoroutine(Beklet());
    }

    private void SonucuGetir()
    {
        bool isCorrect = (selAnswerInd == corAnswerInd) ? true : false;
        
        answerList[selAnswerInd].GetComponent<Image>().color = (isCorrect) ? answerCorrectColor : answerWrongColor;
        AudioManager.Instance.PlaySoundFX(((isCorrect) ? "CorrectSFX" : "InCorrectSFX"));

        if (isCorrect && IsFinished)    GameFinish();
        else if (isCorrect)
        {
            currentLevel++; UpdateHudUI();
            YeniSoruGetir(true);
        }
        else if (ciftCevap)
        {
            ciftCevap = false;
            elenenCevaplar.Add(selAnswerInd);
            mainCanvasGroup.blocksRaycasts = true;
            if(!jokerStopTimer) CountinueTimer();
        }
        else
        {
            answerList[corAnswerInd].GetComponent<Image>().color = answerCorrectColor;
            currentHealth--;    UpdateHudUI();
            if (currentHealth <= 0)
            {
                if (RewAdIsReady)    DisplayReklamPanel(true);
                else                    GameFinish();
            }
            else    YeniSoruGetir(false);
        }
    }

    private void UpdateHudUI()
    {
        curScoreText.text = currentLevel.ToString();
        totalHealth.text = currentHealth.ToString();
        curLevelText.text = currentLevel.ToString();
    }

    public void Joker(int index)
    {
        AudioManager.Instance.PlaySoundFX("JokerUsesSFX");
        JokerList[index].GetComponent<Button>().interactable = false;
        switch (index)
        {
            case 0: YanlisCevapEle(1);  break;
            case 1: YanlisCevapEle(2);  break;
            case 2: ciftCevap = true;   break;

            case 3: 
                mainCanvasGroup.blocksRaycasts = false;
                StopTimer();
                questionsList.Remove(curQuestionInd);
                YeniSoruGetir(false);
                break;

            case 4: 
                jokerStopTimer = true; 
                StopTimer();
                break;

            case 5: 
                Accept(corAnswerInd);
                break;

            default:
                Debug.Log("Joker fonksiyonu uygunsuz çağrıldı!");
                break;
        }
    }

    private void YanlisCevapEle(int count)
    {
        for (int i = 0; i < count; i++)
            if (elenenCevaplar.Count <= 3)
            {
                int random = 0;
                int say = 0;
                do
                {
                    say++;
                    if (say >= 500)
                    {
                        Debug.Log("Hata! YanlisCevapEle() fonksiyonu çalışırken hata oluştu. (500'den fazla değer döndü)");
                        break;
                    }
                    random = UnityEngine.Random.Range(0, 4);
                } while (elenenCevaplar.Contains(random));
                answerList[random].SetActive(false);
                elenenCevaplar.Add(random);
            }
            else break;
    }

    private void GameFinish()
    {
        DisplaySonuc();
    }

    #endregion

    #region Timer Methods

    private void StopTimer()
    {
        if (IE_StartTimer != null) StopCoroutine(IE_StartTimer);
        timeIsRunning = false;
    }

    private void CountinueTimer()
    {
        StartCoroutine(IE_StartTimer);
    }

    private void ReStartTimer(DateTime finishTime)
    {
        if (IE_StartTimer != null) StopCoroutine(IE_StartTimer);
        IE_StartTimer = SureyiBaslat();
        StartCoroutine(IE_StartTimer);
        timePast = 0;
        timeIsRunning = true;
        timeIsOver = false;
    }
    IEnumerator SureyiBaslat()//int totalTime)
    {
        saniye = (bitisSaati - DateTime.Now).Seconds;
        timerText.color = Color.black;
        timerBar.GetComponent<Image>().color = Color.blue;
        while (saniye > 0)
        {
            Debug.Log("scr:" + Screen.currentResolution.width);
            Debug.Log("scw:" + Screen.width);

            saniye = (bitisSaati - DateTime.Now).Seconds;

            if (saniye < 10 && saniye > 5)
            {
                timerText.color = Color.yellow;
                timerBar.GetComponent<Image>().color = Color.yellow;
            }
            else if (saniye <= 5 && saniye > 3)
            {
                AudioManager.Instance.PlaySoundFX("CountDownSFX");
            }
            else if (saniye <= 3)
            {
                AudioManager.Instance.PlaySoundFX("CountDownSFX");
                timerBar.GetComponent<Image>().color = Color.red;
                timerText.color = Color.red;
            }
            timerText.text = saniye.ToString() + "\"";
            yield return new WaitForSeconds(1.0f);
        }

        timeIsRunning = false;
        if(!isGamePaused)
        {
            SureBitti();
        }
        else
        {
            timeIsOver = true;
        }
    }
    
    void Update()
    {
        if (timeIsRunning)
        {
            timePast += Time.deltaTime;
            if (Math.Abs(timePast - (soruSuresi - saniye)) >= 1.5f)
            {
                Debug.Log("toparla");
                timePast = (DateTime.Now - baslangicSaati).Seconds;
            }            
            float xx = timePast / soruSuresi;
            float x2 = (Screen.currentResolution.width * xx);
            timerBar.offsetMax = new Vector2(-x2, timerBar.offsetMax.y);
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            DisplayWarnMsgScreen(true);
        }
    }

    private void SureBitti()
    {
        AudioManager.Instance.PlaySoundFX("InCorrectSFX");
        timerBar.offsetMax = new Vector2(-Screen.currentResolution.width, timerBar.offsetMax.y);
        timerText.color = Color.red;
        timerText.text = "0\"";
        questionsList.Remove(curQuestionInd);
        currentHealth--;
        UpdateHudUI();
        if (currentHealth <= 0)
        {
            if (RewAdIsReady)    DisplayReklamPanel(true);
            else { GameFinish(); }
        }
        else    YeniSoruGetir(false);
    }

    #endregion

    #region Result Screen Methods

    private void DisplayWarnMsgScreen(bool display)
    {
        if(display)
        {
            mainCanvasBlockImage.SetActive(true);
            mainCanvasGroup.blocksRaycasts = false;
            warningMsgScreen.SetActive(true);
            isGamePaused = true;
        }
        else
        {
            mainCanvasBlockImage.SetActive(false);
            mainCanvasGroup.blocksRaycasts = true;
            warningMsgScreen.SetActive(false);
            isGamePaused = false;
            if (timeIsOver) SureBitti();
        }
    }

    private void DisplayReklamPanel(bool goruntule)
    {
        if (IE_ReklamPnlGeriSayim != null)  StopCoroutine(IE_ReklamPnlGeriSayim);
        
        if (goruntule)
        {
            mainCanvasBlockImage.SetActive(true);
            reklamPnl.SetActive(true);
            mainCanvasGroup.blocksRaycasts = false;
            IE_ReklamPnlGeriSayim = ReklamTimerBar();
            StartCoroutine(IE_ReklamPnlGeriSayim);
        }
        else
        {
            mainCanvasBlockImage.SetActive(false);
            reklamPnl.SetActive(false);
            mainCanvasGroup.blocksRaycasts = true;
        }
    }

    IEnumerator ReklamTimerBar()
    {
        int genislik = 500;
        reklamBar.sizeDelta = new Vector2(genislik, 40);
        while (genislik > 0)
        {
            genislik -= 1;
            reklamBar.sizeDelta = new Vector2(genislik, 40);
            yield return new WaitForSeconds(0.015f);
        }
        if (genislik <= 0)
        {
            Debug.Log("reklam panel suresi bitti");
            reklamPnl.SetActive(false);
            DisplaySonuc();
        }
    }

    private void DisplaySonuc()
    {
        mainCanvasGroup.blocksRaycasts = false;
        int hgScore = 0;

        if(events.playModeisONLINE)
        {
            hgScore = events.userData.highScore;
            if (hgScore < currentLevel)
            {
                hgScore = currentLevel;
                events.reference.Child("UserData").Child(
                    events.user.UserId).Child("highScore").SetValueAsync(hgScore);

                events.reference.Child("LeaderBoard").Child(
                events.user.UserId).Child("highScore").SetValueAsync(hgScore);

                //FirebaseController.Instance.UpdateUserData();
            }
        }
        else
        {
            hgScore = PlayerPrefs.GetInt(GameRecords.SaveOfflineHighScoreKey);
            if (hgScore < currentLevel)
            {
                hgScore = currentLevel;
                PlayerPrefs.SetInt(GameRecords.SaveOfflineHighScoreKey, hgScore);
            }
        }
        
        highLevelText.text = hgScore.ToString();
        resultLevelText.text = currentLevel.ToString();
        resultScreenRT.gameObject.SetActive(true);
    }

    #endregion

    #region Ad Methods

    private void RewardedAdLoaded()
    {
        Debug.Log(" RewardedAdLoaded ");
        rewardedAdBtn.SetActive(true);
        RewAdIsReady = true;
    }

    private void RewardedAdFailedToLoad()
    {
        Debug.Log(" RewardedAdFailedToLoad ");
    }

    private void RewardedAdOpening()
    {
        Debug.Log(" RewardedAdOpening ");
    }

    private void RewardedAdFailedToShow()
    {
        Debug.Log(" RewardedAdFailedToShow ");
    }

    private void UserEarnedReward()
    {        
        adHealth--;
        currentHealth++;
        UpdateHudUI();
        if (currentHealth == 1) DisplayReklamPanel(true);
        Debug.Log(" UserEarnedReward ");
    }

    private void RewardedAdClosed()
    {
        rewardedAdBtn.SetActive(false);
        //Ads if (adHealth >= 1) AdsManager.Instance.CreateAndLoadRewardedAd();
        Debug.Log(" RewardedAdClosed ");
    }

    #endregion

    #region Navigations

    public void RewardedAdClick()
    {
        if (currentHealth == 0) DisplayReklamPanel(false);
        //Ads AdsManager.Instance.UserChoseToWatchAd();
    }

    public void Cancel()
    {
        DisplayWarnMsgScreen(false);
    }

    public void NoThanks()
    {
        DisplayReklamPanel(false);
        GameFinish();
    }

    public void WacthRewAd()    
    {
        DisplayReklamPanel(false);
        adHealth--;
        currentHealth++;
        UpdateHudUI();
        if (currentHealth == 1)
        {
            YeniSoruGetir(false);
        }
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void MainMenu()
    {
        //Ads AdsManager.Instance.HideBanner();
        SceneManager.LoadScene((SceneManager.GetActiveScene().buildIndex) - 1);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    #endregion 
}

