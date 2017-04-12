using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Facebook.Unity;
using UnityEngine.UI;

public class ProfileController : MonoBehaviour, IScenesController
{
    public GameObject loadingIndicator;
    public GameObject btnLogout;
    public GameObject btnLinkFB;
    public GameObject btnUnlinkFB;

    public Text UserName;
    public Image ImageUser;
    public Text Streak;



    void Start()
    {
        UserInfo currenUser = TemporarilyStatus.getInstance().userInfo;

        if (TemporarilyStatus.getInstance().userInfo.isAnonymous == true)
        {
            btnLinkFB.gameObject.SetActive(true);
            btnUnlinkFB.gameObject.SetActive(false);
            UserName.text = currenUser.userID;
        }
        else
        {
            btnLinkFB.gameObject.SetActive(false);
            btnUnlinkFB.gameObject.SetActive(true);
            UserName.text = currenUser.username;            
        }

        //display user info: name, avatar...
        
    }

    public void OnBackButtonClick()
    {
        SceneManager.UnloadSceneAsync("Profile");
    }

    public void OnLinkToFacebookButtonClick()
    {
        //link to facbook
        var perms = new List<string>() { "public_profile", "email", "user_friends" };
        FB.LogInWithReadPermissions(perms, AuthCallbackForFBLinking);
    }

    public void OnLogOutButtonClick()
    {
        FirebaseHelper.getInstance().signOut();

        if (FB.IsLoggedIn)
        {
            FB.LogOut();
        }

        SceneManager.LoadScene("Login", LoadSceneMode.Single);
    }

    public void OnUnlinkFBButtonClick()
    {
        showHideLoadingIndicator(true);
        FirebaseHelper.getInstance().unlinkingAccount(userInfo =>
        {
            Debug.Log("OnUnlinkFBButtonClick");

            if (userInfo != null && userInfo.userID != "")
            {
                Debug.Log(String.Format("OnUnlinkButtonClick successfully :: {0}", userInfo.userID));

            }
            else
            {
                Debug.Log("OnUnlinkButtonClick failed.");
            }

            showHideLoadingIndicator(false);
        });
    }

    private void AuthCallbackForFBLinking(ILoginResult result)
    {
        if (FB.IsLoggedIn)
        {
            // AccessToken class will have session details
            var aToken = Facebook.Unity.AccessToken.CurrentAccessToken;

            showHideLoadingIndicator(true);
            FirebaseHelper.getInstance().linkingAccount(aToken.TokenString, handleLinkFBResult);

        }
        else
        {
            Debug.Log("Link fb canceled");
        }
    }

    private void handleLinkFBResult(UserInfo userInfo)
    {
        Debug.Log("handleLinkFBResult");
        if (userInfo != null && userInfo.firebase_token != "")
        {
            //show alert link successfully
            Debug.Log("Link fb successfully");
            showHideLoadingIndicator(false);

        }
        else
        {
            //after linking failed, anonymous account is also logged out too.
            //need to signin again???
            Debug.Log("Link fb failed");
            string curUserToken = PlayerPrefsHelper.loadUserToken();
            Debug.Log("loadUserToken :: " + curUserToken);

            if (curUserToken != null && curUserToken.Length > 0)
            {
                FirebaseHelper.getInstance().loginWithUserToken(curUserToken, anonymousUserInfo =>
                {
                    if (anonymousUserInfo != null && anonymousUserInfo.userID != "")
                    {
                        Debug.Log(String.Format("loginWithUserToken successfully :: {0}", anonymousUserInfo.userID));

                    }
                    else
                    {
                        //						showHideLoadingIndicator(false);
                        Debug.Log("loginWithUserToken failed");
                    }

                    showHideLoadingIndicator(false);
                });
            }
            else
            {
                showHideLoadingIndicator(false);
            }
        }

    }

    private void showHideLoadingIndicator(bool show)
    {
        if (loadingIndicator != null)
        {
            loadingIndicator.SetActive(show);
        }
    }
}
