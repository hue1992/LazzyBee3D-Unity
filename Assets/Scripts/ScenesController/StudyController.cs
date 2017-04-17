using System.Timers;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Firebase.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System;

public class StudyController : MonoBehaviour, IScenesController
{
    enum CURRENT_LIST_NAME
    {
        LIST_REVIEW,
        LIST_AGAIN,
        LIST_NEW
    };
    public GameObject loadingIndicator;
	public GameObject webViewContainer;
	public GameObject dialogMsg;
	private DialogMessageController dialogMessageController;

	public GameObject noInternetPanel;

    private List<string> newWords = new List<string>();
    private List<string> againWords = new List<string>();
    private List<string> reviewWords = new List<string>();

    private List<string> subAgainWords = new List<string>();    //contain again list in current session, if user close study screen, will reload again list to againWords array

    private CURRENT_LIST_NAME curListName = CURRENT_LIST_NAME.LIST_REVIEW;
    private int currentWordInd = 0;     //always keep it = 0
    private string currentWord = "";
    private UserLearning wordUserLearning = null;

    public Button btnShowAnswer;
    public GameObject ButtonAnswer;
    public Button btnAgain;
    public Button btnHard;
    public Button btnNormal;
    public Button btnEasy;

    public Text titleText;
    public Text newText;
    public Text againText;
    public Text reviewText;

    private string htmlText;

	private const string STR_REVIEW	= "Review";
	private const string STR_AGAIN	= "Again";
	private const string STR_NEW	= "New";

	private const string STR_NO_CONNECTION_ALERT_CONTENT	= "No internet connection!\nPlease double check wifi/3G connection.";

	Timer timer = new Timer();

    void Start()
    {
        handlerButton();
        getUserData();

		//set timer to check connection
		timer.Interval = 5000;  //millisecond
		timer.Enabled = true;
		timer.Elapsed += new ElapsedEventHandler(_timerElapsed);
		timer.Start();
    }

    void Update()
    {
        //update title screen
        if (curListName == CURRENT_LIST_NAME.LIST_REVIEW)
        {
			titleText.text = STR_REVIEW;

        }
        else if (curListName == CURRENT_LIST_NAME.LIST_AGAIN)
        {
			titleText.text = STR_AGAIN;

        }
        else if (curListName == CURRENT_LIST_NAME.LIST_NEW)
        {
			titleText.text = STR_NEW;
        }

		newText.text 	= STR_NEW + "\n" + newWords.Count.ToString();
		againText.text 	= STR_AGAIN + "\n" + (againWords.Count + subAgainWords.Count).ToString();
		reviewText.text = STR_REVIEW + "\n" + reviewWords.Count.ToString();
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
        btnShowAnswer.gameObject.SetActive(false);
        ButtonAnswer.SetActive(true);

        loadAnswer();
    }

    void AgainClick()
    {
		if (Application.internetReachability == NetworkReachability.NotReachable) {
			showDialogNoconnection();

		} else {
	        //update word with again option
	        FirebaseHelper.getInstance()
	            .updateWordProgressWithEaseOption(currentWord,
	                                            wordUserLearning.wordProgress,
	                                            CommonDefine.OPTION_AGAIN);
	        //add to again queue (add to the end of queue)
	        //if user is learning in again queue, it it not a problem because this word will be removed in endSessionStudy function and update to db
	        subAgainWords.Add(currentWord);
	        updateAgainFieldInLearningProgressToday();

	        endSessionStudy();
		}
    }

    void HardClick()
    {
		if (Application.internetReachability == NetworkReachability.NotReachable) {
			showDialogNoconnection();

		} else {
	        //update word with hard option
	        FirebaseHelper.getInstance()
	            .updateWordProgressWithEaseOption(currentWord,
	                wordUserLearning.wordProgress,
	                CommonDefine.OPTION_HARD);

	        endSessionStudy();
		}
    }

    void NormalClick()
    {
		if (Application.internetReachability == NetworkReachability.NotReachable) {
			showDialogNoconnection();

		} else {
	        //update word with normal option
	        FirebaseHelper.getInstance()
	            .updateWordProgressWithEaseOption(currentWord,
	                wordUserLearning.wordProgress,
	                CommonDefine.OPTION_GOOD);

	        endSessionStudy();
		}
    }

