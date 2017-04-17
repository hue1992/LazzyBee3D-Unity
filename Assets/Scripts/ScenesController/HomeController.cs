using System;
using System.Timers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Facebook.Unity;

public class HomeController : MonoBehaviour
{
	enum DIALOG_TAG_NAME {
		TAG_NO_CONNECTION = 0,
		TAG_NOT_COMPLETED_TARGET = 1,	//click more words when not completed target
		TAG_CONGRATULATION = 2,			//after finishing target
		TAG_NORMORE_WORD_TO_LEARN = 3,	//click study after finished target
		TAG_STILL_HAVE_MORE_WORDS = 4	//completed target but still have words to review
	};

    Timer timer = new Timer();

    public GameObject loadingIndicator;
	public GameObject settingsPanel;
	public GameObject dialogMsg;
	private DialogMessageController dialogMessageController;

	public GameObject noInternetPanel;

    private bool needToReloadData = false;  //need to reload data when new day was come

	private const string STR_NO_CONNECTION_ALERT_CONTENT	= "No internet connection!\nPlease double check wifi/3G connection.";
	private const string STR_NOT_COMPLETED_DAILY_TARGET		= "You have not completed daily target. Complete it before adding more words.";
	private const string STR_CONGRATULATION					= "Congratulation!\nNow is the time to relax.";
	private const string STR_NOMORE_WORD_TO_LEARN			= "You had finished daily target. Click \"More words\" if you really want to learn more.";
	private const string STR_STILL_HAVE_MORE_WORDS			= "You still have a few more words need to review today. Learn now?";

    [HideInInspector]
    public bool isShowingStudyScene = false;    //must be public to access it from study scene

    //load when a new day was come
	private void _loadNewData() {
        needToReloadData = false;

        int beginOfDay = DateTimeHelper.getBeginOfDayInSec();
        TemporarilyStatus.getInstance().timeLoadedData = beginOfDay;
        showHideLoadingIndicator(true);
        _loadTodayData();
    }

	private void _loadRemainWordsToReview() {
		showHideLoadingIndicator(true);
		FirebaseHelper.getInstance().prepareInreviewList(1000,	//no limit
			againRes =>
			{
				showHideLoadingIndicator(false);
				Debug.Log("Again count :: " + againRes);

				if (againRes > 0) {
					showDialogStillHaveMoreWordToLearn();

				} else {
					//do nothing
				}
			});
	}

	//add more word to learn
	private void _loadMoreData()
	{
		//do not need to update the loaded data date
		showHideLoadingIndicator(true);
		FirebaseHelper.getInstance().checkStreakToday(isCompletedTarget => {
			if (isCompletedTarget == true) {
				FirebaseHelper.getInstance().prepareListNewWordsToLearn(TemporarilyStatus.getInstance().new_card_a_day,
					newRes =>
					{
						showHideLoadingIndicator(false);
						Debug.Log("New count :: " + newRes);
						if (newRes > 0) {
							OnBtnStartClickHandle();

						} else {
							//show alert: no words to learn
							showDialogNoWordToLearn();
						}
					});
				
			} else {
				showHideLoadingIndicator(false);
				//show alert: you have to comleted daily target...
				showDialogNotCompleteDailyTarget();
			}
		});
	}

    private void _loadTodayData()
    {
        Debug.Log("loadTodayData");

        int reviewCount = 0;
        int newCount = 0;
        int againCount = 0;
        try
        {
			FirebaseHelper.getInstance().resetCompletedTodayFlag();

            FirebaseHelper.getInstance().prepareInreviewList(TemporarilyStatus.getInstance().total_card_a_day,
                reviewRes =>
                {

                    reviewCount = reviewRes;
                    Debug.Log("Review count :: " + reviewRes);

                    againCount = TemporarilyStatus.getInstance().total_card_a_day - reviewCount;
                    FirebaseHelper.getInstance().prepareAgainList(againCount,
                        againRes =>
                        {

                            againCount = againRes;
                            Debug.Log("Again count :: " + againRes);

                            newCount = TemporarilyStatus.getInstance().total_card_a_day - againCount - reviewCount;
                            if (newCount > TemporarilyStatus.getInstance().new_card_a_day)
                            {
                                newCount = TemporarilyStatus.getInstance().new_card_a_day;
                            }

                            FirebaseHelper.getInstance().prepareListNewWordsToLearn(newCount,
                                newRes =>
                                {
                                    showHideLoadingIndicator(false);
                                    Debug.Log("New count :: " + newRes);
                                });
                        });
                });
        }
        catch (Exception e)
        {
            showHideLoadingIndicator(false);
            Debug.Log("HomeController :: _loadTodayData :: " + e.ToString());
        }
    }

