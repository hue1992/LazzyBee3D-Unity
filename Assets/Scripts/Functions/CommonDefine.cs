using System.Collections;
using System.Collections.Generic;


public class CommonDefine {
	public const int QUEUE_UNKNOWN 		= 0;
	public const int QUEUE_AGAIN 		= 1;
	public const int QUEUE_REVIEW 		= 2;
	public const int QUEUE_NEW_WORD 	= 3;
	public const int QUEUE_SUSPENDED	= -1;	//ignore
	public const int QUEUE_DONE 		= -2;	//learned

	public const int OPTION_AGAIN	= 0;	//choose to learn a word again
	public const int OPTION_HARD 	= 1;
	public const int OPTION_GOOD 	= 2;
	public const int OPTION_EASY 	= 3;

	public const int SECONDS_PERDAY 	= 86400;
	public const float BONUS_EASY 		= 1.4f;
	public const int MIN_FACTOR 		= 1300;
	public const int FORGET_FINE 		= 300;
	public const int DEFAULT_EFACTOR	= 2500;

	public const string STR_TIME_LEARN_AGAIN = "10min";
	public const int TIME_LEARN_AGAINE 	= 600;	//10 mins

	//default settings
	public const int SETTINGS_DEFAULT_LEVEL 	= 1;
	public const int SETTINGS_NEWCARD_A_DAY 	= 5;
	public const int SETTINGS_TOTALCARD_A_DAY 	= 40;
	public const int SETTINGS_TIME_SHOW_ANSWER 	= 3;
	public const int SETTINGS_AUTO_PLAY 		= 1;
	public const int SETTINGS_NOTIFICATION	 	= 1;
}
