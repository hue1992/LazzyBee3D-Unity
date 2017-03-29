using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

#if !UNITY_WEBGL
using Firebase;
using Firebase.Database;
using Firebase.Unity.Editor;
using System.Threading.Tasks;

public class FirebaseHelper  {
	private const string DICTIONARY 		= "dictionary";
	private const string LEARNING_PROGRESS 	= "learning_progress";
	private const string LEVELS 			= "levels";
	private const string NOTES 				= "notes";
	private const string SETTINGS 			= "settings";
	private const string STREAKS 			= "streaks";
	private const string USERS 				= "users";

	//learning progress key
	private const string PROGRESS_INPROGRESS_KEY 	= "inprogress";
	private const string PROGRESS_INREVIEW_KEY 		= "inreview";
	private const string PROGRESS_NEWWORDS_KEY 		= "newwords";
	private const string PROGRESS_AGAIN_KEY 		= "again";
	private const string PROGRESS_CURRENT_STS_KEY 	= "current_status";

	private const string PROGRESS_WORD_KEY 	= "words";
	private const string PROGRESS_DATE_KEY 	= "date";
	private const string PROGRESS_COUNT_KEY = "count";

	//settings key
	private const string SETTINGS_MY_LEVEL_KEY 			= "my_level";
	private const string SETTINGS_AUTOPLAY_KEY 			= "auto_play_sound";
	private const string SETTINGS_NEW_CARD_KEY 			= "new_card_a_day";
	private const string SETTINGS_TOTAL_CARD_KEY 		= "total_card_a_day";
	private const string SETTINGS_TIME_SHOW_ANSWER_KEY 	= "time_to_show_answer";
	private const string SETTINGS_NOTIFICATION_KEY 		= "notification";

    private static FirebaseHelper _instance = null;
	private static Firebase.Auth.FirebaseAuth auth = null;
	private static Firebase.Auth.FirebaseUser firebaseUser = null;

    private bool initiated = false;
	bool signedIn = false;

	void OnDestroy() {
		auth.StateChanged -= AuthStateChanged;
		auth = null;
	}

    public static FirebaseHelper getInstance() {
        if (_instance == null) {
            _instance = new FirebaseHelper();

			_instance.initFirebase();
        }
        return _instance;
    }

	private void _initFirebase() {
		FirebaseApp app = FirebaseApp.DefaultInstance;
		auth = Firebase.Auth.FirebaseAuth.DefaultInstance;

		app.SetEditorDatabaseUrl("https://lazeebee-977.firebaseio.com/");
		//app.SetEditorP12FileName ("filebaseTest-2e653eef7319.p12");
		app.SetEditorServiceAccountEmail("born2go.com@gmail.com");

		auth.StateChanged += AuthStateChanged;
		AuthStateChanged(this, null);

		initiated = true;
	}

	// Track state changes of the auth object.
	void AuthStateChanged(object sender, System.EventArgs eventArgs) {
		Debug.Log("AuthStateChanged");

		if (auth.CurrentUser != firebaseUser) {
			signedIn = firebaseUser != auth.CurrentUser && auth.CurrentUser != null;
			if (!signedIn && firebaseUser != null) {
				Debug.Log("Signed out :: " + firebaseUser.UserId);
			}

			firebaseUser = auth.CurrentUser;

			if (signedIn) {
				Debug.LogFormat("AuthStateChanged :: User Changed: {0} ({1})",
					firebaseUser.DisplayName, firebaseUser.UserId);
				
			} else {
				Debug.Log("AuthStateChanged :: No user is signed in :: "  + firebaseUser.UserId);
			}
		}
	}

    DependencyStatus dependencyStatus = DependencyStatus.UnavailableOther;
    public void initFirebase() {
        if (!initiated) {
            dependencyStatus = FirebaseApp.CheckDependencies();
            if (dependencyStatus != DependencyStatus.Available) {	//if jump into this case => bug => will fix later
                FirebaseApp.FixDependenciesAsync().ContinueWith(task => {
					
                    dependencyStatus = FirebaseApp.CheckDependencies();
                    if (dependencyStatus == DependencyStatus.Available) {
                        initiated = true;
                        _initFirebase();

                    } else {
                        // This should never happen if we're only using Firebase Analytics.
                        // It does not rely on any external dependencies.
                        Debug.LogError("Could --  not resolve all Firebase dependencies: " + dependencyStatus);
                    }
                });
            } else {
				Debug.Log("_initFirebase");
                _initFirebase();
            }
        }
    }

	/**************** functions for USERS *****************/
	public void loginAsAnnonymousUser(System.Action<UserInfo> callbackWhenDone) {
		Debug.Log("loginAsAnnonymousUser");

		if (signedIn == false) {	//only allow to login if it is not logged in
			auth.SignInAnonymouslyAsync().ContinueWith(task => {
				UserInfo user = new UserInfo();

				if (task.IsCanceled) {
					Debug.LogError("SignInAnonymouslyAsync was canceled.");
					callbackWhenDone(user);
					return;
				}
				if (task.IsFaulted) {
					Debug.LogError("SignInAnonymouslyAsync encountered an error: " + task.Exception);
					callbackWhenDone(user);
					return;
				}

				firebaseUser = task.Result;
				Debug.LogFormat("User signed in successfully: {0} ({1})",
					firebaseUser.DisplayName, firebaseUser.UserId);

				Debug.Log("RefreshToken :: " + firebaseUser.RefreshToken);
				Debug.Log("DisplayName :: " + firebaseUser.DisplayName);
				Debug.Log("UserId :: " + firebaseUser.UserId);

				user.userID = firebaseUser.UserId;
				user.username = firebaseUser.DisplayName;
				user.firebase_token = firebaseUser.RefreshToken;

				createNewUser(user);

				callbackWhenDone(user);
			});
		} else {
			Debug.Log("there is a user logged in already");

			callbackWhenDone(null);
		}
	}

