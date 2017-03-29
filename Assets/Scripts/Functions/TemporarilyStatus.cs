using System.Collections;
using System.Collections.Generic;

public class TemporarilyStatus {
//	public int current_level 		= CommonDefine.SETTINGS_DEFAULT_LEVEL;	//current level, even if my_level is 2, current_level could be 3 when user leaerned all words in level 2
	public int current_word_index 	= 0;

	public int my_level 			= CommonDefine.SETTINGS_DEFAULT_LEVEL;	//set in settings
	public int new_card_a_day 		= CommonDefine.SETTINGS_NEWCARD_A_DAY;
	public int total_card_a_day 	= CommonDefine.SETTINGS_TOTALCARD_A_DAY;
	public int time_to_show_answer 	= CommonDefine.SETTINGS_TIME_SHOW_ANSWER;
	public int auto_play_sound		= CommonDefine.SETTINGS_AUTO_PLAY;		//0 or 1
	public int notification			= CommonDefine.SETTINGS_NOTIFICATION;	//0 or 1

	public float speaking_speed		= 1.0f;

	public UserInfo userInfo 		= null;

	public string[] days 			= null;
	public int streaks 				= 0;

	private static TemporarilyStatus _instance = null;

	public static TemporarilyStatus getInstance() {
		if (_instance == null) {
			_instance = new TemporarilyStatus();

		}
		return _instance;
	}

	public void addDayToStreak (string day) {
		if (days != null) {
			List<string> tmp = new List<string>(days);

			tmp.Add(day);

			if (tmp.Count > 7) {
				tmp.RemoveAt(0);
			}

			days = tmp.ToArray();
		}
	}

	public string convertDayStreakToString () {
		string res = "";

		if (days != null) {
			res = string.Join(',', days);
		}

		return res;
	}
}
