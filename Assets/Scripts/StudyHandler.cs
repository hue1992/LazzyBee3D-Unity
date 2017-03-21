using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;
using Firebase.Database;
using System.Collections.Generic;

public class StudyHandler : MonoBehaviour
{

    public Button btnShowAnswer;

    public GameObject ButtonAnswer;
    public Button btnAgain;
    public Button btnHard;
    public Button btnNormal;
    public Button btnEasy;


    string Url;
    GUIText status;
    public string htmlText;
#if UNITY_IOS || UNITY_ANDROID || UNITY_WP8
    // Use this for initialization
    void Start()
    {
        handlerButton();

        getUserData();

        var webView = CreateWebView();

        webView.LoadHTMLString(htmlText, null);
        webView.Show();
    }

    UniWebView CreateWebView()
    {
        var webViewGameObject = GameObject.Find("WebView");
        if (webViewGameObject == null)
        {
            webViewGameObject = new GameObject("WebView");
        }

        var webView = webViewGameObject.AddComponent<UniWebView>();

        webView.toolBarShow = true;
        return webView;
    }


    private void handlerButton()
    {
        btnShowAnswer.onClick.AddListener(ShowAnswer);
        btnAgain.onClick.AddListener(AgainClick);
        btnHard.onClick.AddListener(HardClick);
        btnNormal.onClick.AddListener(NormalClick);
        btnEasy.onClick.AddListener(EasyClick);
    }

    void ShowAnswer()
    {
        btnShowAnswer.interactable = false;
        ButtonAnswer.SetActive(true);

    }

    void AgainClick()
    {
        endSessionStudy();
    }
    void HardClick()
    {
        endSessionStudy();
    }
    void NormalClick()
    {
        endSessionStudy();
    }
    void EasyClick()
    {
        endSessionStudy();
    }

    void endSessionStudy()
    {
        btnShowAnswer.interactable = true;
        ButtonAnswer.SetActive(false);
    }


    private void getUserData()
    {
        Firebase.Database.FirebaseDatabase.DefaultInstance.GetReference("private/user/").GetValueAsync().ContinueWith(task =>
        {

            if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                List<object> wordList = snapshot.Value as List<object>;
                DebugOnScreen.Log("Size:" + wordList.Capacity);
                //foreach (var word in wordList)
                //{
                //    if (!(word is Dictionary<string, object>))
                //        continue;


                //}

            }
        });

    }
#else //End of #if UNITY_IOS || UNITY_ANDROID || UNITY_WP8
    void Start() {
        Debug.LogWarning("UniWebView only works on iOS/Android/WP8. Please switch to these platforms in Build Settings.");
    }
#endif
}