	public void loginWithFacebook(string accessToken, System.Action<UserInfo> callbackWhenDone) {

		if (signedIn == false) {	//only allow to login if it is not logged in
			Firebase.Auth.Credential credential =
				Firebase.Auth.FacebookAuthProvider.GetCredential(accessToken);

			auth.SignInWithCredentialAsync(credential).ContinueWith(task => {
				UserInfo user 		= new UserInfo();

				if (task.IsCanceled) {
					Debug.LogError("FB :: SignInWithCredentialAsync was canceled.");
					callbackWhenDone(user);
					return;
				}
				if (task.IsFaulted) {
					Debug.LogError("FB :: SignInWithCredentialAsync encountered an error: " + task.Exception);
					callbackWhenDone(user);
					return;
				}

				firebaseUser = task.Result;
				Debug.LogFormat("FB :: User signed in successfully: {0} ({1})",
					firebaseUser.DisplayName, firebaseUser.UserId);

				user.userID 		= firebaseUser.UserId;
				user.email 			= firebaseUser.Email;
				user.username 		= firebaseUser.DisplayName;
				user.firebase_token = firebaseUser.RefreshToken;

				createNewUser(user);

				callbackWhenDone(user);
			});
		} else {
			Debug.Log("there is a user logged in already");

			callbackWhenDone(null);
		}
	}

	public void linkingAccount(string accessToken, System.Action<UserInfo> callbackWhenDone) {

		if (signedIn == true) {
			Firebase.Auth.Credential credential =
				Firebase.Auth.FacebookAuthProvider.GetCredential(accessToken);

			firebaseUser.LinkWithCredentialAsync(credential).ContinueWith(task => {
				UserInfo newInfo = new UserInfo();

				if (task.IsCanceled) {
					Debug.LogError("LinkWithCredentialAsync was canceled.");
					newInfo.userID = task.Exception.ToString();
					callbackWhenDone(newInfo);
					return;
				}
				if (task.IsFaulted) {
					Debug.LogError("LinkWithCredentialAsync encountered an error: " + task.Exception);
					newInfo.userID = task.Exception.ToString();
					callbackWhenDone(newInfo);
					return;
				}

				if (auth.CurrentUser != firebaseUser) {
					Debug.LogError("linkingAccount :: There is something wrong :: auth.CurrentUser != firebaseUser");
				}

				firebaseUser = task.Result;
				Debug.LogFormat("Credentials successfully linked to Firebase user: {0} ({1})",
					firebaseUser.DisplayName, firebaseUser.UserId);

				//update user info
				newInfo.userID			= firebaseUser.UserId;
				newInfo.email 			= firebaseUser.Email;
				newInfo.username 		= firebaseUser.DisplayName;
				newInfo.firebase_token 	= firebaseUser.RefreshToken;

				updateUserInfo(newInfo);

				//call back
				callbackWhenDone(newInfo);

			});

		} else {
			Debug.Log("No user is signed in");
			Debug.Log("will sign in as a new user");

			callbackWhenDone(null);
		}
	}

	public void unlinkingAccount(System.Action<UserInfo> callbackWhenDone) {
		// Unlink the sign-in provider from the currently active user.
		// providerIdString is a string identifying a provider,
		// retrieved via FirebaseAuth.FetchProvidersForEmail(). @Nam: why must invoke this function while we have it via firebaseUser.ProviderId

		Debug.Log("unlinkingAccount :: ProviderId :: " + firebaseUser.ProviderId);
		Debug.Log("unlinkingAccount :: ProviderId :: " + firebaseUser.Email);
		if (signedIn == true) {
			displayProviders(provider => {
				auth.CurrentUser.UnlinkAsync(provider).ContinueWith(task => {
					UserInfo newInfo = new UserInfo();

					if (task.IsCanceled) {
						Debug.LogError("unlinkingAccount was canceled.");
						callbackWhenDone(newInfo);

						return;
					}

					if (task.IsFaulted) {
						Debug.LogError("unlinkingAccount encountered an error: " + task.Exception);
						callbackWhenDone(newInfo);

						return;
					}

					if (auth.CurrentUser != firebaseUser) {
						Debug.LogError("unlinkingAccount :: There is something wrong??? :: auth.CurrentUser != firebaseUser");
					}

					// The user has been unlinked from the provider.
					firebaseUser = task.Result;
					Debug.LogFormat("Credentials successfully unlinked from user: {0} ({1})",
						firebaseUser.DisplayName, firebaseUser.UserId);

					newInfo.userID = firebaseUser.UserId;
					newInfo.username = firebaseUser.DisplayName;
					newInfo.firebase_token = firebaseUser.RefreshToken;

					//call back
					callbackWhenDone(newInfo);

				});	
			});
		} else {
			Debug.Log("No user is signed in");

			callbackWhenDone(null);
		}
	}

	// Show the providers for the current email address.
	private void displayProviders(System.Action<string> callbackWhenDone) {
		auth.FetchProvidersForEmailAsync(firebaseUser.Email).ContinueWith((authTask) => {
			if (authTask != null) {
				Debug.Log(String.Format("Email Providers for '{0}': ", firebaseUser.Email));
				string provider = "";
				foreach (string pv in authTask.Result) {
					provider = pv;
				}
				Debug.Log("provider");
				Debug.Log("provider :: " + provider);
				callbackWhenDone(provider);
			}
		});
	}

	public UserInfo getCurrentUserInfo () {

		if (firebaseUser != null) {
			// User is signed in.
			Debug.Log("RefreshToken :: " + firebaseUser.RefreshToken);
			Debug.Log("DisplayName :: " + firebaseUser.DisplayName);
			Debug.Log("UserId :: " + firebaseUser.UserId);

			UserInfo userRes = new UserInfo();
			userRes.userID = firebaseUser.UserId;
			userRes.username = firebaseUser.DisplayName;
			userRes.firebase_token = firebaseUser.RefreshToken;

			TemporarilyStatus.getInstance().userInfo = userRes;

			return userRes;

		} else {
			// No user is signed in.
			Debug.Log("No user is signed in");
		}

		return null;
	}