    void EasyClick()
    {
		if (Application.internetReachability == NetworkReachability.NotReachable) {
			showDialogNoconnection();

		} else {
	        //update word with easy option
	        FirebaseHelper.getInstance()
	            .updateWordProgressWithEaseOption(currentWord,
	                wordUserLearning.wordProgress,
	                CommonDefine.OPTION_EASY);

	        endSessionStudy();
		}
    }

    void endSessionStudy()
    {
        Debug.Log("endSessionStudy");
        btnShowAnswer.gameObject.SetActive(true);
        ButtonAnswer.SetActive(false);
        bool needRemoveWord = true;

        currentWordInd = 0;
        currentWord = "";
        wordUserLearning = null;

        showHideLoadingIndicator(true);

        if (curListName == CURRENT_LIST_NAME.LIST_REVIEW)
        {
            if (needRemoveWord == true)
            {
                reviewWords.RemoveAt(currentWordInd);
                needRemoveWord = false;

                FirebaseHelper.getInstance().updateInreviewFieldInLearningProgressToday(reviewWords.ToArray());
            }

            if (reviewWords.Count == 0)
            {
                curListName = CURRENT_LIST_NAME.LIST_AGAIN;
                currentWordInd = 0;

            }
            else
            {
                //currentWordInd++;	//always get item at 0 because we remove words after learn them
                currentWord = reviewWords.ElementAt(currentWordInd);
            }

        }

        if (curListName == CURRENT_LIST_NAME.LIST_AGAIN)
        {
            if (needRemoveWord == true)
            {
                againWords.RemoveAt(currentWordInd);
                needRemoveWord = false;

                updateAgainFieldInLearningProgressToday();
            }

            if (againWords.Count == 0)
            {
                curListName = CURRENT_LIST_NAME.LIST_NEW;
                currentWordInd = 0;

            }
            else
            {

                currentWord = againWords.ElementAt(currentWordInd);
            }

        }

        if (curListName == CURRENT_LIST_NAME.LIST_NEW)
        {
            if (needRemoveWord == true)
            {
                newWords.RemoveAt(currentWordInd);
                needRemoveWord = false;

                FirebaseHelper.getInstance().updateNewWordsFieldInLearningProgressToday(newWords.ToArray());
            }

            if (newWords.Count == 0)
            {
                currentWordInd = 0;
                //check subAgainlist for current session
                if (subAgainWords.Count > 0)
                {
                    curListName = CURRENT_LIST_NAME.LIST_AGAIN;
                    againWords.AddRange(subAgainWords.ToArray());
                    currentWord = againWords.ElementAt(currentWordInd);

                    subAgainWords.Clear();

                }
                else
                {
                    //finish daily target, record daily target here.
                    //check streak
                    FirebaseHelper.getInstance().checkStreakAfterLearningFinished(isStreak =>
                    {
                        Debug.Log("Is Count streak :: " + isStreak.ToString());

                        //close study screen
                        showHideLoadingIndicator(false);
						Scene home = SceneManager.GetSceneByName("Home");
						GameObject[] gameObjs = home.GetRootGameObjects();

						foreach (GameObject gObj in gameObjs) {
							if (gObj.name.Equals("ScriptHolder") == true) {
								Debug.Log("Study Controller :: ScriptHolder found");
								HomeController homeController = gObj.GetComponent<HomeController>();

									homeController.completedDailyTargetHandle(isStreak);
							}
						}

                        unloadSceneAsync();
                    });
                }

            }
            else
            {
                currentWord = newWords.ElementAt(currentWordInd);
            }
        }

        if (currentWord.Length > 0)
        {
            fetchWordUserLearningInfo();

        }
        else
        {
            showHideLoadingIndicator(false);
        }
    }


