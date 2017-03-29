using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Facebook.Unity;

public class LoginController : MonoBehaviour {
	public Text test;
	// Use this for initialization
	void Start () {
		_checkCurrentUser();
	}
	
	void Awake () {
		FirebaseHelper.getInstance();
		if (!FB.IsInitialized) {
			// Initialize the Facebook SDK
			FB.Init(InitCallback, OnHideUnity);

		} else {
			// Already initialized, signal an app activation App Event
			Debug.Log("Already initialized, signal an app activation App Event");
			FB.ActivateApp();
		}
	}

	private void InitCallback () {
		if (FB.IsInitialized) {
			// Signal an app activation App Event
			FB.ActivateApp();
			// Continue with Facebook SDK
			// ...
		} else {
			Debug.Log("Failed to Initialize the Facebook SDK");
		}
	}

	private void OnHideUnity (bool isGameShown)	{
		if (!isGameShown) {
			// Pause the game - we will need to hide
			Time.timeScale = 0;
		} else {
			// Resume the game - we're getting focus again
			Time.timeScale = 1;
		}
	}

	public void OnLoginButtonFBClick() {
		var perms = new List<string>(){"public_profile", "email", "user_friends"};
		FB.LogInWithReadPermissions(perms, AuthCallback);
	}

	private void AuthCallback (ILoginResult result) {
		if (FB.IsLoggedIn) {
			// AccessToken class will have session details
			var aToken = Facebook.Unity.AccessToken.CurrentAccessToken;

			FirebaseHelper.getInstance().loginWithFacebook(aToken.TokenString, handleSigninFBResult);

		} else {
			Debug.Log("User cancelled login");
			test.text = "AuthCallback failed";
		}
	}

	void handleSigninFBResult(UserInfo userInfo) {
		Debug.Log("handleSigninFBResult");
		if (userInfo != null && userInfo.userID != "") {
//			test.text = String.Format("fb login successfully :: {0}", userInfo.userID);

			Debug.Log(String.Format("fb login successfully :: {0}", userInfo.userID));
			//load home screen
			configUserSettings(() => {
				SceneManager.LoadScene("Home");	
			});

		} else {
			test.text = "FB login failed";
		}

	}

	//click login anonymous
	public void OnLoginButtonAnonymousClick () {
		FirebaseHelper.getInstance().loginAsAnnonymousUser(userInfo => {
			if (userInfo != null && userInfo.userID != "") {
//				test.text = String.Format("Anonymous login successfully :: {0}", userInfo.userID);

				Debug.Log(String.Format("Anonymous login successfully :: {0}", userInfo.userID));
				//load home screen
				configUserSettings(() => {
					SceneManager.LoadScene("Home");	
				});

			} else {
				test.text = "no anonymous signed in";
			}
		});
	}

	//just for testing
	public void OnLinkFBAccountClick () {
//		UserInfo user = FirebaseHelper.getInstance().checkCurrentUser();

		var perms = new List<string>(){"public_profile", "email", "user_friends"};
		FB.LogInWithReadPermissions(perms, AuthCallbackForFBLinking);
	}

	private void AuthCallbackForFBLinking (ILoginResult result) {
		if (FB.IsLoggedIn) {
			// AccessToken class will have session details
			var aToken = Facebook.Unity.AccessToken.CurrentAccessToken;

			FirebaseHelper.getInstance().linkingAccount(aToken.TokenString, handleLinkFBResult);

		} else {
			Debug.Log("User cancelled login");
			test.text = "AuthCallbackForFBLinking failed";
		}
	}

	void handleLinkFBResult(UserInfo userInfo) {
		Debug.Log("handleLinkFBResult");
		if (userInfo != null && userInfo.firebase_token != "") {
			test.text = String.Format("Link successfully :: {0}", userInfo.userID);

			configUserSettings(() => {
				SceneManager.LoadScene("Home");	
			});

		} else {
			test.text = String.Format("Link failed :: {0}", userInfo.userID);

			//after linking failed, anonymous account is also logged out too.
			//need to signin again???
		}

	}

	private void _checkCurrentUser () {
		UserInfo userInfo = FirebaseHelper.getInstance().getCurrentUserInfo();

		if (userInfo != null && userInfo.userID != "") {
			Debug.Log(String.Format("Logged in already :: {0}", userInfo.userID));
			//load settings and load home screen
			configUserSettings(() => {
				SceneManager.LoadScene("Home");	
			});

		} else {
			test.text = "no user signed in";
		}
	}

	//click unlink
	public void OnUnlinkButtonClick () {
		FirebaseHelper.getInstance().unlinkingAccount(userInfo => {
			test.text = "OnUnlinkButtonClick";
			if (userInfo != null && userInfo.userID != "") {
				test.text = String.Format("OnUnlinkButtonClick successfully :: {0}", userInfo.userID);

			} else {
				test.text = "OnUnlinkButtonClick failed.";
			}
		});

		//FirebaseHelper.getInstance().DisplayProviders();
	}

	public void OnLogOutButtonClick() {
		FirebaseHelper.getInstance().signOut();

		if (FB.IsLoggedIn) {
			FB.LogOut();
		}

		test.text = "Logged out";
	}

	public void configUserSettings (System.Action callbackWhenDone) {
		FirebaseHelper.getInstance().getUserSettings(isExist => {
			//if isExist is true, user settings are set in TemporaryStatus already
			if (isExist == false) {
				FirebaseHelper.getInstance().configDefaultSettings();
			}

			callbackWhenDone();
		});
	}
}
