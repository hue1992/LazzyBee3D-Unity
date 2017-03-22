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

    void Start()
    {
        handlerButton();
        getUserData();          
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
}
