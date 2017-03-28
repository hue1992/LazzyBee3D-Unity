using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Algorithm {
	private static Algorithm _instance = null;

	public static Algorithm getInstance() {
		if (_instance == null) {
			_instance = new Algorithm();

		}
		return _instance;
	}

	/************ mark internal function **************/
	private int factorAdditionValue(int easeOption) {
		int res = 0;

		if (easeOption == CommonDefine.OPTION_AGAIN) {
			res = -300;

		} else if (easeOption == CommonDefine.OPTION_AGAIN) {
			res = -150;

		} else if (easeOption == CommonDefine.OPTION_GOOD) {
			res = 0;

		} else if (easeOption == CommonDefine.OPTION_AGAIN) {
			res = 150;

		} else {
			res = 0;
		}

		return res;
	}

	/*
 	* Return string of the next time span to review those are corresponded to ease level
 	*/
	private string nextIntervalStringWithEaseOption(WordProgress wordProgress, int easeOption) {
		string str;
		int ivl = nextIntervalBySecondsWithEaseOption(wordProgress, easeOption);

		if (ivl < CommonDefine.SECONDS_PERDAY) {
			str =  CommonDefine.STR_TIME_LEARN_AGAIN;

		} else {
			int day = ivl / CommonDefine.SECONDS_PERDAY;
			if (day <= 30)
				str = String.Format("{0} {1}", (int)Mathf.Round(day), "day(s)");
			else {
				float month = (float)day / 30;
				str = String.Format("{0:0.0} {1}", month, "month(s)");

				if (month > 12) {
					float year = month / 12;

					str = String.Format("{0:0.0} {1}", year, "year(s)");
				}
			}
		}

		return str;
	}

	/**
 	* Return the next interval for CARD, in seconds.
 	*/
	private int nextIntervalBySecondsWithEaseOption(WordProgress wordProgress, int easeOption) {
		if (easeOption == CommonDefine.OPTION_AGAIN) {
			return CommonDefine.TIME_LEARN_AGAIN; /*10 minute*/
		}

		return (nextIntervalByDaysWithEaseOption(wordProgress, easeOption) * CommonDefine.SECONDS_PERDAY);
	}

	/**
 	* Ideal next interval by days for CARD, given EASE > 0
 	*/
	private int nextIntervalByDaysWithEaseOption(WordProgress wordProgress, int easeOption) {
		int delay = daysLate(wordProgress);
		int interval = 0;

		double fct = wordProgress.e_fact / 1000.0;
		int intLastInterval = wordProgress.last_ivl;
		int ivl_hard = Mathf.Max((int)((intLastInterval + delay/4) * 1.2), intLastInterval + 1);
		int ivl_good = Mathf.Max((int)((intLastInterval + delay/2) * fct), ivl_hard + 1);
		int ivl_easy = Mathf.Max((int)((intLastInterval + delay) * fct * CommonDefine.BONUS_EASY), ivl_good + 1);

		if (easeOption == CommonDefine.OPTION_HARD) {
			interval = ivl_hard;

		} else if (easeOption == CommonDefine.OPTION_GOOD) {
			interval = ivl_good;

		} else if (easeOption == CommonDefine.OPTION_EASY) {
			interval = ivl_easy;
		}

		// Should we maximize the interval?
		return interval;
	}

	/**
 	* Number of days later than scheduled.
 	* only for reviewing, not just learned few minute ago
 	*/
	private int daysLate(WordProgress wordProgress) {
		int queue = wordProgress.queue;

		if (queue != CommonDefine.QUEUE_REVIEW) {
			return 0;
		}

		int due = wordProgress.due;
		int now = DateTimeHelper.getCurrentDateTimeInSeconds();    //have to get exactly date time in sec

		int diff_day = (int)(now - due)/CommonDefine.SECONDS_PERDAY;
		return Mathf.Max(0, diff_day);
	}


	/************ mark external function **************/
	/*
 	* Return strings of the next time spans to review those are corresponded to ease level
 	*/
	public string[] nextIntervalStringsList(WordProgress wordProgress) {
		List<string> res = new List<string>();
		for (int i = 0; i < 4; i++){
				res.Add(nextIntervalStringWithEaseOption(wordProgress, i));
		}

		return res.ToArray();
	}

	/**
	 * Whenever a Card is answered, call this function on Card.
	 * Scheduler will update the following parameters into Card's instance:
	 * <ul>
	 * <li>due
	 * <li>last_ivl
	 * <li>queue
	 * <li>e_factor
	 * <li>rev_count
	 * </ul>
	 * After 'answerCard', the caller will check Card's data for further decisions
	 * (update database or/and put it back to app's queue)
	 */
	public  void updateWordProgressWithEaseOption(ref WordProgress wordProgress, int easeOption) {
		int nextIvl = nextIntervalBySecondsWithEaseOption(wordProgress, easeOption);
		int current = DateTimeHelper.getCurrentDateTimeInSeconds();	//have to get exactly date time in seconds

		//Now we decrease for EASE_AGAIN only when it from review queue
		int queue 	= wordProgress.queue;
		int eFactor = wordProgress.e_fact;

		if (queue == CommonDefine.QUEUE_REVIEW && easeOption == CommonDefine.OPTION_AGAIN) {
			eFactor = wordProgress.e_fact - CommonDefine.FORGET_FINE;
			wordProgress.e_fact = eFactor;

		} else {
			eFactor = Mathf.Max(CommonDefine.MIN_FACTOR, (wordProgress.e_fact + factorAdditionValue(easeOption)));
			wordProgress.e_fact = eFactor;
		}

		if (nextIvl < CommonDefine.SECONDS_PERDAY) {
			/*User forget card or just learn
         	* We don't re-count 'due', because app will put it back to learned queue
         	*/
			wordProgress.queue = CommonDefine.QUEUE_AGAIN;

			//Reset last-interval to reduce next review
			wordProgress.last_ivl = 0;

		} else {
			wordProgress.queue = CommonDefine.QUEUE_REVIEW;
			wordProgress.due = current + nextIvl;
			wordProgress.last_ivl =  nextIntervalByDaysWithEaseOption(wordProgress, easeOption);
		}
	}
}