    // Use this for initialization
    void Start()
    {
        //check login status again
        //if it is logged out due to some unknown reason 
        //it could take time to complete this progress => show loading indicator
        //		Debug.Log("checkLoginStatus");
        //		if (FirebaseHelper.getInstance().checkLoginStatus() == false) {
        //			Debug.Log("Not logged in");
        //			//show login screen
        //			SceneManager.LoadScene("Login", LoadSceneMode.Single);
        //
        //			//for testing => login facebook is default
        //			/*
        //			if (!FB.IsInitialized) {
        //				// Initialize the Facebook SDK
        //				FB.Init(InitCallback, OnHideUnity);
        //
        //			} else {
        //				// Already initialized, signal an app activation App Event
        //				Debug.Log("Already initialized, signal an app activation App Event");
        //				FB.ActivateApp();
        //			}
        //			*/
        //
        //		} else {
        Debug.Log("Logged in already");

        //load today learning data
        //get datetime in /newwords field, check if it is obsolete, prepare new list
        //date in /newwords is always equal to /inreview
        showHideLoadingIndicator(true);
        FirebaseHelper.getInstance().getCurrentDatetimeInNewWordsField(date =>
        {
            Debug.Log("HomeController :: date :: " + date.ToString());

            int curDate = DateTimeHelper.getBeginOfDayInSec();
            Debug.Log("HomeController :: curDate :: " + curDate.ToString());

            if (date != curDate)
            {
                Debug.Log("HomeController :: load new data");
                TemporarilyStatus.getInstance().timeLoadedData = curDate;   //in timer handler function, if current time is greater than this time 86400, reload data.

                _loadTodayData();

            }
            else
            {
                showHideLoadingIndicator(false);
                TemporarilyStatus.getInstance().timeLoadedData = date;
                Debug.Log("HomeController :: do not load new data");
            }

        });

        //can load/set streak here, because it is used right now
        //when use streak info, should check days != null
        FirebaseHelper.getInstance().getUserStreaks(isSuccessful =>
        {
            if (isSuccessful == false)
            {
                FirebaseHelper.getInstance().updateUserStreaks();
            }
        });

        //set timer to check date time, if it is changed to new day, reload data
        timer.Interval = 5000;  //millisecond
        timer.Enabled = true;
        timer.Elapsed += new ElapsedEventHandler(_timerElapsed);
        timer.Start();
    }

    public void OnBtnStartClickHandle()
    {
		if (Application.internetReachability == NetworkReachability.NotReachable) {
			showDialogNoconnection();

		} else {
	        isShowingStudyScene = true;
	        SceneManager.LoadScene("Study", LoadSceneMode.Additive);
		}
    }

	public void OnBtnMoreWordsClickHandle()
	{
		if (Application.internetReachability == NetworkReachability.NotReachable) {
			showDialogNoconnection();

		} else {
			_loadMoreData();
		}
	}


    private void _timerElapsed(object sender, ElapsedEventArgs e)
    {
        int curDate = DateTimeHelper.getCurrentDateTimeInSeconds();


        if (curDate >= TemporarilyStatus.getInstance().timeLoadedData + CommonDefine.SECONDS_PERDAY)
        {
            needToReloadData = true;

            //reload data if study scene is not on top
            //else if study scene is on top, reload data from study scene when user finish their target
            //if user come back home scene when they do not finish target, _timerElapsed is still called and reload data.
            if (isShowingStudyScene == false)
            {
				_loadNewData();
            }
        }

		//check internet connection
		if (Application.internetReachability == NetworkReachability.NotReachable) {
			showHideNoConnectionPanel(true);

		} else {
			showHideNoConnectionPanel(false);
		}
    }

    void OnApplicationPause(bool pause)
    {
        if (pause == true)
        {
            Debug.Log("Home Controller :: OnBecameInvisible");
            if (timer != null)
            {
                timer.Stop();
            }

        }
        else
        {
            Debug.Log("Home Controller :: OnBecameVisible");
            if (timer != null)
            {
                timer.Start();
            }
        }
    }

    void OnDestroy() {
        Debug.Log("Home Controller :: OnDestroy");
        if (timer != null) {
            timer.Stop();
        }
    }

    private void showHideLoadingIndicator(bool show) {
        if (loadingIndicator != null) {
            loadingIndicator.SetActive(show);
        }
    }

    public void OnBtnSettingClickHandle() {
		settingsPanel.SetActive(true);
    }

    public void OnProfileButtonClick() {
        SceneManager.LoadScene("Profile", LoadSceneMode.Additive);
    }

