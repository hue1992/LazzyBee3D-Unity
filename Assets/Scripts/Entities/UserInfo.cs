using System;
using System.Collections;
using System.Collections.Generic;

[Serializable]
public class UserInfo {
	public string userID;
	public string username;
	public string email;
	public string firebase_token;

	public UserInfo() {
		userID = "";
		username = "";
		email = "";
		firebase_token = "";
	} 
}
