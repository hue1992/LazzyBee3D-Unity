public class Setting 
{
    public string my_level;
    public string my_subject;
    public string new_card_per_day;
    public string total_card_per_day;
    public string speed_text_to_speed;
    public string time_to_show_answer;
    public bool auto_play_sound;
    public bool notification;
    public string time_show_notification;

    public Setting()
    {

    }

    public Setting(string my_level, string my_subject, string new_card_per_day, string total_card_per_day, string speed_text_to_speed, string time_to_show_answer, bool auto_play_sound, bool notification, string time_show_notification)
    {
        this.my_level = my_level;
        this.my_subject = my_subject;
        this.new_card_per_day = new_card_per_day;
        this.total_card_per_day = total_card_per_day;
        this.speed_text_to_speed = speed_text_to_speed;
        this.time_to_show_answer = time_to_show_answer;
        this.auto_play_sound = auto_play_sound;
        this.notification = notification;
        this.time_show_notification = time_show_notification;
    }



    public static Setting DefaultSetting()
    {
        return new Setting(Shared.DEFAULT_LEVEL, Shared.DEFAULT_SUBJECT, Shared.DEFAULT_NEW_CARD_PER_DAY, Shared.DEFAULT_TOTAL_CARD_PER_DAY, Shared.DEFAULT_SPEED_TEXT_TO_SPEED, Shared.DEFAULT_TIME_TO_SHOW_ANSWER, Shared.DEFAULT_AUTO_PLAY_SOUND, Shared.DEFAULT_NOTIFICATION, Shared.DEFAULT_TIME_SHOW_NOTIFICATION);
    }

   

}
