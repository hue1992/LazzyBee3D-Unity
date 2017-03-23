using UnityEngine;
using UnityEngine.UI;
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
    public string htmlText;

    void Start()
    {
        handlerButton();
        getUserData();

        LoadFromText();        
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
        //FirebaseDatabase.DefaultInstance.GetReference("private/user/").GetValueAsync().ContinueWith(task =>
        //{

        //    if (task.IsCompleted)
        //    {
        //        DataSnapshot snapshot = task.Result;
        //        List<object> wordList = snapshot.Value as List<object>;
        //        DebugOnScreen.Log("Size:" + wordList.Capacity);
        //        //foreach (var word in wordList)
        //        //{
        //        //    if (!(word is Dictionary<string, object>))
        //        //        continue;


        //        //}

        //    }
        //});

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

    public void LoadFromText()
    {
        var webView = CreateWebView();

        webView.LoadHTMLString(htmlText, null);
        webView.backButtonEnable = false;      
        webView.insets.top = 200;
        webView.insets.bottom = 180;
        webView.Show();
    }
}
