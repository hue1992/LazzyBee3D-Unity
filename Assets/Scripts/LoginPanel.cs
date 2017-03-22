using Facebook.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class LoginPanel : MonoBehaviour
{

    public Button loginAsGuest;
    public Button loginWithFacebookGuest;
    public Button btnSingOut;
    public GameObject loginPanel;
    public GameObject login;
    Firebase.Auth.FirebaseAuth mAuth;

    public void Start()
    {
        mAuth = Firebase.Auth.FirebaseAuth.DefaultInstance;
    }

    public void LoginAsGuest()
    {
        Debug.Log("LoginAsGuest");
        login.SetActive(false);
        btnSingOut.gameObject.SetActive(true);

        //Firebase.Auth.Credential credential = Firebase.Auth.FacebookAuthProvider.GetCredential(Facebook.Unity.AccessToken.CurrentAccessToken.TokenString);
        //mAuth.SignInAnonymouslyAsync().ContinueWith(task =>
        //{
        //    if (task.IsCanceled)
        //    {
        //        DebugOnScreen.Log("SignInWithCredentialAsync was canceled.");
        //        return;
        //    }
        //    if (task.IsFaulted)
        //    {
        //        DebugOnScreen.Log("SignInWithCredentialAsync encountered an error: " + task.Exception);
        //        return;
        //    }

        //    Firebase.Auth.FirebaseUser newUser = task.Result;
        //    DebugOnScreen.Log("User signed in successfully: " + newUser.DisplayName + " (," + newUser.UserId + ")");
        //});

    }

    public void LoginWithFacebook()
    {
        Debug.Log("LoginWithFacebook");
        login.SetActive(false);
        btnSingOut.gameObject.SetActive(true);
    }

    private void logiWithFacebook()
    {
        var perms = new List<string>() { "public_profile", "email", "user_friends" };
        FB.LogInWithReadPermissions(perms, FacebookAuthCallback);
    }

    private void FacebookAuthCallback(ILoginResult result)
    {
        if (FB.IsLoggedIn)
        {
            // AccessToken class will have session details
            var aToken = AccessToken.CurrentAccessToken;
            Debug.Log(AccessToken.CurrentAccessToken.TokenString);

            Firebase.Auth.Credential credential = Firebase.Auth.FacebookAuthProvider.GetCredential(AccessToken.CurrentAccessToken.TokenString);
            mAuth.SignInWithCredentialAsync(credential).ContinueWith(task =>
            {
                if (task.IsCanceled)
                {
                    return;
                }
                if (task.IsFaulted)
                {
                    return;
                }

                Firebase.Auth.FirebaseUser newUser = task.Result;
                Debug.Log("User signed in successfully: " + newUser.DisplayName + " (," + newUser.UserId + ")");
            });

        }
        else
        {
            Debug.Log("User cancelled login");
        }
    }

    public void SingOutClick()
    {
        if (FB.IsLoggedIn)
        {
            FB.LogOut();
        }

        if (mAuth.CurrentUser != null)
        {
            mAuth.SignOut();
        }

        login.SetActive(true);
        btnSingOut.gameObject.SetActive(false);
    }
}