	//false: no user is signed in
	public bool checkLoginStatus () {
		return signedIn;
	}

	public void signOut() {

		auth.SignOut();
	}

	//create new user
	public void createNewUser (UserInfo user) {

		TemporarilyStatus.getInstance().userInfo = user;

		FirebaseDatabase.DefaultInstance
			.GetReference(USERS)
			.Child(user.userID)
			.SetRawJsonValueAsync(JsonUtility.ToJson(user));
	}

	//update user
	public void updateUserInfo (UserInfo user) {

		if (user.userID != null && user.userID.Length > 0) {

			TemporarilyStatus.getInstance().userInfo = user;

			Dictionary<string, object> userUpdate = new Dictionary<string, object> ();
			userUpdate["userID"]	 		= user.userID;
			userUpdate["username"] 			= user.username;
			userUpdate["email"] 			= user.email;
			userUpdate["firebase_token"] 	= user.firebase_token;

			FirebaseDatabase.DefaultInstance
				.GetReference(USERS)
				.Child(user.userID)
				.UpdateChildrenAsync(userUpdate);
		}
	}

	/**************** functions for DICTIONARY *****************/

	//get all words
	public void getListWords (int limit, System.Action<WordInfo[]> callbackWhenDone) {
		Debug.Log("FirebaseHelper :: getListWords");
		if (limit > 20) {	//due to limited quota
			limit = 20;
		}
		FirebaseDatabase.DefaultInstance
			.GetReference(DICTIONARY)
			.OrderByKey()
			.LimitToFirst(limit)
			.GetValueAsync().ContinueWith(task => {
				if (task.IsFaulted) {
					// Handle the error...
					Debug.Log("getListWords :: task :: error" + task.ToString());
					callbackWhenDone(new WordInfo[0]);

				} else if (task.IsCompleted) {
					DataSnapshot snapshot = task.Result;
					List<WordInfo> wordsList = new List<WordInfo>();
					// Do something with snapshot...
					Debug.Log("snapshot.Children :: count :: " + snapshot.ChildrenCount);
					foreach (var item in snapshot.Children) {
						Debug.Log("snapshot.Children :: " + item.Key);
						WordInfo wd = JsonUtility.FromJson<WordInfo>(item.GetRawJsonValue());

						wd.word = item.Key;
						wordsList.Add(wd);
					}

					callbackWhenDone(wordsList.ToArray());
				}
			});
	}

	//get information of a word
	public void fetchWordInformation(string word, System.Action<WordInfo> callbackWhenDone) {
		Debug.Log("FirebaseHelper :: fetchWordInformation :: " + word);

		if (word.Length > 0) {
			FirebaseDatabase.DefaultInstance
				.GetReference(DICTIONARY)
				.Child(word)
				.GetValueAsync().ContinueWith(task => {
					if (task.IsFaulted) {
						// Handle the error...
						Debug.Log("fetchWordInformation :: task :: error" + task.ToString());
						callbackWhenDone(new WordInfo());

					} else if (task.IsCompleted) {
						DataSnapshot snapshot = task.Result;
						Debug.Log("snapshot :: " + snapshot.Key);

						if (snapshot != null) {
							string rawInfo = snapshot.GetRawJsonValue();
							Debug.Log("fetchWordInformation :: rawInfo :: " + rawInfo);
							if (rawInfo != null) {
								Debug.Log("fetchWordInformation :: rawInfo != null");
								WordInfo wd = JsonUtility.FromJson<WordInfo>(rawInfo);
								Debug.Log("fetchWordInformation :: wd != null");
								wd.word = snapshot.Key;

								callbackWhenDone(wd);
								return;
							}
						}
						Debug.Log("fetchWordInformation :: something is null");
						callbackWhenDone(new WordInfo());
					}
				});	
		} else {
			callbackWhenDone(null);
		}
	}

	//update words in DICTIONARY (update queue)
	public void updateQueueForWordInDictionary(string word, int newQueue) {
		//refer to the word them update its queue field
		if (word.Length > 0) {
			Dictionary<string, object> queueUpdate = new Dictionary<string, object> ();
			queueUpdate["queue"] = newQueue;

			FirebaseDatabase.DefaultInstance
				.GetReference(DICTIONARY)
				.Child(word)
				.UpdateChildrenAsync(queueUpdate);
		}
	}

	/**************** functions for LEARNING_PROGRESS *****************/

	//update newwords for a user in LEARNING_PROGRESS
	private bool _updateNewWordsFieldInLearningProgress(string[] words, int date) {
		bool res = false;
		//refer to the word them update its queue field
		if (signedIn == true) {
			if (words != null && date > 0) {
				Dictionary<string, object> newwordsUpdate = new Dictionary<string, object> ();
				newwordsUpdate[PROGRESS_WORD_KEY] = String.Join(",", words);
				newwordsUpdate[PROGRESS_DATE_KEY] = date;
				newwordsUpdate[PROGRESS_COUNT_KEY] = words.Length;

				FirebaseDatabase.DefaultInstance
					.GetReference(LEARNING_PROGRESS)
					.Child(firebaseUser.UserId)
					.Child(PROGRESS_NEWWORDS_KEY)
					.UpdateChildrenAsync(newwordsUpdate);

				res = true;
			}
		} else {
			Debug.Log("No user is signed in");
			res = false;
		}

		return res;
	}

	//call this function when remove word from queue (when tapping on 4 buttons)
	public void updateNewWordsFieldInLearningProgressToday (string[] words) {

		if (words != null) {
			getCurrentDatetimeInNewWordsField(oldDate => {
				//when user still learning with old data (yesterday data..)
				//still update list words for old date
				//when user finish learning (with old date or new date), check this date to count streak
				if (oldDate > 0) {
					Dictionary<string, object> newwordsUpdate = new Dictionary<string, object> ();
					newwordsUpdate[PROGRESS_WORD_KEY] = String.Join(",", words);
					newwordsUpdate[PROGRESS_DATE_KEY] = oldDate;
					newwordsUpdate[PROGRESS_COUNT_KEY] = words.Length;

					FirebaseDatabase.DefaultInstance
						.GetReference(LEARNING_PROGRESS)
						.Child(firebaseUser.UserId)
						.Child(PROGRESS_NEWWORDS_KEY)
						.UpdateChildrenAsync(newwordsUpdate);
				}
			});
		}
	}

