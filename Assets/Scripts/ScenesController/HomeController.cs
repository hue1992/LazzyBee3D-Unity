using System;
using System.Timers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Facebook.Unity;

public class HomeController : MonoBehaviour
{
    Timer timer = new Timer();

    public GameObject loadingIndicator;
	public GameObject settingsPanel;
	public GameObject dialogMsg;
	private DialogMessageController dialogMessageController;

    private bool needToReloadData = false;  //need to reload data when new day was come
    [HideInInspector]
    public bool isShowingStudyScene = false;    //must be public to access it from study scene

    //load when a new day was come
	private void _loadNewData()
    {
        needToReloadData = false;

        int beginOfDay = DateTimeHelper.getBeginOfDayInSec();
        TemporarilyStatus.getInstance().timeLoadedData = beginOfDay;
        showHideLoadingIndicator(true);
        _loadTodayData();
    }

	//add more word to learn
	public void loadMoreData()
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
						}
					});
				
			} else {
				//show alert: you have to comleted daily target...
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
        timer.Interval = 1000;  //millisecond
        timer.Enabled = true;
        timer.Elapsed += new ElapsedEventHandler(_timerElapsed);
        timer.Start();
    }

    public void OnBtnStartClickHandle()
    {
        isShowingStudyScene = true;
        SceneManager.LoadScene("Study", LoadSceneMode.Additive);
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

    void OnDestroy()
    {
        Debug.Log("Home Controller :: OnDestroy");
        if (timer != null)
        {
            timer.Stop();
        }
    }

    private void showHideLoadingIndicator(bool show)
    {
        if (loadingIndicator != null)
        {
            loadingIndicator.SetActive(show);
        }
    }

    public void OnBtnSettingClickHandle()
    {
		settingsPanel.SetActive(true);
    }

    public void OnProfileButtonClick()
    {
        SceneManager.LoadScene("Profile", LoadSceneMode.Additive);
    }

	//dialog message
	private void showDialog (string message) {
//		DIALOG_TYPE 
//		dialogMessageController = dialogMsg.GetComponentInChildren<DialogMessageController>();
//		dialogMessageController.OnButtonOkClickDelegate = OnButtonOkClickDelegate;
	}

}
