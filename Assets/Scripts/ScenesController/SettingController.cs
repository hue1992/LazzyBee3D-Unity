using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SettingController : MonoBehaviour {
	enum SLIDE_NAME	{
		SLIDE_NAME_WORD_PER_DAY,
		SLIDE_NAME_TOTAL_PER_DAY,
		SLIDE_NAME_WAITING_TIME,
		SLIDE_NAME_SPEAKING_SPEED
	};

	enum TOGGLE_NAME	{
		TOGGLE_NAME_AUTOPLAY_SOUND,
		TOGGLE_NAME_DISPLAY_MEANING,
		TOGGLE_NAME_NOTIFICATION
	};

	public Dropdown optionsLevels;
	public Slider slideWordPerDay;
	public Slider slideTotalPerDay;
	public Slider slideWaitingTime;
	public Slider slideSpeakingSpeed;
	public Toggle toggleAutoPlay;
	public Toggle toggleDisplayMeaing;
	public Toggle toggleNotification;

	public Text wordPerDayValue;
	public Text totalPerDayValue;
	public Text waitingTimevalue;

    // Use this for initialization
    void Start() {
        int my_level = TemporarilyStatus.getInstance().my_level;
        int new_card_a_day = TemporarilyStatus.getInstance().new_card_a_day;
        int total_card_a_day = TemporarilyStatus.getInstance().total_card_a_day;
        int time_to_show_answer = TemporarilyStatus.getInstance().time_to_show_answer;
        int auto_play_sound = TemporarilyStatus.getInstance().auto_play_sound;
		int display_meaning = TemporarilyStatus.getInstance().display_meaning;
        int notification = TemporarilyStatus.getInstance().notification;
        float speaking_speed = TemporarilyStatus.getInstance().speaking_speed;

		Debug.Log("new cards a day :: " +new_card_a_day.ToString());

		/* DROPDOWN CONTROLLER */
		//level
		optionsLevels.value = my_level - 1;	//optionsLevels.value is counted from 0
		optionsLevels.onValueChanged.AddListener(delegate {
			levelDropdownValueChangedHandler();
		});

		/* SLIDE CONTROLLER */
		//word per day
		slideWordPerDay.value = new_card_a_day;
		slideWordPerDay.onValueChanged.AddListener(delegate {
			slideValueChangedHandler(SLIDE_NAME.SLIDE_NAME_WORD_PER_DAY);
		});

		wordPerDayValue.text = new_card_a_day.ToString();

		//total per day
		slideTotalPerDay.value = total_card_a_day;
		slideTotalPerDay.onValueChanged.AddListener(delegate {
			slideValueChangedHandler(SLIDE_NAME.SLIDE_NAME_TOTAL_PER_DAY);
		});

		totalPerDayValue.text = total_card_a_day.ToString();

		//waiting time
		slideWaitingTime.value = time_to_show_answer;
		slideWaitingTime.onValueChanged.AddListener(delegate {
			slideValueChangedHandler(SLIDE_NAME.SLIDE_NAME_WAITING_TIME);
		});

		waitingTimevalue.text = time_to_show_answer.ToString();

		//speaking speed
		slideSpeakingSpeed.value = speaking_speed;
		slideSpeakingSpeed.onValueChanged.AddListener(delegate {
			slideValueChangedHandler(SLIDE_NAME.SLIDE_NAME_SPEAKING_SPEED);
		});

		/* TOGGLE CONTROLLER */
		//autoplay sound
		toggleAutoPlay.isOn = (auto_play_sound == 0) ? false : true;
		toggleAutoPlay.onValueChanged.AddListener(delegate {
			toggleValueChangedHandler(TOGGLE_NAME.TOGGLE_NAME_AUTOPLAY_SOUND);
		});

		//display meaning
		toggleDisplayMeaing.isOn = (display_meaning == 0) ? false : true;
		toggleDisplayMeaing.onValueChanged.AddListener(delegate {
			toggleValueChangedHandler(TOGGLE_NAME.TOGGLE_NAME_DISPLAY_MEANING);
		});

		//on off notification
		toggleNotification.isOn = (notification == 0) ? false : true;
		toggleNotification.onValueChanged.AddListener(delegate {
			toggleValueChangedHandler(TOGGLE_NAME.TOGGLE_NAME_NOTIFICATION);
		});
        //get user setting
//        FirebaseHelper.getInstance().getUserSettings(handlerGetUserSetting); //do not need to load User settings here, User settings were loaded right away after login successfully
    }

    public void OnCloseButtonClick() {
		Debug.Log("OnCloseButtonClick");
//		GameObject tmp = GameObject.Find("Dropdown List");
//		if (tmp != null) {
//			Debug.Log("Destroy :: Dropdown List");
//			Destroy(tmp);
//		}

		gameObject.SetActive(false);
    }

	//drop down handler
	void levelDropdownValueChangedHandler() {
		TemporarilyStatus.getInstance().my_level = optionsLevels.value + 1;	//optionsLevels.value is counted from 0

		FirebaseHelper.getInstance().updateSettingLevel(TemporarilyStatus.getInstance().my_level);
	}

	//slide handler
	void slideValueChangedHandler(SLIDE_NAME slideName) {
		Debug.Log("slideValueChangedHandler :: " +slideName.ToString());

		if (slideName == SLIDE_NAME.SLIDE_NAME_SPEAKING_SPEED) {
			TemporarilyStatus.getInstance().speaking_speed = slideSpeakingSpeed.value;
			FirebaseHelper.getInstance().updateSettingSpeakingSpeed(TemporarilyStatus.getInstance().speaking_speed);

		} else if (slideName == SLIDE_NAME.SLIDE_NAME_TOTAL_PER_DAY) {
			TemporarilyStatus.getInstance().total_card_a_day = Mathf.RoundToInt(slideTotalPerDay.value);
			totalPerDayValue.text = TemporarilyStatus.getInstance().total_card_a_day.ToString();

			FirebaseHelper.getInstance().updateSettingTotalPerDay(TemporarilyStatus.getInstance().total_card_a_day);

		} else if (slideName == SLIDE_NAME.SLIDE_NAME_WAITING_TIME) {
			TemporarilyStatus.getInstance().time_to_show_answer = Mathf.RoundToInt(slideWaitingTime.value);
			waitingTimevalue.text = TemporarilyStatus.getInstance().time_to_show_answer.ToString();

			FirebaseHelper.getInstance().updateSettingTimeShowAnswer(TemporarilyStatus.getInstance().time_to_show_answer);

		} else if (slideName == SLIDE_NAME.SLIDE_NAME_WORD_PER_DAY) {
			TemporarilyStatus.getInstance().new_card_a_day = Mathf.RoundToInt(slideWordPerDay.value);
			wordPerDayValue.text = TemporarilyStatus.getInstance().new_card_a_day.ToString();
			FirebaseHelper.getInstance().updateSettingWordPerDay(TemporarilyStatus.getInstance().new_card_a_day);
		}
	}

	//toggle handler
	void toggleValueChangedHandler(TOGGLE_NAME toggleName) {
		Debug.Log("toggleValueChangedHandler :: " +toggleName.ToString());

		if (toggleName == TOGGLE_NAME.TOGGLE_NAME_AUTOPLAY_SOUND) {
			TemporarilyStatus.getInstance().auto_play_sound = toggleAutoPlay.isOn == true? 1 : 0;
			FirebaseHelper.getInstance().updateSettingAutoplaySound(TemporarilyStatus.getInstance().auto_play_sound);

		} else if (toggleName == TOGGLE_NAME.TOGGLE_NAME_DISPLAY_MEANING) {
			TemporarilyStatus.getInstance().display_meaning = toggleDisplayMeaing.isOn == true? 1 : 0;
			FirebaseHelper.getInstance().updateSettingDisplayMeaning(TemporarilyStatus.getInstance().display_meaning);

		} else if (toggleName == TOGGLE_NAME.TOGGLE_NAME_NOTIFICATION) {
			TemporarilyStatus.getInstance().notification = toggleNotification.isOn == true? 1 : 0;
			FirebaseHelper.getInstance().updateSettingOnOffNotification(TemporarilyStatus.getInstance().notification);
		}
	}

	void OnDestroy () {

		optionsLevels.onValueChanged.RemoveAllListeners();
		slideWordPerDay.onValueChanged.RemoveAllListeners();
		slideTotalPerDay.onValueChanged.RemoveAllListeners();
		slideWaitingTime.onValueChanged.RemoveAllListeners();
		slideSpeakingSpeed.onValueChanged.RemoveAllListeners();
		toggleAutoPlay.onValueChanged.RemoveAllListeners();
		toggleDisplayMeaing.onValueChanged.RemoveAllListeners();
		toggleNotification.onValueChanged.RemoveAllListeners();
	}
}
