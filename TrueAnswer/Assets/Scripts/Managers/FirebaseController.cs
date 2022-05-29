using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using Firebase;
using Firebase.Analytics;
using Firebase.Auth;
using Firebase.Extensions;
using Firebase.Database;
using Firebase.Unity;
using Firebase.Unity.Editor;
using Google;

[Serializable]
public class UserData
{
    public string userId = "";
    public string email = "userEmail";
    public string name = "userName";
    public string language = "Turkish";

    public bool premium = false;
    public bool extraJoker = false;

    public int highScore = 0;
}

[Serializable]
public class LeaderBoardData
{
    public string userId = "";
    public string name = "userName";
    public int highScore = 0;
}

public class FirebaseController : MonoBehaviour
{
    #region Variables

    [Header("References")]
    [SerializeField]
    private GameEvents events = null;
    public static FirebaseController Instance = null;

    #endregion

    #region Default Unity Methods

    private void OnEnable()
    {
        FirebaseDatabase.DefaultInstance.GetReference("UserData").ValueChanged += HandleUserDataValueChanged;
    }
    private void OnDisable()
    {
        FirebaseDatabase.DefaultInstance.GetReference("UserData").ValueChanged -= HandleUserDataValueChanged;
    }

    private void Awake()
    {
        Debug.Log("FirebaseController Awake");
        if (Instance == null) Instance = this;
        FirebaseApp.DefaultInstance.SetEditorDatabaseUrl("https://true-reply-2p0p2.firebaseio.com/");
        events.reference = FirebaseDatabase.DefaultInstance.RootReference;
    }

    #endregion

    #region Create and Receive Datas

    private void CreateNewUserData()
    {
        if (events.user.DisplayName == "" || events.user.DisplayName == null)
        {
            string newName = "user";
            Debug.Log("name is null");
            if (events.userData.name != "")
            {
                Debug.Log("name is events.userName");
                newName = events.userData.name;
            }
            else
            {
                Debug.LogError("name is random");
                for (int i = 0; i < 5; i++) newName += UnityEngine.Random.Range(0, 10);
            }
            UpdateUserProfile(newName, "");
        }

        UserData userdata = new UserData
        {
            premium = false,
            extraJoker = false,
            email = events.user.Email,
            name = events.user.DisplayName,
            highScore = 0,
            userId = events.user.UserId,
            language = "Turkish"
        };

        LeaderBoardData leaderBoardData = new LeaderBoardData
        {
            userId = events.user.UserId,
            name = events.user.DisplayName,
            highScore = 0
        };

        string jsonFileUD = JsonUtility.ToJson(userdata);
        string jsonFileLBD = JsonUtility.ToJson(leaderBoardData);

        try
        {
            events.reference.Child("UserData").Child(events.user.UserId).SetRawJsonValueAsync(jsonFileUD);
            events.reference.Child("LeaderBoard").Child(events.user.UserId).SetRawJsonValueAsync(jsonFileLBD);
            GetUserDatas();
        }
        catch (Exception e)
        {
            Debug.LogError("CreateNewUserData() was failed: " + e);
        }
    }

    public void GetUserDatas()
    {
        Debug.Log("GetUserDatas()");
        try
        {
            events.reference.Child("UserData").Child(events.user.UserId).GetValueAsync().ContinueWithOnMainThread(task =>
            {
                if (task.IsFaulted)
                {
                    Debug.LogError("GetUserData was faulted.");
                    return;
                }
                else if (task.IsCompleted)
                {
                    DataSnapshot snapshot = task.Result;
                    if (snapshot.GetRawJsonValue() == null)
                    {
                        Debug.LogWarning("User data is empty.");
                        CreateNewUserData();
                    }
                    else
                    {
                        Debug.Log("User data readed:\n" + snapshot.GetRawJsonValue());
                        events.userData = JsonUtility.FromJson<UserData>(snapshot.GetRawJsonValue());
                    }
                }
            });
        }
        catch (Exception e)
        {
            Debug.LogError("GetUserData() was failed: " + e);
        }
    }

    public void GetLeaderBoardDatas()
    {
        Debug.Log("GetLeaderBoardDatas()");
        try
        {
            FirebaseDatabase.DefaultInstance.GetReference("LeaderBoard").OrderByChild("highScore").LimitToLast(6)
                      .GetValueAsync().ContinueWithOnMainThread(task =>
                      {
                          if (task.IsFaulted)
                          {
                              Debug.LogError("FirebaseDatabase GetValueAsync encountered an error: " + task.Exception);
                              return;
                          }
                          else if (task.IsCompleted)
                          {
                              DataSnapshot snapshot = task.Result;
                              int count = 0;
                              foreach (DataSnapshot ld in snapshot.Children)
                              {
                                  LeaderBoardData ld_data = new LeaderBoardData();
                                  ld_data.userId = ld.Key;
                                  ld_data.name = ld.Child("name").Value.ToString();
                                  ld_data.highScore = Convert.ToInt32(ld.Child("highScore").Value);
                                  events.leaderBoardDatas[events.leaderBoardDatas.Length - 1 - count] = ld_data;
                                  count++;
                              }
                          }
                      });
        }
        catch (Exception e)
        {
            Debug.LogError("GetLeaderBoard() was failed: " + e);
        }
    }

    #endregion

    #region Update User Datas

