using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
//using GoogleMobileAds.Api;

public class AdsManager : MonoBehaviour
{
    /*[Header("References")]
    [SerializeField] private GameEvents events = null;

    [Space]
    [Header("Application ID")]
    public string idAndroid_App = "";
    public string idIOS_App = "";

    [Space]
    [Header("Ad Units")]
    public string idAndroidBan = "";
    public string idAndroidInt = "";
    public string idAndroidRew = "";

    private BannerView banner;
    private InterstitialAd interstitialAd;
    private RewardedAd rewardedAd;

    public static AdsManager Instance = null;

    private void Awake()
    {
        if (Instance == null) Instance = this;
    }

    private void Start()
    {
        MobileAds.Initialize(initStatus => { });
    }

    public void SendRequestes()
    {
        RequestBanner();
        RequestInterstitial();
        CreateAndLoadRewardedAd();
    }

    #region Banner

    public void RequestBanner()
    {
        this.banner = new BannerView(idAndroidBan, AdSize.Banner, AdPosition.Top);

        // Called when an ad request has successfully loaded.
        this.banner.OnAdLoaded += this.HandleOnAdLoaded;
        // Called when an ad request failed to load.
        this.banner.OnAdFailedToLoad += this.HandleOnAdFailedToLoad;
        // Called when an ad is clicked.
        this.banner.OnAdOpening += this.HandleOnAdOpened;
        // Called when the user returned from the app after an ad click.
        this.banner.OnAdClosed += this.HandleOnAdClosed;
        // Called when the ad click caused the user to leave the application.
        this.banner.OnAdLeavingApplication += this.HandleOnAdLeavingApplication;

        AdRequest request = new AdRequest.Builder().Build();
        this.banner.LoadAd(request);
        banner.Show();
    }

    public void HideBanner()
    {
        banner.Hide();
    }

    #endregion

    #region  Banner Ad Events

    public void HandleOnAdLoaded(object sender, EventArgs args)
    {
        Debug.Log("");
        MonoBehaviour.print("HandleAdLoaded event received");
    }
    public void HandleOnAdFailedToLoad(object sender, AdFailedToLoadEventArgs args)
    {
        MonoBehaviour.print("HandleFailedToReceiveAd event received with message: "
                            + args.Message);
    }

    public void HandleOnAdOpened(object sender, EventArgs args)
    {
        MonoBehaviour.print("HandleAdOpened event received");
    }

    public void HandleOnAdClosed(object sender, EventArgs args)
    {
        MonoBehaviour.print("HandleAdClosed event received");
    }

    public void HandleOnAdLeavingApplication(object sender, EventArgs args)
    {
        MonoBehaviour.print("HandleAdLeavingApplication event received");
    }

    #endregion

    #region Interstitial

    private void RequestInterstitial()
    {
        // Initialize an InterstitialAd.
        this.interstitialAd = new InterstitialAd(idAndroidInt);

        // Called when an ad request has successfully loaded.
        this.interstitialAd.OnAdLoaded += HandleOnAdLoaded;
        // Called when an ad request failed to load.
        this.interstitialAd.OnAdFailedToLoad += HandleOnAdFailedToLoad;
        // Called when an ad is shown.
        this.interstitialAd.OnAdOpening += HandleOnAdOpened;
        // Called when the ad is closed.
        this.interstitialAd.OnAdClosed += InterstitialAdClosed;
        // Called when the ad click caused the user to leave the application.
        this.interstitialAd.OnAdLeavingApplication += HandleOnAdLeavingApplication;


        // Create an empty ad request.
        AdRequest request = new AdRequest.Builder().Build();
        // Load the interstitial with the request.
        this.interstitialAd.LoadAd(request);

    }

    public void ShowGec()
    {
        //GameManager.Instance.ReklamIzleniyor();
        if (this.interstitialAd.IsLoaded())
        {
            this.interstitialAd.Show();
        }
        else
        {
            Debug.Log("interstitial Ad is not loaded");
        }
    }

    private void InterstitialAdClosed(object sender, EventArgs e)
    {
        //GameManager.Instance.ReklamIzlemeSonaErdi(false);
        this.RequestInterstitial();
    }

    #endregion

    #region Rewarded

    public void CreateAndLoadRewardedAd()
    {
        this.rewardedAd = new RewardedAd(idAndroidRew);

        // Called when an ad request has successfully loaded.
        this.rewardedAd.OnAdLoaded += HandleRewardedAdLoaded;
        // Called when an ad request failed to load.
        this.rewardedAd.OnAdFailedToLoad += HandleRewardedAdFailedToLoad;
        // Called when an ad is shown.
        this.rewardedAd.OnAdOpening += HandleRewardedAdOpening;
        // Called when an ad request failed to show.
        this.rewardedAd.OnAdFailedToShow += HandleRewardedAdFailedToShow;
        // Called when the user should be rewarded for interacting with the ad.
        this.rewardedAd.OnUserEarnedReward += HandleUserEarnedReward;
        // Called when the ad is closed.
        this.rewardedAd.OnAdClosed += HandleRewardedAdClosed;

        AdRequest request = new AdRequest.Builder().Build();
        this.rewardedAd.LoadAd(request);
    }

    public void UserChoseToWatchAd()
    {
        if (this.rewardedAd.IsLoaded())
        {
            this.rewardedAd.Show();
        }
        else
        {
            Debug.Log("Rewarded Ad is not loaded");
        }
    }

    #endregion

    #region Rewarded Ad Events
    private void HandleRewardedAdLoaded(object sender, EventArgs args)
    {
        events.RewardedAdLoaded?.Invoke();
    }
    private void HandleRewardedAdFailedToLoad(object sender, AdErrorEventArgs args)
    {
        events.RewardedAdFailedToLoad?.Invoke();
    }
    private void HandleRewardedAdOpening(object sender, EventArgs args)
    {
        events.RewardedAdOpening?.Invoke();
    }
    private void HandleRewardedAdFailedToShow(object sender, AdErrorEventArgs args)
    {
        events.RewardedAdFailedToShow?.Invoke();
    }
    private void HandleUserEarnedReward(object sender, Reward args)
    {
        events.UserEarnedReward?.Invoke();
    }
    private void HandleRewardedAdClosed(object sender, EventArgs args)
    {
        //this.CreateAndLoadRewardedAd();
        events.RewardedAdClosed?.Invoke();
    }
    #endregion*/
}
