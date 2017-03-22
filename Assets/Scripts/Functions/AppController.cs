using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Auth;
using Facebook.Unity;

public class AppController : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
//		FirebaseHelper.getInstance().getListWords(2, wordsList => {
//			foreach (WordInfo word in wordsList) {
//				Debug.Log("getListWords :: word :: " + word.word);
//				Debug.Log("getListWords :: meaning :: " + word.common);
//			}
//		});
//
//		FirebaseHelper.getInstance().getWordInformation("announce", word => {
//			Debug.Log("getWordInformation :: word :: " + word.word);
//			Debug.Log("getWordInformation :: meaning :: " + word.common.meaning);
//		});

//		FirebaseHelper.getInstance().updateQueueForWordInDictionary("announce", CommonDefine.QUEUE_NEW_WORD);

//		string[] test = new string[]{"announce", "suffer"};
//
//		FirebaseHelper.getInstance().updateInreviewFieldInLearningProgress(test, 12345678);

//		WordProgress test = new WordProgress();
//
//		FirebaseHelper.getInstance().fetchWordUserLearningProgress("announce", wordProgress => {
//			Debug.Log("fetchWordUserLearningProgress :: e_fact :: " + wordProgress.e_fact);
//		});

//		FirebaseHelper.getInstance().loginAsAnnonymousUser(userInfo => {
//			Debug.Log("loginAsAnnonymousUser :: name :: " + userInfo.username);
//			Debug.Log("loginAsAnnonymousUser :: id :: " + userInfo.userID);
//		});


//		FirebaseHelper.getInstance().checkCurrentUser();
		//test get and insert new words
//		FirebaseHelper.getInstance().prepareListNewWordsToLearn(2, count => {
//			Debug.Log("prepareListNewWordsToLearn :: count :: " + count);
//		});

		//get datetime in /newwords field, check if it is obsolete, prepare new list
		FirebaseHelper.getInstance().getCurrentDatetimeInNewWordsField(date => {
			Debug.Log("getCurrentDatetimeInNewWordsField :: date :: " + date.ToString());

			int curDate = DateTimeHelper.getBeginOfDayInSec();

			if (date != curDate) {
				//prepare new list
				FirebaseHelper.getInstance().prepareListNewWordsToLearn(2, count => {
					Debug.Log("prepareListNewWordsToLearn :: count :: " + count);
				});
			}
		});
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