	public void updateInreviewFieldInLearningProgressToday (string[] words) {

		if (words != null) {
			getCurrentDatetimeInReviewField(oldDate => {
				//when user still learning with old data (yesterday data..)
				//still update list words for old date
				//when user finish learning (with old date or new date), check this date to count streak
				if (oldDate > 0) {
					Dictionary<string, object> newwordsUpdate = new Dictionary<string, object> ();
					newwordsUpdate[PROGRESS_WORD_KEY] = String.Join(",", words);
					newwordsUpdate[PROGRESS_DATE_KEY] = oldDate;
					newwordsUpdate[PROGRESS_COUNT_KEY] = words.Length;

					FirebaseDatabase.DefaultInstance
						.GetReference(LEARNING_PROGRESS)
						.Child(firebaseUser.UserId)
						.Child(PROGRESS_INREVIEW_KEY)
						.UpdateChildrenAsync(newwordsUpdate);
				}
			});
		}
	}

	//get new words list from /newwords field
	//after open learning screen, get newwords list from "learning_progress/newwords"
	public void getCurrentNewWordsList (System.Action<string[]> callbackWhenDone) {
		if (signedIn == true) {
			FirebaseDatabase.DefaultInstance
				.GetReference(LEARNING_PROGRESS)
				.Child(firebaseUser.UserId)
				.Child(PROGRESS_NEWWORDS_KEY)
				.GetValueAsync().ContinueWith(task => {
					if (task.IsFaulted) {
						// Handle the error...
						Debug.Log("getCurrentNewWordsList :: task :: error" + task.ToString());
						callbackWhenDone(new string[0]);

					} else if (task.IsCompleted) {
						DataSnapshot snapshot = task.Result;
						Debug.Log("snapshot :: " + snapshot.Key);

						string worsString = snapshot.Child(PROGRESS_WORD_KEY).GetRawJsonValue();
						Debug.Log("worsString :: " + worsString);

						if (worsString != null && worsString.Length > 0) {
							worsString = worsString.Trim('"');
							char[] splitters = { ',' };
							string[] res = worsString.Split(splitters, StringSplitOptions.RemoveEmptyEntries);

							Debug.Log("worsString :: count :: " + res.Length);
							Debug.Log("worsString :: res :: " + res);

							if (res != null && res.Length > 0) {
								callbackWhenDone(res);

							} else {
								callbackWhenDone(new string[0]);
							}

						} else {
							callbackWhenDone(new string[0]);
						}

					}
				});
			
		} else {
			callbackWhenDone(null);
		}
	}

	//get review list from /inreview field
	//after open learning screen, get inreview list from "learning_progress/inreview"
	public void getCurrentReviewList (System.Action<string[]> callbackWhenDone) {
		if (signedIn == true) {
			FirebaseDatabase.DefaultInstance
				.GetReference(LEARNING_PROGRESS)
				.Child(firebaseUser.UserId)
				.Child(PROGRESS_INREVIEW_KEY)
				.GetValueAsync().ContinueWith(task => {
					if (task.IsFaulted) {
						// Handle the error...
						Debug.Log("getCurrentReviewList :: task :: error" + task.ToString());
						callbackWhenDone(new string[0]);

					} else if (task.IsCompleted) {
						DataSnapshot snapshot = task.Result;
						Debug.Log("snapshot :: " + snapshot.Key);

						string worsString = snapshot.Child(PROGRESS_WORD_KEY).GetRawJsonValue();

						if (worsString != null && worsString.Length > 0) {
							worsString = worsString.Trim('"');
							char[] splitters = { ',' };

							string[] res = worsString.Split(splitters, StringSplitOptions.RemoveEmptyEntries);

							if ((res != null) && (res.Length > 0)) {
								callbackWhenDone(res);

							} else {
								callbackWhenDone(new string[0]);
							}

						} else {
							callbackWhenDone(new string[0]);
						}
					}
				});

		} else {
			callbackWhenDone(null);
		}
	}

	public void addAWordToAgainList (string word) {
		if (signedIn == true) {

			FirebaseDatabase.DefaultInstance
				.GetReference(LEARNING_PROGRESS)
				.Child(firebaseUser.UserId)
				.Child(PROGRESS_AGAIN_KEY)
				.GetValueAsync().ContinueWith(task => {
					if (task.IsFaulted) {
						// Handle the error...
						Debug.Log("addAWordToAgainList :: task :: error" + task.ToString());

					} else if (task.IsCompleted) {
						DataSnapshot snapshot = task.Result;
						Debug.Log("addAWordToAgainList :: " + snapshot.Key);

						string againList = snapshot.GetRawJsonValue().Trim('"');
						againList = String.Format("{0},{1}", againList, word);

						Dictionary<string, object> newAgainList = new Dictionary<string, object> ();
						newAgainList[PROGRESS_AGAIN_KEY] = againList;

						FirebaseDatabase.DefaultInstance
							.GetReference(LEARNING_PROGRESS)
							.Child(firebaseUser.UserId)
							.Child(PROGRESS_AGAIN_KEY)
							.UpdateChildrenAsync(newAgainList);
					}
				});
		} else {
			Debug.Log("No user is signed in");
		}
	}

	public void updateAgainList (string[] words) {
		if (signedIn == true) {

			if (words != null) {
				Dictionary<string, object> newAgainList = new Dictionary<string, object> ();
				newAgainList[PROGRESS_AGAIN_KEY] = String.Join(",", words);;

				FirebaseDatabase.DefaultInstance
					.GetReference(LEARNING_PROGRESS)
					.Child(firebaseUser.UserId)
					.Child(PROGRESS_AGAIN_KEY)
					.UpdateChildrenAsync(newAgainList);
			}
			
		} else {
			Debug.Log("No user is signed in");
		}

	}

