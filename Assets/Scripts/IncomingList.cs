using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using Firebase.Database;

public class IncomingList : MonoBehaviour
{


    public GameObject item;
    public GameObject list;
    public ScrollRect scroll;

    // Use this for initialization
    void Start()
    {
      
        list.SetActive(false);
        populateListWord();
    }

    private void populateListWord()
    {
        Firebase.Database.FirebaseDatabase.DefaultInstance.GetReference("public/dictionary").LimitToFirst(10).GetValueAsync().ContinueWith(task =>
        {

            if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                List<object> wordList = snapshot.Value as List<object>;
                foreach (var word in wordList)
                {
                    if (!(word is Dictionary<string, object>))
                        continue;

                    string question = ((Dictionary<string, object>)word)["question"].ToString();
                    GameObject obj = (GameObject)Instantiate(item, list.transform);
                    ItemWorkScripts itemWordButton = obj.GetComponent<ItemWorkScripts>();
                    itemWordButton.lbQuestion.text = question;
                    itemWordButton.lbAnswer.text = "answer";

                    item.transform.localScale = new Vector3(1, 1, 1);
                }
                list.SetActive(true);

                Canvas.ForceUpdateCanvases();
                scroll.verticalNormalizedPosition = 1f;
                Canvas.ForceUpdateCanvases();
            }
        });
        scroll.verticalNormalizedPosition = 1;
    }
}