    public void UpdateUserProfile(string name, string photoUrl)
    {
        Debug.Log("UpdateUserProfile()");
        Firebase.Auth.FirebaseUser user = events.auth.CurrentUser;
        if (user != null)
        {
            Firebase.Auth.UserProfile profile = new Firebase.Auth.UserProfile();
            profile.DisplayName = name;
            try
            {
                MenuManager.Instance.DisableBlockCanvas(true);
                user.UpdateUserProfileAsync(profile).ContinueWithOnMainThread(task =>
                            {
                                if (task.IsCanceled)
                                {
                                    Debug.LogError("UpdateUserProfileAsync was canceled.");
                                    return;
                                }
                                if (task.IsFaulted)
                                {
                                    Debug.LogError("UpdateUserProfileAsync encountered an error: " + task.Exception);
                                    return;
                                }
                                events.reference.Child("UserData").Child(
                                                       user.UserId).Child("name").SetValueAsync(name);
                                events.reference.Child("LeaderBoard").Child(
                                   user.UserId).Child("name").SetValueAsync(name);
                                Debug.Log("User profile updated successfully: " + events.auth.CurrentUser.DisplayName);
                                MenuManager.Instance.DisableBlockCanvas(false);
                            });
            }
            catch (Exception e)
            {
                Debug.LogError("UpdateUserProfile()/UpdateUserProfileAsync is failed: " + e);
                MenuManager.Instance.DisableBlockCanvas(false);
            }
            finally
            {
                Debug.Log("UpdateUserProfile()/UpdateUserProfileAsync is done.");
            }
        }
    }

    public void UpdateUserEmail(string eMail)
    {
        Debug.Log("UpdateUserEmail");
        Firebase.Auth.FirebaseUser user = events.auth.CurrentUser;
        if (user != null)
        {
            try
            {
                MenuManager.Instance.DisableBlockCanvas(true);
                user.UpdateEmailAsync(eMail).ContinueWithOnMainThread(task =>
                {
                    if (task.IsCanceled)
                    {
                        Debug.LogError("UpdateEmailAsync was canceled.");
                        MenuManager.Instance.DisableBlockCanvas(false);
                        return;
                    }
                    if (task.IsFaulted)
                    {
                        Debug.LogError("UpdateEmailAsync encountered an error: " + task.Exception);
                        MenuManager.Instance.DisableBlockCanvas(false);
                        return;
                    }
                    Debug.Log("User email updated successfully.");
                    events.reference.Child("UserData").Child(user.UserId).
                        Child("email").SetValueAsync(eMail).ContinueWithOnMainThread(task2 =>
                       {
                           LogOut();
                       });
                });
            }
            catch (Exception e)
            {
                Debug.LogError("UpdateUserEmail()/UpdateEmailAsync is failed: " + e);
                MenuManager.Instance.DisableBlockCanvas(false);
            }
            finally
            {
                Debug.Log("UpdateUserEmail()/UpdateEmailAsync is done. ");
            }
        }
    }

    public void UpdateUserPassword(string newPassword)
    {
        Debug.Log("UpdateUserPassword");
        Firebase.Auth.FirebaseUser user = events.auth.CurrentUser;

        if (user != null)
        {
            try
            {
                MenuManager.Instance.DisableBlockCanvas(true);
                user.UpdatePasswordAsync(newPassword).ContinueWithOnMainThread(task =>
                           {
                               if (task.IsCanceled)
                               {
                                   Debug.LogError("UpdatePasswordAsync was canceled.");
                                   MenuManager.Instance.DisableBlockCanvas(false);
                                   return;
                               }
                               if (task.IsFaulted)
                               {
                                   Debug.LogError("UpdatePasswordAsync encountered an error: " + task.Exception);
                                   MenuManager.Instance.DisableBlockCanvas(false);
                                   return;
                               }
                               Debug.Log("Password updated successfully.");
                               LogOut();
                           });
            }
            catch (Exception e)
            {
                Debug.LogError("UpdateUserPassword()/UpdatePasswordAsync is failed: " + e);
                MenuManager.Instance.DisableBlockCanvas(false);
            }
            finally
            {
                Debug.Log("UpdateUserPassword()/UpdatePasswordAsync is done.");
            }
        }
    }

    #endregion

    private void HandleUserDataValueChanged(object sender, ValueChangedEventArgs args)
    {
        Debug.Log("HandleUserDataValueChanged()");

        if (args.DatabaseError != null)
        {
            Debug.LogError(args.DatabaseError.Message);
            return;
        }
        if (events.reference == null || events.user == null)
        {
            Debug.Log("reference or user is null");
            return;
        }
    }

    public void LogOut()
    {
        try
        {
            PlayerPrefs.DeleteAll();
            PlayerPrefs.SetString(GameRecords.SaveLastUserMail, events.auth.CurrentUser.Email);
            if (Firebase.Auth.FirebaseAuth.DefaultInstance != null) Firebase.Auth.FirebaseAuth.DefaultInstance.SignOut();
            if (GoogleSignIn.DefaultInstance != null)
            {
                Debug.Log("Gs not null");
                GoogleSignIn.DefaultInstance.SignOut();
            }
            else
            {
                Debug.Log("Gs is null");
                GoogleSignIn.DefaultInstance.SignOut();
            }
        }
        catch (Exception e)
        {
            Debug.LogError("Log out.. exception:" + e);
            MenuManager.Instance.DisableBlockCanvas(false);
        }
        finally
        {
            SceneManager.LoadScene((SceneManager.GetActiveScene().buildIndex) - 1);
        }
    }
}