	public void getAgainList (System.Action<string[]> callbackWhenDone) {
		if (signedIn == true) {

			FirebaseDatabase.DefaultInstance
				.GetReference(LEARNING_PROGRESS)
				.Child(firebaseUser.UserId)
				.Child(PROGRESS_AGAIN_KEY)
				.GetValueAsync().ContinueWith(task => {
					if (task.IsFaulted) {
						// Handle the error...
						Debug.Log("getAgainList :: task :: error" + task.ToString());
						callbackWhenDone(new string[0]);

					} else if (task.IsCompleted) {
						DataSnapshot snapshot = task.Result;
						Debug.Log("getAgainList :: " + snapshot.Key);
						string againList = snapshot.GetRawJsonValue();

						if (againList != null && againList.Length > 0) {
							againList = againList.Trim('"');
							char[] splitters = { ',' };
							string[] resList = againList.Split(splitters, StringSplitOptions.RemoveEmptyEntries);

							if (resList != null && resList.Length > 0) {
								Debug.Log("resList # null");
								callbackWhenDone(resList);

							} else {
								Debug.Log("resList == null");
								callbackWhenDone(new string[0]);
							}

						} else {
							Debug.Log("againList == null");
							callbackWhenDone(new string[0]);
						}
					}
				});
		} else {
			Debug.Log("No user is signed in");
			callbackWhenDone(null);
		}
	}

	//update inrview for a user in LEARNING_PROGRESS
	public bool updateInreviewFieldInLearningProgress(string[] words, int date) {
		bool res = false;
		//refer to the word them update its queue field
		if (signedIn == true) {
			if (words != null && date > 0) {
				Dictionary<string, object> newwordsUpdate = new Dictionary<string, object> ();
				newwordsUpdate[PROGRESS_WORD_KEY] = String.Join(",", words);
				newwordsUpdate[PROGRESS_DATE_KEY] = date;
				newwordsUpdate[PROGRESS_COUNT_KEY] = words.Length;

				FirebaseDatabase.DefaultInstance
					.GetReference(LEARNING_PROGRESS)
					.Child(firebaseUser.UserId)
					.Child(PROGRESS_INREVIEW_KEY)
					.UpdateChildrenAsync(newwordsUpdate);

				res = true;
			}
		} else {
			Debug.Log("No user is signed in");
			res = false;
		}

		return res;
	}

	//update Inprogress for a user in LEARNING_PROGRESS
	//call when need to update or create new
	public bool updateInprogressFieldInLearningProgress(string word, WordProgress progress) {
		bool res = false;
		//refer to the word them update its queue field
		if (signedIn == true) {
			if (word.Length > 0) {
				FirebaseDatabase.DefaultInstance
					.GetReference(LEARNING_PROGRESS)
					.Child(firebaseUser.UserId)
					.Child(PROGRESS_INPROGRESS_KEY)
					.Child(word)
					.UpdateChildrenAsync(convertJsonStringToObject(JsonUtility.ToJson(progress)));

				res = true;
			}	
		} else {
			Debug.Log("No user is signed in");
			res = false;
		}

		return res;
	}

	//return wordprogress of a word
	public void fetchWordProgress(string word, System.Action<WordProgress> callbackWhenDone) {
		Debug.Log("fetchWordProgress");
		if (signedIn == true) {
			FirebaseDatabase.DefaultInstance
				.GetReference(LEARNING_PROGRESS)
				.Child(firebaseUser.UserId)
				.Child(PROGRESS_INPROGRESS_KEY)
				.Child(word)
				.GetValueAsync().ContinueWith(task => {
					if (task.IsFaulted) {
						// Handle the error...
						Debug.Log("fetchWordProgress :: task :: error" + task.ToString());
						callbackWhenDone(new WordProgress());

					} else if (task.IsCompleted) {
						DataSnapshot snapshot = task.Result;
						Debug.Log("snapshot :: " + snapshot.Key);

						if (snapshot != null) {
							string rawInfo = snapshot.GetRawJsonValue();

							if (rawInfo != null) {
								WordProgress wordProgress = JsonUtility.FromJson<WordProgress>(rawInfo);

								if (wordProgress != null) {
									callbackWhenDone(wordProgress);

									return;
								}
							}
						}

						Debug.Log("fetchWordProgress :: create new WordProgress");	//do not create new progress here, create when tap on the 4 buttons
						//create new progress
						WordProgress newWordProgress = new WordProgress();
//						updateInprogressFieldInLearningProgress(word, newWordProgress);

						callbackWhenDone(newWordProgress);
					}
				});
		} else {
			callbackWhenDone(null);
		}
	}

	public void fetchWordUserLearningInfo (string word, System.Action<UserLearning> callbackWhenDone) {
		Debug.Log("fetchWordUserLearningInfo :: " +word);
		UserLearning userLearning = new UserLearning();
		try {
			fetchWordInformation (word, wordInfo => {
				if (wordInfo.word != null) {
					userLearning.wordInfo = wordInfo;

					fetchWordProgress(word, wordProgress => {
						if (wordProgress.e_fact > 0) {
							Debug.Log("fetchWordUserLearningInfo :: successfully");
							userLearning.wordProgress = wordProgress;

							callbackWhenDone(userLearning);

						} else {
							Debug.Log("fetchWordUserLearningInfo :: fetchWordProgress is null");
							callbackWhenDone(new UserLearning());
						}
					});

				} else {
					Debug.Log("fetchWordUserLearningInfo :: fetchWordInformation is null");
					callbackWhenDone(new UserLearning());
				}
			});
		} catch (Exception e) {
			Debug.Log("fetchWordUserLearningInfo :: exception :: " + e.ToString());
			callbackWhenDone(new UserLearning());
		}
	}