    private void getUserData()
    {
        //review list, again list, new word list had been prepared in home screen,
        //so do not need to count number of each list
        //just load them
        currentWordInd = 0;
        currentWord = "";
        wordUserLearning = null;

        reviewWords.Clear();
        againWords.Clear();
        newWords.Clear();
        subAgainWords.Clear();

        curListName = CURRENT_LIST_NAME.LIST_REVIEW; //reset, review is default
        bool found = false;

        showHideLoadingIndicator(true);
        try
        {
            FirebaseHelper.getInstance().getCurrentReviewList(rwords =>
            {
                reviewWords.AddRange(rwords);


                FirebaseHelper.getInstance().getAgainList(agwords =>
                {
                    againWords.AddRange(agwords);


                    FirebaseHelper.getInstance().getCurrentNewWordsList(nwords =>
                    {
                        newWords.AddRange(nwords);

                        Debug.Log("reviewWords :: count :: " + reviewWords.Count);
                        Debug.Log("againWords :: count :: " + againWords.Count);
                        Debug.Log("newWords :: count :: " + newWords.Count);

                        if (reviewWords.Count > 0)
                        {
                            Debug.Log("reviewWords :: " + reviewWords);
                            curListName = CURRENT_LIST_NAME.LIST_REVIEW;
                            currentWord = reviewWords.ElementAt(currentWordInd);
                            found = true;

                        }
                        else if (againWords.Count > 0)
                        {
                            Debug.Log("againWords :: " + againWords);
                            curListName = CURRENT_LIST_NAME.LIST_AGAIN;
                            currentWord = againWords.ElementAt(currentWordInd);
                            found = true;

                        }
                        else if (newWords.Count > 0)
                        {
                            Debug.Log("newWords :: " + newWords);
                            curListName = CURRENT_LIST_NAME.LIST_NEW;
                            currentWord = newWords.ElementAt(currentWordInd);
                            found = true;
                        }

                        if (found == false)
                        {
                            showHideLoadingIndicator(false);
                            //show alert: no word to learn
							Scene home = SceneManager.GetSceneByName("Home");
							GameObject[] gameObjs = home.GetRootGameObjects();

							foreach (GameObject gObj in gameObjs) {
								if (gObj.name.Equals("ScriptHolder") == true) {
									Debug.Log("Study Controller :: ScriptHolder found");
									HomeController homeController = gObj.GetComponent<HomeController>();

									homeController.showDialogNowordToLearnFromStudyScene();
								}
							}

                            //close screen
                            Debug.Log("UnloadSceneAsync(\"Study\")");
                            unloadSceneAsync();

                        }
                        else
                        {
                            //fetch word info and learning progress
                            fetchWordUserLearningInfo();
                        }

                    });
                });
            });
        }
        catch (Exception e)
        {
            Debug.Log("StudyController :: getUserData :: exception :: " + e.ToString());
            showHideLoadingIndicator(false);
        }
    }

    private void fetchWordUserLearningInfo()
    {
        Debug.Log("fetchWordUserLearningInfo :: " + currentWord);
        showHideLoadingIndicator(true);

        if (currentWord.Length > 0)
        {
            FirebaseHelper.getInstance().fetchWordUserLearningInfo(currentWord, userLearning =>
            {
                if (userLearning.wordInfo != null)
                {
                    Debug.Log("fetchWordUserLearningInfo :: display HTML");
                    wordUserLearning = userLearning;

                    Debug.Log("wordUserLearning :: wordInfo.word :: " + wordUserLearning.wordInfo.word);
                    showHideLoadingIndicator(false);
                    loadQuestion();

                }
                else
                {
                    wordUserLearning = null;
                    endSessionStudy();
                }
            });
        }
        else
        {
            endSessionStudy();
        }
    }

    private string[] removeBlankItems(string[] arr)
    {
        List<string> resList = new List<string>();

        foreach (string item in arr)
        {
            Debug.Log("removeBlankItems :: " + item);
            if (item.Length > 0)
            {
                resList.Add(item);
            }
        }

        return resList.ToArray();
    }

    private void loadQuestion()
    {
        htmlText = HTMLHelper.createHTMLForQuestion(wordUserLearning.wordInfo);

        Debug.Log("loadQuestion :: htmlText :: " + htmlText);
        LoadFromText();
    }

    private void loadAnswer()
    {
        htmlText = HTMLHelper.createHTMLForAnswer(wordUserLearning.wordInfo);

        Debug.Log("loadAnswer :: htmlText :: " + htmlText);
		setTextFor4Buttons();
        LoadFromText();
    }

    UniWebView CreateWebView()
    {
        UniWebView webView = null;
		webView = webViewContainer.GetComponent<UniWebView>();

        if (webView == null)
        {
			webView = webViewContainer.AddComponent<UniWebView>();
        }

        webView.toolBarShow = false;
        webView.SetShowSpinnerWhenLoading(false);
		webView.backButtonEnable = false;
		webView.insets.top = 100;
		webView.insets.bottom = 70;
		webView.SetBackgroundColor(Color.clear);

        return webView;
    }

    public void LoadFromText()
    {
        UniWebView webView = CreateWebView();

        webView.LoadHTMLString(htmlText, null);
        webView.Show();
    }

