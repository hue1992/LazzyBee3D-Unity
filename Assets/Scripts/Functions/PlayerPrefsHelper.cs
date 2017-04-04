using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPrefsHelper {
	//PlayerPrefs key
	private const string PLAYER_PREFS_USER_TOKEN_KEY = "PrefsUserTokenKey";

	public static void saveUserToken (string userToken) {
		PlayerPrefs.SetString(PLAYER_PREFS_USER_TOKEN_KEY, userToken);
	}

	public static string loadUserToken () {
		return PlayerPrefs.GetString(PLAYER_PREFS_USER_TOKEN_KEY);
	}

}
