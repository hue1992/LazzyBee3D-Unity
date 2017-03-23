using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Firebase.Database;
using System.Collections.Generic;

public class StudyHandler : MonoBehaviour
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
	private int currentWordInd = 0;
	private string currentWord = "";
	private UserLearning wordUserLearning = null;

    public Button btnShowAnswer;
    public GameObject ButtonAnswer;
    public Button btnAgain;
    public Button btnHard;
    public Button btnNormal;
    public Button btnEasy;

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

    }

    void AgainClick()
    {
		//update word with again option

        endSessionStudy();
    }
    void HardClick()
    {
		//update word with hard option
        endSessionStudy();
    }
    void NormalClick()
    {
		//update word with normal option
        endSessionStudy();
    }
    void EasyClick()
    {
		//update word with easy option
        endSessionStudy();
    }

    void endSessionStudy()
    {
        btnShowAnswer.interactable = true;
        ButtonAnswer.SetActive(false);

		if (curListName == CURRENT_LIST_NAME.LIST_REVIEW) {
			
			if (reviewWords.Count == 0) {
				curListName = CURRENT_LIST_NAME.LIST_AGAIN;
				currentWordInd = 0;
			}

		}

		if (curListName == CURRENT_LIST_NAME.LIST_AGAIN) {
			if (againWords.Count == 0) {
				curListName = CURRENT_LIST_NAME.LIST_NEW;
				currentWordInd = 0;
			}

		}

		if (curListName == CURRENT_LIST_NAME.LIST_NEW) {
			if (newWords.Count == 0) {
				currentWordInd = 0;
				//finish daily target, record daily target here.

				//close study screen
				SceneManager.UnloadSceneAsync("Study");
			}
		} 
    }


    private void getUserData() {
		//review list, again list, new word list had been prepared in home screen,
		//so do not need to count number of each list
		//just load them
		FirebaseHelper.getInstance().getCurrentReviewList(words => {
			reviewWords.AddRange(words);
		});

		FirebaseHelper.getInstance().getAgainList(words => {
			againWords.AddRange(words);
		});

		FirebaseHelper.getInstance().getCurrentNewWordsList(words => {
			newWords.AddRange(words);
		});

		currentWordInd = 0;
		curListName  = CURRENT_LIST_NAME.LIST_REVIEW; //reset, review is default
		bool found = false;

		if (reviewWords.Count > 0) {
			curListName  = CURRENT_LIST_NAME.LIST_REVIEW;
			currentWord = reviewWords[currentWordInd];
			found = true;

		} else if (againWords.Count > 0) {
			curListName  = CURRENT_LIST_NAME.LIST_AGAIN;
			currentWord = againWords[currentWordInd];
			found = true;

		} else if (newWords.Count > 0) {
			curListName  = CURRENT_LIST_NAME.LIST_NEW;
			currentWord = newWords[currentWordInd];
			found = true;
		}

		if (found == false) {
			//show alert: no word to learn

			//close screen
			SceneManager.UnloadSceneAsync("Study");

		} else {
			//fetch word info and learning progress
			fetchWordUserLearningInfo();
		}
    }

	private void fetchWordUserLearningInfo() {
		if (currentWord.Length > 0) {
			FirebaseHelper.getInstance().fetchWordUserLearningInfo(currentWord, userLearning => {
				if (userLearning != null) {
					wordUserLearning = userLearning;

				} else {
					wordUserLearning = null;
				}
			});
		}
	}
}
