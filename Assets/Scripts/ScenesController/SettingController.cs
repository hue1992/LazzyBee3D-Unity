using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SettingController : MonoBehaviour,IScenesController
{

    public Button btnMy_level;
    public Button btnLanguage;
    public Button btnTotal_card_pre_day;
    public Button btnMax_new_card_per_day;
    public Button btnDisplay_meaning;
    public Button btnAuto_play_sound;

    public Text txtMy_level;
    public Text txtTotal_card_pre_day;
    public Text txtMax_new_card_per_day;
    public Text txtAuto_play_sound;
    public Text txtDisplay_meaning;

    public Text txtLanguage;
    public Text txtAbout;


    // Use this for initialization
    void Start()
    {
        DebugOnScreen.Log("Test");
        int my_level = TemporarilyStatus.getInstance().my_level;
        int new_card_a_day = TemporarilyStatus.getInstance().new_card_a_day;
        int total_card_a_day = TemporarilyStatus.getInstance().total_card_a_day;
        int time_to_show_answer = TemporarilyStatus.getInstance().time_to_show_answer;
        int auto_play_sound = TemporarilyStatus.getInstance().auto_play_sound;
        int notification = TemporarilyStatus.getInstance().notification;
        float speaking_speed = TemporarilyStatus.getInstance().speaking_speed;

        txtMy_level.text = my_level.ToString();
        txtTotal_card_pre_day.text = total_card_a_day.ToString();
        txtMax_new_card_per_day.text = new_card_a_day.ToString();
        txtAuto_play_sound.text = auto_play_sound == 0 ? "ON" : "OFF";
        txtDisplay_meaning.text = auto_play_sound == 0 ? "ON" : "OFF";

        //get user setting
        FirebaseHelper.getInstance().getUserSettings(handlerGetUserSetting);
    }

    private void handlerGetUserSetting(bool obj)
    {
        DebugOnScreen.Log("get User setting:" + obj);
        if (obj)
        {
            int my_level = TemporarilyStatus.getInstance().my_level;
            int new_card_a_day = TemporarilyStatus.getInstance().new_card_a_day;
            int total_card_a_day = TemporarilyStatus.getInstance().total_card_a_day;
            int time_to_show_answer = TemporarilyStatus.getInstance().time_to_show_answer;
            int auto_play_sound = TemporarilyStatus.getInstance().auto_play_sound;
            int notification = TemporarilyStatus.getInstance().notification;
            float speaking_speed = TemporarilyStatus.getInstance().speaking_speed;
            DebugOnScreen.Log("My level:" + my_level);

            txtMy_level.text = my_level.ToString();
            txtTotal_card_pre_day.text = total_card_a_day.ToString();
            txtMax_new_card_per_day.text = new_card_a_day.ToString();
            txtAuto_play_sound.text = auto_play_sound == 0 ? "ON" : "OFF";
            txtDisplay_meaning.text = auto_play_sound == 0 ? "ON" : "OFF";
        }
        else
        {
            DebugOnScreen.Log("get User setting failse");
        }
    }

    public void OnBackButtonClick()
    {
        SceneManager.LoadScene("Home", LoadSceneMode.Additive);
    }

   
}