	//call this function when starting app
	//insert this list to inreview field
	//when user start app, check current inreview filed, if it is obsolete, get new list (by this function) and update inreview field.
	//(the same to newwords field.)
	//callbackWhenDone -> return number of words
	//after open learning screen, get inreview list from "learning_progress/inreview"
	public void prepareInreviewList(int limit, System.Action<int> callbackWhenDone) {
		Debug.Log("FirebaseHelper :: prepareInreviewList");
		if (limit > 20) {	//due to limited quota
			limit = 20;
		}

		_prepareInreviewList(limit, words => {
			int date = DateTimeHelper.getBeginOfDayInSec();
			updateInreviewFieldInLearningProgress(words, date);
			callbackWhenDone(words.Length);
		});
	}

	private void _prepareInreviewList (int limit, System.Action<string[]> callbackWhenDone) {
		Debug.Log("FirebaseHelper :: _prepareInreviewList");
		if (limit > 20) {	//due to limited quota
			limit = 20;
		}
		//where queue = %d AND due <= %f ORDER BY level LIMIT %ld", QUEUE_REVIEW, [self getEndOfDayInSec], (long)TOTAL_WORDS_A_DAY_MAX
		FirebaseDatabase.DefaultInstance
			.GetReference(LEARNING_PROGRESS)
			.Child(firebaseUser.UserId)
			.Child(PROGRESS_INPROGRESS_KEY)
			.OrderByChild("due")
			.EndAt(DateTimeHelper.getEndOfDayInSec())
			.LimitToFirst(limit)
			.GetValueAsync().ContinueWith(task => {
				if (task.IsFaulted) {
					// Handle the error...
					Debug.Log("getListWords :: task :: error" + task.ToString());
					callbackWhenDone(new string[0]);

				} else if (task.IsCompleted) {
					DataSnapshot snapshot = task.Result;
					List<string> wordsList = new List<string>();
					// Do something with snapshot...
					Debug.Log("snapshot.Children :: count :: " + snapshot.ChildrenCount);
					foreach (var item in snapshot.Children) {
						Debug.Log("snapshot.Children :: " + item.Key);

						if (Int32.Parse(item.Child("queue").GetRawJsonValue()) == CommonDefine.QUEUE_REVIEW) {
							wordsList.Add(item.Key);
						}
					}

					callbackWhenDone(wordsList.ToArray());
				}
			});
	}

	//call when user tap on one of 4 buttons (again, easy, normal, hard)
	public void updateWordProgressWithEaseOption (string word, WordProgress wordProgress, int easeOption) {
		Algorithm.getInstance().updateWordProgressWithEaseOption(ref wordProgress, easeOption);

		updateInprogressFieldInLearningProgress(word, wordProgress);
	}

	//get datetime in /newwords
	public void getCurrentDatetimeInNewWordsField (System.Action<int> callbackWhenDone) {
		Debug.Log("FirebaseHelper :: getCurrentDatetimeInNewWordsField");
		if (signedIn == true) {
			_getCurrentDatetimeOfLearningProgress(PROGRESS_NEWWORDS_KEY, date => {
				Debug.Log("FirebaseHelper :: getCurrentDatetimeInNewWordsField :: date :: " +date.ToString());
				callbackWhenDone(date);
			});

		} else {
			callbackWhenDone(0);
		}
	}

	//get datetime in /inreview
	public void getCurrentDatetimeInReviewField (System.Action<int> callbackWhenDone) {
		Debug.Log("FirebaseHelper :: getCurrentDatetimeInReviewField");
		if (signedIn == true) {
			_getCurrentDatetimeOfLearningProgress(PROGRESS_INREVIEW_KEY, date => {
				Debug.Log("FirebaseHelper :: getCurrentDatetimeInReviewField :: date :: " +date.ToString());
				callbackWhenDone(date);
			});

		} else {
			callbackWhenDone(0);
		}
	}

	//get datetime in /newwords or /inreview (to check whether data is obsolete or not)
	//fieldPath = "newwords" or "inreview"
	private void _getCurrentDatetimeOfLearningProgress (string fieldPath, System.Action<int> callbackWhenDone) {
		Debug.Log("_getCurrentDatetimeOfLearningProgress");
		if (signedIn == true) {
			FirebaseDatabase.DefaultInstance
				.GetReference(LEARNING_PROGRESS)
				.Child(firebaseUser.UserId)
				.Child(fieldPath)
				.GetValueAsync().ContinueWith(task => {
					if (task.IsFaulted) {
						// Handle the error...
						Debug.Log("_getCurrentDatetimeOfLearningProgress :: task :: error" + task.ToString());
						callbackWhenDone(0);

					} else if (task.IsCompleted) {
						DataSnapshot snapshot = task.Result;
						Debug.Log("_getCurrentDatetimeOfLearningProgress :: snapshot :: " + snapshot.Key);

						string strDate = snapshot.Child(PROGRESS_DATE_KEY).GetRawJsonValue();
						Debug.Log("_getCurrentDatetimeOfLearningProgress :: PROGRESS_DATE_KEY :: " + strDate);

						if (strDate != null && strDate.Length > 0) {
							int date = Int32.Parse(snapshot.Child(PROGRESS_DATE_KEY).GetRawJsonValue());

							Debug.Log("_getCurrentDatetimeOfLearningProgress :: date :: " + date);

							callbackWhenDone(date);

						} else {
							callbackWhenDone(0);
						}

					} else {

						callbackWhenDone(0);
						Debug.Log("_getCurrentDatetimeOfLearningProgress :: failed");
					}
				});

		} else {
			callbackWhenDone(0);
		}
	}