    private void updateAgainFieldInLearningProgressToday() {
        List<string> tmp = new List<string>();

        tmp.AddRange(againWords.ToArray());
        tmp.AddRange(subAgainWords.ToArray());

        FirebaseHelper.getInstance().updateAgainFieldInLearningProgressToday(tmp.ToArray());
    }

    private void unloadSceneAsync()
    {
        //set isShowingStudyScene = false;
        Scene home = SceneManager.GetSceneByName("Home");
        GameObject[] gameObjs = home.GetRootGameObjects();

        foreach (GameObject gObj in gameObjs)
        {
            if (gObj.name.Equals("ScriptHolder") == true)
            {
                Debug.Log("Study Controller :: ScriptHolder found");
                HomeController homeController = gObj.GetComponent<HomeController>();
                homeController.isShowingStudyScene = false;
            }
        }

		//dont knwon why webview still display after unload scene
		UniWebView webView = null;
		webView = webViewContainer.GetComponent<UniWebView>();
		if (webView != null)
		{
			Debug.Log("Study Controller :: Destroy webView");
			Destroy(webView.gameObject);
		}

        SceneManager.UnloadSceneAsync("Study");
    }


    public void OnBackButtonClick() {
		unloadSceneAsync();
    }

    void OnDestroy() {
		Debug.Log("Study Controller :: OnDestroy");

		if (timer != null) {
			timer.Stop();
		}
    }

    private void showHideLoadingIndicator(bool show) {
        if (loadingIndicator != null) {
            loadingIndicator.SetActive(show);
        }
    }

	private void setTextFor4Buttons () {
		string[] titles = Algorithm.getInstance().nextIntervalStringsList(wordUserLearning.wordProgress);
		Debug.Log("setTextFor4Buttons :: Again :: " + titles[0]);
		Debug.Log("setTextFor4Buttons :: Hard :: " + titles[1]);
		Debug.Log("setTextFor4Buttons :: Good :: " + titles[2]);
		Debug.Log("setTextFor4Buttons :: Easy :: " + titles[3]);

		btnAgain.GetComponentInChildren<Text>().text 	= String.Format("{0}\n{1}", "Again", titles[0]);
		btnHard.GetComponentInChildren<Text>().text 	= String.Format("{0}\n{1}", "Hard", titles[1]);
		btnNormal.GetComponentInChildren<Text>().text	= String.Format("{0}\n{1}", "Good", titles[2]);
		btnEasy.GetComponentInChildren<Text>().text 	= String.Format("{0}\n{1}", "Easy", titles[3]);
	}
		
	private void _timerElapsed(object sender, ElapsedEventArgs e)
	{
		//check internet connection
		if (Application.internetReachability == NetworkReachability.NotReachable) {
			showHideNoConnectionPanel(true);

		} else {
			showHideNoConnectionPanel(false);
		}
	}

	private void showHideNoConnectionPanel (bool flag) {
		noInternetPanel.SetActive(flag);
	}

	void OnApplicationPause(bool pause)
	{
		if (pause == true)
		{
			Debug.Log("study Controller :: OnBecameInvisible");
			if (timer != null)
			{
				timer.Stop();
			}

		}
		else
		{
			Debug.Log("study Controller :: OnBecameVisible");
			if (timer != null)
			{
				timer.Start();
			}
		}
	}

	/************ Dialog message ************/
	private void showDialogNoconnection () {
		showSingleButtonDialog(STR_NO_CONNECTION_ALERT_CONTENT);
	}

	private void showSingleButtonDialog (string message) {
		if (dialogMessageController == null) {
			dialogMessageController = dialogMsg.GetComponentInChildren<DialogMessageController>();	
		}

		dialogMessageController.setDialogType(DialogMessageController.DIALOG_TYPE.DIALOG_TYPE_ONEBUTTON);

		dialogMessageController.OnButtonOkClickDelegate = OnButtonOkClickDelegate;

		dialogMessageController.setMessage(message);
		dialogMessageController.Show();
	}

	private void hideDialog() {
		if (dialogMessageController == null) {
			dialogMessageController = dialogMsg.GetComponentInChildren<DialogMessageController>();	
		}

		dialogMessageController.Hide();
	}

	private void OnButtonOkClickDelegate () {
		Debug.Log("OnButtonOkClickDelegate");
		hideDialog();
	}
}
