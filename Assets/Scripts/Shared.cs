
public class Shared 
{

    public static string PUBLIC = "public/";
    public static string PRIVATE = "private/";
    public static string USER = "user/";
    public static string DICTIONARY = "dictionary/";
    public static string SETTING = "setting/";
    public static string REVIEW="review";

    //Default value
    public static string DEFAULT_LEVEL = "2";
    public static string DEFAULT_SUBJECT = "common";
    public static string DEFAULT_NEW_CARD_PER_DAY = "10";
    public static string DEFAULT_TOTAL_CARD_PER_DAY = "40";
    public static string DEFAULT_SPEED_TEXT_TO_SPEED = "1f";
    public static string DEFAULT_TIME_TO_SHOW_ANSWER = "3";
    public static bool DEFAULT_AUTO_PLAY_SOUND = true;
    public static bool DEFAULT_NOTIFICATION = true;
    public static string DEFAULT_TIME_SHOW_NOTIFICATION = "13h30";
    

    public static string MySettingRef(string uuid)
    {
        return PRIVATE + USER + uuid + "/" + SETTING;
    }

}