	//get current word index that is corresponding to level
	//my_level
	//current_word_index_lvxxx
	//reserve in TemporarilyStatus
	//call this functions before prepare newwords list and review list
	public void getCurrentLearningWordIndex (System.Action<bool> callbackWhenDone) {
		
		if (signedIn == true) {
			FirebaseDatabase.DefaultInstance
				.GetReference(LEARNING_PROGRESS)
				.Child(firebaseUser.UserId)
				.Child(PROGRESS_CURRENT_STS_KEY)
				.GetValueAsync().ContinueWith(task => {
					if (task.IsFaulted) {
						// Handle the error...
						Debug.Log("getCurrentLearningWordIndex :: task :: error" + task.ToString());
						callbackWhenDone(false);

					} else if (task.IsCompleted) {
						DataSnapshot snapshot = task.Result;
						Debug.Log("snapshot :: " + snapshot.Key);
//						TemporarilyStatus.getInstance().current_level = Int32.Parse(snapshot.Child("current_level").GetRawJsonValue());

						string current_word_ind_key = "current_word_index_lv" + TemporarilyStatus.getInstance().my_level;
						TemporarilyStatus.getInstance().current_word_index = Int32.Parse(snapshot.Child(current_word_ind_key).GetRawJsonValue());

						callbackWhenDone(true);
					}
				});
		} else {
			callbackWhenDone(false);
		}
	}

	//update current learning word id
	public bool updateCurrentLearningWordIndex() {
		bool res = false;
		//refer to the word then update its queue field
		if (signedIn == true) {
			Dictionary<string, object> newCurStatus = new Dictionary<string, object> ();
			//current_word_index_lvxxx
//			newCurStatus["current_level"] = TemporarilyStatus.getInstance().current_level;

			string current_word_ind_key = "current_word_index_lv" + TemporarilyStatus.getInstance().my_level;
			newCurStatus[current_word_ind_key] = TemporarilyStatus.getInstance().current_word_index;

			FirebaseDatabase.DefaultInstance
				.GetReference(LEARNING_PROGRESS)
				.Child(firebaseUser.UserId)
				.Child(PROGRESS_CURRENT_STS_KEY)
				.UpdateChildrenAsync(convertJsonStringToObject(JsonUtility.ToJson(newCurStatus)));

			res = true;

		} else {
			Debug.Log("No user is signed in");
			res = false;
		}

		return res;
	}

	//increase current word index
	public void increaseCurrentWordIndex(int level) {
		if (signedIn == true) {
			string current_word_ind_key = "current_word_index_lv" + level;
				
			FirebaseDatabase.DefaultInstance
				.GetReference(LEARNING_PROGRESS)
				.Child(firebaseUser.UserId)
				.Child(PROGRESS_CURRENT_STS_KEY)
				.Child(current_word_ind_key)
				.GetValueAsync().ContinueWith(task => {
					if (task.IsFaulted) {
						// Handle the error...
						Debug.Log("increaseCurrentWordIndex :: task :: error" + task.ToString());

					} else if (task.IsCompleted) {
						DataSnapshot snapshot = task.Result;
						Debug.Log("snapshot :: " + snapshot.Key);
						int ind = Int32.Parse(snapshot.GetRawJsonValue());
						ind = ind + 1;

						Dictionary<string, object> newCurInd = new Dictionary<string, object> ();
						//current_word_index_lvxxx
						newCurInd[current_word_ind_key] = ind;

						FirebaseDatabase.DefaultInstance
							.GetReference(LEARNING_PROGRESS)
							.Child(firebaseUser.UserId)
							.Child(PROGRESS_CURRENT_STS_KEY)
							.Child(current_word_ind_key)
							.UpdateChildrenAsync(newCurInd);
					}
				});
		} else {
			Debug.Log("No user is signed in");
		}
	}

	/**************** functions for LEVELS *****************/

	//call this function when starting app
	//insert this list to newwords field
	//when user start app, check current newwords filed, if it is obsolete, get new list (by this function) and update newwords field.
	//update current word index
	//callbackWhenDone -> return number of words
	//(the same to inreview field.)
	//after open learning screen, get newwords list from "learning_progress/newwords"
	public void prepareListNewWordsToLearn (int limit, System.Action<int> callbackWhenDone) {
		Debug.Log("FirebaseHelper :: prepareListNewWordsToLearn");
		if (limit > 20) {	//due to limited quota
			limit = 20;
		}

		_pickNewWordsFromLevelsAddToQueue(limit, words => {
			int date = DateTimeHelper.getBeginOfDayInSec();
			_updateNewWordsFieldInLearningProgress(words, date);

			callbackWhenDone(words.Length);
		});
	}

	private void _pickNewWordsFromLevelsAddToQueue (int limit, System.Action<string[]> callbackWhenDone) {
		Debug.Log("FirebaseHelper :: _pickNewWordsFromLevelsAddToQueue");
		if (limit > 20) {	//due to limited quota
			limit = 20;
		}

		string level = "level" + TemporarilyStatus.getInstance().my_level;
		Debug.Log("_pickNewWordsFromLevelsAddToQueue :: level :: " + level);

		FirebaseDatabase.DefaultInstance
			.GetReference(LEVELS)
			.Child(level)
			.OrderByKey()
			.StartAt(TemporarilyStatus.getInstance().current_word_index.ToString())
			.LimitToFirst(limit)
			.GetValueAsync().ContinueWith(task => {
				if (task.IsFaulted) {
					// Handle the error...
					Debug.Log("_pickNewWordsFromLevelsAddToQueue :: task :: error" + task.ToString());
					callbackWhenDone(new string[0]);

				} else if (task.IsCompleted) {
					DataSnapshot snapshot = task.Result;
					List<string> wordsList = new List<string>();
					// Do something with snapshot...
					Debug.Log("snapshot.Children :: count :: " + snapshot.ChildrenCount);
					foreach (var item in snapshot.Children) {
						Debug.Log("snapshot.Children :: " + item.GetRawJsonValue().Trim('"'));

						wordsList.Add(item.GetRawJsonValue().Trim('"'));
					}

					//update counter
					TemporarilyStatus.getInstance().current_word_index = TemporarilyStatus.getInstance().current_word_index + wordsList.Count;
					updateCurrentLearningWordIndex();

					if (wordsList.Count < limit) {
						TemporarilyStatus.getInstance().my_level++;
						updateLevelSetting(TemporarilyStatus.getInstance().my_level);
					}

					callbackWhenDone(wordsList.ToArray());
				}
			});
	}

