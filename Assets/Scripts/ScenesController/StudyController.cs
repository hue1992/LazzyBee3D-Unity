using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Firebase.Database;
using System.Collections.Generic;
using System.Linq;

public class StudyController : MonoBehaviour
{
	enum CURRENT_LIST_NAME {
		LIST_REVIEW,
		LIST_AGAIN,
		LIST_NEW
	};

	private List<string> newWords = new List<string>();
	private List<string> againWords = new List<string>();
	private List<string> reviewWords = new List<string>();

	private CURRENT_LIST_NAME curListName = CURRENT_LIST_NAME.LIST_REVIEW;
	private int currentWordInd = 0;		//always keep it = 0
	private string currentWord = "";
	private UserLearning wordUserLearning = null;

    public Button btnShowAnswer;
    public GameObject ButtonAnswer;
    public Button btnAgain;
    public Button btnHard;
    public Button btnNormal;
    public Button btnEasy;
    public string htmlText;

    void Start() {
        handlerButton();
        getUserData();
    }

	void Update() {
		//update title screen
		if (curListName == CURRENT_LIST_NAME.LIST_REVIEW) {
			

		} else if (curListName == CURRENT_LIST_NAME.LIST_AGAIN) {
			

		} else if (curListName == CURRENT_LIST_NAME.LIST_NEW) {
			
		} 
	}

 
    private void handlerButton()
    {
        btnShowAnswer.onClick.AddListener(ShowAnswer);
        btnAgain.onClick.AddListener(AgainClick);
        btnHard.onClick.AddListener(HardClick);
        btnNormal.onClick.AddListener(NormalClick);
        btnEasy.onClick.AddListener(EasyClick);
    }

    void ShowAnswer()
    {
        btnShowAnswer.interactable = false;
        ButtonAnswer.SetActive(true);

		loadAnswer();
    }

    void AgainClick()
    {
		//update word with again option
		FirebaseHelper.getInstance()
			.updateWordProgressWithEaseOption(currentWord,
											wordUserLearning.wordProgress,
											CommonDefine.OPTION_AGAIN);

		endSessionStudy();
    }
    void HardClick()
    {
		//update word with hard option
		FirebaseHelper.getInstance()
			.updateWordProgressWithEaseOption(currentWord,
				wordUserLearning.wordProgress,
				CommonDefine.OPTION_HARD);
		
        endSessionStudy();
    }
    void NormalClick()
    {
		//update word with normal option
		FirebaseHelper.getInstance()
			.updateWordProgressWithEaseOption(currentWord,
				wordUserLearning.wordProgress,
				CommonDefine.OPTION_GOOD);
		
        endSessionStudy();
    }
    void EasyClick()
    {
		//update word with easy option
		FirebaseHelper.getInstance()
			.updateWordProgressWithEaseOption(currentWord,
				wordUserLearning.wordProgress,
				CommonDefine.OPTION_EASY);
		
        endSessionStudy();
    }

    void endSessionStudy()
    {
		Debug.Log("endSessionStudy");
        btnShowAnswer.interactable = true;
        ButtonAnswer.SetActive(false);
		bool needRemoveWord = true;

		currentWordInd = 0;
		currentWord = "";
		wordUserLearning = null;

		if (curListName == CURRENT_LIST_NAME.LIST_REVIEW) {
			if (needRemoveWord == true) {
				reviewWords.RemoveAt(currentWordInd);
				needRemoveWord = false;

				FirebaseHelper.getInstance().updateInreviewFieldInLearningProgressToday(reviewWords.ToArray());
			}

			if (reviewWords.Count == 0) {
				curListName = CURRENT_LIST_NAME.LIST_AGAIN;
				currentWordInd = 0;

			} else {
//				currentWordInd++;	//always get item at 0 because we remove words after learn them
				currentWord = reviewWords.ElementAt(currentWordInd);
			}

		}

		if (curListName == CURRENT_LIST_NAME.LIST_AGAIN) {
			if (needRemoveWord == true) {
				againWords.RemoveAt(currentWordInd);
				needRemoveWord = false;

				FirebaseHelper.getInstance().updateAgainList(againWords.ToArray());
			}

			if (againWords.Count == 0) {
				curListName = CURRENT_LIST_NAME.LIST_NEW;
				currentWordInd = 0;

			} else {
//				currentWordInd++;
				currentWord = againWords.ElementAt(currentWordInd);
			}

		}

		if (curListName == CURRENT_LIST_NAME.LIST_NEW) {
			if (needRemoveWord == true) {
				newWords.RemoveAt(currentWordInd);
				needRemoveWord = false;

				FirebaseHelper.getInstance().updateNewWordsFieldInLearningProgressToday(newWords.ToArray());
			}

			if (newWords.Count == 0) {
				currentWordInd = 0;
				//finish daily target, record daily target here.

				//close study screen
				SceneManager.UnloadSceneAsync("Study");
			} else {
//				currentWordInd++;
				currentWord = newWords.ElementAt(currentWordInd);
			}
		}

		if (currentWord.Length > 0) {
			fetchWordUserLearningInfo();
		}
    }


