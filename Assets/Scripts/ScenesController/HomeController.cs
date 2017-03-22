using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Facebook.Unity;

public class HomeController : MonoBehaviour {

	void Awake () {
		//check login status again
		//if it is logged out due to some unknown reason 
		//it could take time to complete this progress => show loading indicator
		Debug.Log("checkLoginStatus");
		if (FirebaseHelper.getInstance().checkLoginStatus() == false) {
			Debug.Log("Not logged in");
			//show login screen

			//for testing => login facebook is default
			if (!FB.IsInitialized) {
				// Initialize the Facebook SDK
				FB.Init(InitCallback, OnHideUnity);

			} else {
				// Already initialized, signal an app activation App Event
				Debug.Log("Already initialized, signal an app activation App Event");
				FB.ActivateApp();
			}

		} else {
			Debug.Log("Logged in already");
			//load today learning data
			loadTodayData();
		}
	}

	public void loadTodayData () {
		Debug.Log("loadTodayData");

		int reviewCount = 0;
		int newCount = 0;
		FirebaseHelper.getInstance().prepareInreviewList(TemporarilyStatus.getInstance().total_card_a_day,
			reviewRes => {
				reviewCount = reviewRes;
				Debug.Log("Review count :: " +reviewRes);

				newCount = TemporarilyStatus.getInstance().total_card_a_day - reviewCount;
				if (newCount > TemporarilyStatus.getInstance().new_card_a_day) {
					newCount = TemporarilyStatus.getInstance().new_card_a_day;
				}

				FirebaseHelper.getInstance().prepareListNewWordsToLearn(newCount,
					newRes => {

						Debug.Log("New count :: " +newRes);
					});
			});
	}

	/* for testing - begin */
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

	public void OnLoginButtonFBClick()	{
		var perms = new List<string>(){"public_profile", "email", "user_friends"};
		FB.LogInWithReadPermissions(perms, AuthCallback);
	}

	private void AuthCallback (ILoginResult result) {
		if (FB.IsLoggedIn) {
			// AccessToken class will have session details
			var aToken = Facebook.Unity.AccessToken.CurrentAccessToken;

			FirebaseHelper.getInstance().loginWithFacebook(aToken.TokenString, handleSigninResult);

		} else {
			Debug.Log("User cancelled login");
//			test.text = "AuthCallback failed";
		}
	}

	void handleSigninResult(UserInfo userInfo) {
		Debug.Log("handleSigninResult");
//		if (userInfo != null && userInfo.userID != "") {
//			test.text = String.Format("fb login successfully :: {0}", userInfo.userID);
//		} else {
//			test.text = "fb login failed";
//		}

	}
	/* for testing - end */

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