	/**************** functions for SETTINGS *****************/
	//reserve in TemporarilyStatus
	//auto_play_sound
	//my_level
	//new_card_per_day
	//total_card_per_day
	//notification
	//time_to_show_answer
	public void getUserSettings (System.Action<bool> callbackWhenDone) {
		Debug.Log("FirebaseHelper :: getUserSettings :: " +firebaseUser.UserId);
		if (signedIn == true) {
			FirebaseDatabase.DefaultInstance
				.GetReference(SETTINGS)
				.Child(firebaseUser.UserId)
				.GetValueAsync().ContinueWith(task => {
					if (task.IsFaulted) {
						// Handle the error...
						Debug.Log("getUserSettings :: task :: error" + task.ToString());
						callbackWhenDone(false);

					} else if (task.IsCompleted) {
						Debug.Log("FirebaseHelper :: getUserSettings :: success");

						DataSnapshot snapshot = task.Result;
						Debug.Log("snapshot :: getUserSettings :: " + snapshot.Key);

						if (snapshot.GetRawJsonValue() != null) {
							Debug.Log("snapshot.Child(SETTINGS_MY_LEVEL_KEY) :: " + snapshot.Child(SETTINGS_MY_LEVEL_KEY).GetRawJsonValue());
							Debug.Log("snapshot.Child(SETTINGS_TOTAL_CARD_KEY) :: " + snapshot.Child(SETTINGS_TOTAL_CARD_KEY).GetRawJsonValue());

							TemporarilyStatus.getInstance().auto_play_sound 	= Int32.Parse(snapshot.Child(SETTINGS_AUTOPLAY_KEY).GetRawJsonValue().Trim('"'));
							TemporarilyStatus.getInstance().my_level  			= Int32.Parse(snapshot.Child(SETTINGS_MY_LEVEL_KEY).GetRawJsonValue().Trim('"'));
							TemporarilyStatus.getInstance().new_card_a_day 		= Int32.Parse(snapshot.Child(SETTINGS_NEW_CARD_KEY).GetRawJsonValue().Trim('"'));
							TemporarilyStatus.getInstance().total_card_a_day 	= Int32.Parse(snapshot.Child(SETTINGS_TOTAL_CARD_KEY).GetRawJsonValue().Trim('"'));
							TemporarilyStatus.getInstance().time_to_show_answer	= Int32.Parse(snapshot.Child(SETTINGS_TIME_SHOW_ANSWER_KEY).GetRawJsonValue().Trim('"'));
							TemporarilyStatus.getInstance().notification	 	= Int32.Parse(snapshot.Child(SETTINGS_NOTIFICATION_KEY).GetRawJsonValue().Trim('"'));

							callbackWhenDone(true);

						} else {
							Debug.Log("FirebaseHelper :: getUserSettings :: no settings");
							callbackWhenDone(false);
						}



					} else {
						Debug.Log("FirebaseHelper :: getUserSettings :: error");
						callbackWhenDone(false);
					}
				});
			
		} else {
			callbackWhenDone(false);
		}
	}

	public void configDefaultSettings() {
		_updateUserSettings(SETTINGS_MY_LEVEL_KEY, CommonDefine.SETTINGS_DEFAULT_LEVEL);
		_updateUserSettings(SETTINGS_AUTOPLAY_KEY, CommonDefine.SETTINGS_AUTO_PLAY);
		_updateUserSettings(SETTINGS_NEW_CARD_KEY, CommonDefine.SETTINGS_NEWCARD_A_DAY);
		_updateUserSettings(SETTINGS_TOTAL_CARD_KEY, CommonDefine.SETTINGS_TOTALCARD_A_DAY);
		_updateUserSettings(SETTINGS_TIME_SHOW_ANSWER_KEY, CommonDefine.SETTINGS_TIME_SHOW_ANSWER);
		_updateUserSettings(SETTINGS_NOTIFICATION_KEY, CommonDefine.SETTINGS_NOTIFICATION);

		TemporarilyStatus.getInstance().auto_play_sound 	= CommonDefine.SETTINGS_AUTO_PLAY;
		TemporarilyStatus.getInstance().my_level  			= CommonDefine.SETTINGS_DEFAULT_LEVEL;
		TemporarilyStatus.getInstance().new_card_a_day 		= CommonDefine.SETTINGS_NEWCARD_A_DAY;
		TemporarilyStatus.getInstance().total_card_a_day 	= CommonDefine.SETTINGS_TOTALCARD_A_DAY;
		TemporarilyStatus.getInstance().time_to_show_answer	= CommonDefine.SETTINGS_TIME_SHOW_ANSWER;
		TemporarilyStatus.getInstance().notification	 	= CommonDefine.SETTINGS_NOTIFICATION;
	}

	public void updateLevelSetting(int value) {
		_updateUserSettings(SETTINGS_MY_LEVEL_KEY, value);
	}

	private void _updateUserSettings(string settingKey, int value) {
		if (signedIn == true) {
			Dictionary<string, object> newValue = new Dictionary<string, object> ();
			newValue [settingKey] = value;

			FirebaseDatabase.DefaultInstance
				.GetReference (SETTINGS)
				.Child (firebaseUser.UserId)
				.UpdateChildrenAsync (newValue);
			
		} else {
			Debug.Log("No user is signed in");
		}
	}


	private Dictionary<string, object> convertJsonStringToObject(string jsonString) {
		Debug.Log("convertJsonStringToObject :: " + jsonString);

		Dictionary<string, object> dictionary = new Dictionary<string, object>();
		string[] items = jsonString.TrimStart('{').TrimEnd('}').Split(',');

		foreach (string item in items) {
			string[] keyValue = item.Split(':');
			dictionary.Add(keyValue[0].Trim('"'), Int32.Parse(keyValue[1]));
		}

		return dictionary;
	}

	//add user to learning_progress
	//always add users to learning_progress after create user account
}
#endif