    private void getUserData() {
		//review list, again list, new word list had been prepared in home screen,
		//so do not need to count number of each list
		//just load them
		currentWordInd = 0;
		currentWord = "";
		wordUserLearning = null;

		reviewWords.Clear();
		againWords.Clear();
		newWords.Clear();

		curListName  = CURRENT_LIST_NAME.LIST_REVIEW; //reset, review is default
		bool found = false;

		FirebaseHelper.getInstance().getCurrentReviewList(rwords => {
			reviewWords.AddRange(rwords);


			FirebaseHelper.getInstance().getAgainList(agwords => {
				againWords.AddRange(agwords);


				FirebaseHelper.getInstance().getCurrentNewWordsList(nwords => {
					newWords.AddRange(nwords);

					Debug.Log("reviewWords :: count :: " + reviewWords.Count);
					Debug.Log("againWords :: count :: " + againWords.Count);
					Debug.Log("newWords :: count :: " + newWords.Count);

					if (reviewWords.Count > 0) {
						Debug.Log("reviewWords :: " + reviewWords);
						curListName  = CURRENT_LIST_NAME.LIST_REVIEW;
						currentWord = reviewWords.ElementAt(currentWordInd);
						found = true;

					} else if (againWords.Count > 0) {
						Debug.Log("againWords :: " + againWords);
						curListName  = CURRENT_LIST_NAME.LIST_AGAIN;
						currentWord = againWords.ElementAt(currentWordInd);
						found = true;

					} else if (newWords.Count > 0) {
						Debug.Log("newWords :: " + newWords);
						curListName  = CURRENT_LIST_NAME.LIST_NEW;
						currentWord = newWords.ElementAt(currentWordInd);
						found = true;
					}

					if (found == false) {
						//show alert: no word to learn

						//close screen
						Debug.Log("UnloadSceneAsync(\"Study\")");
						SceneManager.UnloadSceneAsync("Study");

					} else {
						//fetch word info and learning progress
						fetchWordUserLearningInfo();
					}

				});
			});
		});
    }

	private void fetchWordUserLearningInfo() {
		Debug.Log("fetchWordUserLearningInfo :: " +currentWord);

		if (currentWord.Length > 0) {
			FirebaseHelper.getInstance().fetchWordUserLearningInfo(currentWord, userLearning => {
				if (userLearning.wordInfo != null) {
					Debug.Log("fetchWordUserLearningInfo :: display HTML");
					wordUserLearning = userLearning;

					Debug.Log("wordUserLearning :: wordInfo.word :: " +wordUserLearning.wordInfo.word);

					loadQuestion();

				} else {
					wordUserLearning = null;
					endSessionStudy();
				}
			});
		} else {
			endSessionStudy();
		}
	}

	private string[] removeBlankItems (string[] arr) {
		List<string> resList = new List<string>();

		foreach (string item in arr) {
			Debug.Log("removeBlankItems :: " +item);
			if (item.Length > 0) {
				resList.Add(item);
			}
		}

		return resList.ToArray();
	}

	private void loadQuestion () {
		htmlText = HTMLHelper.createHTMLForQuestion(wordUserLearning.wordInfo);

		Debug.Log("fetchWordUserLearningInfo :: htmlText :: " +htmlText);
		LoadFromText();
	}

	private void loadAnswer () {
		htmlText = HTMLHelper.createHTMLForAnswer(wordUserLearning.wordInfo);

		Debug.Log("fetchWordUserLearningInfo :: htmlText :: " +htmlText);
		LoadFromText();
	}

    UniWebView CreateWebView() {
        var webViewGameObject = GameObject.Find("WebView");
		UniWebView webView = null;

        if (webViewGameObject == null) {
            webViewGameObject = new GameObject("WebView");

			webView = webViewGameObject.AddComponent<UniWebView>();

		} else {
			webView = webViewGameObject.GetComponent<UniWebView>();

			if (webView == null) {
				webView = webViewGameObject.AddComponent<UniWebView>();
			}
		}

        webView.toolBarShow = false;
//		webView.SetShowSpinnerWhenLoading(false);	//un-comment after have loading indicator
        return webView;
    }

    public void LoadFromText()
    {
		UniWebView webView = CreateWebView();

        webView.LoadHTMLString(htmlText, null);
        webView.backButtonEnable = false;      
        webView.insets.top = 200;
        webView.insets.bottom = 180;
        webView.Show();
    }

}