	public void completedDailyTargetHandle () {
		//show streak scene
		//after show streak scene call _loadRemainWordsToReview()
		if (needToReloadData == true) {	//if it is a new day, just load new data
			_loadNewData();

		} else {
			//else: show streak scene if it is completed target session
			//or if it is "learn more" session, show alert congrat
			showHideLoadingIndicator(true);
			FirebaseHelper.getInstance().checkStreakToday(isCompletedTarget => {
				showHideLoadingIndicator(false);

				if (isCompletedTarget == true) {
					//show streak scene
					showDialogCongratulation(); //for test

				} else {
					showDialogCongratulation();
				}
			});
		}
	}

	public void showDialogNowordToLearnFromStudyScene () {
		showDialogNoWordToLearn();
	}

	/************ Dialog message ************/
	/*
	enum DIALOG_TAG_NAME {
		TAG_NO_CONNECTION,
		TAG_NOT_COMPLETED_TARGET,
		TAG_CONGRATULATION,
		TAG_NORMORE_WORDS_TO_LEARN,
		TAG_STILL_HAVE_MORE_WORDS
	};
	*/

	private void showDialogNoconnection () {
		showSingleButtonDialog(STR_NO_CONNECTION_ALERT_CONTENT);

		setDialogTag(DIALOG_TAG_NAME.TAG_NO_CONNECTION);
	}

	private void showDialogNotCompleteDailyTarget () {
		showTwoButtonDialog(STR_NOT_COMPLETED_DAILY_TARGET, "Later", "Learn");

		setDialogTag(DIALOG_TAG_NAME.TAG_NOT_COMPLETED_TARGET);
	}

	private void showDialogCongratulation () {
		showSingleButtonDialog(STR_CONGRATULATION);

		setDialogTag(DIALOG_TAG_NAME.TAG_CONGRATULATION);
	}

	private void showDialogNoWordToLearn () {
		showSingleButtonDialog(STR_NOMORE_WORD_TO_LEARN);

		setDialogTag(DIALOG_TAG_NAME.TAG_NORMORE_WORD_TO_LEARN);
	}

	private void showDialogStillHaveMoreWordToLearn () {
		showTwoButtonDialog(STR_STILL_HAVE_MORE_WORDS, "Later", "Learn");

		setDialogTag(DIALOG_TAG_NAME.TAG_STILL_HAVE_MORE_WORDS);
	}

	private void setDialogTag(DIALOG_TAG_NAME tg) {
		if (dialogMessageController == null) {
			dialogMessageController = dialogMsg.GetComponentInChildren<DialogMessageController>();	
		}

		dialogMessageController.setDiglogTag((int)tg);
	}

	private DIALOG_TAG_NAME getDialogTag () {
		if (dialogMessageController == null) {
			dialogMessageController = dialogMsg.GetComponentInChildren<DialogMessageController>();	
		}

		return (DIALOG_TAG_NAME)dialogMessageController.diglogTag;
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

	private void showTwoButtonDialog (string message, string btnOne, string btnTwo) {
		if (dialogMessageController == null) {
			dialogMessageController = dialogMsg.GetComponentInChildren<DialogMessageController>();	
		}

		dialogMessageController.setDialogType(DialogMessageController.DIALOG_TYPE.DIALOG_TYPE_TWOBUTTON);

		dialogMessageController.OnButtonOneClickDelegate = OnButtonOneClickDelegate;
		dialogMessageController.OnButtonTwoClickDelegate = OnButtonTwoClickDelegate;

		dialogMessageController.setMessage(message);
		dialogMessageController.setButtonOneText(btnOne);
		dialogMessageController.setButtonTwoText(btnTwo);

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

		DIALOG_TAG_NAME tag = getDialogTag();
		if (tag == DIALOG_TAG_NAME.TAG_CONGRATULATION) {
			//if still have words to review, show alert to inform
			//else do nothing, just close

		}
	}

	private void OnButtonOneClickDelegate () {
		Debug.Log("OnButtonOneClickDelegate");
		hideDialog();
	}

	private void OnButtonTwoClickDelegate () {
		Debug.Log("OnButtonTwoClickDelegate");
		hideDialog();

		DIALOG_TAG_NAME tag = getDialogTag();
		if (tag == DIALOG_TAG_NAME.TAG_NOT_COMPLETED_TARGET) {
			OnBtnStartClickHandle();

		} else if (tag == DIALOG_TAG_NAME.TAG_STILL_HAVE_MORE_WORDS) {
			OnBtnStartClickHandle();
		}
	}

	private void showHideNoConnectionPanel (bool flag) {
		noInternetPanel.SetActive(flag);
	}
}
