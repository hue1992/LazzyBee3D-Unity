using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DateTimeHelper {

	public static int convertDateTimeToSec (System.DateTime dateTime) {
		System.DateTime epochStart = new System.DateTime(1970, 1, 1, 0, 0, 0, System.DateTimeKind.Utc);

		int dateTimeInSec = (int)(dateTime - epochStart).TotalSeconds;

		return dateTimeInSec;
	}

	public static int getCurrentDateTimeInSeconds () {
		System.DateTime epochStart = new System.DateTime(1970, 1, 1, 0, 0, 0, System.DateTimeKind.Utc);
		int cur_time = (int)(System.DateTime.UtcNow - epochStart).TotalSeconds;

		return cur_time;
	}

	public static int getBeginOfDayInSec () {
		System.DateTime curDate = System.DateTime.Now;
		Debug.Log("curDate :: " + convertDateTimeToSec(curDate).ToString());
		System.DateTime beginOfDate = new System.DateTime(curDate.Year, curDate.Month, curDate.Day, 0, 0, 0, System.DateTimeKind.Utc);

		Debug.Log("beginOfDate :: " + convertDateTimeToSec(beginOfDate).ToString());

		return convertDateTimeToSec(beginOfDate);
	}

	public static int getEndOfDayInSec () {
		return (getBeginOfDayInSec() + 24*3600);
	}

	public static string getDayOfWeek(int date) {
		System.DateTime start 		= new System.DateTime(1970, 1, 1, 0, 0, 0, System.DateTimeKind.Utc);
		System.DateTime dateTime	= start.AddMilliseconds(date).ToLocalTime();

		string res = dateTime.DayOfWeek.ToString();

		if (res != null && res.Length > 0) {
			res = res.Substring(0, 3);

		} else {
			res = "";
		}

		return res;
	}
}
