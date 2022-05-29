using UnityEngine;
using Firebase.Auth;
using Firebase.Database;

[CreateAssetMenu(fileName = "GameEvents", menuName = "New GameEvents")]
public class GameEvents : ScriptableObject
{
    public delegate void RewardedAdLoadedCallback();
    public RewardedAdLoadedCallback RewardedAdLoaded = null;

    public delegate void RewardedAdFailedToLoadCallback();
    public RewardedAdFailedToLoadCallback RewardedAdFailedToLoad = null;

    public delegate void RewardedAdOpeningCallback();
    public RewardedAdOpeningCallback RewardedAdOpening = null;

    public delegate void RewardedAdFailedToShowCallback();
    public RewardedAdFailedToShowCallback RewardedAdFailedToShow = null;

    public delegate void UserEarnedRewardCallback();
    public UserEarnedRewardCallback UserEarnedReward = null;

    public delegate void RewardedAdClosedCallback();
    public RewardedAdClosedCallback RewardedAdClosed = null;

    public  FirebaseAuth auth = null;
    public  FirebaseUser user = null;
    public  Firebase.FirebaseApp app = null;
    public  DatabaseReference reference = null;

    [Header("User Datas")]
    public  UserData userData = new UserData();
    [Space]
    [Header("LeaderBoard Datas")]
    public  LeaderBoardData[] leaderBoardDatas = new LeaderBoardData[6];
    [Space]
    [Header("Settings")]
    public bool playModeisONLINE = false;
    [Space]
    public bool soundEffect = true;
    public bool gameMusic = true;
    public bool vibration = true;
    [Space]
    public int maxLevel = 100;
}

